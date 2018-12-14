using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ProductVersionsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", false, false, false);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, false);
      _fieldMap.AddMap("ProductVersionStatusID", "ProductVersionStatusID", false, false, false);
      _fieldMap.AddMap("VersionNumber", "VersionNumber", false, false, false);
      _fieldMap.AddMap("ReleaseDate", "ReleaseDate", false, false, false);
      _fieldMap.AddMap("IsReleased", "IsReleased", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("VersionStatus", "VersionStatus", false, false, false);
      _fieldMap.AddMap("ProductName", "ProductName", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ProductFamilyID", "ProductFamilyID", false, false, false);
      _fieldMap.AddMap("JiraProjectKey", "JiraProjectKey", false, false, false);
            
    }
  }
  
}
