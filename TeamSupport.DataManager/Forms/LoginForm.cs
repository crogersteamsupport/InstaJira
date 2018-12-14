using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;

namespace TeamSupport.DataManager
{
  public partial class LoginForm : Form
  {
    public LoginForm()
    {
      InitializeComponent();
      Telerik.WinControls.ThemeResolutionService.ApplicationThemeName = "Vista";
    }

    public static bool Login()
    {
      LoginForm form = new LoginForm();
      if (form.ShowDialog() == DialogResult.OK)
      {
        return true;
      }
      else
      {
        return false;
      }

    }

    private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult != DialogResult.OK) return;
      string connectionString = System.Configuration.ConfigurationManager.AppSettings["MainConnection"];
      LoginSession.LoginUser = new LoginUser(connectionString, 34, 1078, null);
    }
  }
}
