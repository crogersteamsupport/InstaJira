using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Analyze the last year of tickets
    /// Do calculations in days to prevent Ticks overflow
    /// </summary>
    class IntervalStrategy
    {
        TicketJoin[] _tickets;

        public IntervalStrategy(TicketJoin[] tickets)
        {
            _tickets = tickets;
        }

        /// <summary> organize tickets for each open and close </summary>
        List<Tuple<DateTime, TicketJoin>> GetAsChronological()
        {
            // insert each ticket by DateCreated and DateClosed (duplicates allowed)
            List<Tuple<DateTime, TicketJoin>> chronological = new List<Tuple<DateTime, TicketJoin>>();   // allow duplicates for DateTime
            foreach (TicketJoin t in _tickets)
            {
                chronological.Add(new Tuple<DateTime, TicketJoin>(t.DateCreated, t));  // opened
                if(t.DateClosed.HasValue)
                    chronological.Add(new Tuple<DateTime, TicketJoin>(t.DateClosed.Value, t));  // closed
            }

            // sort by date and TicketID
            chronological.Sort((x, y) =>
            {
                int result = x.Item1.CompareTo(y.Item1);
                return result == 0 ? x.Item2.CompareTo(y.Item2) : result;
            });

            return chronological;
        }

        /// <summary>
        /// Sample the days open at the specified interval
        /// </summary>
        /// <param name="dateRange">how often do we sample - daily, weekly,... </param>
        public List<Metrics> GenerateIntervalData(DateRange dateRange)
        {
            List<Metrics> results = new List<Metrics>();
            try
            {
                // tickets open at that moment in time
                HashSet<TicketJoin> currentlyOpenTickets = new HashSet<TicketJoin>();    // tickets open at that moment in time
                HashSet<TicketJoin> intervalClosedTickets = new HashSet<TicketJoin>();  // tickets closed in that time interval
                int newTicketsCount = 0;

                // Complete list of all the ticket open and close times
                List<Tuple<DateTime, TicketJoin>> chronological = GetAsChronological();

                // move to the first interval where we have data
                DateTime nextInterval = dateRange.StartDate + dateRange.IntervalTimeSpan;
                while (nextInterval < chronological[0].Item1)
                    nextInterval += dateRange.IntervalTimeSpan;

                // walk through all the ticket open/close and keep the running tally for each interval
                int totalTicketsCreated = 0;
                foreach (Tuple<DateTime, TicketJoin> pair in chronological)
                {
                    while (pair.Item1 > nextInterval)
                    {
                        // snapshot of the data at this time
                        if ((currentlyOpenTickets.Count() > 0) || (intervalClosedTickets.Count() > 0) || (newTicketsCount > 0))
                        {
                            totalTicketsCreated += newTicketsCount;
                            results.Add(new Metrics(nextInterval, currentlyOpenTickets, intervalClosedTickets, newTicketsCount, totalTicketsCreated));
                            intervalClosedTickets.Clear();
                            newTicketsCount = 0;
                        }
                        nextInterval += dateRange.IntervalTimeSpan;
                    }

                    if (pair.Item1 == pair.Item2.DateCreated)
                    {
                        currentlyOpenTickets.Add(pair.Item2);    // new ticket opened
                        ++newTicketsCount;
                    }
                    else
                    {
                        currentlyOpenTickets.Remove(pair.Item2); // ticket closed
                        intervalClosedTickets.Add(pair.Item2);
                    }
                }

                // the final interval
                results.Add(new Metrics(nextInterval, currentlyOpenTickets, intervalClosedTickets, newTicketsCount, totalTicketsCreated + newTicketsCount));
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("AnalyzeDaysOpen failed", e);
            }

            return results;
        }

        /// <summary> Count the days open </summary>
        //void TimeOpenHistogram(TimeScale timeScale)
        //{
        //    CDIEventLog.Instance.WriteLine("TimeOpen({0})	TicketCount", timeScale);
        //    TallyDictionary<int> open = new TallyDictionary<int>();
        //    foreach (TicketJoin t in _tickets)
        //    {
        //        int timeOpen = (int)Math.Round(t.ScaledTimeOpen(timeScale));
        //        open.Increment(timeOpen);
        //    }

        //    open.Write();
        //}

        /// <summary> Count the ticket types </summary>
        //void TicketTypeHistogram(TimeScale timeScale)
        //{
        //    CDIEventLog.Instance.WriteLine("TicketType({0})	TicketCount", timeScale);
        //    TallyDictionary<string> open = new TallyDictionary<string>();
        //    foreach (TicketJoin t in _tickets)
        //        open.Increment(t.TicketTypeName);

        //    open.Write();
        //}

        /// <summary> Count the ticket types </summary>
        //void TicketDateCreatedHistogram(DateRange dateRange)
        //{
        //    TallyDictionary<DateTime> open = new TallyDictionary<DateTime>();
        //    foreach (TicketJoin t in _tickets)
        //        open.Increment(dateRange.TonightMidnight(t.DateCreated));

        //    open.Write();
        //}
    }
}

