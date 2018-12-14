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
  [KnownType(typeof(ActionsViewItemProxy))]
  public class ActionsViewItemProxy
  {
    public ActionsViewItemProxy() {}
    [DataMember] public int ActionID { get; set; }
    [DataMember] public int? ActionTypeID { get; set; }
    [DataMember] public SystemActionType SystemActionTypeID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int? TimeSpent { get; set; }
    [DataMember] public DateTime? DateStarted { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgeBase { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember] public string ModifierName { get; set; }
    [DataMember] public string ActionType { get; set; }
    /*
    [DataMember] public string ProductName { get; set; }
    [DataMember] public string ReportedVersion { get; set; }
    [DataMember] public string SolvedVersion { get; set; }
    [DataMember] public string GroupName { get; set; }
    [DataMember] public string TicketType { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public string Status { get; set; }
    [DataMember] public int? StatusPosition { get; set; }
    [DataMember] public int? SeverityPosition { get; set; }
    [DataMember] public bool IsClosed { get; set; }
    [DataMember] public string Severity { get; set; }
    [DataMember] public int? TicketNumber { get; set; }
    [DataMember] public int? ReportedVersionID { get; set; }
    [DataMember] public int? SolvedVersionID { get; set; }
    [DataMember] public int? ProductID { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public int? TicketStatusID { get; set; }
    [DataMember] public int? TicketTypeID { get; set; }
    [DataMember] public int? TicketSeverityID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public string TicketName { get; set; }
    [DataMember] public DateTime? DateClosed { get; set; }
    [DataMember] public int? CloserID { get; set; }
    [DataMember] public int DaysClosed { get; set; }
    [DataMember] public int? DaysOpened { get; set; }
    [DataMember] public string CloserName { get; set; }
    [DataMember] public decimal? HoursSpent { get; set; }*/
    [DataMember] public string DisplayName { get; set; }
          
  }
  
  public partial class ActionsViewItem : BaseItem
  {
    public ActionsViewItemProxy GetProxy()
    {
      ActionsViewItemProxy result = new ActionsViewItemProxy();
     /* result.HoursSpent = this.HoursSpent;
      result.CloserName = this.CloserName;
      result.DaysOpened = this.DaysOpened;
      result.DaysClosed = this.DaysClosed;
      result.CloserID = this.CloserID;
      result.TicketName = this.TicketName;
      result.OrganizationID = this.OrganizationID;
      result.TicketSeverityID = this.TicketSeverityID;
      result.TicketTypeID = this.TicketTypeID;
      result.TicketStatusID = this.TicketStatusID;
      result.UserID = this.UserID;
      result.GroupID = this.GroupID;
      result.ProductID = this.ProductID;
      result.SolvedVersionID = this.SolvedVersionID;
      result.ReportedVersionID = this.ReportedVersionID;
      result.TicketNumber = this.TicketNumber;
      result.Severity = this.Severity;
      result.IsClosed = this.IsClosed;
      result.SeverityPosition = this.SeverityPosition;
      result.StatusPosition = this.StatusPosition;
      result.Status = this.Status;
      result.UserName = this.UserName;
      result.TicketType = this.TicketType;
      result.GroupName = this.GroupName;
      result.SolvedVersion = this.SolvedVersion;
      result.ReportedVersion = this.ReportedVersion;
      result.ProductName = this.ProductName;*/
      result.ActionType = this.ActionType;
      result.ModifierName = this.ModifierName;
      result.CreatorName = this.CreatorName;
      result.TicketID = this.TicketID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.TimeSpent = this.TimeSpent;
      result.Description = this.Description;
      result.Name = this.Name;
      result.SystemActionTypeID = this.SystemActionTypeID;
      result.ActionTypeID = this.ActionTypeID;
      result.ActionID = this.ActionID;
      result.DisplayName = this.DisplayName;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      //result.DateClosed = this.DateClosedUtc == null ? this.DateClosedUtc : DateTime.SpecifyKind((DateTime)this.DateClosedUtc, DateTimeKind.Utc); 
      result.DateStarted = this.DateStartedUtc == null ? this.DateStartedUtc : DateTime.SpecifyKind((DateTime)this.DateStartedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
