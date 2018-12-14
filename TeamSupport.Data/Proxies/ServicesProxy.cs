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
  [KnownType(typeof(ServiceProxy))]
  public class ServiceProxy
  {
    public ServiceProxy() {}
    [DataMember] public int ServiceID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public bool Enabled { get; set; }
    [DataMember] public int Interval { get; set; }
    [DataMember] public DateTime? LastStartTime { get; set; }
    [DataMember] public DateTime? LastEndTime { get; set; }
    [DataMember] public string LastResult { get; set; }
    [DataMember] public string LastError { get; set; }
    [DataMember] public int ErrorCount { get; set; }
    [DataMember] public int RunCount { get; set; }
    [DataMember] public int RunTimeAvg { get; set; }
    [DataMember] public int RunTimeMax { get; set; }
    [DataMember] public string AssemblyName { get; set; }
    [DataMember] public bool AutoStart { get; set; }
    [DataMember] public DateTime? HealthTime { get; set; }
    [DataMember] public string NameSpace { get; set; }
    [DataMember] public int HealthMaxMinutes { get; set; }
          
  }
  
  public partial class Service : BaseItem
  {
    public ServiceProxy GetProxy()
    {
      ServiceProxy result = new ServiceProxy();
      result.HealthMaxMinutes = this.HealthMaxMinutes;
      result.NameSpace = this.NameSpace;
      result.AutoStart = this.AutoStart;
      result.AssemblyName = this.AssemblyName;
      result.RunTimeMax = this.RunTimeMax;
      result.RunTimeAvg = this.RunTimeAvg;
      result.RunCount = this.RunCount;
      result.ErrorCount = this.ErrorCount;
      result.LastError = this.LastError;
      result.LastResult = this.LastResult;
      result.Interval = this.Interval;
      result.Enabled = this.Enabled;
      result.Name = this.Name;
      result.ServiceID = this.ServiceID;
       
       
      result.HealthTime = this.HealthTimeUtc == null ? this.HealthTimeUtc : DateTime.SpecifyKind((DateTime)this.HealthTimeUtc, DateTimeKind.Utc); 
      result.LastEndTime = this.LastEndTimeUtc == null ? this.LastEndTimeUtc : DateTime.SpecifyKind((DateTime)this.LastEndTimeUtc, DateTimeKind.Utc); 
      result.LastStartTime = this.LastStartTimeUtc == null ? this.LastStartTimeUtc : DateTime.SpecifyKind((DateTime)this.LastStartTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
