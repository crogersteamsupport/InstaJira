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
  [KnownType(typeof(OrganizationsViewItemProxy))]
  public class OrganizationsViewItemProxy
  {
    public OrganizationsViewItemProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public string Website { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public string InActiveReason { get; set; }
    [DataMember] public int? PrimaryUserID { get; set; }
    [DataMember] public string PrimaryContact { get; set; }
    [DataMember] public int? ParentID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public bool HasPortalAccess { get; set; }
    [DataMember] public string CreatedBy { get; set; }
    [DataMember] public string LastModifiedBy { get; set; }
    [DataMember] public DateTime? SAExpirationDate { get; set; }
    [DataMember] public string SlaName { get; set; }
    [DataMember] public string CRMLinkID { get; set; }
    [DataMember] public Guid PortalGuid { get; set; }
    [DataMember] public int? SlaLevelID { get; set; }
    [DataMember] public int? DefaultWikiArticleID { get; set; }
    [DataMember] public int? DefaultSupportGroupID { get; set; }
    [DataMember] public int? DefaultSupportUserID { get; set; }
    [DataMember] public int? SupportHoursUsed { get; set; }
    [DataMember] public int? SupportHoursRemaining { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
          
  }
  
  public partial class OrganizationsViewItem : BaseItem
  {
    public OrganizationsViewItemProxy GetProxy()
    {
      OrganizationsViewItemProxy result = new OrganizationsViewItemProxy();

      result.NeedsIndexing = this.NeedsIndexing;
      result.SupportHoursRemaining = this.SupportHoursRemaining;
      result.SupportHoursUsed = this.SupportHoursUsed;
      result.DefaultSupportUserID = this.DefaultSupportUserID;
      result.DefaultSupportGroupID = this.DefaultSupportGroupID;
      result.DefaultWikiArticleID = this.DefaultWikiArticleID;
      result.SlaLevelID = this.SlaLevelID;
      result.PortalGuid = this.PortalGuid;
      result.CRMLinkID = this.CRMLinkID;
      result.SlaName = this.SlaName;
      result.LastModifiedBy = this.LastModifiedBy;
      result.CreatedBy = this.CreatedBy;
      result.HasPortalAccess = this.HasPortalAccess;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.ParentID = this.ParentID;
      result.PrimaryContact = this.PrimaryContact;
      result.PrimaryUserID = this.PrimaryUserID;
      result.InActiveReason = this.InActiveReason;
      result.IsActive = this.IsActive;
      result.Website = (this.Website);
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.OrganizationID = this.OrganizationID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.SAExpirationDate = this.SAExpirationDateUtc == null ? this.SAExpirationDateUtc : DateTime.SpecifyKind((DateTime)this.SAExpirationDateUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
