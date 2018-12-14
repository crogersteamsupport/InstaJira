using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class BackdoorLogins
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("BackdoorLoginID", "BackdoorLoginID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("Token", "Token", false, false, false);
      _fieldMap.AddMap("DateIssued", "DateIssued", false, false, false);
      _fieldMap.AddMap("ContactID", "ContactID", false, false, false);
            
    }
  }
  
}
