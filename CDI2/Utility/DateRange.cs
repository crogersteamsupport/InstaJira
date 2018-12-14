using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace TeamSupport.CDI
{
    /// <summary>
    /// What time scale do we want to use to analyze the data
    /// </summary>
    public class DateRange : IEnumerable<DateTime>
    {
        public static DateTime EndTimeNow = DateTime.UtcNow;

        /// <summary>Count of intervals to analyze</summary>
        public int IntervalCount { get; private set; }
        /// <summary>time span of a single interval</summary>
        public TimeSpan IntervalTimeSpan { get; private set; }
        /// <summary>Only analyze tickets created after start date</summary>
        public DateTime StartDate { get; private set; }
        /// <summary>Only analyze tickets before end date</summary>
        public DateTime EndDate { get; private set; }

        public DateRange(TimeSpan analysisInterval, int intervalCount)
        {
            IntervalCount = intervalCount;
            IntervalTimeSpan = analysisInterval;
            EndDate = TonightMidnight(EndTimeNow);
            StartDate = EndDate - TimeSpan.FromDays(analysisInterval.TotalDays * intervalCount);
        }

        /// <summary>Round to the previous midnight</summary>
        public DateTime TonightMidnight(DateTime value)
        {
            TimeSpan oneDay = new TimeSpan(24, 0, 0);
            return value.AddTicks(oneDay.Ticks - (value.Ticks % oneDay.Ticks));
        }

        public override string ToString()
        {
            return String.Format("[{0}, {1}]", StartDate.ToShortDateString(), EndDate.ToShortDateString());
        }

        public DateTime this[int index]
        {
            get
            {
                return StartDate.AddDays(IntervalTimeSpan.TotalDays * index);
            }
        }

        public IEnumerator<DateTime> GetEnumerator()
        {
            return new DateRangeEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DateRangeEnumerator(this);
        }
    }

    /// <summary>
    /// Enumerator for DateRange
    /// </summary>
    public class DateRangeEnumerator : IEnumerator<DateTime>
    {
        DateRange _collection;
        private int _curIndex;

        public DateRangeEnumerator(DateRange collection)
        {
            _collection = collection;
            _curIndex = -1;  // defaults to before start until first call to MoveNext
        }

        public bool MoveNext() { return ++_curIndex < _collection.IntervalCount; }
        public void Reset() { _curIndex = -1; }
        void IDisposable.Dispose() { }
        public DateTime Current { get { return _collection[_curIndex]; } }
        object IEnumerator.Current { get { return _collection[_curIndex]; } }
    }
}
