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
}
