using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class UserDevices
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("UserDeviceID", "UserDeviceID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("DeviceID", "DeviceID", false, false, false);
      _fieldMap.AddMap("DateActivated", "DateActivated", false, false, false);
      _fieldMap.AddMap("IsActivated", "IsActivated", false, false, false);
            
    }
  }
  
}
