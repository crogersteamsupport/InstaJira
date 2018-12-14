using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    /// <summary> 
    /// [dbo].[TicketStatuses] 
    /// The status values allowed for an organization
    /// </summary>
    [Table(Name = "TicketStatuses")]
    class TicketStatus
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketStatusID;
        [Column(Storage = "_ticketStatusID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketStatusID { get { return _ticketStatusID; } }

        [Column]
        public string Name;
        [Column]
        public bool IsClosed;
        [Column]
        public bool ExcludeFromCDI;
#pragma warning restore CS0649
    }
}
