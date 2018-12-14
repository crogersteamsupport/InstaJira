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
  [KnownType(typeof(TicketRelationshipProxy))]
  public class TicketRelationshipProxy
  {
    public TicketRelationshipProxy() {}
    [DataMember] public int TicketRelationshipID { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public int Ticket1ID { get; set; }
    [DataMember] public int Ticket2ID { get; set; }
    [DataMember] public int CreatorID { get; set; }
    [DataMember] public DateTime DateCreated { get; set; }
    [DataMember] public int? ImportFileID { get; set; }
          
  }
}
