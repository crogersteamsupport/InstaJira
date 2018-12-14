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
  [KnownType(typeof(UserTicketProxy))]
  public class UserTicketProxy
  {
    public UserTicketProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public bool? SentToSalesForce { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
  
  public partial class UserTicket : BaseItem
  {
    public UserTicketProxy GetProxy()
    {
      UserTicketProxy result = new UserTicketProxy();
      result.ImportFileID = this.ImportFileID;
      result.SentToSalesForce = this.SentToSalesForce;
      result.CreatorID = this.CreatorID;
      result.UserID = this.UserID;
      result.TicketID = this.TicketID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
