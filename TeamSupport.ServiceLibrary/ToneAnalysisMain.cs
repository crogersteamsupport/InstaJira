using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WatsonToneAnalyzer
{
    public static class Program
    {

        static void Main(string[] args)
        {
            if (args.Contains("RebuildTicketSentimentsTable"))
            {
                WatsonEventLog.WriteEntry("RebuildTicketSentimentsTable started...");
                RebuildTicketSentimentsTable ticketSentiments = new RebuildTicketSentimentsTable(); // we can recreate TicketSentiments from ActionSentiments
                ticketSentiments.DoRebuild();
                WatsonEventLog.WriteEntry("RebuildTicketSentimentsTable completed.");
                return;
            }

            using (var service = new WatsonToneAnalyzerService())
            {
                if (!Environment.UserInteractive)
                {
                    ServiceBase.Run(service);   // running as a service
                }
                else
                {
                    // run from console
                    service.StartTimer();

                    Console.WriteLine("Press any key to stop...");
                    Console.ReadKey(true);

                    service.StopTimer();
                }
            }
        }
    }
}
