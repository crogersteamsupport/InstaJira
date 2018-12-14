using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace WatsonToneAnalyzer
{
    // https://docs.microsoft.com/en-us/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer#BK_WriteCode
    // C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\Users\sprichard\source\repos\ts-app\TeamSupport.ServiceLibrary\bin\Debug\WatsonToneAnalyzer.exe
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
