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
  [KnownType(typeof(ActionLogsViewItemProxy))]
  public class ActionLogsViewItemProxy
  {
    public ActionLogsViewItemProxy() {}
    [DataMember] public int ActionLogID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public ReferenceType RefType { get; set; }
    [DataMember] public int RefID { get; set; }
    [DataMember] public int ActionLogType { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public string Actor { get; set; }
          
  }
  
}
