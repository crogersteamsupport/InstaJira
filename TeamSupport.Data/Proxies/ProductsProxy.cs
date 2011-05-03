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
  [KnownType(typeof(ProductProxy))]
  public class ProductProxy
  {
    public ProductProxy() {}
    [DataMember] public int ProductID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class Product : BaseItem
  {
    public ProductProxy GetProxy()
    {
      ProductProxy result = new ProductProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.Description = this.Description;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.ProductID = this.ProductID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreated, DateTimeKind.Local);
      result.DateModified = DateTime.SpecifyKind(this.DateModified, DateTimeKind.Local);
       
       
      return result;
    }	
  }
}
