using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Data.BusinessObjects.Reporting
{
    public class ReportTablePage
    {
        LoginUser _loginUser;
        Report _report;
        TabularReport _tabularReport;
        TabularReportSql _tabularReportSql;
        ReportTicketsViewTempTable _reportTicketsView;

        public ReportTablePage(LoginUser loginUser, Report report)
        {
            _loginUser = loginUser;
            _report = report;
            _tabularReportSql = new TabularReportSql(report.Collection.LoginUser); ;
        }

        public GridResult GetReportData(int from, int to, string sortField, bool isDesc, bool useUserFilter)
        {
            if (_report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;

            GridResult result;
            try { result = GetReportDataPage(from, to, sortField, isDesc, useUserFilter); }
            catch (Exception) { result = null; }

            // try without the sort
            if (result == null)
            {
                try { result = GetReportDataPage(from, to, null, isDesc, useUserFilter); }
                catch (Exception) { result = null; }
            }

            // try without the user filters
            if (result == null)
            {
                result = GetReportDataPage(from, to, null, isDesc, false);

                UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)_report.Row["Settings"]);
                userFilters.Filters = null;
                _report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
                _report.Collection.Save();
            }

            _report.LastTimeTaken = (int)(DateTime.Now - timeStart).TotalSeconds;
            _report.Collection.Save();
            return result;
        }

        public DataTable GetReportTable(int from, int to, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            if (_report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;

            DataTable result = null;
            try { result = GetReportTablePage(from, to, sortField, isDesc, useUserFilter, includeHiddenFields); }
            catch (Exception) { result = null; }

            // try without the sort
            if (result == null)
            {
                try { result = GetReportTablePage(from, to, null, isDesc, useUserFilter, includeHiddenFields); }
                catch (Exception) { result = null; }
            }

            // try without the user filters
            if (result == null)
            {
                result = GetReportTablePage(from, to, null, isDesc, false, includeHiddenFields);

                UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)_report.Row["Settings"]);
                userFilters.Filters = null;
                _report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
                _report.Collection.Save();
            }

            _report.LastTimeTaken = (int)(DateTime.Now - timeStart).TotalSeconds;
            _report.Collection.Save();
            return result;
        }

        //For exports
        public DataTable GetReportTableForExports(string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            if (_report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;

            DataTable result = null;
            try { result = GetReportTablePageForExports(sortField, isDesc, useUserFilter, includeHiddenFields); }
            catch (Exception) { result = null; }

            // try without the sort
            if (result == null)
            {
                try { result = GetReportTablePageForExports(null, isDesc, useUserFilter, includeHiddenFields); }
                catch (Exception) { result = null; }
            }

            // try without the user filters
            if (result == null)
            {
                result = GetReportTablePageForExports(null, isDesc, false, includeHiddenFields);

                UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)_report.Row["Settings"]);
                userFilters.Filters = null;
                _report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
                _report.Collection.Save();
            }

            _report.LastTimeTaken = (int)(DateTime.Now - timeStart).TotalSeconds;
            _report.Collection.Save();
            return result;
        }

        private GridResult GetReportDataPage(int from, int to, string sortField, bool isDesc, bool useUserFilter)
        {
            DataTable table = GetReportTablePage(from, to, sortField, isDesc, useUserFilter, true);
            GridResult result = new GridResult();
            result.From = from;
            result.To = to;
            int page = to - from + 1;
            if (table.Rows.Count < 1)
            {
                //0 rows or exact match
                result.Total = from > 0 ? from : 0;
            }
            else if (table.Rows.Count == page)
            {
                //page is full, add some padding
                result.Total = to + 100;
            }
            else
            {
                //page is not full, so set the proper total
                result.Total = from + table.Rows.Count;
            }
            result.Data = table;
            return result;
        }

        private DataTable GetReportTablePage(int from, int to, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            from++;
            to++;

            SqlCommand command = GetSqlCommand(from, to, ref sortField, ref isDesc, useUserFilter, includeHiddenFields);

            _report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            _report.Collection.Save();
            BaseCollection.FixCommandParameters(command);

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                    transaction.Commit();
                    table = DataUtils.DecodeDataTable(table);
                    table = DataUtils.StripHtmlDataTable(table);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionLogs.LogException(_loginUser, ex, "Report Data", DataUtils.GetCommandTextSql(command));
                    throw;
                }
                connection.Close();
            }

            if (!includeHiddenFields) table.Columns.Remove("RowNum");

            return table;
        }

        private SqlCommand GetSqlCommand(int from, int to, ref string sortField, ref bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = new SqlCommand();

            string query = @"WITH  q AS({0}) SELECT * FROM q WHERE RowNum BETWEEN @From AND @To ORDER BY RowNum ASC";

            /*WITH 
            q AS ({0}),
            r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) AS 'RowNum' FROM q)
            SELECT  *{3} FROM r
            WHERE RowNum BETWEEN @From AND @To";
            */

            if (_report.ReportDefType == ReportType.Custom)
            {
                query = @"
WITH 
{0}
,r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) AS 'RowNum' FROM q)
SELECT  * FROM r
WHERE RowNum BETWEEN @From AND @To";
            }

            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = Reports.GetReportColumnNames(_loginUser, _report.ReportID)[0];
                isDesc = false;
            }

            if (includeHiddenFields && _report.ReportSubcategoryID == 70)
            {
                switch (sortField)
                {
                    case "Severity":
                        sortField = "hiddenSeverityPosition";
                        break;
                    case "Status":
                        sortField = "hiddenStatusPosition";
                        break;
                }
            }


            command.Parameters.AddWithValue("@From", from);
            command.Parameters.AddWithValue("@To", to);

            if (_report.ReportDefType != ReportType.Custom)
            {
                GetCommand(command, includeHiddenFields, false, useUserFilter, sortField, isDesc ? "DESC" : "ASC");
                command.CommandText = string.Format(query, command.CommandText);
            }
            else
            {
                GetCommand(command, includeHiddenFields, false, useUserFilter);
                command.CommandText = string.Format(query, command.CommandText, sortField, isDesc ? "DESC" : "ASC");
            }

            AddReportTicketsViewTempTable(command);

            return command;
        }

        //For Exports
        private DataTable GetReportTablePageForExports(string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = GetExportsSqlCommand(ref sortField, ref isDesc, useUserFilter, includeHiddenFields);

            _report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            _report.Collection.Save();
            BaseCollection.FixCommandParameters(command);

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                    transaction.Commit();
                    table = DataUtils.DecodeDataTable(table);
                    table = DataUtils.StripHtmlDataTable(table);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionLogs.LogException(_loginUser, ex, "Report Data", DataUtils.GetCommandTextSql(command));
                    throw;
                }
                connection.Close();
            }

            //On exports it is always false
            //if (!includeHiddenFields) 
            if (table.Columns.Contains("RowNum"))
                table.Columns.Remove("RowNum");

            AddReportTicketsViewTempTable(command);

            return table;
        }

        private SqlCommand GetExportsSqlCommand(ref string sortField, ref bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = new SqlCommand();
            string query = string.Empty;

            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = Reports.GetReportColumnNames(_loginUser, _report.ReportID)[0];
                isDesc = false;
            }

            if (includeHiddenFields && _report.ReportSubcategoryID == 70)
            {
                switch (sortField)
                {
                    case "Severity":
                        sortField = "hiddenSeverityPosition";
                        break;
                    case "Status":
                        sortField = "hiddenStatusPosition";
                        break;
                }
            }

            if (_report.ReportDefType != ReportType.Custom)
            {
                _report.GetCommandForExports(command, includeHiddenFields, false, useUserFilter, sortField, isDesc ? "DESC" : "ASC");
            }
            else
            {
                _report.GetCommandForExports(command, includeHiddenFields, false, useUserFilter);
            }

            return command;
        }

        private void AddReportTicketsViewTempTable(SqlCommand command)
        {
            if (!_tabularReportSql.IsOrganizationID ||   // not Parent organizationID report
                (_report.ReportDefType == ReportType.Custom))   // not a custom report
                throw new Exception("optimization not supported on report type");

            if (!ReportTicketsViewTempTable.Enable)
                return;

            _tabularReport = JsonConvert.DeserializeObject<TabularReport>(_report.ReportDef);
            _reportTicketsView = new ReportTicketsViewTempTable(_report.Collection.LoginUser, _tabularReport);
            string tempTable = _reportTicketsView.ToSql();
            if (!String.IsNullOrEmpty(tempTable))
                command.CommandText = (tempTable + command.CommandText).Replace("ReportTicketsView", "#ReportTicketsView");
        }

        public void GetCommand(SqlCommand command, bool inlcudeHiddenFields = true, bool isSchemaOnly = false, bool useUserFilter = true, string sortField = null, string sortDir = null)
        {
            _report.MigrateToNewReport();

            command.CommandType = CommandType.Text;
            command.CommandTimeout = SystemSettings.GetReportTimeout();
            switch (_report.ReportDefType)
            {
                case ReportType.Table:
                    _tabularReportSql.GetTabularSql(command, JsonConvert.DeserializeObject<TabularReport>(_report.ReportDef), inlcudeHiddenFields, isSchemaOnly, _report.ReportID, useUserFilter, sortField, sortDir);
                    break;
                case ReportType.TicketView:
                    _tabularReportSql.GetTabularSql(command, JsonConvert.DeserializeObject<TabularReport>(_report.ReportDef), inlcudeHiddenFields, isSchemaOnly, _report.ReportID, useUserFilter, sortField, sortDir);
                    break;
                default:
                    break;
            }

            Report.AddCommandParametersForExport(command, _report.Collection.LoginUser);
            command.CommandText = $" /* ReportID: {_report.ReportID.ToString()} OrganizationID: {_report.OrganizationID.ToString()} */ " + command.CommandText;
        }

    }

}
