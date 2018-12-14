using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.Linq;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Interface to CDI strategy
    /// </summary>
    public interface ICDIStrategy
    {
        Metrics CalculateRawMetrics();
        void InvokeCDIStrategy(MetricPercentiles clientPercentiles, linq.CDI_Settings weights);
        void Save(DataContext db, Table<linq.OrganizationSentiment> table);
        void Save(Table<linq.CustDistHistory> table);
        int? CDI { get; }

        // for unit testing
        Metrics RawMetrics { get; }
        Metrics NormalizedMetrics { get; }
    }


    /// <summary>
    /// Generate CDI metrics for an individual organization
    /// </summary>
    class OrganizationAnalysis
    {
        public DateRange _dateRange { get; private set; }
        public TicketJoin[] Tickets { get; private set; }
        public int OrganizationID { get { return Tickets[0].OrganizationID; } }
        public int ParentID { get { return Tickets[0].ParentID.Value; } }
        IntervalStrategy _intervalStrategy;
        public List<Metrics> Intervals { get; private set; }
        //public int CreatorIDCount { get; private set; }
        //public int TicketCount { get; private set; }

        /// <summary>
        /// Create from subset of all tickets (faster than using linq to query)
        /// </summary>
        /// <param name="analysisInterval">date range and intervals to analyze</param>
        /// <param name="allTickets">complete set for date range, all organizations</param>
        /// <param name="startIndex">start index into all tickets for this organization</param>
        /// <param name="endIndex">end index into all tickets for this organization</param>
        public OrganizationAnalysis(DateRange analysisInterval, TicketJoin[] allTickets, int startIndex, int endIndex)
        {
            try
            {
                _dateRange = analysisInterval;

                // pull out the range for this organization
                //TicketCount = endIndex - startIndex;
                Tickets = new TicketJoin[endIndex - startIndex];
                Array.Copy(allTickets, startIndex, Tickets, 0, Tickets.Length);
                //CreatorIDCount = Tickets.Select(t => t.CreatorID).Distinct().Count();

                // collect metrics for each interval
                _intervalStrategy = new IntervalStrategy(Tickets);
            }
            catch(Exception ex)
            {
                CDIEventLog.Instance.WriteEntry("New organization failed", ex);
            }
        }

        public Metrics Current()
        {
            if (Intervals.Last()._timeStamp == _dateRange.EndDate)
                return Intervals.Last();

            return null;
        }

        public void GenerateIntervals()
        {
            Intervals = _intervalStrategy.GenerateIntervalData(_dateRange);
        }

        public double MedianDaysOpen()
        {
            double[] daysOpen = Tickets.Where(t => !t.IsClosed && !t.DateClosed.HasValue).Select(t => t.TotalDaysOpenToTimestamp(DateRange.EndTimeNow)).ToArray();
            if (daysOpen.Length == 0)
                return 0;
            return Metrics.Median(daysOpen).Value;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}({2})", OrganizationID, Intervals.Count, Tickets.Length);
        }

        public string CDIValuesToString()
        {
            Intervals.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
            int intervalIndex = 0;
            StringBuilder str = new StringBuilder();
            str.Append(OrganizationID);
            foreach (DateTime time in _dateRange)
            {
                str.Append("\t");
                if((intervalIndex < Intervals.Count()) && (Intervals[intervalIndex]._timeStamp == time))
                    str.Append(Intervals[intervalIndex++].CDI);
            }

            //if (intervalIndex != IntervalData.Count() - 1)
            //    Debugger.Break();

            return str.ToString();
        }

        public void WriteItervals()
        {
            Metrics.WriteHeader();
            foreach (Metrics interval in Intervals)
                CDIEventLog.Instance.WriteLine(interval.ToString());
        }

    }
}
