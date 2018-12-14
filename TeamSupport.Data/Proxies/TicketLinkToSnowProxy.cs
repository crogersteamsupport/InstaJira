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
  [KnownType(typeof(TicketLinkToSnowItemProxy))]
  public class TicketLinkToSnowItemProxy
  {
    public TicketLinkToSnowItemProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public DateTime? DateModifiedBySync { get; set; }
    [DataMember] public bool Sync { get; set; }
    [DataMember] public string AppId { get; set; }
    [DataMember] public string Number { get; set; }
    [DataMember] public string URL { get; set; }
    [DataMember] public string State { get; set; }
    [DataMember] public int? CreatorID { get; set; }
    [DataMember] public int? CrmLinkID { get; set; }
          
  }
  
  public partial class TicketLinkToSnowItem : BaseItem
  {
    public TicketLinkToSnowItemProxy GetProxy()
    {
      TicketLinkToSnowItemProxy result = new TicketLinkToSnowItemProxy();
      result.CrmLinkID = this.CrmLinkID;
      result.CreatorID = this.CreatorID;
      result.State = this.State;
      result.URL = this.URL;
      result.Number = this.Number;
      result.AppId = this.AppId;
      result.Sync = this.Sync;
      result.TicketID = this.TicketID;
      result.Id = this.Id;
       
       
      result.DateModifiedBySync = this.DateModifiedBySyncUtc == null ? this.DateModifiedBySyncUtc : DateTime.SpecifyKind((DateTime)this.DateModifiedBySyncUtc, DateTimeKind.Utc); 
       
      return result;
    }	
  }
}
