using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TaskEmailPost : BaseItem
  {
    private TaskEmailPosts _taskEmailPosts;
    
    public TaskEmailPost(DataRow row, TaskEmailPosts taskEmailPosts): base(row, taskEmailPosts)
    {
      _taskEmailPosts = taskEmailPosts;
    }
	
    #region Properties
    
    public TaskEmailPosts Collection
    {
      get { return _taskEmailPosts; }
    }
        
    
    
    
    public int TaskEmailPostID
    {
      get { return (int)Row["TaskEmailPostID"]; }
    }
    

    
    public string LockProcessID
    {
      get { return Row["LockProcessID"] != DBNull.Value ? (string)Row["LockProcessID"] : null; }
      set { Row["LockProcessID"] = CheckValue("LockProcessID", value); }
    }
    
    public int? OldUserID
    {
      get { return Row["OldUserID"] != DBNull.Value ? (int?)Row["OldUserID"] : null; }
      set { Row["OldUserID"] = CheckValue("OldUserID", value); }
    }
    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int TaskID
    {
      get { return (int)Row["TaskID"]; }
      set { Row["TaskID"] = CheckValue("TaskID", value); }
    }
    
    public int HoldTime
    {
      get { return (int)Row["HoldTime"]; }
      set { Row["HoldTime"] = CheckValue("HoldTime", value); }
    }
    
    public int TaskEmailPostType
    {
      get { return (int)Row["TaskEmailPostType"]; }
      set { Row["TaskEmailPostType"] = CheckValue("TaskEmailPostType", value); }
    }
    

    /* DateTime */
    
    
    

    

    
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

  public partial class TaskEmailPosts : BaseCollection, IEnumerable<TaskEmailPost>
  {
    public TaskEmailPosts(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TaskEmailPosts"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TaskEmailPostID"; }
    }



    public TaskEmailPost this[int index]
    {
      get { return new TaskEmailPost(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TaskEmailPost taskEmailPost);
    partial void AfterRowInsert(TaskEmailPost taskEmailPost);
    partial void BeforeRowEdit(TaskEmailPost taskEmailPost);
    partial void AfterRowEdit(TaskEmailPost taskEmailPost);
    partial void BeforeRowDelete(int taskEmailPostID);
    partial void AfterRowDelete(int taskEmailPostID);    

    partial void BeforeDBDelete(int taskEmailPostID);
    partial void AfterDBDelete(int taskEmailPostID);    

    #endregion

    #region Public Methods

    public TaskEmailPostProxy[] GetTaskEmailPostProxies()
    {
      List<TaskEmailPostProxy> list = new List<TaskEmailPostProxy>();

      foreach (TaskEmailPost item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int taskEmailPostID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TaskEmailPosts] WHERE ([TaskEmailPostID] = @TaskEmailPostID);";
        deleteCommand.Parameters.Add("TaskEmailPostID", SqlDbType.Int);
        deleteCommand.Parameters["TaskEmailPostID"].Value = taskEmailPostID;

        BeforeDBDelete(taskEmailPostID);
        BeforeRowDelete(taskEmailPostID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(taskEmailPostID);
        AfterDBDelete(taskEmailPostID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TaskEmailPostsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TaskEmailPosts] SET     [TaskEmailPostType] = @TaskEmailPostType,    [HoldTime] = @HoldTime,    [TaskID] = @TaskID,    [LockProcessID] = @LockProcessID,    [OldUserID] = @OldUserID  WHERE ([TaskEmailPostID] = @TaskEmailPostID);";

		
		tempParameter = updateCommand.Parameters.Add("TaskEmailPostID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TaskEmailPostType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("HoldTime", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("LockProcessID", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OldUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TaskEmailPosts] (    [TaskEmailPostType],    [HoldTime],    [DateCreated],    [TaskID],    [CreatorID],    [LockProcessID],    [OldUserID]) VALUES ( @TaskEmailPostType, @HoldTime, @DateCreated, @TaskID, @CreatorID, @LockProcessID, @OldUserID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OldUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LockProcessID", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("HoldTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TaskEmailPostType", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TaskEmailPosts] WHERE ([TaskEmailPostID] = @TaskEmailPostID);";
		deleteCommand.Parameters.Add("TaskEmailPostID", SqlDbType.Int);

		try
		{
		  foreach (TaskEmailPost taskEmailPost in this)
		  {
			if (taskEmailPost.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(taskEmailPost);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = taskEmailPost.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TaskEmailPostID"].AutoIncrement = false;
			  Table.Columns["TaskEmailPostID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				taskEmailPost.Row["TaskEmailPostID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(taskEmailPost);
			}
			else if (taskEmailPost.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(taskEmailPost);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = taskEmailPost.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(taskEmailPost);
			}
			else if (taskEmailPost.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)taskEmailPost.Row["TaskEmailPostID", DataRowVersion.Original];
			  deleteCommand.Parameters["TaskEmailPostID"].Value = id;
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

      foreach (TaskEmailPost taskEmailPost in this)
      {
        if (taskEmailPost.Row.Table.Columns.Contains("CreatorID") && (int)taskEmailPost.Row["CreatorID"] == 0) taskEmailPost.Row["CreatorID"] = LoginUser.UserID;
        if (taskEmailPost.Row.Table.Columns.Contains("ModifierID")) taskEmailPost.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TaskEmailPost FindByTaskEmailPostID(int taskEmailPostID)
    {
      foreach (TaskEmailPost taskEmailPost in this)
      {
        if (taskEmailPost.TaskEmailPostID == taskEmailPostID)
        {
          return taskEmailPost;
        }
      }
      return null;
    }

    public virtual TaskEmailPost AddNewTaskEmailPost()
    {
      if (Table.Columns.Count < 1) LoadColumns("TaskEmailPosts");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TaskEmailPost(row, this);
    }
    
    public virtual void LoadByTaskEmailPostID(int taskEmailPostID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TaskEmailPostID], [TaskEmailPostType], [HoldTime], [DateCreated], [TaskID], [CreatorID], [LockProcessID], [OldUserID] FROM [dbo].[TaskEmailPosts] WHERE ([TaskEmailPostID] = @TaskEmailPostID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TaskEmailPostID", taskEmailPostID);
        Fill(command);
      }
    }
    
    public static TaskEmailPost GetTaskEmailPost(LoginUser loginUser, int taskEmailPostID)
    {
      TaskEmailPosts taskEmailPosts = new TaskEmailPosts(loginUser);
      taskEmailPosts.LoadByTaskEmailPostID(taskEmailPostID);
      if (taskEmailPosts.IsEmpty)
        return null;
      else
        return taskEmailPosts[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TaskEmailPost> Members

    public IEnumerator<TaskEmailPost> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TaskEmailPost(row, this);
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
