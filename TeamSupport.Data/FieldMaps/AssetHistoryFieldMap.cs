using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class AssetHistory
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("HistoryID", "HistoryID", false, false, false);
      _fieldMap.AddMap("AssetID", "AssetID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ActionTime", "ActionTime", false, false, false);
      _fieldMap.AddMap("ActionDescription", "ActionDescription", false, true, false);
      _fieldMap.AddMap("ShippedFrom", "ShippedFrom", false, false, false);
      _fieldMap.AddMap("ShippedTo", "ShippedTo", false, true, false);
      _fieldMap.AddMap("TrackingNumber", "TrackingNumber", false, true, false);
      _fieldMap.AddMap("ShippingMethod", "ShippingMethod", false, true, false);
      _fieldMap.AddMap("ReferenceNum", "ReferenceNum", false, true, false);
      _fieldMap.AddMap("Comments", "Comments", false, true, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("Actor", "Actor", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, true, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("ShippedFromRefType", "ShippedFromRefType", false, false, false);
            
    }
  }
  
}
