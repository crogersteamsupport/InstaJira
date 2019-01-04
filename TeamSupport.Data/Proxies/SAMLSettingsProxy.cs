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
  [KnownType(typeof(SAMLSettingProxy))]
  public class SAMLSettingProxy
  {
    public SAMLSettingProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public bool IsEnabled { get; set; }
    [DataMember] public string IdPUrl { get; set; }
    [DataMember] public string Certificate { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class SAMLSetting : BaseItem
  {
    public SAMLSettingProxy GetProxy()
    {
      SAMLSettingProxy result = new SAMLSettingProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Certificate = this.Certificate;
      result.IdPUrl = this.IdPUrl;
      result.IsEnabled = this.IsEnabled;
      result.OrganizationID = this.OrganizationID;
      result.Id = this.Id;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
