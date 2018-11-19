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
  [KnownType(typeof(CreditCardProxy))]
  public class CreditCardProxy
  {
    public CreditCardProxy() {}
    [DataMember] public int CreditCardID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string DisplayNumber { get; set; }
    [DataMember] public CreditCardType CreditCardType { get; set; }
    [DataMember] public Byte[] CardNumber { get; set; }
    [DataMember] public string SecurityCode { get; set; }
    [DataMember] public DateTime ExpirationDate { get; set; }
    [DataMember] public string NameOnCard { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModfied { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
}