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
  [KnownType(typeof(OrganizationSettingProxy))]
  public class OrganizationSettingProxy
  {
    public OrganizationSettingProxy() {}
    [DataMember] public int OrganizationSettingID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string SettingKey { get; set; }
    [DataMember] public string SettingValue { get; set; }
          
  }
  
  public partial class OrganizationSetting : BaseItem
  {
    public OrganizationSettingProxy GetProxy()
    {
      OrganizationSettingProxy result = new OrganizationSettingProxy();
      result.SettingValue = this.SettingValue;
      result.SettingKey = this.SettingKey;
      result.OrganizationID = this.OrganizationID;
      result.OrganizationSettingID = this.OrganizationSettingID;
       
       
       
      return result;
    }	
  }
}
