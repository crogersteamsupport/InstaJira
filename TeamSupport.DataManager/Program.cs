using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TeamSupport.Data;

namespace TeamSupport.DataManager
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      try
      {
        LoginSession.LoginUser = new LoginUser(System.Configuration.ConfigurationManager.AppSettings["MainConnection"], 34, 1078, null);
        Application.Run(new MainForm());
      }
      catch (Exception ex)
      {
        MessageBox.Show("Exception: " + ex.Message);
        MessageBox.Show(ex.StackTrace);
      }
    }
  }
}
