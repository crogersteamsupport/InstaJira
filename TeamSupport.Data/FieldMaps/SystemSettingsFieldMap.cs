using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SystemSettings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("SystemSettingID", "SystemSettingID", false, false, false);
      _fieldMap.AddMap("SettingKey", "SettingKey", false, false, false);
      _fieldMap.AddMap("SettingValue", "SettingValue", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
