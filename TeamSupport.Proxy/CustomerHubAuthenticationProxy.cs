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
  [KnownType(typeof(CustomerHubAuthenticationItemProxy))]
  public class CustomerHubAuthenticationItemProxy
  {
    public CustomerHubAuthenticationItemProxy() {}
    [DataMember] public int CustomerHubAuthenticationID { get; set; }
    [DataMember] public int CustomerHubID { get; set; }
    [DataMember] public bool EnableSelfRegister { get; set; }
    [DataMember] public bool EnableRequestAccess { get; set; }
    [DataMember] public bool EnableSSO { get; set; }
    [DataMember] public int RequestTicketType { get; set; }
    [DataMember] public int RequestGroupType { get; set; }
    [DataMember] public bool AnonymousHubAccess { get; set; }
    [DataMember] public bool AnonymousWikiAccess { get; set; }
    [DataMember] public bool AnonymousKBAccess { get; set; }
    [DataMember] public bool AnonymousProductAccess { get; set; }
    [DataMember] public bool AnonymousTicketAccess { get; set; }
    [DataMember] public bool HonorServiceAgreementExpirationDate { get; set; }
    [DataMember] public bool HonorSupportExpiration { get; set; }
    [DataMember] public bool RequireTermsAndConditions { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int? ModifierID { get; set; }
    [DataMember] public bool AnonymousChatAccess { get; set; }
          
  }
}
