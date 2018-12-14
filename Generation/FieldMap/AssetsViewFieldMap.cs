using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class AssetsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AssetID", "AssetID", false, false, false);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, false);
      _fieldMap.AddMap("ProductName", "ProductName", false, false, false);
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", false, false, false);
      _fieldMap.AddMap("ProductVersionNumber", "ProductVersionNumber", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("SerialNumber", "SerialNumber", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("Location", "Location", false, false, false);
      _fieldMap.AddMap("Notes", "Notes", false, false, false);
      _fieldMap.AddMap("WarrantyExpiration", "WarrantyExpiration", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("CreatorName", "CreatorName", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, false);
      _fieldMap.AddMap("SubPartOf", "SubPartOf", false, false, false);
      _fieldMap.AddMap("Status", "Status", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
            
    }
  }
  
}
