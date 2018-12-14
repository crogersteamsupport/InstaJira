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
  [KnownType(typeof(OrganizationProxy))]
  public class OrganizationProxy
  {
    public OrganizationProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public string Website { get; set; }
    [DataMember] public string WhereHeard { get; set; }
    [DataMember] public string PromoCode { get; set; }
    [DataMember] public bool IsCustomerFree { get; set; }
    [DataMember] public int UserSeats { get; set; }
    [DataMember] public int PortalSeats { get; set; }
    [DataMember] public int ChatSeats { get; set; }
    [DataMember] public int ExtraStorageUnits { get; set; }
    [DataMember] public string ImportID { get; set; }
    [DataMember] public bool IsActive { get; set; }
    [DataMember] public bool IsApiActive { get; set; }
    [DataMember] public bool IsApiEnabled { get; set; }
    [DataMember] public bool IsInventoryEnabled { get; set; }
    [DataMember] public string TimeZoneID { get; set; }
    [DataMember] public string InActiveReason { get; set; }
    [DataMember] public bool HasPortalAccess { get; set; }
    [DataMember] public bool IsAdvancedPortal { get; set; }
    [DataMember] public bool IsBasicPortal { get; set; }
    [DataMember] public int? PrimaryUserID { get; set; }
    [DataMember] public int? DefaultPortalGroupID { get; set; }
    [DataMember] public int? DefaultSupportGroupID { get; set; }
    [DataMember] public int? DefaultSupportUserID { get; set; }
    [DataMember] public ProductType ProductType { get; set; }
    [DataMember] public int? ParentID { get; set; }
    [DataMember] public Guid WebServiceID { get; set; }
    [DataMember] public Guid SystemEmailID { get; set; }
    [DataMember] public Guid ChatID { get; set; }
    [DataMember] public Guid PortalGuid { get; set; }
    [DataMember] public string CRMLinkID { get; set; }
    [DataMember] public DateTime? SAExpirationDate { get; set; }
    [DataMember] public int APIRequestLimit { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public bool RequireNewKeyword { get; set; }
    [DataMember] public bool? RequireKnownUserForNewEmail { get; set; }
    [DataMember] public string EmailDelimiter { get; set; }
    [DataMember] public string OrganizationReplyToAddress { get; set; }
    [DataMember] public string CompanyDomains { get; set; }
    [DataMember] public bool AdminOnlyCustomers { get; set; }
    [DataMember] public bool AdminOnlyReports { get; set; }
    [DataMember] public bool ShowWiki { get; set; }
    [DataMember] public int? DefaultWikiArticleID { get; set; }
    [DataMember] public int? SlaLevelID { get; set; }
    [DataMember] public int? InternalSlaLevelID { get; set; }
    [DataMember] public int BusinessDays { get; set; }
    [DataMember] public DateTime BusinessDayStart { get; set; }
    [DataMember] public DateTime BusinessDayEnd { get; set; }
    [DataMember] public bool? UseEuropeDate { get; set; }
    [DataMember] public string CultureName { get; set; }
    [DataMember] public bool TimedActionsRequired { get; set; }
    [DataMember] public bool? MatchEmailSubject { get; set; }
    [DataMember] public bool? MarkSpam { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
    [DataMember] public string PrimaryInterest { get; set; }
    [DataMember] public string PotentialSeats { get; set; }
    [DataMember] public string EvalProcess { get; set; }
    [DataMember] public bool? AddAdditionalContacts { get; set; }
    [DataMember] public bool? ChangeStatusIfClosed { get; set; }
    [DataMember] public bool IsPublicArticles { get; set; }
    [DataMember] public bool UseForums { get; set; }
    [DataMember] public bool SetNewActionsVisibleToCustomers { get; set; }
    [DataMember] public int SupportHoursMonth { get; set; }
    [DataMember] public bool ProductRequired { get; set; }
    [DataMember] public bool ProductVersionRequired { get; set; }
    [DataMember] public bool AllowUnsecureAttachmentViewing { get; set; }
    [DataMember] public bool ForceBCCEmailsPrivate { get; set; }
    [DataMember] public int? UnknownCompanyID { get; set; }
    [DataMember] public bool IsRebuildingIndex { get; set; }
    [DataMember] public DateTime LastIndexRebuilt { get; set; }
    [DataMember] public bool IsIndexLocked { get; set; }
    [DataMember] public bool NeedsIndexing { get; set; }
    [DataMember] public bool NeedCustForTicketMatch { get; set; }
    [DataMember] public int TotalTicketsCreated { get; set; }
    [DataMember] public int TicketsOpen { get; set; }
    [DataMember] public int CreatedLast30 { get; set; }
    [DataMember] public int AvgTimeOpen { get; set; }
    [DataMember] public int AvgTimeToClose { get; set; }
    [DataMember] public int CustDisIndex { get; set; }
    [DataMember] public FontFamily FontFamily { get; set; }
    [DataMember] public FontSize FontSize { get; set; }
    [DataMember] public bool SlaInitRespAnyAction { get; set; }
    [DataMember] public bool ShowGroupMembersFirstInTicketAssignmentList { get; set; }
    [DataMember] public bool UpdateTicketChildrenGroupWithParent { get; set; }
    [DataMember] public bool ReplyToAlternateEmailAddresses { get; set; }
    [DataMember] public bool ForceUseOfReplyTo { get; set; }
    [DataMember] public bool AgentRating { get; set; }
    [DataMember] public bool AddEmailViaTS { get; set; }
    [DataMember]
    public bool HideDismissNonAdmins { get; set; }
    [DataMember]
    public int? CustDistIndexTrend { get; set; }
    [DataMember] public bool UseProductFamilies { get; set; }
    [DataMember] public bool IsCustomerInsightsActive { get; set; }
    [DataMember] public bool TwoStepVerificationEnabled { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
    [DataMember] public int DaysBeforePasswordExpire { get; set; }
    [DataMember] public bool NoAttachmentsInOutboundEmail { get; set; }
    [DataMember] public bool AutoAssignCustomerWithAssetOnTickets { get; set; }
    [DataMember] public bool AutoAssociateCustomerToTicketBasedOnAssetAssignment { get; set; }
    [DataMember] public bool RequireGroupAssignmentOnTickets { get; set; }
    [DataMember] public bool AlertContactNoEmail { get; set; }
    [DataMember] public bool DisableSupportLogin { get; set; }
    [DataMember] public string NoAttachmentsInOutboundExcludeProductLine { get; set; }
    [DataMember] public bool UseWatson { get; set; }
    [DataMember] public bool RequireTwoFactor { get; set; }
    [DataMember] public int APIRequestMinuteLimit { get; set; }

  }
}
