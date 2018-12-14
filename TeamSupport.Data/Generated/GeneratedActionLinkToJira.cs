using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ActionLinkToJiraItem : BaseItem
  {
    private ActionLinkToJira _actionLinkToJira;
    
    public ActionLinkToJiraItem(DataRow row, ActionLinkToJira actionLinkToJira): base(row, actionLinkToJira)
    {
      _actionLinkToJira = actionLinkToJira;
    }
	
    #region Properties
    
    public ActionLinkToJira Collection
    {
      get { return _actionLinkToJira; }
    }
        
    
    
    
    public int id
    {
      get { return (int)Row["id"]; }
    }
    

    
    public int? ActionID
    {
      get { return Row["ActionID"] != DBNull.Value ? (int?)Row["ActionID"] : null; }
      set { Row["ActionID"] = CheckValue("ActionID", value); }
    }
    
    public int? JiraID
    {
      get { return Row["JiraID"] != DBNull.Value ? (int?)Row["JiraID"] : null; }
      set { Row["JiraID"] = CheckValue("JiraID", value); }
    }
    

    

    /* DateTime */
    
    
    

    
    public DateTime? DateModifiedByJiraSync
    {
      get { return Row["DateModifiedByJiraSync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedByJiraSync"]) : null; }
      set { Row["DateModifiedByJiraSync"] = CheckValue("DateModifiedByJiraSync", value); }
    }

    public DateTime? DateModifiedByJiraSyncUtc
    {
      get { return Row["DateModifiedByJiraSync"] != DBNull.Value ? (DateTime?)Row["DateModifiedByJiraSync"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class ActionLinkToJira : BaseCollection, IEnumerable<ActionLinkToJiraItem>
  {
    public ActionLinkToJira(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ActionLinkToJira"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "id"; }
    }



    public ActionLinkToJiraItem this[int index]
    {
      get { return new ActionLinkToJiraItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ActionLinkToJiraItem actionLinkToJiraItem);
    partial void AfterRowInsert(ActionLinkToJiraItem actionLinkToJiraItem);
    partial void BeforeRowEdit(ActionLinkToJiraItem actionLinkToJiraItem);
    partial void AfterRowEdit(ActionLinkToJiraItem actionLinkToJiraItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public ActionLinkToJiraItemProxy[] GetActionLinkToJiraItemProxies()
    {
      List<ActionLinkToJiraItemProxy> list = new List<ActionLinkToJiraItemProxy>();

      foreach (ActionLinkToJiraItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
      BeforeDBDelete(id);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionLinkToJira] WHERE ([id] = @id);";
        deleteCommand.Parameters.Add("id", SqlDbType.Int);
        deleteCommand.Parameters["id"].Value = id;

        BeforeRowDelete(id);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(id);
      }
      AfterDBDelete(id);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ActionLinkToJiraSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ActionLinkToJira] SET     [ActionID] = @ActionID,    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,    [JiraID] = @JiraID  WHERE ([id] = @id);";

		
		tempParameter = updateCommand.Parameters.Add("id", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ActionLinkToJira] (    [ActionID],    [DateModifiedByJiraSync],    [JiraID]) VALUES ( @ActionID, @DateModifiedByJiraSync, @JiraID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionLinkToJira] WHERE ([id] = @id);";
		deleteCommand.Parameters.Add("id", SqlDbType.Int);

		try
		{
		  foreach (ActionLinkToJiraItem actionLinkToJiraItem in this)
		  {
			if (actionLinkToJiraItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(actionLinkToJiraItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = actionLinkToJiraItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["id"].AutoIncrement = false;
			  Table.Columns["id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				actionLinkToJiraItem.Row["id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(actionLinkToJiraItem);
			}
			else if (actionLinkToJiraItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(actionLinkToJiraItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = actionLinkToJiraItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(actionLinkToJiraItem);
			}
			else if (actionLinkToJiraItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)actionLinkToJiraItem.Row["id", DataRowVersion.Original];
			  deleteCommand.Parameters["id"].Value = id;
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

      foreach (ActionLinkToJiraItem actionLinkToJiraItem in this)
      {
        if (actionLinkToJiraItem.Row.Table.Columns.Contains("CreatorID") && (int)actionLinkToJiraItem.Row["CreatorID"] == 0) actionLinkToJiraItem.Row["CreatorID"] = LoginUser.UserID;
        if (actionLinkToJiraItem.Row.Table.Columns.Contains("ModifierID")) actionLinkToJiraItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ActionLinkToJiraItem FindByid(int id)
    {
      foreach (ActionLinkToJiraItem actionLinkToJiraItem in this)
      {
        if (actionLinkToJiraItem.id == id)
        {
          return actionLinkToJiraItem;
        }
      }
      return null;
    }

    public virtual ActionLinkToJiraItem AddNewActionLinkToJiraItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ActionLinkToJira");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ActionLinkToJiraItem(row, this);
    }
    
    public virtual void LoadByid(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [id], [ActionID], [DateModifiedByJiraSync], [JiraID] FROM [dbo].[ActionLinkToJira] WHERE ([id] = @id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("id", id);
        Fill(command);
      }
    }
    
    public static ActionLinkToJiraItem GetActionLinkToJiraItem(LoginUser loginUser, int id)
    {
      ActionLinkToJira actionLinkToJira = new ActionLinkToJira(loginUser);
      actionLinkToJira.LoadByid(id);
      if (actionLinkToJira.IsEmpty)
        return null;
      else
        return actionLinkToJira[0];
    }
    
    public static ActionLinkToJiraItem GetActionLinkToJiraItemByActionID(LoginUser loginUser, int id)
    {
      ActionLinkToJira actionLinkToJira = new ActionLinkToJira(loginUser);
      actionLinkToJira.LoadByActionid(id);
      if (actionLinkToJira.IsEmpty)
        return null;
      else
        return actionLinkToJira[0];
    }    
    
	    public virtual void LoadByActionid(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [id], [ActionID], [DateModifiedByJiraSync], [JiraID] FROM [dbo].[ActionLinkToJira] WHERE ([ActionID] = @id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("id", id);
        Fill(command);
      }
    }    
    

    #endregion

    #region IEnumerable<ActionLinkToJiraItem> Members

    public IEnumerator<ActionLinkToJiraItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ActionLinkToJiraItem(row, this);
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
