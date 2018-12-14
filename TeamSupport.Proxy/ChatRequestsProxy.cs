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
  [KnownType(typeof(ChatRequestProxy))]
  public class ChatRequestProxy
  {
    public ChatRequestProxy() {}
    [DataMember] public int ChatRequestID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int ChatID { get; set; }
    [DataMember] public int RequestorID { get; set; }
    [DataMember] public ChatParticipantType RequestorType { get; set; }
    [DataMember] public int? TargetUserID { get; set; }
    [DataMember] public string Message { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public ChatRequestType RequestType { get; set; }
    [DataMember] public bool IsAccepted { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
}
