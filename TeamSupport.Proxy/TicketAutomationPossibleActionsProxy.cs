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
  [KnownType(typeof(TicketAutomationPossibleActionProxy))]
  public class TicketAutomationPossibleActionProxy
  {
    public TicketAutomationPossibleActionProxy() {}
    [DataMember] public int ActionID { get; set; }
    [DataMember] public string DisplayName { get; set; }
    [DataMember] public string ActionName { get; set; }
    [DataMember] public bool RequireValue { get; set; }
    [DataMember] public string ValueList { get; set; }
    [DataMember] public string ValueList2 { get; set; }
    [DataMember] public bool Active { get; set; }
          
  }
}
