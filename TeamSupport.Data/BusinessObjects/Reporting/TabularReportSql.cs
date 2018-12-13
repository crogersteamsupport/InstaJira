﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace TeamSupport.Data.BusinessObjects.Reporting
{
    public class TabularReport
    {
        public TabularReport() { }
        public int Subcategory { get; set; }
        public ReportSelectedField[] Fields { get; set; }
        public ReportFilter[] Filters { get; set; }
    }


    public class TabularReportSql
    {
        UserRights _userRights;
        string _organizationIDFieldName;
        public bool IsOrganizationID    // can't optimize if using ParentID
        {
            get
            {
                return string.IsNullOrEmpty(_organizationIDFieldName) ? true : _organizationIDFieldName.Equals("OrganizationID");
            }
        }

        public TabularReportSql(LoginUser loginUser)
        {
            _userRights = new UserRights(loginUser);
        }

        public void GetTabularSql(SqlCommand command, TabularReport tabularReport, bool inlcudeHiddenFields, bool isSchemaOnly, int? reportID, bool useUserFilter, string sortField = null, string sortDir = null)
        {
            LoginUser loginUser = _userRights._loginUser;
            StringBuilder builder = new StringBuilder();
            GetTabluarSelectClause(command, builder, tabularReport, inlcudeHiddenFields, isSchemaOnly, sortField, sortDir);
            if (isSchemaOnly)
            {
                command.CommandText = builder.ToString();
            }
            else
            {

                string primaryTableKeyFieldName = null;
                if (tabularReport.Subcategory == 70)
                {
                    primaryTableKeyFieldName = "UserTicketsView.TicketID";
                }

                Report.GetWhereClause(loginUser, command, builder, tabularReport.Filters, primaryTableKeyFieldName);
                if (useUserFilter && reportID != null)
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
                            ExceptionLogs.LogException(loginUser, ex, "Tabular SQL - User filters");
                        }
                    }
                }

                command.CommandText = builder.ToString();
            }
        }

        public static void GetTabularSqlForExports(LoginUser loginUser, SqlCommand command, TabularReport tabularReport, bool inlcudeHiddenFields, bool isSchemaOnly, int? reportID, bool useUserFilter, string sortField = null, string sortDir = null)
        {
            StringBuilder builder = new StringBuilder();
            TabularReportSql tabularReportSql = new TabularReportSql(loginUser);
            tabularReportSql.GetTabluarSelectClause(command, builder, tabularReport, inlcudeHiddenFields, isSchemaOnly, sortField, sortDir);
            if (isSchemaOnly)
            {
                command.CommandText = builder.ToString();
            }
            else
            {

                string primaryTableKeyFieldName = null;
                if (tabularReport.Subcategory == 70)
                {
                    primaryTableKeyFieldName = "UserTicketsView.TicketID";
                }

                GetWhereClauseForExport(loginUser, command, builder, tabularReport.Filters, primaryTableKeyFieldName);
                if (useUserFilter && reportID != null)
                {
                    Report report = Reports.GetReport(loginUser, (int)reportID, loginUser.UserID);
                    if (report != null && report.Row["Settings"] != DBNull.Value)
                    {
                        try
                        {
                            UserTabularSettings userFilters = JsonConvert.DeserializeObject<UserTabularSettings>((string)report.Row["Settings"]);

                            if (userFilters != null)
                            {
                                GetWhereClauseForExport(loginUser, command, builder, userFilters.Filters);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogs.LogException(loginUser, ex, "Tabular SQL - User filters");
                        }
                    }
                }

                command.CommandText = builder.ToString();

            }
        }
        public static void GetPortalTicketsSQL(LoginUser loginUser, SqlCommand command, TabularReport tabularReport, bool inlcudeHiddenFields, bool isSchemaOnly)
        {
            StringBuilder builder = new StringBuilder();
            TabularReportSql tabularReportSql = new TabularReportSql(loginUser);
            tabularReportSql.GetTabluarSelectClause(command, builder, tabularReport, inlcudeHiddenFields, isSchemaOnly);
            if (isSchemaOnly)
            {
                command.CommandText = builder.ToString();
            }
            else
            {

                string primaryTableKeyFieldName = null;
                if (tabularReport.Subcategory == 70)
                {
                    primaryTableKeyFieldName = "UserTicketsView.TicketID";
                }
                else if (tabularReport.Subcategory == 74) primaryTableKeyFieldName = "TicketsView.TicketID";


                Report.GetWhereClause(loginUser, command, builder, tabularReport.Filters, primaryTableKeyFieldName);

                command.CommandText = builder.ToString();
            }
        }

        private void GetTabluarSelectClause(SqlCommand command, StringBuilder builder, TabularReport tabularReport, bool includeHiddenFields, bool isSchemaOnly, string sortField = null, string sortDir = null)
        {
            LoginUser loginUser = _userRights._loginUser;
            ReportSubcategory sub = ReportSubcategories.GetReportSubcategory(loginUser, tabularReport.Subcategory);

            ReportTables tables = new ReportTables(loginUser);
            tables.LoadAll();

            ReportTableFields tableFields = new ReportTableFields(loginUser);
            tableFields.LoadAll();
            TimeSpan offset = loginUser.Offset;
            TicketTypes ticketTypes = new TicketTypes(loginUser);
            ticketTypes.LoadByOrganizationID(loginUser.OrganizationID);

            string sortClause = "";


            foreach (ReportSelectedField field in tabularReport.Fields)
            {

                if (field.IsCustom)
                {
                    CustomField customField = (CustomField)CustomFields.GetCustomField(loginUser, field.FieldID);
                    if (customField == null) continue;
                    string fieldName = DataUtils.GetReportPrimaryKeyFieldName(customField.RefType);
                    GetCommandCustomField(builder, tabularReport, sortField, loginUser, ref offset, ticketTypes, ref sortClause, customField, ref fieldName);
                }
                else
                {
                    GetCommandField(builder, sortField, tables, tableFields, ref offset, ref sortClause, field);
                }
            }

            GetCommandHelper(command, builder, tabularReport, includeHiddenFields, isSchemaOnly, sortDir, sub, tables, offset, sortClause, sortField);
        }

        private static void GetCommandCustomField(StringBuilder builder, TabularReport tabularReport, string sortField, LoginUser loginUser, ref TimeSpan offset, TicketTypes ticketTypes, ref string sortClause, CustomField customField, ref string fieldName)
        {
            if (fieldName != "")
            {
                //handle the ticket views custom fields
                if (tabularReport.Subcategory == 70)
                {
                    fieldName = "UserTicketsView.TicketID";
                }
                else if (tabularReport.Subcategory == 74) fieldName = "TicketsView.TicketID";


                fieldName = DataUtils.GetCustomFieldColumn(loginUser, customField, fieldName, true, false);
                string colName = fieldName;

                if (customField.FieldType == CustomFieldType.DateTime)
                {
                    fieldName = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
                    fieldName,
                    offset < TimeSpan.Zero ? "-" : "+",
                    Math.Abs(offset.Hours),
                    Math.Abs(offset.Minutes));
                }
                else if (customField.FieldType == CustomFieldType.Boolean)
                {
                    fieldName = string.Format("(SELECT ISNULL(({0}),0))", fieldName);
                }

                if (!string.IsNullOrWhiteSpace(sortField) && colName == sortField) {
                    sortClause = fieldName;
                }

                builder.Append(builder.Length < 1 ? "SELECT " : ", ");
                string displayName = customField.Name;
                if (customField.AuxID > 0 && customField.RefType == ReferenceType.Tickets)
                {
                    TicketType ticketType = ticketTypes.FindByTicketTypeID(customField.AuxID);
                    if (ticketType != null && ticketType.OrganizationID == customField.OrganizationID)
                    {
                        displayName = $"{customField.Name} ({ticketType.Name})";
                    }
                }
                builder.Append($"{fieldName} AS [{displayName}]");

                if (!string.IsNullOrWhiteSpace(sortField) && displayName == sortField)
                {
                    sortClause = fieldName;
                }

            }
        }


        private static void GetCommandField(StringBuilder builder, string sortField, ReportTables tables, ReportTableFields tableFields, ref TimeSpan offset, ref string sortClause, ReportSelectedField field)
        {
            ReportTableField tableField = tableFields.FindByReportTableFieldID(field.FieldID);

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

            if (!string.IsNullOrWhiteSpace(sortField) && tableField.Alias == sortField)
            {
                sortClause = fieldName;
            }

            if (builder.Length < 1)
            {
                builder.Append("SELECT " + fieldName + " AS [" + tableField.Alias + "]");
            }
            else
            {
                builder.Append(", " + fieldName + " AS [" + tableField.Alias + "]");
            }
        }

        private void GetCommandHelper(SqlCommand command, StringBuilder builder, TabularReport tabularReport, bool includeHiddenFields, bool isSchemaOnly, string sortDir, ReportSubcategory sub, ReportTables tables, TimeSpan offset, string sortClause, string sortField)
        {
            if (!string.IsNullOrWhiteSpace(sortClause))
            {
                builder.Append($", ROW_NUMBER() OVER (ORDER BY {sortClause} {sortDir}) AS [RowNum]");
            }

            if (includeHiddenFields)
            {
                GetCommandHiddenFields(builder, tabularReport, sub, tables, offset, sortField);
            }
            GetCommandHelper1(command, builder, tabularReport, isSchemaOnly, sub, tables);
        }

        private static void GetCommandHiddenFields(StringBuilder builder, TabularReport tabularReport, ReportSubcategory sub, ReportTables tables, TimeSpan offset, string sortField)
        {
            ReportTable hiddenTable = tables.FindByReportTableID(sub.ReportCategoryTableID);
            AppendField(builder, hiddenTable.TableName, hiddenTable.LookupKeyFieldName, sortField);

            if (sub.ReportTableID != null)
            {
                hiddenTable = tables.FindByReportTableID((int)sub.ReportTableID);
                AppendField(builder, hiddenTable.TableName, hiddenTable.LookupKeyFieldName, sortField);
            }

            if (tabularReport.Subcategory == 70)
            {
                offset = AppendDateField(builder, offset, sortField, hiddenTable, "DueDate");
                offset = AppendDateField(builder, offset, sortField, hiddenTable, "DateModified");

                AppendField(builder, hiddenTable.TableName, "SlaWarningTime", sortField);
                AppendField(builder, hiddenTable.TableName, "SlaViolationTime", sortField);
                AppendField(builder, hiddenTable.TableName, "IsRead", sortField);
                AppendField(builder, hiddenTable.TableName, "IsClosed", sortField);
                AppendField(builder, hiddenTable.TableName, "TicketTypeID", sortField);
                AppendField(builder, hiddenTable.TableName, "UserID", sortField);
                AppendField(builder, hiddenTable.TableName, "SeverityPosition", sortField);
                AppendField(builder, hiddenTable.TableName, "StatusPosition", sortField);
            }
        }

        private static TimeSpan AppendDateField(StringBuilder builder, TimeSpan offset, string sortField, ReportTable hiddenTable, string fieldName)
        {
            string dueDateField = hiddenTable.TableName + "." + fieldName;
            dueDateField = string.Format("CAST(SWITCHOFFSET(TODATETIMEOFFSET({0}, '+00:00'), '{1}{2:D2}:{3:D2}') AS DATETIME)",
                dueDateField,
                offset < TimeSpan.Zero ? "-" : "+",
                Math.Abs(offset.Hours),
                Math.Abs(offset.Minutes));
            AppendField(builder, hiddenTable.TableName, fieldName, sortField, dueDateField);
            return offset;
        }

        private static void AppendField(StringBuilder builder, string tableName, string fieldName, string sortField, string dboField = "")
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return;

            string hiddenFieldName = "hidden" + fieldName;
            if(String.IsNullOrEmpty(dboField))
                dboField = $"{tableName}.{fieldName}";
            builder.Append($", {dboField} AS [{hiddenFieldName}]");

            if(hiddenFieldName.Equals(sortField))
                builder.Append($", ROW_NUMBER() OVER (ORDER BY {tableName}.{fieldName}) AS [RowNum]");
        }

        private void GetCommandHelper1(SqlCommand command, StringBuilder builder, TabularReport tabularReport, bool isSchemaOnly, ReportSubcategory sub, ReportTables tables)
        {
            builder.Append(" " + sub.BaseQuery);

            ReportTable mainTable = tables.FindByReportTableID(sub.ReportCategoryTableID);

            _organizationIDFieldName = mainTable.OrganizationIDFieldName;

            builder.Append(" WHERE (" + mainTable.TableName + "." + mainTable.OrganizationIDFieldName + " = @OrganizationID)");
            if (tabularReport.Subcategory == 70)
            {
                builder.Append(" AND (" + mainTable.TableName + ".ViewerID = @UserID)");
            }

            _userRights.UseTicketRights((int)tabularReport.Subcategory, tables, command, builder);

            if (isSchemaOnly) builder.Append(" AND (0=1)");
        }

        private static void GetWhereClauseForExport(LoginUser loginUser, SqlCommand command, StringBuilder builder, ReportFilter[] filters, string primaryTableKeyName = null)
        {
            if (filters != null) WriteFiltersForExports(loginUser, command, builder, filters, null, primaryTableKeyName);
        }


        private static void WriteFiltersForExports(LoginUser loginUser, SqlCommand command, StringBuilder builder, ReportFilter[] filters, ReportFilter parentFilter, string primaryTableKeyName = null)
        {
            foreach (ReportFilter filter in filters)
            {
                if (filter.Conditions.Length < 1) continue;

                builder.Append(string.Format(" {0} (", parentFilter == null ? "AND" : parentFilter.Conjunction.ToUpper()));
                Report.WriteFilterForExport(loginUser, command, builder, filter, primaryTableKeyName);
                WriteFiltersForExports(loginUser, command, builder, filter.Filters, filter, primaryTableKeyName);
                builder.Append(")");
            }
        }

        public static DataTable GetPortalTicketsTablePage(LoginUser loginUser, int from, int to, string sortField, TabularReport tabReport)
        {
            from++;
            to++;

            SqlCommand command = new SqlCommand();
            GetPortalTicketsSQL(loginUser, command, tabReport, true, false);

            command.Parameters.AddWithValue("@From", from);
            command.Parameters.AddWithValue("@To", to);

            Report.AddCommandParameters(command, loginUser);

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
                    ExceptionLogs.LogException(loginUser, ex, "Portal Tickets Data", DataUtils.GetCommandTextSql(command));
                    throw;
                }
                connection.Close();
            }

            return table;
        }

    }
}
