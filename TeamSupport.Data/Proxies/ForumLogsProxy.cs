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
  
  public partial class ForumLog : BaseItem
  {
    public ForumLogProxy GetProxy()
    {
      ForumLogProxy result = new ForumLogProxy();
      result.OrgID = this.OrgID;
      result.UserID = this.UserID;
      result.TopicID = this.TopicID;
      result.ForumLogID = this.ForumLogID;
       
      result.ViewTime = DateTime.SpecifyKind(this.ViewTimeUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
