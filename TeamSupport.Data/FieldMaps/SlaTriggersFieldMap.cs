using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SlaTriggers
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("SlaTriggerID", "SlaTriggerID", false, false, false);
      _fieldMap.AddMap("SlaLevelID", "SlaLevelID", false, false, false);
      _fieldMap.AddMap("TicketTypeID", "TicketTypeID", false, false, false);
      _fieldMap.AddMap("TicketSeverityID", "TicketSeverityID", false, false, false);
      _fieldMap.AddMap("TimeInitialResponse", "TimeInitialResponse", false, false, false);
      _fieldMap.AddMap("TimeLastAction", "TimeLastAction", false, false, false);
      _fieldMap.AddMap("TimeToClose", "TimeToClose", false, false, false);
      _fieldMap.AddMap("NotifyUserOnWarning", "NotifyUserOnWarning", false, false, false);
      _fieldMap.AddMap("NotifyGroupOnWarning", "NotifyGroupOnWarning", false, false, false);
      _fieldMap.AddMap("NotifyUserOnViolation", "NotifyUserOnViolation", false, false, false);
      _fieldMap.AddMap("NotifyGroupOnViolation", "NotifyGroupOnViolation", false, false, false);
      _fieldMap.AddMap("WarningTime", "WarningTime", false, false, false);
      _fieldMap.AddMap("UseBusinessHours", "UseBusinessHours", false, false, false);
            
    }
  }
  
}
