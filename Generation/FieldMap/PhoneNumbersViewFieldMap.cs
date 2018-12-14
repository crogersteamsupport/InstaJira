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
      _fieldMap.AddMap("PhoneID", "PhoneID", false, false, false);
      _fieldMap.AddMap("PhoneTypeID", "PhoneTypeID", false, false, false);
      _fieldMap.AddMap("RefID", "RefID", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("PhoneNumber", "PhoneNumber", false, false, false);
      _fieldMap.AddMap("Extension", "Extension", false, false, false);
      _fieldMap.AddMap("OtherTypeName", "OtherTypeName", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("PhoneType", "PhoneType", false, false, false);
      _fieldMap.AddMap("CreatorName", "CreatorName", false, false, false);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, false);
            
    }
  }
  
}
