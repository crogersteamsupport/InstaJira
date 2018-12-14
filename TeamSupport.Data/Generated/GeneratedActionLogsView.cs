using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ActionLogsViewItem : BaseItem
  {
    private ActionLogsView _actionLogsView;
    
    public ActionLogsViewItem(DataRow row, ActionLogsView actionLogsView): base(row, actionLogsView)
    {
      _actionLogsView = actionLogsView;
    }
	
    #region Properties
    
    public ActionLogsView Collection
    {
      get { return _actionLogsView; }
    }
        
    
    
    
    public string Actor
    {
      get { return (string)Row["Actor"]; }
    }
    

    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
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
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int ActionLogType
    {
      get { return (int)Row["ActionLogType"]; }
      set { Row["ActionLogType"] = CheckValue("ActionLogType", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public ReferenceType RefType
    {
      get { return (ReferenceType)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int ActionLogID
    {
      get { return (int)Row["ActionLogID"]; }
      set { Row["ActionLogID"] = CheckValue("ActionLogID", value); }
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

  public partial class ActionLogsView : BaseCollection, IEnumerable<ActionLogsViewItem>
  {
    public ActionLogsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ActionLogsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ActionLogID"; }
    }



    public ActionLogsViewItem this[int index]
    {
      get { return new ActionLogsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ActionLogsViewItem actionLogsViewItem);
    partial void AfterRowInsert(ActionLogsViewItem actionLogsViewItem);
    partial void BeforeRowEdit(ActionLogsViewItem actionLogsViewItem);
    partial void AfterRowEdit(ActionLogsViewItem actionLogsViewItem);
    partial void BeforeRowDelete(int actionLogID);
    partial void AfterRowDelete(int actionLogID);    

    partial void BeforeDBDelete(int actionLogID);
    partial void AfterDBDelete(int actionLogID);    

    #endregion

    #region Public Methods

    public ActionLogsViewItemProxy[] GetActionLogsViewItemProxies()
    {
      List<ActionLogsViewItemProxy> list = new List<ActionLogsViewItemProxy>();

      foreach (ActionLogsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int actionLogID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionLogsView] WHERE ([ActionLogID] = @ActionLogID);";
        deleteCommand.Parameters.Add("ActionLogID", SqlDbType.Int);
        deleteCommand.Parameters["ActionLogID"].Value = actionLogID;

        BeforeDBDelete(actionLogID);
        BeforeRowDelete(actionLogID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(actionLogID);
        AfterDBDelete(actionLogID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ActionLogsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ActionLogsView] SET     [OrganizationID] = @OrganizationID,    [RefType] = @RefType,    [RefID] = @RefID,    [ActionLogType] = @ActionLogType,    [Description] = @Description,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [Actor] = @Actor  WHERE ([ActionLogID] = @ActionLogID);";

		
		tempParameter = updateCommand.Parameters.Add("ActionLogID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionLogType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
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
		
		tempParameter = updateCommand.Parameters.Add("Actor", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ActionLogsView] (    [ActionLogID],    [OrganizationID],    [RefType],    [RefID],    [ActionLogType],    [Description],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [Actor]) VALUES ( @ActionLogID, @OrganizationID, @RefType, @RefID, @ActionLogType, @Description, @DateCreated, @DateModified, @CreatorID, @ModifierID, @Actor); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Actor", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionLogType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("ActionLogID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ActionLogsView] WHERE ([ActionLogID] = @ActionLogID);";
		deleteCommand.Parameters.Add("ActionLogID", SqlDbType.Int);

		try
		{
		  foreach (ActionLogsViewItem actionLogsViewItem in this)
		  {
			if (actionLogsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(actionLogsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = actionLogsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ActionLogID"].AutoIncrement = false;
			  Table.Columns["ActionLogID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				actionLogsViewItem.Row["ActionLogID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(actionLogsViewItem);
			}
			else if (actionLogsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(actionLogsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = actionLogsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(actionLogsViewItem);
			}
			else if (actionLogsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)actionLogsViewItem.Row["ActionLogID", DataRowVersion.Original];
			  deleteCommand.Parameters["ActionLogID"].Value = id;
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

      foreach (ActionLogsViewItem actionLogsViewItem in this)
      {
        if (actionLogsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)actionLogsViewItem.Row["CreatorID"] == 0) actionLogsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (actionLogsViewItem.Row.Table.Columns.Contains("ModifierID")) actionLogsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ActionLogsViewItem FindByActionLogID(int actionLogID)
    {
      foreach (ActionLogsViewItem actionLogsViewItem in this)
      {
        if (actionLogsViewItem.ActionLogID == actionLogID)
        {
          return actionLogsViewItem;
        }
      }
      return null;
    }

    public virtual ActionLogsViewItem AddNewActionLogsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ActionLogsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ActionLogsViewItem(row, this);
    }
    
    public virtual void LoadByActionLogID(int actionLogID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ActionLogID], [OrganizationID], [RefType], [RefID], [ActionLogType], [Description], [DateCreated], [DateModified], [CreatorID], [ModifierID], [Actor] FROM [dbo].[ActionLogsView] WHERE ([ActionLogID] = @ActionLogID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ActionLogID", actionLogID);
        Fill(command);
      }
    }
    
    public static ActionLogsViewItem GetActionLogsViewItem(LoginUser loginUser, int actionLogID)
    {
      ActionLogsView actionLogsView = new ActionLogsView(loginUser);
      actionLogsView.LoadByActionLogID(actionLogID);
      if (actionLogsView.IsEmpty)
        return null;
      else
        return actionLogsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ActionLogsViewItem> Members

    public IEnumerator<ActionLogsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ActionLogsViewItem(row, this);
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
