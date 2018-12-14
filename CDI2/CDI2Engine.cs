using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Configuration;
using TeamSupport.CDI.linq;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Data analysis engine 
    ///     load tickets
    ///     collect metrics across time intervals
    ///     calculate CDI based on distribution of time intervals (worse than usual?)
    ///     detect trends
    /// </summary>
    public class CDI2Engine
    {
        DateRange _dateRange;
        TicketReader _ticketReader;    // cache the tickets for analysis
        HashSet<Customer> _customers;  // raw analysis by organization
        bool _verboseLog;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="timeSpan">interval to sample and average across</param>
        /// <param name="intervalCount">Use an even number of intervals</param>
        public CDI2Engine(TimeSpan timeSpan, int intervalCount)
        {
            _dateRange = new DateRange(timeSpan, intervalCount);
            _ticketReader = new TicketReader(_dateRange);
            _verboseLog = false;
        }

        void VerboseLog(string text)
        {
            if (_verboseLog)
                CDIEventLog.Instance.WriteEntry(text);
            else
                Console.WriteLine(text);    // command line execution
        }

        public void Run(string[] args)
        {
            if (args.Contains("verbose"))
                _verboseLog = true;

            // Load the tickets
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (args.Contains("ForceCDIUpdate"))
                _ticketReader.LoadNeedComputeOrganizationTickets();
            else
            {
                CDIEventLog.Instance.WriteEntry("CDI Update started...");
                _ticketReader.LoadAllTickets();
            }

            if ((_ticketReader.AllTickets == null) || (_ticketReader.AllTickets.Length == 0))
                return;
            VerboseLog(String.Format("{0} Tickets loaded {1} in {2:0.00} sec",
                 _ticketReader.AllTickets.Length, _dateRange, stopwatch.ElapsedMilliseconds / 1000.0));

            // Load Customers and Clients
            stopwatch.Restart();
            LoadCustomers();
            VerboseLog(String.Format("Customers and Clients loaded in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000.0));

            // Analyze the tickets
            stopwatch.Restart();
            foreach (Customer customer in _customers)
                customer.AnalyzeTickets();
            VerboseLog(String.Format("Tickets analyzed in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000.0));

            // Do the CDI dirty work
            stopwatch.Restart();
            foreach (Customer customer in _customers)
            {
                customer.InvokeCDIStrategy();
                //customer.Write();

                //OptimalWeighting stats = new OptimalWeighting(customer);
                //stats.CalculatePercentiles();
                //stats.FindOptimalMix();
            }
            CDIEventLog.Instance.WriteEntry(String.Format("CDI values generated in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000.0));

            // Save to Client Organizations
            stopwatch.Restart();
            SaveOrganizations();
            VerboseLog(String.Format("Organization CDI Saved in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000.0));

            // Save History
            stopwatch.Restart();
            SaveCustDistHistory();
            VerboseLog(String.Format("CDI History Created in {0:0.00} sec", stopwatch.ElapsedMilliseconds / 1000.0));

            // unit test output...
            UnitTest(args);
        }

        /// <summary>
        /// ParentID = Customer, OrganizationID = Client
        /// </summary>
        void LoadCustomers()
        {
            TicketJoin[] allTickets = _ticketReader.AllTickets;
            Array.Sort(allTickets, (lhs, rhs) => lhs.ParentID.Value.CompareTo(rhs.ParentID.Value));
            _customers = new HashSet<Customer>();

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].ParentID.Value;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if(allTickets[i].ParentID != startId)
                {
                    _customers.Add(new Customer(new OrganizationAnalysis(_dateRange, allTickets, startIndex, i)));
                    startIndex = i;
                    startId = allTickets[startIndex].ParentID.Value;
                }
            }

            _customers.Add(new Customer(new OrganizationAnalysis(_dateRange, allTickets, startIndex, allTickets.Length)));
        }

        /// <summary>Save client organization CDI data</summary>
        void SaveOrganizations()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    //db.Log = CDIEventLog.Instance;
                    Table<OrganizationSentiment> table = db.GetTable<OrganizationSentiment>();
                    OrganizationSentiment.Initialize(db);
                    foreach (Customer customer in _customers)
                        customer.Save(db, table);
                    db.SubmitChanges();

                    // only keep the records for organizations with data
                    OrganizationSentiment.DeleteRemainder(db);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("Save failed", e);
            }
        }

        /// <summary>Save client organization CDI data</summary>
        public void SaveCustDistHistory()
        {
            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    //db.Log = CDIEventLog.Instance;
                    Table<CustDistHistory> table = db.GetTable<CustDistHistory>();
                    foreach (Customer customer in _customers)
                        customer.Save(table);
                    db.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("Save failed", e);
            }
        }

        void UnitTest(string[] args)
        {
            // test output
            Customer customer;
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                switch (arg)
                {
                    case "customer":
                        if (TryGetCustomer(int.Parse(args[++i]), out customer))
                            customer.WriteCdiByOrganization();
                        break;
                    //case "customerIntervals":
                    //    if (TryGetCustomer(int.Parse(args[++i]), out customer))
                    //        customer.WriteIntervals();
                    //    break;
                    case "clients":
                        if (TryGetCustomer(int.Parse(args[++i]), out customer))
                            customer.WriteClients();
                        break;
                }
            }
        }

        bool TryGetCustomer(int id, out Customer customer)
        {
            customer = _customers.Where(c => c.OrganizationID == id).FirstOrDefault();
            return customer != null;
        }


    }
}
