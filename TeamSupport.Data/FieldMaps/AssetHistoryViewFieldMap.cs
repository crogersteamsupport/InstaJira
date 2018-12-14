using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class AssetHistoryView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("HistoryID", "HistoryID", false, false, false);
      _fieldMap.AddMap("AssetID", "AssetID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ActionTime", "ActionTime", false, false, true);
      _fieldMap.AddMap("ActionDescription", "ActionDescription", false, false, true);
      _fieldMap.AddMap("ShippedFrom", "ShippedFrom", false, false, true);
      _fieldMap.AddMap("NameAssignedFrom", "NameAssignedFrom", false, false, true);
      _fieldMap.AddMap("ShippedTo", "ShippedTo", false, false, true);
      _fieldMap.AddMap("NameAssignedTo", "NameAssignedTo", false, false, true);
      _fieldMap.AddMap("TrackingNumber", "TrackingNumber", false, false, true);
      _fieldMap.AddMap("ShippingMethod", "ShippingMethod", false, false, true);
      _fieldMap.AddMap("ReferenceNum", "ReferenceNum", false, false, true);
      _fieldMap.AddMap("Comments", "Comments", false, false, true);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, true);
      _fieldMap.AddMap("Actor", "Actor", false, false, true);
      _fieldMap.AddMap("ActorName", "ActorName", false, false, true);
      _fieldMap.AddMap("RefType", "RefType", false, false, true);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, true);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, true);
      _fieldMap.AddMap("ShippedFromRefType", "ShippedFromRefType", false, false, true);
            
    }
  }
  
}
