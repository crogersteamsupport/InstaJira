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
    class ReportTablePage
    {
        public static GridResult GetReportData(LoginUser loginUser, int reportID, int from, int to, string sortField, bool isDesc, bool useUserFilter)
        {
            Report report = Reports.GetReport(loginUser, reportID, loginUser.UserID);

            if (report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;
            GridResult result;
            try
            {
                    result = GetReportDataPage(loginUser, report, from, to, sortField, isDesc, useUserFilter);
            }
            catch (Exception)
            {
                // try without the sort
                try
                {
                        result = GetReportDataPage(loginUser, report, from, to, null, isDesc, useUserFilter);
                }
                catch (Exception)
                {
                    // try without the user filters
                        result = GetReportDataPage(loginUser, report, from, to, null, isDesc, false);

                    UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
                    userFilters.Filters = null;
                    report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
                    report.Collection.Save();
                }
            }
            report.LastTimeTaken = (int)(DateTime.Now - timeStart).TotalSeconds;
            report.Collection.Save();
            return result;

        }

        public static DataTable GetReportTable(LoginUser loginUser, int reportID, int from, int to, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            Report report = Reports.GetReport(loginUser, reportID);
            if (report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;
            DataTable result = null;
            try
            {
                    result = GetReportTablePage(loginUser, report, from, to, sortField, isDesc, useUserFilter, includeHiddenFields);
            }
            catch (Exception)
            {
                // try without the sort
                try
                {
                        result = GetReportTablePage(loginUser, report, from, to, null, isDesc, useUserFilter, includeHiddenFields);
                }
                catch (Exception)
                {
                    // try without the user filters

                        result = GetReportTablePage(loginUser, report, from, to, null, isDesc, false, includeHiddenFields);

                    UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
                    userFilters.Filters = null;
                    report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
                    report.Collection.Save();
                }
            }
            report.LastTimeTaken = (int)(DateTime.Now - timeStart).TotalSeconds;
            report.Collection.Save();


            return result;
        }

        //For exports
        public static DataTable GetReportTableForExports(LoginUser loginUser, int reportID, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            Report report = Reports.GetReport(loginUser, reportID);
            if (report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;
            DataTable result = null;
            try
            {
                    result = GetReportTablePageForExports(loginUser, report, sortField, isDesc, useUserFilter, includeHiddenFields);
            }
            catch (Exception)
            {
                // try without the sort
                try
                {
                        result = GetReportTablePageForExports(loginUser, report, null, isDesc, useUserFilter, includeHiddenFields);
                }
                catch (Exception)
                {
                    // try without the user filters
                        result = GetReportTablePageForExports(loginUser, report, null, isDesc, false, includeHiddenFields);

                    UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
                    userFilters.Filters = null;
                    report.Row["Settings"] = JsonConvert.SerializeObject(userFilters);
                    report.Collection.Save();
                }
            }
            report.LastTimeTaken = (int)(DateTime.Now - timeStart).TotalSeconds;
            report.Collection.Save();


            return result;
        }
        public static GridResult GetReportDataPage(LoginUser loginUser, Report report, int from, int to, string sortField, bool isDesc, bool useUserFilter)
        {
            DataTable table = GetReportTablePage(loginUser, report, from, to, sortField, isDesc, useUserFilter, true);
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

        public static DataTable GetReportTablePage(LoginUser loginUser, Report report, int from, int to, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            from++;
            to++;

            SqlCommand command = new SqlCommand();

            string query = @"WITH  q AS({0}) SELECT * FROM q WHERE RowNum BETWEEN @From AND @To ORDER BY RowNum ASC";

            /*WITH 
            q AS ({0}),
            r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY [{1}] {2}) AS 'RowNum' FROM q)
            SELECT  *{3} FROM r
            WHERE RowNum BETWEEN @From AND @To";
            */

            if (report.ReportDefType == ReportType.Custom)
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
                sortField = Reports.GetReportColumnNames(loginUser, report.ReportID)[0];
                isDesc = false;
            }

            if (includeHiddenFields && report.ReportSubcategoryID == 70)
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

            if (report.ReportDefType != ReportType.Custom)
            {
                report.GetCommand(command, includeHiddenFields, false, useUserFilter, sortField, isDesc ? "DESC" : "ASC");
                command.CommandText = string.Format(query, command.CommandText);
            }
            else
            {
                report.GetCommand(command, includeHiddenFields, false, useUserFilter);
                command.CommandText = string.Format(query, command.CommandText, sortField, isDesc ? "DESC" : "ASC");
            }

            report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            report.Collection.Save();
            BaseCollection.FixCommandParameters(command);

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
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
                    ExceptionLogs.LogException(loginUser, ex, "Report Data", DataUtils.GetCommandTextSql(command));
                    throw;
                }
                connection.Close();
            }

            if (!includeHiddenFields) table.Columns.Remove("RowNum");

            return table;
        }

        //For Exports
        public static DataTable GetReportTablePageForExports(LoginUser loginUser, Report report, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {

            SqlCommand command = new SqlCommand();
            string query = string.Empty;

            if (string.IsNullOrWhiteSpace(sortField))
            {
                sortField = Reports.GetReportColumnNames(loginUser, report.ReportID)[0];
                isDesc = false;
            }

            if (includeHiddenFields && report.ReportSubcategoryID == 70)
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

            if (report.ReportDefType != ReportType.Custom)
            {
                report.GetCommandForExports(command, includeHiddenFields, false, useUserFilter, sortField, isDesc ? "DESC" : "ASC");
            }
            else
            {
                report.GetCommandForExports(command, includeHiddenFields, false, useUserFilter);
            }

            report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            report.Collection.Save();
            BaseCollection.FixCommandParameters(command);

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
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
                    ExceptionLogs.LogException(loginUser, ex, "Report Data", DataUtils.GetCommandTextSql(command));
                    throw;
                }
                connection.Close();
            }

            //On exports it is always false
            //if (!includeHiddenFields) 
            if (table.Columns.Contains("RowNum"))
                table.Columns.Remove("RowNum");

            return table;
        }

    }
}
