using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class WebHooksPending
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("OrganizationId", "OrganizationId", false, false, false);
      _fieldMap.AddMap("RefType", "RefType", false, false, false);
      _fieldMap.AddMap("RefId", "RefId", false, false, false);
      _fieldMap.AddMap("Type", "Type", false, false, false);
      _fieldMap.AddMap("Url", "Url", false, false, false);
      _fieldMap.AddMap("BodyData", "BodyData", false, false, false);
      _fieldMap.AddMap("Token", "Token", false, false, false);
      _fieldMap.AddMap("Inbound", "Inbound", false, false, false);
      _fieldMap.AddMap("IsProcessing", "IsProcessing", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
            
    }
  }
  
}
