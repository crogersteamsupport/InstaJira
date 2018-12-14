using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class SearchSorters
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("SorterID", "SorterID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("TableID", "TableID", false, false, false);
      _fieldMap.AddMap("FieldID", "FieldID", false, false, false);
      _fieldMap.AddMap("Descending", "Descending", false, false, false);
            
    }
  }
  
}
