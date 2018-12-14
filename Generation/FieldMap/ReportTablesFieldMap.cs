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
      _fieldMap.AddMap("ReportTableID", "ReportTableID", false, false, false);
      _fieldMap.AddMap("TableName", "TableName", false, false, false);
      _fieldMap.AddMap("Alias", "Alias", false, false, false);
      _fieldMap.AddMap("CustomFieldRefType", "CustomFieldRefType", false, false, false);
      _fieldMap.AddMap("IsCategory", "IsCategory", false, false, false);
      _fieldMap.AddMap("OrganizationIDFieldName", "OrganizationIDFieldName", false, false, false);
      _fieldMap.AddMap("LookupKeyFieldName", "LookupKeyFieldName", false, false, false);
      _fieldMap.AddMap("LookupDisplayClause", "LookupDisplayClause", false, false, false);
      _fieldMap.AddMap("LookupOrderBy", "LookupOrderBy", false, false, false);
      _fieldMap.AddMap("UseTicketRights", "UseTicketRights", false, false, false);
            
    }
  }
  
}
