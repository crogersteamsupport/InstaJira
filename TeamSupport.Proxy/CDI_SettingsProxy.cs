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
  [KnownType(typeof(CDI_SettingProxy))]
  public class CDI_SettingProxy
  {
    public CDI_SettingProxy() {}
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public float? TotalTicketsWeight { get; set; }
    [DataMember] public float? OpenTicketsWeight { get; set; }
    [DataMember] public float? Last30Weight { get; set; }
    [DataMember] public float? AvgDaysOpenWeight { get; set; }
    [DataMember] public float? AvgDaysToCloseWeight { get; set; }
    [DataMember] public int? GreenUpperRange { get; set; }
    [DataMember] public int? YellowUpperRange { get; set; }
    [DataMember] public DateTime? LastCompute { get; set; }
    [DataMember] public bool? NeedCompute { get; set; }
    [DataMember] public float? AverageActionCountWeight { get; set; }  
    [DataMember] public float? AverageSentimentScoreWeight { get; set; }  
    [DataMember] public float? AverageSeverityWeight { get; set; }  
    [DataMember] public float? AgentRatingsWeight { get; set; }  

    [DataMember] public DateTime? CDIDate { get; set; }
  }
}
