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
  [KnownType(typeof(ArticleStatProxy))]
  public class ArticleStatProxy
  {
    public ArticleStatProxy() {}
    [DataMember] public int ArticleViewID { get; set; }
    [DataMember] public int? OrganizationID { get; set; }
    [DataMember] public int? ArticleID { get; set; }
    [DataMember] public DateTime? ViewDateTime { get; set; }
    [DataMember] public string ViewIP { get; set; }
    [DataMember] public int? UserID { get; set; }
          
  }
}
