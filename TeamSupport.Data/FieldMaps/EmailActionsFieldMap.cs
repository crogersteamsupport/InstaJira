using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class EmailActions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("EMailActionID", "EMailActionID", false, false, false);
      _fieldMap.AddMap("DateTime", "DateTime", false, false, false);
      _fieldMap.AddMap("EMailFrom", "EMailFrom", false, false, false);
      _fieldMap.AddMap("EMailTo", "EMailTo", false, false, false);
      _fieldMap.AddMap("EMailSubject", "EMailSubject", false, false, false);
      _fieldMap.AddMap("EMailBody", "EMailBody", false, false, false);
      _fieldMap.AddMap("OrganizationGUID", "OrganizationGUID", false, false, false);
      _fieldMap.AddMap("ActionAdded", "ActionAdded", false, false, false);
      _fieldMap.AddMap("Status", "Status", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
            
    }
  }
  
}
