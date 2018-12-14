using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;

namespace TeamSupport.CDI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("AssignTicketSeverities"))
            {
                AssignTicketSeverities();
                return;
            }

            Stopwatch totalTimer = new Stopwatch();
            totalTimer.Start();
            int IntervalTimeSpanInDays = int.Parse(ConfigurationManager.AppSettings.Get("IntervalTimeSpanInDays"));
            int IntervalCount = int.Parse(ConfigurationManager.AppSettings.Get("IntervalCount"));
            CDI2Engine cdi = new CDI2Engine(TimeSpan.FromDays(IntervalTimeSpanInDays), IntervalCount);
            cdi.Run(args);
            if (!args.Contains("ForceCDIUpdate"))
                CDIEventLog.Instance.WriteLine(String.Format("CDI Total Execution Time {0:0.00} sec", totalTimer.ElapsedMilliseconds / 1000.0));
        }

        public static void AssignTicketSeverities()
        {
            try
            {
                linq.TicketSeverity.AssignTicketSeverities();
                CDIEventLog.Instance.WriteLine("TicketSeverities assigned");
            }
            catch (Exception ex)
            {
                CDIEventLog.Instance.WriteLine(ex.ToString());
                CDIEventLog.Instance.WriteLine("\nERROR: Did you run the following command on the database?");
                CDIEventLog.Instance.WriteLine(" \"ALTER TABLE dbo.TicketSeverities ADD Severity int NULL\"");
            }
        }
    }
}
