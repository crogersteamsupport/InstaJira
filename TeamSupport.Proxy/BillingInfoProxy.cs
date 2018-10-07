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
}
