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
  [KnownType(typeof(SlaTicketProxy))]
  public class SlaTicketProxy
  {
    public SlaTicketProxy() {}
    [DataMember] public int TicketId { get; set; }
    [DataMember] public int SlaTriggerId { get; set; }
    [DataMember] public bool IsPending { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
          
  }
}
