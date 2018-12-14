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
  [KnownType(typeof(GroupAssignmentHistoryItemProxy))]
  public class GroupAssignmentHistoryItemProxy
  {
    public GroupAssignmentHistoryItemProxy() {}
    [DataMember] public int GroupAssignmentHistoryID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public DateTime DateAssigned { get; set; }
          
  }
}
