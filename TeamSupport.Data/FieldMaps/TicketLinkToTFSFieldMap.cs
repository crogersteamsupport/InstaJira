using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketLinkToTFS
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("id", "id", false, false, false);
      _fieldMap.AddMap("TicketID", "TicketID", false, false, false);
      _fieldMap.AddMap("DateModifiedByTFSSync", "DateModifiedByTFSSync", false, false, false);
      _fieldMap.AddMap("SyncWithTFS", "SyncWithTFS", false, false, false);
      _fieldMap.AddMap("TFSID", "TFSID", false, false, false);
      _fieldMap.AddMap("TFSTitle", "TFSTitle", false, false, false);
      _fieldMap.AddMap("TFSURL", "TFSURL", false, false, false);
      _fieldMap.AddMap("TFSState", "TFSState", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("CrmLinkID", "CrmLinkID", false, false, false);
            
    }
  }
  
}
