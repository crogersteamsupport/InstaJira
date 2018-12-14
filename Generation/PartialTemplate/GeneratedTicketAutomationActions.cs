using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketAutomationAction : BaseItem
  {
    private TicketAutomationActions _ticketAutomationActions;
    
    public TicketAutomationAction(DataRow row, TicketAutomationActions ticketAutomationActions): base(row, ticketAutomationActions)
    {
      _ticketAutomationActions = ticketAutomationActions;
    }
	
    #region Properties
    
    public TicketAutomationActions Collection
    {
      get { return _ticketAutomationActions; }
    }
        
    
    
    
    public int TicketActionID
    {
      get { return (int)Row["TicketActionID"]; }
    }
    

    
    public string ActionValue
    {
      get { return Row["ActionValue"] != DBNull.Value ? (string)Row["ActionValue"] : null; }
      set { Row["ActionValue"] = CheckValue("ActionValue", value); }
    }
    
    public string ActionValue2
    {
      get { return Row["ActionValue2"] != DBNull.Value ? (string)Row["ActionValue2"] : null; }
      set { Row["ActionValue2"] = CheckValue("ActionValue2", value); }
    }
    

    
    public int ActionID
    {
      get { return (int)Row["ActionID"]; }
      set { Row["ActionID"] = CheckValue("ActionID", value); }
    }
    
    public int TriggerID
    {
      get { return (int)Row["TriggerID"]; }
      set { Row["TriggerID"] = CheckValue("TriggerID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class TicketAutomationActions : BaseCollection, IEnumerable<TicketAutomationAction>
  {
    public TicketAutomationActions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketAutomationActions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketActionID"; }
    }



    public TicketAutomationAction this[int index]
    {
      get { return new TicketAutomationAction(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketAutomationAction ticketAutomationAction);
    partial void AfterRowInsert(TicketAutomationAction ticketAutomationAction);
    partial void BeforeRowEdit(TicketAutomationAction ticketAutomationAction);
    partial void AfterRowEdit(TicketAutomationAction ticketAutomationAction);
    partial void BeforeRowDelete(int ticketActionID);
    partial void AfterRowDelete(int ticketActionID);    

    partial void BeforeDBDelete(int ticketActionID);
    partial void AfterDBDelete(int ticketActionID);    

    #endregion

    #region Public Methods

    public TicketAutomationActionProxy[] GetTicketAutomationActionProxies()
    {
      List<TicketAutomationActionProxy> list = new List<TicketAutomationActionProxy>();

      foreach (TicketAutomationAction item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketActionID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationActions] WHERE ([TicketActionID] = @TicketActionID);";
        deleteCommand.Parameters.Add("TicketActionID", SqlDbType.Int);
        deleteCommand.Parameters["TicketActionID"].Value = ticketActionID;

        BeforeDBDelete(ticketActionID);
        BeforeRowDelete(ticketActionID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketActionID);
        AfterDBDelete(ticketActionID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketAutomationActionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketAutomationActions] SET     [TriggerID] = @TriggerID,    [ActionID] = @ActionID,    [ActionValue] = @ActionValue,    [ActionValue2] = @ActionValue2  WHERE ([TicketActionID] = @TicketActionID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketActionID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionValue", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionValue2", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketAutomationActions] (    [TriggerID],    [ActionID],    [ActionValue],    [ActionValue2]) VALUES ( @TriggerID, @ActionID, @ActionValue, @ActionValue2); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ActionValue2", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionValue", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationActions] WHERE ([TicketActionID] = @TicketActionID);";
		deleteCommand.Parameters.Add("TicketActionID", SqlDbType.Int);

		try
		{
		  foreach (TicketAutomationAction ticketAutomationAction in this)
		  {
			if (ticketAutomationAction.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketAutomationAction);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketAutomationAction.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketActionID"].AutoIncrement = false;
			  Table.Columns["TicketActionID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketAutomationAction.Row["TicketActionID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketAutomationAction);
			}
			else if (ticketAutomationAction.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketAutomationAction);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketAutomationAction.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketAutomationAction);
			}
			else if (ticketAutomationAction.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketAutomationAction.Row["TicketActionID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketActionID"].Value = id;
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

      foreach (TicketAutomationAction ticketAutomationAction in this)
      {
        if (ticketAutomationAction.Row.Table.Columns.Contains("CreatorID") && (int)ticketAutomationAction.Row["CreatorID"] == 0) ticketAutomationAction.Row["CreatorID"] = LoginUser.UserID;
        if (ticketAutomationAction.Row.Table.Columns.Contains("ModifierID")) ticketAutomationAction.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketAutomationAction FindByTicketActionID(int ticketActionID)
    {
      foreach (TicketAutomationAction ticketAutomationAction in this)
      {
        if (ticketAutomationAction.TicketActionID == ticketActionID)
        {
          return ticketAutomationAction;
        }
      }
      return null;
    }

    public virtual TicketAutomationAction AddNewTicketAutomationAction()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketAutomationActions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketAutomationAction(row, this);
    }
    
    public virtual void LoadByTicketActionID(int ticketActionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketActionID], [TriggerID], [ActionID], [ActionValue], [ActionValue2] FROM [dbo].[TicketAutomationActions] WHERE ([TicketActionID] = @TicketActionID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketActionID", ticketActionID);
        Fill(command);
      }
    }
    
    public static TicketAutomationAction GetTicketAutomationAction(LoginUser loginUser, int ticketActionID)
    {
      TicketAutomationActions ticketAutomationActions = new TicketAutomationActions(loginUser);
      ticketAutomationActions.LoadByTicketActionID(ticketActionID);
      if (ticketAutomationActions.IsEmpty)
        return null;
      else
        return ticketAutomationActions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketAutomationAction> Members

    public IEnumerator<TicketAutomationAction> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketAutomationAction(row, this);
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
