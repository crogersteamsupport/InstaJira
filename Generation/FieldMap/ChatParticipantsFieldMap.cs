using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ChatParticipants
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ChatParticipantID", "ChatParticipantID", false, false, false);
      _fieldMap.AddMap("ChatID", "ChatID", false, false, false);
      _fieldMap.AddMap("ParticipantID", "ParticipantID", false, false, false);
      _fieldMap.AddMap("ParticipantType", "ParticipantType", false, false, false);
      _fieldMap.AddMap("IPAddress", "IPAddress", false, false, false);
      _fieldMap.AddMap("LastMessageID", "LastMessageID", false, false, false);
      _fieldMap.AddMap("LastPreviewedMessageID", "LastPreviewedMessageID", false, false, false);
      _fieldMap.AddMap("LastTyped", "LastTyped", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
      _fieldMap.AddMap("DateJoined", "DateJoined", false, false, false);
      _fieldMap.AddMap("DateLeft", "DateLeft", false, false, false);
            
    }
  }
  
}
