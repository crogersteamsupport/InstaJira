using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  
  public partial class ChatRequests
  {
    protected override void BuildFieldMap()
    {
      _fieldMap = new FieldMap();
      _fieldMap.AddMap("ChatRequestID", "ChatRequestID", false, false, false);
      _fieldMap.AddMap("OrganizationID", "OrganizationID", false, false, false);
      _fieldMap.AddMap("ChatID", "ChatID", false, false, false);
      _fieldMap.AddMap("RequestorID", "RequestorID", false, false, false);
      _fieldMap.AddMap("RequestorType", "RequestorType", false, false, false);
      _fieldMap.AddMap("TargetUserID", "TargetUserID", false, false, false);
      _fieldMap.AddMap("Message", "Message", false, false, false);
      _fieldMap.AddMap("GroupID", "GroupID", false, false, false);
      _fieldMap.AddMap("RequestType", "RequestType", false, false, false);
      _fieldMap.AddMap("IsAccepted", "IsAccepted", false, false, false);
      _fieldMap.AddMap("DateCreated", "DateCreated", false, false, false);
            
    }
  }
  
}
