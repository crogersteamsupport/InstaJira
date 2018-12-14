using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ReportFields
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ReportFieldID", "ReportFieldID", true, true, true);
      _fieldMap.AddMap("ReportID", "ReportID", true, true, true);
      _fieldMap.AddMap("LinkedFieldID", "LinkedFieldID", true, true, true);
      _fieldMap.AddMap("IsCustomField", "IsCustomField", true, true, true);
            
    }
  }
  
}
