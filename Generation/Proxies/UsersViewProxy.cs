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
  [KnownType(typeof(UsersViewItemProxy))]
  public class UsersViewItemProxy
  {
    public UsersViewItemProxy() {}
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
    [DataMember] public bool IsOnline { get; set; }
    [DataMember] public string CryptedPassword { get; set; }
    [DataMember] public bool IsChatUser { get; set; }
          
  }
  
  public partial class UsersViewItem : BaseItem
  {
    public UsersViewItemProxy GetProxy()
    {
      UsersViewItemProxy result = new UsersViewItemProxy();
      result.IsChatUser = this.IsChatUser;
      result.CryptedPassword = this.CryptedPassword;
      result.IsOnline = this.IsOnline;
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
       
      result.LastLogin = DateTime.SpecifyKind(this.LastLogin, DateTimeKind.Local);
      result.LastActivity = DateTime.SpecifyKind(this.LastActivity, DateTimeKind.Local);
      result.ActivatedOn = DateTime.SpecifyKind(this.ActivatedOn, DateTimeKind.Local);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreated, DateTimeKind.Local);
      result.DateModified = DateTime.SpecifyKind(this.DateModified, DateTimeKind.Local);
       
      result.DeactivatedOn = this.DeactivatedOn == null ? this.DeactivatedOn : DateTime.SpecifyKind((DateTime)this.DeactivatedOn, DateTimeKind.Local); 
      result.LastPing = this.LastPing == null ? this.LastPing : DateTime.SpecifyKind((DateTime)this.LastPing, DateTimeKind.Local); 
       
      return result;
    }	
  }
}
