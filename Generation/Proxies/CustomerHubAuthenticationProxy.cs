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
  [KnownType(typeof(CustomerHubAuthenticationItemProxy))]
  public class CustomerHubAuthenticationItemProxy
  {
    public CustomerHubAuthenticationItemProxy() {}
    [DataMember] public int CustomerHubAuthenticationID { get; set; }
    [DataMember] public int CustomerHubID { get; set; }
    [DataMember] public bool EnableSelfRegister { get; set; }
    [DataMember] public bool EnableRequestAccess { get; set; }
    [DataMember] public bool EnableSSO { get; set; }
          
  }
  
  public partial class CustomerHubAuthenticationItem : BaseItem
  {
    public CustomerHubAuthenticationItemProxy GetProxy()
    {
      CustomerHubAuthenticationItemProxy result = new CustomerHubAuthenticationItemProxy();
      result.EnableSSO = this.EnableSSO;
      result.EnableRequestAccess = this.EnableRequestAccess;
      result.EnableSelfRegister = this.EnableSelfRegister;
      result.CustomerHubID = this.CustomerHubID;
      result.CustomerHubAuthenticationID = this.CustomerHubAuthenticationID;
       
       
       
      return result;
    }	
  }
}
