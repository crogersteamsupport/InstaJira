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
      _fieldMap.AddMap("ReportFieldID", "ReportFieldID", false, false, false);
      _fieldMap.AddMap("ReportID", "ReportID", false, false, false);
      _fieldMap.AddMap("LinkedFieldID", "LinkedFieldID", false, false, false);
      _fieldMap.AddMap("IsCustomField", "IsCustomField", false, false, false);
            
    }
  }
  
}
