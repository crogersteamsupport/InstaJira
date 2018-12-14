using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ActionLinkToJira
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("id", "id", false, false, false);
      _fieldMap.AddMap("ActionID", "ActionID", false, false, false);
      _fieldMap.AddMap("DateModifiedByJiraSync", "DateModifiedByJiraSync", false, false, false);
      _fieldMap.AddMap("JiraID", "JiraID", false, false, false);
            
    }
  }
  
}
