using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class LoginAttempts
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("LoginAttemptID", "LoginAttemptID", true, true, true);
      _fieldMap.AddMap("UserID", "UserID", true, true, true);
      _fieldMap.AddMap("Successful", "Successful", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", true, true, true);
            
    }
  }
  
}
