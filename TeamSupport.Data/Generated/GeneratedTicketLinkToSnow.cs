using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketLinkToSnowItem : BaseItem
  {
    private TicketLinkToSnow _ticketLinkToSnow;
    
    public TicketLinkToSnowItem(DataRow row, TicketLinkToSnow ticketLinkToSnow): base(row, ticketLinkToSnow)
    {
      _ticketLinkToSnow = ticketLinkToSnow;
    }
	
    #region Properties
    
    public TicketLinkToSnow Collection
    {
      get { return _ticketLinkToSnow; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    
    public string AppId
    {
      get { return Row["AppId"] != DBNull.Value ? (string)Row["AppId"] : null; }
      set { Row["AppId"] = CheckValue("AppId", value); }
    }
    
    public string Number
    {
      get { return Row["Number"] != DBNull.Value ? (string)Row["Number"] : null; }
      set { Row["Number"] = CheckValue("Number", value); }
    }
    
    public string URL
    {
      get { return Row["URL"] != DBNull.Value ? (string)Row["URL"] : null; }
      set { Row["URL"] = CheckValue("URL", value); }
    }
    
    public string State
    {
      get { return Row["State"] != DBNull.Value ? (string)Row["State"] : null; }
      set { Row["State"] = CheckValue("State", value); }
    }
    
    public int? CreatorID
    {
      get { return Row["CreatorID"] != DBNull.Value ? (int?)Row["CreatorID"] : null; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int? CrmLinkID
    {
      get { return Row["CrmLinkID"] != DBNull.Value ? (int?)Row["CrmLinkID"] : null; }
      set { Row["CrmLinkID"] = CheckValue("CrmLinkID", value); }
    }
    

    
    public bool Sync
    {
      get { return (bool)Row["Sync"]; }
      set { Row["Sync"] = CheckValue("Sync", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateModifiedBySync
    {
      get { return Row["DateModifiedBySync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedBySync"]) : null; }
      set { Row["DateModifiedBySync"] = CheckValue("DateModifiedBySync", value); }
    }

    public DateTime? DateModifiedBySyncUtc
    {
      get { return Row["DateModifiedBySync"] != DBNull.Value ? (DateTime?)Row["DateModifiedBySync"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class TicketLinkToSnow : BaseCollection, IEnumerable<TicketLinkToSnowItem>
  {
    public TicketLinkToSnow(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketLinkToSnow"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public TicketLinkToSnowItem this[int index]
    {
      get { return new TicketLinkToSnowItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketLinkToSnowItem ticketLinkToSnowItem);
    partial void AfterRowInsert(TicketLinkToSnowItem ticketLinkToSnowItem);
    partial void BeforeRowEdit(TicketLinkToSnowItem ticketLinkToSnowItem);
    partial void AfterRowEdit(TicketLinkToSnowItem ticketLinkToSnowItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public TicketLinkToSnowItemProxy[] GetTicketLinkToSnowItemProxies()
    {
      List<TicketLinkToSnowItemProxy> list = new List<TicketLinkToSnowItemProxy>();

      foreach (TicketLinkToSnowItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketLinkToSnow] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketLinkToSnowSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketLinkToSnow] SET     [TicketID] = @TicketID,    [DateModifiedBySync] = @DateModifiedBySync,    [Sync] = @Sync,    [AppId] = @AppId,    [Number] = @Number,    [URL] = @URL,    [State] = @State,    [CrmLinkID] = @CrmLinkID  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedBySync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("Sync", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AppId", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Number", SqlDbType.VarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("URL", SqlDbType.VarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("State", SqlDbType.VarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CrmLinkID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketLinkToSnow] (    [TicketID],    [DateModifiedBySync],    [Sync],    [AppId],    [Number],    [URL],    [State],    [CreatorID],    [CrmLinkID]) VALUES ( @TicketID, @DateModifiedBySync, @Sync, @AppId, @Number, @URL, @State, @CreatorID, @CrmLinkID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("CrmLinkID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("State", SqlDbType.VarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("URL", SqlDbType.VarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Number", SqlDbType.VarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AppId", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Sync", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedBySync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketLinkToSnow] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (TicketLinkToSnowItem ticketLinkToSnowItem in this)
		  {
			if (ticketLinkToSnowItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketLinkToSnowItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketLinkToSnowItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketLinkToSnowItem.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketLinkToSnowItem);
			}
			else if (ticketLinkToSnowItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketLinkToSnowItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketLinkToSnowItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketLinkToSnowItem);
			}
			else if (ticketLinkToSnowItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketLinkToSnowItem.Row["Id", DataRowVersion.Original];
			  deleteCommand.Parameters["Id"].Value = id;
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

      foreach (TicketLinkToSnowItem ticketLinkToSnowItem in this)
      {
        if (ticketLinkToSnowItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketLinkToSnowItem.Row["CreatorID"] == 0) ticketLinkToSnowItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketLinkToSnowItem.Row.Table.Columns.Contains("ModifierID")) ticketLinkToSnowItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketLinkToSnowItem FindById(int id)
    {
      foreach (TicketLinkToSnowItem ticketLinkToSnowItem in this)
      {
        if (ticketLinkToSnowItem.Id == id)
        {
          return ticketLinkToSnowItem;
        }
      }
      return null;
    }

    public virtual TicketLinkToSnowItem AddNewTicketLinkToSnowItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketLinkToSnow");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketLinkToSnowItem(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [TicketID], [DateModifiedBySync], [Sync], [AppId], [Number], [URL], [State], [CreatorID], [CrmLinkID] FROM [dbo].[TicketLinkToSnow] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static TicketLinkToSnowItem GetTicketLinkToSnowItem(LoginUser loginUser, int id)
    {
      TicketLinkToSnow ticketLinkToSnow = new TicketLinkToSnow(loginUser);
      ticketLinkToSnow.LoadById(id);
      if (ticketLinkToSnow.IsEmpty)
        return null;
      else
        return ticketLinkToSnow[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketLinkToSnowItem> Members

    public IEnumerator<TicketLinkToSnowItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketLinkToSnowItem(row, this);
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
