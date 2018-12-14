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
  [KnownType(typeof(WikiArticlesViewItemProxy))]
  public class WikiArticlesViewItemProxy
  {
    public WikiArticlesViewItemProxy() {}
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
    [DataMember] public string Creator { get; set; }
    [DataMember] public string Modifier { get; set; }
    [DataMember] public string Organization { get; set; }
          
  }
  
  public partial class WikiArticlesViewItem : BaseItem
  {
    public WikiArticlesViewItemProxy GetProxy()
    {
      WikiArticlesViewItemProxy result = new WikiArticlesViewItemProxy();
      result.Organization = this.Organization;
      result.Modifier = this.Modifier;
      result.Creator = this.Creator;
      result.NeedsIndexing = this.NeedsIndexing;
      result.ModifiedBy = this.ModifiedBy;
      result.CreatedBy = this.CreatedBy;
      result.IsDeleted = this.IsDeleted;
      result.Private = this.Private;
      result.PortalEdit = this.PortalEdit;
      result.PortalView = this.PortalView;
      result.PublicEdit = this.PublicEdit;
      result.PublicView = this.PublicView;
      result.Version = this.Version;
      result.Body = (this.Body);
      result.ArticleName = (this.ArticleName);
      result.OrganizationID = this.OrganizationID;
      result.ParentID = this.ParentID;
      result.ArticleID = this.ArticleID;
       
       
      result.ModifiedDate = this.ModifiedDateUtc == null ? this.ModifiedDateUtc : DateTime.SpecifyKind((DateTime)this.ModifiedDateUtc, DateTimeKind.Utc); 
      result.CreatedDate = this.CreatedDateUtc == null ? this.CreatedDateUtc : DateTime.SpecifyKind((DateTime)this.CreatedDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
