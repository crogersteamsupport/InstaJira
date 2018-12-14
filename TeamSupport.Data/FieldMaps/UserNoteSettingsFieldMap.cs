using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class UserNoteSettings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("IsDismissed", "IsDismissed", false, false, false);
      _fieldMap.AddMap("IsSnoozed", "IsSnoozed", false, false, false);
      _fieldMap.AddMap("SnoozeTime", "SnoozeTime", false, false, false);
            
    }
  }
  
}
