using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class FilePaths
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ID", "ID", false, false, false);
      _fieldMap.AddMap("Value", "Value", false, false, false);
            
    }
  }
  
}
