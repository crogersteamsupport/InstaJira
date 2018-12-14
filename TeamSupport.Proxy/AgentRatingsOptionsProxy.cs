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
  [KnownType(typeof(AgentRatingsOptionProxy))]
  public class AgentRatingsOptionProxy
  {
    public AgentRatingsOptionProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string PositiveRatingText { get; set; }
    [DataMember] public string NeutralRatingText { get; set; }
    [DataMember] public string NegativeRatingText { get; set; }
    [DataMember] public string PositiveImage { get; set; }
    [DataMember] public string NeutralImage { get; set; }
    [DataMember] public string NegativeImage { get; set; }
    [DataMember] public string RedirectURL { get; set; }
    [DataMember] public string ExternalPageLink { get; set; }
          
  }
}
