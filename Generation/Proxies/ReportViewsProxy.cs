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
  [KnownType(typeof(ReportViewProxy))]
  public class ReportViewProxy
  {
    public ReportViewProxy() {}
    [DataMember] public int ReportViewID { get; set; }
    [DataMember] public int ReportID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public DateTime DateViewed { get; set; }
    [DataMember] public double DurationToLoad { get; set; }
    [DataMember] public string SQLExecuted { get; set; }
    [DataMember] public string ErrorMessage { get; set; }
          
  }
  
  public partial class ReportView : BaseItem
  {
    public ReportViewProxy GetProxy()
    {
      ReportViewProxy result = new ReportViewProxy();
      result.ErrorMessage = this.ErrorMessage;
      result.SQLExecuted = this.SQLExecuted;
      result.DurationToLoad = this.DurationToLoad;
      result.UserID = this.UserID;
      result.ReportID = this.ReportID;
      result.ReportViewID = this.ReportViewID;
       
      result.DateViewed = DateTime.SpecifyKind(this.DateViewedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
