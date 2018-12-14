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
  [KnownType(typeof(CustomerHubDisplaySettingProxy))]
  public class CustomerHubDisplaySettingProxy
  {
    public CustomerHubDisplaySettingProxy() {}
    [DataMember] public int CustomerHubDisplaySettingID { get; set; }
    [DataMember] public int CustomerHubID { get; set; }
    [DataMember] public string FontFamily { get; set; }
    [DataMember] public string FontColor { get; set; }
    [DataMember] public string Color1 { get; set; }
    [DataMember] public string Color2 { get; set; }
    [DataMember] public string Color3 { get; set; }
    [DataMember] public string Color4 { get; set; }
    [DataMember] public string Color5 { get; set; }
          
  }
  
  public partial class CustomerHubDisplaySetting : BaseItem
  {
    public CustomerHubDisplaySettingProxy GetProxy()
    {
      CustomerHubDisplaySettingProxy result = new CustomerHubDisplaySettingProxy();
      result.Color5 = this.Color5;
      result.Color4 = this.Color4;
      result.Color3 = this.Color3;
      result.Color2 = this.Color2;
      result.Color1 = this.Color1;
      result.FontColor = this.FontColor;
      result.FontFamily = this.FontFamily;
      result.CustomerHubID = this.CustomerHubID;
      result.CustomerHubDisplaySettingID = this.CustomerHubDisplaySettingID;
       
       
       
      return result;
    }	
  }
}
