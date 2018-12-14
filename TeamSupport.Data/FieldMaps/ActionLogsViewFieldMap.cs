using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ActionLogsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ActionLogID", "ActionLogID", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, true);
      _fieldMap.AddMap("RefType", "RefType", true, true, true);
      _fieldMap.AddMap("RefID", "RefID", true, true, true);
      _fieldMap.AddMap("ActionLogType", "ActionLogType", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("Actor", "Actor", true, true, true);
            
    }
  }
  
}
