using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ReportViews
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ReportViewID", "ReportViewID", false, false, false);
      _fieldMap.AddMap("ReportID", "ReportID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("DateViewed", "DateViewed", false, false, false);
      _fieldMap.AddMap("DurationToLoad", "DurationToLoad", false, false, false);
      _fieldMap.AddMap("SQLExecuted", "SQLExecuted", false, false, false);
      _fieldMap.AddMap("ErrorMessage", "ErrorMessage", false, false, false);
            
    }
  }
  
}
