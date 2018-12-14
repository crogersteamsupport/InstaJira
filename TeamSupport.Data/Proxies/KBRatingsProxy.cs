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
  [KnownType(typeof(KBRatingProxy))]
  public class KBRatingProxy
  {
    public KBRatingProxy() {}
    [DataMember] public int KBRatingID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public string IP { get; set; }
    [DataMember] public bool? Rating { get; set; }
    [DataMember] public DateTime? DateUpdated { get; set; }
    [DataMember] public string Comment { get; set; }
          
  }
  
  public partial class KBRating : BaseItem
  {
    public KBRatingProxy GetProxy()
    {
      KBRatingProxy result = new KBRatingProxy();
      result.Comment = this.Comment;
      result.Rating = this.Rating;
      result.IP = this.IP;
      result.UserID = this.UserID;
      result.TicketID = this.TicketID;
      result.KBRatingID = this.KBRatingID;
       
       
      result.DateUpdated = this.DateUpdatedUtc == null ? this.DateUpdatedUtc : DateTime.SpecifyKind((DateTime)this.DateUpdatedUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
