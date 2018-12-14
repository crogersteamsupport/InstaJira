using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ReportTicketsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketID", "TicketID", false, false, true);
      _fieldMap.AddMap("ProductName", "ProductName", false, false, true);
      _fieldMap.AddMap("ReportedVersion", "ReportedVersion", false, false, true);
      _fieldMap.AddMap("SolvedVersion", "SolvedVersion", false, false, true);
      _fieldMap.AddMap("GroupName", "GroupName", false, false, true);
      _fieldMap.AddMap("TicketTypeName", "TicketTypeName", false, false, true);
      _fieldMap.AddMap("UserName", "UserName", false, false, true);
      _fieldMap.AddMap("Status", "Status", false, false, true);
      _fieldMap.AddMap("StatusPosition", "StatusPosition", false, false, true);
      _fieldMap.AddMap("SeverityPosition", "SeverityPosition", false, false, true);
      _fieldMap.AddMap("IsClosed", "IsClosed", false, false, true);
      _fieldMap.AddMap("Severity", "Severity", false, false, true);
      _fieldMap.AddMap("TicketNumber", "TicketNumber", false, false, true);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", false, false, true);
      _fieldMap.AddMap("IsKnowledgeBase", "IsKnowledgeBase", false, false, true);
      _fieldMap.AddMap("ReportedVersionID", "ReportedVersionID", false, false, true);
      _fieldMap.AddMap("SolvedVersionID", "SolvedVersionID", false, false, true);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, true);
      _fieldMap.AddMap("GroupID", "GroupID", false, false, true);
      _fieldMap.AddMap("UserID", "UserID", false, false, true);
      _fieldMap.AddMap("TicketStatusID", "TicketStatusID", false, false, true);
      _fieldMap.AddMap("TicketTypeID", "TicketTypeID", false, false, true);
      _fieldMap.AddMap("TicketSeverityID", "TicketSeverityID", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, true);
      _fieldMap.AddMap("Name", "Name", false, false, true);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateClosed", "DateClosed", false, false, true);
      _fieldMap.AddMap("CloserID", "CloserID", false, false, true);
      _fieldMap.AddMap("DaysClosed", "DaysClosed", false, false, true);
      _fieldMap.AddMap("MinutesClosed", "MinutesClosed", false, false, true);
      _fieldMap.AddMap("DaysOpened", "DaysOpened", false, false, true);
      _fieldMap.AddMap("MinutesOpened", "MinutesOpened", false, false, true);
      _fieldMap.AddMap("CloserName", "CloserName", false, false, true);
      _fieldMap.AddMap("CreatorName", "CreatorName", false, false, true);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, true);
      _fieldMap.AddMap("HoursSpent", "HoursSpent", false, false, true);
      _fieldMap.AddMap("Tags", "Tags", false, false, true);
      _fieldMap.AddMap("SlaViolationTime", "SlaViolationTime", false, false, true);
      _fieldMap.AddMap("SlaWarningTime", "SlaWarningTime", false, false, true);
      _fieldMap.AddMap("SlaViolationHours", "SlaViolationHours", false, false, true);
      _fieldMap.AddMap("SlaWarningHours", "SlaWarningHours", false, false, true);
      _fieldMap.AddMap("MinsSinceCreated", "MinsSinceCreated", false, false, true);
      _fieldMap.AddMap("DaysSinceCreated", "DaysSinceCreated", false, false, true);
      _fieldMap.AddMap("MinsSinceModified", "MinsSinceModified", false, false, true);
      _fieldMap.AddMap("DaysSinceModified", "DaysSinceModified", false, false, true);
      _fieldMap.AddMap("Contacts", "Contacts", false, false, true);
      _fieldMap.AddMap("Customers", "Customers", false, false, true);
      _fieldMap.AddMap("SlaViolationTimeClosed", "SlaViolationTimeClosed", false, false, true);
      _fieldMap.AddMap("SlaViolationLastAction", "SlaViolationLastAction", false, false, true);
      _fieldMap.AddMap("SlaViolationInitialResponse", "SlaViolationInitialResponse", false, false, true);
      _fieldMap.AddMap("SlaWarningTimeClosed", "SlaWarningTimeClosed", false, false, true);
      _fieldMap.AddMap("SlaWarningLastAction", "SlaWarningLastAction", false, false, true);
      _fieldMap.AddMap("SlaWarningInitialResponse", "SlaWarningInitialResponse", false, false, true);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, true);
      _fieldMap.AddMap("SlaViolationDate", "SlaViolationDate", false, false, true);
      _fieldMap.AddMap("SlaWarningDate", "SlaWarningDate", false, false, true);
      _fieldMap.AddMap("TicketSource", "TicketSource", false, false, true);
      _fieldMap.AddMap("ForumCategory", "ForumCategory", false, false, true);
      _fieldMap.AddMap("CategoryName", "CategoryName", false, false, true);
      _fieldMap.AddMap("CreatorEmail", "CreatorEmail", false, false, true);
      _fieldMap.AddMap("ModifierEmail", "ModifierEmail", false, false, true);
      _fieldMap.AddMap("KnowledgeBaseCategoryID", "KnowledgeBaseCategoryID", false, false, true);
      _fieldMap.AddMap("KnowledgeBaseCategoryName", "KnowledgeBaseCategoryName", false, false, true);
      _fieldMap.AddMap("KnowledgeBaseParentCategoryName", "KnowledgeBaseParentCategoryName", false, false, true);
      _fieldMap.AddMap("DueDate", "DueDate", false, false, true);
      _fieldMap.AddMap("SalesForceID", "SalesForceID", false, false, false);
      _fieldMap.AddMap("DateModifiedBySalesForceSync", "DateModifiedBySalesForceSync", false, false, false);
      _fieldMap.AddMap("DateModifiedByJiraSync", "DateModifiedByJiraSync", false, false, false);
      _fieldMap.AddMap("JiraID", "JiraID", false, false, false);
      _fieldMap.AddMap("SyncWithJira", "SyncWithJira", false, false, false);
      _fieldMap.AddMap("JiraKey", "JiraKey", false, false, false);
      _fieldMap.AddMap("JiraLinkURL", "JiraLinkURL", false, false, false);
      _fieldMap.AddMap("JiraStatus", "JiraStatus", false, false, false);
      _fieldMap.AddMap("ChildTicketCount", "ChildTicketCount", false, false, true);
      _fieldMap.AddMap("RelatedTicketCount", "RelatedTicketCount", false, false, true);
      _fieldMap.AddMap("MinutesToInitialResponse", "MinutesToInitialResponse", false, false, true);
      _fieldMap.AddMap("DayOfWeekCreated", "DayOfWeekCreated", false, false, true);
      _fieldMap.AddMap("HourOfDayCreated", "HourOfDayCreated", false, false, true);
            
    }
  }
  
}
