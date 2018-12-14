using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{

  public partial class Users
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("UserID", "UserID", false, false, true);
      _fieldMap.AddMap("Email", "Email", true, true, true);
      _fieldMap.AddMap("FirstName", "FirstName", true, true, true);
      _fieldMap.AddMap("MiddleName", "MiddleName", true, true, true);
      _fieldMap.AddMap("LastName", "LastName", true, true, true);
      _fieldMap.AddMap("Title", "Title", true, true, true);
      _fieldMap.AddMap("CryptedPassword", "CryptedPassword", true, true, false);
      _fieldMap.AddMap("IsActive", "IsActive", true, true, true);
      _fieldMap.AddMap("MarkDeleted", "MarkDeleted", false, false, false);
      _fieldMap.AddMap("TimeZoneID", "TimeZoneID", false, false, false);
      _fieldMap.AddMap("CultureName", "CultureName", false, false, false);
      _fieldMap.AddMap("LastLogin", "LastLogin", true, true, true);
      _fieldMap.AddMap("LastActivity", "LastActivity", false, false, true);
      _fieldMap.AddMap("LastPing", "LastPing", false, false, true);
      _fieldMap.AddMap("IsSystemAdmin", "IsSystemAdmin", false, false, false);
      _fieldMap.AddMap("IsFinanceAdmin", "IsFinanceAdmin", false, false, false);
      _fieldMap.AddMap("IsPasswordExpired", "IsPasswordExpired", false, false, false);
      _fieldMap.AddMap("IsPortalUser", "IsPortalUser", true, true, true);
      _fieldMap.AddMap("IsChatUser", "IsChatUser", false, false, true);
      _fieldMap.AddMap("PrimaryGroupID", "PrimaryGroupID", false, false, true);
      _fieldMap.AddMap("InOffice", "InOffice", true, true, true);
      _fieldMap.AddMap("InOfficeComment", "InOfficeComment", true, true, true);
      _fieldMap.AddMap("ReceiveTicketNotifications", "ReceiveTicketNotifications", false, false, true);
      _fieldMap.AddMap("ActivatedOn", "ActivatedOn", false, false, true);
      _fieldMap.AddMap("DeactivatedOn", "DeactivatedOn", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, true);
      _fieldMap.AddMap("LastVersion", "LastVersion", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("SalesForceID", "SalesForceID", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("FontFamily", "FontFamily", false, false, false);
      _fieldMap.AddMap("FontSize", "FontSize", false, false, false);
      _fieldMap.AddMap("RestrictUserFromEditingAnyActions", "RestrictUserFromEditingAnyActions", false, false, false);
      _fieldMap.AddMap("AllowUserToEditAnyAction", "AllowUserToEditAnyAction", false, false, false);
      _fieldMap.AddMap("UserCanPinAction", "UserCanPinAction", false, false, false);
      _fieldMap.AddMap("PortalLimitOrgTickets", "DisableOrganizationTicketsViewOnPortal", true, true, true);
      _fieldMap.AddMap("ProductFamiliesRights", "ProductLinesRights", false, false, false);
      _fieldMap.AddMap("BlockEmailFromCreatingOnly", "BlockEmailFromCreatingOnly", false, false, false);
	  _fieldMap.AddMap("PortalViewOnly", "PortalViewOnly", true, false, true);
      _fieldMap.AddMap("verificationPhoneNumber", "verificationPhoneNumber", false, false, false);
      _fieldMap.AddMap("verificationCode", "verificationCode", false, false, false);
      _fieldMap.AddMap("verificationCodeExpiration", "verificationCodeExpiration", false, false, false);
      _fieldMap.AddMap("PasswordCreatedUtc", "PasswordCreatedUtc", false, false, false);
      _fieldMap.AddMap("PortalLimitOrgChildrenTickets", "PortalLimitOrgChildrenTickets", false, false, false);
	  _fieldMap.AddMap("BlockInboundEmail", "BlockInboundEmail", true, false, true);
	  _fieldMap.AddMap("CanBulkMerge", "CanBulkMerge", false, false, false);
	  _fieldMap.AddMap("DisablePublic", "DisablePublic", false, false, false);                        
    }
  }

}
