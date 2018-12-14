using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class FullContactUpdates
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("UserId", "UserId", false, false, false);
      _fieldMap.AddMap("OrganizationId", "OrganizationId", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
            
    }
  }
  
}
