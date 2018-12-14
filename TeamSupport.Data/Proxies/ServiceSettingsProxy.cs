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
  [KnownType(typeof(ServiceSettingProxy))]
  public class ServiceSettingProxy
  {
    public ServiceSettingProxy() {}
    [DataMember] public int ServiceSettingID { get; set; }
    [DataMember] public int ServiceID { get; set; }
    [DataMember] public string SettingKey { get; set; }
    [DataMember] public string SettingValue { get; set; }
          
  }
  
  public partial class ServiceSetting : BaseItem
  {
    public ServiceSettingProxy GetProxy()
    {
      ServiceSettingProxy result = new ServiceSettingProxy();
      result.SettingValue = this.SettingValue;
      result.SettingKey = this.SettingKey;
      result.ServiceID = this.ServiceID;
      result.ServiceSettingID = this.ServiceSettingID;
       
       
       
      return result;
    }	
  }
}
