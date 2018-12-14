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
  [KnownType(typeof(TicketLinkToSnowItemProxy))]
  public class TicketLinkToSnowItemProxy
  {
    public TicketLinkToSnowItemProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public DateTime? DateModifiedBySync { get; set; }
    [DataMember] public bool Sync { get; set; }
    [DataMember] public string AppId { get; set; }
    [DataMember] public string Number { get; set; }
    [DataMember] public string URL { get; set; }
    [DataMember] public string State { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? CrmLinkID { get; set; }
          
  }
}
