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
  [KnownType(typeof(TicketProxy))]
  public class TicketProxy
  {
    public TicketProxy() {}
    [DataMember] public int TicketID { get; set; }
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
    [DataMember] public int TicketNumber { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgeBase { get; set; }
    [DataMember] public DateTime? DateClosed { get; set; }
    [DataMember] public int? CloserID { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public DateTime? LastViolationTime { get; set; }
    [DataMember] public DateTime? LastWarningTime { get; set; }
    [DataMember] public string TicketSource { get; set; }
    [DataMember] public string PortalEmail { get; set; }
    [DataMember] public DateTime? SlaViolationTimeClosed { get; set; }
    [DataMember] public DateTime? SlaViolationLastAction { get; set; }
    [DataMember] public DateTime? SlaViolationInitialResponse { get; set; }
    [DataMember] public DateTime? SlaWarningTimeClosed { get; set; }
    [DataMember] public DateTime? SlaWarningLastAction { get; set; }
    [DataMember] public DateTime? SlaWarningInitialResponse { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public int? DocID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public DateTime? DueDate { get; set; }
    [DataMember] public int? KnowledgeBaseCategoryID { get; set; }
    [DataMember] public DateTime? DateModifiedBySalesForceSync { get; set; }
    [DataMember] public string SalesForceID { get; set; }
    [DataMember] public string JiraStatus { get; set; }
    [DataMember] public DateTime? DateModifiedByJiraSync { get; set; }
    [DataMember] public bool? SyncWithJira { get; set; }
    [DataMember] public int? JiraID { get; set; }
    [DataMember] public string JiraKey { get; set; }
    [DataMember] public string JiraLinkURL { get; set; }
    [DataMember] public string EmailReplyToAddress { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class Ticket : BaseItem
  {
    public TicketProxy GetProxy()
    {
      TicketProxy result = new TicketProxy();
      result.ImportFileID = this.ImportFileID;
      result.EmailReplyToAddress = this.EmailReplyToAddress;
      result.JiraLinkURL = this.JiraLinkURL;
      result.JiraKey = this.JiraKey;
      result.JiraID = this.JiraID;
      result.SyncWithJira = this.SyncWithJira;
      result.JiraStatus = this.JiraStatus;
      result.SalesForceID = this.SalesForceID;
      result.KnowledgeBaseCategoryID = this.KnowledgeBaseCategoryID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.DocID = this.DocID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.PortalEmail = this.PortalEmail;
      result.TicketSource = this.TicketSource;
      result.ImportID = this.ImportID;
      result.CloserID = this.CloserID;
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TicketNumber = this.TicketNumber;
      result.ParentID = this.ParentID;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.TicketSeverityID = this.TicketSeverityID;
      result.TicketTypeID = this.TicketTypeID;
      result.TicketStatusID = this.TicketStatusID;
      result.UserID = this.UserID;
      result.GroupID = this.GroupID;
      result.ProductID = this.ProductID;
      result.SolvedVersionID = this.SolvedVersionID;
      result.ReportedVersionID = this.ReportedVersionID;
      result.TicketID = this.TicketID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.DateModifiedByJiraSync = this.DateModifiedByJiraSyncUtc == null ? this.DateModifiedByJiraSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedByJiraSyncUtc, DateTimeKind.Utc); 
      result.DateModifiedBySalesForceSync = this.DateModifiedBySalesForceSyncUtc == null ? this.DateModifiedBySalesForceSyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedBySalesForceSyncUtc, DateTimeKind.Utc); 
      result.DueDate = this.DueDateUtc == null ? this.DueDateUtc : DateTime.SpecifyKind((DateTime)this.DueDateUtc, DateTimeKind.Utc); 
      result.SlaWarningInitialResponse = this.SlaWarningInitialResponseUtc == null ? this.SlaWarningInitialResponseUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningInitialResponseUtc, DateTimeKind.Utc); 
      result.SlaWarningLastAction = this.SlaWarningLastActionUtc == null ? this.SlaWarningLastActionUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningLastActionUtc, DateTimeKind.Utc); 
      result.SlaWarningTimeClosed = this.SlaWarningTimeClosedUtc == null ? this.SlaWarningTimeClosedUtc : DateTime.SpecifyKind((DateTime)this.SlaWarningTimeClosedUtc, DateTimeKind.Utc); 
      result.SlaViolationInitialResponse = this.SlaViolationInitialResponseUtc == null ? this.SlaViolationInitialResponseUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationInitialResponseUtc, DateTimeKind.Utc); 
      result.SlaViolationLastAction = this.SlaViolationLastActionUtc == null ? this.SlaViolationLastActionUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationLastActionUtc, DateTimeKind.Utc); 
      result.SlaViolationTimeClosed = this.SlaViolationTimeClosedUtc == null ? this.SlaViolationTimeClosedUtc : DateTime.SpecifyKind((DateTime)this.SlaViolationTimeClosedUtc, DateTimeKind.Utc); 
      result.LastWarningTime = this.LastWarningTimeUtc == null ? this.LastWarningTimeUtc : DateTime.SpecifyKind((DateTime)this.LastWarningTimeUtc, DateTimeKind.Utc); 
      result.LastViolationTime = this.LastViolationTimeUtc == null ? this.LastViolationTimeUtc : DateTime.SpecifyKind((DateTime)this.LastViolationTimeUtc, DateTimeKind.Utc); 
      result.DateClosed = this.DateClosedUtc == null ? this.DateClosedUtc : DateTime.SpecifyKind((DateTime)this.DateClosedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
