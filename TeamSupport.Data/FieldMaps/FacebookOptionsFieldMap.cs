using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class FacebookOptions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("DisplayArticles", "DisplayArticles", false, false, false);
      _fieldMap.AddMap("DisplayKB", "DisplayKB", false, false, false);
            
    }
  }
  
}
