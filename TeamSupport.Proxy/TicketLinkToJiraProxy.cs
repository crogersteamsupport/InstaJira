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
  [KnownType(typeof(TicketLinkToJiraItemProxy))]
  public class TicketLinkToJiraItemProxy
  {
    public TicketLinkToJiraItemProxy() {}
    [DataMember] public int id { get; set; }
    [DataMember] public int? TicketID { get; set; }
    [DataMember] public DateTime? DateModifiedByJiraSync { get; set; }
    [DataMember] public bool? SyncWithJira { get; set; }
    [DataMember] public int? JiraID { get; set; }
    [DataMember] public string JiraKey { get; set; }
    [DataMember] public string JiraLinkURL { get; set; }
    [DataMember] public string JiraStatus { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? CrmLinkID { get; set; }
          
  }
}
