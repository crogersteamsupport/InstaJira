using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomerRelationships
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomerRelationshipID", "CustomerRelationshipID", false, false, false);
      _fieldMap.AddMap("CustomerID", "CustomerID", false, false, false);
      _fieldMap.AddMap("RelatedCustomerID", "RelatedCustomerID", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
            
    }
  }
  
}
