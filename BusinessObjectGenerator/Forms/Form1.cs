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
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    public static void DoIt(SqlConnection connection)
    {
      Form1 form = new Form1();
      form.Show();
      form.LoadData(connection);
    }

    private void LoadData(SqlConnection connection)
    {
      SqlCommand command = new SqlCommand("SELECT * FROM Actions", connection);
      SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly);
      DataTable table = reader.GetSchemaTable();
      dataGridView1.DataSource = table;
      reader.Close();

    }
    private void button1_Click(object sender, EventArgs e)
    {

    }
  }
}
