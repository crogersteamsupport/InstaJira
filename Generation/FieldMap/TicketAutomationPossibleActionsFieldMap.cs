using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketAutomationPossibleActions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ActionID", "ActionID", false, false, false);
      _fieldMap.AddMap("DisplayName", "DisplayName", false, false, false);
      _fieldMap.AddMap("ActionName", "ActionName", false, false, false);
      _fieldMap.AddMap("RequireValue", "RequireValue", false, false, false);
      _fieldMap.AddMap("ValueList", "ValueList", false, false, false);
      _fieldMap.AddMap("ValueList2", "ValueList2", false, false, false);
      _fieldMap.AddMap("Active", "Active", false, false, false);
            
    }
  }
  
}
