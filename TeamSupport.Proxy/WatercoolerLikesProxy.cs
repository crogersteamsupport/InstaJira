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
  [KnownType(typeof(WatercoolerLikeProxy))]
  public class WatercoolerLikeProxy
  {
    public WatercoolerLikeProxy() {}
    [DataMember] public int MessageID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
}
