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
}
