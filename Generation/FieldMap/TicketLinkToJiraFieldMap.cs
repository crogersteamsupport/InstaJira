using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketLinkToJira
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("id", "id", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("DateModifiedByJiraSync", "DateModifiedByJiraSync", false, false, false);
      _fieldMap.AddMap("SyncWithJira", "SyncWithJira", false, false, false);
      _fieldMap.AddMap("JiraID", "JiraID", false, false, false);
      _fieldMap.AddMap("JiraKey", "JiraKey", false, false, false);
      _fieldMap.AddMap("JiraLinkURL", "JiraLinkURL", false, false, false);
      _fieldMap.AddMap("JiraStatus", "JiraStatus", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("CrmLinkID", "CrmLinkID", false, false, false);
            
    }
  }
  
}
