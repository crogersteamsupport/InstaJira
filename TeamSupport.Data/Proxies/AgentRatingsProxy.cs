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
  [KnownType(typeof(AgentRatingProxy))]
  public class AgentRatingProxy
  {
    public AgentRatingProxy() {}
    [DataMember] public int AgentRatingID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int CompanyID { get; set; }
    [DataMember] public int ContactID { get; set; }
    [DataMember] public int Rating { get; set; }
    [DataMember] public string RatingText { get; set; }
    [DataMember] public string Comment { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int TicketNumber { get; set; }
          
  }
  
  public partial class AgentRating : BaseItem
  {
    public AgentRatingProxy GetProxy()
    {
      AgentRatingProxy result = new AgentRatingProxy();
      result.TicketID = this.TicketID;
      result.Comment = this.Comment;
      result.Rating = this.Rating;
      result.ContactID = this.ContactID;
      result.CompanyID = this.CompanyID;
      result.OrganizationID = this.OrganizationID;
      result.AgentRatingID = this.AgentRatingID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);

      Tickets t = new Tickets(BaseCollection.LoginUser);
      t.LoadByTicketID(this.TicketID);

      result.TicketNumber = t[0].TicketNumber;

      AgentRatingsOptions options = new AgentRatingsOptions(BaseCollection.LoginUser);
      options.LoadByOrganizationID(this.OrganizationID);

      switch (this.Rating)
      {
          case -1:
              if (!options.IsEmpty)
              {
                  if (options[0].NegativeImage == null)
                      result.RatingText = "<img class='imgsm' src='../Images/face-negative.png' alt='negative' />";
                  else
                      result.RatingText = "<img style='width:20px;height:20px' src='" + options[0].NegativeImage + "' />";
              }
              else
                  result.RatingText = "<img class='imgsm' src='../Images/face-negative.png' alt='negative' />";

              break;
          case 0:
              if (!options.IsEmpty)
              {
                  if (options[0].NeutralImage == null)
                      result.RatingText = "<img class='imgsm' src='../Images/face-neutral.png' alt='neutral' />";
                  else
                      result.RatingText = "<img style='width:20px;height:20px' src='" + options[0].NeutralImage + "' />";
              }
              else
                  result.RatingText = "<img class='imgsm' src='../Images/face-neutral.png' alt='neutral' />";

              break;
          case 1:
              if (!options.IsEmpty)
              {
                if (options[0].PositiveImage == null)
                    result.RatingText = "<img class='imgsm' src='../Images/face-positive.png' alt='positive' />";
                else
                    result.RatingText = "<img style='width:20px;height:20px' src='" + options[0].PositiveImage + "' />";
              }
              else
                  result.RatingText = "<img class='imgsm' src='../Images/face-positive.png' alt='positive' />";
              break;
      }
      return result;
    }	
  }
}
