using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
  public partial class CDI_Setting : BaseItem
  {
    public CDI_SettingProxy GetProxy()
    {
      CDI_SettingProxy result = new CDI_SettingProxy();
      result.NeedCompute = this.NeedCompute;
      result.YellowUpperRange = this.YellowUpperRange;
      result.GreenUpperRange = this.GreenUpperRange;
      result.AvgDaysToCloseWeight = this.AvgDaysToCloseWeight;
      result.AvgDaysOpenWeight = this.AvgDaysOpenWeight;
      result.Last30Weight = this.Last30Weight;
      result.OpenTicketsWeight = this.OpenTicketsWeight;
      result.TotalTicketsWeight = this.TotalTicketsWeight;
      result.OrganizationID = this.OrganizationID;

      result.LastCompute = this.LastComputeUtc == null ? this.LastComputeUtc : DateTime.SpecifyKind((DateTime)this.LastComputeUtc, DateTimeKind.Utc); 
      result.AverageActionCountWeight = this.AverageActionCountWeight;       
      result.AverageSentimentScoreWeight = this.AverageSentimentScoreWeight;
      result.AverageSeverityWeight = this.AverageSeverityWeight;       
      result.AgentRatingsWeight = this.AgentRatingsWeight;       
      return result;
    }	
  }
}
