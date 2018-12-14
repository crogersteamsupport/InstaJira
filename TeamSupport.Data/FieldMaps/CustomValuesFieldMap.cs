using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomValues
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomValueID", "CustomValueID", true, true, true);
      _fieldMap.AddMap("CustomFieldID", "CustomFieldID", true, true, true);
      _fieldMap.AddMap("RefID", "RefID", true, true, true);
      _fieldMap.AddMap("CustomValue", "CustomValue", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
            
    }
  }
  
}
