using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class AgentRatingUsers
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("AgentRatingID", "AgentRatingID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
            
    }
  }
  
}
