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
  [KnownType(typeof(DeletedTicketProxy))]
  public class DeletedTicketProxy
  {
    public DeletedTicketProxy() {}
    [DataMember] public int ID { get; set; }
    [DataMember] public int TicketID { get; set; }
    [DataMember] public int TicketNumber { get; set; }
    [DataMember] public int OrganizationID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public DateTime DateDeleted { get; set; }
    [DataMember] public int DeleterID { get; set; }
          
  }
}
