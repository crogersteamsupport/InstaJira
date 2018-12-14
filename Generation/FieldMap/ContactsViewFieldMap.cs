using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ContactsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Email", "Email", false, false, false);
      _fieldMap.AddMap("FirstName", "FirstName", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("MiddleName", "MiddleName", false, false, false);
      _fieldMap.AddMap("LastName", "LastName", false, false, false);
      _fieldMap.AddMap("Title", "Title", false, false, false);
      _fieldMap.AddMap("IsActive", "IsActive", false, false, false);
      _fieldMap.AddMap("MarkDeleted", "MarkDeleted", false, false, false);
      _fieldMap.AddMap("LastLogin", "LastLogin", false, false, false);
      _fieldMap.AddMap("LastActivity", "LastActivity", false, false, false);
      _fieldMap.AddMap("LastPing", "LastPing", false, false, false);
      _fieldMap.AddMap("IsSystemAdmin", "IsSystemAdmin", false, false, false);
      _fieldMap.AddMap("IsFinanceAdmin", "IsFinanceAdmin", false, false, false);
      _fieldMap.AddMap("IsPasswordExpired", "IsPasswordExpired", false, false, false);
      _fieldMap.AddMap("IsPortalUser", "IsPortalUser", false, false, false);
      _fieldMap.AddMap("PrimaryGroupID", "PrimaryGroupID", false, false, false);
      _fieldMap.AddMap("InOffice", "InOffice", false, false, false);
      _fieldMap.AddMap("InOfficeComment", "InOfficeComment", false, false, false);
      _fieldMap.AddMap("ActivatedOn", "ActivatedOn", false, false, false);
      _fieldMap.AddMap("DeactivatedOn", "DeactivatedOn", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Organization", "Organization", false, false, false);
      _fieldMap.AddMap("LastVersion", "LastVersion", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("OrganizationParentID", "OrganizationParentID", false, false, false);
      _fieldMap.AddMap("CryptedPassword", "CryptedPassword", false, false, false);
      _fieldMap.AddMap("SalesForceID", "SalesForceID", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("OrganizationActive", "OrganizationActive", false, false, false);
      _fieldMap.AddMap("OrganizationSAExpirationDate", "OrganizationSAExpirationDate", false, false, false);
      _fieldMap.AddMap("PortalLimitOrgTickets", "PortalLimitOrgTickets", false, false, false);
      _fieldMap.AddMap("LinkedIn", "LinkedIn", false, false, false);
      _fieldMap.AddMap("PortalViewOnly", "PortalViewOnly", false, false, true);
      _fieldMap.AddMap("BlockInboundEmail", "BlockInboundEmail", false, false, true);
            
    }
  }
  
}
