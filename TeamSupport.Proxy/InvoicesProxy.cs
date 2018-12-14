using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  [DataContract(Namespace="http://teamsupport.com/")]
  [KnownType(typeof(InvoiceProxy))]
  public class InvoiceProxy
  {
    public InvoiceProxy() {}
    [DataMember] public int InvoiceID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int? CreditCardID { get; set; }
    [DataMember] public decimal UserSeats { get; set; }
    [DataMember] public decimal PortalSeats { get; set; }
    [DataMember] public decimal ExtraStorageUnits { get; set; }
    [DataMember] public decimal UserPrice { get; set; }
    [DataMember] public decimal PortalPrice { get; set; }
    [DataMember] public decimal StoragePrice { get; set; }
    [DataMember] public bool IsPortalBilled { get; set; }
    [DataMember] public decimal TaxRate { get; set; }
    [DataMember] public decimal TotalUserPrice { get; set; }
    [DataMember] public decimal TotalPortalPrice { get; set; }
    [DataMember] public decimal TotalStoragePrice { get; set; }
    [DataMember] public decimal TotalTaxPrice { get; set; }
    [DataMember] public decimal TotalAmount { get; set; }
    [DataMember] public DateTime DateStart { get; set; }
    [DataMember] public DateTime DateEnd { get; set; }
    [DataMember] public DateTime DateBilled { get; set; }
    [DataMember] public DateTime DateDue { get; set; }
    [DataMember] public bool IsPaid { get; set; }
    [DataMember] public bool IsPaymentFailed { get; set; }
    [DataMember] public string PaymentMethod { get; set; }
    [DataMember] public string PaymentFailedReason { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
}
