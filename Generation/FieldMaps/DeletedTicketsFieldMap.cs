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
      _fieldMap.AddMap("ID", "ID", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("TicketNumber", "TicketNumber", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Name", "Name", false, false, false);
      _fieldMap.AddMap("DateDeleted", "DateDeleted", false, false, false);
      _fieldMap.AddMap("DeleterID", "DeleterID", false, false, false);
            
    }
  }
  
}
