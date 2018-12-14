using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ImportField : BaseItem
  {
    private ImportFields _importFields;
    
    public ImportField(DataRow row, ImportFields importFields): base(row, importFields)
    {
      _importFields = importFields;
    }
	
    #region Properties
    
    public ImportFields Collection
    {
      get { return _importFields; }
    }
        
    
    
    
    public int ImportFieldID
    {
      get { return (int)Row["ImportFieldID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    

    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public bool Enabled
    {
      get { return (bool)Row["Enabled"]; }
      set { Row["Enabled"] = CheckValue("Enabled", value); }
    }
    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public bool IsRequired
    {
      get { return (bool)Row["IsRequired"]; }
      set { Row["IsRequired"] = CheckValue("IsRequired", value); }
    }
    
    public bool IsVisible
    {
      get { return (bool)Row["IsVisible"]; }
      set { Row["IsVisible"] = CheckValue("IsVisible", value); }
    }
    
    public int Size
    {
      get { return (int)Row["Size"]; }
      set { Row["Size"] = CheckValue("Size", value); }
    }
    
    public string DataType
    {
      get { return (string)Row["DataType"]; }
      set { Row["DataType"] = CheckValue("DataType", value); }
    }
    
    public string Alias
    {
      get { return (string)Row["Alias"]; }
      set { Row["Alias"] = CheckValue("Alias", value); }
    }
    
    public string FieldName
    {
      get { return (string)Row["FieldName"]; }
      set { Row["FieldName"] = CheckValue("FieldName", value); }
    }
    
    public string TableName
    {
      get { return (string)Row["TableName"]; }
      set { Row["TableName"] = CheckValue("TableName", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ImportFields : BaseCollection, IEnumerable<ImportField>
  {
    public ImportFields(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ImportFields"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ImportFieldID"; }
    }



    public ImportField this[int index]
    {
      get { return new ImportField(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ImportField importField);
    partial void AfterRowInsert(ImportField importField);
    partial void BeforeRowEdit(ImportField importField);
    partial void AfterRowEdit(ImportField importField);
    partial void BeforeRowDelete(int importFieldID);
    partial void AfterRowDelete(int importFieldID);    

    partial void BeforeDBDelete(int importFieldID);
    partial void AfterDBDelete(int importFieldID);    

    #endregion

    #region Public Methods

    public ImportFieldProxy[] GetImportFieldProxies()
    {
      List<ImportFieldProxy> list = new List<ImportFieldProxy>();

      foreach (ImportField item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int importFieldID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ImportFields] WHERE ([ImportFieldID] = @ImportFieldID);";
        deleteCommand.Parameters.Add("ImportFieldID", SqlDbType.Int);
        deleteCommand.Parameters["ImportFieldID"].Value = importFieldID;

        BeforeDBDelete(importFieldID);
        BeforeRowDelete(importFieldID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(importFieldID);
        AfterDBDelete(importFieldID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ImportFieldsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ImportFields] SET     [TableName] = @TableName,    [FieldName] = @FieldName,    [Alias] = @Alias,    [DataType] = @DataType,    [Size] = @Size,    [IsVisible] = @IsVisible,    [IsRequired] = @IsRequired,    [Description] = @Description,    [RefType] = @RefType,    [Enabled] = @Enabled,    [Position] = @Position  WHERE ([ImportFieldID] = @ImportFieldID);";

		
		tempParameter = updateCommand.Parameters.Add("ImportFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TableName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FieldName", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Alias", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DataType", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Size", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisible", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Enabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ImportFields] (    [TableName],    [FieldName],    [Alias],    [DataType],    [Size],    [IsVisible],    [IsRequired],    [Description],    [RefType],    [Enabled],    [Position]) VALUES ( @TableName, @FieldName, @Alias, @DataType, @Size, @IsVisible, @IsRequired, @Description, @RefType, @Enabled, @Position); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Enabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsRequired", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisible", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Size", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DataType", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Alias", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FieldName", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TableName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ImportFields] WHERE ([ImportFieldID] = @ImportFieldID);";
		deleteCommand.Parameters.Add("ImportFieldID", SqlDbType.Int);

		try
		{
		  foreach (ImportField importField in this)
		  {
			if (importField.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(importField);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = importField.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ImportFieldID"].AutoIncrement = false;
			  Table.Columns["ImportFieldID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				importField.Row["ImportFieldID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(importField);
			}
			else if (importField.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(importField);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = importField.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(importField);
			}
			else if (importField.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)importField.Row["ImportFieldID", DataRowVersion.Original];
			  deleteCommand.Parameters["ImportFieldID"].Value = id;
			  BeforeRowDelete(id);
			  deleteCommand.ExecuteNonQuery();
			  AfterRowDelete(id);
			}
		  }
		  //transaction.Commit();
		}
		catch (Exception)
		{
		  //transaction.Rollback();
		  throw;
		}
		Table.AcceptChanges();
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public void BulkSave()
    {

      foreach (ImportField importField in this)
      {
        if (importField.Row.Table.Columns.Contains("CreatorID") && (int)importField.Row["CreatorID"] == 0) importField.Row["CreatorID"] = LoginUser.UserID;
        if (importField.Row.Table.Columns.Contains("ModifierID")) importField.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ImportField FindByImportFieldID(int importFieldID)
    {
      foreach (ImportField importField in this)
      {
        if (importField.ImportFieldID == importFieldID)
        {
          return importField;
        }
      }
      return null;
    }

    public virtual ImportField AddNewImportField()
    {
      if (Table.Columns.Count < 1) LoadColumns("ImportFields");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ImportField(row, this);
    }
    
    public virtual void LoadByImportFieldID(int importFieldID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ImportFieldID], [TableName], [FieldName], [Alias], [DataType], [Size], [IsVisible], [IsRequired], [Description], [RefType], [Enabled], [Position] FROM [dbo].[ImportFields] WHERE ([ImportFieldID] = @ImportFieldID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ImportFieldID", importFieldID);
        Fill(command);
      }
    }
    
    public static ImportField GetImportField(LoginUser loginUser, int importFieldID)
    {
      ImportFields importFields = new ImportFields(loginUser);
      importFields.LoadByImportFieldID(importFieldID);
      if (importFields.IsEmpty)
        return null;
      else
        return importFields[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ImportField> Members

    public IEnumerator<ImportField> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ImportField(row, this);
      }
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    #endregion
  }
}
