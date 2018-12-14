using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ChatUserSettings
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("UserID", "UserID", false, false, false);
      _fieldMap.AddMap("CurrentChatID", "CurrentChatID", false, false, false);
      _fieldMap.AddMap("IsAvailable", "IsAvailable", false, false, false);
      _fieldMap.AddMap("LastChatRequestID", "LastChatRequestID", false, false, false);
            
    }
  }
  
}
