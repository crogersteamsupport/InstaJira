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
        public static float Milliseconds(long ticks) { return (float)ticks / 10000f; }
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

        public float Milliseconds { get { return Ticks.Milliseconds(_sum); } }
        public override string ToString() { return $"{_count}, {Ticks.Milliseconds(_sum)}"; }
    }

    /// <summary> 
    /// Time single method execution 
    /// Don't let this class do a throw
    /// </summary>
    public class ScopedElapsedTime : IDisposable
    {
        // static data
        private static bool _enable = true; // global enable/disable of performance tracking
        public static Dictionary<string, MethodTime> MethodTimes { get; private set; }
        static System.Timers.Timer _timer;
        static int _timeoutExpired;

        // static initialization
        static ScopedElapsedTime()
        {
            // interval to register metrics
            string timeoutString = System.Configuration.ConfigurationManager.AppSettings["ScopedElapsedTime"];
            if (!int.TryParse(timeoutString, out int timeout))
                timeout = 30;
            _timer = new System.Timers.Timer(timeout);    // record results every minute
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
            _timeoutExpired = 0;

            MethodTimes = new Dictionary<string, MethodTime>(); // data collection
        }
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            _timeoutExpired = 1;
        }

        public static ScopedElapsedTime Trace([System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            try
            {
                return _enable ? new ScopedElapsedTime(sourceFilePath, callerMemberName) : null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // implementation
        string _fileMethod;
        long _start;

        private ScopedElapsedTime(string sourceFilePath, string callerMemberName)
        {
            _start = Ticks.TickCount;

            // keep name so when we get results it can be added 
            _fileMethod = $"{sourceFilePath}/{callerMemberName}";
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }


        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (!MethodTimes.TryGetValue(_fileMethod, out MethodTime methodTime))
                        methodTime = MethodTimes[_fileMethod] = new MethodTime();
                    methodTime.Add(Ticks.Elapsed(_start));
                }

                if (0 != Interlocked.Exchange(ref _timeoutExpired, 0))
                    DumpMetrics();
            }
            catch (Exception ex)
            {

            }
        }

        public static void DumpMetrics()
        {
            _timer.Stop();  // prevent overlapping writes
            try
            {
                // not thread safe but close enough
                Dictionary<string, MethodTime> next = new Dictionary<string, MethodTime>();
                Dictionary<string, MethodTime> tmp = ScopedElapsedTime.MethodTimes;
                MethodTimes = next;

                foreach (KeyValuePair<string, MethodTime> pair in tmp)
                    NewRelic.Api.Agent.NewRelic.RecordMetric($"Custom/{pair.Key}", pair.Value.Milliseconds);

                //StringBuilder builder = new StringBuilder();
                //foreach (KeyValuePair<string, MethodTime> pair in tmp)
                //    builder.AppendLine($"{DateTime.UtcNow}, {pair.Key}, {pair.Value}");
                //Debug.WriteLine(builder.ToString());    // Chris Rogers - send to NewRelic !!!!!!!!!!!!!!!!!!!!

            }
            catch (Exception ex)
            {

            }
            finally
            {
                _timer.Start(); // resume
            }
        }

    }

}
