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
  [KnownType(typeof(TicketAutomationActionProxy))]
  public class TicketAutomationActionProxy
  {
    public TicketAutomationActionProxy() {}
    [DataMember] public int TicketActionID { get; set; }
    [DataMember] public int TriggerID { get; set; }
    [DataMember] public int ActionID { get; set; }
    [DataMember] public string ActionValue { get; set; }
    [DataMember] public string ActionValue2 { get; set; }
          
  }
  
  public partial class TicketAutomationAction : BaseItem
  {
    public TicketAutomationActionProxy GetProxy()
    {
      TicketAutomationActionProxy result = new TicketAutomationActionProxy();
      result.ActionValue2 = this.ActionValue2;
      result.ActionValue = this.ActionValue;
      result.ActionID = this.ActionID;
      result.TriggerID = this.TriggerID;
      result.TicketActionID = this.TicketActionID;
       
       
       
      return result;
    }	
  }
}
