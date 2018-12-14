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
  [KnownType(typeof(ChatClientProxy))]
  public class ChatClientProxy
  {
    public ChatClientProxy() {}
    [DataMember] public int ChatClientID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string FirstName { get; set; }
    [DataMember] public string LastName { get; set; }
    [DataMember] public string Email { get; set; }
    [DataMember] public string CompanyName { get; set; }
    [DataMember] public DateTime LastPing { get; set; }
    [DataMember] public int? LinkedUserID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
}
