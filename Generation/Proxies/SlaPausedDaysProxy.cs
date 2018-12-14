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
  [KnownType(typeof(SlaPausedDayProxy))]
  public class SlaPausedDayProxy
  {
    public SlaPausedDayProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int SlaTriggerId { get; set; }
    [DataMember] public DateTime DateToPause { get; set; }
          
  }
  
  public partial class SlaPausedDay : BaseItem
  {
    public SlaPausedDayProxy GetProxy()
    {
      SlaPausedDayProxy result = new SlaPausedDayProxy();
      result.SlaTriggerId = this.SlaTriggerId;
      result.Id = this.Id;
       
      result.DateToPause = DateTime.SpecifyKind(this.DateToPauseUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
