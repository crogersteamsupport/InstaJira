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
  [KnownType(typeof(FullContactUpdatesItemProxy))]
  public class FullContactUpdatesItemProxy
  {
    public FullContactUpdatesItemProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int? UserId { get; set; }
    [DataMember] public int? OrganizationId { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
          
  }
}
