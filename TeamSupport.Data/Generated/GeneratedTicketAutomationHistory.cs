using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketAutomationHistoryItem : BaseItem
  {
    private TicketAutomationHistory _ticketAutomationHistory;
    
    public TicketAutomationHistoryItem(DataRow row, TicketAutomationHistory ticketAutomationHistory): base(row, ticketAutomationHistory)
    {
      _ticketAutomationHistory = ticketAutomationHistory;
    }
	
    #region Properties
    
    public TicketAutomationHistory Collection
    {
      get { return _ticketAutomationHistory; }
    }
        
    
    
    
    public int HistoryID
    {
      get { return (int)Row["HistoryID"]; }
    }
    

    
    public string ActionType
    {
      get { return Row["ActionType"] != DBNull.Value ? (string)Row["ActionType"] : null; }
      set { Row["ActionType"] = CheckValue("ActionType", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int TriggerID
    {
      get { return (int)Row["TriggerID"]; }
      set { Row["TriggerID"] = CheckValue("TriggerID", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? TriggerDateTime
    {
      get { return Row["TriggerDateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["TriggerDateTime"]) : null; }
      set { Row["TriggerDateTime"] = CheckValue("TriggerDateTime", value); }
    }

    public DateTime? TriggerDateTimeUtc
    {
      get { return Row["TriggerDateTime"] != DBNull.Value ? (DateTime?)Row["TriggerDateTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class TicketAutomationHistory : BaseCollection, IEnumerable<TicketAutomationHistoryItem>
  {
    public TicketAutomationHistory(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketAutomationHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "HistoryID"; }
    }



    public TicketAutomationHistoryItem this[int index]
    {
      get { return new TicketAutomationHistoryItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketAutomationHistoryItem ticketAutomationHistoryItem);
    partial void AfterRowInsert(TicketAutomationHistoryItem ticketAutomationHistoryItem);
    partial void BeforeRowEdit(TicketAutomationHistoryItem ticketAutomationHistoryItem);
    partial void AfterRowEdit(TicketAutomationHistoryItem ticketAutomationHistoryItem);
    partial void BeforeRowDelete(int historyID);
    partial void AfterRowDelete(int historyID);    

    partial void BeforeDBDelete(int historyID);
    partial void AfterDBDelete(int historyID);    

    #endregion

    #region Public Methods

    public TicketAutomationHistoryItemProxy[] GetTicketAutomationHistoryItemProxies()
    {
      List<TicketAutomationHistoryItemProxy> list = new List<TicketAutomationHistoryItemProxy>();

      foreach (TicketAutomationHistoryItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int historyID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationHistory] WHERE ([HistoryID] = @HistoryID);";
        deleteCommand.Parameters.Add("HistoryID", SqlDbType.Int);
        deleteCommand.Parameters["HistoryID"].Value = historyID;

        BeforeDBDelete(historyID);
        BeforeRowDelete(historyID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(historyID);
        AfterDBDelete(historyID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketAutomationHistorySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketAutomationHistory] SET     [TicketID] = @TicketID,    [TriggerID] = @TriggerID,    [OrganizationID] = @OrganizationID,    [TriggerDateTime] = @TriggerDateTime,    [ActionType] = @ActionType  WHERE ([HistoryID] = @HistoryID);";

		
		tempParameter = updateCommand.Parameters.Add("HistoryID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("TriggerID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("TriggerDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketAutomationHistory] (    [TicketID],    [TriggerID],    [OrganizationID],    [TriggerDateTime],    [ActionType]) VALUES ( @TicketID, @TriggerID, @OrganizationID, @TriggerDateTime, @ActionType); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ActionType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TriggerDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TriggerID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationHistory] WHERE ([HistoryID] = @HistoryID);";
		deleteCommand.Parameters.Add("HistoryID", SqlDbType.Int);

		try
		{
		  foreach (TicketAutomationHistoryItem ticketAutomationHistoryItem in this)
		  {
			if (ticketAutomationHistoryItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketAutomationHistoryItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketAutomationHistoryItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["HistoryID"].AutoIncrement = false;
			  Table.Columns["HistoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketAutomationHistoryItem.Row["HistoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketAutomationHistoryItem);
			}
			else if (ticketAutomationHistoryItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketAutomationHistoryItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketAutomationHistoryItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketAutomationHistoryItem);
			}
			else if (ticketAutomationHistoryItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketAutomationHistoryItem.Row["HistoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["HistoryID"].Value = id;
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

      foreach (TicketAutomationHistoryItem ticketAutomationHistoryItem in this)
      {
        if (ticketAutomationHistoryItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketAutomationHistoryItem.Row["CreatorID"] == 0) ticketAutomationHistoryItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketAutomationHistoryItem.Row.Table.Columns.Contains("ModifierID")) ticketAutomationHistoryItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketAutomationHistoryItem FindByHistoryID(int historyID)
    {
      foreach (TicketAutomationHistoryItem ticketAutomationHistoryItem in this)
      {
        if (ticketAutomationHistoryItem.HistoryID == historyID)
        {
          return ticketAutomationHistoryItem;
        }
      }
      return null;
    }

    public virtual TicketAutomationHistoryItem AddNewTicketAutomationHistoryItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketAutomationHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketAutomationHistoryItem(row, this);
    }
    
    public virtual void LoadByHistoryID(int historyID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [HistoryID], [TicketID], [TriggerID], [OrganizationID], [TriggerDateTime], [ActionType] FROM [dbo].[TicketAutomationHistory] WHERE ([HistoryID] = @HistoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("HistoryID", historyID);
        Fill(command);
      }
    }
    
    public static TicketAutomationHistoryItem GetTicketAutomationHistoryItem(LoginUser loginUser, int historyID)
    {
      TicketAutomationHistory ticketAutomationHistory = new TicketAutomationHistory(loginUser);
      ticketAutomationHistory.LoadByHistoryID(historyID);
      if (ticketAutomationHistory.IsEmpty)
        return null;
      else
        return ticketAutomationHistory[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketAutomationHistoryItem> Members

    public IEnumerator<TicketAutomationHistoryItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketAutomationHistoryItem(row, this);
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
