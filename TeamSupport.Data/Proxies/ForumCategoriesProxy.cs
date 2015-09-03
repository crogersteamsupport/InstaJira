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
          
  }
  
  public partial class ForumCategory : BaseItem
  {
    public ForumCategoryProxy GetProxy()
    {
      var sanitizer = new HtmlSanitizer();
      sanitizer.AllowedAttributes.Add("class");
      sanitizer.AllowedAttributes.Add("id");

      ForumCategoryProxy result = new ForumCategoryProxy();
      result.ProductID = this.ProductID;
      result.GroupID = this.GroupID;
      result.TicketType = this.TicketType;
      result.Position = this.Position;
      result.OrganizationID = this.OrganizationID;
      result.CategoryDesc = sanitizer.Sanitize(this.CategoryDesc);
      result.CategoryName = sanitizer.Sanitize(this.CategoryName);
      result.ParentID = this.ParentID;
      result.CategoryID = this.CategoryID;
       
       
       
      return result;
    }	
  }
}
