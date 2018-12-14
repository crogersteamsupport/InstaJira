using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ActionLinkToTFSItem : BaseItem
  {
    private ActionLinkToTFS _actionLinkToTFS;
    
    public ActionLinkToTFSItem(DataRow row, ActionLinkToTFS actionLinkToTFS): base(row, actionLinkToTFS)
    {
      _actionLinkToTFS = actionLinkToTFS;
    }
	
    #region Properties
    
    public ActionLinkToTFS Collection
    {
      get { return _actionLinkToTFS; }
    }
        
    
    
    
    public int id
    {
      get { return (int)Row["id"]; }
    }
    

    
    public int? TFSID
    {
      get { return Row["TFSID"] != DBNull.Value ? (int?)Row["TFSID"] : null; }
      set { Row["TFSID"] = CheckValue("TFSID", value); }
    }
    

    
    public int ActionID
    {
      get { return (int)Row["ActionID"]; }
      set { Row["ActionID"] = CheckValue("ActionID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateModifiedByTFSSync
    {
      get { return Row["DateModifiedByTFSSync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedByTFSSync"]) : null; }
      set { Row["DateModifiedByTFSSync"] = CheckValue("DateModifiedByTFSSync", value); }
    }

    public DateTime? DateModifiedByTFSSyncUtc
    {
      get { return Row["DateModifiedByTFSSync"] != DBNull.Value ? (DateTime?)Row["DateModifiedByTFSSync"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class ActionLinkToTFS : BaseCollection, IEnumerable<ActionLinkToTFSItem>
  {
    public ActionLinkToTFS(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ActionLinkToTFS"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "id"; }
    }



    public ActionLinkToTFSItem this[int index]
    {
      get { return new ActionLinkToTFSItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ActionLinkToTFSItem actionLinkToTFSItem);
    partial void AfterRowInsert(ActionLinkToTFSItem actionLinkToTFSItem);
    partial void BeforeRowEdit(ActionLinkToTFSItem actionLinkToTFSItem);
    partial void AfterRowEdit(ActionLinkToTFSItem actionLinkToTFSItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public ActionLinkToTFSItemProxy[] GetActionLinkToTFSItemProxies()
    {
      List<ActionLinkToTFSItemProxy> list = new List<ActionLinkToTFSItemProxy>();

      foreach (ActionLinkToTFSItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionLinkToTFS] WHERE ([id] = @id);";
        deleteCommand.Parameters.Add("id", SqlDbType.Int);
        deleteCommand.Parameters["id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ActionLinkToTFSSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ActionLinkToTFS] SET     [ActionID] = @ActionID,    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,    [TFSID] = @TFSID  WHERE ([id] = @id);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedByTFSSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("TFSID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ActionLinkToTFS] (    [ActionID],    [DateModifiedByTFSSync],    [TFSID]) VALUES ( @ActionID, @DateModifiedByTFSSync, @TFSID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("TFSID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedByTFSSync", SqlDbType.DateTime, 8);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionLinkToTFS] WHERE ([id] = @id);";
		deleteCommand.Parameters.Add("id", SqlDbType.Int);

		try
		{
		  foreach (ActionLinkToTFSItem actionLinkToTFSItem in this)
		  {
			if (actionLinkToTFSItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(actionLinkToTFSItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = actionLinkToTFSItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["id"].AutoIncrement = false;
			  Table.Columns["id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				actionLinkToTFSItem.Row["id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(actionLinkToTFSItem);
			}
			else if (actionLinkToTFSItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(actionLinkToTFSItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = actionLinkToTFSItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(actionLinkToTFSItem);
			}
			else if (actionLinkToTFSItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)actionLinkToTFSItem.Row["id", DataRowVersion.Original];
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

      foreach (ActionLinkToTFSItem actionLinkToTFSItem in this)
      {
        if (actionLinkToTFSItem.Row.Table.Columns.Contains("CreatorID") && (int)actionLinkToTFSItem.Row["CreatorID"] == 0) actionLinkToTFSItem.Row["CreatorID"] = LoginUser.UserID;
        if (actionLinkToTFSItem.Row.Table.Columns.Contains("ModifierID")) actionLinkToTFSItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ActionLinkToTFSItem FindByid(int id)
    {
      foreach (ActionLinkToTFSItem actionLinkToTFSItem in this)
      {
        if (actionLinkToTFSItem.id == id)
        {
          return actionLinkToTFSItem;
        }
      }
      return null;
    }

    public virtual ActionLinkToTFSItem AddNewActionLinkToTFSItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ActionLinkToTFS");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ActionLinkToTFSItem(row, this);
    }
    
    public virtual void LoadByid(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [id], [ActionID], [DateModifiedByTFSSync], [TFSID] FROM [dbo].[ActionLinkToTFS] WHERE ([id] = @id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("id", id);
        Fill(command);
      }
    }
    
    public static ActionLinkToTFSItem GetActionLinkToTFSItem(LoginUser loginUser, int id)
    {
      ActionLinkToTFS actionLinkToTFS = new ActionLinkToTFS(loginUser);
      actionLinkToTFS.LoadByid(id);
      if (actionLinkToTFS.IsEmpty)
        return null;
      else
        return actionLinkToTFS[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ActionLinkToTFSItem> Members

    public IEnumerator<ActionLinkToTFSItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ActionLinkToTFSItem(row, this);
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
