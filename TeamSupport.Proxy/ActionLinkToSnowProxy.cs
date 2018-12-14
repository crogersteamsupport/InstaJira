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
  [KnownType(typeof(ActionLinkToSnowItemProxy))]
  public class ActionLinkToSnowItemProxy
  {
    public ActionLinkToSnowItemProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int ActionID { get; set; }
    [DataMember] public DateTime? DateModifiedBySync { get; set; }
    [DataMember] public string AppId { get; set; }
          
  }
}
