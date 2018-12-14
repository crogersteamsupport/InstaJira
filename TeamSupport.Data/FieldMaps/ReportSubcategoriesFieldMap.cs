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
      _fieldMap.AddMap("ReportSubcategoryID", "ReportSubcategoryID", true, true, true);
      _fieldMap.AddMap("ReportCategoryTableID", "ReportCategoryTableID", true, true, true);
      _fieldMap.AddMap("ReportTableID", "ReportTableID", true, true, true);
      _fieldMap.AddMap("BaseQuery", "BaseQuery", true, true, true);
            
    }
  }
  
}
