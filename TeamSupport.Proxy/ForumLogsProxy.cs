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
  [KnownType(typeof(ForumLogProxy))]
  public class ForumLogProxy
  {
    public ForumLogProxy() {}
    [DataMember] public int ForumLogID { get; set; }
    [DataMember] public int TopicID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int OrgID { get; set; }
    [DataMember] public DateTime ViewTime { get; set; }
          
  }
}
