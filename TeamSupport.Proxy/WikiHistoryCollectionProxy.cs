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
  [KnownType(typeof(WikiHistoryProxy))]
  public class WikiHistoryProxy
  {
    public WikiHistoryProxy() {}
    [DataMember] public int HistoryID { get; set; }
    [DataMember] public int ArticleID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string ArticleName { get; set; }
    [DataMember] public string Body { get; set; }
    [DataMember] public int? Version { get; set; }
    [DataMember] public int? CreatedBy { get; set; }
    [DataMember] public DateTime? CreatedDate { get; set; }
    [DataMember] public int? ModifiedBy { get; set; }
    [DataMember] public DateTime? ModifiedDate { get; set; }
          
  }
}
