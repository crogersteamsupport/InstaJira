using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Reports
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ReportID", "ReportID", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, true);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("Query", "Query", true, true, true);
      _fieldMap.AddMap("CustomFieldKeyName", "CustomFieldKeyName", true, true, true);
      _fieldMap.AddMap("CustomRefType", "CustomRefType", true, true, true);
      _fieldMap.AddMap("CustomAuxID", "CustomAuxID", true, true, true);
      _fieldMap.AddMap("ReportSubcategoryID", "ReportSubcategoryID", true, true, true);
      _fieldMap.AddMap("QueryObject", "QueryObject", true, true, true);
      _fieldMap.AddMap("ExternalURL", "ExternalURL", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
            
    }
  }
  
}
