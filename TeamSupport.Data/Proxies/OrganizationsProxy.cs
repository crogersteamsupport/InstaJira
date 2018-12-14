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

  public partial class Organization : BaseItem
  {
    public OrganizationProxy GetProxy()
    {
      OrganizationProxy result = new OrganizationProxy();
      result.APIRequestMinuteLimit = this.APIRequestMinuteLimit;
      result.UseWatson = this.UseWatson;
      result.RequireTwoFactor = this.RequireTwoFactor;
      result.NoAttachmentsInOutboundExcludeProductLine = this.NoAttachmentsInOutboundExcludeProductLine;
      result.AlertContactNoEmail = this.AlertContactNoEmail;
      result.RequireGroupAssignmentOnTickets = this.RequireGroupAssignmentOnTickets;
      result.AutoAssociateCustomerToTicketBasedOnAssetAssignment = this.AutoAssociateCustomerToTicketBasedOnAssetAssignment;
      result.AutoAssignCustomerWithAssetOnTickets = this.AutoAssignCustomerWithAssetOnTickets;
	  result.DaysBeforePasswordExpire = this.DaysBeforePasswordExpire;
	  result.NoAttachmentsInOutboundEmail = this.NoAttachmentsInOutboundEmail;
      result.ImportFileID = this.ImportFileID;
      result.TwoStepVerificationEnabled = this.TwoStepVerificationEnabled;
      result.IsCustomerInsightsActive = this.IsCustomerInsightsActive;
      result.UseProductFamilies = this.UseProductFamilies;
      result.CustDistIndexTrend = this.CustDistIndexTrend;
      result.HideDismissNonAdmins = this.HideDismissNonAdmins;
      result.AddEmailViaTS = this.AddEmailViaTS;
      result.AgentRating = this.AgentRating;
      result.ForceUseOfReplyTo = this.ForceUseOfReplyTo;
      result.ReplyToAlternateEmailAddresses = this.ReplyToAlternateEmailAddresses;
      result.UpdateTicketChildrenGroupWithParent = this.UpdateTicketChildrenGroupWithParent;
      result.ShowGroupMembersFirstInTicketAssignmentList = this.ShowGroupMembersFirstInTicketAssignmentList;
      result.SlaInitRespAnyAction = this.SlaInitRespAnyAction;
      result.FontSize = this.FontSize;
      result.FontFamily = this.FontFamily;
      result.CustDisIndex = this.CustDisIndex;
      result.AvgTimeToClose = this.AvgTimeToClose;
      result.AvgTimeOpen = this.AvgTimeOpen;
      result.CreatedLast30 = this.CreatedLast30;
      result.TicketsOpen = this.TicketsOpen;
      result.TotalTicketsCreated = this.TotalTicketsCreated;
      result.NeedCustForTicketMatch = this.NeedCustForTicketMatch;
      result.NeedsIndexing = this.NeedsIndexing;
      result.IsIndexLocked = this.IsIndexLocked;
      result.IsRebuildingIndex = this.IsRebuildingIndex;
      result.UnknownCompanyID = this.UnknownCompanyID;
      result.ForceBCCEmailsPrivate = this.ForceBCCEmailsPrivate;
      result.AllowUnsecureAttachmentViewing = this.AllowUnsecureAttachmentViewing;
      result.ProductVersionRequired = this.ProductVersionRequired;
      result.ProductRequired = this.ProductRequired;
      result.SupportHoursMonth = this.SupportHoursMonth;
      result.SetNewActionsVisibleToCustomers = this.SetNewActionsVisibleToCustomers;
      result.UseForums = this.UseForums;
      result.IsPublicArticles = this.IsPublicArticles;
      result.ChangeStatusIfClosed = this.ChangeStatusIfClosed;
      result.AddAdditionalContacts = this.AddAdditionalContacts;
      result.EvalProcess = this.EvalProcess;
      result.PotentialSeats = this.PotentialSeats;
      result.PrimaryInterest = this.PrimaryInterest;
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.MatchEmailSubject = this.MatchEmailSubject;
      result.MarkSpam = this.MarkSpam;
      result.TimedActionsRequired = this.TimedActionsRequired;
      result.CultureName = this.CultureName;
      result.UseEuropeDate = this.UseEuropeDate;
      result.BusinessDays = this.BusinessDays;
      result.InternalSlaLevelID = this.InternalSlaLevelID;
      result.SlaLevelID = this.SlaLevelID;
      result.DefaultWikiArticleID = this.DefaultWikiArticleID;
      result.ShowWiki = this.ShowWiki;
      result.AdminOnlyReports = this.AdminOnlyReports;
      result.AdminOnlyCustomers = this.AdminOnlyCustomers;
      result.CompanyDomains = this.CompanyDomains;
      result.OrganizationReplyToAddress = this.OrganizationReplyToAddress;
      result.EmailDelimiter = this.EmailDelimiter;
      result.RequireKnownUserForNewEmail = this.RequireKnownUserForNewEmail;
      result.RequireNewKeyword = this.RequireNewKeyword;
      result.APIRequestLimit = this.APIRequestLimit;
      result.CRMLinkID = this.CRMLinkID;
      result.PortalGuid = this.PortalGuid;
      result.ChatID = this.ChatID;
      result.SystemEmailID = this.SystemEmailID;
      result.WebServiceID = this.WebServiceID;
      result.ParentID = this.ParentID;
      result.ProductType = this.ProductType;
      result.DefaultSupportUserID = this.DefaultSupportUserID;
      result.DefaultSupportGroupID = this.DefaultSupportGroupID;
      result.DefaultPortalGroupID = this.DefaultPortalGroupID;
      result.PrimaryUserID = this.PrimaryUserID;
      result.IsBasicPortal = this.IsBasicPortal;
      result.IsAdvancedPortal = this.IsAdvancedPortal;
      result.HasPortalAccess = this.HasPortalAccess;
      result.InActiveReason = this.InActiveReason;
      result.TimeZoneID = this.TimeZoneID;
      result.IsInventoryEnabled = this.IsInventoryEnabled;
      result.IsApiEnabled = this.IsApiEnabled;
      result.IsApiActive = this.IsApiActive;
      result.IsActive = this.IsActive;
      result.ImportID = this.ImportID;
      result.ExtraStorageUnits = this.ExtraStorageUnits;
      result.ChatSeats = this.ChatSeats;
      result.PortalSeats = this.PortalSeats;
      result.UserSeats = this.UserSeats;
      result.IsCustomerFree = this.IsCustomerFree;
      result.PromoCode = this.PromoCode;
      result.WhereHeard = this.WhereHeard;
      result.Website = (this.Website);
      result.Description = (this.Description);
      result.Name = (this.Name);
      result.OrganizationID = this.OrganizationID;
      result.DisableSupportLogin = this.DisableSupportLogin;


      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.BusinessDayStart = DateTime.SpecifyKind(this.BusinessDayStartUtc, DateTimeKind.Utc);
      result.BusinessDayEnd = DateTime.SpecifyKind(this.BusinessDayEndUtc, DateTimeKind.Utc);
      result.LastIndexRebuilt = DateTime.SpecifyKind(this.LastIndexRebuiltUtc, DateTimeKind.Utc);

      result.SAExpirationDate = this.SAExpirationDateUtc == null ? this.SAExpirationDateUtc : DateTime.SpecifyKind((DateTime)this.SAExpirationDateUtc, DateTimeKind.Utc);


      return result;
    }
  }
}
