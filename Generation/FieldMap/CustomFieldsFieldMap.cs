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
      _fieldMap.AddMap("CustomFieldID", "CustomFieldID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("ApiFieldName", "ApiFieldName", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("FieldType", "FieldType", false, false, false);
      _fieldMap.AddMap("AuxID", "AuxID", false, false, false);
      _fieldMap.AddMap("Position", "Position", false, false, false);
      _fieldMap.AddMap("ListValues", "ListValues", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", false, false, false);
      _fieldMap.AddMap("IsFirstIndexSelect", "IsFirstIndexSelect", false, false, false);
      _fieldMap.AddMap("IsRequired", "IsRequired", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("CustomFieldCategoryID", "CustomFieldCategoryID", false, false, false);
      _fieldMap.AddMap("IsRequiredToClose", "IsRequiredToClose", false, false, false);
      _fieldMap.AddMap("Mask", "Mask", false, false, false);
      _fieldMap.AddMap("ParentCustomFieldID", "ParentCustomFieldID", false, false, false);
      _fieldMap.AddMap("ParentCustomValue", "ParentCustomValue", false, false, false);
      _fieldMap.AddMap("ParentProductID", "ParentProductID", false, false, false);
            
    }
  }
  
}
