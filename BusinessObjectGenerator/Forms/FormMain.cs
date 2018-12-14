using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace BusinessObjectGenerator
{
  public partial class FormMain : Form
  {
    TableSchema _tableSchema;
    bool _schemaModified = false;
    string _schemaFileName = "";
    string _databaseName = "";

    public FormMain()
    {
      InitializeComponent();
      _tableSchema = new TableSchema();
      LoadTemplates(AppSettings.Default.TemplateFolder);
      foreach (ToolStripItem item in tsMain.Items)
      {
        item.Enabled = false;
      }

      tsbConnect.Enabled = true;
    }

    #region Events
    
    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = !ConfirmSchemaSave();
      AppSettings.Default.Save();
    }

    void cmbDatabases_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (_databaseName == cmbDatabases.Text) return;

      if (!ConfirmSchemaSave())
      {
        cmbDatabases.Text = _databaseName;
        return;
      }
      _databaseName = cmbDatabases.Text;
      AppSettings.Default.Database = _databaseName;
      LoadDBSchema();
    }

    private void gridFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      SetSchemaModified(true);
      RefreshPreview();
    }

    private void gridTables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      SetSchemaModified(true);

    }

    private void gridTemplates_SelectionChanged(object sender, EventArgs e)
    {
      string fileName = GetSelectedTemplateFile();
      if (!File.Exists(fileName)) return;

      txtTemplate.Text = GetTemplateText(fileName);
      RefreshPreview();
    }

    private void tsmiConnect_Click(object sender, EventArgs e)
    {
      GetConnection();
    }

    private void tsmiExit_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void gridTables_SelectionChanged(object sender, EventArgs e)
    {
      fieldsBindingSource.Filter = "TableName = '" + GetSelectedTableName() + "'";
      RefreshPreview();
    }

    private void tsbGenerate_Click(object sender, EventArgs e)
    {
      GenerateAll();
    }

    private void tsbRefreshPreview_Click(object sender, EventArgs e)
    {
      RefreshPreview();
    }

    private void tsbOpenSchema_Click(object sender, EventArgs e)
    {
      if (!ConfirmSchemaSave()) return;
      dialogOpenSchema.FileName = AppSettings.Default.LastSchema;
      try
      {
        dialogOpenSchema.InitialDirectory = Path.GetDirectoryName(AppSettings.Default.LastSchema);
      }
      catch (Exception)
      {
      }
      if (dialogOpenSchema.ShowDialog() == DialogResult.OK)
      {
        LoadSchemaFile(dialogOpenSchema.FileName);
      }
    }

    private void tsbSaveSchema_Click(object sender, EventArgs e)
    {
      SaveSchema();
    }

    private void tsbSaveSchemaAs_Click(object sender, EventArgs e)
    {
      SaveSchemaAs();
    }

    private void tsbOpenTemplates_Click(object sender, EventArgs e)
    {
      OpenTemplateFolder();
    }
    
    private void tsbSettings_Click(object sender, EventArgs e)
    {
      Forms.FormSettings.Open();
    }

    private void tsbDeleteStoredProcs_Click(object sender, EventArgs e)
    {
      if (MessageBox.Show("Are you sure you would like to delete all your generated stored procedures?", "Delete Stored Procedures", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
      
      SqlConnection connection = new SqlConnection(GetConnectionString(_databaseName));
      connection.Open();
      SqlDataAdapter adapter = new SqlDataAdapter("SELECT name FROM sysobjects WHERE name like 'uspGenerated%'", connection);
      DataTable table = new DataTable();
      adapter.Fill(table);
      SqlCommand command = new SqlCommand();
      command.Connection = connection;

      foreach (DataRow row in table.Rows)
      {
        command.CommandText = "DROP PROCEDURE " + (string)row[0];
        command.ExecuteNonQuery();
      }
    }

    private void tsbOpenDevelopment_Click(object sender, EventArgs e)
    {

      string path = AppSettings.Default.DevelopmentPath;

      if (!Directory.Exists(path))
      {
        FolderBrowserDialog dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          AppSettings.Default.DevelopmentPath = dialog.SelectedPath;
          path = dialog.SelectedPath;
        }
        else
        {
          return;
        }
      }
      System.Diagnostics.Process.Start(path);
    }

    private void tsbOpenOutput_Click(object sender, EventArgs e)
    {
      string path = AppSettings.Default.OutputPath;

      if (!Directory.Exists(path))
      {
        FolderBrowserDialog dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          AppSettings.Default.OutputPath = dialog.SelectedPath;
          path = dialog.SelectedPath;
        }
        else
        {
          return;
        }
      }
      System.Diagnostics.Process.Start(path);
    }

    #endregion
    
    #region Private Methods

    private void GetConnection()
    {
      if (!Forms.FormConnect.Connect()) return;

      cmbDatabases.Items.Clear();

      foreach (ToolStripItem item in tsMain.Items)
      {
        item.Enabled = false;
      }

      tsbConnect.Enabled = true;

      tsmSchema.Enabled = false;
      tsmTemplate.Enabled = false;
      pnlContent.Visible = false;

      using (SqlConnection connection = new SqlConnection(GetConnectionString()))
      {
        try
        {
          connection.Open();
          slServer.Text = "Host: " + AppSettings.Default.Host;
          SetSchemaModified(false);
          LoadDatabases(connection);
          connection.Close();
          if (cmbDatabases.SelectedIndex < 0) return;

          LoadDBSchema();

          foreach (ToolStripItem item in tsMain.Items)
          {
            item.Enabled = true;
          }
          tsmSchema.Enabled = true;
          tsmTemplate.Enabled = true;
          pnlContent.Visible = true;
          SetSchemaModified(false);
          SetSchemaFileName("");
        }
        catch (Exception ex)
        {
          MessageBox.Show("Unable to connect to the database server.\nError: " + ex.Message + "\n" + ex.StackTrace);
          return;
        }
      }

    
    }

    private string GetConnectionString()
    {
      return string.Format("Data Source={0};Persist Security Info=True;User ID={1};password={2}", AppSettings.Default.Host, AppSettings.Default.UserName, AppSettings.Default.Password);
    }

    private string GetConnectionString(string catalog)
    {
      return string.Format("Data Source={0};Persist Security Info=True;User ID={1};password={2};Initial Catalog={3}", AppSettings.Default.Host, AppSettings.Default.UserName, AppSettings.Default.Password, catalog);
    }

    private void LoadDatabases(SqlConnection connection)
    {
      cmbDatabases.SelectedIndexChanged -= cmbDatabases_SelectedIndexChanged;
      cmbDatabases.Items.Clear();

      DataTable table = connection.GetSchema("DataBases");
      foreach (DataRow row in table.Rows)
      {
        cmbDatabases.Items.Add(row["database_name"].ToString());
      }
      cmbDatabases.Sorted = true;

      cmbDatabases.SelectedIndexChanged += new EventHandler(cmbDatabases_SelectedIndexChanged);
      cmbDatabases.Text = AppSettings.Default.Database;
      if (cmbDatabases.SelectedIndex < 0 && cmbDatabases.Items.Count > 0) cmbDatabases.SelectedIndex = 0;
    }

    private void LoadSchemaFile(string fileName)
    {
      if (!File.Exists(fileName)) return;
      
      LoadDBSchema();

      try
      {
        DataSet dataset = new DataSet();
        dataset.ReadXml(fileName);

        foreach (DataRow srcRow in dataset.Tables["TableNames"].Rows)
        {
          DataRow dstRow = _tableSchema.TableNames.FindByTableName((string)srcRow["TableName"]);
          if (dstRow != null)
          {
            foreach (DataColumn column in dstRow.Table.Columns)
	          {
              if (!column.ReadOnly)
              {
                if (srcRow.Table.Columns.Contains(column.ColumnName))
                {
                  object o = srcRow[column.ColumnName];
                  if (o != null)
                  {
                    dstRow[column] = o;
                  }
                }
              }
	          }
          }
        }

        foreach (DataRow srcRow in dataset.Tables["Fields"].Rows)
        {
          DataRow dstRow = _tableSchema.Fields.FindByTableNameFieldName((string)srcRow["TableName"], (string)srcRow["FieldName"]);
          if (dstRow != null)
          {
            foreach (DataColumn column in dstRow.Table.Columns)
            {
              if (!column.ReadOnly)
              {
                if (srcRow.Table.Columns.Contains(column.ColumnName))
                {
                  object o = srcRow[column.ColumnName];
                  if (o != null)
                  {
                    dstRow[column] = o;
                  }
                }
              }
            }
          }
        }

        AppSettings.Default.LastSchema = fileName;
        SetSchemaFileName(fileName);
        SetSchemaModified(false);

    }
      catch (Exception e)
      {
        MessageBox.Show("Unable to load file due to unexpected error.\nError: " + e.Message + "\n\n" + e.StackTrace);
      }
    }

    private void LoadSchemaFile2(string fileName)
    {
      if (!File.Exists(fileName)) return;

      TableSchema tableSchema = new TableSchema();
      tableSchema.ReadXml(fileName);

      foreach (TableSchema.TableNamesRow table in tableSchema.TableNames.Rows)
      {
        TableSchema.TableNamesRow row = _tableSchema.TableNames.FindByTableName(table.TableName);
        if (row != null)
        {
          row.ItemName = table.ItemName;
          row.CollectionName = table.CollectionName;
          row.Include = table.Include;
          row.IsDBReadOnly = table.IsDBReadOnly;
        }
      }

      foreach (TableSchema.FieldsRow field in tableSchema.Fields.Rows)
      {
        TableSchema.FieldsRow row = _tableSchema.Fields.FindByTableNameFieldName(field.TableName, field.FieldName);
        if (row != null)
        {
          row.CustomType = field.CustomType;
          row.IsReadOnly = field.IsReadOnly;
          row.AllowDBNull = field.AllowDBNull;
          row.IsPrimaryKey = field.IsPrimaryKey;
        }
      }

      AppSettings.Default.LastSchema = fileName;
      SetSchemaFileName(fileName);
      SetSchemaModified(false);
    }
    
    private bool ConfirmSchemaSave()
    {
      if (_schemaModified)
      {
        switch (MessageBox.Show("Would you like to save your schema before continuing?", "Save Schema?", MessageBoxButtons.YesNoCancel)  )
        {
          case DialogResult.Cancel: return false;
          case DialogResult.Yes: SaveSchema(); break;
          default: break;
        }
      }
      return true;
    
    }

    private void SaveSchema()
    {
      if (!File.Exists(_schemaFileName))
      { 
        SaveSchemaAs();
      }
      else
      {
        SaveSchemaToFile(_schemaFileName);
      }
    }

    private void SaveSchemaAs()
    {
      dialogSaveSchema.FileName = _schemaFileName == "" ? _databaseName + ".bos" : _schemaFileName;
      if (dialogSaveSchema.ShowDialog() == DialogResult.OK)
      {
        SaveSchemaToFile(dialogSaveSchema.FileName);
      }
    }

    private void SaveSchemaToFile(string fileName)
    {
      _tableSchema.WriteXml(fileName);
      SetSchemaFileName(fileName);
      SetSchemaModified(false);
    }
    
    private bool LoadDBSchema()
    {
      try
      {

        SetSchemaFileName("");
        SetSchemaModified(false);

        _tableSchema.Clear();

        using (SqlConnection connection = new SqlConnection(GetConnectionString(_databaseName)))
        {
          connection.Open();
          //Forms.Form1.DoIt(connection);

          DataTable table = connection.GetSchema("Tables");

          foreach (DataRow row in table.Rows)
          {
            string tableName = (string)row["TABLE_NAME"];
            TableSchema.TableNamesRow tableNameRow = _tableSchema.TableNames.NewTableNamesRow();
            tableNameRow.TableName = tableName;
            tableNameRow.Include = false;
            string collection = tableName;
            string item = tableName;
            if (tableName.Substring(tableName.Length - 3, 3) == "ies")
            {
              item = tableName.Remove(tableName.Length - 3, 3) + "y" ;
            }
            else if (tableName.Substring(tableName.Length - 2, 2) == "es")
            {
              //collection = tableName + "Collection";
              item = tableName.Remove(tableName.Length - 2, 2);
            }
            else if (tableName.Substring(tableName.Length - 1, 1) == "s")
            {
              item = tableName.Remove(tableName.Length - 1, 1);
            }
            else
            {
              item = item + "Item";
            }

            tableNameRow.ItemName = item;
            tableNameRow.CollectionName = tableName;
            tableNameRow.BaseCollectionClass = AppSettings.Default.BaseCollectionClass;
            tableNameRow.BaseItemClass = AppSettings.Default.BaseItemClass;
            tableNameRow.IsDBReadOnly = false;
            _tableSchema.TableNames.AddTableNamesRow(tableNameRow);

            LoadTableSchema(tableName, connection);
          }

          tableNamesBindingSource.DataSource = _tableSchema;
          tableNamesBindingSource.DataMember = "TableNames";
          tableNamesBindingSource.Sort = "TableName";
          fieldsBindingSource.DataSource = _tableSchema;
          fieldsBindingSource.DataMember = "Fields";
          gridFields.AutoGenerateColumns = true;
          gridFields.Columns[0].Visible = false;
          gridTables.AutoGenerateColumns = true;

          connection.Close();

          gridFields.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

          foreach (DataGridViewColumn column in gridFields.Columns)
          {
            if (column.ReadOnly)
            {
              column.DefaultCellStyle.BackColor = SystemColors.GradientInactiveCaption;
            }
          
          
          }

        }
        SetSchemaModified(false);
      }
      catch (Exception ex)
      {
        MessageBox.Show("Unable to retrieve the database schema.\n\n" + ex.Message);
        return false;
      }
      return true;

    }

    private void LoadTableSchema(string tableName, SqlConnection connection)
    {
      DataSet dataset = new DataSet();
      string sql = "SELECT * FROM " + tableName + " WHERE 0=1";
      SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
      adapter.FillSchema(dataset, SchemaType.Source);

      SqlCommand command = new SqlCommand(sql, connection);
      SqlDataReader reader = command.ExecuteReader(CommandBehavior.SchemaOnly);
      DataTable table = reader.GetSchemaTable();
      reader.Close();

      foreach (DataRow row in table.Rows)
      {
        TableSchema.FieldsRow field = _tableSchema.Fields.NewFieldsRow();
        field.TableName = tableName;
        field.FieldName = (string)row["ColumnName"];
        field.DBType = (string)row["DataTypeName"];
        SqlDbType sqlDbType = (SqlDbType)row["ProviderType"];
        field.SqlDbType = "SqlDbType." + (sqlDbType).ToString();
        field.SystemType = row["DataType"].ToString();  // (string)row["DataType"];
        field.CustomType = ObjectGenerator.ConvertSystemTypeToShortHand(field.SystemType);
        int size = (int)row["ColumnSize"];
        field.Size = (sqlDbType == SqlDbType.VarChar || sqlDbType == SqlDbType.NVarChar) && size > 8000 ? -1 : size;
        field.Precision = (short)row["NumericPrecision"];
        field.Scale = (short)row["NumericScale"];
        field.IsIdentity = (bool)row["IsIdentity"];
        field.IsAutoInc = (bool)row["IsAutoIncrement"];
        field.IsReadOnly = (bool)row["IsReadOnly"];
        field.AllowDBNull = (bool)row["AllowDBNull"];
        field.IsNullable = field.AllowDBNull && field.CustomType != "string";
        field.IsPrimaryKey = row.Table.Columns.Contains("IsPrimaryKey") ? (bool)row["IsPrimaryKey"] : false;

        for (int i = 0; i < dataset.Tables[0].PrimaryKey.Length; i++)
        {
          if (field.FieldName == dataset.Tables[0].PrimaryKey[i].ColumnName)
          {
            field.IsPrimaryKey = true;
          }
        }

        _tableSchema.Fields.AddFieldsRow(field);

      }

    }

    private void OpenTemplateFolder()
    {
      FolderBrowserDialog dialog = new FolderBrowserDialog();
      dialog.SelectedPath = AppSettings.Default.TemplateFolder;
      if (dialog.ShowDialog() == DialogResult.OK)
      {
        LoadTemplates(dialog.SelectedPath);
      }
    }

    private void LoadTemplates(string path)
    {
      if (!Directory.Exists(path)) return;
      AppSettings.Default.TemplateFolder = path;
      slTemplate.Text = "Template Folder: " + path;

      string[] files = Directory.GetFiles(path, "*.bot");

      DataTable table = new DataTable();
      table.Columns.Add("Include", Type.GetType("System.Boolean"));
      table.Columns.Add("FileName", Type.GetType("System.String"));
      table.Columns[1].ReadOnly = true;

      foreach (string fileName in files)
      {
        DataRow row = table.NewRow();
        row[0] = true;
        row[1] = Path.GetFileNameWithoutExtension(fileName);
        table.Rows.Add(row);
      }
      gridTemplates.DataSource = table;    
    
    }

    private void SetSchemaModified(bool value)
    {
      _schemaModified = value;

      slModified.Text = value ? "Modified" : "";
    }

    private void SetSchemaFileName(string fileName)
    {
      if (fileName != "")
      {
        AppSettings.Default.LastSchema = fileName;
        slSchema.Text = "Schema File: " + Path.GetFileName(fileName);
      }
      else
      {
        slSchema.Text = "Schema File: [New Schema]";
      }
      _schemaFileName = fileName;
    }

    private string GetSelectedTableString(string columnName)
    {
      if (gridTables.SelectedRows.Count < 1) return "";
      object o = gridTables.SelectedRows[0].Cells[columnName].Value;
      if (o == DBNull.Value) return "";
      return (string)gridTables.SelectedRows[0].Cells[columnName].Value;
    }

    private string GetSelectedTableName()
    {
      return GetSelectedTableString("TableName");
    }

    private string GetSelectedConditions()
    {
      return GetSelectedTableString("Conditions");
    }

    private string GetSelectedTemplateFile()
    {
      if (gridTemplates.SelectedRows.Count < 1) return "";
      string fileName = (string)gridTemplates.SelectedRows[0].Cells[1].Value + ".bot";
      fileName = Path.Combine(AppSettings.Default.TemplateFolder, fileName);
      return fileName;
    }

    private string GetPrimaryKey(string tableName)
    {
      foreach (TableSchema.FieldsRow row in _tableSchema.Fields.Rows)
      {
        if (row.TableName == tableName && row.IsPrimaryKey)
        {
          return row.FieldName;
        }
      }
      return "";
    }

    private string GetTemplateOutputFileName(string fileName, string collectionName)
    {
      StreamReader reader = File.OpenText(fileName);
      string output = reader.ReadLine().Replace("<$ItemCollection$>", collectionName);
      reader.Close();
      return output;
    }

    private string GetTemplateText(string fileName)
    {
      StreamReader reader = File.OpenText(fileName);
      StringBuilder builder = new StringBuilder();

      reader.ReadLine(); //remove FileName 

      string line;
      while ((line = reader.ReadLine()) != null)
      {
        builder.AppendLine(line);
      }
      reader.Close();

      return builder.ToString();
 
    }

    private void RefreshPreview()
    {
      string tableName = GetSelectedTableName();
      string fileName = GetSelectedTemplateFile();
      if (!File.Exists(fileName) || tableName == "") return;

      string itemName = (string)gridTables.SelectedRows[0].Cells[2].Value;
      string collectionName = (string)gridTables.SelectedRows[0].Cells[3].Value;


      txtPreview.Text = GenerateObject(itemName, collectionName, tableName , GetSelectedTemplateFile(), GetSelectedTableString("BaseItemClass"), GetSelectedTableString("BaseCollectionClass"), GetSelectedConditions(), false);
    }

    private string GenerateObject(string itemName, string collectionName, string tableName, string templateFileName, string baseItemClass, string baseCollectionClass, string conditions, bool isReadOnly)
    {
      string template = GetTemplateText(templateFileName);

      ObjectGenerator generator = new ObjectGenerator(itemName, collectionName, tableName, GetPrimaryKey(tableName), conditions, isReadOnly);
      generator.CreatorIDFieldName = AppSettings.Default.CreatorIDField;
      generator.DateCreatedFieldName = AppSettings.Default.CreatedDateField;
      generator.ModifierIDFieldName = AppSettings.Default.ModifierIDField;
      generator.DateModifiedFieldName = AppSettings.Default.ModifiedDateField;
      generator.BaseCollectionClass = baseCollectionClass;
      generator.BaseItemClass = baseItemClass;

      foreach (TableSchema.FieldsRow row in _tableSchema.Fields.Rows)
      {
        if (row.TableName == tableName)
        {
          LoadField(generator.AddField(), row);
        }
      }
      return generator.Generate(template);
    }

    private void LoadField(Field field, TableSchema.FieldsRow row)
    {
      field.PropertyName = row.PropertyName;
      field.FieldName = row.FieldName;
      field.DBType = row.DBType;
      field.CustomType = row.CustomType;
      field.SystemType = row.SystemType;
      field.SqlDbType = row.SqlDbType;
      field.Size = row.Size;
      field.Scale = row.Scale;
      field.Precision = row.Precision;
      field.IsAutoInc = row.IsAutoInc;
      field.IsPrimaryKey = row.IsPrimaryKey;
      field.IsReadOnly = row.IsReadOnly;
      field.AllowDBNull = row.AllowDBNull;
      field.IsIdentity = row.IsIdentity;
      field.IsNullable = row.IsNullable;

    }

    private StoredProcGenerator GeneartStoredProc(string tableName, string itemName, bool isDBReadOnly)
    {
      StoredProcGenerator generator = new StoredProcGenerator(tableName, itemName, isDBReadOnly);
      generator.CreatorIDFieldName = AppSettings.Default.CreatorIDField;
      generator.DateCreatedFieldName = AppSettings.Default.CreatedDateField;
      generator.ModifierIDFieldName = AppSettings.Default.ModifierIDField;
      generator.DateModifiedFieldName = AppSettings.Default.ModifiedDateField;

      foreach (TableSchema.FieldsRow row in _tableSchema.Fields.Rows)
      {
        if (row.TableName == tableName)
        {
          LoadField(generator.AddField(), row);
        }
      }

      return generator;
    }

    private void GenerateAll()
    {
      string outputPath = AppSettings.Default.OutputPath;

      if (!Directory.Exists(outputPath))
      {
        FolderBrowserDialog dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          AppSettings.Default.OutputPath = dialog.SelectedPath;
          outputPath = dialog.SelectedPath;
        }
        else
        {
          return;
        }
      }

      StringBuilder builder = new StringBuilder();

      foreach (DataRow row in ((DataTable)gridTemplates.DataSource).Rows)
      {
        if ((bool)row[0])
        {
          string path = Path.Combine(outputPath, (string)row[1]);
          if (!Directory.Exists(path)) Directory.CreateDirectory(path);

          string fileName = Path.Combine(AppSettings.Default.TemplateFolder, (string)row[1] + ".bot");

          foreach (TableSchema.TableNamesRow table in _tableSchema.TableNames.Rows)
          {
            if (table.Include)
            {
              string output = GenerateObject(table.ItemName, table.CollectionName, table.TableName, fileName, table.BaseItemClass, table.BaseCollectionClass, table.Conditions, table.IsDBReadOnly);
              string outFileName = Path.Combine(path, GetTemplateOutputFileName(fileName, table.CollectionName));
              StreamWriter writer = new StreamWriter(outFileName);
              writer.Write(output);
              writer.Close();

              builder.Append(GeneartStoredProc(table.TableName, table.ItemName, table.IsDBReadOnly).Generate());
              builder.AppendLine();
            }
          }
        }
      }

      string storedProcFileName = Path.Combine(outputPath, "StoredProcedures.sql");
      StreamWriter storedProcWriter = new StreamWriter(storedProcFileName);
      storedProcWriter.Write(builder.ToString());
      storedProcWriter.Close();
    }

    #endregion



 

   

  }
}
