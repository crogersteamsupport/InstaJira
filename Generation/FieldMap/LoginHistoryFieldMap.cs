using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class LoginHistory
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("LoginHistoryID", "LoginHistoryID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("IPAddress", "IPAddress", false, false, false);
      _fieldMap.AddMap("Browser", "Browser", false, false, false);
      _fieldMap.AddMap("Version", "Version", false, false, false);
      _fieldMap.AddMap("MajorVersion", "MajorVersion", false, false, false);
      _fieldMap.AddMap("CookiesEnabled", "CookiesEnabled", false, false, false);
      _fieldMap.AddMap("Platform", "Platform", false, false, false);
      _fieldMap.AddMap("UserAgent", "UserAgent", false, false, false);
      _fieldMap.AddMap("Language", "Language", false, false, false);
      _fieldMap.AddMap("PixelDepth", "PixelDepth", false, false, false);
      _fieldMap.AddMap("ScreenHeight", "ScreenHeight", false, false, false);
      _fieldMap.AddMap("ScreenWidth", "ScreenWidth", false, false, false);
      _fieldMap.AddMap("URL", "URL", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("IsSupport", "IsSupport", false, false, false);
      _fieldMap.AddMap("DeviceID", "DeviceID", false, false, false);
            
    }
  }
  
}
