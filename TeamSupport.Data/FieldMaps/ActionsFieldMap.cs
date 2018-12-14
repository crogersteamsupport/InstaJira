using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Actions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ActionID", "ActionID", false, false, true);
      _fieldMap.AddMap("ActionTypeID", "ActionTypeID", true, true, true);
      _fieldMap.AddMap("SystemActionTypeID", "SystemActionTypeID", true, true, true);
      _fieldMap.AddMap("Name", "Name", true, true, true);
      _fieldMap.AddMap("Description", "Description", true, true, true);
      _fieldMap.AddMap("TimeSpent", "TimeSpent", true, true, true);
      _fieldMap.AddMap("DateStarted", "DateStarted", true, true, true);
      _fieldMap.AddMap("IsVisibleOnPortal", "IsVisibleOnPortal", true, true, true);
      _fieldMap.AddMap("IsKnowledgeBase", "IsKnowledgeBase", true, true, true);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
      _fieldMap.AddMap("DateModified", "DateModified", true, true, true);
      _fieldMap.AddMap("CreatorID", "CreatorID", true, true, true);
      _fieldMap.AddMap("ModifierID", "ModifierID", true, true, true);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, true);
      _fieldMap.AddMap("SalesForceID", "SalesForceID", false, false, false);
      _fieldMap.AddMap("DateModifiedBySalesForceSync", "DateModifiedBySalesForceSync", false, false, false);
      _fieldMap.AddMap("Pinned", "Pinned", false, false, false);
      _fieldMap.AddMap("ImportFileID", "ImportFileID", false, false, false);
            
    }
  }
  
}
