using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;

namespace TeamSupport.Service
{
  [RunInstaller(true)]
  public class TSInstaller : Installer
  {
    public TSInstaller()
    {
      var processInstaller = new ServiceProcessInstaller();
      var serviceInstaller = new ServiceInstaller();

      //set the privileges
      processInstaller.Account = ServiceAccount.LocalSystem;

      serviceInstaller.DisplayName = ConfigurationManager.AppSettings["ServiceDisplayName"];
      serviceInstaller.ServiceName = ConfigurationManager.AppSettings["ServiceName"];
      serviceInstaller.StartType = ServiceStartMode.Automatic;


      this.Installers.Add(processInstaller);
      this.Installers.Add(serviceInstaller);
    }
  }
}
