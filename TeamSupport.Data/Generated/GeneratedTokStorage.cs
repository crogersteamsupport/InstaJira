using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TokStorageItem : BaseItem
  {
    private TokStorage _tokStorage;
    
    public TokStorageItem(DataRow row, TokStorage tokStorage): base(row, tokStorage)
    {
      _tokStorage = tokStorage;
    }
	
    #region Properties
    
    public TokStorage Collection
    {
      get { return _tokStorage; }
    }




        
    public bool Transcoded
        {
      get { return (bool)Row["Transcoded"]; }
      set { Row["Transcoded"] = CheckValue("Transcoded", value); }
    }

    public string AmazonPath
    {
      get { return Row["AmazonPath"] != DBNull.Value ? (string)Row["AmazonPath"] : null; }
      set { Row["AmazonPath"] = CheckValue("AmazonPath", value); }
    }
    
    public string ArchiveID
    {
      get { return Row["ArchiveID"] != DBNull.Value ? (string)Row["ArchiveID"] : null; }
      set { Row["ArchiveID"] = CheckValue("ArchiveID", value); }
    }
    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime CreatedDate
    {
      get { return DateToLocal((DateTime)Row["CreatedDate"]); }
      set { Row["CreatedDate"] = CheckValue("CreatedDate", value); }
    }

    public DateTime CreatedDateUtc
    {
      get { return (DateTime)Row["CreatedDate"]; }
    }
    

    #endregion
    
    
  }

  public partial class TokStorage : BaseCollection, IEnumerable<TokStorageItem>
  {
    public TokStorage(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TokStorage"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public TokStorageItem this[int index]
    {
      get { return new TokStorageItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TokStorageItem tokStorageItem);
    partial void AfterRowInsert(TokStorageItem tokStorageItem);
    partial void BeforeRowEdit(TokStorageItem tokStorageItem);
    partial void AfterRowEdit(TokStorageItem tokStorageItem);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public TokStorageItemProxy[] GetTokStorageItemProxies()
    {
      List<TokStorageItemProxy> list = new List<TokStorageItemProxy>();

      foreach (TokStorageItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationID)
    {
      BeforeDBDelete(organizationID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TokStorage] WHERE ([OrganizationID] = @OrganizationID);";
        deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationID"].Value = organizationID;

        BeforeRowDelete(organizationID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(organizationID);
      }
      AfterDBDelete(organizationID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TokStorageSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TokStorage] SET     [AmazonPath] = @AmazonPath,    [CreatedDate] = @CreatedDate,    [ArchiveID] = @ArchiveID, [Transcoded] = @Transcoded  WHERE ([OrganizationID] = @OrganizationID);";

		
		tempParameter = updateCommand.Parameters.Add("Transcoded", SqlDbType.Bit, 1);
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
		
		tempParameter = updateCommand.Parameters.Add("AmazonPath", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatedDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ArchiveID", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TokStorage] (  [Transcoded],  [OrganizationID],    [AmazonPath],    [CreatedDate],    [CreatorID],    [ArchiveID]) VALUES ( @Transcoded, @OrganizationID, @AmazonPath, @CreatedDate, @CreatorID, @ArchiveID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ArchiveID", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatedDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("AmazonPath", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Transcoded", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TokStorage] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (TokStorageItem tokStorageItem in this)
		  {
			if (tokStorageItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(tokStorageItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = tokStorageItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				tokStorageItem.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(tokStorageItem);
			}
			else if (tokStorageItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(tokStorageItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = tokStorageItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(tokStorageItem);
			}
			else if (tokStorageItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)tokStorageItem.Row["OrganizationID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationID"].Value = id;
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

      foreach (TokStorageItem tokStorageItem in this)
      {
        if (tokStorageItem.Row.Table.Columns.Contains("CreatorID") && (int)tokStorageItem.Row["CreatorID"] == 0) tokStorageItem.Row["CreatorID"] = LoginUser.UserID;
        if (tokStorageItem.Row.Table.Columns.Contains("ModifierID")) tokStorageItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TokStorageItem FindByOrganizationID(int organizationID)
    {
      foreach (TokStorageItem tokStorageItem in this)
      {
        if (tokStorageItem.OrganizationID == organizationID)
        {
          return tokStorageItem;
        }
      }
      return null;
    }

    public virtual TokStorageItem AddNewTokStorageItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TokStorage");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TokStorageItem(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [AmazonPath], [CreatedDate], [CreatorID], [ArchiveID], [Transcoded] FROM [dbo].[TokStorage] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }


   
    public static TokStorageItem GetTokStorageItem(LoginUser loginUser, int organizationID)
    {
      TokStorage tokStorage = new TokStorage(loginUser);
      tokStorage.LoadByOrganizationID(organizationID);
      if (tokStorage.IsEmpty)
        return null;
      else
        return tokStorage[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TokStorageItem> Members

    public IEnumerator<TokStorageItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TokStorageItem(row, this);
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
