using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class KnowledgeBaseCategories
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CategoryID", "CategoryID", false, false, true);
      _fieldMap.AddMap("ParentID", "ParentID", false, false, true);
      _fieldMap.AddMap("CategoryName", "CategoryName", false, false, true);
      _fieldMap.AddMap("CategoryDesc", "CategoryDesc", false, false, true);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Position", "Position", false, false, true);
      _fieldMap.AddMap("VisibleOnPortal", "VisibleOnPortal", false, false, false);
      _fieldMap.AddMap("ProductFamilyID", "ProductFamilyID", false, false, false);
            
    }
  }
  
}
