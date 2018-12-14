using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class PhoneNumbersView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("PhoneID", "PhoneID", false, false, true);
      _fieldMap.AddMap("PhoneTypeID", "PhoneTypeID", true, true, true);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("PhoneNumber", "Number", true, true, true);
      _fieldMap.AddMap("Extension", "Extension", true, true, true);
      _fieldMap.AddMap("OtherTypeName", "OtherTypeName", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("PhoneType", "PhoneType", true, true, true);
      _fieldMap.AddMap("CreatorName", "CreatorName", false, false, true);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, true);
            
    }
  }
  
}
