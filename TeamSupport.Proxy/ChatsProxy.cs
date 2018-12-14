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
  [KnownType(typeof(ChatProxy))]
  public class ChatProxy
  {
    public ChatProxy() {}
    [DataMember] public int ChatID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int InitiatorID { get; set; }
    [DataMember] public ChatParticipantType InitiatorType { get; set; }
    [DataMember] public int? ActionID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
}
