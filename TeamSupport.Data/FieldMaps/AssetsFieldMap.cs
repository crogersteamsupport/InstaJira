using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Assets
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AssetID", "AssetID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("SerialNumber", "SerialNumber", true, true, false);
      _fieldMap.AddMap("Name", "Name", true, true, false);
      _fieldMap.AddMap("Location", "Location", true, false, false);
      _fieldMap.AddMap("Notes", "Notes", true, true, false);
      _fieldMap.AddMap("ProductID", "ProductID", true, true, false);
      _fieldMap.AddMap("WarrantyExpiration", "WarrantyExpiration", true, true, false);
      _fieldMap.AddMap("AssignedTo", "AssignedTo", true, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("SubPartOf", "SubPartOf", false, false, false);
      _fieldMap.AddMap("Status", "Status", false, false, false);
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", true, true, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
            
    }
  }
  
}
