using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketAutomationTriggerLogic
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TriggerLogicID", "TriggerLogicID", false, false, false);
      _fieldMap.AddMap("TriggerID", "TriggerID", false, false, false);
      _fieldMap.AddMap("TableID", "TableID", false, false, false);
      _fieldMap.AddMap("FieldID", "FieldID", false, false, false);
      _fieldMap.AddMap("Measure", "Measure", false, false, false);
      _fieldMap.AddMap("TestValue", "TestValue", false, false, false);
      _fieldMap.AddMap("MatchAll", "MatchAll", false, false, false);
            
    }
  }
  
}
