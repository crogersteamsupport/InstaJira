using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace TeamSupport.Data.BusinessObjects.Reporting
{
    class ReportSummaryAll
    {
        LoginUser _loginUser;
        Report _report;
        SummaryReport _summaryReport;
        ReportTicketsViewTempTable _reportTicketsView;

        public ReportSummaryAll(LoginUser loginUser, Report report)
        {
            _loginUser = loginUser;
            _report = report;
        }

        public GridResult GetReportData(string sortField, bool isDesc, bool useUserFilter)
        {
            if (_report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;

            GridResult result;
            try { result = GetReportDataAll(sortField, isDesc, useUserFilter); }
            catch (Exception) { result = null; }

            // try without the sort
            if (result == null)
            {
                try { result = GetReportDataAll(null, isDesc, useUserFilter); }
                catch (Exception) { result = null; }
            }

            // try without the user filters
            if (result == null)
            {
                result = GetReportDataAll(null, isDesc, false);

                UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)_report.Row["Settings"]);
                userFilters.Filters = null;
                _report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
                _report.Collection.Save();
            }

            _report.LastTimeTaken = (int)(DateTime.Now - timeStart).TotalSeconds;
            _report.Collection.Save();
            return result;
        }

        public GridResult GetReportDataAll(string sortField, bool isDesc, bool useUserFilter)
        {
            DataTable table = GetReportTableAll(sortField, isDesc, useUserFilter, true);

            GridResult result = new GridResult();
            result.From = 0;
            result.To = table.Rows.Count - 1;
            result.Total = table.Rows.Count;
            result.Data = table;
            return result;
        }

        public DataTable GetReportTable(string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            if (_report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;

            DataTable result = null;
            try { result = GetReportTableAll(sortField, isDesc, useUserFilter, includeHiddenFields); }
            catch (Exception) { result = null; }

            // try without the sort
            if (result == null)
            {
                try { result = GetReportTableAll(null, isDesc, useUserFilter, includeHiddenFields); }
                catch (Exception) { result = null; }
            }

            // try without the user filters
            if (result == null)
            {
                result = GetReportTableAll(null, isDesc, false, includeHiddenFields);

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
            try { result = GetReportTableAllForExports(sortField, isDesc, useUserFilter, includeHiddenFields); }
            catch (Exception) { result = null; }

            // try without the sort
            if (result == null)
            {
                try { result = GetReportTableAllForExports(null, isDesc, useUserFilter, includeHiddenFields); }
                catch (Exception) { result = null; }
            }

            // try without the user filters
            if (result == null)
            {
                result = GetReportTableAllForExports(null, isDesc, false, includeHiddenFields);

                UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)_report.Row["Settings"]);
                userFilters.Filters = null;
                _report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
                _report.Collection.Save();
            }

            _report.LastTimeTaken = (int)(DateTime.Now - timeStart).TotalSeconds;
            _report.Collection.Save();
            return result;
        }

        public DataTable GetReportTableAll(string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = GetSqlCommand(ref sortField, ref isDesc, useUserFilter, includeHiddenFields);

            _report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            _report.Collection.Save();
            BaseCollection.FixCommandParameters(command);

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
            {
                connection.Open();
                command.Connection = connection;
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        adapter.Fill(table);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(_loginUser, ex, "Report Data");
                        throw;
                    }
                }
                connection.Close();
            }
            return table;
        }

        private SqlCommand GetSqlCommand(ref string sortField, ref bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = new SqlCommand();

            _report.GetCommand(command, includeHiddenFields, false, useUserFilter);
            if (command.CommandText.ToLower().IndexOf(" order by ") < 0)
            {
                if (string.IsNullOrWhiteSpace(sortField))
                {
                    sortField = Reports.GetReportColumnNames(_loginUser, _report.ReportID)[0];
                    isDesc = false;
                }
                command.CommandText = command.CommandText + " ORDER BY [" + sortField + (isDesc ? "] DESC" : "] ASC");
            }

            AddReportTicketsViewTempTable(command);

            return command;
        }

        public DataTable GetReportTableAllForExports(string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = GetExportsSqlCommand(ref sortField, ref isDesc, useUserFilter, includeHiddenFields);

            _report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            _report.Collection.Save();
            BaseCollection.FixCommandParameters(command);

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(_loginUser.ConnectionString))
            {
                connection.Open();
                command.Connection = connection;
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        adapter.Fill(table);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogs.LogException(_loginUser, ex, "Report Data");
                        throw;
                    }
                }
                connection.Close();
            }
            return table;
        }

        private SqlCommand GetExportsSqlCommand(ref string sortField, ref bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = new SqlCommand();

            _report.GetCommandForExports(command, includeHiddenFields, false, useUserFilter);
            if (command.CommandText.ToLower().IndexOf(" order by ") < 0)
            {
                if (string.IsNullOrWhiteSpace(sortField))
                {
                    sortField = Reports.GetReportColumnNames(_loginUser, _report.ReportID)[0];
                    isDesc = false;
                }
                command.CommandText = command.CommandText + " ORDER BY [" + sortField + (isDesc ? "] DESC" : "] ASC");
            }

            AddReportTicketsViewTempTable(command);

            return command;
        }

        private void AddReportTicketsViewTempTable(SqlCommand command)
        {
            if (ReportTicketsViewTempTable.Enable && (_report.ReportDefType != ReportType.Custom))
            {
                _summaryReport = JsonConvert.DeserializeObject<SummaryReport>(_report.ReportDef);
                _reportTicketsView = new ReportTicketsViewTempTable(_report.Collection.LoginUser, _summaryReport);
                string tempTable = _reportTicketsView.ToSql();
                if (!String.IsNullOrEmpty(tempTable))
                    command.CommandText = (tempTable + command.CommandText).Replace("ReportTicketsView", "#ReportTicketsView");
            }
        }

    }

}
