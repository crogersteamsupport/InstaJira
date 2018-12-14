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
  [KnownType(typeof(WebHooksTokenItemProxy))]
  public class WebHooksTokenItemProxy
  {
    public WebHooksTokenItemProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int OrganizationId { get; set; }
    [DataMember] public string Token { get; set; }
    [DataMember] public bool IsEnabled { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int CreatorId { get; set; }
    [DataMember] public DateTime? DateModified { get; set; }
    [DataMember] public int? ModifierId { get; set; }
          
  }
}
