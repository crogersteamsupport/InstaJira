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
  [KnownType(typeof(ForumPermissionProxy))]
  public class ForumPermissionProxy
  {
    public ForumPermissionProxy() {}
    [DataMember] public int CategoryID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int? FilterUserID { get; set; }
    [DataMember] public int? FilterOrgID { get; set; }
          
  }
  
  public partial class ForumPermission : BaseItem
  {
    public ForumPermissionProxy GetProxy()
    {
      ForumPermissionProxy result = new ForumPermissionProxy();
      result.FilterOrgID = this.FilterOrgID;
      result.FilterUserID = this.FilterUserID;
      result.OrganizationID = this.OrganizationID;
      result.CategoryID = this.CategoryID;
       
       
       
      return result;
    }	
  }
}
