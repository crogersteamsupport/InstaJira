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
  
  public partial class TicketRelationship : BaseItem
  {
    public TicketRelationshipProxy GetProxy()
    {
      TicketRelationshipProxy result = new TicketRelationshipProxy();
      result.ImportFileID = this.ImportFileID;
      result.CreatorID = this.CreatorID;
      result.Ticket2ID = this.Ticket2ID;
      result.Ticket1ID = this.Ticket1ID;
      result.OrganizationID = this.OrganizationID;
      result.TicketRelationshipID = this.TicketRelationshipID;
       
      result.DateCreated = DateTime.SpecifyKind(this.DateCreatedUtc, DateTimeKind.Utc);
       
       
      return result;
    }	
  }
}
