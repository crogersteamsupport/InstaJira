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
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("ViolationTimeClosed", "ViolationTimeClosed", false, false, false);
      _fieldMap.AddMap("WarningTimeClosed", "WarningTimeClosed", false, false, false);
      _fieldMap.AddMap("ViolationLastAction", "ViolationLastAction", false, false, false);
      _fieldMap.AddMap("WarningLastAction", "WarningLastAction", false, false, false);
      _fieldMap.AddMap("ViolationInitialResponse", "ViolationInitialResponse", false, false, false);
      _fieldMap.AddMap("WarningInitialResponse", "WarningInitialResponse", false, false, false);
            
    }
  }
  
}
