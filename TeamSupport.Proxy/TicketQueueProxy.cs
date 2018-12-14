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
  [KnownType(typeof(TicketQueueItemProxy))]
  public class TicketQueueItemProxy
  {
    public TicketQueueItemProxy() {}
    [DataMember] public int TicketQueueID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int UserID { get; set; }
    [DataMember] public decimal? EstimatedDays { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public DateTime DateModified { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public int ModifierID { get; set; }
          
  }
}
