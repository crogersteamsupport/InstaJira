using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class Invoices
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("InvoiceID", "InvoiceID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("CreditCardID", "CreditCardID", false, false, false);
      _fieldMap.AddMap("UserSeats", "UserSeats", false, false, false);
      _fieldMap.AddMap("PortalSeats", "PortalSeats", false, false, false);
      _fieldMap.AddMap("ExtraStorageUnits", "ExtraStorageUnits", false, false, false);
      _fieldMap.AddMap("UserPrice", "UserPrice", false, false, false);
      _fieldMap.AddMap("PortalPrice", "PortalPrice", false, false, false);
      _fieldMap.AddMap("StoragePrice", "StoragePrice", false, false, false);
      _fieldMap.AddMap("IsPortalBilled", "IsPortalBilled", false, false, false);
      _fieldMap.AddMap("TaxRate", "TaxRate", false, false, false);
      _fieldMap.AddMap("TotalUserPrice", "TotalUserPrice", false, false, false);
      _fieldMap.AddMap("TotalPortalPrice", "TotalPortalPrice", false, false, false);
      _fieldMap.AddMap("TotalStoragePrice", "TotalStoragePrice", false, false, false);
      _fieldMap.AddMap("TotalTaxPrice", "TotalTaxPrice", false, false, false);
      _fieldMap.AddMap("TotalAmount", "TotalAmount", false, false, false);
      _fieldMap.AddMap("DateStart", "DateStart", false, false, false);
      _fieldMap.AddMap("DateEnd", "DateEnd", false, false, false);
      _fieldMap.AddMap("DateBilled", "DateBilled", false, false, false);
      _fieldMap.AddMap("DateDue", "DateDue", false, false, false);
      _fieldMap.AddMap("IsPaid", "IsPaid", false, false, false);
      _fieldMap.AddMap("IsPaymentFailed", "IsPaymentFailed", false, false, false);
      _fieldMap.AddMap("PaymentMethod", "PaymentMethod", false, false, false);
      _fieldMap.AddMap("PaymentFailedReason", "PaymentFailedReason", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("CreatorID", "CreatorID", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
            
    }
  }
  
}
