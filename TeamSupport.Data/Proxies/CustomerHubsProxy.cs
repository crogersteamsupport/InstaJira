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
  [KnownType(typeof(CustomerHubProxy))]
  public class CustomerHubProxy
  {
    public CustomerHubProxy() {}
    [DataMember] public int CustomerHubID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string PortalName { get; set; }
    [DataMember] public string CNameURL { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public bool EnableMigration { get; set; }
          
  }
  
  public partial class CustomerHub : BaseItem
  {
    public CustomerHubProxy GetProxy()
    {
      CustomerHubProxy result = new CustomerHubProxy();
      result.EnableMigration = this.EnableMigration;
      result.ModifierID = this.ModifierID;
      result.ProductFamilyID = this.ProductFamilyID;
      result.IsActive = this.IsActive;
      result.CNameURL = this.CNameURL;
      result.PortalName = this.PortalName;
      result.OrganizationID = this.OrganizationID;
      result.CustomerHubID = this.CustomerHubID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
