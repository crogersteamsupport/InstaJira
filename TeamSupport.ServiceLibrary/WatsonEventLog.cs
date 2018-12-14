using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WatsonToneAnalyzer
{
    /// <summary>
    /// Send detailed logging to TeamSupport custom log
    /// </summary>
    class WatsonEventLog
    {
        public static EventLog _eventLog;

        /// <summary>
        /// Static constructor
        /// </summary>
        static WatsonEventLog()
        {
            try
            {
                // To uninstall this log, run these commands in PowerShell and then reboot 
                //  Remove -EventLog -Source "WatsonToneAnalyzerService"
                //  Remove-EventLog -LogName "TeamSupport"
                const string logName = "TeamSupport";
                const string source = "WatsonToneAnalyzerService";

                _eventLog = new EventLog(logName);
                _eventLog.Source = source;
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, logName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                EventLog.WriteEntry("Application", "Unable to open TeamSupport log on WatsonToneAnalyzerService source");
            }
        }

        static bool _IsDebuggerAttached = Debugger.IsAttached;

        public static void WriteEntry(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            Console.WriteLine(message);
            if (_IsDebuggerAttached)
            {
                Debug.WriteLine(message);
                if (type == EventLogEntryType.Error)
                    Debugger.Break();
            }
            else
                _eventLog.WriteEntry(message, type);
        }

        public static void WriteEntry(string message, Exception e)
        {
            WriteEntry(message + e.ToString() + " ----- STACK: " + e.StackTrace.ToString(), EventLogEntryType.Error);
        }
    }
}
