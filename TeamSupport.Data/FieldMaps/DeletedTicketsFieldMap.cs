using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class DeletedTickets
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ID", "ID", false, false, true);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, true);
      _fieldMap.AddMap("TicketNumber", "TicketNumber", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, true);
      _fieldMap.AddMap("Name", "Name", false, false, true);
      _fieldMap.AddMap("DateDeleted", "DateDeleted", false, false, true);
      _fieldMap.AddMap("DeleterID", "DeleterID", false, false, true);
            
    }
  }
  
}
