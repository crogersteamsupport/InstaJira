using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ForumTickets
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("ForumCategory", "ForumCategory", false, false, false);
      _fieldMap.AddMap("ViewCount", "ViewCount", false, false, false);
            
    }
  }
  
}
