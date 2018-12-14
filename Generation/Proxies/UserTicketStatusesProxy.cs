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
  
  public partial class UserTicketStatus : BaseItem
  {
    public UserTicketStatusProxy GetProxy()
    {
      UserTicketStatusProxy result = new UserTicketStatusProxy();
      result.IsFlagged = this.IsFlagged;
      result.UserID = this.UserID;
      result.TicketID = this.TicketID;
      result.UserTicketStatusID = this.UserTicketStatusID;
       
      result.DateRead = DateTime.SpecifyKind(this.DateReadUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
