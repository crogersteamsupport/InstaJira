using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TaskLog : BaseItem
  {
    private TaskLogs _taskLogs;
    
    public TaskLog(DataRow row, TaskLogs taskLogs): base(row, taskLogs)
    {
      _taskLogs = taskLogs;
    }
	
    #region Properties
    
    public TaskLogs Collection
    {
      get { return _taskLogs; }
    }
        
    
    
    
    public int TaskLogID
    {
      get { return (int)Row["TaskLogID"]; }
    }
    

    
    public int? TaskID
    {
      get { return Row["TaskID"] != DBNull.Value ? (int?)Row["TaskID"] : null; }
      set { Row["TaskID"] = CheckValue("TaskID", value); }
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

  public partial class TaskLogs : BaseCollection, IEnumerable<TaskLog>
  {
    public TaskLogs(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TaskLogs"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TaskLogID"; }
    }



    public TaskLog this[int index]
    {
      get { return new TaskLog(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TaskLog taskLog);
    partial void AfterRowInsert(TaskLog taskLog);
    partial void BeforeRowEdit(TaskLog taskLog);
    partial void AfterRowEdit(TaskLog taskLog);
    partial void BeforeRowDelete(int taskLogID);
    partial void AfterRowDelete(int taskLogID);    

    partial void BeforeDBDelete(int taskLogID);
    partial void AfterDBDelete(int taskLogID);    

    #endregion

    #region Public Methods

    public TaskLogProxy[] GetTaskLogProxies()
    {
      List<TaskLogProxy> list = new List<TaskLogProxy>();

      foreach (TaskLog item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int taskLogID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TaskLogs] WHERE ([TaskLogID] = @TaskLogID);";
        deleteCommand.Parameters.Add("TaskLogID", SqlDbType.Int);
        deleteCommand.Parameters["TaskLogID"].Value = taskLogID;

        BeforeDBDelete(taskLogID);
        BeforeRowDelete(taskLogID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(taskLogID);
        AfterDBDelete(taskLogID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TaskLogsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TaskLogs] SET     [TaskID] = @TaskID,    [Description] = @Description,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([TaskLogID] = @TaskLogID);";

		
		tempParameter = updateCommand.Parameters.Add("TaskLogID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaskID", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TaskLogs] (    [TaskID],    [Description],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @TaskID, @Description, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TaskLogs] WHERE ([TaskLogID] = @TaskLogID);";
		deleteCommand.Parameters.Add("TaskLogID", SqlDbType.Int);

		try
		{
		  foreach (TaskLog taskLog in this)
		  {
			if (taskLog.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(taskLog);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = taskLog.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TaskLogID"].AutoIncrement = false;
			  Table.Columns["TaskLogID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				taskLog.Row["TaskLogID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(taskLog);
			}
			else if (taskLog.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(taskLog);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = taskLog.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(taskLog);
			}
			else if (taskLog.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)taskLog.Row["TaskLogID", DataRowVersion.Original];
			  deleteCommand.Parameters["TaskLogID"].Value = id;
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

      foreach (TaskLog taskLog in this)
      {
        if (taskLog.Row.Table.Columns.Contains("CreatorID") && (int)taskLog.Row["CreatorID"] == 0) taskLog.Row["CreatorID"] = LoginUser.UserID;
        if (taskLog.Row.Table.Columns.Contains("ModifierID")) taskLog.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TaskLog FindByTaskLogID(int taskLogID)
    {
      foreach (TaskLog taskLog in this)
      {
        if (taskLog.TaskLogID == taskLogID)
        {
          return taskLog;
        }
      }
      return null;
    }

    public virtual TaskLog AddNewTaskLog()
    {
      if (Table.Columns.Count < 1) LoadColumns("TaskLogs");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TaskLog(row, this);
    }
    
    public virtual void LoadByTaskLogID(int taskLogID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TaskLogID], [TaskID], [Description], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[TaskLogs] WHERE ([TaskLogID] = @TaskLogID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TaskLogID", taskLogID);
        Fill(command);
      }
    }
    
    public static TaskLog GetTaskLog(LoginUser loginUser, int taskLogID)
    {
      TaskLogs taskLogs = new TaskLogs(loginUser);
      taskLogs.LoadByTaskLogID(taskLogID);
      if (taskLogs.IsEmpty)
        return null;
      else
        return taskLogs[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TaskLog> Members

    public IEnumerator<TaskLog> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TaskLog(row, this);
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
