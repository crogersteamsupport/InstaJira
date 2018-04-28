﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Interface to CDI strategy
    /// </summary>
    interface ICDIStrategy
    {
        void CalculateCDI(IntervalData intervalData);
    }


    /// <summary>
    /// Generate CDI metrics for an individual organization
    /// </summary>
    class Organization
    {
        DateRange _dateRange;
        TicketJoin[] _tickets;
        public int OrganizationID { get; private set; }
        IntervalStrategy _intervalStrategy;
        public List<IntervalData> IntervalData { get; private set; }
        ICDIStrategy _cdiStrategy;

        /// <summary>
        /// Create from subset of all tickets (faster than using linq to query)
        /// </summary>
        /// <param name="analysisInterval">date range and intervals to analyze</param>
        /// <param name="allTickets">complete set for date range, all organizations</param>
        /// <param name="startIndex">start index into all tickets for this organization</param>
        /// <param name="endIndex">end index into all tickets for this organization</param>
        public Organization(DateRange analysisInterval, TicketJoin[] allTickets, int startIndex, int endIndex)
        {
            try
            {
                _dateRange = analysisInterval;

                // pull out the range for this organization
                _tickets = new TicketJoin[endIndex - startIndex];
                Array.Copy(allTickets, startIndex, _tickets, 0, _tickets.Length);
                OrganizationID = _tickets[0].OrganizationID;

                // collect metrics for each interval
                _intervalStrategy = new IntervalStrategy(_tickets);
                IntervalData = _intervalStrategy.GenerateIntervalData(_dateRange);

                // calculate the CDI using normalized data
                _cdiStrategy = new CDIPercentileStrategy(IntervalData);
                foreach (IntervalData intervalData in IntervalData)
                    _cdiStrategy.CalculateCDI(intervalData);
            }
            catch(Exception ex)
            {
                CDIEventLog.WriteEntry("New organization failed", ex);
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1}({2})", OrganizationID, IntervalData.Count, _tickets.Length);
        }
    }
}
