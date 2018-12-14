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
  [KnownType(typeof(PortalLoginHistoryItemProxy))]
  public class PortalLoginHistoryItemProxy
  {
    public PortalLoginHistoryItemProxy() {}
    [DataMember] public int PortalLoginID { get; set; }
    [DataMember] public string UserName { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public string OrganizationName { get; set; }
    [DataMember] public bool? Success { get; set; }
    [DataMember] public DateTime? LoginDateTime { get; set; }
    [DataMember] public string IPAddress { get; set; }
    [DataMember] public string Browser { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public string Source { get; set; }
          
  }
}
