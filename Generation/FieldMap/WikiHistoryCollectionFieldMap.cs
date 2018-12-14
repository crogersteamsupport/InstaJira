using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class WikiHistoryCollection
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("HistoryID", "HistoryID", false, false, false);
      _fieldMap.AddMap("ArticleID", "ArticleID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ArticleName", "ArticleName", false, false, false);
      _fieldMap.AddMap("Body", "Body", false, false, false);
      _fieldMap.AddMap("Version", "Version", false, false, false);
      _fieldMap.AddMap("CreatedBy", "CreatedBy", false, false, false);
      _fieldMap.AddMap("CreatedDate", "CreatedDate", false, false, false);
      _fieldMap.AddMap("ModifiedBy", "ModifiedBy", false, false, false);
      _fieldMap.AddMap("ModifiedDate", "ModifiedDate", false, false, false);
      _fieldMap.AddMap("Comment", "Comment", false, false, false);
            
    }
  }
  
}
