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
  [KnownType(typeof(SlaNotificationProxy))]
  public class SlaNotificationProxy
  {
    public SlaNotificationProxy() {}
    [DataMember] public int TicketID { get; set; }
    [DataMember] public DateTime? TimeClosedViolationDate { get; set; }
    [DataMember] public DateTime? LastActionViolationDate { get; set; }
    [DataMember] public DateTime? InitialResponseViolationDate { get; set; }
    [DataMember] public DateTime? TimeClosedWarningDate { get; set; }
    [DataMember] public DateTime? LastActionWarningDate { get; set; }
    [DataMember] public DateTime? InitialResponseWarningDate { get; set; }
          
  }
}
