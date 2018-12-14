using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class WaterCooler
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("MessageID", "MessageID", false, false, false);
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("TimeStamp", "TimeStamp", false, false, false);
      _fieldMap.AddMap("GroupFor", "GroupFor", false, false, false);
      _fieldMap.AddMap("ReplyTo", "ReplyTo", false, false, false);
      _fieldMap.AddMap("Message", "Message", false, false, false);
      _fieldMap.AddMap("MessageType", "MessageType", false, false, false);
            
    }
  }
  
}
