using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class EmailTemplateTables
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("EmailTemplateTableID", "EmailTemplateTableID", false, false, false);
      _fieldMap.AddMap("EmailTemplateID", "EmailTemplateID", false, false, false);
      _fieldMap.AddMap("ReportTableID", "ReportTableID", false, false, false);
      _fieldMap.AddMap("Alias", "Alias", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
            
    }
  }
  
}
