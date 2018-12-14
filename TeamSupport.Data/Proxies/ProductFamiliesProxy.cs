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
  [KnownType(typeof(ProductFamilyProxy))]
  public class ProductFamilyProxy
  {
    public ProductFamilyProxy() {}
    [DataMember] public int ProductFamilyID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int NeedsIndexing { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class ProductFamily : BaseItem
  {
    public ProductFamilyProxy GetProxy()
    {
      ProductFamilyProxy result = new ProductFamilyProxy();
      result.ImportFileID = this.ImportFileID;
      result.ImportID = this.ImportID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.OrganizationID = this.OrganizationID;
      result.ProductFamilyID = this.ProductFamilyID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
