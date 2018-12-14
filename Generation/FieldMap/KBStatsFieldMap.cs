using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class KBStats
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("KBViewID", "KBViewID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("KBArticleID", "KBArticleID", false, false, false);
      _fieldMap.AddMap("ViewDateTime", "ViewDateTime", false, false, false);
      _fieldMap.AddMap("ViewIP", "ViewIP", false, false, false);
      _fieldMap.AddMap("SearchTerm", "SearchTerm", false, false, false);
            
    }
  }
  
}
