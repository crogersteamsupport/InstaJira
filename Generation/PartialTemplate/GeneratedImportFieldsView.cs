using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ImportFieldsViewItem : BaseItem
  {
    private ImportFieldsView _importFieldsView;
    
    public ImportFieldsViewItem(DataRow row, ImportFieldsView importFieldsView): base(row, importFieldsView)
    {
      _importFieldsView = importFieldsView;
    }
	
    #region Properties
    
    public ImportFieldsView Collection
    {
      get { return _importFieldsView; }
    }
        
    
    
    

    
    public string FieldName
    {
      get { return Row["FieldName"] != DBNull.Value ? (string)Row["FieldName"] : null; }
      set { Row["FieldName"] = CheckValue("FieldName", value); }
    }
    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int? ImportMapID
    {
      get { return Row["ImportMapID"] != DBNull.Value ? (int?)Row["ImportMapID"] : null; }
      set { Row["ImportMapID"] = CheckValue("ImportMapID", value); }
    }
    
    public int? ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (int?)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }
    
    public string SourceName
    {
      get { return Row["SourceName"] != DBNull.Value ? (string)Row["SourceName"] : null; }
      set { Row["SourceName"] = CheckValue("SourceName", value); }
    }
    
    public bool? IsCustom
    {
      get { return Row["IsCustom"] != DBNull.Value ? (bool?)Row["IsCustom"] : null; }
      set { Row["IsCustom"] = CheckValue("IsCustom", value); }
    }
    
    public string FileName
    {
      get { return Row["FileName"] != DBNull.Value ? (string)Row["FileName"] : null; }
      set { Row["FileName"] = CheckValue("FileName", value); }
    }
    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
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
    
    public string TableName
    {
      get { return (string)Row["TableName"]; }
      set { Row["TableName"] = CheckValue("TableName", value); }
    }
    
    public int ImportFieldID
    {
      get { return (int)Row["ImportFieldID"]; }
      set { Row["ImportFieldID"] = CheckValue("ImportFieldID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ImportFieldsView : BaseCollection, IEnumerable<ImportFieldsViewItem>
  {
    public ImportFieldsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ImportFieldsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return ""; }
    }



    public ImportFieldsViewItem this[int index]
    {
      get { return new ImportFieldsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ImportFieldsViewItem importFieldsViewItem);
    partial void AfterRowInsert(ImportFieldsViewItem importFieldsViewItem);
    partial void BeforeRowEdit(ImportFieldsViewItem importFieldsViewItem);
    partial void AfterRowEdit(ImportFieldsViewItem importFieldsViewItem);
    partial void BeforeRowDelete(int );
    partial void AfterRowDelete(int );    

    partial void BeforeDBDelete(int );
    partial void AfterDBDelete(int );    

    #endregion

    #region Public Methods

    public ImportFieldsViewItemProxy[] GetImportFieldsViewItemProxies()
    {
      List<ImportFieldsViewItemProxy> list = new List<ImportFieldsViewItemProxy>();

      foreach (ImportFieldsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int )
    {
      BeforeDBDelete();
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ImportFieldsView] WH);";
        deleteCommand.Parameters.Add("", SqlDbType.Int);
        deleteCommand.Parameters[""].Value = ;

        BeforeRowDelete();
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete();
      }
      AfterDBDelete();
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ImportFieldsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ImportFieldsView] SET     [ImportFieldID] = @ImportFieldID,    [TableName] = @TableName,    [FieldName] = @FieldName,    [Alias] = @Alias,    [DataType] = @DataType,    [Size] = @Size,    [IsVisible] = @IsVisible,    [IsRequired] = @IsRequired,    [Description] = @Description,    [RefType] = @RefType,    [Position] = @Position,    [ImportMapID] = @ImportMapID,    [ImportID] = @ImportID,    [SourceName] = @SourceName,    [IsCustom] = @IsCustom,    [FileName] = @FileName,    [OrganizationID] = @OrganizationID  WH);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("FieldName", SqlDbType.VarChar, 561);
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
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportMapID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SourceName", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsCustom", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FileName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ImportFieldsView] (    [ImportFieldID],    [TableName],    [FieldName],    [Alias],    [DataType],    [Size],    [IsVisible],    [IsRequired],    [Description],    [RefType],    [Position],    [ImportMapID],    [ImportID],    [SourceName],    [IsCustom],    [FileName],    [OrganizationID]) VALUES ( @ImportFieldID, @TableName, @FieldName, @Alias, @DataType, @Size, @IsVisible, @IsRequired, @Description, @RefType, @Position, @ImportMapID, @ImportID, @SourceName, @IsCustom, @FileName, @OrganizationID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("FileName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsCustom", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SourceName", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ImportMapID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		
		tempParameter = insertCommand.Parameters.Add("FieldName", SqlDbType.VarChar, 561);
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
		
		tempParameter = insertCommand.Parameters.Add("ImportFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ImportFieldsView] WH);";
		deleteCommand.Parameters.Add("", SqlDbType.Int);

		try
		{
		  foreach (ImportFieldsViewItem importFieldsViewItem in this)
		  {
			if (importFieldsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(importFieldsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = importFieldsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns[""].AutoIncrement = false;
			  Table.Columns[""].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				importFieldsViewItem.Row[""] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(importFieldsViewItem);
			}
			else if (importFieldsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(importFieldsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = importFieldsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(importFieldsViewItem);
			}
			else if (importFieldsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)importFieldsViewItem.Row["", DataRowVersion.Original];
			  deleteCommand.Parameters[""].Value = id;
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

      foreach (ImportFieldsViewItem importFieldsViewItem in this)
      {
        if (importFieldsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)importFieldsViewItem.Row["CreatorID"] == 0) importFieldsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (importFieldsViewItem.Row.Table.Columns.Contains("ModifierID")) importFieldsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ImportFieldsViewItem FindBy(int )
    {
      foreach (ImportFieldsViewItem importFieldsViewItem in this)
      {
        if (importFieldsViewItem. == )
        {
          return importFieldsViewItem;
        }
      }
      return null;
    }

    public virtual ImportFieldsViewItem AddNewImportFieldsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ImportFieldsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ImportFieldsViewItem(row, this);
    }
    
    public virtual void LoadBy(int )
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ImportFieldID], [TableName], [FieldName], [Alias], [DataType], [Size], [IsVisible], [IsRequired], [Description], [RefType], [Position], [ImportMapID], [ImportID], [SourceName], [IsCustom], [FileName], [OrganizationID] FROM [dbo].[ImportFieldsView] WH);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("", );
        Fill(command);
      }
    }
    
    public static ImportFieldsViewItem GetImportFieldsViewItem(LoginUser loginUser, int )
    {
      ImportFieldsView importFieldsView = new ImportFieldsView(loginUser);
      importFieldsView.LoadBy();
      if (importFieldsView.IsEmpty)
        return null;
      else
        return importFieldsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ImportFieldsViewItem> Members

    public IEnumerator<ImportFieldsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ImportFieldsViewItem(row, this);
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
