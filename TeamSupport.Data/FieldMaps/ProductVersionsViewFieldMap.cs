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
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", true, true, true);
      _fieldMap.AddMap("ProductID", "ProductID", true, true, true);
      _fieldMap.AddMap("ProductVersionStatusID", "ProductVersionStatusID", true, true, true);
      _fieldMap.AddMap("VersionNumber", "VersionNumber", true, true, true);
      _fieldMap.AddMap("ReleaseDate", "ReleaseDate", true, true, true);
      _fieldMap.AddMap("IsReleased", "IsReleased", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("VersionStatus", "VersionStatus", true, true, true);
      _fieldMap.AddMap("ProductName", "ProductName", true, true, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, true, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("ProductFamilyID", "ProductLineID", false, false, false);
      _fieldMap.AddMap("JiraProjectKey", "JiraProjectKey", false, false, false);
      _fieldMap.AddMap("TFSProjectName", "TFSProjectName", false, false, false);
            
    }
  }
  
}
