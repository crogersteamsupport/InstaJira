using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace TeamSupport.DataManager.Forms
{
  public partial class MySqlConnectionDialog : Form
  {
    public MySqlConnectionDialog()
    {
      InitializeComponent();
    }

    private static string _connectionString = "Server={0};Uid={1};Pwd={2};Database={3}";

    public static string[] GetConnectionString()
    {
      MySqlConnectionDialog form = new MySqlConnectionDialog();
      List<string> result = new List<string>();

      if (form.ShowDialog() == DialogResult.OK)
      {
        Properties.Settings.Default.Save();
        result.Add(string.Format(_connectionString, form.textServer.Text, form.textUser.Text, form.textPassword.Text, form.textDbName.Text  ));
        result.Add("My Sql - " + form.textDbName.Text);
        return result.ToArray();
      }
      else
      {
        return null;
      }

    }

    private void btnTest_Click(object sender, EventArgs e)
    {
      try 
	    {
        MySqlConnection connection = new MySqlConnection(string.Format(_connectionString, textServer.Text, textUser.Text, textPassword.Text, textDbName.Text));
        connection.Open();
        connection.Close();
        MessageBox.Show("Connection was successful.");
	    }
	    catch (Exception)
	    {
        MessageBox.Show("Connection failed.");
	    }

    }

  }
}
