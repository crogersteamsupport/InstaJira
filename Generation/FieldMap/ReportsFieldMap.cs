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
      _fieldMap.AddMap("ReportID", "ReportID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("Query", "Query", false, false, false);
      _fieldMap.AddMap("CustomFieldKeyName", "CustomFieldKeyName", false, false, false);
      _fieldMap.AddMap("CustomRefType", "CustomRefType", false, false, false);
      _fieldMap.AddMap("CustomAuxID", "CustomAuxID", false, false, false);
      _fieldMap.AddMap("ReportSubcategoryID", "ReportSubcategoryID", false, false, false);
      _fieldMap.AddMap("QueryObject", "QueryObject", false, false, false);
      _fieldMap.AddMap("ExternalURL", "ExternalURL", false, false, false);
      _fieldMap.AddMap("LastSqlExecuted", "LastSqlExecuted", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("ReportType", "ReportType", false, false, false);
      _fieldMap.AddMap("ReportDef", "ReportDef", false, false, false);
      _fieldMap.AddMap("ReportDefType", "ReportDefType", false, false, false);
      _fieldMap.AddMap("DateEdited", "DateEdited", false, false, false);
      _fieldMap.AddMap("EditorID", "EditorID", false, false, false);
      _fieldMap.AddMap("FolderID", "FolderID", false, false, false);
      _fieldMap.AddMap("IsPrivate", "IsPrivate", false, false, false);
            
    }
  }
  
}
