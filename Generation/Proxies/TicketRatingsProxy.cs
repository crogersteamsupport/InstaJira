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
  [KnownType(typeof(TicketRatingProxy))]
  public class TicketRatingProxy
  {
    public TicketRatingProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int? TicketType { get; set; }
    [DataMember] public int? Votes { get; set; }
    [DataMember] public float? Rating { get; set; }
    [DataMember] public int? Views { get; set; }
    [DataMember] public int? ThumbsUp { get; set; }
    [DataMember] public int? ThumbsDown { get; set; }
    [DataMember] public DateTime? LastViewed { get; set; }
          
  }
  
  public partial class TicketRating : BaseItem
  {
    public TicketRatingProxy GetProxy()
    {
      TicketRatingProxy result = new TicketRatingProxy();
      result.ThumbsDown = this.ThumbsDown;
      result.ThumbsUp = this.ThumbsUp;
      result.Views = this.Views;
      result.Rating = this.Rating;
      result.Votes = this.Votes;
      result.TicketType = this.TicketType;
      result.TicketID = this.TicketID;
       
       
      result.LastViewed = this.LastViewedUtc == null ? this.LastViewedUtc : DateTime.SpecifyKind((DateTime)this.LastViewedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
