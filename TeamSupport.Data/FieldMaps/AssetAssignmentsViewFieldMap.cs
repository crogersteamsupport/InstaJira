using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class AssetAssignmentsView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AssetAssignmentsID", "AssetAssignmentsID", false, false, false);
      _fieldMap.AddMap("HistoryID", "HistoryID", false, false, false);
      _fieldMap.AddMap("AssetID", "AssetID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ActionTime", "ActionTime", false, false, false);
      _fieldMap.AddMap("ActionDescription", "ActionDescription", false, false, false);
      _fieldMap.AddMap("ShippedFrom", "ShippedFrom", false, false, false);
      _fieldMap.AddMap("ShippedTo", "ShippedTo", false, false, false);
      _fieldMap.AddMap("NameAssignedTo", "NameAssignedTo", false, false, false);
      _fieldMap.AddMap("TrackingNumber", "TrackingNumber", false, false, false);
      _fieldMap.AddMap("ShippingMethod", "ShippingMethod", false, false, false);
      _fieldMap.AddMap("ReferenceNum", "ReferenceNum", false, false, false);
      _fieldMap.AddMap("Comments", "Comments", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("Actor", "Actor", false, false, false);
      _fieldMap.AddMap("ActorName", "ActorName", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("ModifierName", "ModifierName", false, false, false);
            
    }
  }
  
}
