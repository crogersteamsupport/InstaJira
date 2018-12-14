using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Configuration;
using TeamSupport.CDI.linq;

namespace TeamSupport.CDI
{
    class Customer
    {
        OrganizationAnalysis _organizationAnalysis;
        public HashSet<Client> _clients;
        MetricPercentiles _percentiles;

        public Customer(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
            _clients = new HashSet<Client>();
            LoadClients();
        }

        public int OrganizationID { get { return _organizationAnalysis.ParentID; } }


        /// <summary>
        /// ParentID = Customer, OrganizationID = Client
        /// </summary>
        void LoadClients()
        {
            TicketJoin[] allTickets = _organizationAnalysis.Tickets;//.Where(t => t.OrganizationID).ToArray();
            if (allTickets.Length == 0)
                return;

            Array.Sort(allTickets, (lhs, rhs) => lhs.OrganizationID.CompareTo(rhs.OrganizationID));

            // spin through each organization
            int startIndex = 0;
            int startId = allTickets[startIndex].OrganizationID;
            for (int i = 1; i < allTickets.Length; ++i)
            {
                if (allTickets[i].OrganizationID != startId)
                {
                    // not a client of ourselves
                    if (startId != _organizationAnalysis.OrganizationID)
                        _clients.Add(new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, i)));
                    startIndex = i;
                    startId = allTickets[startIndex].OrganizationID;
                }
            }

            _clients.Add(new Client(new OrganizationAnalysis(_organizationAnalysis._dateRange, allTickets, startIndex, allTickets.Length)));
        }

        public void AnalyzeTickets()
        {
            foreach (Client client in _clients)
                client.AnalyzeTickets();
        }

        linq.CDI_Settings _weights;

        public linq.CDI_Settings GetCDISettings(int organizationID)
        {
            if (_weights != null)
                return _weights;

            try
            {
                string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (DataContext db = new DataContext(connection))
                {
                    //db.Log = CDIEventLog.Instance;
                    db.ObjectTrackingEnabled = false;   // read-only
                    Table<CDI_Settings> table = db.GetTable<CDI_Settings>();
                    _weights = table.Where(s => s.OrganizationID==organizationID).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("CDI_Settings Read failed", e);
            }

            if ((_weights == null) ||
                !_weights.TotalTicketsWeight.HasValue ||
                !_weights.OpenTicketsWeight.HasValue ||
                !_weights.Last30Weight.HasValue ||
                !_weights.AvgDaysOpenWeight.HasValue ||
                !_weights.AvgDaysToCloseWeight.HasValue)
            {
                const float equalWeight = 0.2f;
                _weights = new linq.CDI_Settings()
                {
                    TotalTicketsWeight = equalWeight,
                    OpenTicketsWeight = equalWeight,
                    Last30Weight = equalWeight,
                    AvgDaysOpenWeight = equalWeight,
                    AvgDaysToCloseWeight = equalWeight
                };
            }

            return _weights;
        }

        public void InvokeCDIStrategy()
        {
            // calculate the percentiles for the client metrics
            List<Metrics> clientIntervals = new List<Metrics>();
            foreach (Client client in _clients)
            {
                Metrics current = client.GetCDIIntervalData();
                if(current != null)
                    clientIntervals.Add(current);
            }
            _percentiles = new MetricPercentiles(clientIntervals);

            // construct the customer strategy
            //_iCdiStrategy = new CustomerPercentileStrategy(clientIntervals, Callback);
            //_iCdiStrategy.CalculateCDI();    // customer CDI not used

            // calculate the CDI for each client
            linq.CDI_Settings settings = GetCDISettings(_organizationAnalysis.ParentID);
            foreach (Client client in _clients)
                client.InvokeCDIStrategy(_percentiles, settings);
        }

        public void Write()
        {
            CDIEventLog.Instance.WriteLine("------------------------------------------");
            List<Metrics> raw = new List<Metrics>();
            foreach (Client client in _clients)
                raw.Add(client.RawMetrics);
            Metrics.Write(raw);

            CDIEventLog.Instance.WriteLine("------------------------------------------");
            raw.Clear();
            foreach (Client client in _clients)
                raw.Add(client.NormalizedMetrics);
            Metrics.Write(raw);

            //IntervalData.WriteHeader();
            //foreach (Client client in _clients)
            //    CDIEventLog.Instance.WriteLine("{0}\t{1}", client.OrganizationID, client.NormalizedMetrics.ToString());
        }

        public override string ToString()
        {
            return _organizationAnalysis.ToString();
        }

        public void WriteCdiByOrganization()
        {
            CDIEventLog.Instance.WriteLine("------------------------------------------");
            CDIEventLog.Instance.WriteLine(_organizationAnalysis.OrganizationID.ToString());
            CDIEventLog.Instance.WriteLine("------------------------------------------");
            //CDIEventLog.Instance.Write("ClientID\tClient\tActiveWeeks\tCDI\tCDI-2\t");

            CDIEventLog.Instance.Write("ClientID\tClient\tCreatedLast30\tTotalTicketsCreated\tCDI\tnew\ttotal\tCDI-2\t");

            Metrics.WriteHeader();
            foreach (Client client in _clients)
                client.WriteCdiByOrganization(_organizationAnalysis.Intervals[0]._timeStamp);
        }

        public void WriteClients()
        {
            CDIEventLog.Instance.WriteLine("ClientID\tClient\tTotalTicketsCreated\tTicketsOpen\tCreatedLast30\tAvgTimeOpen\tAvgTimeToClose\tCustDisIndex\tDate\tTotalTicketsCreated\tTicketsOpen\tCreatedLast30\tAvgTimeOpen\tAvgTimeToClose\tCustDisIndex-2" +
                "CDI-2");
            foreach (Client client in _clients)
                client.Write();
        }

        public void Save(DataContext db, Table<OrganizationSentiment> table)
        {
            foreach (Client client in _clients)
                client.Save(db, table);
        }

        public void Save(Table<CustDistHistory> table)
        {
            foreach (Client client in _clients)
                client.Save(table);
        }


    }
}
