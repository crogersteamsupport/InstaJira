using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{

  public partial class Organizations
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("Website", "Website", false, false, false);
      _fieldMap.AddMap("WhereHeard", "WhereHeard", false, false, false);
      _fieldMap.AddMap("PromoCode", "PromoCode", false, false, false);
      _fieldMap.AddMap("IsCustomerFree", "IsCustomerFree", false, false, false);
      _fieldMap.AddMap("UserSeats", "UserSeats", false, false, false);
      _fieldMap.AddMap("PortalSeats", "PortalSeats", false, false, false);
      _fieldMap.AddMap("ChatSeats", "ChatSeats", false, false, false);
      _fieldMap.AddMap("ExtraStorageUnits", "ExtraStorageUnits", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("IsActive", "IsActive", false, false, false);
      _fieldMap.AddMap("IsApiActive", "IsApiActive", false, false, false);
      _fieldMap.AddMap("IsApiEnabled", "IsApiEnabled", false, false, false);
      _fieldMap.AddMap("IsInventoryEnabled", "IsInventoryEnabled", false, false, false);
      _fieldMap.AddMap("TimeZoneID", "TimeZoneID", false, false, false);
      _fieldMap.AddMap("InActiveReason", "InActiveReason", false, false, false);
      _fieldMap.AddMap("HasPortalAccess", "HasPortalAccess", false, false, false);
      _fieldMap.AddMap("IsAdvancedPortal", "IsAdvancedPortal", false, false, false);
      _fieldMap.AddMap("IsBasicPortal", "IsBasicPortal", false, false, false);
      _fieldMap.AddMap("PrimaryUserID", "PrimaryUserID", false, false, false);
      _fieldMap.AddMap("DefaultPortalGroupID", "DefaultPortalGroupID", false, false, false);
      _fieldMap.AddMap("DefaultSupportGroupID", "DefaultSupportGroupID", false, false, false);
      _fieldMap.AddMap("DefaultSupportUserID", "DefaultSupportUserID", false, false, false);
      _fieldMap.AddMap("ProductType", "ProductType", false, false, false);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, false);
      _fieldMap.AddMap("WebServiceID", "WebServiceID", false, false, false);
      _fieldMap.AddMap("SystemEmailID", "SystemEmailID", false, false, false);
      _fieldMap.AddMap("ChatID", "ChatID", false, false, false);
      _fieldMap.AddMap("PortalGuid", "PortalGuid", false, false, false);
      _fieldMap.AddMap("CRMLinkID", "CRMLinkID", false, false, false);
      _fieldMap.AddMap("SAExpirationDate", "SAExpirationDate", false, false, false);
      _fieldMap.AddMap("APIRequestLimit", "APIRequestLimit", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("RequireNewKeyword", "RequireNewKeyword", false, false, false);
      _fieldMap.AddMap("RequireKnownUserForNewEmail", "RequireKnownUserForNewEmail", false, false, false);
      _fieldMap.AddMap("EmailDelimiter", "EmailDelimiter", false, false, false);
      _fieldMap.AddMap("OrganizationReplyToAddress", "OrganizationReplyToAddress", false, false, false);
      _fieldMap.AddMap("CompanyDomains", "CompanyDomains", false, false, false);
      _fieldMap.AddMap("AdminOnlyCustomers", "AdminOnlyCustomers", false, false, false);
      _fieldMap.AddMap("AdminOnlyReports", "AdminOnlyReports", false, false, false);
      _fieldMap.AddMap("ShowWiki", "ShowWiki", false, false, false);
      _fieldMap.AddMap("DefaultWikiArticleID", "DefaultWikiArticleID", false, false, false);
      _fieldMap.AddMap("SlaLevelID", "SlaLevelID", false, false, false);
      _fieldMap.AddMap("InternalSlaLevelID", "InternalSlaLevelID", false, false, false);
      _fieldMap.AddMap("BusinessDays", "BusinessDays", false, false, false);
      _fieldMap.AddMap("BusinessDayStart", "BusinessDayStart", false, false, false);
      _fieldMap.AddMap("BusinessDayEnd", "BusinessDayEnd", false, false, false);
      _fieldMap.AddMap("UseEuropeDate", "UseEuropeDate", false, false, false);
      _fieldMap.AddMap("CultureName", "CultureName", false, false, false);
      _fieldMap.AddMap("TimedActionsRequired", "TimedActionsRequired", false, false, false);
      _fieldMap.AddMap("MatchEmailSubject", "MatchEmailSubject", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("PrimaryInterest", "PrimaryInterest", false, false, false);
      _fieldMap.AddMap("PotentialSeats", "PotentialSeats", false, false, false);
      _fieldMap.AddMap("EvalProcess", "EvalProcess", false, false, false);
      _fieldMap.AddMap("AddAdditionalContacts", "AddAdditionalContacts", false, false, false);
      _fieldMap.AddMap("ChangeStatusIfClosed", "ChangeStatusIfClosed", false, false, false);
      _fieldMap.AddMap("IsPublicArticles", "IsPublicArticles", false, false, false);
      _fieldMap.AddMap("UseForums", "UseForums", false, false, false);
      _fieldMap.AddMap("SetNewActionsVisibleToCustomers", "SetNewActionsVisibleToCustomers", false, false, false);
      _fieldMap.AddMap("SupportHoursMonth", "SupportHoursMonth", false, false, false);
      _fieldMap.AddMap("ProductRequired", "ProductRequired", false, false, false);
      _fieldMap.AddMap("ProductVersionRequired", "ProductVersionRequired", false, false, false);
      _fieldMap.AddMap("AllowUnsecureAttachmentViewing", "AllowUnsecureAttachmentViewing", false, false, false);
      _fieldMap.AddMap("ForceBCCEmailsPrivate", "ForceBCCEmailsPrivate", false, false, false);
      _fieldMap.AddMap("UnknownCompanyID", "UnknownCompanyID", false, false, false);
      _fieldMap.AddMap("IsRebuildingIndex", "IsRebuildingIndex", false, false, false);
      _fieldMap.AddMap("LastIndexRebuilt", "LastIndexRebuilt", false, false, false);
      _fieldMap.AddMap("IsIndexLocked", "IsIndexLocked", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("NeedCustForTicketMatch", "NeedCustForTicketMatch", false, false, false);
      _fieldMap.AddMap("TotalTicketsCreated", "TotalTicketsCreated", false, false, false);
      _fieldMap.AddMap("TicketsOpen", "TicketsOpen", false, false, false);
      _fieldMap.AddMap("CreatedLast30", "CreatedLast30", false, false, false);
      _fieldMap.AddMap("AvgTimeOpen", "AvgTimeOpen", false, false, false);
      _fieldMap.AddMap("AvgTimeToClose", "AvgTimeToClose", false, false, false);
      _fieldMap.AddMap("CustDisIndex", "CustDisIndex", false, false, false);
      _fieldMap.AddMap("SlaInitRespAnyAction", "SlaInitRespAnyAction", false, false, false);
      _fieldMap.AddMap("FontFamily", "FontFamily", false, false, false);
      _fieldMap.AddMap("FontSize", "FontSize", false, false, false);
      _fieldMap.AddMap("ShowGroupMembersFirstInTicketAssignmentList", "ShowGroupMembersFirstInTicketAssignmentList", false, false, false);
      _fieldMap.AddMap("UpdateTicketChildrenGroupWithParent", "UpdateTicketChildrenGroupWithParent", false, false, false);
      _fieldMap.AddMap("ReplyToAlternateEmailAddresses", "ReplyToAlternateEmailAddresses", false, false, false);
      _fieldMap.AddMap("ForceUseOfReplyTo", "ForceUseOfReplyTo", false, false, false);
      _fieldMap.AddMap("AgentRating", "AgentRating", false, false, false);
      _fieldMap.AddMap("AddEmailViaTS", "AddEmailViaTS", false, false, false);
      _fieldMap.AddMap("SignUpToken", "SignUpToken", false, false, false);
      _fieldMap.AddMap("IsValidated", "IsValidated", false, false, false);
      _fieldMap.AddMap("DateLastIndexed", "DateLastIndexed", false, false, false);
      _fieldMap.AddMap("HideDismissNonAdmins", "HideDismissNonAdmins", false, false, false);
      _fieldMap.AddMap("CustDistIndexTrend", "CustDistIndexTrend", false, false, false);
      _fieldMap.AddMap("UseProductFamilies", "UseProductFamilies", false, false, false);
      _fieldMap.AddMap("IsCustomerInsightsActive", "IsCustomerInsightsActive", false, false, false);
      _fieldMap.AddMap("TwoStepVerificationEnabled", "TwoStepVerificationEnabled", false, false, false);
      _fieldMap.AddMap("DaysBeforePasswordExpire", "DaysBeforePasswordExpire", false, false, false);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
      _fieldMap.AddMap("NoAttachmentsInOutboundEmail", "NoAttachmentsInOutboundEmail", false, false, false);
      _fieldMap.AddMap("AutoAssignCustomerWithAssetOnTickets", "AutoAssignCustomerWithAssetOnTickets", false, false, false);
      _fieldMap.AddMap("AutoAssociateCustomerToTicketBasedOnAssetAssignment", "AutoAssociateCustomerToTicketBasedOnAssetAssignment", false, false, false);
      _fieldMap.AddMap("RequireGroupAssignmentOnTickets", "RequireGroupAssignmentOnTickets", false, false, false);
      _fieldMap.AddMap("AlertContactNoEmail", "AlertContactNoEmail", false, false, false);
      _fieldMap.AddMap("DisableSupportLogin", "DisableSupportLogin", false, false, false);
      _fieldMap.AddMap("NoAttachmentsInOutboundExcludeProductLine", "NoAttachmentsInOutboundExcludeProductLine", false, false, false);
      _fieldMap.AddMap("UseWatson", "UseWatson", false, false, false);
      _fieldMap.AddMap("RequireTwoFactor", "RequireTwoFactor", false, false, false);
      _fieldMap.AddMap("MarkSpam", "MarkSpam", false, false, false);
      _fieldMap.AddMap("APIRequestMinuteLimit", "APIRequestMinuteLimit", false, false, false);

    }
  }

}
