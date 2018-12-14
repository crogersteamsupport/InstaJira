using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ReportTableFields
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ReportTableFieldID", "ReportTableFieldID", false, false, false);
      _fieldMap.AddMap("ReportTableID", "ReportTableID", false, false, false);
      _fieldMap.AddMap("FieldName", "FieldName", false, false, false);
      _fieldMap.AddMap("Alias", "Alias", false, false, false);
      _fieldMap.AddMap("DataType", "DataType", false, false, false);
      _fieldMap.AddMap("Size", "Size", false, false, false);
      _fieldMap.AddMap("IsVisible", "IsVisible", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("LookupTableID", "LookupTableID", false, false, false);
      _fieldMap.AddMap("IsReadOnly", "IsReadOnly", false, false, false);
      _fieldMap.AddMap("IsOpenable", "IsOpenable", false, false, false);
      _fieldMap.AddMap("IsEmail", "IsEmail", false, false, false);
      _fieldMap.AddMap("IsLink", "IsLink", false, false, false);
      _fieldMap.AddMap("IsSortable", "IsSortable", false, false, false);
            
    }
  }
  
}
