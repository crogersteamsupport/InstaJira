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
  [KnownType(typeof(SlaPausedTimProxy))]
  public class SlaPausedTimProxy
  {
    public SlaPausedTimProxy() {}
    [DataMember] public int Id { get; set; }
    [DataMember] public int TicketId { get; set; }
    [DataMember] public int TicketStatusId { get; set; }
    [DataMember] public int SlaTriggerId { get; set; }
    [DataMember] public DateTime PausedOn { get; set; }
    [DataMember] public DateTime? ResumedOn { get; set; }
    [DataMember] public int? BusinessPausedTime { get; set; }
          
  }
}
