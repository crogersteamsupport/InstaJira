using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;

namespace TeamSupport.CDI
{
    public enum TimeScale
    {
        Minutes,
        Hours,
        Days,
        Weeks
    }

    public class TicketJoin : IComparable<TicketJoin>
    {
        public int OrganizationID;
        public DateTime? DateClosed;
        public DateTime DateCreated;
        public int? ActionsCount;
        public bool IsClosed;
        public double? AverageActionSentiment;
        public int? ParentID;
        public int? Severity;
        //public int CreatorID;

        public int CompareTo(TicketJoin other) { return DateCreated.CompareTo(other.DateCreated); }

        public double? TotalDaysToClose
        {
            get
            {
                double? result = null;
                if (IsClosed && DateClosed.HasValue)
                    result = (DateClosed.Value - DateCreated).TotalDays;
                return result;
            }
        }
        public double TotalDaysOpenToTimestamp(DateTime timestamp)
        {
            DateTime endTime = (DateClosed.HasValue && (DateClosed.Value < timestamp)) ? DateClosed.Value : timestamp;
            TimeSpan span = endTime - DateCreated;
            return span.TotalDays;
        }

        public double ScaledTimeOpen(TimeScale scale)
        {
            if (!DateClosed.HasValue)
                return 0;

            TimeSpan timeSpan = DateClosed.Value - DateCreated;
            double result;
            switch (scale)
            {
                case TimeScale.Minutes:
                    result = timeSpan.TotalMinutes;
                    break;
                case TimeScale.Hours:
                    result = timeSpan.TotalHours;
                    break;
                case TimeScale.Days:
                    result = timeSpan.TotalDays;
                    break;
                case TimeScale.Weeks:
                    result = timeSpan.TotalDays / 7;
                    break;
                default:
                    result = Double.PositiveInfinity;
                    break;
            }

            return result;
        }
    }
}
