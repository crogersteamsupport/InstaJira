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
  [KnownType(typeof(DeflectionLogItemProxy))]
  public class DeflectionLogItemProxy
  {
    public DeflectionLogItemProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public string Source { get; set; }
    [DataMember] public bool Helpful { get; set; }
    [DataMember] public DateTime Date { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public int? OrgID { get; set; }
          
  }
}
