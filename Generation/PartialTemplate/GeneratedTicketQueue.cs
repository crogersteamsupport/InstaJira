using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketQueueItem : BaseItem
  {
    private TicketQueue _ticketQueue;
    
    public TicketQueueItem(DataRow row, TicketQueue ticketQueue): base(row, ticketQueue)
    {
      _ticketQueue = ticketQueue;
    }
	
    #region Properties
    
    public TicketQueue Collection
    {
      get { return _ticketQueue; }
    }
        
    
    
    
    public int TicketQueueID
    {
      get { return (int)Row["TicketQueueID"]; }
    }
    

    
    public decimal? EstimatedDays
    {
      get { return Row["EstimatedDays"] != DBNull.Value ? (decimal?)Row["EstimatedDays"] : null; }
      set { Row["EstimatedDays"] = CheckValue("EstimatedDays", value); }
    }
    

    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
    }
    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    

    #endregion
    
    
  }

  public partial class TicketQueue : BaseCollection, IEnumerable<TicketQueueItem>
  {
    public TicketQueue(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketQueue"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketQueueID"; }
    }



    public TicketQueueItem this[int index]
    {
      get { return new TicketQueueItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketQueueItem ticketQueueItem);
    partial void AfterRowInsert(TicketQueueItem ticketQueueItem);
    partial void BeforeRowEdit(TicketQueueItem ticketQueueItem);
    partial void AfterRowEdit(TicketQueueItem ticketQueueItem);
    partial void BeforeRowDelete(int ticketQueueID);
    partial void AfterRowDelete(int ticketQueueID);    

    partial void BeforeDBDelete(int ticketQueueID);
    partial void AfterDBDelete(int ticketQueueID);    

    #endregion

    #region Public Methods

    public TicketQueueItemProxy[] GetTicketQueueItemProxies()
    {
      List<TicketQueueItemProxy> list = new List<TicketQueueItemProxy>();

      foreach (TicketQueueItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketQueueID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketQueue] WHERE ([TicketQueueID] = @TicketQueueID);";
        deleteCommand.Parameters.Add("TicketQueueID", SqlDbType.Int);
        deleteCommand.Parameters["TicketQueueID"].Value = ticketQueueID;

        BeforeDBDelete(ticketQueueID);
        BeforeRowDelete(ticketQueueID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketQueueID);
        AfterDBDelete(ticketQueueID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketQueueSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketQueue] SET     [TicketID] = @TicketID,    [UserID] = @UserID,    [EstimatedDays] = @EstimatedDays,    [Position] = @Position,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([TicketQueueID] = @TicketQueueID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketQueueID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("EstimatedDays", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketQueue] (    [TicketID],    [UserID],    [EstimatedDays],    [Position],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @TicketID, @UserID, @EstimatedDays, @Position, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EstimatedDays", SqlDbType.Decimal, 17);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 18;
		  tempParameter.Scale = 18;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketQueue] WHERE ([TicketQueueID] = @TicketQueueID);";
		deleteCommand.Parameters.Add("TicketQueueID", SqlDbType.Int);

		try
		{
		  foreach (TicketQueueItem ticketQueueItem in this)
		  {
			if (ticketQueueItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketQueueItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketQueueItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketQueueID"].AutoIncrement = false;
			  Table.Columns["TicketQueueID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketQueueItem.Row["TicketQueueID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketQueueItem);
			}
			else if (ticketQueueItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketQueueItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketQueueItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketQueueItem);
			}
			else if (ticketQueueItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketQueueItem.Row["TicketQueueID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketQueueID"].Value = id;
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

      foreach (TicketQueueItem ticketQueueItem in this)
      {
        if (ticketQueueItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketQueueItem.Row["CreatorID"] == 0) ticketQueueItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketQueueItem.Row.Table.Columns.Contains("ModifierID")) ticketQueueItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketQueueItem FindByTicketQueueID(int ticketQueueID)
    {
      foreach (TicketQueueItem ticketQueueItem in this)
      {
        if (ticketQueueItem.TicketQueueID == ticketQueueID)
        {
          return ticketQueueItem;
        }
      }
      return null;
    }

    public virtual TicketQueueItem AddNewTicketQueueItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketQueue");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketQueueItem(row, this);
    }
    
    public virtual void LoadByTicketQueueID(int ticketQueueID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketQueueID], [TicketID], [UserID], [EstimatedDays], [Position], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[TicketQueue] WHERE ([TicketQueueID] = @TicketQueueID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketQueueID", ticketQueueID);
        Fill(command);
      }
    }
    
    public static TicketQueueItem GetTicketQueueItem(LoginUser loginUser, int ticketQueueID)
    {
      TicketQueue ticketQueue = new TicketQueue(loginUser);
      ticketQueue.LoadByTicketQueueID(ticketQueueID);
      if (ticketQueue.IsEmpty)
        return null;
      else
        return ticketQueue[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketQueueItem> Members

    public IEnumerator<TicketQueueItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketQueueItem(row, this);
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
