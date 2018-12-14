using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ForumLogs
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ForumLogID", "ForumLogID", false, false, false);
      _fieldMap.AddMap("TopicID", "TopicID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("OrgID", "OrgID", false, false, false);
      _fieldMap.AddMap("ViewTime", "ViewTime", false, false, false);
            
    }
  }
  
}
