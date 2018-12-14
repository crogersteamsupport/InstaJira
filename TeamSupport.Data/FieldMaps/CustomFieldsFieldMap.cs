using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomFields
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomFieldID", "CustomFieldID", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, true);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("ApiFieldName", "ApiFieldName", true, true, true);
      _fieldMap.AddMap("RefType", "RefType", true, true, true);
      _fieldMap.AddMap("FieldType", "FieldType", true, true, true);
      _fieldMap.AddMap("AuxID", "AuxID", true, true, true);
      _fieldMap.AddMap("Position", "Position", true, true, true);
      _fieldMap.AddMap("ListValues", "ListValues", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("IsRequiredToClose", "IsRequiredToClose", false, false, false);
      _fieldMap.AddMap("Mask", "Mask", false, false, false);
      _fieldMap.AddMap("ParentCustomFieldID", "ParentCustomFieldID", false, false, false);
      _fieldMap.AddMap("ParentCustomValue", "ParentCustomValue", false, false, false);
      _fieldMap.AddMap("ParentProductID", "ParentProductID", false, false, false);
            
    }
  }
  
}
