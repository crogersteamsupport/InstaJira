using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class UserProductsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Product", "Product", false, false, false);
      _fieldMap.AddMap("VersionStatus", "VersionStatus", false, false, false);
      _fieldMap.AddMap("IsShipping", "IsShipping", false, false, false);
      _fieldMap.AddMap("IsDiscontinued", "IsDiscontinued", false, false, false);
      _fieldMap.AddMap("VersionNumber", "VersionNumber", false, false, false);
      _fieldMap.AddMap("ProductVersionStatusID", "ProductVersionStatusID", false, false, false);
      _fieldMap.AddMap("ReleaseDate", "ReleaseDate", false, false, false);
      _fieldMap.AddMap("IsReleased", "IsReleased", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("UserProductID", "UserProductID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("UserName", "UserName", false, false, false);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, false);
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", false, false, false);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", false, false, false);
      _fieldMap.AddMap("SupportExpiration", "SupportExpiration", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
