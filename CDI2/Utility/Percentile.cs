using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Collect percentiles to determine where the current value lies in the distribution
    /// </summary>
    public class Percentile //where T : IComparable<T>
    {
        int _intervalCount;
        SortedDictionary<double, int> _counts;
        SortedDictionary<double, int> _percentiles;

        public Percentile(List<Metrics> intervals, Func<Metrics, double> getFunc)
        {
            _intervalCount = intervals.Count();
            intervals.Sort((lhs, rhs) => getFunc(lhs).CompareTo(getFunc(rhs)));
            _counts = new SortedDictionary<double, int>();

            foreach(Metrics interval in intervals)
            {
                double value = getFunc(interval);
                if (_counts.ContainsKey(value))
                    ++_counts[value];
                else
                    _counts[value] = 1;
            }

            _percentiles = new SortedDictionary<double, int>();
            int index = 0;
            foreach (KeyValuePair<double, int> pair in _counts)
            {
                double offset = (double)pair.Value / 2;
                double value = (offset + index) / (double)_intervalCount;
                value = value * value * value * value;  // intentionally skews the results to more closely match CDI1
                _percentiles[pair.Key] = (int)Math.Round(100 * value);
                index += pair.Value;
            }
        }

        /// <summary>
        /// AsPercentile
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int AsPercentile(double value)
        {
            if (_percentiles.ContainsKey(value))
                return _percentiles[value];

            // find closest...
            foreach (KeyValuePair<double, int> pair in _percentiles)
            {
                if (value <= pair.Key)  // use the lower bound of percentile (TODO - invert negative correlated metrics!)
                    return pair.Value;
            }
            return 99;
        }
    }
}
