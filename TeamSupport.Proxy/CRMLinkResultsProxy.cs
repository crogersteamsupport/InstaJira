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
  [KnownType(typeof(CRMLinkResultProxy))]
  public class CRMLinkResultProxy
  {
    public CRMLinkResultProxy() {}
    [DataMember] public int CRMResultsID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public DateTime? AttemptDateTime { get; set; }
    [DataMember] public string AttemptResult { get; set; }
          
  }
}
