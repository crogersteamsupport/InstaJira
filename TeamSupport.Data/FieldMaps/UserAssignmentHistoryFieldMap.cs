using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class UserAssignmentHistory
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("UserAssignmentHistoryID", "UserAssignmentHistoryID", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("DateAssigned", "DateAssigned", false, false, false);
            
    }
  }
  
}
