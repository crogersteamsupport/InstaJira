using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    /// <summary>calculate the CDI using a rolling percentile lookup</summary>
    class CustomerPercentileStrategy : ICDIStrategy
    {
        List<IntervalData> _intervals;
        public IntervalPercentiles _cdiPercentile { get; private set; }
        public DateTime IntervalTimestamp { get; private set; }
        Action _callback;

        public CustomerPercentileStrategy(List<IntervalData> intervals, Action callback)
        {
            _intervals = intervals;
            _callback = callback;
        }

        /// <summary>calculate the CDI using a rolling percentile lookup</summary>
        public bool CalculateCDI()
        {
            //CDIEventLog.WriteLine("Date\tNew\tOpen\tClosed\tDaysOpen\tDaysToClose\tActions\tSentiment\tCDI");
            _intervals.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
            DateTime begin = _intervals.First()._timeStamp;
            DateTime end = begin.AddDays(365);  // 1 year rolling average

            // get a rolling year into the queue
            List<IntervalData> rollingYear = _intervals.Where(t => t._timeStamp < end).ToList();
            _cdiPercentile = new IntervalPercentiles(rollingYear);  // mixes up rollingYear...
            foreach (IntervalData interval in _intervals)
            {
                // rolling year for percentile
                bool update = false;
                rollingYear.Sort((lhs, rhs) => lhs._timeStamp.CompareTo(rhs._timeStamp));
                if (interval._timeStamp > rollingYear.Last()._timeStamp)
                {
                    rollingYear.Add(interval);  // add future data
                    update = true;
                }
                while (rollingYear.First()._timeStamp < interval._timeStamp.AddDays(-365))
                {
                    rollingYear.RemoveAt(0);    // drop old data
                    update = true;
                }

                if (update)
                    _cdiPercentile = new IntervalPercentiles(rollingYear);  // updated percentile strategy

                //_cdiPercentile.CDI1(interval);  // TODO - remove, calculate CDI on Customer

                IntervalTimestamp = interval._timeStamp;
                _callback();

                // do the same for all clients with this intervalData
            }
            return true;
        }

        /// <summary>calculate the CDI using a rolling percentile lookup</summary>
        public bool CalculateCDI(IntervalData interval)
        {
            IntervalData normalized;
            return _cdiPercentile.CDI1(interval, out normalized);
        }

    }
}
