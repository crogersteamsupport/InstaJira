using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace TeamSupport.UnitTest
{
    /// <summary> Win32 accessor to get thread time (kernal + user time) </summary>
    public class Ticks
    {
        [DllImport("kernel32.dll")]
        private static extern long GetThreadTimes(IntPtr threadHandle, out long creationTime, out long exitTime, out long kernelTime, out long userTime);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        /// <summary>time elapsed since midnight on January 1, 1601 UTC</summary>
        public static long TickCount
        {
            get
            {
                long retcode = GetThreadTimes(GetCurrentThread(), out long unused, out unused, out long kernelTime, out long userTime);
                return Convert.ToBoolean(retcode) ? kernelTime + userTime : 0;
            }
        }

        /// <summary>100-nanosecond units</summary>
        public static double Milliseconds(long ticks) { return (double)ticks / 10000f; }
        public static long Elapsed(long start) { return TickCount - start; }
    }

    /// <summary> Accumulator for thread times on single method </summary>
    public class MethodTime
    {
        long _sum;
        int _count;
        public MethodTime() { _sum = _count = 0; }

        public void Add(long value)
        {
            Interlocked.Add(ref _sum, value);
            Interlocked.Increment(ref _count);
        }
        public override string ToString() { return $"{_count}, {Ticks.Milliseconds(_sum)}"; }
    }

    /// <summary> Time single method execution </summary>
    public class ScopedElapsedTime : IDisposable
    {
        // static data
        private static bool _enable = true; // global enable/disable of performance tracking
        public static Dictionary<string, MethodTime> MethodTimes { get; private set; }
        static System.Timers.Timer aTimer;

        // static initialization
        static ScopedElapsedTime()
        {
            string timeoutString = System.Configuration.ConfigurationManager.AppSettings["ScopedElapsedTime"];

            int timeout;   // default
            if(!int.TryParse(timeoutString, out timeout))
                timeout = 30;

            aTimer = new System.Timers.Timer(timeout);    // record results every minute
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Start();
            Reset();
        }
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            _dumpMetrics = 1;
        }
        public static ScopedElapsedTime Trace([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            return _enable ? new ScopedElapsedTime(sourceFilePath, callerMemberName) : null;
        }

        public static void Reset()
        {
            MethodTimes = new Dictionary<string, MethodTime>();
            _dumpMetrics = 0;
        }


        // implementation
        MethodTime _methodTime;
        long _start;

        private ScopedElapsedTime(string sourceFilePath, string callerMemberName)
        {
            _start = Ticks.TickCount;

            // get reference to callerMemberName MethodTimes to put the results
            string fileMethod = $"{sourceFilePath},{callerMemberName}";
            if (!MethodTimes.TryGetValue(fileMethod, out _methodTime))
                _methodTime = MethodTimes[fileMethod] = new MethodTime();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        static int _dumpMetrics;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _methodTime.Add(Ticks.Elapsed(_start));

            if (1 == Interlocked.Exchange(ref _dumpMetrics, 0))
                DumpMetrics();
        }

        public static void DumpMetrics()
        {
            aTimer.Stop();  // prevent overlapping writes

            // not thread safe but close enough
            Dictionary<string, MethodTime> next = new Dictionary<string, MethodTime>();
            Dictionary<string, MethodTime> tmp = ScopedElapsedTime.MethodTimes;
            MethodTimes = next;

            
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, MethodTime> pair in tmp)
                builder.AppendLine($"{DateTime.UtcNow}, {pair.Key}, {pair.Value}");

            Debug.WriteLine(builder.ToString());    // Chris Rogers - send to NewRelic !!!!!!!!!!!!!!!!!!!!

            aTimer.Start(); // resume
        }

    }

}
