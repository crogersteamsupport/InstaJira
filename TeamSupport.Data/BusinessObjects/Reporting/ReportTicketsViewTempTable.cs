using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Data.BusinessObjects.Reporting
{
    public class ReportTicketsViewTempTable
    {
        public static bool Enable { get; set; }

        static Dictionary<EField, EJoin> _mapFieldToJoin;
        Dictionary<int, EField> _mapIdToField;    // update in case DB changes

        LoginUser _loginUser;
        List<ReportTicketsViewField> _fields;    // store in order added to list
        SortedList<EJoin, ReportTicketsViewJoin> _joins;   // store in order of enum

        static ReportTicketsViewTempTable()
        {
            // WARNING - Update this if ReportTicketsView changes
            _mapFieldToJoin = new Dictionary<EField, EJoin>()
            {
                [EField.TicketID] = EJoin.Tickets,  // dbo.Tickets.TicketID
                [EField.ProductName] = EJoin.ProductID, // "p.NAME AS ProductName",
                [EField.ReportedVersion] = EJoin.ReportedVersionID, // "pv1.VersionNumber AS ReportedVersion",
                [EField.SolvedVersion] = EJoin.SolvedVersionID, // "pv2.VersionNumber AS SolvedVersion",
                [EField.GroupName] = EJoin.Groups, // "g.NAME AS GroupName",
                [EField.TicketTypeName] = EJoin.TicketTypes, // "tt.NAME AS TicketTypeName",
                [EField.UserName] = EJoin.UserID, // "u.FirstName + ' ' + u.LastName AS UserName",
                [EField.Status] = EJoin.TicketStatuses, // "ts.NAME AS STATUS",
                [EField.StatusPosition] = EJoin.TicketStatuses, // "ts.Position AS StatusPosition",
                [EField.SeverityPosition] = EJoin.TicketSeverities, // "tse.Position AS SeverityPosition",
                [EField.IsClosed] = EJoin.TicketStatuses, // "ISNULL(ts.IsClosed, 0) AS IsClosed",
                [EField.Severity] = EJoin.TicketSeverities, // "tse.NAME AS Severity",
                [EField.TicketNumber] = EJoin.Tickets, // "t.TicketNumber",
                [EField.IsVisibleOnPortal] = EJoin.Tickets, // "t.IsVisibleOnPortal",
                [EField.IsKnowledgeBase] = EJoin.Tickets, // "t.IsKnowledgeBase",
                [EField.ReportedVersionID] = EJoin.Tickets, // "t.ReportedVersionID",
                [EField.SolvedVersionID] = EJoin.Tickets, // "t.SolvedVersionID",
                [EField.ProductID] = EJoin.Tickets, // "t.ProductID",
                [EField.GroupID] = EJoin.Tickets, // "t.GroupID",
                [EField.UserID] = EJoin.Tickets, // "t.UserID",
                [EField.TicketStatusID] = EJoin.Tickets, // "t.TicketStatusID",
                [EField.TicketTypeID] = EJoin.Tickets, // "t.TicketTypeID",
                [EField.TicketSeverityID] = EJoin.Tickets, // "t.TicketSeverityID",
                [EField.OrganizationID] = EJoin.Tickets, // "t.OrganizationID",
                [EField.Name] = EJoin.Tickets, // "t.NAME",
                [EField.ParentID] = EJoin.Tickets, // "t.ParentID",
                [EField.CreatorID] = EJoin.Tickets, // "t.ModifierID",
                [EField.ModifierID] = EJoin.Tickets, // "t.ModifierID",
                [EField.DateModified] = EJoin.Tickets, // "t.DateModified",
                [EField.DateCreated] = EJoin.Tickets, // "t.DateCreated",
                [EField.DateClosed] = EJoin.Tickets, // "t.DateClosed",
                [EField.CloserID] = EJoin.Tickets, // "t.CloserID",
                [EField.DaysClosed] = EJoin.Tickets, // "ISNULL(DATEDIFF(day, t.DateClosed, GETUTCDATE()), 0) AS DaysClosed",
                [EField.MinutesClosed] = EJoin.Tickets, // "ISNULL(DATEDIFF(MINUTE, t.DateClosed, GETUTCDATE()), 0) AS MinutesClosed",
                [EField.DaysOpened] = EJoin.Tickets, // "CASE WHEN t.DateClosed IS NOT NULL THEN FLOOR(datediff(hh, t.datecreated, t.dateclosed) / 24) ELSE FLOOR(datediff(hh, t.DateCreated, GETUTCDATE()) / 24) END AS DaysOpened",
                [EField.MinutesOpened] = EJoin.Tickets, // "CASE WHEN t.DateClosed IS NOT NULL THEN DATEDIFF(MINUTE, t.DateCreated, t.DateClosed) ELSE DATEDIFF(MINUTE, t.DateCreated, GETUTCDATE()) END AS MinutesOpened",
                [EField.CloserName] = EJoin.CloserID, // "u2.FirstName + ' ' + u2.LastName AS CloserName",
                [EField.CreatorName] = EJoin.CreatorID, // "u3.FirstName + ' ' + u3.LastName AS CreatorName",
                [EField.ModifierName] = EJoin.ModifierID, // "u4.FirstName + ' ' + u4.LastName AS ModifierName",
                [EField.HoursSpent] = EJoin.TimeSpent, // "CAST(ISNULL(tsv.TimeSpent, 0) AS DECIMAL) / 60 AS HoursSpent",
                [EField.Tags] = EJoin.Tickets, // "dbo.uspGetTags(17, t.TicketID) AS Tags",
                [EField.SlaViolationTime] = EJoin.Tickets, // "DATEDIFF(mi, GETUTCDATE(), dbo.GetMinViolationDate(t.SlaViolationTimeClosed, t.SlaViolationLastAction, t.SlaViolationInitialResponse)) AS SlaViolationTime",
                [EField.SlaWarningTime] = EJoin.Tickets, // "DATEDIFF(mi, GETUTCDATE(), dbo.GetMinViolationDate(t.SlaWarningTimeClosed, t.SlaWarningLastAction, t.SlaWarningInitialResponse)) AS SlaWarningTime",
                [EField.SlaViolationHours] = EJoin.Tickets, // "CAST(ISNULL(DATEDIFF(mi, GETUTCDATE(), dbo.GetMinViolationDate(t.SlaViolationTimeClosed, t.SlaViolationLastAction, t.SlaViolationInitialResponse)), 0.0) AS DECIMAL) / 60 AS SlaViolationHours",
                [EField.SlaWarningHours] = EJoin.Tickets, // "CAST(ISNULL(DATEDIFF(mi, GETUTCDATE(), dbo.GetMinViolationDate(t.SlaWarningTimeClosed, t.SlaWarningLastAction, t.SlaWarningInitialResponse)), 0.0) AS DECIMAL) / 60 AS SlaWarningHours",
                [EField.MinsSinceCreated] = EJoin.Tickets, // "DATEDIFF(minute, t.DateCreated, GETUTCDATE()) AS MinsSinceCreated",
                [EField.DaysSinceCreated] = EJoin.Tickets, // "DATEDIFF(day, t.DateCreated, GETUTCDATE()) AS DaysSinceCreated",
                [EField.MinsSinceModified] = EJoin.Tickets, // "DATEDIFF(minute, t.DateModified, GETUTCDATE()) AS MinsSinceModified",
                [EField.DaysSinceModified] = EJoin.Tickets, // "DATEDIFF(day, t.DateModified, GETUTCDATE()) AS DaysSinceModified",
                [EField.Contacts] = EJoin.Tickets, // "dbo.GetTicketContacts(t.TicketID) AS Contacts",
                [EField.Customers] = EJoin.Tickets, // "dbo.GetTicketCustomersReports(t.TicketID) AS Customers",
                [EField.SlaViolationTimeClosed] = EJoin.Tickets, // "t.SlaViolationTimeClosed",
                [EField.SlaViolationLastAction] = EJoin.Tickets, // "t.SlaViolationLastAction",
                [EField.SlaViolationInitialResponse] = EJoin.Tickets, // "t.SlaViolationInitialResponse",
                [EField.SlaWarningTimeClosed] = EJoin.Tickets, // "t.SlaWarningTimeClosed",
                [EField.SlaWarningLastAction] = EJoin.Tickets, // "t.SlaWarningLastAction",
                [EField.SlaWarningInitialResponse] = EJoin.Tickets, // "t.SlaWarningInitialResponse",
                [EField.NeedsIndexing] = EJoin.Tickets, // "t.NeedsIndexing",
                [EField.SlaViolationDate] = EJoin.Tickets, // "dbo.GetMinViolationDate(t.SlaViolationTimeClosed, t.SlaViolationLastAction, t.SlaViolationInitialResponse) AS SlaViolationDate",
                [EField.SlaWarningDate] = EJoin.Tickets, // "dbo.GetMinViolationDate(t.SlaWarningTimeClosed, t.SlaWarningLastAction, t.SlaWarningInitialResponse) AS SlaWarningDate",
                [EField.TicketSource] = EJoin.Tickets, // "ISNULL(t.TicketSource, 'Agent') AS TicketSource",
                [EField.ForumCategory] = EJoin.ForumTickets, // "ft.ForumCategory",
                [EField.CategoryName] = EJoin.ForumCategories, // "fc.CategoryName",
                [EField.CreatorEmail] = EJoin.CreatorID, // "u3.Email AS CreatorEmail",
                [EField.ModifierEmail] = EJoin.ModifierID, // "u4.Email AS ModifierEmail",
                [EField.KnowledgeBaseCategoryID] = EJoin.Tickets, // "t.KnowledgeBaseCategoryID",
                [EField.KnowledgeBaseCategoryName] = EJoin.CategoryID, // "kbc.CategoryName AS KnowledgeBaseCategoryName",
                [EField.KnowledgeBaseParentCategoryName] = EJoin.ParentID, // "kbcp.CategoryName AS KnowledgeBaseParentCategoryName",
                [EField.DueDate] = EJoin.Tickets, // "t.DueDate",
                [EField.SalesForceID] = EJoin.Tickets, // "t.SalesForceID",
                [EField.DateModifiedBySalesForceSync] = EJoin.Tickets, // "t.DateModifiedBySalesForceSync",
                [EField.DateModifiedByJiraSync] = EJoin.TicketLinkToJira, // "TicketLinkToJira.DateModifiedByJiraSync",
                [EField.JiraID] = EJoin.TicketLinkToJira, // "TicketLinkToJira.JiraID",
                [EField.SyncWithJira] = EJoin.TicketLinkToJira, // "TicketLinkToJira.SyncWithJira",
                [EField.JiraKey] = EJoin.TicketLinkToJira, // "TicketLinkToJira.JiraKey",
                [EField.JiraLinkURL] = EJoin.TicketLinkToJira, // "TicketLinkToJira.JiraLinkURL",
                [EField.JiraStatus] = EJoin.TicketLinkToJira, // "TicketLinkToJira.JiraStatus",
                [EField.ChildTicketCount] = EJoin.Tickets, // "(SELECT COUNT(*) AS Expr1 FROM dbo.Tickets AS child WHERE (ParentID = t.TicketID)) AS ChildTicketCount",
                [EField.RelatedTicketCount] = EJoin.Tickets, // "(SELECT COUNT(*) AS Expr1 FROM dbo.TicketRelationships AS tr WHERE (Ticket1ID = t.TicketID) OR (Ticket2ID = t.TicketID) ) AS RelatedTicketCount",
                [EField.DayOfWeekCreated] = EJoin.Tickets, // "DATENAME(DW, t.DateCreated) AS DayOfWeekCreated",
                [EField.HourOfDayCreated] = EJoin.Tickets, // "DATEPART(HOUR, t.DateCreated) AS HourOfDayCreated",
                [EField.MinutesToInitialResponse] = EJoin.TimeSpent, // "DATEDIFF(MINUTE, (SELECT TOP (1) DateCreated FROM dbo.Actions AS a WHERE (TicketID = t.TicketID) AND (SystemActionTypeID = 1) ORDER BY DateCreated), " +
                [EField.MinsSinceInitialPublicResponse] = EJoin.TimeSpent, // "DATEDIFF(MINUTE, (SELECT TOP (1) DateCreated FROM dbo.Actions AS a WHERE (TicketID = t.TicketID) AND (SystemActionTypeID = 1) ORDER BY DateCreated), " +
                [EField.WeekdaysOpenened] = EJoin.Tickets,
                //[EField.EmailReplyToAddress] = EJoin.Tickets, // "t.EmailReplyToAddress",
                [EField.ProductFamily] = EJoin.ProductFamilies, // "pf.Name AS [ProductFamily]"
            };
            Enable = true;
        }

        private ReportTicketsViewTempTable(LoginUser loginUser)
        {
            _loginUser = loginUser;
            _fields = new List<ReportTicketsViewField>();
            _joins = new SortedList<EJoin, ReportTicketsViewJoin>();
            _mapIdToField = MapFieldIdToEField();
        }

        /// <summary> Tabular Report </summary>
        public ReportTicketsViewTempTable(LoginUser loginUser, TabularReport tabularReport) : this(loginUser)
        {
            // SELECT 
            foreach (ReportSelectedField field in tabularReport.Fields)
                Add(field.FieldID);

            // filter fields (WHERE, ORDER BY...)
            foreach (ReportFilter filter in tabularReport.Filters)
                foreach (ReportFilterCondition condition in filter.Conditions)
                    Add(condition.FieldID);

            if (_fields.Count > 0)  // include TicketID and OrganizationID
            {
                Add(EField.TicketID);
                Add(EField.OrganizationID);
            }
        }

        /// <summary> Summary Report </summary>
        public ReportTicketsViewTempTable(LoginUser loginUser, SummaryReport summaryReport) : this(loginUser)
        {
            // SELECT fields
            foreach (ReportSummaryDescriptiveField field in summaryReport.Fields.Descriptive)
                Add(field.Field.FieldID);

            foreach (ReportSummaryCalculatedField field in summaryReport.Fields.Calculated)
                Add(field.Field.FieldID);

            // filter fields (WHERE, ORDER BY...)
            foreach (ReportFilter filter in summaryReport.Filters)
                foreach (ReportFilterCondition condition in filter.Conditions)
                    Add(condition.FieldID);

            if (_fields.Count > 0)  // include TicketID and OrganizationID
            {
                Add(EField.TicketID);
                Add(EField.OrganizationID);
            }
        }

        Dictionary<int, EField> MapFieldIdToEField()
        {
            // load all 39 reporting tables to find ReportTicketsView
            ReportTables tables = new ReportTables(_loginUser);
            tables.LoadAll();
            int reportTableID = tables.Where(t => t.TableName == "ReportTicketsView").Select(t => t.ReportTableID).Min();    // Min is faster than First or Single

            // load all 964 and find the ReportTicketsView fields
            ReportTableFields tableFields = new ReportTableFields(_loginUser);
            tableFields.LoadAll();
            var queryable = tableFields.Where(f => f.ReportTableID == reportTableID);

            Dictionary<int, EField>  mapIdToField = new Dictionary<int, EField>();
            foreach (ReportTableField row in queryable)
            {
                if (!Enum.TryParse(row.FieldName, out EField eField))
                    if (System.Diagnostics.Debugger.IsAttached) System.Diagnostics.Debugger.Break();
                mapIdToField[row.ReportTableFieldID] = eField;
            }
            return mapIdToField;
        }

        void Add(int fieldID)
        {
            EField field;
            if (!_mapIdToField.TryGetValue(fieldID, out field))
                return;

            Add(field);
        }

        void Add(EField field)
        {
            if (_fields.Where(f => f.ViewField == field).Any())
                return; // found

            _fields.Add(new ReportTicketsViewField(field));

            EJoin table = _mapFieldToJoin[field];
            if (!_joins.ContainsKey(table))
                _joins[table] = new ReportTicketsViewJoin(table);
        }

        public string ToSql()
        {
            if (_fields.Count == 0)
                return String.Empty;

            StringBuilder builder = new StringBuilder();
            builder.Append("SELECT ");
            bool first = true;
            foreach (ReportTicketsViewField field in _fields)
            {
                field.ToSql(builder, first);
                first = false;
            }

            builder.Append("\nINTO ReportTicketsView \nFROM ");
            foreach (KeyValuePair<EJoin, ReportTicketsViewJoin> pair in _joins)
                pair.Value.ToSql(builder);

            builder.Append($"\nWHERE t.OrganizationID={_loginUser.OrganizationID};\n");
            return builder.ToString();
        }
    }
}
