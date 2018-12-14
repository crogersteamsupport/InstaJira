using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class CustomerHubAuthentication
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("CustomerHubAuthenticationID", "CustomerHubAuthenticationID", false, false, false);
      _fieldMap.AddMap("CustomerHubID", "CustomerHubID", false, false, false);
      _fieldMap.AddMap("EnableSelfRegister", "EnableSelfRegister", false, false, false);
      _fieldMap.AddMap("EnableRequestAccess", "EnableRequestAccess", false, false, false);
      _fieldMap.AddMap("EnableSSO", "EnableSSO", false, false, false);
      _fieldMap.AddMap("RequestTicketType", "RequestTicketType", false, false, false);
      _fieldMap.AddMap("RequestGroupType", "RequestGroupType", false, false, false);
      _fieldMap.AddMap("AnonymousHubAccess", "AnonymousHubAccess", false, false, false);
      _fieldMap.AddMap("AnonymousWikiAccess", "AnonymousWikiAccess", false, false, false);
      _fieldMap.AddMap("AnonymousKBAccess", "AnonymousKBAccess", false, false, false);
      _fieldMap.AddMap("AnonymousProductAccess", "AnonymousProductAccess", false, false, false);
      _fieldMap.AddMap("AnonymousTicketAccess", "AnonymousTicketAccess", false, false, false);
      _fieldMap.AddMap("HonorServiceAgreementExpirationDate", "HonorServiceAgreementExpirationDate", false, false, false);
      _fieldMap.AddMap("HonorSupportExpiration", "HonorSupportExpiration", false, false, false);
      _fieldMap.AddMap("RequireTermsAndConditions", "RequireTermsAndConditions", false, false, false);
      _fieldMap.AddMap("DateModified", "DateModified", false, false, false);
      _fieldMap.AddMap("ModifierID", "ModifierID", false, false, false);
      _fieldMap.AddMap("AnonymousChatAccess", "AnonymousChatAccess", false, false, false);
            
    }
  }
  
}
