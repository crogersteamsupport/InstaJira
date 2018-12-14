using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class UsersView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Email", "Email", false, false, false);
      _fieldMap.AddMap("FirstName", "FirstName", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
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
      _fieldMap.AddMap("IsOnline", "IsOnline", false, false, false);
      _fieldMap.AddMap("CryptedPassword", "CryptedPassword", false, false, false);
      _fieldMap.AddMap("IsChatUser", "IsChatUser", false, false, false);
      _fieldMap.AddMap("PortalLimitOrgTickets", "PortalLimitOrgTickets", false, false, false);
            
    }
  }
  
}
