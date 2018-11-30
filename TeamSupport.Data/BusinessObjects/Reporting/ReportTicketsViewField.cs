using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Data.BusinessObjects.Reporting
{
    public enum EField
    {
        TicketID,
        ProductName,
        ReportedVersion,
        SolvedVersion,
        GroupName,
        TicketTypeName,
        UserName,
        Status,
        StatusPosition,
        SeverityPosition,
        IsClosed,
        Severity,
        TicketNumber,
        IsVisibleOnPortal,
        IsKnowledgeBase,
        ReportedVersionID,
        SolvedVersionID,
        ProductID,
        GroupID,
        UserID,
        TicketStatusID,
        TicketTypeID,
        TicketSeverityID,
        OrganizationID,
        Name,
        ParentID,
        ModifierID,
        CreatorID,
        DateModified,
        DateCreated,
        DateClosed,
        CloserID,
        DaysClosed,
        MinutesClosed,
        DaysOpened,
        MinutesOpened,
        CloserName,
        CreatorName,
        ModifierName,
        HoursSpent,
        Tags,
        SlaViolationTime,
        SlaWarningTime,
        SlaViolationHours,
        SlaWarningHours,
        MinsSinceCreated,
        DaysSinceCreated,
        MinsSinceModified,
        DaysSinceModified,
        Contacts,
        Customers,
        SlaViolationTimeClosed,
        SlaViolationLastAction,
        SlaViolationInitialResponse,
        SlaWarningTimeClosed,
        SlaWarningLastAction,
        SlaWarningInitialResponse,
        NeedsIndexing,
        SlaViolationDate,
        SlaWarningDate,
        TicketSource,
        ForumCategory,
        CategoryName,
        CreatorEmail,
        ModifierEmail,
        KnowledgeBaseCategoryID,
        KnowledgeBaseCategoryName,
        KnowledgeBaseParentCategoryName,
        DueDate,
        SalesForceID,
        DateModifiedBySalesForceSync,
        DateModifiedByJiraSync,
        JiraID,
        SyncWithJira,
        JiraKey,
        JiraLinkURL,
        JiraStatus,
        ChildTicketCount,
        RelatedTicketCount,
        DayOfWeekCreated,
        HourOfDayCreated,
        MinutesToInitialResponse,
        MinsSinceInitialPublicResponse,
        WeekdaysOpenened,
        //EmailReplyToAddress,
        ProductFamily
    }

    /// <summary>
    /// The SELECT fields in dbo.ReportTicketsView
    /// </summary>
    class ReportTicketsViewField
    {
        static Dictionary<EField, string> _fieldAs;

        public EField ViewField { get; private set; }

        static ReportTicketsViewField()
        {
            _fieldAs = new Dictionary<EField, string>()
            {
                [EField.TicketID] = "t.TicketID",
                [EField.ProductName] = "p.NAME AS ProductName",
                [EField.ReportedVersion] = "pv1.VersionNumber AS ReportedVersion",
                [EField.SolvedVersion] = "pv2.VersionNumber AS SolvedVersion",
                [EField.GroupName] = "g.NAME AS GroupName",
                [EField.TicketTypeName] = "tt.NAME AS TicketTypeName",
                [EField.UserName] = "u.FirstName + ' ' + u.LastName AS UserName",
                [EField.Status] = "ts.NAME AS STATUS",
                [EField.StatusPosition] = "ts.Position AS StatusPosition",
                [EField.SeverityPosition] = "tse.Position AS SeverityPosition",
                [EField.IsClosed] = "ISNULL(ts.IsClosed, 0) AS IsClosed",
                [EField.Severity] = "tse.NAME AS Severity",
                [EField.TicketNumber] = "t.TicketNumber",
                [EField.IsVisibleOnPortal] = "t.IsVisibleOnPortal",
                [EField.IsKnowledgeBase] = "t.IsKnowledgeBase",
                [EField.ReportedVersionID] = "t.ReportedVersionID",
                [EField.SolvedVersionID] = "t.SolvedVersionID",
                [EField.ProductID] = "t.ProductID",
                [EField.GroupID] = "t.GroupID",
                [EField.UserID] = "t.UserID",
                [EField.TicketStatusID] = "t.TicketStatusID",
                [EField.TicketTypeID] = "t.TicketTypeID",
                [EField.TicketSeverityID] = "t.TicketSeverityID",
                [EField.OrganizationID] = "t.OrganizationID",
                [EField.Name] = "t.NAME",
                [EField.ParentID] = "t.ParentID",
                [EField.ModifierID] = "t.ModifierID",
                [EField.CreatorID] = "t.CreatorID",
                [EField.DateModified] = "t.DateModified",
                [EField.DateCreated] = "t.DateCreated",
                [EField.DateClosed] = "t.DateClosed",
                [EField.CloserID] = "t.CloserID",
                [EField.DaysClosed] = "ISNULL(DATEDIFF(day, t.DateClosed, GETUTCDATE()), 0) AS DaysClosed",
                [EField.MinutesClosed] = "ISNULL(DATEDIFF(MINUTE, t.DateClosed, GETUTCDATE()), 0) AS MinutesClosed",
                [EField.DaysOpened] = "CASE WHEN t.DateClosed IS NOT NULL THEN FLOOR(datediff(hh, t.datecreated, t.dateclosed) / 24) ELSE FLOOR(datediff(hh, t.DateCreated, GETUTCDATE()) / 24) END AS DaysOpened",
                [EField.MinutesOpened] = "CASE WHEN t.DateClosed IS NOT NULL THEN DATEDIFF(MINUTE, t.DateCreated, t.DateClosed) ELSE DATEDIFF(MINUTE, t.DateCreated, GETUTCDATE()) END AS MinutesOpened",
                [EField.CloserName] = "u2.FirstName + ' ' + u2.LastName AS CloserName",
                [EField.CreatorName] = "u3.FirstName + ' ' + u3.LastName AS CreatorName",
                [EField.ModifierName] = "u4.FirstName + ' ' + u4.LastName AS ModifierName",
                [EField.HoursSpent] = "CAST(ISNULL(tsv.TimeSpent, 0) AS DECIMAL) / 60 AS HoursSpent",
                [EField.Tags] = "dbo.uspGetTags(17, t.TicketID) AS Tags",
                [EField.SlaViolationTime] = "DATEDIFF(mi, GETUTCDATE(), dbo.GetMinViolationDate(t.SlaViolationTimeClosed, t.SlaViolationLastAction, t.SlaViolationInitialResponse)) AS SlaViolationTime",
                [EField.SlaWarningTime] = "DATEDIFF(mi, GETUTCDATE(), dbo.GetMinViolationDate(t.SlaWarningTimeClosed, t.SlaWarningLastAction, t.SlaWarningInitialResponse)) AS SlaWarningTime",
                [EField.SlaViolationHours] = "CAST(ISNULL(DATEDIFF(mi, GETUTCDATE(), dbo.GetMinViolationDate(t.SlaViolationTimeClosed, t.SlaViolationLastAction, t.SlaViolationInitialResponse)), 0.0) AS DECIMAL) / 60 AS SlaViolationHours",
                [EField.SlaWarningHours] = "CAST(ISNULL(DATEDIFF(mi, GETUTCDATE(), dbo.GetMinViolationDate(t.SlaWarningTimeClosed, t.SlaWarningLastAction, t.SlaWarningInitialResponse)), 0.0) AS DECIMAL) / 60 AS SlaWarningHours",
                [EField.MinsSinceCreated] = "DATEDIFF(minute, t.DateCreated, GETUTCDATE()) AS MinsSinceCreated",
                [EField.DaysSinceCreated] = "DATEDIFF(day, t.DateCreated, GETUTCDATE()) AS DaysSinceCreated",
                [EField.MinsSinceModified] = "DATEDIFF(minute, t.DateModified, GETUTCDATE()) AS MinsSinceModified",
                [EField.DaysSinceModified] = "DATEDIFF(day, t.DateModified, GETUTCDATE()) AS DaysSinceModified",
                [EField.Contacts] = "dbo.GetTicketContacts(t.TicketID) AS Contacts",
                [EField.Customers] = "dbo.GetTicketCustomersReports(t.TicketID) AS Customers",
                [EField.SlaViolationTimeClosed] = "t.SlaViolationTimeClosed",
                [EField.SlaViolationLastAction] = "t.SlaViolationLastAction",
                [EField.SlaViolationInitialResponse] = "t.SlaViolationInitialResponse",
                [EField.SlaWarningTimeClosed] = "t.SlaWarningTimeClosed",
                [EField.SlaWarningLastAction] = "t.SlaWarningLastAction",
                [EField.SlaWarningInitialResponse] = "t.SlaWarningInitialResponse",
                [EField.NeedsIndexing] = "t.NeedsIndexing",
                [EField.SlaViolationDate] = "dbo.GetMinViolationDate(t.SlaViolationTimeClosed, t.SlaViolationLastAction, t.SlaViolationInitialResponse) AS SlaViolationDate",
                [EField.SlaWarningDate] = "dbo.GetMinViolationDate(t.SlaWarningTimeClosed, t.SlaWarningLastAction, t.SlaWarningInitialResponse) AS SlaWarningDate",
                [EField.TicketSource] = "ISNULL(t.TicketSource, 'Agent') AS TicketSource",
                [EField.ForumCategory] = "ft.ForumCategory",
                [EField.CategoryName] = "fc.CategoryName",
                [EField.CreatorEmail] = "u3.Email AS CreatorEmail",
                [EField.ModifierEmail] = "u4.Email AS ModifierEmail",
                [EField.KnowledgeBaseCategoryID] = "t.KnowledgeBaseCategoryID",
                [EField.KnowledgeBaseCategoryName] = "kbc.CategoryName AS KnowledgeBaseCategoryName",
                [EField.KnowledgeBaseParentCategoryName] = "kbcp.CategoryName AS KnowledgeBaseParentCategoryName",
                [EField.DueDate] = "t.DueDate",
                [EField.SalesForceID] = "t.SalesForceID",
                [EField.DateModifiedBySalesForceSync] = "t.DateModifiedBySalesForceSync",
                [EField.DateModifiedByJiraSync] = "TicketLinkToJira.DateModifiedByJiraSync",
                [EField.JiraID] = "TicketLinkToJira.JiraID",
                [EField.SyncWithJira] = "TicketLinkToJira.SyncWithJira",
                [EField.JiraKey] = "TicketLinkToJira.JiraKey",
                [EField.JiraLinkURL] = "TicketLinkToJira.JiraLinkURL",
                [EField.JiraStatus] = "TicketLinkToJira.JiraStatus",
                [EField.ChildTicketCount] = "(SELECT COUNT(*) AS Expr1 FROM dbo.Tickets AS child WHERE (ParentID = t.TicketID)) AS ChildTicketCount",
                [EField.RelatedTicketCount] = "(SELECT COUNT(*) AS Expr1 FROM dbo.TicketRelationships AS tr WHERE (Ticket1ID = t.TicketID) OR (Ticket2ID = t.TicketID) ) AS RelatedTicketCount",
                [EField.DayOfWeekCreated] = "DATENAME(DW, t.DateCreated) AS DayOfWeekCreated",
                [EField.HourOfDayCreated] = "DATEPART(HOUR, t.DateCreated) AS HourOfDayCreated",
                [EField.MinutesToInitialResponse] = "DATEDIFF(MINUTE, (SELECT TOP (1) DateCreated FROM dbo.Actions AS a WHERE (TicketID = t.TicketID) AND (SystemActionTypeID = 1) ORDER BY DateCreated), " +
                   "(SELECT TOP (1) DateCreated FROM dbo.Actions AS a WHERE (TicketID = t.TicketID) AND (SystemActionTypeID <> 1) AND (SystemActionTypeID <> 5) AND (IsVisibleOnPortal = 1) ORDER BY DateCreated)" +
                   ") AS MinutesToInitialResponse",
                [EField.MinsSinceInitialPublicResponse] = "DATEDIFF(MINUTE, (SELECT TOP (1) DateCreated FROM dbo.Actions AS a WHERE (TicketID = t.TicketID) AND (SystemActionTypeID = 1) ORDER BY DateCreated), " +
                    "(SELECT TOP (1) DateCreated FROM dbo.Actions AS a WHERE (TicketID = t.TicketID) AND (IsVisibleOnPortal = 1) AND (SystemActionTypeID <> 1) AND (SystemActionTypeID <> 5) ORDER BY DateCreated)" +
                    ") AS MinsSinceInitialPublicResponse",
                [EField.WeekdaysOpenened] = "CASE WHEN t.DateClosed IS NOT NULL THEN FLOOR(datediff(hh, t.datecreated, t.dateclosed) / 24) - (DATEDIFF(wk, t.DateCreated, t.DateClosed) * 2) - " +
                    "(CASE WHEN DATENAME(dw, t.DateCreated) = 'Sunday' THEN 1 ELSE 0 END) - " +
                    "(CASE WHEN DATENAME(dw, t.DateClosed) = 'Saturday' THEN 1 ELSE 0 END) ELSE FLOOR(datediff(hh, t.datecreated, GETUTCDATE()) / 24) - (DATEDIFF(wk, t.DateCreated, GETUTCDATE()) * 2) - " +
                    "(CASE WHEN DATENAME(dw, t.DateCreated) = 'Sunday' THEN 1 ELSE 0 END) - " +
                    "(CASE WHEN DATENAME(dw, GETUTCDATE()) = 'Saturday' THEN 1 ELSE 0 END) END AS WeekdaysOpenened",
                //[EField.EmailReplyToAddress] = "t.EmailReplyToAddress",
                [EField.ProductFamily] = "pf.Name AS [ProductFamily]"
            };
        }

        public ReportTicketsViewField(EField field)
        {
            ViewField = field;
        }

        public void ToSql(StringBuilder builder, bool first)
        {
            if(!first)
                builder.Append("\n  ,");
            builder.Append(_fieldAs[ViewField]);
        }

    }
}
