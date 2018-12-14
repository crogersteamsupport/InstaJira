using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ActionLinkToSnowItem : BaseItem
  {
    private ActionLinkToSnow _actionLinkToSnow;
    
    public ActionLinkToSnowItem(DataRow row, ActionLinkToSnow actionLinkToSnow): base(row, actionLinkToSnow)
    {
      _actionLinkToSnow = actionLinkToSnow;
    }
	
    #region Properties
    
    public ActionLinkToSnow Collection
    {
      get { return _actionLinkToSnow; }
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
    

    
    public int ActionID
    {
      get { return (int)Row["ActionID"]; }
      set { Row["ActionID"] = CheckValue("ActionID", value); }
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

  public partial class ActionLinkToSnow : BaseCollection, IEnumerable<ActionLinkToSnowItem>
  {
    public ActionLinkToSnow(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ActionLinkToSnow"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public ActionLinkToSnowItem this[int index]
    {
      get { return new ActionLinkToSnowItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ActionLinkToSnowItem actionLinkToSnowItem);
    partial void AfterRowInsert(ActionLinkToSnowItem actionLinkToSnowItem);
    partial void BeforeRowEdit(ActionLinkToSnowItem actionLinkToSnowItem);
    partial void AfterRowEdit(ActionLinkToSnowItem actionLinkToSnowItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public ActionLinkToSnowItemProxy[] GetActionLinkToSnowItemProxies()
    {
      List<ActionLinkToSnowItemProxy> list = new List<ActionLinkToSnowItemProxy>();

      foreach (ActionLinkToSnowItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionLinkToSnow] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ActionLinkToSnowSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ActionLinkToSnow] SET     [ActionID] = @ActionID,    [DateModifiedBySync] = @DateModifiedBySync,    [AppId] = @AppId  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedBySync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("AppId", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ActionLinkToSnow] (    [ActionID],    [DateModifiedBySync],    [AppId]) VALUES ( @ActionID, @DateModifiedBySync, @AppId); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("AppId", SqlDbType.VarChar, 50);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionLinkToSnow] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (ActionLinkToSnowItem actionLinkToSnowItem in this)
		  {
			if (actionLinkToSnowItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(actionLinkToSnowItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = actionLinkToSnowItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				actionLinkToSnowItem.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(actionLinkToSnowItem);
			}
			else if (actionLinkToSnowItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(actionLinkToSnowItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = actionLinkToSnowItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(actionLinkToSnowItem);
			}
			else if (actionLinkToSnowItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)actionLinkToSnowItem.Row["Id", DataRowVersion.Original];
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

      foreach (ActionLinkToSnowItem actionLinkToSnowItem in this)
      {
        if (actionLinkToSnowItem.Row.Table.Columns.Contains("CreatorID") && (int)actionLinkToSnowItem.Row["CreatorID"] == 0) actionLinkToSnowItem.Row["CreatorID"] = LoginUser.UserID;
        if (actionLinkToSnowItem.Row.Table.Columns.Contains("ModifierID")) actionLinkToSnowItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ActionLinkToSnowItem FindById(int id)
    {
      foreach (ActionLinkToSnowItem actionLinkToSnowItem in this)
      {
        if (actionLinkToSnowItem.Id == id)
        {
          return actionLinkToSnowItem;
        }
      }
      return null;
    }

    public virtual ActionLinkToSnowItem AddNewActionLinkToSnowItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ActionLinkToSnow");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ActionLinkToSnowItem(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [ActionID], [DateModifiedBySync], [AppId] FROM [dbo].[ActionLinkToSnow] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static ActionLinkToSnowItem GetActionLinkToSnowItem(LoginUser loginUser, int id)
    {
      ActionLinkToSnow actionLinkToSnow = new ActionLinkToSnow(loginUser);
      actionLinkToSnow.LoadById(id);
      if (actionLinkToSnow.IsEmpty)
        return null;
      else
        return actionLinkToSnow[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ActionLinkToSnowItem> Members

    public IEnumerator<ActionLinkToSnowItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ActionLinkToSnowItem(row, this);
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
