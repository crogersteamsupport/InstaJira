using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class JiraInstanceProduct
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("JiraInstanceProductId", "JiraInstanceProductId", false, false, false);
      _fieldMap.AddMap("CrmLinkId", "CrmLinkId", false, false, false);
      _fieldMap.AddMap("ProductId", "ProductId", false, false, false);
            
    }
  }
  
}
