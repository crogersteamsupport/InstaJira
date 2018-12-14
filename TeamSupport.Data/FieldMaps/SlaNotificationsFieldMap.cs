using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SlaNotifications
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("TimeClosedViolationDate", "TimeClosedViolationDate", false, false, false);
      _fieldMap.AddMap("LastActionViolationDate", "LastActionViolationDate", false, false, false);
      _fieldMap.AddMap("InitialResponseViolationDate", "InitialResponseViolationDate", false, false, false);
      _fieldMap.AddMap("TimeClosedWarningDate", "TimeClosedWarningDate", false, false, false);
      _fieldMap.AddMap("LastActionWarningDate", "LastActionWarningDate", false, false, false);
      _fieldMap.AddMap("InitialResponseWarningDate", "InitialResponseWarningDate", false, false, false);
            
    }
  }
  
}
