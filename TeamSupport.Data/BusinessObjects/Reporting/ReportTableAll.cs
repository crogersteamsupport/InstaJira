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
    class ReportTableAll
    {
        public static GridResult GetReportData(LoginUser loginUser, int reportID, int from, int to, string sortField, bool isDesc, bool useUserFilter)
        {
            Report report = Reports.GetReport(loginUser, reportID, loginUser.UserID);

            if (report.IsDisabled || SystemSettings.GetIsReportsDisabled()) return null;
            DateTime timeStart = DateTime.Now;
            GridResult result;
            try
            {
                    result = GetReportDataAll(loginUser, report, sortField, isDesc, useUserFilter);
            }
            catch (Exception)
            {
                // try without the sort
                try
                {
                        result = GetReportDataAll(loginUser, report, null, isDesc, useUserFilter);
                }
                catch (Exception)
                {
                    // try without the user filters
                        result = GetReportDataAll(loginUser, report, null, isDesc, false);

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

        public static GridResult GetReportDataAll(LoginUser loginUser, Report report, string sortField, bool isDesc, bool useUserFilter)
        {
            DataTable table = GetReportTableAll(loginUser, report, sortField, isDesc, useUserFilter, true);

            GridResult result = new GridResult();
            result.From = 0;
            result.To = table.Rows.Count - 1;
            result.Total = table.Rows.Count;
            result.Data = table;
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
                    result = GetReportTableAll(loginUser, report, sortField, isDesc, useUserFilter, includeHiddenFields);
            }
            catch (Exception)
            {
                // try without the sort
                try
                {
                        result = GetReportTableAll(loginUser, report, null, isDesc, useUserFilter, includeHiddenFields);
                }
                catch (Exception)
                {
                    // try without the user filters
                        result = GetReportTableAll(loginUser, report, null, isDesc, false, includeHiddenFields);

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
                    result = GetReportTableAllForExports(loginUser, report, sortField, isDesc, useUserFilter, includeHiddenFields);
            }
            catch (Exception)
            {
                // try without the sort
                try
                {
                        result = GetReportTableAllForExports(loginUser, report, null, isDesc, useUserFilter, includeHiddenFields);
                }
                catch (Exception)
                {
                    // try without the user filters
                        result = GetReportTableAllForExports(loginUser, report, null, isDesc, false, includeHiddenFields);

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

        public static DataTable GetReportTableAll(LoginUser loginUser, Report report, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = new SqlCommand();

            report.GetCommand(command, includeHiddenFields, false, useUserFilter);
            if (command.CommandText.ToLower().IndexOf(" order by ") < 0)
            {
                if (string.IsNullOrWhiteSpace(sortField))
                {
                    sortField = Reports.GetReportColumnNames(loginUser, report.ReportID)[0];
                    isDesc = false;
                }
                command.CommandText = command.CommandText + " ORDER BY [" + sortField + (isDesc ? "] DESC" : "] ASC");
            }

            report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            report.Collection.Save();
            BaseCollection.FixCommandParameters(command);

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
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
                        ExceptionLogs.LogException(loginUser, ex, "Report Data");
                        throw;
                    }
                }
                connection.Close();
            }
            return table;
        }

        public static DataTable GetReportTableAllForExports(LoginUser loginUser, Report report, string sortField, bool isDesc, bool useUserFilter, bool includeHiddenFields)
        {
            SqlCommand command = new SqlCommand();

            report.GetCommandForExports(command, includeHiddenFields, false, useUserFilter);
            if (command.CommandText.ToLower().IndexOf(" order by ") < 0)
            {
                if (string.IsNullOrWhiteSpace(sortField))
                {
                    sortField = Reports.GetReportColumnNames(loginUser, report.ReportID)[0];
                    isDesc = false;
                }
                command.CommandText = command.CommandText + " ORDER BY [" + sortField + (isDesc ? "] DESC" : "] ASC");
            }

            report.LastSqlExecuted = DataUtils.GetCommandTextSql(command);
            report.Collection.Save();
            BaseCollection.FixCommandParameters(command);

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
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
                        ExceptionLogs.LogException(loginUser, ex, "Report Data");
                        throw;
                    }
                }
                connection.Close();
            }
            return table;
        }

    }
}
