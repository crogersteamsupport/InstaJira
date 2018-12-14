using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ImportMap : BaseItem
  {
    private ImportMaps _importMaps;
    
    public ImportMap(DataRow row, ImportMaps importMaps): base(row, importMaps)
    {
      _importMaps = importMaps;
    }
	
    #region Properties
    
    public ImportMaps Collection
    {
      get { return _importMaps; }
    }
        
    
    
    
    public int ImportMapID
    {
      get { return (int)Row["ImportMapID"]; }
    }
    

    

    
    public bool IsCustom
    {
      get { return (bool)Row["IsCustom"]; }
      set { Row["IsCustom"] = CheckValue("IsCustom", value); }
    }
    
    public int FieldID
    {
      get { return (int)Row["FieldID"]; }
      set { Row["FieldID"] = CheckValue("FieldID", value); }
    }
    
    public string SourceName
    {
      get { return (string)Row["SourceName"]; }
      set { Row["SourceName"] = CheckValue("SourceName", value); }
    }
    
    public int ImportID
    {
      get { return (int)Row["ImportID"]; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ImportMaps : BaseCollection, IEnumerable<ImportMap>
  {
    public ImportMaps(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ImportMaps"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ImportMapID"; }
    }



    public ImportMap this[int index]
    {
      get { return new ImportMap(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ImportMap importMap);
    partial void AfterRowInsert(ImportMap importMap);
    partial void BeforeRowEdit(ImportMap importMap);
    partial void AfterRowEdit(ImportMap importMap);
    partial void BeforeRowDelete(int importMapID);
    partial void AfterRowDelete(int importMapID);    

    partial void BeforeDBDelete(int importMapID);
    partial void AfterDBDelete(int importMapID);    

    #endregion

    #region Public Methods

    public ImportMapProxy[] GetImportMapProxies()
    {
      List<ImportMapProxy> list = new List<ImportMapProxy>();

      foreach (ImportMap item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int importMapID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ImportMaps] WHERE ([ImportMapID] = @ImportMapID);";
        deleteCommand.Parameters.Add("ImportMapID", SqlDbType.Int);
        deleteCommand.Parameters["ImportMapID"].Value = importMapID;

        BeforeDBDelete(importMapID);
        BeforeRowDelete(importMapID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(importMapID);
        AfterDBDelete(importMapID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ImportMapsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ImportMaps] SET     [ImportID] = @ImportID,    [SourceName] = @SourceName,    [FieldID] = @FieldID,    [IsCustom] = @IsCustom  WHERE ([ImportMapID] = @ImportMapID);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("FieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsCustom", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ImportMaps] (    [ImportID],    [SourceName],    [FieldID],    [IsCustom]) VALUES ( @ImportID, @SourceName, @FieldID, @IsCustom); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsCustom", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ImportMaps] WHERE ([ImportMapID] = @ImportMapID);";
		deleteCommand.Parameters.Add("ImportMapID", SqlDbType.Int);

		try
		{
		  foreach (ImportMap importMap in this)
		  {
			if (importMap.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(importMap);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = importMap.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ImportMapID"].AutoIncrement = false;
			  Table.Columns["ImportMapID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				importMap.Row["ImportMapID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(importMap);
			}
			else if (importMap.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(importMap);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = importMap.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(importMap);
			}
			else if (importMap.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)importMap.Row["ImportMapID", DataRowVersion.Original];
			  deleteCommand.Parameters["ImportMapID"].Value = id;
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

      foreach (ImportMap importMap in this)
      {
        if (importMap.Row.Table.Columns.Contains("CreatorID") && (int)importMap.Row["CreatorID"] == 0) importMap.Row["CreatorID"] = LoginUser.UserID;
        if (importMap.Row.Table.Columns.Contains("ModifierID")) importMap.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ImportMap FindByImportMapID(int importMapID)
    {
      foreach (ImportMap importMap in this)
      {
        if (importMap.ImportMapID == importMapID)
        {
          return importMap;
        }
      }
      return null;
    }

    public virtual ImportMap AddNewImportMap()
    {
      if (Table.Columns.Count < 1) LoadColumns("ImportMaps");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ImportMap(row, this);
    }
    
    public virtual void LoadByImportMapID(int importMapID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ImportMapID], [ImportID], [SourceName], [FieldID], [IsCustom] FROM [dbo].[ImportMaps] WHERE ([ImportMapID] = @ImportMapID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ImportMapID", importMapID);
        Fill(command);
      }
    }
    
    public static ImportMap GetImportMap(LoginUser loginUser, int importMapID)
    {
      ImportMaps importMaps = new ImportMaps(loginUser);
      importMaps.LoadByImportMapID(importMapID);
      if (importMaps.IsEmpty)
        return null;
      else
        return importMaps[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ImportMap> Members

    public IEnumerator<ImportMap> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ImportMap(row, this);
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
