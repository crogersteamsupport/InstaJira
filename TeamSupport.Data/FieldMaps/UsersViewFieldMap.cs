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
            _fieldMap.AddMap("Email", "Email", true, true, true);
            _fieldMap.AddMap("FirstName", "FirstName", true, true, true);
            _fieldMap.AddMap("UserID", "UserID", true, true, true);
            _fieldMap.AddMap("MiddleName", "MiddleName", true, true, true);
            _fieldMap.AddMap("LastName", "LastName", true, true, true);
            _fieldMap.AddMap("Title", "Title", true, true, true);
            _fieldMap.AddMap("IsActive", "IsActive", true, true, true);
            _fieldMap.AddMap("MarkDeleted", "MarkDeleted", false, false, false);
            _fieldMap.AddMap("LastLogin", "LastLogin", false, false, true);
            _fieldMap.AddMap("LastActivity", "LastActivity", false, false, true);
            _fieldMap.AddMap("LastPing", "LastPing", false, false, true);
            _fieldMap.AddMap("IsSystemAdmin", "IsSystemAdmin", true, true, true);
            _fieldMap.AddMap("IsFinanceAdmin", "IsFinanceAdmin", true, true, true);
            _fieldMap.AddMap("IsPasswordExpired", "IsPasswordExpired", false, false, false);
            _fieldMap.AddMap("IsPortalUser", "IsPortalUser", true, true, true);
            _fieldMap.AddMap("PrimaryGroupID", "PrimaryGroupID", true, true, true);
            _fieldMap.AddMap("InOffice", "InOffice", true, true, true);
            _fieldMap.AddMap("InOfficeComment", "InOfficeComment", true, true, true);
            _fieldMap.AddMap("ActivatedOn", "ActivatedOn", false, false, true);
            _fieldMap.AddMap("DeactivatedOn", "DeactivatedOn", false, false, true);
            _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, true);
            _fieldMap.AddMap("Organization", "Organization", false, false, true);
            _fieldMap.AddMap("LastVersion", "LastVersion", false, false, false);
            _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
            _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
            _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
            _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
            _fieldMap.AddMap("IsOnline", "IsOnline", false, false, true);
            _fieldMap.AddMap("CryptedPassword", "CryptedPassword", true, true, false);
            _fieldMap.AddMap("IsChatUser", "IsChatUser", true, true, true);

        }
    }

}
