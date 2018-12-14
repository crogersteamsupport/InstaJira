using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    /// <summary> 
    /// [dbo].[OrganizationTickets] 
    /// Client Tickets (customer is Organization.Parent) 
    /// </summary>
    [Table(Name = "OrganizationTickets")]
    class OrganizationTicket
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketID;
        [Column(Storage = "_ticketID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketID { get { return _ticketID; } }

        [Column]
        public int OrganizationID;
#pragma warning restore CS0649
    }
}
