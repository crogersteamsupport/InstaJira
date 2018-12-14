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
  [KnownType(typeof(TicketStatusProxy))]
  public class TicketStatusProxy
  {
    public TicketStatusProxy() {}
    [DataMember] public int TicketStatusID { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public string Description { get; set; }
    [DataMember] public int Position { get; set; }
    [DataMember] public int TicketTypeID { get; set; }
    [DataMember] public bool IsClosed { get; set; }
    [DataMember] public bool IsClosedEmail { get; set; }
    [DataMember] public bool IsEmailResponse { get; set; }
    //[DataMember] public int OrganizationID { get; set; }
    //[DataMember] public DateTime DateCreated { get; set; }
    //[DataMember] public DateTime DateModified { get; set; }
    //[DataMember] public int CreatorID { get; set; }
    //[DataMember] public int ModifierID { get; set; }
    [DataMember] public bool PauseSLA { get; set; }
          
  }
}
