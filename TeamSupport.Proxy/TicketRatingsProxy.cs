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
}
