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
  [KnownType(typeof(ScheduledReportsRecurrencyItemProxy))]
  public class ScheduledReportsRecurrencyItemProxy
  {
    public ScheduledReportsRecurrencyItemProxy() {}
    [DataMember] public byte id { get; set; }
    [DataMember] public string recurrency { get; set; }
          
  }
  
  public partial class ScheduledReportsRecurrencyItem : BaseItem
  {
    public ScheduledReportsRecurrencyItemProxy GetProxy()
    {
      ScheduledReportsRecurrencyItemProxy result = new ScheduledReportsRecurrencyItemProxy();
      result.recurrency = this.recurrency;
      result.id = this.id;
       
       
       
      return result;
    }	
  }
}
