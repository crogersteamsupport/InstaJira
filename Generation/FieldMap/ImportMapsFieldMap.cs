using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ImportMaps
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ImportMapID", "ImportMapID", false, false, false);
      _fieldMap.AddMap("ImportID", "ImportID", false, false, false);
      _fieldMap.AddMap("SourceName", "SourceName", false, false, false);
      _fieldMap.AddMap("FieldID", "FieldID", false, false, false);
      _fieldMap.AddMap("IsCustom", "IsCustom", false, false, false);
            
    }
  }
  
}
