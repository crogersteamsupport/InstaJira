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
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, true);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("Website", "Website", true, true, true);
      _fieldMap.AddMap("WhereHeard", "WhereHeard", false, false, false);
      _fieldMap.AddMap("IsCustomerFree", "IsCustomerFree", false, false, false);
      _fieldMap.AddMap("UserSeats", "UserSeats", false, false, false);
      _fieldMap.AddMap("PortalSeats", "PortalSeats", false, false, false);
      _fieldMap.AddMap("ChatSeats", "ChatSeats", false, false, false);
      _fieldMap.AddMap("ExtraStorageUnits", "ExtraStorageUnits", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("IsActive", "IsActive", true, true, true);
      _fieldMap.AddMap("IsApiActive", "IsApiActive", false, false, false);
      _fieldMap.AddMap("IsApiEnabled", "IsApiEnabled", false, false, false);
      _fieldMap.AddMap("TimeZoneID", "TimeZoneID", true, true, true);
      _fieldMap.AddMap("InActiveReason", "InActiveReason", false, false, false);
      _fieldMap.AddMap("HasPortalAccess", "HasPortalAccess", true, true, true);
      _fieldMap.AddMap("IsAdvancedPortal", "IsAdvancedPortal", false, false, false);
      _fieldMap.AddMap("IsBasicPortal", "IsBasicPortal", false, false, false);
      _fieldMap.AddMap("PrimaryUserID", "PrimaryUserID", true, true, true);
      _fieldMap.AddMap("DefaultPortalGroupID", "DefaultPortalGroupID", true, true, true);
      _fieldMap.AddMap("DefaultSupportGroupID", "DefaultSupportGroupID", true, true, true);
      _fieldMap.AddMap("DefaultSupportUserID", "DefaultSupportUserID", true, true, true);
      _fieldMap.AddMap("ProductType", "ProductType", false, false, false);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, false);
      _fieldMap.AddMap("WebServiceID", "WebServiceID", false, false, false);
      _fieldMap.AddMap("SystemEmailID", "SystemEmailID", false, false, false);
      _fieldMap.AddMap("ChatID", "ChatID", false, false, true);
      _fieldMap.AddMap("PortalGuid", "PortalGuid", false, false, true);
      _fieldMap.AddMap("CRMLinkID", "CRMLinkID", false, false, true);
      _fieldMap.AddMap("SAExpirationDate", "SAExpirationDate", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("RequireNewKeyword", "RequireNewKeyword", false, false, false);
      _fieldMap.AddMap("RequireKnownUserForNewEmail", "RequireKnownUserForNewEmail", false, false, false);
      _fieldMap.AddMap("OrganizationReplyToAddress", "OrganizationReplyToAddress", false, false, false);
      _fieldMap.AddMap("CompanyDomains", "Domains", true, true, true);
      _fieldMap.AddMap("ShowWiki", "ShowWiki", false, false, false);
      _fieldMap.AddMap("DefaultWikiArticleID", "DefaultWikiArticleID", false, false, false);
      _fieldMap.AddMap("SlaLevelID", "SlaLevelID", true, true, true);
      _fieldMap.AddMap("InternalSlaLevelID", "InternalSlaLevelID", false, false, false);
      _fieldMap.AddMap("BusinessDays", "BusinessDays", false, false, false);
      _fieldMap.AddMap("BusinessDayStart", "BusinessDayStart", false, false, false);
      _fieldMap.AddMap("BusinessDayEnd", "BusinessDayEnd", false, false, false);
      _fieldMap.AddMap("UseEuropeDate", "UseEuropeDate", false, false, false);
      _fieldMap.AddMap("CultureName", "CultureName", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("SupportHoursMonth", "SupportHoursMonth", true, true, true);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("FontFamily", "FontFamily", false, false, false);
      _fieldMap.AddMap("FontSize", "FontSize", false, false, false);
      _fieldMap.AddMap("ShowGroupMembersFirstInTicketAssignmentList", "ShowGroupMembersFirstInTicketAssignmentList", false, false, false);
      _fieldMap.AddMap("UpdateTicketChildrenGroupWithParent", "UpdateTicketChildrenGroupWithParent", false, false, false);
      _fieldMap.AddMap("UseProductFamilies", "UseProductLines", false, false, false);
	  _fieldMap.AddMap("IsCustomerInsightsActive", "IsCustomerInsightsActive", false, false, false);
      _fieldMap.AddMap("TwoStepVerificationEnabled", "TwoStepVerificationEnabled", false, false, false);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
      _fieldMap.AddMap("DaysBeforePasswordExpire", "DaysBeforePasswordExpire", false, false, false);
      _fieldMap.AddMap("AutoAssignCustomerWithAssetOnTickets", "AutoAssignCustomerWithAssetOnTickets", false, false, false);
      _fieldMap.AddMap("AutoAssociateCustomerToTicketBasedOnAssetAssignment", "AutoAssociateCustomerToTicketBasedOnAssetAssignment", false, false, false);
      _fieldMap.AddMap("NoAttachmentsInOutboundExcludeProductLine", "NoAttachmentsInOutboundExcludeProductLine", false, false, false);
      _fieldMap.AddMap("UseWatson", "UseWatson", false, false, false);
      _fieldMap.AddMap("RequireTwoFactor", "RequireTwoFactor", false, false, false);      
      _fieldMap.AddMap("APIRequestMinuteLimit", "APIRequestMinuteLimit", false, false, false);

    }
  }

}
