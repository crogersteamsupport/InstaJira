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
  [KnownType(typeof(WatercoolerLikProxy))]
  public class WatercoolerLikProxy
  {
    public WatercoolerLikProxy() {}
    [DataMember] public int MessageID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class WatercoolerLike : BaseItem
  {
    public WatercoolerLikProxy GetProxy()
    {
      WatercoolerLikProxy result = new WatercoolerLikProxy();
      result.UserID = this.UserID;
      result.MessageID = this.MessageID;

      result.UserName = Users.GetUserFullName(BaseCollection.LoginUser, this.UserID);        
         
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
