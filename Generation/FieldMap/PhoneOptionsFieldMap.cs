using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class PhoneOptions
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("PhoneOptionID", "PhoneOptionID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("PhoneActive", "PhoneActive", false, false, false);
      _fieldMap.AddMap("AccountSID", "AccountSID", false, false, false);
      _fieldMap.AddMap("AccountToken", "AccountToken", false, false, false);
      _fieldMap.AddMap("WelcomeAudioURL", "WelcomeAudioURL", false, false, false);
            
    }
  }
  
}
