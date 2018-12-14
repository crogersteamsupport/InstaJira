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
}
