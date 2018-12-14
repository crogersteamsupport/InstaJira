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
  [KnownType(typeof(ForumCategoryProxy))]
  public class ForumCategoryProxy
  {
    public ForumCategoryProxy() {}
    [DataMember] public int CategoryID { get; set; }
    [DataMember] public int ParentID { get; set; }
    [DataMember] public string CategoryName { get; set; }
    [DataMember] public string CategoryDesc { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int? Position { get; set; }
    [DataMember] public int? TicketType { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public int? ProductID { get; set; }
    [DataMember] public int? ProductFamilyID { get; set; }
          
  }
  
  public partial class ForumCategory : BaseItem
  {
    public ForumCategoryProxy GetProxy()
    {
      ForumCategoryProxy result = new ForumCategoryProxy();
      result.ProductFamilyID = this.ProductFamilyID;
      result.ProductID = this.ProductID;
      result.GroupID = this.GroupID;
      result.TicketType = this.TicketType;
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
