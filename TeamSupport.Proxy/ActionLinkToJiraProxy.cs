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
  [KnownType(typeof(ActionLinkToJiraItemProxy))]
  public class ActionLinkToJiraItemProxy
  {
    public ActionLinkToJiraItemProxy() {}
    [DataMember] public int id { get; set; }
    [DataMember] public int? ActionID { get; set; }
    [DataMember] public DateTime? DateModifiedByJiraSync { get; set; }
    [DataMember] public int? JiraID { get; set; }
          
  }
}
