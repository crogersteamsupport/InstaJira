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
      _fieldMap.AddMap("ReportTableFieldID", "ReportTableFieldID", true, true, true);
      _fieldMap.AddMap("ReportTableID", "ReportTableID", true, true, true);
      _fieldMap.AddMap("FieldName", "FieldName", true, true, true);
      _fieldMap.AddMap("Alias", "Alias", true, true, true);
      _fieldMap.AddMap("DataType", "DataType", true, true, true);
      _fieldMap.AddMap("Size", "Size", true, true, true);
      _fieldMap.AddMap("IsVisible", "IsVisible", true, true, true);
            
    }
  }
  
}
