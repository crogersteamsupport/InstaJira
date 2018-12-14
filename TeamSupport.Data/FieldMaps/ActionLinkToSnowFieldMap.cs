using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ActionLinkToSnow
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("ActionID", "ActionID", false, false, false);
      _fieldMap.AddMap("DateModifiedBySync", "DateModifiedBySync", false, false, false);
      _fieldMap.AddMap("AppId", "AppId", false, false, false);
            
    }
  }
  
}
