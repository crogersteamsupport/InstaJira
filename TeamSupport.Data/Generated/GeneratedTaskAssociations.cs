using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TaskAssociation : BaseItem
  {
    private TaskAssociations _taskAssociations;
    
    public TaskAssociation(DataRow row, TaskAssociations taskAssociations): base(row, taskAssociations)
    {
      _taskAssociations = taskAssociations;
    }
	
    #region Properties
    
    public TaskAssociations Collection
    {
      get { return _taskAssociations; }
    }
        
    
    
    

    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public TaskAssociationProxy.References RefType
    {
      get { return (TaskAssociationProxy.References)Row["RefType"]; }
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

  public partial class TaskAssociations : BaseCollection, IEnumerable<TaskAssociation>
  {
    public TaskAssociations(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TaskAssociations"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TaskID"; }
    }



    public TaskAssociation this[int index]
    {
      get { return new TaskAssociation(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TaskAssociation taskAssociation);
    partial void AfterRowInsert(TaskAssociation taskAssociation);
    partial void BeforeRowEdit(TaskAssociation taskAssociation);
    partial void AfterRowEdit(TaskAssociation taskAssociation);
    partial void BeforeRowDelete(int taskID);
    partial void AfterRowDelete(int taskID);    

    partial void BeforeDBDelete(int taskID);
    partial void AfterDBDelete(int taskID);    

    #endregion

    #region Public Methods

    //public TaskAssociationProxy[] GetTaskAssociationProxies()
    //{
    //  List<TaskAssociationProxy> list = new List<TaskAssociationProxy>();

    //  foreach (TaskAssociation item in this)
    //  {
    //    list.Add(item.GetProxy()); 
    //  }

    //  return list.ToArray();
    //}	
	
    public virtual void DeleteFromDB(int taskID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TaskAssociations] WHERE ([TaskID] = @TaskID AND [RefID] = @RefID AND [RefType] = @RefType);";
        deleteCommand.Parameters.Add("TaskID", SqlDbType.Int);
        deleteCommand.Parameters["TaskID"].Value = taskID;

        BeforeDBDelete(taskID);
        BeforeRowDelete(taskID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(taskID);
        AfterDBDelete(taskID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TaskAssociationsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TaskAssociations] SET   WHERE ([TaskID] = @TaskID AND [RefID] = @RefID AND [RefType] = @RefType);";

		
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TaskAssociations] (    [TaskID],    [RefID],    [RefType],    [CreatorID],    [DateCreated]) VALUES ( @TaskID, @RefID, @RefType, @CreatorID, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TaskAssociations] WHERE ([TaskID] = @TaskID AND [RefID] = @RefID AND [RefType] = @RefType);";
		deleteCommand.Parameters.Add("TaskID", SqlDbType.Int);

		try
		{
		  foreach (TaskAssociation taskAssociation in this)
		  {
			if (taskAssociation.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(taskAssociation);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = taskAssociation.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TaskID"].AutoIncrement = false;
			  Table.Columns["TaskID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				taskAssociation.Row["TaskID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(taskAssociation);
			}
			else if (taskAssociation.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(taskAssociation);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = taskAssociation.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(taskAssociation);
			}
			else if (taskAssociation.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)taskAssociation.Row["TaskID", DataRowVersion.Original];
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

      foreach (TaskAssociation taskAssociation in this)
      {
        if (taskAssociation.Row.Table.Columns.Contains("CreatorID") && (int)taskAssociation.Row["CreatorID"] == 0) taskAssociation.Row["CreatorID"] = LoginUser.UserID;
        if (taskAssociation.Row.Table.Columns.Contains("ModifierID")) taskAssociation.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TaskAssociation FindByTaskID(int taskID)
    {
      foreach (TaskAssociation taskAssociation in this)
      {
        if (taskAssociation.TaskID == taskID)
        {
          return taskAssociation;
        }
      }
      return null;
    }

    public virtual TaskAssociation AddNewTaskAssociation()
    {
      if (Table.Columns.Count < 1) LoadColumns("TaskAssociations");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TaskAssociation(row, this);
    }
    
    public virtual void LoadByTaskID(int taskID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TaskID], [RefID], [RefType], [CreatorID], [DateCreated] FROM [dbo].[TaskAssociations] WHERE ([TaskID] = @TaskID AND [RefID] = @RefID AND [RefType] = @RefType);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TaskID", taskID);
        Fill(command);
      }
    }
    
    public static TaskAssociation GetTaskAssociation(LoginUser loginUser, int taskID)
    {
      TaskAssociations taskAssociations = new TaskAssociations(loginUser);
      taskAssociations.LoadByTaskID(taskID);
      if (taskAssociations.IsEmpty)
        return null;
      else
        return taskAssociations[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TaskAssociation> Members

    public IEnumerator<TaskAssociation> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TaskAssociation(row, this);
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
