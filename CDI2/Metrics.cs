using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TeamSupport.CDI
{
    public enum EMetrics
    {
        New30 = 0x1,    // _newCount
        Open = 0x2,   // _openCount
        DaysOpen = 0x4,   // _medianDaysOpen
        TotalTickets = 0x8,   // _totalTicketsCreated
        Closed30 = 0x10, // _closedCount
        DaysToClose = 0x20,    // _medianDaysToClose
        ActionCountClosed = 0x40,    // _averageActionCount
        SeverityClosed = 0x80,    // _averageSeverity
        SentimentClosed = 0x100
    };

    /// <summary>
    /// Keep the results for the analysis of closed tickets for the given time interval
    /// </summary>
    public class Metrics
    {
        public DateTime _timeStamp; // date time for this data

        // new tickets
        public int _newCount; // new (this interval)
        public int _openCount;    // currently Open (since start)
        public double _medianDaysOpen; // median of currently open tickets (days)
        public int _totalTicketsCreated;    // used for Robert's algorithm

        // closed tickets
        public int _closedCount;  // closed (this interval)
        public double? _medianDaysToClose;  // average time to close (this interval)
        public double? _averageActionCount;   // how many actions do the closed ticket have?
        public double? _averageSentimentScore;
        public double? _averageSeverity;
        //public int _CreatorIDCount;  // how many users created the closed tickets

        public int? CDI { get; set; }    // CDI !!

        public Metrics() { }

        public Metrics(DateTime nextDay, HashSet<TicketJoin> openTickets, HashSet<TicketJoin> closedTickets, int ticketsCreated, int totalTicketsCreated)
        {
            _timeStamp = nextDay;
            _newCount = ticketsCreated;
            _openCount = openTickets.Count;
            _medianDaysOpen = openTickets.Count == 0 ? 0 : MedianTotalDaysOpenToTimestamp(openTickets, _timeStamp).Value;
            _totalTicketsCreated = totalTicketsCreated;
            _closedCount = closedTickets.Count;

            if (closedTickets.Count > 0)
            {
                _medianDaysToClose = MedianTotalDaysToClose(closedTickets);
                _averageActionCount = closedTickets.Average(x => x.ActionsCount);
                _averageSentimentScore = closedTickets.Average(x => x.AverageActionSentiment);
                _averageSeverity = closedTickets.Average(x => x.Severity);
                //_CreatorIDCount = closedTickets.Select(t => t.CreatorID).Distinct().Count();
            }
        }

        public double? GetAsCDIPercentile(EMetrics metric)
        {
            double? result = null;
            switch(metric)
            {
                case EMetrics.ActionCountClosed:
                    result = _averageActionCount;
                    break;
                case EMetrics.Closed30:
                    result = 100 - _closedCount;  // invert - high is good
                    break;
                case EMetrics.DaysOpen:
                    result = _medianDaysOpen;
                    break;
                case EMetrics.DaysToClose:
                    result = _medianDaysToClose;
                    break;
                case EMetrics.New30:
                    result = _newCount;
                    break;
                case EMetrics.Open:
                    result = _openCount;
                    break;
                case EMetrics.SentimentClosed:
                    result = 100 - _averageSentimentScore;  // invert - high is good
                    break;
                case EMetrics.SeverityClosed:
                    result = 100 - _averageSeverity;    // invert - high is good
                    break;
                case EMetrics.TotalTickets:
                    result = _totalTicketsCreated;
                    break;
            }
            return result;
        }

        public static double? Median(double[] values)
        {
            double? result = null;
            if (values.Length > 0)
            {
                result = values.Average();  // use average for now so it more closely matches CDI1
                //Array.Sort(values);
                //int centerIndex = values.Length / 2;
                //result = (values.Length % 2 == 1) ? values[centerIndex] : (values[centerIndex - 1] + values[centerIndex]) / 2;
                //CDIEventLog.Instance.WriteLine(String.Format("{0:0.0}, {1:0.0}", values.Average(), result.Value));
            }
            return result;
        }

        public static double? MedianTotalDaysToClose(TicketJoin[] tickets)
        {
            if (tickets.Length == 0)
                return null;

            TicketJoin[] closedTickets = tickets.Where(t => t.IsClosed && t.DateClosed.HasValue).ToArray();
            double[] totalDays = closedTickets.Select(ticket => ticket.TotalDaysToClose.Value).ToArray();
            return Median(totalDays);
        }

        public static double? MedianTotalDaysToClose(HashSet<TicketJoin> tickets)
        {
            if (tickets.Count == 0)
                return null;

            TicketJoin[] closedTickets = tickets.Where(t => t.IsClosed && t.DateClosed.HasValue).ToArray();
            double[] totalDays = closedTickets.Select(ticket => ticket.TotalDaysToClose.Value).ToArray();
            return Median(totalDays);
        }

        private static double? MedianTotalDaysOpenToTimestamp(HashSet<TicketJoin> tickets, DateTime timestamp)
        {
            if (tickets.Count == 0)
                return null;

            double[] totalDays = tickets.Select(ticket => ticket.TotalDaysOpenToTimestamp(timestamp)).ToArray();
            return Median(totalDays);
        }

        public static void WriteHeader()
        {
            CDIEventLog.Instance.WriteLine("Date\tNew\tOpen\tMedianDaysOpen\tClosed\tMedianDaysToClose\tAvgActions\tAvgSentiment\tAverageSeverity\tCDI");
        }

        public static void Write(List<Metrics> intervals)
        {
            WriteHeader();
            foreach (Metrics interval in intervals)
                CDIEventLog.Instance.WriteLine(interval.ToString());
        }

        public override string ToString()
        {
            return String.Format("{0} \t{1} \t{2} \t{3:0.00} \t{4} \t{5:0.00} \t{6:0.00} \t{7:0.00} \t{8:0.00} \t{9}",
                _timeStamp.ToShortDateString(), _newCount, _openCount, _medianDaysOpen, _closedCount, _medianDaysToClose, _averageActionCount, _averageSentimentScore, _averageSeverity, CDI);
        }

        public string ToStringCDI1()
        {
            return String.Format("{0} \t{1} \t{2} \t{3} \t{4:0.00} \t{5:0.00} \t{6}",
                _timeStamp.ToShortDateString(),
                _totalTicketsCreated, // 1. TotalTicketsCreated
                _newCount, // 2. CreatedLast30
                _openCount,   // 3. TicketsOpen
                _medianDaysOpen, // 4. AvgTimeOpen
                _medianDaysToClose,  // 5. AvgTimeToClose
                CDI);
        }

    }

}
