using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Task : BaseItem
  {
    private Tasks _tasks;
    
    public Task(DataRow row, Tasks tasks): base(row, tasks)
    {
      _tasks = tasks;
    }
	
    #region Properties
    
    public Tasks Collection
    {
      get { return _tasks; }
    }
        
    
    
    
    public int TaskID
    {
      get { return (int)Row["TaskID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int? ParentID
    {
      get { return Row["ParentID"] != DBNull.Value ? (int?)Row["ParentID"] : null; }
      set { Row["ParentID"] = CheckValue("ParentID", value); }
    }
    
    public int? ReminderID
    {
      get { return Row["ReminderID"] != DBNull.Value ? (int?)Row["ReminderID"] : null; }
      set { Row["ReminderID"] = CheckValue("ReminderID", value); }
    }
    
    public string CompletionComment
    {
      get { return Row["CompletionComment"] != DBNull.Value ? (string)Row["CompletionComment"] : null; }
      set { Row["CompletionComment"] = CheckValue("CompletionComment", value); }
    }
    

    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
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
    
    public bool IsComplete
    {
      get { return (bool)Row["IsComplete"]; }
      set { Row["IsComplete"] = CheckValue("IsComplete", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DueDate
    {
      get { return Row["DueDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["DueDate"]) : null; }
      set { Row["DueDate"] = CheckValue("DueDate", value); }
    }

    public DateTime? DueDateUtc
    {
      get { return Row["DueDate"] != DBNull.Value ? (DateTime?)Row["DueDate"] : null; }
    }
    
    public DateTime? DateCompleted
    {
      get { return Row["DateCompleted"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateCompleted"]) : null; }
      set { Row["DateCompleted"] = CheckValue("DateCompleted", value); }
    }

    public DateTime? DateCompletedUtc
    {
      get { return Row["DateCompleted"] != DBNull.Value ? (DateTime?)Row["DateCompleted"] : null; }
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

  public partial class Tasks : BaseCollection, IEnumerable<Task>
  {
    public Tasks(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Tasks"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TaskID"; }
    }



    public Task this[int index]
    {
      get { return new Task(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Task task);
    partial void AfterRowInsert(Task task);
    partial void BeforeRowEdit(Task task);
    partial void AfterRowEdit(Task task);
    partial void BeforeRowDelete(int taskID);
    partial void AfterRowDelete(int taskID);    

    partial void BeforeDBDelete(int taskID);
    partial void AfterDBDelete(int taskID);    

    #endregion

    #region Public Methods

    public TaskProxy[] GetTaskProxies()
    {
      List<TaskProxy> list = new List<TaskProxy>();

      foreach (Task item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int taskID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Tasks] WHERE ([TaskID] = @TaskID);";
        deleteCommand.Parameters.Add("TaskID", SqlDbType.Int);
        deleteCommand.Parameters["TaskID"].Value = taskID;

        BeforeDBDelete(taskID);
        BeforeRowDelete(taskID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(taskID);
        AfterDBDelete(taskID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TasksSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Tasks] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name,    [Description] = @Description,    [DueDate] = @DueDate,    [UserID] = @UserID,    [IsComplete] = @IsComplete,    [DateCompleted] = @DateCompleted,    [ParentID] = @ParentID,    [ModifierID] = @ModifierID,    [DateModified] = @DateModified,    [ReminderID] = @ReminderID,    [NeedsIndexing] = @NeedsIndexing,    [CompletionComment] = @CompletionComment  WHERE ([TaskID] = @TaskID);";

		
		tempParameter = updateCommand.Parameters.Add("TaskID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.NVarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsComplete", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateCompleted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReminderID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CompletionComment", SqlDbType.NVarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Tasks] (    [OrganizationID],    [Name],    [Description],    [DueDate],    [UserID],    [IsComplete],    [DateCompleted],    [ParentID],    [CreatorID],    [DateCreated],    [ModifierID],    [DateModified],    [ReminderID],    [NeedsIndexing],    [CompletionComment]) VALUES ( @OrganizationID, @Name, @Description, @DueDate, @UserID, @IsComplete, @DateCompleted, @ParentID, @CreatorID, @DateCreated, @ModifierID, @DateModified, @ReminderID, @NeedsIndexing, @CompletionComment); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("CompletionComment", SqlDbType.NVarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReminderID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCompleted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsComplete", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.NVarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Tasks] WHERE ([TaskID] = @TaskID);";
		deleteCommand.Parameters.Add("TaskID", SqlDbType.Int);

		try
		{
		  foreach (Task task in this)
		  {
			if (task.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(task);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = task.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TaskID"].AutoIncrement = false;
			  Table.Columns["TaskID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				task.Row["TaskID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(task);
			}
			else if (task.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(task);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = task.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(task);
			}
			else if (task.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)task.Row["TaskID", DataRowVersion.Original];
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

      foreach (Task task in this)
      {
        if (task.Row.Table.Columns.Contains("CreatorID") && (int)task.Row["CreatorID"] == 0) task.Row["CreatorID"] = LoginUser.UserID;
        if (task.Row.Table.Columns.Contains("ModifierID")) task.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Task FindByTaskID(int taskID)
    {
      foreach (Task task in this)
      {
        if (task.TaskID == taskID)
        {
          return task;
        }
      }
      return null;
    }

    public virtual Task AddNewTask()
    {
      if (Table.Columns.Count < 1) LoadColumns("Tasks");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Task(row, this);
    }
    
    public virtual void LoadByTaskID(int taskID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TaskID], [OrganizationID], [Name], [Description], [DueDate], [UserID], [IsComplete], [DateCompleted], [ParentID], [CreatorID], [DateCreated], [ModifierID], [DateModified], [ReminderID], [NeedsIndexing], [CompletionComment] FROM [dbo].[Tasks] WHERE ([TaskID] = @TaskID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TaskID", taskID);
        Fill(command);
      }
    }
    
    public static Task GetTask(LoginUser loginUser, int taskID)
    {
      Tasks tasks = new Tasks(loginUser);
      tasks.LoadByTaskID(taskID);
      if (tasks.IsEmpty)
        return null;
      else
        return tasks[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Task> Members

    public IEnumerator<Task> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Task(row, this);
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
