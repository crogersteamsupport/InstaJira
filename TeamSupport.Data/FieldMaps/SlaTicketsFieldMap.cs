using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SlaTickets
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketId", "TicketId", false, false, false);
      _fieldMap.AddMap("SlaTriggerId", "SlaTriggerId", false, false, false);
      _fieldMap.AddMap("IsPending", "IsPending", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
            
    }
  }
  
}
