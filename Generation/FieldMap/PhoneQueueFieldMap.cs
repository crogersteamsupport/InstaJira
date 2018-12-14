using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class PhoneQueue
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("PhoneQueueID", "PhoneQueueID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("CallSID", "CallSID", false, false, false);
      _fieldMap.AddMap("AccountSID", "AccountSID", false, false, false);
      _fieldMap.AddMap("CallTo", "CallTo", false, false, false);
      _fieldMap.AddMap("CallFrom", "CallFrom", false, false, false);
      _fieldMap.AddMap("Status", "Status", false, false, false);
      _fieldMap.AddMap("CallDateTime", "CallDateTime", false, false, false);
      _fieldMap.AddMap("LastActionDateTime", "LastActionDateTime", false, false, false);
      _fieldMap.AddMap("ActionValue", "ActionValue", false, false, false);
            
    }
  }
  
}
