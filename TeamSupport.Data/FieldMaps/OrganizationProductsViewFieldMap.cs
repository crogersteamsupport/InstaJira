using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class OrganizationProductsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Product", "Product", false, false, true);
      _fieldMap.AddMap("VersionStatus", "VersionStatus", false, false, true);
      _fieldMap.AddMap("IsShipping", "IsShipping", false, false, false);
      _fieldMap.AddMap("IsDiscontinued", "IsDiscontinued", false, false, false);
      _fieldMap.AddMap("VersionNumber", "VersionNumber", false, false, true);
      _fieldMap.AddMap("ProductVersionStatusID", "ProductVersionStatusID", false, false, false);
      _fieldMap.AddMap("ReleaseDate", "ReleaseDate", false, false, true);
      _fieldMap.AddMap("IsReleased", "IsReleased", true, true, true);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("OrganizationProductID", "OrganizationProductID", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, true);
      _fieldMap.AddMap("OrganizationName", "OrganizationName", false, false, false);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, true);
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", true, true, true);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", false, false, false);
      _fieldMap.AddMap("SupportExpiration", "SupportExpiration", true, true, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
            
    }
  }
  
}
