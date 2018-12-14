using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class OrganizationProducts
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationProductID", "OrganizationProductID", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", true, false, true);
      _fieldMap.AddMap("ProductID", "ProductID", true, false, true);
      _fieldMap.AddMap("ProductVersionID", "ProductVersionID", true, true, true);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", false, false, false);
      _fieldMap.AddMap("SupportExpiration", "SupportExpiration", true, true, true);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, false, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
      _fieldMap.AddMap("SlaLevelID", "SlaLevelID", false, false, false);
            
    }
  }
  
}
