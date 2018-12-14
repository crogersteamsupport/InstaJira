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
  [KnownType(typeof(SlaTicketProxy))]
  public class SlaTicketProxy
  {
    public SlaTicketProxy() {}
    [DataMember] public int TicketId { get; set; }
    [DataMember] public int SlaTriggerId { get; set; }
    [DataMember] public bool IsPending { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
          
  }
  
  public partial class SlaTicket : BaseItem
  {
    public SlaTicketProxy GetProxy()
    {
      SlaTicketProxy result = new SlaTicketProxy();
      result.IsPending = this.IsPending;
      result.SlaTriggerId = this.SlaTriggerId;
      result.TicketId = this.TicketId;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
      result.DateModified = DateTime.SpecifyKind(this.DateModifiedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
