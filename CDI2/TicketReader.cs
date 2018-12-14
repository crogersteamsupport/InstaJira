using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Linq;
using System.IO;
using TeamSupport.CDI.linq;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Load the tickets from the database from AnalysisInterval StartDate to EndDate
    /// * ignore tickets closed in less than a second
    /// * ignore tickets imported from SalesForce
    /// * ignore ExcludeFromCDI ticket types and ticket statuses
    /// </summary>
    class TicketReader
    {
        DateRange _dateRange;

        TicketJoin[] _allTickets;
        public TicketJoin[] AllTickets { get { return _allTickets; } }

        /// <summary>Time frame to analyze the ticket data</summary>
        /// <param name="analysisInterval"></param>
        public TicketReader(DateRange analysisInterval)
        {
            _dateRange = analysisInterval;
        }

        // TEST QUERY - CreatedLast30
        //use[TeamSupportNightly]
        //SELECT TOP 1000 [CDI2ID]
        //    ,c.[OrganizationID]
        //    ,c.[ParentID]
        //    ,c.[CreatedLast30]
        //    ,o.CreatedLast30
        //    ,(SELECT COUNT(*)
        //    FROM organizations AS o
        //    JOIN OrganizationTickets AS ot ON ot.OrganizationID=o.OrganizationID
        //    JOIN tickets AS t ON ot.ticketid=t.ticketid
        //    WHERE  o.isactive=1 
        //    and t.datecreated > cast(GETUTCDATE()-29 as date)
        //    and o.OrganizationID=c.OrganizationID) as CreatedLast30
        //    FROM[dbo].[CDI] AS c
        //    JOIN dbo.Organizations o on c.OrganizationID= o.OrganizationID

        // TEST QUERY - TicketsOpen
        //use TeamSupportNightly
        //SELECT o.OrganizationID, o.Name, Count(*) as TicketsOpen, c.TicketsOpen AS TicketsOpen2
        //FROM   organizations       AS o
        //	JOIN organizationtickets AS ot ON o.OrganizationID = ot.OrganizationID
        //	JOIN Tickets AS t ON ot.TicketID = t.TicketID
        //	JOIN TicketStatuses AS s ON t.TicketStatusID = s.TicketStatusID
        //	JOIN CDI AS c on o.OrganizationID = c.OrganizationID
        //WHERE  o.isactive=1 
        //	AND    s.isclosed=0 
        //	AND    o.ParentID=1078 
        //	AND    t.DateCreated > GETUTCDATE() - 359
        //GROUP BY o.OrganizationID, o.Name, c.TicketsOpen
        //ORDER BY TicketsOpen DESC

        public void LoadAllTickets()
        {
            //new WatsonTicketSize().WatsonActionSize();;
            
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    //db.Log = CDIEventLog.Instance;

                    // define the tables we will reference in the query
                    // - each table class only contains the fields we care about
                    db.ObjectTrackingEnabled = false;   // read-only
                    Table<Organization> organizationTable = db.GetTable<Organization>();
                    Table<OrganizationTicket> organizationTicketsTable = db.GetTable<OrganizationTicket>();
                    Table<Ticket> ticketsTable = db.GetTable<Ticket>();
                    Table<TicketStatus> ticketStatusesTable = db.GetTable<TicketStatus>();
                    Table<TicketType> ticketTypesTable = db.GetTable<TicketType>();
                    Table<TicketSentiment> ticketSentimentsTable = db.GetTable<TicketSentiment>();
                    Table<TicketSeverity> severityTable = db.GetTable<TicketSeverity>();

                    var query = from o in organizationTable.Where(o => o.IsActive == true)
                                join ot in organizationTicketsTable on o.OrganizationID equals ot.OrganizationID
                                join t in ticketsTable on ot.TicketID equals t.TicketID
                                join tt in ticketTypesTable on t.TicketTypeID equals tt.TicketTypeID
                                join ts in ticketStatusesTable on t.TicketStatusID equals ts.TicketStatusID
                                where (t.DateCreated > _dateRange.StartDate) &&
                                    (!ts.IsClosed || (t.DateClosed.Value > t.DateCreated)) &&
                                    (t.TicketSource != "SalesForce") &&    // ignore imported tickets
                                    //(o.ParentID == 1078) && // 1116568
                                    //(o.OrganizationID == 563644) &&
                                    (o.IsActive) &&
                                    (!tt.ExcludeFromCDI) &&
                                    (!ts.ExcludeFromCDI)
                                select new TicketJoin()
                                {
                                    OrganizationID = ot.OrganizationID,
                                    DateClosed = t.DateClosed,
                                    DateCreated = t.DateCreated,
                                    IsClosed = ts.IsClosed,
                                    ActionsCount = (from tst in ticketSentimentsTable where t.TicketID == tst.TicketID select tst.ActionSentimentCount).Min(),
                                    AverageActionSentiment = (from tst in ticketSentimentsTable where t.TicketID == tst.TicketID select tst.AverageActionSentiment).Min(),  // for some reason Min is faster than First()
                                    ParentID = o.ParentID,
                                    Severity = (from s in severityTable where t.TicketSeverityID == s.TicketSeverityID select s.Severity).Min()
                                };

                    // run the query
                   _allTickets = query.ToArray();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("Ticket Read failed", e);
            }
        }

        public void LoadNeedComputeOrganizationTickets()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    //db.Log = CDIEventLog.Instance;
                    Table<CDI_Settings> table = db.GetTable<CDI_Settings>();
                    CDI_Settings[] settings = table.Where(t => t.NeedCompute.HasValue && t.NeedCompute.Value).ToArray();
                    foreach(CDI_Settings setting in settings)
                    {
                        setting.NeedCompute = false;
                        setting.LastCompute = DateRange.EndTimeNow;
                        TicketJoin[] tickets = LoadTickets(db, setting.OrganizationID);
                        if(AllTickets == null)
                        {
                            _allTickets = tickets;  // first time just use the array
                        }
                        else
                        {
                            // append additional tickets to analyze
                            int initialLength = _allTickets.Length;
                            Array.Resize(ref _allTickets, _allTickets.Length + tickets.Length);
                            Array.Copy(tickets, 0, _allTickets, initialLength, tickets.Length);
                        }
                    }
                    db.SubmitChanges(); // turn off NeedCompute
                }
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("Ticket Read failed", e);
            }
        }

        TicketJoin[] LoadTickets(DataContext db, int organizationID)
        {
            // define the tables we will reference in the query
            // - each table class only contains the fields we care about
            Table<Organization> organizationTable = db.GetTable<Organization>();
            Table<OrganizationTicket> organizationTicketsTable = db.GetTable<OrganizationTicket>();
            Table<Ticket> ticketsTable = db.GetTable<Ticket>();
            Table<TicketStatus> ticketStatusesTable = db.GetTable<TicketStatus>();
            Table<TicketType> ticketTypesTable = db.GetTable<TicketType>();
            Table<TicketSentiment> ticketSentimentsTable = db.GetTable<TicketSentiment>();
            Table<TicketSeverity> severityTable = db.GetTable<TicketSeverity>();

            var query = from o in organizationTable.Where(o => o.IsActive == true)
                        join ot in organizationTicketsTable on o.OrganizationID equals ot.OrganizationID
                        join t in ticketsTable on ot.TicketID equals t.TicketID
                        join tt in ticketTypesTable on t.TicketTypeID equals tt.TicketTypeID
                        join ts in ticketStatusesTable on t.TicketStatusID equals ts.TicketStatusID
                        where (t.DateCreated > _dateRange.StartDate) &&
                            (!ts.IsClosed || (t.DateClosed.Value > t.DateCreated)) &&
                            (t.TicketSource != "SalesForce") &&    // ignore imported tickets
                            (o.ParentID == organizationID) &&
                            (o.IsActive) &&
                            (!tt.ExcludeFromCDI) &&
                            (!ts.ExcludeFromCDI)
                        select new TicketJoin()
                        {
                            OrganizationID = ot.OrganizationID,
                            DateClosed = t.DateClosed,
                            DateCreated = t.DateCreated,
                            IsClosed = ts.IsClosed,
                            ActionsCount = (from tst in ticketSentimentsTable where t.TicketID == tst.TicketID select tst.ActionSentimentCount).Min(),
                            AverageActionSentiment = (from tst in ticketSentimentsTable where t.TicketID == tst.TicketID select tst.AverageActionSentiment).Min(),  // for some reason Min is faster than First()
                            ParentID = o.ParentID,
                            Severity = (from s in severityTable where t.TicketSeverityID == s.TicketSeverityID select s.Severity).Min()
                        };

            // run the query
            return query.ToArray();
        }

    }
}
