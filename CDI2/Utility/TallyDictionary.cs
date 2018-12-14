using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Utility class for histogram - counter by bin
    /// </summary>
    class TallyDictionary<T>
    {
        SortedDictionary<T, int> _sum = new SortedDictionary<T, int>();

        /// <summary> add an item to the bin </summary>
        /// <param name="binIndex">which bin does this record land in</param>
        public void Increment(T binIndex)
        {
            if (!_sum.ContainsKey(binIndex))
                _sum[binIndex] = 1;
            else
                _sum[binIndex]++;
        }

        /// <summary> remove an item from the bin </summary>
        /// <param name="binIndex">which bin does this record land in</param>
        public void Decrement(T binIndex)
        {
            if (!_sum.ContainsKey(binIndex))
                _sum[binIndex] = -1;
            else
                _sum[binIndex]--;
        }

        public void Write()
        {
            foreach (KeyValuePair<T, int> pair in _sum)
                CDIEventLog.WriteLine("{0}\t{1}", pair.Key, pair.Value);
        }
    }
}
