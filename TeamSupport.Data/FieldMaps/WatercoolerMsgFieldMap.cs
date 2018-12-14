using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class WatercoolerMsg
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("MessageID", "MessageID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("TimeStamp", "TimeStamp", false, false, false);
      _fieldMap.AddMap("Message", "Message", false, false, false);
      _fieldMap.AddMap("MessageParent", "MessageParent", false, false, false);
      _fieldMap.AddMap("IsDeleted", "IsDeleted", false, false, false);
      _fieldMap.AddMap("LastModified", "LastModified", false, false, false);
      _fieldMap.AddMap("NeedsIndexing", "NeedsIndexing", false, false, false);
            
    }
  }
  
}
