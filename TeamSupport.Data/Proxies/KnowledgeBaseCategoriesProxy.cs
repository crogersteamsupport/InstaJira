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
  [KnownType(typeof(KnowledgeBaseCategoryProxy))]
  public class KnowledgeBaseCategoryProxy
  {
    public KnowledgeBaseCategoryProxy() {}
    [DataMember] public int CategoryID { get; set; }
    [DataMember] public int ParentID { get; set; }
    [DataMember] public string CategoryName { get; set; }
    [DataMember] public string CategoryDesc { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int? Position { get; set; }
    [DataMember] public bool? VisibleOnPortal { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
          
  }
  
  public partial class KnowledgeBaseCategory : BaseItem
  {
    public KnowledgeBaseCategoryProxy GetProxy()
    {
      KnowledgeBaseCategoryProxy result = new KnowledgeBaseCategoryProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.VisibleOnPortal = this.VisibleOnPortal;
      result.Position = this.Position;
      result.OrganizationID = this.OrganizationID;
      result.CategoryDesc = this.CategoryDesc;
      result.CategoryName = this.CategoryName;
      result.ParentID = this.ParentID;
      result.CategoryID = this.CategoryID;
       
       
       
      return result;
    }	
  }
}
