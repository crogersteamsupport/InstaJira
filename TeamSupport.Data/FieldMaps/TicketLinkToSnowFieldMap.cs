using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketLinkToSnow
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("DateModifiedBySync", "DateModifiedBySync", false, false, false);
      _fieldMap.AddMap("Sync", "Sync", false, false, false);
      _fieldMap.AddMap("AppId", "AppId", false, false, false);
      _fieldMap.AddMap("Number", "Number", false, false, false);
      _fieldMap.AddMap("URL", "URL", false, false, false);
      _fieldMap.AddMap("State", "State", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("CrmLinkID", "CrmLinkID", false, false, false);
            
    }
  }
  
}
