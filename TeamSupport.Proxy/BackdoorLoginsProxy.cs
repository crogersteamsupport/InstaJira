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
  [KnownType(typeof(BackdoorLoginProxy))]
  public class BackdoorLoginProxy
  {
    public BackdoorLoginProxy() {}
    [DataMember] public int BackdoorLoginID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public Guid Token { get; set; }
    [DataMember] public DateTime DateIssued { get; set; }
    [DataMember] public int ContactID { get; set; }
          
  }
}
