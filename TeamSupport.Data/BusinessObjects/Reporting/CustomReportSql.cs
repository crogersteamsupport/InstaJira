using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace TeamSupport.Data.BusinessObjects.Reporting
{
    public class CustomReportSql
    {
        public static void GetCustomSql(LoginUser loginUser, SqlCommand command, bool isSchemaOnly, bool useUserFilter, Report report)
        {
            if (isSchemaOnly)
            {
                command.CommandText = string.Format("WITH q AS ({0}) SELECT * FROM q WHERE (0=1)", report.Query);
                return;
            }

            report = Reports.GetReport(loginUser, report.ReportID, loginUser.UserID);
            if (report != null && report.Row["Settings"] != DBNull.Value)
            {
                try
                {
                    UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
                    StringBuilder builder = new StringBuilder();
                    if (userFilters != null && userFilters.Filters != null && userFilters.Filters.Length > 0)
                    {
                        Report.GetWhereClause(loginUser, command, builder, userFilters.Filters);
                        builder.Remove(0, 4);
                        command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c WHERE {1})", report.Query, builder.ToString());
                    }
                    else
                    {
                        command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c)", report.Query);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(loginUser, ex, "Tabular SQL - User filters");
                    throw;
                }
            }
            else
            {
                command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c)", report.Query);

            }


        }

        public static void GetCustomSqlForExport(LoginUser loginUser, SqlCommand command, bool useUserFilter, Report report)
        {
            //if (isSchemaOnly)
            //{
            //    command.CommandText = string.Format("WITH q AS ({0}) SELECT * FROM q WHERE (0=1)", Query);
            //    return;
            //}

            report = Reports.GetReport(loginUser, report.ReportID, loginUser.UserID);
            if (report != null && report.Row["Settings"] != DBNull.Value)
            {
                try
                {
                    UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
                    StringBuilder builder = new StringBuilder();
                    if (userFilters != null && userFilters.Filters != null && userFilters.Filters.Length > 0)
                    {
                        Report.GetWhereClause(loginUser, command, builder, userFilters.Filters);
                        builder.Remove(0, 4);
                        //command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c WHERE {1})", Query, builder.ToString());
                        command.CommandText = string.Format("WITH c AS ({0}) SELECT * FROM c WHERE {1}", report.Query, builder.ToString());
                    }
                    else
                    {
                        //command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c)", Query);
                        command.CommandText = report.Query;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(loginUser, ex, "Tabular SQL - User filters");
                    throw;
                }
            }
            else
            {
                //command.CommandText = string.Format("c AS ({0}), q AS (SELECT * FROM c)", Query);
                command.CommandText = report.Query;

            }


        }

    }
}
