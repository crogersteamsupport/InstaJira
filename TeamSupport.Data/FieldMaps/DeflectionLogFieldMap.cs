using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class DeflectionLog
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("Source", "Source", false, false, false);
      _fieldMap.AddMap("Helpful", "Helpful", false, false, false);
      _fieldMap.AddMap("Date", "Date", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("OrgID", "OrgID", false, false, false);
            
    }
  }
  
}
