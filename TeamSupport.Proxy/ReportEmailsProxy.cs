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
  [KnownType(typeof(ReportEmailProxy))]
  public class ReportEmailProxy
  {
    public ReportEmailProxy() {}
    [DataMember] public int ReportEmailID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public int ReportID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime? DateSent { get; set; }
          
  }
}
