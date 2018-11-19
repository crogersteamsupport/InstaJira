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
  [KnownType(typeof(ReportSubcategoryProxy))]
  public class ReportSubcategoryProxy
  {
    public ReportSubcategoryProxy() {}
    [DataMember] public int ReportSubcategoryID { get; set; }
    [DataMember] public int ReportCategoryTableID { get; set; }
    [DataMember] public int? ReportTableID { get; set; }
    [DataMember] public string BaseQuery { get; set; }
          
  }
}