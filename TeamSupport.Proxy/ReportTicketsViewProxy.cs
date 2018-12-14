using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(ReportTicketsViewItemProxy))]
  public class ReportTicketsViewItemProxy
  {
    public ReportTicketsViewItemProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public string ProductName { get; set; }
    [DataMember] public string ReportedVersion { get; set; }
    [DataMember] public string SolvedVersion { get; set; }
    [DataMember] public string GroupName { get; set; }
    [DataMember] public string TicketTypeName { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public string Status { get; set; }
    [DataMember] public int? StatusPosition { get; set; }
    [DataMember] public int? SeverityPosition { get; set; }
    [DataMember] public bool IsClosed { get; set; }
    [DataMember] public string Severity { get; set; }
    [DataMember] public int TicketNumber { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgeBase { get; set; }
    [DataMember] public int? ReportedVersionID { get; set; }
    [DataMember] public int? SolvedVersionID { get; set; }
    [DataMember] public int? ProductID { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public int TicketStatusID { get; set; }
    [DataMember] public int TicketTypeID { get; set; }
    [DataMember] public int TicketSeverityID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public int? ParentID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime? DateClosed { get; set; }
    [DataMember] public int? CloserID { get; set; }
    [DataMember] public int DaysClosed { get; set; }
    [DataMember] public int MinutesClosed { get; set; }
    [DataMember] public int? DaysOpened { get; set; }
    [DataMember] public int? MinutesOpened { get; set; }
    [DataMember] public string CloserName { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember] public string ModifierName { get; set; }
    [DataMember] public double? HoursSpent { get; set; }
    [DataMember] public string Tags { get; set; }
    [DataMember] public int? SlaViolationTime { get; set; }
    [DataMember] public int? SlaWarningTime { get; set; }
    [DataMember] public double? SlaViolationHours { get; set; }
    [DataMember] public double? SlaWarningHours { get; set; }
    [DataMember] public int? MinsSinceCreated { get; set; }
    [DataMember] public int? DaysSinceCreated { get; set; }
    [DataMember] public int? MinsSinceModified { get; set; }
    [DataMember] public int? DaysSinceModified { get; set; }
    [DataMember] public string Contacts { get; set; }
    [DataMember] public string Customers { get; set; }
    [DataMember] public DateTime? SlaViolationTimeClosed { get; set; }
    [DataMember] public DateTime? SlaViolationLastAction { get; set; }
    [DataMember] public DateTime? SlaViolationInitialResponse { get; set; }
    [DataMember] public DateTime? SlaWarningTimeClosed { get; set; }
    [DataMember] public DateTime? SlaWarningLastAction { get; set; }
    [DataMember] public DateTime? SlaWarningInitialResponse { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public DateTime? SlaViolationDate { get; set; }
    [DataMember] public DateTime? SlaWarningDate { get; set; }
    [DataMember] public string TicketSource { get; set; }
    [DataMember] public int? ForumCategory { get; set; }
    [DataMember] public string CategoryName { get; set; }
    [DataMember] public string CreatorEmail { get; set; }
    [DataMember] public string ModifierEmail { get; set; }
    [DataMember] public int? KnowledgeBaseCategoryID { get; set; }
    [DataMember] public string KnowledgeBaseCategoryName { get; set; }
    [DataMember] public string KnowledgeBaseParentCategoryName { get; set; }
    [DataMember] public DateTime? DueDate { get; set; }
    [DataMember] public string SalesForceID { get; set; }
    [DataMember] public DateTime? DateModifiedBySalesForceSync { get; set; }
    [DataMember] public DateTime? DateModifiedByJiraSync { get; set; }
    [DataMember] public int? JiraID { get; set; }
    [DataMember] public bool? SyncWithJira { get; set; }
    [DataMember] public string JiraKey { get; set; }
    [DataMember] public string JiraLinkURL { get; set; }
    [DataMember] public string JiraStatus { get; set; }
    [DataMember] public int? ChildTicketCount { get; set; }
    [DataMember] public int? RelatedTicketCount { get; set; }
    [DataMember] public int? MinutesToInitialResponse { get; set; }
    [DataMember] public string DayOfWeekCreated { get; set; }
    [DataMember] public int? HourOfDayCreated { get; set; }
          
  }
}
