using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class WebHooksToken
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("OrganizationId", "OrganizationId", false, false, false);
      _fieldMap.AddMap("Token", "Token", false, false, false);
      _fieldMap.AddMap("IsEnabled", "IsEnabled", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("CreatorId", "CreatorId", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("ModifierId", "ModifierId", false, false, false);
            
    }
  }
  
}
