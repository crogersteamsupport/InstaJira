using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Action : BaseItem
  {
    private Actions _actions;
    
    public Action(DataRow row, Actions actions): base(row, actions)
    {
      _actions = actions;
    }
	
    #region Properties
    
    public Actions Collection
    {
      get { return _actions; }
    }
        
    
    
    
    public int ActionID
    {
      get { return (int)Row["ActionID"]; }
    }
    

    
    public int? ActionTypeID
    {
      get { return Row["ActionTypeID"] != DBNull.Value ? (int?)Row["ActionTypeID"] : null; }
      set { Row["ActionTypeID"] = CheckValue("ActionTypeID", value); }
    }
    
    public int? TimeSpent
    {
      get { return Row["TimeSpent"] != DBNull.Value ? (int?)Row["TimeSpent"] : null; }
      set { Row["TimeSpent"] = CheckValue("TimeSpent", value); }
    }
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }
    
    public string ActionSource
    {
      get { return Row["ActionSource"] != DBNull.Value ? (string)Row["ActionSource"] : null; }
      set { Row["ActionSource"] = CheckValue("ActionSource", value); }
    }
    
    public string SalesForceID
    {
      get { return Row["SalesForceID"] != DBNull.Value ? (string)Row["SalesForceID"] : null; }
      set { Row["SalesForceID"] = CheckValue("SalesForceID", value); }
    }
    
    public int? JiraID
    {
      get { return Row["JiraID"] != DBNull.Value ? (int?)Row["JiraID"] : null; }
      set { Row["JiraID"] = CheckValue("JiraID", value); }
    }
    
    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
    }
    

    
    public bool IsClean
    {
      get { return (bool)Row["IsClean"]; }
      set { Row["IsClean"] = CheckValue("IsClean", value); }
    }
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public bool Pinned
    {
      get { return (bool)Row["Pinned"]; }
      set { Row["Pinned"] = CheckValue("Pinned", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
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
    
    public bool IsKnowledgeBase
    {
      get { return (bool)Row["IsKnowledgeBase"]; }
      set { Row["IsKnowledgeBase"] = CheckValue("IsKnowledgeBase", value); }
    }
    
    public bool IsVisibleOnPortal
    {
      get { return (bool)Row["IsVisibleOnPortal"]; }
      set { Row["IsVisibleOnPortal"] = CheckValue("IsVisibleOnPortal", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public SystemActionType SystemActionTypeID
    {
      get { return (SystemActionType)Row["SystemActionTypeID"]; }
      set { Row["SystemActionTypeID"] = CheckValue("SystemActionTypeID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateStarted
    {
      get { return Row["DateStarted"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateStarted"]) : null; }
      set { Row["DateStarted"] = CheckValue("DateStarted", value); }
    }

    public DateTime? DateStartedUtc
    {
      get { return Row["DateStarted"] != DBNull.Value ? (DateTime?)Row["DateStarted"] : null; }
    }
    
    public DateTime? DateModifiedBySalesForceSync
    {
      get { return Row["DateModifiedBySalesForceSync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedBySalesForceSync"]) : null; }
      set { Row["DateModifiedBySalesForceSync"] = CheckValue("DateModifiedBySalesForceSync", value); }
    }

    public DateTime? DateModifiedBySalesForceSyncUtc
    {
      get { return Row["DateModifiedBySalesForceSync"] != DBNull.Value ? (DateTime?)Row["DateModifiedBySalesForceSync"] : null; }
    }
    
    public DateTime? DateModifiedByJiraSync
    {
      get { return Row["DateModifiedByJiraSync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedByJiraSync"]) : null; }
      set { Row["DateModifiedByJiraSync"] = CheckValue("DateModifiedByJiraSync", value); }
    }

    public DateTime? DateModifiedByJiraSyncUtc
    {
      get { return Row["DateModifiedByJiraSync"] != DBNull.Value ? (DateTime?)Row["DateModifiedByJiraSync"] : null; }
    }
    

    
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

  public partial class Actions : BaseCollection, IEnumerable<Action>
  {
    public Actions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Actions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ActionID"; }
    }



    public Action this[int index]
    {
      get { return new Action(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Action action);
    partial void AfterRowInsert(Action action);
    partial void BeforeRowEdit(Action action);
    partial void AfterRowEdit(Action action);
    partial void BeforeRowDelete(int actionID);
    partial void AfterRowDelete(int actionID);    

    partial void BeforeDBDelete(int actionID);
    partial void AfterDBDelete(int actionID);    

    #endregion

    #region Public Methods

    public ActionProxy[] GetActionProxies()
    {
      List<ActionProxy> list = new List<ActionProxy>();

      foreach (Action item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int actionID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Actions] WHERE ([ActionID] = @ActionID);";
        deleteCommand.Parameters.Add("ActionID", SqlDbType.Int);
        deleteCommand.Parameters["ActionID"].Value = actionID;

        BeforeDBDelete(actionID);
        BeforeRowDelete(actionID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(actionID);
        AfterDBDelete(actionID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ActionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Actions] SET     [ActionTypeID] = @ActionTypeID,    [SystemActionTypeID] = @SystemActionTypeID,    [Name] = @Name,    [TimeSpent] = @TimeSpent,    [DateStarted] = @DateStarted,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [IsKnowledgeBase] = @IsKnowledgeBase,    [ImportID] = @ImportID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [TicketID] = @TicketID,    [ActionSource] = @ActionSource,    [DateModifiedBySalesForceSync] = @DateModifiedBySalesForceSync,    [SalesForceID] = @SalesForceID,    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,    [JiraID] = @JiraID,    [Pinned] = @Pinned,    [Description] = @Description,    [IsClean] = @IsClean,    [ImportFileID] = @ImportFileID  WHERE ([ActionID] = @ActionID);";

		
		tempParameter = updateCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SystemActionTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeSpent", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateStarted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionSource", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedBySalesForceSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedByJiraSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Pinned", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsClean", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Actions] (    [ActionTypeID],    [SystemActionTypeID],    [Name],    [TimeSpent],    [DateStarted],    [IsVisibleOnPortal],    [IsKnowledgeBase],    [ImportID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [TicketID],    [ActionSource],    [DateModifiedBySalesForceSync],    [SalesForceID],    [DateModifiedByJiraSync],    [JiraID],    [Pinned],    [Description],    [IsClean],    [ImportFileID]) VALUES ( @ActionTypeID, @SystemActionTypeID, @Name, @TimeSpent, @DateStarted, @IsVisibleOnPortal, @IsKnowledgeBase, @ImportID, @DateCreated, @DateModified, @CreatorID, @ModifierID, @TicketID, @ActionSource, @DateModifiedBySalesForceSync, @SalesForceID, @DateModifiedByJiraSync, @JiraID, @Pinned, @Description, @IsClean, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsClean", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Pinned", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedByJiraSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SalesForceID", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedBySalesForceSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionSource", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
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
		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateStarted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeSpent", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SystemActionTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionTypeID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Actions] WHERE ([ActionID] = @ActionID);";
		deleteCommand.Parameters.Add("ActionID", SqlDbType.Int);

		try
		{
		  foreach (Action action in this)
		  {
			if (action.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(action);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = action.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ActionID"].AutoIncrement = false;
			  Table.Columns["ActionID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				action.Row["ActionID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(action);
			}
			else if (action.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(action);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = action.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(action);
			}
			else if (action.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)action.Row["ActionID", DataRowVersion.Original];
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

      foreach (Action action in this)
      {
        if (action.Row.Table.Columns.Contains("CreatorID") && (int)action.Row["CreatorID"] == 0) action.Row["CreatorID"] = LoginUser.UserID;
        if (action.Row.Table.Columns.Contains("ModifierID")) action.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Action FindByActionID(int actionID)
    {
      foreach (Action action in this)
      {
        if (action.ActionID == actionID)
        {
          return action;
        }
      }
      return null;
    }

    public virtual Action AddNewAction()
    {
      if (Table.Columns.Count < 1) LoadColumns("Actions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Action(row, this);
    }
    
    public virtual void LoadByActionID(int actionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ActionID], [ActionTypeID], [SystemActionTypeID], [Name], [TimeSpent], [DateStarted], [IsVisibleOnPortal], [IsKnowledgeBase], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [TicketID], [ActionSource], [DateModifiedBySalesForceSync], [SalesForceID], [DateModifiedByJiraSync], [JiraID], [Pinned], [Description], [IsClean], [ImportFileID] FROM [dbo].[Actions] WHERE ([ActionID] = @ActionID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ActionID", actionID);
        Fill(command);
      }
    }
    
    public static Action GetAction(LoginUser loginUser, int actionID)
    {
      Actions actions = new Actions(loginUser);
      actions.LoadByActionID(actionID);
      if (actions.IsEmpty)
        return null;
      else
        return actions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Action> Members

    public IEnumerator<Action> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Action(row, this);
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
