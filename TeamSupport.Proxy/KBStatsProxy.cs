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
  [KnownType(typeof(KBStatProxy))]
  public class KBStatProxy
  {
    public KBStatProxy() {}
    [DataMember] public int KBViewID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int? KBArticleID { get; set; }
    [DataMember] public DateTime? ViewDateTime { get; set; }
    [DataMember] public string ViewIP { get; set; }
    [DataMember] public string SearchTerm { get; set; }
          
  }
}
