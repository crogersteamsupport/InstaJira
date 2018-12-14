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
  [KnownType(typeof(WikiArticleProxy))]
  public class WikiArticleProxy
  {
    public WikiArticleProxy() {}
    [DataMember] public int ArticleID { get; set; }
    [DataMember] public int? ParentID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string ArticleName { get; set; }
    [DataMember] public string Body { get; set; }
    [DataMember] public int? Version { get; set; }
    [DataMember] public bool? PublicView { get; set; }
    [DataMember] public bool? PublicEdit { get; set; }
    [DataMember] public bool? PortalView { get; set; }
    [DataMember] public bool? PortalEdit { get; set; }
    [DataMember] public bool? Private { get; set; }
    [DataMember] public bool? IsDeleted { get; set; }
    [DataMember] public int? CreatedBy { get; set; }
    [DataMember] public DateTime? CreatedDate { get; set; }
    [DataMember] public int? ModifiedBy { get; set; }
    [DataMember] public DateTime? ModifiedDate { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public bool IsOwner { get; set; }
    [DataMember] public bool CanDelete { get; set; }  
   
   
         
  }
}
