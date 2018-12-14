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
  [KnownType(typeof(PhoneQueueItemProxy))]
  public class PhoneQueueItemProxy
  {
    public PhoneQueueItemProxy() {}
    [DataMember] public int PhoneQueueID { get; set; }
    [DataMember] public string OrganizationID { get; set; }
    [DataMember] public string CallSID { get; set; }
    [DataMember] public string AccountSID { get; set; }
    [DataMember] public string CallTo { get; set; }
    [DataMember] public string CallFrom { get; set; }
    [DataMember] public string Status { get; set; }
    [DataMember] public DateTime? CallDateTime { get; set; }
    [DataMember] public DateTime? LastActionDateTime { get; set; }
    [DataMember] public string ActionValue { get; set; }
          
  }
  
  public partial class PhoneQueueItem : BaseItem
  {
    public PhoneQueueItemProxy GetProxy()
    {
      PhoneQueueItemProxy result = new PhoneQueueItemProxy();
      result.ActionValue = this.ActionValue;
      result.Status = this.Status;
      result.CallFrom = this.CallFrom;
      result.CallTo = this.CallTo;
      result.AccountSID = this.AccountSID;
      result.CallSID = this.CallSID;
      result.OrganizationID = this.OrganizationID;
      result.PhoneQueueID = this.PhoneQueueID;
       
       
      result.LastActionDateTime = this.LastActionDateTimeUtc == null ? this.LastActionDateTimeUtc : DateTime.SpecifyKind((DateTime)this.LastActionDateTimeUtc, DateTimeKind.Utc); 
      result.CallDateTime = this.CallDateTimeUtc == null ? this.CallDateTimeUtc : DateTime.SpecifyKind((DateTime)this.CallDateTimeUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
