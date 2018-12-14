using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ForumLog : BaseItem
  {
    private ForumLogs _forumLogs;
    
    public ForumLog(DataRow row, ForumLogs forumLogs): base(row, forumLogs)
    {
      _forumLogs = forumLogs;
    }
	
    #region Properties
    
    public ForumLogs Collection
    {
      get { return _forumLogs; }
    }
        
    
    
    
    public int ForumLogID
    {
      get { return (int)Row["ForumLogID"]; }
    }
    

    

    
    public int OrgID
    {
      get { return (int)Row["OrgID"]; }
      set { Row["OrgID"] = CheckValue("OrgID", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int TopicID
    {
      get { return (int)Row["TopicID"]; }
      set { Row["TopicID"] = CheckValue("TopicID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime ViewTime
    {
      get { return DateToLocal((DateTime)Row["ViewTime"]); }
      set { Row["ViewTime"] = CheckValue("ViewTime", value); }
    }

    public DateTime ViewTimeUtc
    {
      get { return (DateTime)Row["ViewTime"]; }
    }
    

    #endregion
    
    
  }

  public partial class ForumLogs : BaseCollection, IEnumerable<ForumLog>
  {
    public ForumLogs(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ForumLogs"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ForumLogID"; }
    }



    public ForumLog this[int index]
    {
      get { return new ForumLog(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ForumLog forumLog);
    partial void AfterRowInsert(ForumLog forumLog);
    partial void BeforeRowEdit(ForumLog forumLog);
    partial void AfterRowEdit(ForumLog forumLog);
    partial void BeforeRowDelete(int forumLogID);
    partial void AfterRowDelete(int forumLogID);    

    partial void BeforeDBDelete(int forumLogID);
    partial void AfterDBDelete(int forumLogID);    

    #endregion

    #region Public Methods

    public ForumLogProxy[] GetForumLogProxies()
    {
      List<ForumLogProxy> list = new List<ForumLogProxy>();

      foreach (ForumLog item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int forumLogID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ForumLogs] WHERE ([ForumLogID] = @ForumLogID);";
        deleteCommand.Parameters.Add("ForumLogID", SqlDbType.Int);
        deleteCommand.Parameters["ForumLogID"].Value = forumLogID;

        BeforeDBDelete(forumLogID);
        BeforeRowDelete(forumLogID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(forumLogID);
        AfterDBDelete(forumLogID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ForumLogsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ForumLogs] SET     [TopicID] = @TopicID,    [UserID] = @UserID,    [OrgID] = @OrgID,    [ViewTime] = @ViewTime  WHERE ([ForumLogID] = @ForumLogID);";

		
		tempParameter = updateCommand.Parameters.Add("ForumLogID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TopicID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrgID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViewTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ForumLogs] (    [TopicID],    [UserID],    [OrgID],    [ViewTime]) VALUES ( @TopicID, @UserID, @OrgID, @ViewTime); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ViewTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrgID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TopicID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ForumLogs] WHERE ([ForumLogID] = @ForumLogID);";
		deleteCommand.Parameters.Add("ForumLogID", SqlDbType.Int);

		try
		{
		  foreach (ForumLog forumLog in this)
		  {
			if (forumLog.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(forumLog);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = forumLog.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ForumLogID"].AutoIncrement = false;
			  Table.Columns["ForumLogID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				forumLog.Row["ForumLogID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(forumLog);
			}
			else if (forumLog.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(forumLog);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = forumLog.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(forumLog);
			}
			else if (forumLog.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)forumLog.Row["ForumLogID", DataRowVersion.Original];
			  deleteCommand.Parameters["ForumLogID"].Value = id;
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

      foreach (ForumLog forumLog in this)
      {
        if (forumLog.Row.Table.Columns.Contains("CreatorID") && (int)forumLog.Row["CreatorID"] == 0) forumLog.Row["CreatorID"] = LoginUser.UserID;
        if (forumLog.Row.Table.Columns.Contains("ModifierID")) forumLog.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ForumLog FindByForumLogID(int forumLogID)
    {
      foreach (ForumLog forumLog in this)
      {
        if (forumLog.ForumLogID == forumLogID)
        {
          return forumLog;
        }
      }
      return null;
    }

    public virtual ForumLog AddNewForumLog()
    {
      if (Table.Columns.Count < 1) LoadColumns("ForumLogs");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ForumLog(row, this);
    }
    
    public virtual void LoadByForumLogID(int forumLogID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ForumLogID], [TopicID], [UserID], [OrgID], [ViewTime] FROM [dbo].[ForumLogs] WHERE ([ForumLogID] = @ForumLogID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ForumLogID", forumLogID);
        Fill(command);
      }
    }
    
    public static ForumLog GetForumLog(LoginUser loginUser, int forumLogID)
    {
      ForumLogs forumLogs = new ForumLogs(loginUser);
      forumLogs.LoadByForumLogID(forumLogID);
      if (forumLogs.IsEmpty)
        return null;
      else
        return forumLogs[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ForumLog> Members

    public IEnumerator<ForumLog> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ForumLog(row, this);
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
