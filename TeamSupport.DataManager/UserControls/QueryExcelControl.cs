using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Export;
using System.IO;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using TeamSupport.Data;



namespace TeamSupport.DataManager.UserControls
{
  public partial class QueryExcelControl : UserControl
  {
    private System.Data.Common.DbConnection _connection;
    public QueryExcelControl()
    {
      InitializeComponent();
    }

    private void QueryExcelControl_Load(object sender, EventArgs e)
    {
      //SetFile(Properties.Settings.Default.SpreadSheetSourceFileName);
      textQuery.Text = Properties.Settings.Default.SpreadSheetQuery;
    }
    

    private void btnOpen_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.Filter = "Excel Files (*.xls, *.xlsx)|*.xls;*.xlsx|All Files (*.*)|*.*";
      dialog.FileName = Properties.Settings.Default.SpreadSheetSourceFileName;
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        if (dialog.FileName.Trim() == "") return;
        //string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + dialog.FileName + @";Extended Properties=""Excel 8.0;HDR=YES;IMEX=1""";
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + dialog.FileName + @";Extended Properties=""Excel 12.0;HDR=YES;IMEX=1""";

        lblSource.Text = "Spreadsheet: " + dialog.FileName;
        Properties.Settings.Default.SpreadSheetSourceFileName = dialog.FileName;
        Properties.Settings.Default.Save();
        btnExecute.Enabled = true;
        _connection = GetSpreadSheetConnection(connectionString);
      }
    }
    
   
    private OleDbConnection GetSpreadSheetConnection(string connectionString)
    {
      try
      {
        OleDbConnection connection = new OleDbConnection(connectionString);
        connection.Open();
        connection.Close();
        return connection;
      }
      catch (Exception ex)
      {
        MessageBox.Show("There was an error opening the file.\n\n"+ex.Message);
        return null;
      }
    
    }

    private void button1_Click(object sender, EventArgs e)
    {
      string[] result = Forms.MySqlConnectionDialog.GetConnectionString();
      if (result == null) return;

      //server=localhost;User Id=root;Persist Security Info=True;database=fogbugz
      string connectionString = result[0];
      lblSource.Text = result[1];
      btnExecute.Enabled = true;
      _connection = new MySqlConnection(connectionString);
    }


    private void btnExecute_Click(object sender, EventArgs e)
    {
      ExecuteQuery();
    }

    public void ExecuteQuery()
    {
      gridResults.Columns.Clear();
      lblCount.Text = "";
      Properties.Settings.Default.SpreadSheetQuery = textQuery.Text;
      Properties.Settings.Default.Save();
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        string query;
        if (textQuery.SelectionLength > 0)
          query = textQuery.SelectedText;
        else
          query = textQuery.Text;
        System.Data.Common.DbDataAdapter adapter = null;
        if (_connection is OleDbConnection) adapter = new OleDbDataAdapter(query, _connection as OleDbConnection);
        else if (_connection is MySqlConnection) adapter = new MySqlDataAdapter(query, _connection as MySqlConnection);

        if (adapter == null) return;
        DataTable table = new DataTable();
        adapter.Fill(table);
        _connection.Close();
        gridResults.DataSource = table;
        tabControl.SelectedIndex = 0;
        lblCount.Text = "Count: " + table.Rows.Count.ToString();
        
      }
      catch (Exception ex)
      {
        tabControl.SelectedIndex = 1;
        textResults.Lines = new string[] {"There were errors executing your query.", "", ex.Message };
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      
      }

    }

    private void ExportToExcel(string fileName)
    {
      TextWriter writer = new StreamWriter(fileName);

     /* StringBuilder builder = new StringBuilder();
      for (int i = 0; i < gridResults.Columns.Count; i++)
      {
        if (i > 0) builder.Append(",");
        builder.Append(gridResults.Columns[i].HeaderText);
      }
      writer.WriteLine(builder.ToString());

      DataTable table = (DataTable)gridResults.DataSource;

      foreach (DataRow row in table.Rows)
      {
        builder = new StringBuilder();
        for (int i = 0; i < table.Columns.Count; i++)
        {
          if (i > 0) builder.Append(",");
          string value = row[i].ToString();
          if (value.Length > 8000) value = value.Substring(0, 8000);
          
          value = "\"" + value.Replace("\"", "\"\"").Replace(Environment.NewLine, "<br />") + "\"";
          Encoding ascii = Encoding.GetEncoding("us-ascii", new EncoderReplacementFallback("*"), new DecoderReplacementFallback("*"));
          builder.Append(ascii.GetString(ascii.GetBytes(value)));
        }
        writer.WriteLine(builder.ToString());
      }*/

     // writer.Write(DataUtils.TableToCsv((DataTable)gridResults.DataSource, true));
      writer.Close();

    }

    private void btnExport_Click(object sender, EventArgs e)
    {

      Cursor.Current = Cursors.WaitCursor;
      try
      {
        string path = Path.Combine(Application.StartupPath, "Temp");
        Directory.CreateDirectory(path);
        string fileName = Path.Combine(path, Path.GetRandomFileName());
        fileName = Path.ChangeExtension(fileName, "csv");
        //ExportToExcelML exporter = new ExportToExcelML();
        //exporter.RunExport(gridResults, fileName, ExportToExcelML.ExcelMaxRows._1048576, false);
        ExportToExcel(fileName);

        //Telerik.Data.RadGridViewExcelExporter exporter = new Telerik.Data.RadGridViewExcelExporter();
        //exporter.Export(gridResults, fileName, "kevin");

        //ExportToExcel(fileName);
/*        DataTable table = (DataTable)gridResults.DataSource;
        table.TableName = "kevin";
        using (StreamWriter writer = File.CreateText(fileName))
        {
          table.WriteXml(writer, XmlWriteMode.WriteSchema, false);
          writer.Close();
          
        }*/
        MessageBox.Show("Exporting Complete");
        System.Diagnostics.Process.Start(fileName);
      }
      catch (Exception ex)
      {
        MessageBox.Show("There was an error exporting the data.\n\n" + ex.Message);
      }
      finally
      {
        Cursor.Current = Cursors.Default;
      }
      
      
      /*SaveFileDialog dialog = new SaveFileDialog();
      dialog.DefaultExt = ".csv";
      dialog.InitialDirectory = Path.GetDirectoryName(lblFileName.Text);
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        Cursor.Current = Cursors.WaitCursor;
        try
        {
          ExportToExcel(dialog.FileName);

          
          MessageBox.Show("Exporting Complete");
        }
        catch (Exception ex)
        {
          MessageBox.Show("There was an error exporting the data.\n\n" + ex.Message);
        }
        finally
        {
          Cursor.Current = Cursors.Default;          
        }
      }*/
    }

    private void btnOpenQuery_Click(object sender, EventArgs e)
    {
      OpenFileDialog dialog = new OpenFileDialog();
      dialog.FileName = Properties.Settings.Default.SpreadSheetQueryFileName;
      dialog.Filter = "Query Files (*.sql)|*.sql|All Files (*.*)|*.*";
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        Properties.Settings.Default.SpreadSheetQueryFileName = dialog.FileName;
        Properties.Settings.Default.Save();
        TextReader reader = new StreamReader(dialog.FileName);
        textQuery.Text = reader.ReadToEnd();
        reader.Close();
      }
      

    }

    private void btnSaveQuery_Click(object sender, EventArgs e)
    {
      SaveFileDialog dialog = new SaveFileDialog();
      dialog.DefaultExt = ".sql";
      try
      {
        dialog.FileName = Path.GetFileName(Properties.Settings.Default.SpreadSheetQueryFileName);
        dialog.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.SpreadSheetQueryFileName);
      }
      catch (Exception)
      {
        
      }
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        Properties.Settings.Default.SpreadSheetQueryFileName = dialog.FileName;
        Properties.Settings.Default.Save();
        TextWriter writer = new StreamWriter(dialog.FileName);
        writer.Write(textQuery.Text);
        writer.Close();
      }
    }

    private void textQuery_TextChanged(object sender, EventArgs e)
    {

    }

    private void btnExportSingleColumn_Click(object sender, EventArgs e)
    {
      StringBuilder lines = new StringBuilder();
      DataTable table = (DataTable)gridResults.DataSource;

      foreach (DataRow row in table.Rows)
      {
        StringBuilder line = new StringBuilder();

        for (int i = 0; i < table.Columns.Count; i++)
        {
          line.Append(row[i].ToString().Trim());
        }
        lines.AppendLine(line.ToString());
      }

      Clipboard.SetText(lines.ToString());
      MessageBox.Show("The export was copy to the clipboard.");


    }

    private void btnBlob_Click(object sender, EventArgs e)
    {
      Properties.Settings.Default.SpreadSheetQuery = textQuery.Text;
      Properties.Settings.Default.Save();
      try
      {
        tabControl.SelectedIndex = 1;
        Cursor.Current = Cursors.WaitCursor;
        string query = textQuery.SelectionLength > 0 ? textQuery.SelectedText : textQuery.Text;
        System.Data.Common.DbDataAdapter adapter = null;
        if (_connection is OleDbConnection) adapter = new OleDbDataAdapter(query, _connection as OleDbConnection);
        else if (_connection is MySqlConnection) adapter = new MySqlDataAdapter(query, _connection as MySqlConnection);

        if (adapter == null) return;
        DataTable table = new DataTable();
        adapter.Fill(table);
        _connection.Close();

        List<string> results = new List<string>();
        int count = 0;
        foreach (DataRow row in table.Rows)
        {
          count++;
          string fileName = Path.Combine(Path.Combine(row[0].ToString(), row[1].ToString()), row[2].ToString());
          Directory.CreateDirectory(Path.GetDirectoryName(fileName));
          byte[] data = (byte[])row[3];
          FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
          stream.Write(data, 0, data.Length);
          stream.Close();

          results.Add(string.Format("Saved {0} of {1}: {2}", count.ToString(), table.Rows.Count.ToString(), fileName));
          textResults.Lines = results.ToArray();
        }

        MessageBox.Show("Export complete");
      }
      catch (Exception ex)
      {
        textResults.Lines = new string[] { "There were errors executing your query.", "", ex.Message };
      }
      finally
      {
        Cursor.Current = Cursors.Default;

      }


    }





  }
}
