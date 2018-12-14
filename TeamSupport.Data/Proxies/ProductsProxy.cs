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
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
    [DataMember] public string JiraProjectKey { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
    [DataMember] public string EmailReplyToAddress { get; set; }
    [DataMember] public int? SlaLevelID { get; set; }
    [DataMember] public string TFSProjectName { get; set; }
          
  }
  
  public partial class Product : BaseItem
  {
    public ProductProxy GetProxy()
    {
      ProductProxy result = new ProductProxy();
      result.TFSProjectName = this.TFSProjectName;
      result.SlaLevelID = this.SlaLevelID;
      result.EmailReplyToAddress = this.EmailReplyToAddress;
      result.ImportFileID = this.ImportFileID;
      result.JiraProjectKey = this.JiraProjectKey;
      result.ProductFamilyID = this.ProductFamilyID;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ImportID = this.ImportID;
      result.Description = this.Description;
      result.Name = this.Name;
      result.OrganizationID = this.OrganizationID;
      result.ProductID = this.ProductID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
