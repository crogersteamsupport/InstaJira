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
  [KnownType(typeof(ForumTicketProxy))]
  public class ForumTicketProxy
  {
    public ForumTicketProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int? ForumCategory { get; set; }
    [DataMember] public int? ViewCount { get; set; }
          
  }
  
  public partial class ForumTicket : BaseItem
  {
    public ForumTicketProxy GetProxy()
    {
      ForumTicketProxy result = new ForumTicketProxy();
      result.ViewCount = this.ViewCount;
      result.ForumCategory = this.ForumCategory;
      result.TicketID = this.TicketID;
       
       
       
      return result;
    }	
  }
}
