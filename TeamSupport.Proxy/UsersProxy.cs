using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Globalization;
using System.Data.Linq.Mapping;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(UserProxy))]
    [Table(Name = "Users")]
    public class UserProxy
  {
    public UserProxy() {}
    [DataMember, Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)] public int UserID { get; set; }
    [DataMember, Column] public string Email { get; set; }
    [DataMember, Column] public string FirstName { get; set; }
    [DataMember, Column] public string MiddleName { get; set; }
    [DataMember, Column] public string LastName { get; set; }
    [DataMember, Column] public string Title { get; set; }
    [XmlIgnore, Column] public string CryptedPassword { get; set; }
    [DataMember, Column] public bool IsActive { get; set; }
    [DataMember, Column] public bool MarkDeleted { get; set; }
    [DataMember, Column] public string TimeZoneID { get; set; }
    [DataMember, Column] public string CultureName { get; set; }
    [DataMember, Column] public DateTime LastLogin { get; set; }
    [DataMember, Column] public DateTime LastActivity { get; set; }
    [DataMember, Column] public DateTime? LastPing { get; set; }
    [DataMember, Column] public int LastWaterCoolerID { get; set; }
    [DataMember, Column] public bool IsSystemAdmin { get; set; }
    [DataMember, Column] public bool IsFinanceAdmin { get; set; }
    [DataMember, Column] public bool IsPasswordExpired { get; set; }
    [DataMember, Column] public bool IsPortalUser { get; set; }
    [DataMember, Column] public bool IsChatUser { get; set; }
    [DataMember, Column] public int? PrimaryGroupID { get; set; }
    [DataMember, Column] public bool InOffice { get; set; }
    [DataMember, Column] public string InOfficeComment { get; set; }
    [DataMember, Column] public bool ReceiveTicketNotifications { get; set; }
    [DataMember, Column] public bool ReceiveAllGroupNotifications { get; set; }
    [DataMember, Column] public bool ReceiveUnassignedGroupEmails { get; set; }
    [DataMember, Column] public bool SubscribeToNewTickets { get; set; }
    [DataMember, Column] public bool SubscribeToNewActions { get; set; }
    [DataMember, Column] public DateTime ActivatedOn { get; set; }
    [DataMember, Column] public DateTime? DeactivatedOn { get; set; }
    [DataMember, Column] public int OrganizationID { get; set; }
    [DataMember, Column] public string LastVersion { get; set; }
    [DataMember, Column] public Guid? SessionID { get; set; }
    [DataMember, Column] public string ImportID { get; set; }
    [DataMember, Column] public DateTime DateCreated { get; set; }
    [DataMember, Column] public DateTime DateModified { get; set; }
    [DataMember, Column] public int CreatorID { get; set; }
    [DataMember, Column] public int ModifierID { get; set; }
    [DataMember, Column] public string OrgsUserCanSeeOnPortal { get; set; }
    [DataMember, Column] public bool DoNotAutoSubscribe { get; set; }
    [DataMember, Column] public bool IsClassicView { get; set; }
    [DataMember, Column] public string timeZoneDisplay { get; set; }
    [DataMember, Column] public string CultureDisplay { get; set; }
    [DataMember, Column] public bool ShowWelcomePage { get; set; }
    [DataMember, Column] public string UserInformation { get; set; }
    [DataMember, Column] public bool AppChatStatus { get; set; }
    [DataMember, Column] public string AppChatID { get; set; }
    [DataMember, Column] public string MenuItems { get; set; }
    [DataMember, Column] public string LinkedIn { get; set; }
    [DataMember, Column] public TicketRightType TicketRights { get; set; }
    [DataMember, Column] public bool OnlyEmailAfterHours { get; set; }
    [DataMember, Column] public bool BlockInboundEmail { get; set; }
    [DataMember, Column] public string SalesForceID { get; set; }
    [DataMember, Column] public bool ChangeTicketVisibility { get; set; }
    [DataMember, Column] public bool CanChangeCommunityVisibility { get; set; }
    [DataMember, Column] public bool ChangeKbVisibility { get; set; }
    [DataMember, Column] public string Avatar { get; set; }
    [DataMember, Column] public bool EnforceSingleSession { get; set; }
    [DataMember, Column] public bool NeedsIndexing { get; set; }
    [DataMember, Column] public bool AllowAnyTicketCustomer { get; set; }
    [DataMember, Column] public FontFamily FontFamily { get; set; }
    [DataMember, Column] public FontSize FontSize { get; set; }
    [DataMember, Column] public string FontFamilyDescription { get; set; }
    [DataMember, Column] public string FontSizeDescription { get; set; }
    [DataMember, Column] public bool CanCreateCompany { get; set; }
    [DataMember, Column] public bool CanEditCompany { get; set; }
    [DataMember, Column] public bool CanCreateContact { get; set; }
    [DataMember, Column] public bool CanEditContact { get; set; }
    [DataMember, Column] public bool CanCreateAsset { get; set; }
    [DataMember, Column] public bool CanEditAsset { get; set; }
    [DataMember, Column] public bool RestrictUserFromEditingAnyActions { get; set; }
    [DataMember, Column] public bool AllowUserToEditAnyAction { get; set; }
    [DataMember, Column] public bool UserCanPinAction { get; set; }
    [DataMember, Column] public bool PortalLimitOrgTickets { get; set; }
    [DataMember, Column] public bool FilterInactive { get; set; }
    [DataMember, Column] public bool DisableExporting { get; set; }
    [DataMember, Column] public bool DisablePublic { get; set; }
    [DataMember, Column] public bool CanCreateProducts { get; set; }
    [DataMember, Column] public bool CanCreateVersions { get; set; }
    [DataMember, Column] public bool CanEditProducts { get; set; }
    [DataMember, Column] public bool CanEditVersions { get; set; }
    [DataMember, Column] public ProductFamiliesRightType ProductFamiliesRights { get; set; }
    [DataMember, Column] public bool? BlockEmailFromCreatingOnly { get; set; }
    [DataMember, Column] public Guid CalGUID { get; set; }
    [DataMember, Column] public bool PortalViewOnly { get; set; }
	[DataMember, Column] public string verificationCode { get; set; }
	[DataMember, Column] public string verificationPhoneNumber { get; set; }
	[DataMember, Column] public DateTime? verificationCodeExpiration { get; set; }
	[DataMember, Column] public DateTime? PasswordCreatedUtc { get; set; }
    [DataMember, Column] public int? ImportFileID { get; set; }
    [DataMember, Column] public bool PortalLimitOrgChildrenTickets { get; set; }
    [DataMember, Column] public bool CanBulkMerge { get; set; }

  }

}
