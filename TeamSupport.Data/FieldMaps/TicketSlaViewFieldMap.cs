using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketSlaView
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketID", "TicketID", true, true, true);
      _fieldMap.AddMap("ViolationTimeClosed", "ViolationTimeClosed", true, true, true);
      _fieldMap.AddMap("ViolationLastAction", "ViolationLastAction", true, true, true);
      _fieldMap.AddMap("ViolationInitialResponse", "ViolationInitialResponse", true, true, true);
      _fieldMap.AddMap("WarningTimeClosed", "WarningTimeClosed", true, true, true);
      _fieldMap.AddMap("WarningLastAction", "WarningLastAction", true, true, true);
      _fieldMap.AddMap("WarningInitialResponse", "WarningInitialResponse", true, true, true);
            
    }
  }
  
}
