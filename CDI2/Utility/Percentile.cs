﻿using System;
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
    class Percentile //where T : IComparable<T>
    {
        int _intervalCount;
        SortedDictionary<double, int> _counts;
        SortedDictionary<double, int> _percentiles;

        public Percentile(List<IntervalData> intervals, Func<IntervalData, double> getFunc)
        {
            _intervalCount = intervals.Count();
            intervals.Sort((lhs, rhs) => getFunc(lhs).CompareTo(getFunc(rhs)));
            _counts = new SortedDictionary<double, int>();

            foreach(IntervalData interval in intervals)
            {
                double value = getFunc(interval);
                if (_counts.ContainsKey(value))
                    ++_counts[value];
                else
                    _counts[value] = 1;
            }

            _percentiles = new SortedDictionary<double, int>();
            int index = 1;
            foreach (KeyValuePair<double, int> pair in _counts)
            {
                double value = (index - 0.5) / _intervalCount;
                value = value * value * value * value;
                _percentiles[pair.Key] = (int)Math.Round(100 * value);
                index += pair.Value;
            }

            //CDIEventLog.WriteLine("Value\tCount\tPercentile");
            //foreach (KeyValuePair<double, int> pair in _counts)
            //    CDIEventLog.WriteLine(String.Format("{0}\t{1}\t{2}", pair.Key, pair.Value, _percentiles[pair.Key]));
        }

        /// <summary>
        /// https://web.stanford.edu/class/archive/anthsci/anthsci192/anthsci192.1064/handouts/calculating%20percentiles.pdf
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
