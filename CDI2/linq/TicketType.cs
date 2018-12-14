using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{

    //--Avg age of open tickets
    //select avg(datediff(d, t.datecreated, getutcdate())) as AvgDaysTicketsCurrentlyOpen
    // from tickets as t, tickettypes as tt, ticketstatuses as ts
    //where
    //t.tickettypeid = tt.tickettypeid and t.organizationid=1078
    //and t.ticketstatusid = ts.ticketstatusid
    //and ts.isclosed = 0
    //and tt.name = 'issues'

    /// <summary> 
    /// [dbo].[TicketTypes] 
    /// The types of tickets for an organization
    /// </summary>
    [Table(Name = "TicketTypes")]
    class TicketType
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _ticketTypeID;
        [Column(Storage = "_ticketTypeID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int TicketTypeID { get { return _ticketTypeID; } }

        [Column]
        public string Name;
        [Column]
        public bool ExcludeFromCDI;
#pragma warning restore CS0649

    }
}
