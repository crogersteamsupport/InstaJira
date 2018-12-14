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
  [KnownType(typeof(SlaViolationHistoryItemProxy))]
  public class SlaViolationHistoryItemProxy
  {
    public SlaViolationHistoryItemProxy() {}
    [DataMember] public int SlaViolationHistoryID { get; set; }
    [DataMember] public int? UserID { get; set; }
    [DataMember] public int? GroupID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public SlaViolationType ViolationType { get; set; }
    [DataMember] public DateTime DateViolated { get; set; }
    [DataMember] public int? SlaTriggerId { get; set; }
          
  }
}
