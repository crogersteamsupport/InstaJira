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
  [KnownType(typeof(BillingInfoItemProxy))]
  public class BillingInfoItemProxy
  {
    public BillingInfoItemProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int? CreditCardID { get; set; }
    [DataMember] public int? AddressID { get; set; }
    [DataMember] public bool IsAutomatic { get; set; }
    [DataMember] public double? UserPrice { get; set; }
    [DataMember] public double? PortalPrice { get; set; }
    [DataMember] public double? BasicPortalPrice { get; set; }
    [DataMember] public double? ChatPrice { get; set; }
    [DataMember] public double? StoragePrice { get; set; }
    [DataMember] public DateTime NextInvoiceDate { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class BillingInfoItem : BaseItem
  {
    public BillingInfoItemProxy GetProxy()
    {
      BillingInfoItemProxy result = new BillingInfoItemProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.StoragePrice = this.StoragePrice;
      result.ChatPrice = this.ChatPrice;
      result.BasicPortalPrice = this.BasicPortalPrice;
      result.PortalPrice = this.PortalPrice;
      result.UserPrice = this.UserPrice;
      result.IsAutomatic = this.IsAutomatic;
      result.AddressID = this.AddressID;
      result.CreditCardID = this.CreditCardID;
      result.OrganizationID = this.OrganizationID;
       
      result.NextInvoiceDate = DateTime.SpecifyKind(this.NextInvoiceDateUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
