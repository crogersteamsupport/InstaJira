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
  [KnownType(typeof(PluginProxy))]
  public class PluginProxy
  {
    public PluginProxy() {}
    [DataMember] public int PluginID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Code { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
          
  }
  
  public partial class Plugin : BaseItem
  {
    public PluginProxy GetProxy()
    {
      PluginProxy result = new PluginProxy();
      result.CreatorID = this.CreatorID;
      result.Code = this.Code;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.PluginID = this.PluginID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
