using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomPortalColumn : BaseItem
  {
    private CustomPortalColumns _customPortalColumns;
    
    public CustomPortalColumn(DataRow row, CustomPortalColumns customPortalColumns): base(row, customPortalColumns)
    {
      _customPortalColumns = customPortalColumns;
    }
	
    #region Properties
    
    public CustomPortalColumns Collection
    {
      get { return _customPortalColumns; }
    }
        
    
    
    
    public int CustomColumnID
    {
      get { return (int)Row["CustomColumnID"]; }
    }
    

    
    public int? StockFieldID
    {
      get { return Row["StockFieldID"] != DBNull.Value ? (int?)Row["StockFieldID"] : null; }
      set { Row["StockFieldID"] = CheckValue("StockFieldID", value); }
    }
    
    public int? CustomFieldID
    {
      get { return Row["CustomFieldID"] != DBNull.Value ? (int?)Row["CustomFieldID"] : null; }
      set { Row["CustomFieldID"] = CheckValue("CustomFieldID", value); }
    }
    

    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CustomPortalColumns : BaseCollection, IEnumerable<CustomPortalColumn>
  {
    public CustomPortalColumns(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomPortalColumns"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomColumnID"; }
    }



    public CustomPortalColumn this[int index]
    {
      get { return new CustomPortalColumn(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomPortalColumn customPortalColumn);
    partial void AfterRowInsert(CustomPortalColumn customPortalColumn);
    partial void BeforeRowEdit(CustomPortalColumn customPortalColumn);
    partial void AfterRowEdit(CustomPortalColumn customPortalColumn);
    partial void BeforeRowDelete(int customColumnID);
    partial void AfterRowDelete(int customColumnID);    

    partial void BeforeDBDelete(int customColumnID);
    partial void AfterDBDelete(int customColumnID);    

    #endregion

    #region Public Methods

    public CustomPortalColumnProxy[] GetCustomPortalColumnProxies()
    {
      List<CustomPortalColumnProxy> list = new List<CustomPortalColumnProxy>();

      foreach (CustomPortalColumn item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customColumnID)
    {
      BeforeDBDelete(customColumnID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomPortalColumns] WHERE ([CustomColumnID] = @CustomColumnID);";
        deleteCommand.Parameters.Add("CustomColumnID", SqlDbType.Int);
        deleteCommand.Parameters["CustomColumnID"].Value = customColumnID;

        BeforeRowDelete(customColumnID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(customColumnID);
      }
      AfterDBDelete(customColumnID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomPortalColumnsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomPortalColumns] SET     [OrganizationID] = @OrganizationID,    [Position] = @Position,    [StockFieldID] = @StockFieldID,    [CustomFieldID] = @CustomFieldID  WHERE ([CustomColumnID] = @CustomColumnID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomColumnID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("StockFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomPortalColumns] (    [OrganizationID],    [Position],    [StockFieldID],    [CustomFieldID]) VALUES ( @OrganizationID, @Position, @StockFieldID, @CustomFieldID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("CustomFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("StockFieldID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomPortalColumns] WHERE ([CustomColumnID] = @CustomColumnID);";
		deleteCommand.Parameters.Add("CustomColumnID", SqlDbType.Int);

		try
		{
		  foreach (CustomPortalColumn customPortalColumn in this)
		  {
			if (customPortalColumn.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customPortalColumn);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customPortalColumn.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomColumnID"].AutoIncrement = false;
			  Table.Columns["CustomColumnID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customPortalColumn.Row["CustomColumnID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customPortalColumn);
			}
			else if (customPortalColumn.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customPortalColumn);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customPortalColumn.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customPortalColumn);
			}
			else if (customPortalColumn.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customPortalColumn.Row["CustomColumnID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomColumnID"].Value = id;
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

      foreach (CustomPortalColumn customPortalColumn in this)
      {
        if (customPortalColumn.Row.Table.Columns.Contains("CreatorID") && (int)customPortalColumn.Row["CreatorID"] == 0) customPortalColumn.Row["CreatorID"] = LoginUser.UserID;
        if (customPortalColumn.Row.Table.Columns.Contains("ModifierID")) customPortalColumn.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomPortalColumn FindByStockFieldID(int StockFieldID)
    {
        foreach (CustomPortalColumn customPortalColumn in this)
        {
            if (customPortalColumn.StockFieldID == StockFieldID)
            {
                return customPortalColumn;
            }
        }
        return null;
    }

    public CustomPortalColumn FindByCustomFieldID(int CustomFieldID)
    {
        foreach (CustomPortalColumn customPortalColumn in this)
        {
            if (customPortalColumn.CustomFieldID == CustomFieldID)
            {
                return customPortalColumn;
            }
        }
        return null;
    }

    public CustomPortalColumn FindByCustomColumnID(int customColumnID)
    {
      foreach (CustomPortalColumn customPortalColumn in this)
      {
        if (customPortalColumn.CustomColumnID == customColumnID)
        {
          return customPortalColumn;
        }
      }
      return null;
    }

    public virtual CustomPortalColumn AddNewCustomPortalColumn()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomPortalColumns");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomPortalColumn(row, this);
    }

    public virtual void LoadByStockFieldID(int StockFieldID, int OrganizationID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SET NOCOUNT OFF; SELECT [CustomColumnID], [OrganizationID], [Position], [StockFieldID], [CustomFieldID] FROM [dbo].[CustomPortalColumns] WHERE ([StockFieldID] = @StockFieldID and [OrganizationID] = @OrganizationID);";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("OrganizationID", OrganizationID);
            command.Parameters.AddWithValue("StockFieldID", StockFieldID);
            Fill(command);
        }
    }

    public virtual void LoadByCustomFieldID(int CustomFieldID, int OrganizationID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SET NOCOUNT OFF; SELECT [CustomColumnID], [OrganizationID], [Position], [StockFieldID], [CustomFieldID] FROM [dbo].[CustomPortalColumns] WHERE ([CustomFieldID] = @CustomFieldID and [OrganizationID] = @OrganizationID);";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("OrganizationID", OrganizationID);
            command.Parameters.AddWithValue("CustomFieldID", CustomFieldID);
            Fill(command);
        }
    }

    public virtual void LoadByCustomColumnID(int customColumnID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomColumnID], [OrganizationID], [Position], [StockFieldID], [CustomFieldID] FROM [dbo].[CustomPortalColumns] WHERE ([CustomColumnID] = @CustomColumnID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomColumnID", customColumnID);
        Fill(command);
      }
    }

    public virtual void LoadByOrganizationID(int organizationID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SET NOCOUNT OFF; SELECT [CustomColumnID], [OrganizationID], [Position], [StockFieldID], [CustomFieldID] FROM [dbo].[CustomPortalColumns] WHERE ([OrganizationID] = @OrganizationID) order by position;";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("OrganizationID", organizationID);
            Fill(command);
        }
    }

    public static CustomPortalColumn GetCustomPortalColumn(LoginUser loginUser, int customColumnID)
    {
      CustomPortalColumns customPortalColumns = new CustomPortalColumns(loginUser);
      customPortalColumns.LoadByCustomColumnID(customColumnID);
      if (customPortalColumns.IsEmpty)
        return null;
      else
        return customPortalColumns[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomPortalColumn> Members

    public IEnumerator<CustomPortalColumn> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomPortalColumn(row, this);
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
