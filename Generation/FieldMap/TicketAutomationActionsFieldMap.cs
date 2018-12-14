using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketAutomationActions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketActionID", "TicketActionID", false, false, false);
      _fieldMap.AddMap("TriggerID", "TriggerID", false, false, false);
      _fieldMap.AddMap("ActionID", "ActionID", false, false, false);
      _fieldMap.AddMap("ActionValue", "ActionValue", false, false, false);
      _fieldMap.AddMap("ActionValue2", "ActionValue2", false, false, false);
            
    }
  }
  
}
