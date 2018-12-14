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
  [KnownType(typeof(OrganizationProductProxy))]
  public class OrganizationProductProxy
  {
    public OrganizationProductProxy() {}
    [DataMember] public int OrganizationProductID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int ProductID { get; set; }
    [DataMember] public int? ProductVersionID { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public DateTime? SupportExpiration { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class OrganizationProduct : BaseItem
  {
    public OrganizationProductProxy GetProxy()
    {
      OrganizationProductProxy result = new OrganizationProductProxy();
      result.ImportFileID = this.ImportFileID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.ProductVersionID = this.ProductVersionID;
      result.ProductID = this.ProductID;
      result.OrganizationID = this.OrganizationID;
      result.OrganizationProductID = this.OrganizationProductID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.SupportExpiration = this.SupportExpirationUtc == null ? this.SupportExpirationUtc : DateTime.SpecifyKind((DateTime)this.SupportExpirationUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
