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
  [KnownType(typeof(ChatUserSettingProxy))]
  public class ChatUserSettingProxy
  {
    public ChatUserSettingProxy() {}
    [DataMember] public int UserID { get; set; }
    [DataMember] public int CurrentChatID { get; set; }
    [DataMember] public bool IsAvailable { get; set; }
    [DataMember] public int LastChatRequestID { get; set; }
          
  }
}
