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
  [KnownType(typeof(ChatMessageProxy))]
  public class ChatMessageProxy
  {
    public ChatMessageProxy() {}
    [DataMember] public int ChatMessageID { get; set; }
    [DataMember] public int ChatID { get; set; }
    [DataMember] public bool IsNotification { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public int PosterID { get; set; }
    [DataMember] public ChatParticipantType PosterType { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
}
