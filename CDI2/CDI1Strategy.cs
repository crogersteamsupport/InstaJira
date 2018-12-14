using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Linq;
using TeamSupport.CDI.linq;
using System.Configuration;

namespace TeamSupport.CDI
{
    class CDI1Strategy : ICDIStrategy
    {
        OrganizationAnalysis _organizationAnalysis;
        Metrics _rawMetrics;
        Metrics _normalizedMetrics;

        public CDI1Strategy(OrganizationAnalysis organizationAnalysis)
        {
            _organizationAnalysis = organizationAnalysis;
        }

        public Metrics RawMetrics { get { return _rawMetrics; } }
        public Metrics NormalizedMetrics { get { return _normalizedMetrics; } }
        public int? CDI { get { return RawMetrics.CDI; } }  // note that _rawMetrics.CDI == _normalizedMetrics.CDI

        public Metrics CalculateRawMetrics()
        {
            Metrics end = _organizationAnalysis.Current();
            if (end == null)
            {
                Metrics last = _organizationAnalysis.Intervals.Last();
                _rawMetrics = new Metrics()
                {
                    _timeStamp = _organizationAnalysis._dateRange.EndDate,
                    _totalTicketsCreated = last._totalTicketsCreated,
                    _newCount = 0,
                    _openCount = last._openCount,
                    _closedCount = 0,
                    _medianDaysOpen = _organizationAnalysis.MedianDaysOpen(),
                    _averageSeverity = last._averageSeverity,
                    _averageActionCount = last._averageActionCount,
                    _averageSentimentScore = last._averageSentimentScore
                };
            }
            else
            {
                _rawMetrics = new Metrics()
                {
                    _timeStamp = end._timeStamp,
                    _totalTicketsCreated = end._totalTicketsCreated, // 1. TotalTicketsCreated
                    _newCount = end._newCount, // 2. CreatedLast30
                    _openCount = end._openCount,   // 3. TicketsOpen
                    _closedCount = end._closedCount,
                    _medianDaysOpen = end._medianDaysOpen, // 4. AvgTimeOpen
                    _averageSeverity = end._averageSeverity,
                    _averageActionCount = end._averageActionCount,
                    _averageSentimentScore = end._averageSentimentScore
                };
            }

            _rawMetrics._medianDaysToClose = Metrics.MedianTotalDaysToClose(_organizationAnalysis.Tickets);  // 5. AvgTimeToClose
            return _rawMetrics;
        }

        /// <summary> CDI1 </summary>
        public void InvokeCDIStrategy(MetricPercentiles clientPercentiles, linq.CDI_Settings weights)
        {
            _normalizedMetrics = clientPercentiles.Normalize(_rawMetrics);

            double result = 0;
            foreach (EMetrics metric in Enum.GetValues(typeof(EMetrics)))
            {
                double? value = _normalizedMetrics.GetAsCDIPercentile(metric);
                double? weight = weights.GetWeight(metric);
                if (value.HasValue && weight.HasValue)
                    result += value.Value * weight.Value;
            }

            // square it to get a better distribution?
            _normalizedMetrics.CDI = _rawMetrics.CDI = (int)Math.Round(result);
        }

        public string ToStringCDI1()
        {
            //return _organizationAnalysis.ToString();
            if (_rawMetrics == null)
                return string.Empty;
            return _rawMetrics.ToStringCDI1();
        }

        /// <summary>
        /// The linq to sql update compares object tracking as well as uses parameterization to protect
        /// against sql-injection attackes. Thus use a hard-coded update here...
        /// </summary>
        /// <param name="db"></param>
        public void Save(DataContext db, Table<OrganizationSentiment> table)
        {
            // dbo.Organizations
            db.ExecuteCommand(linq.Organization.RawUpdateQuery(_rawMetrics, _organizationAnalysis.OrganizationID));
            SaveOrganizationSentiment(db, table);
        }

        enum DBChangeType
        {
            NONE,
            DELETE,
            UPDATE,
            INSERT,
        };

        DBChangeType GetQueryType(double? score, out OrganizationSentiment organizationSentiment)
        {
            if (OrganizationSentiment.TryPop(_organizationAnalysis.OrganizationID, out organizationSentiment))
            {
                if (!score.HasValue)
                    return DBChangeType.DELETE;
                else if (Math.Floor(score.Value) != Math.Floor(organizationSentiment.OrganizationSentimentScore))
                    return DBChangeType.UPDATE;
            }
            else if (score.HasValue)
                return DBChangeType.INSERT;

            return DBChangeType.NONE;
        }

        static int OrganizationSentimentMonths = Int32.Parse(ConfigurationManager.AppSettings.Get("OrganizationSentimentMonths"));
        void SaveOrganizationSentiment(DataContext db, Table<OrganizationSentiment> table)
        {
            // Ticket Sentiments by month
            List<Metrics> metrics = _organizationAnalysis.Intervals;    // each instance of Metrics = 1 month of data
            if (metrics.Count > OrganizationSentimentMonths)
                metrics = _organizationAnalysis.Intervals.GetRange(_organizationAnalysis.Intervals.Count - OrganizationSentimentMonths, OrganizationSentimentMonths);
            metrics = metrics.Where(m => m._averageSentimentScore.HasValue).ToList();

            // average score
            double? score = null;
            int ticketCount = metrics.Sum(m => m._closedCount);
            if ((metrics.Count > 0) && (ticketCount > 0))
                score = metrics.Sum(m => m._averageSentimentScore * m._closedCount) / ticketCount; ;    // ticket sentiment based on closed tickets

            // save to dbo
            OrganizationSentiment organizationSentiment;
            DBChangeType queryType = GetQueryType(score, out organizationSentiment);
            switch (queryType)
            {
                case DBChangeType.DELETE:   // DELETE
                    db.ExecuteCommand(String.Format(@"DELETE FROM [OrganizationSentiments] WHERE [OrganizationID] = {0}", _organizationAnalysis.OrganizationID));
                    break;
                case DBChangeType.UPDATE:   // UPDATE
                    db.ExecuteCommand(String.Format(@"UPDATE OrganizationSentiments SET OrganizationSentimentScore = {0}, TicketSentimentCount = {1} WHERE IsAgent=0 AND OrganizationID = {2}",
                        score.Value,
                        ticketCount,
                        _organizationAnalysis.OrganizationID));
                    break;
                case DBChangeType.INSERT:   // INSERT
                    table.InsertOnSubmit(new OrganizationSentiment()
                    {
                        OrganizationID = _organizationAnalysis.OrganizationID,
                        IsAgent = false,
                        OrganizationSentimentScore = score.Value,
                        TicketSentimentCount = ticketCount
                    });
                    break;
                default:    // NONE
                    break;
            }
        }

        public void Save(Table<linq.CustDistHistory> table)
        {
            try
            {
                linq.CustDistHistory history = new linq.CustDistHistory()
                {
                    ParentOrganizationID = _organizationAnalysis.ParentID,
                    OrganizationID = _organizationAnalysis.OrganizationID,
                    CDIDate = DateRange.EndTimeNow,
                    CDIValue = _rawMetrics.CDI.Value
                };
                table.InsertOnSubmit(history);
            }
            catch (Exception e)
            {
                CDIEventLog.Instance.WriteEntry("Save failed", e);
            }
        }
    }
}
