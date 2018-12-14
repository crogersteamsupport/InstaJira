using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class TSEMailIgnoreList
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("IgnoreID", "IgnoreID", false, false, false);
      _fieldMap.AddMap("FromAddress", "FromAddress", false, false, false);
      _fieldMap.AddMap("ToAddress", "ToAddress", false, false, false);
            
    }
  }
  
}
