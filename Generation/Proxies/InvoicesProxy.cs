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
  
  public partial class Invoice : BaseItem
  {
    public InvoiceProxy GetProxy()
    {
      InvoiceProxy result = new InvoiceProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.PaymentFailedReason = this.PaymentFailedReason;
      result.PaymentMethod = this.PaymentMethod;
      result.IsPaymentFailed = this.IsPaymentFailed;
      result.IsPaid = this.IsPaid;
      result.TotalAmount = this.TotalAmount;
      result.TotalTaxPrice = this.TotalTaxPrice;
      result.TotalStoragePrice = this.TotalStoragePrice;
      result.TotalPortalPrice = this.TotalPortalPrice;
      result.TotalUserPrice = this.TotalUserPrice;
      result.TaxRate = this.TaxRate;
      result.IsPortalBilled = this.IsPortalBilled;
      result.StoragePrice = this.StoragePrice;
      result.PortalPrice = this.PortalPrice;
      result.UserPrice = this.UserPrice;
      result.ExtraStorageUnits = this.ExtraStorageUnits;
      result.PortalSeats = this.PortalSeats;
      result.UserSeats = this.UserSeats;
      result.CreditCardID = this.CreditCardID;
      result.OrganizationID = this.OrganizationID;
      result.InvoiceID = this.InvoiceID;
       
      result.DateStart = DateTime.SpecifyKind(this.DateStartUtc, DateTimeKind.Utc);
      result.DateEnd = DateTime.SpecifyKind(this.DateEndUtc, DateTimeKind.Utc);
      result.DateBilled = DateTime.SpecifyKind(this.DateBilledUtc, DateTimeKind.Utc);
      result.DateDue = DateTime.SpecifyKind(this.DateDueUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
