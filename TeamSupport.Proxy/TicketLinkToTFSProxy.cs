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
  [KnownType(typeof(TicketLinkToTFSItemProxy))]
  public class TicketLinkToTFSItemProxy
  {
    public TicketLinkToTFSItemProxy() {}
    [DataMember] public int id { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public DateTime? DateModifiedByTFSSync { get; set; }
    [DataMember] public bool SyncWithTFS { get; set; }
    [DataMember] public int? TFSID { get; set; }
    [DataMember] public string TFSTitle { get; set; }
    [DataMember] public string TFSURL { get; set; }
    [DataMember] public string TFSState { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? CrmLinkID { get; set; }
          
  }
}
