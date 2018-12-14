using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketAutomationPossibleAction : BaseItem
  {
    private TicketAutomationPossibleActions _ticketAutomationPossibleActions;
    
    public TicketAutomationPossibleAction(DataRow row, TicketAutomationPossibleActions ticketAutomationPossibleActions): base(row, ticketAutomationPossibleActions)
    {
      _ticketAutomationPossibleActions = ticketAutomationPossibleActions;
    }
	
    #region Properties
    
    public TicketAutomationPossibleActions Collection
    {
      get { return _ticketAutomationPossibleActions; }
    }
        
    
    
    
    public int ActionID
    {
      get { return (int)Row["ActionID"]; }
    }
    

    
    public string ActionName
    {
      get { return Row["ActionName"] != DBNull.Value ? (string)Row["ActionName"] : null; }
      set { Row["ActionName"] = CheckValue("ActionName", value); }
    }
    
    public string ValueList
    {
      get { return Row["ValueList"] != DBNull.Value ? (string)Row["ValueList"] : null; }
      set { Row["ValueList"] = CheckValue("ValueList", value); }
    }
    
    public string ValueList2
    {
      get { return Row["ValueList2"] != DBNull.Value ? (string)Row["ValueList2"] : null; }
      set { Row["ValueList2"] = CheckValue("ValueList2", value); }
    }
    

    
    public bool Active
    {
      get { return (bool)Row["Active"]; }
      set { Row["Active"] = CheckValue("Active", value); }
    }
    
    public bool RequireValue
    {
      get { return (bool)Row["RequireValue"]; }
      set { Row["RequireValue"] = CheckValue("RequireValue", value); }
    }
    
    public string DisplayName
    {
      get { return (string)Row["DisplayName"]; }
      set { Row["DisplayName"] = CheckValue("DisplayName", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class TicketAutomationPossibleActions : BaseCollection, IEnumerable<TicketAutomationPossibleAction>
  {
    public TicketAutomationPossibleActions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketAutomationPossibleActions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ActionID"; }
    }



    public TicketAutomationPossibleAction this[int index]
    {
      get { return new TicketAutomationPossibleAction(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketAutomationPossibleAction ticketAutomationPossibleAction);
    partial void AfterRowInsert(TicketAutomationPossibleAction ticketAutomationPossibleAction);
    partial void BeforeRowEdit(TicketAutomationPossibleAction ticketAutomationPossibleAction);
    partial void AfterRowEdit(TicketAutomationPossibleAction ticketAutomationPossibleAction);
    partial void BeforeRowDelete(int actionID);
    partial void AfterRowDelete(int actionID);    

    partial void BeforeDBDelete(int actionID);
    partial void AfterDBDelete(int actionID);    

    #endregion

    #region Public Methods

    public TicketAutomationPossibleActionProxy[] GetTicketAutomationPossibleActionProxies()
    {
      List<TicketAutomationPossibleActionProxy> list = new List<TicketAutomationPossibleActionProxy>();

      foreach (TicketAutomationPossibleAction item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int actionID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationPossibleActions] WHERE ([ActionID] = @ActionID);";
        deleteCommand.Parameters.Add("ActionID", SqlDbType.Int);
        deleteCommand.Parameters["ActionID"].Value = actionID;

        BeforeDBDelete(actionID);
        BeforeRowDelete(actionID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(actionID);
        AfterDBDelete(actionID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketAutomationPossibleActionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketAutomationPossibleActions] SET     [DisplayName] = @DisplayName,    [ActionName] = @ActionName,    [RequireValue] = @RequireValue,    [ValueList] = @ValueList,    [ValueList2] = @ValueList2,    [Active] = @Active  WHERE ([ActionID] = @ActionID);";

		
		tempParameter = updateCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DisplayName", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequireValue", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ValueList", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ValueList2", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Active", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketAutomationPossibleActions] (    [DisplayName],    [ActionName],    [RequireValue],    [ValueList],    [ValueList2],    [Active]) VALUES ( @DisplayName, @ActionName, @RequireValue, @ValueList, @ValueList2, @Active); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Active", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ValueList2", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ValueList", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequireValue", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DisplayName", SqlDbType.VarChar, 200);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationPossibleActions] WHERE ([ActionID] = @ActionID);";
		deleteCommand.Parameters.Add("ActionID", SqlDbType.Int);

		try
		{
		  foreach (TicketAutomationPossibleAction ticketAutomationPossibleAction in this)
		  {
			if (ticketAutomationPossibleAction.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketAutomationPossibleAction);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketAutomationPossibleAction.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ActionID"].AutoIncrement = false;
			  Table.Columns["ActionID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketAutomationPossibleAction.Row["ActionID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketAutomationPossibleAction);
			}
			else if (ticketAutomationPossibleAction.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketAutomationPossibleAction);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketAutomationPossibleAction.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketAutomationPossibleAction);
			}
			else if (ticketAutomationPossibleAction.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketAutomationPossibleAction.Row["ActionID", DataRowVersion.Original];
			  deleteCommand.Parameters["ActionID"].Value = id;
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

      foreach (TicketAutomationPossibleAction ticketAutomationPossibleAction in this)
      {
        if (ticketAutomationPossibleAction.Row.Table.Columns.Contains("CreatorID") && (int)ticketAutomationPossibleAction.Row["CreatorID"] == 0) ticketAutomationPossibleAction.Row["CreatorID"] = LoginUser.UserID;
        if (ticketAutomationPossibleAction.Row.Table.Columns.Contains("ModifierID")) ticketAutomationPossibleAction.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketAutomationPossibleAction FindByActionID(int actionID)
    {
      foreach (TicketAutomationPossibleAction ticketAutomationPossibleAction in this)
      {
        if (ticketAutomationPossibleAction.ActionID == actionID)
        {
          return ticketAutomationPossibleAction;
        }
      }
      return null;
    }

    public virtual TicketAutomationPossibleAction AddNewTicketAutomationPossibleAction()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketAutomationPossibleActions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketAutomationPossibleAction(row, this);
    }
    
    public virtual void LoadByActionID(int actionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ActionID], [DisplayName], [ActionName], [RequireValue], [ValueList], [ValueList2], [Active] FROM [dbo].[TicketAutomationPossibleActions] WHERE ([ActionID] = @ActionID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ActionID", actionID);
        Fill(command);
      }
    }
    
    public static TicketAutomationPossibleAction GetTicketAutomationPossibleAction(LoginUser loginUser, int actionID)
    {
      TicketAutomationPossibleActions ticketAutomationPossibleActions = new TicketAutomationPossibleActions(loginUser);
      ticketAutomationPossibleActions.LoadByActionID(actionID);
      if (ticketAutomationPossibleActions.IsEmpty)
        return null;
      else
        return ticketAutomationPossibleActions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketAutomationPossibleAction> Members

    public IEnumerator<TicketAutomationPossibleAction> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketAutomationPossibleAction(row, this);
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
