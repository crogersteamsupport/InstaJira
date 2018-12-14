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
  [KnownType(typeof(AgentRatingUserProxy))]
  public class AgentRatingUserProxy
  {
    public AgentRatingUserProxy() {}
    [DataMember] public int AgentRatingID { get; set; }
    [DataMember] public int UserID { get; set; }
          
  }
  
  public partial class AgentRatingUser : BaseItem
  {
    public AgentRatingUserProxy GetProxy()
    {
      AgentRatingUserProxy result = new AgentRatingUserProxy();
      result.UserID = this.UserID;
      result.AgentRatingID = this.AgentRatingID;
       
       
       
      return result;
    }	
  }
}
