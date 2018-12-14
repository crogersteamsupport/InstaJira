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
  [KnownType(typeof(WebHooksPendingItemProxy))]
  public class WebHooksPendingItemProxy
  {
    public WebHooksPendingItemProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int OrganizationId { get; set; }
    [DataMember] public int RefType { get; set; }
    [DataMember] public int? RefId { get; set; }
    [DataMember] public short Type { get; set; }
    [DataMember] public string Url { get; set; }
    [DataMember] public string BodyData { get; set; }
    [DataMember] public string Token { get; set; }
    [DataMember] public bool Inbound { get; set; }
    [DataMember] public bool? IsProcessing { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
          
  }
  
  public partial class WebHooksPendingItem : BaseItem
  {
    public WebHooksPendingItemProxy GetProxy()
    {
      WebHooksPendingItemProxy result = new WebHooksPendingItemProxy();
      result.IsProcessing = this.IsProcessing;
      result.Inbound = this.Inbound;
      result.Token = this.Token;
      result.BodyData = this.BodyData;
      result.Url = this.Url;
      result.Type = this.Type;
      result.RefId = this.RefId;
      result.RefType = this.RefType;
      result.OrganizationId = this.OrganizationId;
      result.Id = this.Id;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
