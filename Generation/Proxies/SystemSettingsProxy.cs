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
  [KnownType(typeof(SystemSettingProxy))]
  public class SystemSettingProxy
  {
    public SystemSettingProxy() {}
    [DataMember] public int SystemSettingID { get; set; }
    [DataMember] public string SettingKey { get; set; }
    [DataMember] public string SettingValue { get; set; }
          
  }
  
  public partial class SystemSetting : BaseItem
  {
    public SystemSettingProxy GetProxy()
    {
      SystemSettingProxy result = new SystemSettingProxy();
      result.SettingValue = this.SettingValue;
      result.SettingKey = this.SettingKey;
      result.SystemSettingID = this.SystemSettingID;
       
       
       
      return result;
    }	
  }
}
