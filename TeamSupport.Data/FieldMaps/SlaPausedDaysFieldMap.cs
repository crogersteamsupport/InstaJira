using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SlaPausedDays
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("Id", "Id", false, false, false);
      _fieldMap.AddMap("SlaTriggerId", "SlaTriggerId", false, false, false);
      _fieldMap.AddMap("DateToPause", "DateToPause", false, false, false);
            
    }
  }
  
}
