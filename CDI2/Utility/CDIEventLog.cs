using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TeamSupport.CDI
{
    /// <summary>
    /// Send detailed logging to TeamSupport custom log
    /// Inheriting from TextWriter allows it to be used as DataContext.Log = CDIEventLog
    /// </summary>
    class CDIEventLog : System.IO.TextWriter
    {
        public static CDIEventLog Instance = new CDIEventLog();

        public EventLog _eventLog;

        public override void Write(char[] buffer, int index, int count)
        {
            Write(new String(buffer, index, count));
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        CDIEventLog()
        {
            try
            {
                // To uninstall this log, run these commands in PowerShell and then reboot 
                //  Remove -EventLog -Source "WatsonToneAnalyzerService"
                //  Remove-EventLog -LogName "TeamSupport"
                const string logName = "TeamSupport";
                const string source = "CDI-2";

                _eventLog = new EventLog(logName);
                _eventLog.Source = source;
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, logName);
            }
            catch(Exception)
            {
                EventLog.WriteEntry("Application", "Unable to open TeamSupport log on CDI-2 source");
            }
        }
        bool _IsDebuggerAttached = Debugger.IsAttached;

        public override void WriteLine(string message) { WriteEntry(message); }

        public override void Write(string message)
        {
            Console.Write(message);
            if (_IsDebuggerAttached)
                Debug.Write(message);
            else
                _eventLog.WriteEntry(message, EventLogEntryType.Information);
        }

        public void WriteEntry(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            Console.WriteLine(message);
            if (_IsDebuggerAttached)
                Debug.WriteLine(message);
            else
                _eventLog.WriteEntry(message, type);
        }

        public void WriteEntry(string message, Exception e)
        {
            if (_IsDebuggerAttached)
                Debugger.Break();
            WriteEntry(message + e.ToString() + " ----- STACK: " + e.StackTrace.ToString(), EventLogEntryType.Error);
        }
    }
}
