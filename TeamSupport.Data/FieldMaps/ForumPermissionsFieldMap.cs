using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ForumPermissions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CategoryID", "CategoryID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("FilterUserID", "FilterUserID", false, false, false);
      _fieldMap.AddMap("FilterOrgID", "FilterOrgID", false, false, false);
            
    }
  }
  
}
