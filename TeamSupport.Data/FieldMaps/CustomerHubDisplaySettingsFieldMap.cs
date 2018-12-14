using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomerHubDisplaySettings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomerHubDisplaySettingID", "CustomerHubDisplaySettingID", false, false, false);
      _fieldMap.AddMap("CustomerHubID", "CustomerHubID", false, false, false);
      _fieldMap.AddMap("FontFamily", "FontFamily", false, false, false);
      _fieldMap.AddMap("FontColor", "FontColor", false, false, false);
      _fieldMap.AddMap("Color1", "Color1", false, false, false);
      _fieldMap.AddMap("Color2", "Color2", false, false, false);
      _fieldMap.AddMap("Color3", "Color3", false, false, false);
      _fieldMap.AddMap("Color4", "Color4", false, false, false);
      _fieldMap.AddMap("Color5", "Color5", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
