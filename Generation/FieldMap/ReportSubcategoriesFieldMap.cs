using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ReportSubcategories
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ReportSubcategoryID", "ReportSubcategoryID", false, false, false);
      _fieldMap.AddMap("ReportCategoryTableID", "ReportCategoryTableID", false, false, false);
      _fieldMap.AddMap("ReportTableID", "ReportTableID", false, false, false);
      _fieldMap.AddMap("BaseQuery", "BaseQuery", false, false, false);
            
    }
  }
  
}
