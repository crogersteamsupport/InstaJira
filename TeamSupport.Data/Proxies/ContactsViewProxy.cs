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
  [KnownType(typeof(ContactsViewItemProxy))]
  public class ContactsViewItemProxy
  {
    public ContactsViewItemProxy() {}
    [DataMember] public string Email { get; set; }
    [DataMember] public string FirstName { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public string MiddleName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public bool MarkDeleted { get; set; }
    [DataMember] public DateTime LastLogin { get; set; }
    [DataMember] public DateTime LastActivity { get; set; }
    [DataMember] public DateTime? LastPing { get; set; }
    [DataMember] public bool IsSystemAdmin { get; set; }
    [DataMember] public bool IsFinanceAdmin { get; set; }
    [DataMember] public bool IsPasswordExpired { get; set; }
    [DataMember] public bool IsPortalUser { get; set; }
    [DataMember] public int? PrimaryGroupID { get; set; }
    [DataMember] public bool InOffice { get; set; }
    [DataMember] public string InOfficeComment { get; set; }
    [DataMember] public DateTime ActivatedOn { get; set; }
    [DataMember] public DateTime? DeactivatedOn { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Organization { get; set; }
    [DataMember] public string LastVersion { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public int? OrganizationParentID { get; set; }
    [DataMember] public string CryptedPassword { get; set; }
    [DataMember] public string SalesForceID { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public bool? OrganizationActive { get; set; }
    [DataMember] public DateTime? OrganizationSAExpirationDate { get; set; }
    [DataMember] public bool PortalLimitOrgTickets { get; set; }
    [DataMember] public string LinkedIn { get; set; }
    [DataMember] public bool PortalViewOnly { get; set; }
    [DataMember] public bool BlockInboundEmail { get; set; }
          
  }
  
  public partial class ContactsViewItem : BaseItem
  {
    public ContactsViewItemProxy GetProxy()
    {
      ContactsViewItemProxy result = new ContactsViewItemProxy();
      result.BlockInboundEmail = this.BlockInboundEmail;
      result.PortalViewOnly = this.PortalViewOnly;
      result.LinkedIn = this.LinkedIn;
      result.PortalLimitOrgTickets = this.PortalLimitOrgTickets;
      result.OrganizationActive = this.OrganizationActive;
      result.NeedsIndexing = this.NeedsIndexing;
      result.SalesForceID = this.SalesForceID;
      result.CryptedPassword = this.CryptedPassword;
      result.OrganizationParentID = this.OrganizationParentID;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.LastVersion = this.LastVersion;
      result.Organization = this.Organization;
      result.OrganizationID = this.OrganizationID;
      result.InOfficeComment = this.InOfficeComment;
      result.InOffice = this.InOffice;
      result.PrimaryGroupID = this.PrimaryGroupID;
      result.IsPortalUser = this.IsPortalUser;
      result.IsPasswordExpired = this.IsPasswordExpired;
      result.IsFinanceAdmin = this.IsFinanceAdmin;
      result.IsSystemAdmin = this.IsSystemAdmin;
      result.MarkDeleted = this.MarkDeleted;
      result.IsActive = this.IsActive;
      result.Title = this.Title;
      result.LastName = this.LastName;
      result.MiddleName = this.MiddleName;
      result.UserID = this.UserID;
      result.FirstName = this.FirstName;
      result.Email = this.Email;
       
      result.LastLogin = DateTime.SpecifyKind(this.LastLoginUtc, DateTimeKind.Utc);
      result.LastActivity = DateTime.SpecifyKind(this.LastActivityUtc, DateTimeKind.Utc);
      result.ActivatedOn = DateTime.SpecifyKind(this.ActivatedOnUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
      result.OrganizationSAExpirationDate = this.OrganizationSAExpirationDateUtc == null ? this.OrganizationSAExpirationDateUtc : DateTime.SpecifyKind((DateTime)this.OrganizationSAExpirationDateUtc, DateTimeKind.Utc); 
      result.DeactivatedOn = this.DeactivatedOnUtc == null ? this.DeactivatedOnUtc : DateTime.SpecifyKind((DateTime)this.DeactivatedOnUtc, DateTimeKind.Utc); 
      result.LastPing = this.LastPingUtc == null ? this.LastPingUtc : DateTime.SpecifyKind((DateTime)this.LastPingUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
