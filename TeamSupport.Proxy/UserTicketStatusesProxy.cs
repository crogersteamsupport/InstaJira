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
  [KnownType(typeof(UserTicketStatusProxy))]
  public class UserTicketStatusProxy
  {
    public UserTicketStatusProxy() {}
    [DataMember] public int UserTicketStatusID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public bool IsFlagged { get; set; }
    [DataMember] public DateTime DateRead { get; set; }
          
  }
}
