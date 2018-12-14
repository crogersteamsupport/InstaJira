using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ArticleStats
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ArticleViewID", "ArticleViewID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ArticleID", "ArticleID", false, false, false);
      _fieldMap.AddMap("ViewDateTime", "ViewDateTime", false, false, false);
      _fieldMap.AddMap("ViewIP", "ViewIP", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
            
    }
  }
  
}
