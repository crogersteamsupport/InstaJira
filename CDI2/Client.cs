using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Linq;

namespace TeamSupport.CDI
{
    class Client
    {
        OrganizationAnalysis _organizationAnalysis;
        public ICDIStrategy _iCdiStrategy;

        public Client(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
            _iCdiStrategy = new CDI1Strategy(_organizationAnalysis);
        }

        public int OrganizationID { get { return _organizationAnalysis.OrganizationID; } }
        public Metrics RawMetrics { get { return _iCdiStrategy.RawMetrics; } }
        public Metrics NormalizedMetrics { get { return _iCdiStrategy.NormalizedMetrics; } }

        public void AnalyzeTickets()
        {
            // summarize the data in IntervalTimeSpanInDays (30) day intervals
            _organizationAnalysis.GenerateIntervals();
            //Metrics.Write(_organizationAnalysis.Intervals);
        }

        Metrics _default = new Metrics();


        public Metrics GetCDIIntervalData()
        {
            return _iCdiStrategy.CalculateRawMetrics();
        }

        public void InvokeCDIStrategy(MetricPercentiles clientPercentiles, linq.CDI_Settings weights)
        {
            _iCdiStrategy.InvokeCDIStrategy(clientPercentiles, weights);
        }

        public void WriteCdiByOrganization(DateTime timestamp)
        {
            //if (!_organizationAnalysis.ClientOrganizationID.HasValue)
            //    return;

            //IntervalData last = _organizationAnalysis.Intervals.Last();
            //if (last._timeStamp < timestamp.AddDays(-90))
            //    return; // too old...

            //int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            //linq.Organization organization;
            //if (linq.Organization.TryGet(clientID, out organization))
            //{
            //    //CDIEventLog.Instance.Write("ClientID\tClient\tCreatedLast30\tTotalTicketsCreated\tCDI\tnew\ttotal\tCDI-2\t");
            //    CDIEventLog.Instance.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", clientID, organization.Name,
            //        organization.CreatedLast30, organization.TotalTicketsCreated, organization.CustDisIndex,
            //        last._newCount, last._totalTicketsCreated, last.CDI));
            //    //CDIEventLog.Instance.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", clientID, organization.Name, _organizationAnalysis.Intervals.Count, organization.CustDisIndex, last.CDI, last.ToString()));
            //}
        }

        public void WriteIntervals()
        {
            //int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            //linq.Organization organization;
            //if (!linq.Organization.TryGet(clientID, out organization))
            //    return;

            //string clientName = organization.Name;
            //foreach (IntervalData interval in _organizationAnalysis.Intervals)
            //    CDIEventLog.Instance.WriteLine(String.Format("{0}\t{1}\t{2}", clientID, clientName, interval.ToString()));
        }

        public void Write()
        {
            ////CDIEventLog.Instance.Write("ClientID\tClient\tTotalTicketsCreated\tTicketsOpen\tCreatedLast30\tAvgTimeOpen\tAvgTimeToClose\tCustDisIndex\tCDI-2");
            //int clientID = _organizationAnalysis.ClientOrganizationID.Value;
            //linq.Organization organization;
            //linq.Organization.TryGet(clientID, out organization);
            //CDIEventLog.Instance.WriteLine(String.Format("{0}\t{1}", organization.ToStringCDI1(), ToStringCDI1()));
        }

        /// <summary>
        /// The linq to sql save is very slow, including a WHERE clause for an exact record match.
        /// Thus use a hard-coded update
        /// </summary>
        /// <param name="db"></param>
        public void Save(DataContext db, Table<linq.OrganizationSentiment> table)
        {
            _iCdiStrategy.Save(db, table);
        }

        /// <summary>
        /// Save - linq is fast enough on insert of new records
        /// WARNING - there is a cron job in the product which deletes CustDistHistory records more than 10 days old
        /// </summary>
        /// <param name="table">table of existing customer distress history</param>
        public void Save(Table<linq.CustDistHistory> table)
        {
            _iCdiStrategy.Save(table);
        }
    }
}