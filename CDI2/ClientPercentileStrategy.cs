using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    class ClientPercentileStrategy : ICDIStrategy
    {
        List<IntervalData> _intervals;
        public CustomerPercentileStrategy _parentStrategy { get; set; }

        public ClientPercentileStrategy(List<IntervalData> intervals, ICDIStrategy iCdiStrategy)
        {
            _intervals = intervals;
            _parentStrategy = iCdiStrategy as CustomerPercentileStrategy;
        }

        public bool CalculateCDI()
        {
            DateTime timestamp = _parentStrategy.IntervalTimestamp;
            IntervalData interval = _intervals.Where(i => i._timeStamp == timestamp).FirstOrDefault();
            return (interval != null) ? _parentStrategy.CalculateCDI(interval) : false;
        }

    }
}
