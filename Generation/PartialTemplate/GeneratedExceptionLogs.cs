using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ExceptionLog : BaseItem
  {
    private ExceptionLogs _exceptionLogs;
    
    public ExceptionLog(DataRow row, ExceptionLogs exceptionLogs): base(row, exceptionLogs)
    {
      _exceptionLogs = exceptionLogs;
    }
	
    #region Properties
    
    public ExceptionLogs Collection
    {
      get { return _exceptionLogs; }
    }
        
    
    
    
    public int ExceptionLogID
    {
      get { return (int)Row["ExceptionLogID"]; }
    }
    

    
    public string URL
    {
      get { return Row["URL"] != DBNull.Value ? (string)Row["URL"] : null; }
      set { Row["URL"] = CheckValue("URL", value); }
    }
    
    public string PageInfo
    {
      get { return Row["PageInfo"] != DBNull.Value ? (string)Row["PageInfo"] : null; }
      set { Row["PageInfo"] = CheckValue("PageInfo", value); }
    }
    
    public string ExceptionName
    {
      get { return Row["ExceptionName"] != DBNull.Value ? (string)Row["ExceptionName"] : null; }
      set { Row["ExceptionName"] = CheckValue("ExceptionName", value); }
    }
    
    public string Message
    {
      get { return Row["Message"] != DBNull.Value ? (string)Row["Message"] : null; }
      set { Row["Message"] = CheckValue("Message", value); }
    }
    
    public string StackTrace
    {
      get { return Row["StackTrace"] != DBNull.Value ? (string)Row["StackTrace"] : null; }
      set { Row["StackTrace"] = CheckValue("StackTrace", value); }
    }
    
    public string Browser
    {
      get { return Row["Browser"] != DBNull.Value ? (string)Row["Browser"] : null; }
      set { Row["Browser"] = CheckValue("Browser", value); }
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

  public partial class ExceptionLogs : BaseCollection, IEnumerable<ExceptionLog>
  {
    public ExceptionLogs(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ExceptionLogs"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ExceptionLogID"; }
    }



    public ExceptionLog this[int index]
    {
      get { return new ExceptionLog(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ExceptionLog exceptionLog);
    partial void AfterRowInsert(ExceptionLog exceptionLog);
    partial void BeforeRowEdit(ExceptionLog exceptionLog);
    partial void AfterRowEdit(ExceptionLog exceptionLog);
    partial void BeforeRowDelete(int exceptionLogID);
    partial void AfterRowDelete(int exceptionLogID);    

    partial void BeforeDBDelete(int exceptionLogID);
    partial void AfterDBDelete(int exceptionLogID);    

    #endregion

    #region Public Methods

    public ExceptionLogProxy[] GetExceptionLogProxies()
    {
      List<ExceptionLogProxy> list = new List<ExceptionLogProxy>();

      foreach (ExceptionLog item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int exceptionLogID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ExceptionLogs] WHERE ([ExceptionLogID] = @ExceptionLogID);";
        deleteCommand.Parameters.Add("ExceptionLogID", SqlDbType.Int);
        deleteCommand.Parameters["ExceptionLogID"].Value = exceptionLogID;

        BeforeDBDelete(exceptionLogID);
        BeforeRowDelete(exceptionLogID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(exceptionLogID);
        AfterDBDelete(exceptionLogID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ExceptionLogsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ExceptionLogs] SET     [URL] = @URL,    [PageInfo] = @PageInfo,    [ExceptionName] = @ExceptionName,    [Message] = @Message,    [StackTrace] = @StackTrace,    [Browser] = @Browser,    [ModifierID] = @ModifierID,    [DateModified] = @DateModified  WHERE ([ExceptionLogID] = @ExceptionLogID);";

		
		tempParameter = updateCommand.Parameters.Add("ExceptionLogID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("URL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PageInfo", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExceptionName", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Message", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("StackTrace", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Browser", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ExceptionLogs] (    [URL],    [PageInfo],    [ExceptionName],    [Message],    [StackTrace],    [Browser],    [CreatorID],    [DateCreated],    [ModifierID],    [DateModified]) VALUES ( @URL, @PageInfo, @ExceptionName, @Message, @StackTrace, @Browser, @CreatorID, @DateCreated, @ModifierID, @DateModified); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("Browser", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("StackTrace", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Message", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ExceptionName", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PageInfo", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("URL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ExceptionLogs] WHERE ([ExceptionLogID] = @ExceptionLogID);";
		deleteCommand.Parameters.Add("ExceptionLogID", SqlDbType.Int);

		try
		{
		  foreach (ExceptionLog exceptionLog in this)
		  {
			if (exceptionLog.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(exceptionLog);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = exceptionLog.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ExceptionLogID"].AutoIncrement = false;
			  Table.Columns["ExceptionLogID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				exceptionLog.Row["ExceptionLogID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(exceptionLog);
			}
			else if (exceptionLog.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(exceptionLog);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = exceptionLog.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(exceptionLog);
			}
			else if (exceptionLog.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)exceptionLog.Row["ExceptionLogID", DataRowVersion.Original];
			  deleteCommand.Parameters["ExceptionLogID"].Value = id;
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

      foreach (ExceptionLog exceptionLog in this)
      {
        if (exceptionLog.Row.Table.Columns.Contains("CreatorID") && (int)exceptionLog.Row["CreatorID"] == 0) exceptionLog.Row["CreatorID"] = LoginUser.UserID;
        if (exceptionLog.Row.Table.Columns.Contains("ModifierID")) exceptionLog.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ExceptionLog FindByExceptionLogID(int exceptionLogID)
    {
      foreach (ExceptionLog exceptionLog in this)
      {
        if (exceptionLog.ExceptionLogID == exceptionLogID)
        {
          return exceptionLog;
        }
      }
      return null;
    }

    public virtual ExceptionLog AddNewExceptionLog()
    {
      if (Table.Columns.Count < 1) LoadColumns("ExceptionLogs");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ExceptionLog(row, this);
    }
    
    public virtual void LoadByExceptionLogID(int exceptionLogID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ExceptionLogID], [URL], [PageInfo], [ExceptionName], [Message], [StackTrace], [Browser], [CreatorID], [DateCreated], [ModifierID], [DateModified] FROM [dbo].[ExceptionLogs] WHERE ([ExceptionLogID] = @ExceptionLogID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ExceptionLogID", exceptionLogID);
        Fill(command);
      }
    }
    
    public static ExceptionLog GetExceptionLog(LoginUser loginUser, int exceptionLogID)
    {
      ExceptionLogs exceptionLogs = new ExceptionLogs(loginUser);
      exceptionLogs.LoadByExceptionLogID(exceptionLogID);
      if (exceptionLogs.IsEmpty)
        return null;
      else
        return exceptionLogs[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ExceptionLog> Members

    public IEnumerator<ExceptionLog> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ExceptionLog(row, this);
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
