using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class EmailPostHistory
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("EmailPostID", "EmailPostID", false, false, false);
      _fieldMap.AddMap("EmailPostType", "EmailPostType", false, false, false);
      _fieldMap.AddMap("HoldTime", "HoldTime", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("Param1", "Param1", false, false, false);
      _fieldMap.AddMap("Param2", "Param2", false, false, false);
      _fieldMap.AddMap("Param3", "Param3", false, false, false);
      _fieldMap.AddMap("Param4", "Param4", false, false, false);
      _fieldMap.AddMap("Param5", "Param5", false, false, false);
      _fieldMap.AddMap("Param6", "Param6", false, false, false);
      _fieldMap.AddMap("Param7", "Param7", false, false, false);
      _fieldMap.AddMap("Param8", "Param8", false, false, false);
      _fieldMap.AddMap("Param9", "Param9", false, false, false);
      _fieldMap.AddMap("Param10", "Param10", false, false, false);
      _fieldMap.AddMap("Text1", "Text1", false, false, false);
      _fieldMap.AddMap("Text2", "Text2", false, false, false);
      _fieldMap.AddMap("Text3", "Text3", false, false, false);
            
    }
  }
  
}
