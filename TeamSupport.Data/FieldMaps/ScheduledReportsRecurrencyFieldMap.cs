using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ScheduledReportsRecurrency
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("id", "id", false, false, false);
      _fieldMap.AddMap("recurrency", "recurrency", false, false, false);
            
    }
  }
  
}
