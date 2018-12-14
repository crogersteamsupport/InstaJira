using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ReportTables
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ReportTableID", "ReportTableID", true, true, true);
      _fieldMap.AddMap("TableName", "TableName", true, true, true);
      _fieldMap.AddMap("Alias", "Alias", true, true, true);
      _fieldMap.AddMap("CustomFieldRefType", "CustomFieldRefType", true, true, true);
      _fieldMap.AddMap("IsCategory", "IsCategory", true, true, true);
      _fieldMap.AddMap("OrganizationIDFieldName", "OrganizationIDFieldName", true, true, true);
            
    }
  }
  
}
