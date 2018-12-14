using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class EMailAlternateInbound
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("SystemEMailID", "SystemEMailID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("Description", "Description", false, false, false);
      _fieldMap.AddMap("GroupToAssign", "GroupToAssign", false, false, false);
      _fieldMap.AddMap("DefaultTicketType", "DefaultTicketType", false, false, false);
      _fieldMap.AddMap("ProductID", "ProductID", false, false, false);
            
    }
  }
  
}
