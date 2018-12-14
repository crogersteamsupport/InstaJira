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
  [KnownType(typeof(PhoneOptionProxy))]
  public class PhoneOptionProxy
  {
    public PhoneOptionProxy() {}
    [DataMember] public int PhoneOptionID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int? PhoneActive { get; set; }
    [DataMember] public string AccountSID { get; set; }
    [DataMember] public string AccountToken { get; set; }
    [DataMember] public string WelcomeAudioURL { get; set; }
          
  }
  
  public partial class PhoneOption : BaseItem
  {
    public PhoneOptionProxy GetProxy()
    {
      PhoneOptionProxy result = new PhoneOptionProxy();
      result.WelcomeAudioURL = this.WelcomeAudioURL;
      result.AccountToken = this.AccountToken;
      result.AccountSID = this.AccountSID;
      result.PhoneActive = this.PhoneActive;
      result.OrganizationID = this.OrganizationID;
      result.PhoneOptionID = this.PhoneOptionID;
       
       
       
      return result;
    }	
  }
}
