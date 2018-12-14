using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SlaPausedTimes
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("TicketId", "TicketId", false, false, false);
      _fieldMap.AddMap("TicketStatusId", "TicketStatusId", false, false, false);
      _fieldMap.AddMap("SlaTriggerId", "SlaTriggerId", false, false, false);
      _fieldMap.AddMap("PausedOn", "PausedOn", false, false, false);
      _fieldMap.AddMap("ResumedOn", "ResumedOn", false, false, false);
      _fieldMap.AddMap("BusinessPausedTime", "BusinessPausedTime", false, false, false);
            
    }
  }
  
}
