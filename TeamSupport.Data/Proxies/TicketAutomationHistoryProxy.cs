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
  [KnownType(typeof(TicketAutomationHistoryItemProxy))]
  public class TicketAutomationHistoryItemProxy
  {
    public TicketAutomationHistoryItemProxy() {}
    [DataMember] public int HistoryID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int TriggerID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public DateTime? TriggerDateTime { get; set; }
    [DataMember] public string ActionType { get; set; }
          
  }
  
  public partial class TicketAutomationHistoryItem : BaseItem
  {
    public TicketAutomationHistoryItemProxy GetProxy()
    {
      TicketAutomationHistoryItemProxy result = new TicketAutomationHistoryItemProxy();
      result.ActionType = this.ActionType;
      result.OrganizationID = this.OrganizationID;
      result.TriggerID = this.TriggerID;
      result.TicketID = this.TicketID;
      result.HistoryID = this.HistoryID;
       
       
      result.TriggerDateTime = this.TriggerDateTimeUtc == null ? this.TriggerDateTimeUtc : DateTime.SpecifyKind((DateTime)this.TriggerDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
