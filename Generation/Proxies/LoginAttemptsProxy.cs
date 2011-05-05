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
  [KnownType(typeof(LoginAttemptProxy))]
  public class LoginAttemptProxy
  {
    public LoginAttemptProxy() {}
    [DataMember] public int LoginAttemptID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public bool Successful { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class LoginAttempt : BaseItem
  {
    public LoginAttemptProxy GetProxy()
    {
      LoginAttemptProxy result = new LoginAttemptProxy();
      result.Successful = this.Successful;
      result.UserID = this.UserID;
      result.LoginAttemptID = this.LoginAttemptID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreated, DateTimeKind.Local);
       
       
      return result;
    }	
  }
}
