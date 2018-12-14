using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI.linq
{
    /// <summary> 
    /// [dbo].[CDI_Settings] 
    /// Weights for CDI calculation
    /// </summary>
    [Table(Name = "CDI_Settings")]
    public class CDI_Settings
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        int _organizationID;
        [Column(Storage = "_organizationID", DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int OrganizationID { get { return _organizationID; } }

        [Column]
        public float? TotalTicketsWeight;
        [Column]
        public float? OpenTicketsWeight;
        [Column]
        public float? Last30Weight;
        [Column]
        public float? AvgDaysOpenWeight;
        [Column]
        public float? AvgDaysToCloseWeight;
        [Column]
        public DateTime? LastCompute;
        [Column]
        public bool? NeedCompute;

        // new CDI2
        [Column]
        public float? ClosedLast30Weight;
        [Column]
        public float? AverageActionCountWeight;
        [Column]
        public float? AverageSentimentScoreWeight;
        [Column]
        public float? AverageSeverityWeight;

#pragma warning restore CS0649

        public double? GetWeight(EMetrics metric)
        {
            double? result = null;
            switch (metric)
            {
                case EMetrics.ActionCountClosed:
                    result = AverageActionCountWeight;
                    break;
                case EMetrics.Closed30:
                    result = ClosedLast30Weight;
                    break;
                case EMetrics.DaysOpen:
                    result = AvgDaysOpenWeight;
                    break;
                case EMetrics.DaysToClose:
                    result = AvgDaysToCloseWeight;
                    break;
                case EMetrics.New30:
                    result = Last30Weight;
                    break;
                case EMetrics.Open:
                    result = OpenTicketsWeight;
                    break;
                case EMetrics.SentimentClosed:
                    result = AverageSentimentScoreWeight;
                    break;
                case EMetrics.SeverityClosed:
                    result = AverageSeverityWeight;
                    break;
                case EMetrics.TotalTickets:
                    result = TotalTicketsWeight;
                    break;
            }
            return result;
        }

        public void SetWeight(EMetrics metric, float? value)
        {
            switch (metric)
            {
                case EMetrics.ActionCountClosed:
                    AverageActionCountWeight = value;
                    break;
                case EMetrics.Closed30:
                    ClosedLast30Weight = value;
                    break;
                case EMetrics.DaysOpen:
                    AvgDaysOpenWeight = value;
                    break;
                case EMetrics.DaysToClose:
                    AvgDaysToCloseWeight = value;
                    break;
                case EMetrics.New30:
                    Last30Weight = value;
                    break;
                case EMetrics.Open:
                    OpenTicketsWeight = value;
                    break;
                case EMetrics.SentimentClosed:
                    AverageSentimentScoreWeight = value;
                    break;
                case EMetrics.SeverityClosed:
                    AverageSeverityWeight = value;
                    break;
                case EMetrics.TotalTickets:
                    TotalTicketsWeight = value;
                    break;
            }
        }

        public void SetEqualWeights(int mask)
        {
            float scalar = 0;
            foreach (EMetrics metric in Enum.GetValues(typeof(EMetrics)))
            {
                if (((int)metric & mask) != 0)
                    ++scalar;
            }
            scalar = 1 / scalar;

            foreach (EMetrics metric in Enum.GetValues(typeof(EMetrics)))
            {
                if (((int)metric & mask) != 0)
                    SetWeight(metric, scalar);
            }
        }

    }
}
