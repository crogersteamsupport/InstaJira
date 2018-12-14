using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    /// <summary> 
    /// [dbo].[Tickets] 
    /// </summary>
    [Table(Name = "Tickets")]
    class Ticket : IComparable<Ticket>
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketID;
        [Column(Storage = "_ticketID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketID { get { return _ticketID; } }

        [Column]
        public int TicketStatusID;
        [Column]
        public int TicketTypeID;
        [Column]
        public int TicketSeverityID;
        [Column]
        public int OrganizationID;
        [Column]
        public DateTime? DateClosed;
        [Column]
        public string TicketSource;
        [Column]
        public DateTime DateCreated;
#pragma warning restore CS0649

        public int CompareTo(Ticket other) { return DateCreated.CompareTo(other.DateCreated); }
        public override string ToString() { return DateCreated.ToShortDateString(); }
    }
}
