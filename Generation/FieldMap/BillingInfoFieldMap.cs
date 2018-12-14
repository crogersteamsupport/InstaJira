using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class BillingInfo
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("CreditCardID", "CreditCardID", false, false, false);
      _fieldMap.AddMap("AddressID", "AddressID", false, false, false);
      _fieldMap.AddMap("IsAutomatic", "IsAutomatic", false, false, false);
      _fieldMap.AddMap("UserPrice", "UserPrice", false, false, false);
      _fieldMap.AddMap("PortalPrice", "PortalPrice", false, false, false);
      _fieldMap.AddMap("BasicPortalPrice", "BasicPortalPrice", false, false, false);
      _fieldMap.AddMap("ChatPrice", "ChatPrice", false, false, false);
      _fieldMap.AddMap("StoragePrice", "StoragePrice", false, false, false);
      _fieldMap.AddMap("NextInvoiceDate", "NextInvoiceDate", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
