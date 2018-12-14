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
  [KnownType(typeof(TicketQueueItemProxy))]
  public class TicketQueueItemProxy
  {
    public TicketQueueItemProxy() {}
    [DataMember] public int TicketQueueID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public decimal? EstimatedDays { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
  
  public partial class TicketQueueItem : BaseItem
  {
    public TicketQueueItemProxy GetProxy()
    {
      TicketQueueItemProxy result = new TicketQueueItemProxy();
      result.ModifierID = this.ModifierID;
      result.CreatorID = this.CreatorID;
      result.Position = this.Position;
      result.EstimatedDays = this.EstimatedDays;
      result.UserID = this.UserID;
      result.TicketID = this.TicketID;
      result.TicketQueueID = this.TicketQueueID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
