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
  [KnownType(typeof(ProductVersionsViewItemProxy))]
  public class ProductVersionsViewItemProxy
  {
    public ProductVersionsViewItemProxy() {}
    [DataMember] public int ProductVersionID { get; set; }
    [DataMember] public int ProductID { get; set; }
    [DataMember] public int ProductVersionStatusID { get; set; }
    [DataMember] public string VersionNumber { get; set; }
    [DataMember] public DateTime? ReleaseDate { get; set; }
    [DataMember]
    public DateTime? ReleaseDateUTC { get; set; }
    [DataMember] public bool IsReleased { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public string VersionStatus { get; set; }
    [DataMember] public string ProductName { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
    [DataMember] public string JiraProjectKey { get; set; }
    [DataMember] public string TFSProjectName { get; set; }
          
  }
  
  public partial class ProductVersionsViewItem : BaseItem
  {
    public ProductVersionsViewItemProxy GetProxy()
    {
      ProductVersionsViewItemProxy result = new ProductVersionsViewItemProxy();
      result.TFSProjectName = this.TFSProjectName;
      result.JiraProjectKey = this.JiraProjectKey;
      result.ProductFamilyID = this.ProductFamilyID;
      result.OrganizationID = this.OrganizationID;
      result.ProductName = (this.ProductName);
      result.VersionStatus = this.VersionStatus;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.Description = (this.Description);
      result.IsReleased = this.IsReleased;
      result.VersionNumber = this.VersionNumber;
      result.ProductVersionStatusID = this.ProductVersionStatusID;
      result.ProductID = this.ProductID;
      result.ProductVersionID = this.ProductVersionID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.ReleaseDate = this.ReleaseDateUtc == null ? this.ReleaseDateUtc : DateTime.SpecifyKind((DateTime)this.ReleaseDateUtc, DateTimeKind.Utc);
      result.ReleaseDateUTC = this.ReleaseDateUtc;
      return result;
    }	
  }
}
