using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TicketNextStatuses
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("TicketNextStatusID", "TicketNextStatusID", false, false, false);
      _fieldMap.AddMap("CurrentStatusID", "CurrentStatusID", false, false, false);
      _fieldMap.AddMap("NextStatusID", "NextStatusID", false, false, false);
      _fieldMap.AddMap("Position", "Position", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
