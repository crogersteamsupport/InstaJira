using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace TeamSupport.Data
{
 
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
