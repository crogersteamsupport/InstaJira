using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Ganss.XSS;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(OrganizationProductsViewItemProxy))]
  public class OrganizationProductsViewItemProxy
  {
    public OrganizationProductsViewItemProxy() {}
    [DataMember] public string Product { get; set; }
    [DataMember] public string VersionStatus { get; set; }
    [DataMember] public bool? IsShipping { get; set; }
    [DataMember] public bool? IsDiscontinued { get; set; }
    [DataMember] public string VersionNumber { get; set; }
    [DataMember] public int? ProductVersionStatusID { get; set; }
    [DataMember] public DateTime? ReleaseDate { get; set; }
    [DataMember] public bool? IsReleased { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int OrganizationProductID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string OrganizationName { get; set; }
    [DataMember] public int ProductID { get; set; }
    [DataMember] public int? ProductVersionID { get; set; }
    [DataMember] public bool IsVisibleOnPortal { get; set; }
    [DataMember] public DateTime? SupportExpiration { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class OrganizationProductsViewItem : BaseItem
  {
    public OrganizationProductsViewItemProxy GetProxy()
    {
      OrganizationProductsViewItemProxy result = new OrganizationProductsViewItemProxy();
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.IsVisibleOnPortal = this.IsVisibleOnPortal;
      result.ProductVersionID = this.ProductVersionID;
      result.ProductID = this.ProductID;
      result.OrganizationName = (this.OrganizationName);
      result.OrganizationID = this.OrganizationID;
      result.OrganizationProductID = this.OrganizationProductID;
      result.Description = (this.Description);
      result.IsReleased = this.IsReleased;
      result.ProductVersionStatusID = this.ProductVersionStatusID;
      result.VersionNumber = this.VersionNumber;
      result.IsDiscontinued = this.IsDiscontinued;
      result.IsShipping = this.IsShipping;
      result.VersionStatus = this.VersionStatus;
      result.Product = this.Product;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.SupportExpiration = this.SupportExpirationUtc == null ? this.SupportExpirationUtc : DateTime.SpecifyKind((DateTime)this.SupportExpirationUtc, DateTimeKind.Utc); 
      result.ReleaseDate = this.ReleaseDateUtc == null ? this.ReleaseDateUtc : DateTime.SpecifyKind((DateTime)this.ReleaseDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
