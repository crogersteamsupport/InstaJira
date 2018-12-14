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
      _fieldMap.AddMap("AssetID", "AssetID", false, false, true);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, true);
      _fieldMap.AddMap("ProductName", "ProductName", false, false, true);
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", false, false, true);
      _fieldMap.AddMap("ProductVersionNumber", "ProductVersionNumber", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("SerialNumber", "SerialNumber", false, false, true);
      _fieldMap.AddMap("Name", "Name", false, false, true);
      _fieldMap.AddMap("Location", "Location", false, false, true);
      _fieldMap.AddMap("Notes", "Notes", false, false, true);
      _fieldMap.AddMap("WarrantyExpiration", "WarrantyExpiration", false, false, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, true);
      _fieldMap.AddMap("CreatorName", "CreatorName", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, true);
      _fieldMap.AddMap("SubPartOf", "SubPartOf", false, false, false);
      _fieldMap.AddMap("Status", "Status", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
            
    }
  }
  
}
