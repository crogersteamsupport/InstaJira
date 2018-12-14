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
}
