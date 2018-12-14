using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class PortalLoginHistory
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("PortalLoginID", "PortalLoginID", false, false, false);
      _fieldMap.AddMap("UserName", "UserName", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("OrganizationName", "OrganizationName", false, false, false);
      _fieldMap.AddMap("Success", "Success", false, false, false);
      _fieldMap.AddMap("LoginDateTime", "LoginDateTime", false, false, false);
      _fieldMap.AddMap("IPAddress", "IPAddress", false, false, false);
      _fieldMap.AddMap("Browser", "Browser", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("Source", "Source", false, false, false);
            
    }
  }
  
}
