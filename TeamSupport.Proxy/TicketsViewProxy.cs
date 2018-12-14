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
  [KnownType(typeof(TicketsViewItemProxy))]
  public class TicketsViewItemProxy
  {
    public TicketsViewItemProxy() {}
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
    [DataMember] public int? DaysOpened { get; set; }
    [DataMember] public string CloserName { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember] public string ModifierName { get; set; }
    [DataMember] public decimal? HoursSpent { get; set; }
    [DataMember] public string Tags { get; set; }
    [DataMember] public int? SlaViolationTime { get; set; }
    [DataMember] public int? SlaWarningTime { get; set; }
    [DataMember] public decimal? SlaViolationHours { get; set; }
    [DataMember] public decimal? SlaWarningHours { get; set; }
    [DataMember] public string Contacts { get; set; }
    [DataMember] public string Customers { get; set; }
    [DataMember] public DateTime? SlaViolationDate { get; set; }
    [DataMember] public DateTime? SlaWarningDate { get; set; }
    [DataMember] public bool IsRead { get; set; }
    [DataMember] public bool IsFlagged { get; set; }
    [DataMember] public bool IsSubscribed { get; set; }
    [DataMember] public bool IsEnqueued { get; set; }
    [DataMember] public int? ViewerID { get; set; }
    [DataMember] public string TicketSource { get; set; }
    [DataMember] public int? ForumCategory { get; set; }
    [DataMember] public string CategoryName { get; set; }
    [DataMember] public int? KnowledgeBaseCategoryID { get; set; }
    [DataMember] public string KnowledgeBaseCategoryName { get; set; }
    [DataMember] public string KnowledgeBaseCategoryDisplayString { get; set; }
    [DataMember] public string CategoryDisplayString { get; set; }
    [DataMember] public string SalesForceID { get; set; }          
    [DataMember] public DateTime? DateModifiedBySalesForceSync { get; set; }
    [DataMember] public DateTime? DueDate { get; set; }          
    [DataMember] public int? ProductFamilyID { get; set; }
  }
}
