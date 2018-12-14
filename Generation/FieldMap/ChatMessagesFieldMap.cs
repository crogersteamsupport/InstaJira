using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ChatMessages
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ChatMessageID", "ChatMessageID", false, false, false);
      _fieldMap.AddMap("ChatID", "ChatID", false, false, false);
      _fieldMap.AddMap("IsNotification", "IsNotification", false, false, false);
      _fieldMap.AddMap("Message", "Message", false, false, false);
      _fieldMap.AddMap("PosterID", "PosterID", false, false, false);
      _fieldMap.AddMap("PosterType", "PosterType", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
            
    }
  }
  
}
