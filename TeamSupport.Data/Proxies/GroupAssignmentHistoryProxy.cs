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
  
  public partial class GroupAssignmentHistoryItem : BaseItem
  {
    public GroupAssignmentHistoryItemProxy GetProxy()
    {
      GroupAssignmentHistoryItemProxy result = new GroupAssignmentHistoryItemProxy();
      result.GroupID = this.GroupID;
      result.TicketID = this.TicketID;
      result.GroupAssignmentHistoryID = this.GroupAssignmentHistoryID;
       
      result.DateAssigned = DateTime.SpecifyKind(this.DateAssignedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
