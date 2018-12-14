using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomerHubCustomViews
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomerHubCustomViewID", "CustomerHubCustomViewID", false, false, false);
      _fieldMap.AddMap("CustomerHubID", "CustomerHubID", false, false, false);
      _fieldMap.AddMap("CustomerHubViewID", "CustomerHubViewID", false, false, false);
      _fieldMap.AddMap("CustomView", "CustomView", false, false, false);
      _fieldMap.AddMap("IsActive", "IsActive", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
