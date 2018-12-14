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
  [KnownType(typeof(TicketAutomationTriggerProxy))]
  public class TicketAutomationTriggerProxy
  {
    public TicketAutomationTriggerProxy() {}
    [DataMember] public int TriggerID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public bool Active { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool UseCustomSQL { get; set; }
    [DataMember] public string CustomSQL { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
}
