using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using TeamSupport.Data;
using TeamSupport.ServiceLibrary;
using TeamSupport.CrmIntegration;
using Microsoft.Win32;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;

namespace TeamSupport.Service
{
  class Program : ServiceBase
  {
    ServiceThread _thread;
 
    static void Main(string[] args)
    {
        if (!Environment.UserInteractive)
        {
            ServiceBase.Run(new Program());   // running as a service
        }
        else
        {
            // run from console
            Program service = new Program("TSCustomerInsights");
            service.OnStart(null);

            for (int i = 0; i < 1000; ++i)
                Thread.Sleep(1000);

            service.OnStop();
        }
    }

    public Program(string serviceName = null)  
    {
        if(serviceName == null)
                serviceName = ConfigurationManager.AppSettings["ServiceName"];
      this.ServiceName = serviceName;
    }

    private ServiceThread GetServiceThread()
    {
      ServiceThread result = null;
      switch (ServiceName)
      {
        case "TSEmailProcessor": result = new ServiceThreadPool<EmailProcessor>("EmailProcessor"); break;
        case "TSEmailSender": result = new ServiceThreadPool<EmailSender>("EmailSender"); break;
        case "TSSlaProcessor": result = new SlaProcessor(); break;
        case "TSSlaCalculator": result = new SlaCalculator(); break;
        case "TSIndexer": result = new ServiceThreadPool<Indexer>("Indexer"); break;
        case "TSIndexRebuilder": result = new ServiceThreadPool<Indexer>("Indexer"); break;
        case "TSCrmPool": result = new CrmPool(SystemUser.CRM); break;
        case "TSReminderProcessor": result = new ReminderProcessor(); break;
        case "TSImportProcessor": result = new ImportProcessor(); break;
        case "TSWebHooksProcessor": result = new WebHooksPool(SystemUser.CRM); break;
        case "TSCustomerInsights": result = new CustomerInsightsProcessor(); break;
        case "TSReportSender": result = new ServiceThreadPool<ReportSender>("ReportSender"); break;
        case "TokTranscoder": result = new ServiceThreadPool<TokTranscoder>("TokTranscoder"); break;
        case "TSTaskProcessor": result = new ServiceThreadPool<TaskProcessor>("TaskProcessor"); break;
        default: result = null; break;
      }

      return result;
    }

    protected override void OnStart(string[] args)
    {
      //System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.Idle;
      _thread = GetServiceThread();
      _thread.Start();
    }

    protected override void OnStop()
    {
      _thread.Stop();
    }

    protected override void OnShutdown()
    {
      _thread.Stop();
    }

    protected override void OnContinue()
    {
      base.OnContinue();
    }

    protected override void OnPause()
    {
      base.OnPause();
    }

    protected override void OnCustomCommand(int command)
    {
      base.OnCustomCommand(command);
    }

    protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
    {
      return base.OnPowerEvent(powerStatus);
    }

    protected override void OnSessionChange(SessionChangeDescription changeDescription)
    {
      base.OnSessionChange(changeDescription);
    }
  }
}
