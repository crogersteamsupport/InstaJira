using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ActionLinkToTFS
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("id", "id", false, false, false);
      _fieldMap.AddMap("ActionID", "ActionID", false, false, false);
      _fieldMap.AddMap("DateModifiedByTFSSync", "DateModifiedByTFSSync", false, false, false);
      _fieldMap.AddMap("TFSID", "TFSID", false, false, false);
            
    }
  }
  
}
