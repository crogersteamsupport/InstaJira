using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ApiLogs
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ApiLogID", "ApiLogID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("IPAddress", "IPAddress", false, false, false);
      _fieldMap.AddMap("Url", "Url", false, false, false);
      _fieldMap.AddMap("Verb", "Verb", false, false, false);
      _fieldMap.AddMap("StatusCode", "StatusCode", false, false, false);
      _fieldMap.AddMap("RequestBody", "RequestBody", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
            
    }
  }
  
}
