using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TaskAssociationsViewItem : BaseItem
  {
    private TaskAssociationsView _taskAssociationsView;
    
    public TaskAssociationsViewItem(DataRow row, TaskAssociationsView taskAssociationsView): base(row, taskAssociationsView)
    {
      _taskAssociationsView = taskAssociationsView;
    }
	
    #region Properties
    
    public TaskAssociationsView Collection
    {
      get { return _taskAssociationsView; }
    }
        
    
    public string User
    {
      get { return Row["User"] != DBNull.Value ? (string)Row["User"] : null; }
    }
    
    public string Contact
    {
      get { return Row["Contact"] != DBNull.Value ? (string)Row["Contact"] : null; }
    }
    
    
    

    
    public int? TicketNumber
    {
      get { return Row["TicketNumber"] != DBNull.Value ? (int?)Row["TicketNumber"] : null; }
      set { Row["TicketNumber"] = CheckValue("TicketNumber", value); }
    }
    
    public string TicketName
    {
      get { return Row["TicketName"] != DBNull.Value ? (string)Row["TicketName"] : null; }
      set { Row["TicketName"] = CheckValue("TicketName", value); }
    }
    
    public string Company
    {
      get { return Row["Company"] != DBNull.Value ? (string)Row["Company"] : null; }
      set { Row["Company"] = CheckValue("Company", value); }
    }
    
    public string Group
    {
      get { return Row["Group"] != DBNull.Value ? (string)Row["Group"] : null; }
      set { Row["Group"] = CheckValue("Group", value); }
    }
    
    public string Product
    {
      get { return Row["Product"] != DBNull.Value ? (string)Row["Product"] : null; }
      set { Row["Product"] = CheckValue("Product", value); }
    }
    

    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public int TaskID
    {
      get { return (int)Row["TaskID"]; }
      set { Row["TaskID"] = CheckValue("TaskID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class TaskAssociationsView : BaseCollection, IEnumerable<TaskAssociationsViewItem>
  {
    public TaskAssociationsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TaskAssociationsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TaskID"; }
    }



    public TaskAssociationsViewItem this[int index]
    {
      get { return new TaskAssociationsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TaskAssociationsViewItem taskAssociationsViewItem);
    partial void AfterRowInsert(TaskAssociationsViewItem taskAssociationsViewItem);
    partial void BeforeRowEdit(TaskAssociationsViewItem taskAssociationsViewItem);
    partial void AfterRowEdit(TaskAssociationsViewItem taskAssociationsViewItem);
    partial void BeforeRowDelete(int taskID);
    partial void AfterRowDelete(int taskID);    

    partial void BeforeDBDelete(int taskID);
    partial void AfterDBDelete(int taskID);    

    #endregion

    #region Public Methods

    public TaskAssociationsViewItemProxy[] GetTaskAssociationsViewItemProxies()
    {
      List<TaskAssociationsViewItemProxy> list = new List<TaskAssociationsViewItemProxy>();

      foreach (TaskAssociationsViewItem item in this)
      {
        var proxy = item.GetProxy();
        if(proxy != null)
            list.Add(proxy); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int taskID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TaskAssociationsView] WHERE ([TaskID] = @TaskID AND [RefID] = @RefID AND [RefType] = @RefType);";
        deleteCommand.Parameters.Add("TaskID", SqlDbType.Int);
        deleteCommand.Parameters["TaskID"].Value = taskID;

        BeforeDBDelete(taskID);
        BeforeRowDelete(taskID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(taskID);
        AfterDBDelete(taskID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TaskAssociationsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TaskAssociationsView] SET     [TicketNumber] = @TicketNumber,    [TicketName] = @TicketName,    [User] = @User,    [Company] = @Company,    [Group] = @Group,    [Product] = @Product,    [Contact] = @Contact  WHERE ([TaskID] = @TaskID AND [RefID] = @RefID AND [RefType] = @RefType);";

		
		tempParameter = updateCommand.Parameters.Add("TaskID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketName", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("User", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Company", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Group", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Product", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Contact", SqlDbType.NVarChar, 459);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TaskAssociationsView] (    [TaskID],    [RefID],    [RefType],    [TicketNumber],    [TicketName],    [User],    [Company],    [Group],    [Product],    [Contact]) VALUES ( @TaskID, @RefID, @RefType, @TicketNumber, @TicketName, @User, @Company, @Group, @Product, @Contact); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Contact", SqlDbType.NVarChar, 459);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Product", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Group", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Company", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("User", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketName", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaskID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TaskAssociationsView] WHERE ([TaskID] = @TaskID AND [RefID] = @RefID AND [RefType] = @RefType);";
		deleteCommand.Parameters.Add("TaskID", SqlDbType.Int);

		try
		{
		  foreach (TaskAssociationsViewItem taskAssociationsViewItem in this)
		  {
			if (taskAssociationsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(taskAssociationsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = taskAssociationsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TaskID"].AutoIncrement = false;
			  Table.Columns["TaskID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				taskAssociationsViewItem.Row["TaskID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(taskAssociationsViewItem);
			}
			else if (taskAssociationsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(taskAssociationsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = taskAssociationsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(taskAssociationsViewItem);
			}
			else if (taskAssociationsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)taskAssociationsViewItem.Row["TaskID", DataRowVersion.Original];
			  deleteCommand.Parameters["TaskID"].Value = id;
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

      foreach (TaskAssociationsViewItem taskAssociationsViewItem in this)
      {
        if (taskAssociationsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)taskAssociationsViewItem.Row["CreatorID"] == 0) taskAssociationsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (taskAssociationsViewItem.Row.Table.Columns.Contains("ModifierID")) taskAssociationsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TaskAssociationsViewItem FindByTaskID(int taskID)
    {
      foreach (TaskAssociationsViewItem taskAssociationsViewItem in this)
      {
        if (taskAssociationsViewItem.TaskID == taskID)
        {
          return taskAssociationsViewItem;
        }
      }
      return null;
    }

    public virtual TaskAssociationsViewItem AddNewTaskAssociationsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TaskAssociationsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TaskAssociationsViewItem(row, this);
    }
    
    public virtual void LoadByTaskID(int taskID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TaskID], [RefID], [RefType], [TicketNumber], [TicketName], [User], [Company], [Group], [Product], [Contact] FROM [dbo].[TaskAssociationsView] WHERE ([TaskID] = @TaskID AND [RefID] = @RefID AND [RefType] = @RefType);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TaskID", taskID);
        Fill(command);
      }
    }
    
    public static TaskAssociationsViewItem GetTaskAssociationsViewItem(LoginUser loginUser, int taskID)
    {
      TaskAssociationsView taskAssociationsView = new TaskAssociationsView(loginUser);
      taskAssociationsView.LoadByTaskID(taskID);
      if (taskAssociationsView.IsEmpty)
        return null;
      else
        return taskAssociationsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TaskAssociationsViewItem> Members

    public IEnumerator<TaskAssociationsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TaskAssociationsViewItem(row, this);
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
