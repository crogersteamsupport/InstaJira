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
  
  public partial class BackdoorLogin : BaseItem
  {
    public BackdoorLoginProxy GetProxy()
    {
      BackdoorLoginProxy result = new BackdoorLoginProxy();
      result.ContactID = this.ContactID;
      result.Token = this.Token;
      result.UserID = this.UserID;
      result.BackdoorLoginID = this.BackdoorLoginID;
       
      result.DateIssued = DateTime.SpecifyKind(this.DateIssuedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
