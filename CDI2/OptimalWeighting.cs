using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    class OptimalWeighting
    {
        Customer _customer;
        HashSet<Client> _clients;
        double?[][] _normalizedIntervals;
        double[] _cdi;
        MetricPercentiles _percentiles;
        linq.CDI_Settings _weights;

        int MetricCount = Enum.GetValues(typeof(EMetrics)).Length;

        public OptimalWeighting(Customer customer)
        {
            _customer = customer;
            _clients = customer._clients;
            _normalizedIntervals = new double?[MetricCount][];
            for (int i = 0; i < MetricCount; ++i)
                _normalizedIntervals[i] = new double?[_clients.Count];
            _cdi = new double[_clients.Count];
        }

        public void CalculatePercentiles()
        {
            List<Metrics> clientIntervals = new List<Metrics>();
            int i = 0;
            foreach (Client client in _clients)
            {
                Metrics interval = client._iCdiStrategy.RawMetrics;
                if (interval == null)
                    continue;

                Insert(i, interval);
                if (++i >= _clients.Count)
                    break;

                clientIntervals.Add(interval);
            }

            _percentiles = new MetricPercentiles(clientIntervals);
        }

        void Insert(int i, Metrics interval)
        {
            // note that casting metric to (int) would incorrectly result in the hex values assigned in the enum
            int m = 0;
            foreach (EMetrics metric in Enum.GetValues(typeof(EMetrics)))
                _normalizedIntervals[m++][i] = interval.GetAsCDIPercentile(metric);
        }

        static bool _writeHeader = true;
        public void FindOptimalMix()
        {
            int maxCombination = 0;
            double maxCorrelation = 0;
            double[] maxRSquared = null;

            int MaxMask = ((int)EMetrics.SentimentClosed) << 1;
            for (int mask = 1; mask < MaxMask; ++mask)
            {
                _weights = new linq.CDI_Settings();
                _weights.SetEqualWeights(mask);

                int i = 0;
                foreach (Client client in _clients)
                {
                    client.InvokeCDIStrategy(_percentiles, _weights);
                    _cdi[i++] = client._iCdiStrategy.CDI.Value;
                }

                double[] rSquared = new double[MetricCount];  // magic n
                for (int m = 0; m < MetricCount; ++m)
                    rSquared[m] = Statistics.RSquared(_normalizedIntervals[m], _cdi);

                double sumAbsCorrelation = rSquared.Sum();
                if (sumAbsCorrelation > maxCorrelation)
                {
                    maxCombination = mask;
                    maxCorrelation = sumAbsCorrelation;
                    maxRSquared = rSquared;
                }
                //CDIEventLog.Instance.WriteLine("{0}\t{1}", mask, sumAbsCorrelation);
            }

            StringBuilder builder = new StringBuilder();
            if (maxRSquared != null)
            {
                foreach (double value in maxRSquared)
                    builder.AppendFormat("\t{0:0.00}", value);
            }

            if (_writeHeader)
            {
                CDIEventLog.Instance.WriteLine("OrganizationID\tMetrics\tSumR^2\tNew30\tOpen\tDaysOpen\tTotalTickets\tClosed30\tDaysToClose\tActionCount\tSeverity\tSentiment");
                _writeHeader = false;
            }
            char[] charArray = Convert.ToString(maxCombination, 2).ToCharArray();   // convert to base 2 and reverse the bits
            Array.Reverse(charArray);
            CDIEventLog.Instance.WriteLine("{0}\t{1}\t{2:0.00}{3}", _customer.OrganizationID, new string(charArray), maxCorrelation, builder.ToString());
        }
    }
}
