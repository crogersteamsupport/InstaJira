using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BusinessObjectGenerator.Forms
{
  public partial class FormConnect : Form
  {
    private FormConnect()
    {
      InitializeComponent();
    }

    public static bool Connect()
    {
      
      FormConnect form = new FormConnect();

      if (form.ShowDialog() == DialogResult.OK)
      {
        AppSettings.Default.Save();
        return true;
      }

      return false;
    }

   

    private void FormConnect_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (DialogResult == DialogResult.Cancel) return;

      string connectionString = string.Format("Data Source={0};Persist Security Info=True;User ID={1};password={2};", txtHost.Text, txtUserName.Text, txtPassword.Text);
      SqlConnection connection = new SqlConnection(connectionString);
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        try
        {
          connection.Open();

        }
        finally
        {
          Cursor.Current = Cursors.Default;
        }
      }
      catch (Exception)
      {
        e.Cancel = true;
        MessageBox.Show("Invalid connection parameters.");
      }
      
      
    }
  }
}
