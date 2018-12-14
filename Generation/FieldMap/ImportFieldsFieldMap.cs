using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ImportFields
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ImportFieldID", "ImportFieldID", false, false, false);
      _fieldMap.AddMap("TableName", "TableName", false, false, false);
      _fieldMap.AddMap("FieldName", "FieldName", false, false, false);
      _fieldMap.AddMap("Alias", "Alias", false, false, false);
      _fieldMap.AddMap("DataType", "DataType", false, false, false);
      _fieldMap.AddMap("Size", "Size", false, false, false);
      _fieldMap.AddMap("IsVisible", "IsVisible", false, false, false);
      _fieldMap.AddMap("IsRequired", "IsRequired", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("Enabled", "Enabled", false, false, false);
      _fieldMap.AddMap("Position", "Position", false, false, false);
            
    }
  }
  
}
