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
    public class SummaryReport
    {
        public SummaryReport() { }
        public int Subcategory { get; set; }
        public ReportSummaryFields Fields { get; set; }
        public ReportFilter[] Filters { get; set; }
    }

    public class ReportSummaryFields
    {
        public ReportSummaryFields() { }
        public ReportSummaryDescriptiveField[] Descriptive { get; set; }
        public ReportSummaryCalculatedField[] Calculated { get; set; }
    }

    public class ReportSummaryDescriptiveField
    {
        public ReportSummaryDescriptiveField() { }
        public SummarySelectedField Field { get; set; }
        public string Value1 { get; set; }
    }

    public class ReportSummaryCalculatedField
    {
        public ReportSummaryCalculatedField() { }
        public SummarySelectedField Field { get; set; }
        public string Aggregate { get; set; }
        public string Comparator { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
    }


    public class SummaryReportSql
    {
        public static void GetSummarySql(LoginUser loginUser, SqlCommand command, SummaryReport summaryReport, bool isSchemaOnly, int? reportID, bool useUserFilter, bool useDefaultOrderBy)
        {
            StringBuilder builder = new StringBuilder();
            ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(loginUser, summaryReport.Subcategory);
            ReportTables tables = new ReportTables(loginUser);
            tables.LoadAll();
            List<DescriptiveClauseItem> descFields = GetSummaryDescFields(loginUser, summaryReport);
            List<CalculatedClauseItem> calcFields = GetSummaryCalcFields(loginUser, summaryReport);

            builder.Append("WITH x AS (");
            bool flag = true;
            foreach (DescriptiveClauseItem descField in descFields)
            {
                if (flag)
                    builder.Append(string.Format(" SELECT {0} AS [{1}]", descField.Field, descField.Alias));
                else
                    builder.Append(string.Format(", {0} AS [{1}]", descField.Field, descField.Alias));
                flag = false;
            }

            foreach (CalculatedClauseItem calcField in calcFields)
            {
                builder.Append(string.Format(", {0} AS [{1}]", calcField.Field, calcField.Alias));
            }

            // from + where clause
            builder.Append(" " + sub.BaseQuery);
            ReportTable mainTable = tables.FindByReportTableID(sub.ReportCategoryTableID);
            builder.Append(" WHERE (" + mainTable.TableName + "." + mainTable.OrganizationIDFieldName + " = @OrganizationID)");
            //add user rights where needed
            UserRights.UseTicketRights(loginUser, (int)summaryReport.Subcategory, tables, command, builder);
            if (isSchemaOnly) builder.Append(" AND (0=1)");

            // filters
            if (!isSchemaOnly)
            {
                Report.GetWhereClause(loginUser, command, builder, summaryReport.Filters);
                if (useUserFilter == true && reportID != null)
                {
                    Report report = Reports.GetReport(loginUser, (int)reportID, loginUser.UserID);
                    if (report != null && report.Row["Settings"] != DBNull.Value)
                    {
                        try
                        {
                            UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);
                            if (userFilters != null)
                            {
                                Report.GetWhereClause(loginUser, command, builder, userFilters.Filters);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogs.LogException(loginUser, ex, "Summary SQL - User filters");
                        }
                    }
                }
            }
            flag = true;

            builder.Append(")"); // end with

            flag = true;
            foreach (DescriptiveClauseItem descField in descFields)
            {
                if (flag)
                    builder.Append(string.Format(" SELECT [{0}]", descField.Alias));
                else
                    builder.Append(string.Format(", [{0}]", descField.Alias));
                flag = false;
            }

            foreach (CalculatedClauseItem calcField in calcFields)
            {
                builder.Append(string.Format(", {0} AS [{1}]", calcField.AggField, calcField.Alias));
            }

            builder.Append(" FROM x ");

            // group by
            flag = true;
            foreach (DescriptiveClauseItem descField in descFields)
            {
                if (flag)
                    builder.Append(string.Format(" GROUP BY [{0}]", descField.Alias));
                else
                    builder.Append(string.Format(", [{0}]", descField.Alias));

                flag = false;
            }

            // having
            flag = true;
            foreach (CalculatedClauseItem calcField in calcFields)
            {
                if (calcField.Comparator == null) continue;
                if (flag)
                    builder.Append(string.Format(" HAVING {0}", calcField.Comparator));
                else
                    builder.Append(string.Format(" AND {0}", calcField.Comparator));
                flag = false;
            }

            if (useDefaultOrderBy)
            {
                // order by
                /* flag = true;
                 foreach (DescriptiveClauseItem descField in descFields)
                 {
                   if (flag)
                     builder.Append(string.Format(" ORDER BY [{0}]", descField.Alias));
                   else
                     builder.Append(string.Format(", [{0}]", descField.Alias));

                   flag = false;
                 }*/

                // order by
                for (int i = descFields.Count - 1; i > -1; i--)
                {
                    if (i == descFields.Count - 1)
                        builder.Append(string.Format(" ORDER BY [{0}]", descFields[i].Alias));
                    else
                        builder.Append(string.Format(", [{0}]", descFields[i].Alias));
                }
            }
            command.CommandText = builder.ToString();
        }

        public static void GetSummaryCommand(LoginUser loginUser, SqlCommand command, SummaryReport summaryReport, bool isSchemaOnly, bool useUserFilter, bool useDefaultOrderBy)
        {
            command.CommandType = CommandType.Text;
            GetSummarySql(loginUser, command, summaryReport, isSchemaOnly, null, useUserFilter, useDefaultOrderBy);
            Report.AddCommandParameters(command, loginUser);
        }

        private static List<DescriptiveClauseItem> GetSummaryDescFields(LoginUser loginUser, SummaryReport summaryReport)
        {
            List<DescriptiveClauseItem> result = new List<DescriptiveClauseItem>();
            ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(loginUser, summaryReport.Subcategory);

            ReportTables tables = new ReportTables(loginUser);
            tables.LoadAll();

            ReportTableFields tableFields = new ReportTableFields(loginUser);
            tableFields.LoadAll();
            TimeSpan offset = loginUser.Offset;
            TicketTypes ticketTypes = new TicketTypes(loginUser);
            ticketTypes.LoadByOrganizationID(loginUser.OrganizationID);

            foreach (ReportSummaryDescriptiveField field in summaryReport.Fields.Descriptive)
            {
                if (field.Field.IsCustom)
                {
                    CustomField customField = (CustomField)CustomFields.GetCustomField(loginUser, field.Field.FieldID);
                    if (customField == null) continue;
                    string fieldName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
                    if (fieldName != "")
                    {
                        fieldName = DataUtils.GetCustomFieldColumn(loginUser, customField, fieldName, true, false);

                        if (customField.FieldType == CustomFieldType.DateTime)
                        {
                            fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
                            fieldName,
                            offset < TimeSpan.Zero ? "-" : "+",
                            Math.Abs(offset.Hours),
                            Math.Abs(offset.Minutes));

                            fieldName = GetDateGroupField(fieldName, field.Value1);
                        }
                        string alias = customField.Name;

                        if (customField.AuxID > 0 && customField.RefType == ReferenceType.Tickets)
                        {
                            TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
                            if (ticketType != null && ticketType.OrganizationID == customField.OrganizationID)
                            {
                                alias = string.Format("{1} ({2})", fieldName, customField.Name, ticketType.Name);
                            }
                        }
                        result.Add(new DescriptiveClauseItem(fieldName, alias));
                    }
                }
                else
                {
                    ReportTableField tableField = tableFields.FindByReportTableFieldID(field.Field.FieldID);
                    ReportTable table = tables.FindByReportTableID(tableField.ReportTableID);
                    string fieldName = table.TableName + "." + tableField.FieldName;
                    if (tableField.DataType.Trim().ToLower() == "datetime")
                    {
                        fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
                          fieldName,
                          offset < TimeSpan.Zero ? "-" : "+",
                          Math.Abs(offset.Hours),
                          Math.Abs(offset.Minutes));
                        fieldName = GetDateGroupField(fieldName, field.Value1);
                    }

                    result.Add(new DescriptiveClauseItem(fieldName, tableField.Alias));
                }
            }
            return result;
        }

        private static List<CalculatedClauseItem> GetSummaryCalcFields(LoginUser loginUser, SummaryReport summaryReport)
        {
            List<CalculatedClauseItem> result = new List<CalculatedClauseItem>();
            ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(loginUser, summaryReport.Subcategory);

            ReportTables tables = new ReportTables(loginUser);
            tables.LoadAll();

            ReportTableFields tableFields = new ReportTableFields(loginUser);
            tableFields.LoadAll(false);
            TimeSpan offset = loginUser.Offset;

            foreach (ReportSummaryCalculatedField field in summaryReport.Fields.Calculated)
            {
                StringBuilder builder = new StringBuilder();
                if (field.Field.IsCustom)
                {
                    CustomField customField = (CustomField)CustomFields.GetCustomField(loginUser, field.Field.FieldID);
                    if (customField == null) continue;
                    string fieldName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
                    if (fieldName != "")
                    {
                        fieldName = DataUtils.GetCustomFieldColumn(loginUser, customField, fieldName, true, false);


                        if (customField.FieldType == CustomFieldType.DateTime)
                        {
                            fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
                            fieldName,
                            offset < TimeSpan.Zero ? "-" : "+",
                            Math.Abs(offset.Hours),
                            Math.Abs(offset.Minutes));
                        }

                        result.Add(GetCalcItem(fieldName, customField.Name, field));
                    }

                }
                else
                {
                    ReportTableField tableField = tableFields.FindByReportTableFieldID(field.Field.FieldID);
                    ReportTable table = tables.FindByReportTableID(tableField.ReportTableID);
                    string fieldName = table.TableName + "." + tableField.FieldName;
                    if (tableField.DataType.Trim().ToLower() == "datetime")
                    {
                        fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
                          fieldName,
                          offset < TimeSpan.Zero ? "-" : "+",
                          Math.Abs(offset.Hours),
                          Math.Abs(offset.Minutes));
                    }
                    result.Add(GetCalcItem(fieldName, tableField.Alias, field));
                }
            }
            return result;
        }

        private static CalculatedClauseItem GetCalcItem(string field, string alias, ReportSummaryCalculatedField calc)
        {
            CalculatedClauseItem result = new CalculatedClauseItem();

            result.Field = field;
            switch (calc.Aggregate.ToLower())
            {
                case "sum":
                    result.Alias = alias + " Sum";
                    result.AggField = string.Format("SUM([{0}])", result.Alias);
                    break;
                case "max":
                    result.Alias = alias + " Max";
                    result.AggField = string.Format("MAX([{0}])", result.Alias);
                    break;
                case "min":
                    result.Alias = alias + " Min";
                    result.AggField = string.Format("MIN([{0}])", result.Alias);
                    break;
                case "avg":
                    result.Alias = alias + " Average";
                    result.AggField = string.Format("AVG([{0}])", result.Alias);
                    break;
                case "stdev":
                    result.Alias = alias + " Std Dev";
                    result.AggField = string.Format("STDEV([{0}])", result.Alias);
                    break;
                case "var":
                    result.Alias = alias + " Variance";
                    result.AggField = string.Format("VAR([{0}])", result.Alias);
                    break;
                case "count":
                    result.Alias = alias + " Count";
                    result.AggField = string.Format("COUNT([{0}])", result.Alias);
                    break;
                case "countdistinct":
                    result.Alias = alias + " Distinct Count";
                    result.AggField = string.Format("COUNT(DISTINCT([{0}]))", result.Alias);
                    break;
                default:
                    break;
            }

            if (calc.Comparator.ToLower() == "none")
            {
                result.Comparator = null;
                return result;
            }

            // verify values are numbers for sql injection
            float.Parse(calc.Value1);


            switch (calc.Comparator.ToLower())
            {
                case "lt": result.Comparator = string.Format("({0} < {1})", result.AggField, calc.Value1); break;
                case "gt": result.Comparator = string.Format("({0} > {1})", result.AggField, calc.Value1); break;
                case "bet": result.Comparator = string.Format("({0} BETWEEN {1} AND {2})", result.AggField, calc.Value1, calc.Value2); float.Parse(calc.Value2); break;
                case "eq": result.Comparator = string.Format("({0} = {1})", result.AggField, calc.Value1); break;
                default:
                    break;
            }


            return result;
        }

        private static string GetDateGroupField(string fieldName, string option)
        {
            switch (option)
            {
                case "year": return string.Format("DATEPART(YEAR, {0})", fieldName);
                case "qtryear": return string.Format("CAST(DATEPART(YEAR, {0}) AS VARCHAR) + '-' + CAST(DATEPART(QUARTER, {0}) AS VARCHAR)", fieldName);
                case "monthyear": return string.Format("CONVERT(CHAR(7), {0}, 121)", fieldName);
                case "weekyear": return string.Format("CAST(DATEPART(YEAR, {0}) AS VARCHAR) + '-' + RIGHT('00' + CAST(DATEPART(WEEK, {0}) AS VARCHAR), 2)", fieldName);
                case "date": return string.Format("CAST({0} AS DATE)", fieldName);
                case "qtr": return string.Format("DATEPART(QUARTER, {0})", fieldName);
                case "month": return string.Format("DATEPART(MONTH, {0}) ", fieldName);
                case "week": return string.Format("DATEPART(WEEK, {0}) ", fieldName);
                case "dayweek": return string.Format("CAST(DATEPART(WEEKDAY, {0}) AS VARCHAR)", fieldName);
                case "daymonth": return string.Format("DATEPART(DAY, {0})", fieldName);
                case "hourday": return string.Format("DATEPART(HOUR, {0})", fieldName);
                default:
                    break;
            }

            return "";
        }

        public static string GetReportChartData(LoginUser loginUser, int reportID)
        {
            Report report = Reports.GetReport(loginUser, reportID, loginUser.UserID);
            Reports.UpdateReportView(loginUser, report.ReportID);
            SummaryReport summaryReport = JsonConvert.DeserializeObject<SummaryReport>(report.ReportDef);
            DataTable table = Reports.GetSummaryData(loginUser, summaryReport, true, report);
            return Reports.BuildChartData(loginUser, table, summaryReport);
        }

        public static string GetChartReportData(LoginUser loginUser, int reportID)
        {
            Report report = Reports.GetReport(loginUser, reportID, loginUser.UserID);
            Reports.UpdateReportView(loginUser, report.ReportID);
            SummaryReport summaryReport = JsonConvert.DeserializeObject<SummaryReport>(report.ReportDef);
            DataTable table = Reports.GetSummaryData(loginUser, summaryReport, true, report);
            return Reports.BuildChartData(loginUser, table, summaryReport);
        }

        public static string GetChartData(LoginUser loginUser, string summaryReportFields)
        {
            SummaryReport summaryReport = JsonConvert.DeserializeObject<SummaryReport>(summaryReportFields);
            DataTable table = Reports.GetSummaryData(loginUser, summaryReport, true);
            return Reports.BuildChartData(loginUser, table, summaryReport);
        }

        public static string GetHubChartData(LoginUser loginUser, Report report)
        {
            SummaryReport summaryReport = JsonConvert.DeserializeObject<SummaryReport>(report.ReportDef);
            DataTable table = Reports.GetSummaryData(loginUser, summaryReport, true, report);
            return Reports.BuildChartData(loginUser, table, summaryReport);
        }

    }
}
