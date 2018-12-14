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
  [KnownType(typeof(TicketTimeLineViewItemProxy))]
  public class TicketTimeLineViewItemProxy
  {
    public TicketTimeLineViewItemProxy() {}
    [DataMember] public int? TicketID { get; set; }
    [DataMember] public int? TicketNumber { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public bool IsWC { get; set; }
    [DataMember] public string MessageType { get; set; }
    [DataMember] public int? ActionTypeID { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime? DateStarted { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public string CreatorName { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public bool IsKnowledgeBase { get; set; }
    [DataMember] public bool IsPinned { get; set; }
    [DataMember] public int? WCUserID { get; set; }
    [DataMember] public int? TimeSpent { get; set; }
          
  }
  
  public partial class TicketTimeLineViewItem : BaseItem
  {
    public TicketTimeLineViewItemProxy GetProxy()
    {
      TicketTimeLineViewItemProxy result = new TicketTimeLineViewItemProxy();
      result.WCUserID = this.WCUserID;
      result.IsPinned = this.IsPinned;
      result.IsKnowledgeBase = this.IsKnowledgeBase;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.CreatorName = this.CreatorName;
      result.CreatorID = this.CreatorID;
      result.OrganizationID = this.OrganizationID;
      result.Message = this.Message;
      result.MessageType = this.MessageType == null ? "Description" : this.MessageType;
      result.ActionTypeID = this.ActionTypeID;
      result.IsWC = this.IsWC;
      result.RefID = this.RefID;
      result.TicketNumber = this.TicketNumber;
      result.TicketID = this.TicketID;
      result.TimeSpent = this.TimeSpent;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateStarted = this.DateStartedUtc == null ? this.DateStartedUtc : DateTime.SpecifyKind((DateTime)this.DateStartedUtc, DateTimeKind.Utc); 

       
      return result;
    }	
  }
}
