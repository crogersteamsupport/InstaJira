using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Use percentiles of the interval distribution
    /// </summary>
    public class MetricPercentiles
    {
        Dictionary<EMetrics, Percentile> _percentiles;

        public MetricPercentiles(List<Metrics> intervals)
        {
            _percentiles = new Dictionary<EMetrics, Percentile>();

            _percentiles[EMetrics.New30] = new Percentile(intervals, x => x._newCount);
            _percentiles[EMetrics.Open] = new Percentile(intervals, x => x._openCount);
            _percentiles[EMetrics.DaysOpen] = new Percentile(intervals, x => x._medianDaysOpen);
            _percentiles[EMetrics.TotalTickets] = new Percentile(intervals, x => x._totalTicketsCreated);
            _percentiles[EMetrics.Closed30] = new Percentile(intervals, x => x._closedCount);

            List<Metrics> tmp = intervals.Where(m => m._medianDaysToClose.HasValue).ToList();
            _percentiles[EMetrics.DaysToClose] = new Percentile(tmp, t => t._medianDaysToClose.Value);

            tmp = intervals.Where(t => t._averageActionCount.HasValue).ToList();
            _percentiles[EMetrics.ActionCountClosed] = new Percentile(tmp, x => x._averageActionCount.Value);

            tmp = intervals.Where(t => t._averageSeverity.HasValue).ToList();
            _percentiles[EMetrics.SeverityClosed] = new Percentile(tmp, x => x._averageSeverity.Value);

            tmp = intervals.Where(t => t._averageSentimentScore.HasValue).ToList();
            _percentiles[EMetrics.SentimentClosed] = new Percentile(tmp, x => x._averageSentimentScore.Value);
        }

        public Metrics Normalize(Metrics interval)
        {
            Metrics normalized = new Metrics()
            {
                _timeStamp = interval._timeStamp,
                _newCount = _percentiles[EMetrics.New30].AsPercentile(interval._newCount),  // 2. Tickets Created in Last 30
                _openCount = _percentiles[EMetrics.Open].AsPercentile(interval._openCount),   // 3. Number of Tickets Currently Open
                _medianDaysOpen = _percentiles[EMetrics.DaysOpen].AsPercentile(interval._medianDaysOpen),    // 4. Average Time Tickets have been open
                _totalTicketsCreated = _percentiles[EMetrics.TotalTickets].AsPercentile(interval._totalTicketsCreated),  // 1. Total Tickets Created
                _closedCount = _percentiles[EMetrics.Closed30].AsPercentile(interval._closedCount),
            };

            if ((_percentiles[EMetrics.DaysToClose] != null) && interval._medianDaysToClose.HasValue)
                normalized._medianDaysToClose = _percentiles[EMetrics.DaysToClose].AsPercentile(interval._medianDaysToClose.Value);  // 5. Average Time tickets took to close

            if ((_percentiles[EMetrics.ActionCountClosed] != null) && interval._averageActionCount.HasValue)
                normalized._averageActionCount = _percentiles[EMetrics.ActionCountClosed].AsPercentile(interval._averageActionCount.Value);

            if ((_percentiles[EMetrics.SeverityClosed] != null) && interval._averageSeverity.HasValue)
                normalized._averageSeverity = _percentiles[EMetrics.SeverityClosed].AsPercentile(interval._averageSeverity.Value);

            if (interval._averageSentimentScore.HasValue)
                normalized._averageSentimentScore = interval._averageSentimentScore.Value / 10; // [0, 1000] => [0, 100]

            return normalized;
        }

    }

}
