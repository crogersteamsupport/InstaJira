using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.IO;


namespace TeamSupport.TimeSync
{
  class Program : ServiceBase
  {
    Thread _thread = null;

    public Program()
    {
      this.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
    }

    private void TestConnection()
    {
      string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        try
        {
          connection.Open();
          connection.Close();
        }
        catch (Exception ex)
        {
          WriteEvent(connectionString + " " + ex.Message);
          EventLog.WriteEntry(ServiceName, connectionString + " " + ex.Message);
          throw new Exception(connectionString + " " + ex.Message, ex);
        }
      }
    }

    static void Main(string[] args)
    {
      ServiceBase.Run(new Program());
    }

    protected override void OnStart(string[] args)
    {
      WriteEvent("[" + DateTime.Now.ToString() + "] Started.");
      if (!EventLog.SourceExists(ServiceName)) EventLog.CreateEventSource(ServiceName, "Start");
      TestConnection();
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;
      
      _thread = new Thread(new ThreadStart(Run));
      _thread.Start();

    }

    protected override void OnStop()
    {
      StopProcessing();
    }

    protected override void OnShutdown()
    {
      StopProcessing();
    }

    private void StopProcessing()
    {
      WriteEvent("["+DateTime.Now.ToString()+"] Stopped.");
      _thread.Abort();
      _thread.Join();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
      public ushort wYear;
      public ushort wMonth;
      public ushort wDayOfWeek;
      public ushort wDay;
      public ushort wHour;
      public ushort wMinute;
      public ushort wSecond;
      public ushort wMilliseconds;
    }

    //http://www.pcreview.co.uk/forums/setting-date-and-time-w-c-t1300916.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetSystemTime(ref SystemTime theDateTime);

    private void Run()
    {
      Stopwatch watch = new Stopwatch();
      watch.Start();
      while (true)
      {
        try
        {
          int interval = int.Parse(ConfigurationManager.AppSettings["UpdateIntervalMS"]);
          int variance = int.Parse(ConfigurationManager.AppSettings["MaxVarianceMS"]);
          Thread.Sleep(1000);
          
          if (watch.ElapsedMilliseconds > interval)
          {
            watch.Reset();
            watch.Start();

            try
            {
              using (SqlConnection connection = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
              {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT GETUTCDATE()", connection);
                DateTime sqlTime = (DateTime)command.ExecuteScalar();
                DateTime sysTime = DateTime.UtcNow;
                connection.Close();

                string message = "SQL Time: {0}  System Time: {1}  Variance: {2:0}  Message: {3}";
                double offset = Math.Abs((sysTime - sqlTime).TotalMilliseconds);
                
                if (offset > variance)
                {
                  SystemTime time = new SystemTime();

                  time.wYear = (ushort)sqlTime.Year;
                  time.wMonth = (ushort)sqlTime.Month;
                  time.wDay = (ushort)sqlTime.Day;
                  time.wDayOfWeek = (ushort)sqlTime.DayOfWeek;
                  time.wHour = (ushort)sqlTime.Hour;
                  time.wMinute = (ushort)sqlTime.Minute;
                  time.wSecond = (ushort)sqlTime.Second;
                  time.wMilliseconds = (ushort)sqlTime.Millisecond;

                  if (SetSystemTime(ref time) != true)
                  {
                    WriteEvent(string.Format(message, sqlTime.ToString(), sysTime.ToString(), offset,  "Error setting system time: " + Marshal.GetLastWin32Error().ToString()));
                  }
                  else
                  {
                    WriteEvent(string.Format(message, sqlTime.ToString(), sysTime.ToString(), offset, "System time was updated."));
                  }
                }
                else
                {
                  //WriteEvent(string.Format(message, sqlTime.ToString(), sysTime.ToString(), offset, "No Change."));

                }
              }
            }
            catch (Exception ex)
            {
              EventLog.WriteEntry(ServiceName, ex.Message + "  Stack: " + ex.StackTrace);
            }

          }
        }
        catch (Exception ex)
        {
          EventLog.WriteEntry(ServiceName, ex.Message + "  Stack: " + ex.StackTrace);
        }
      }
    }

    private static void WriteEvent(string message)
    {
      string path = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, 
          string.Format("TeamSupport.TimeSync {0}-{1}-{2}.log", DateTime.Now.Year.ToString(),DateTime.Now.Month.ToString(),DateTime.Now.Day.ToString()));
      if (!File.Exists(path))
      {
        using (StreamWriter writer = File.CreateText(path))
        {
          writer.WriteLine(message);
        }
      }
      else
	    {
        using (StreamWriter writer = File.AppendText(path))
        {
          writer.WriteLine(message);
        }
	    }
    }

  }
}
