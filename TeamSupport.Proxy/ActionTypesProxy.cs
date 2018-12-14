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
  [KnownType(typeof(ActionTypeProxy))]
  public class ActionTypeProxy
  {
    public ActionTypeProxy() {}
    [DataMember] public int ActionTypeID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public bool IsTimed { get; set; }
    [DataMember] public int Position { get; set; }
  }
}
