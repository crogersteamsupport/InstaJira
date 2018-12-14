using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SourceCommitLog : BaseItem
  {
    private SourceCommitLogs _sourceCommitLogs;
    
    public SourceCommitLog(DataRow row, SourceCommitLogs sourceCommitLogs): base(row, sourceCommitLogs)
    {
      _sourceCommitLogs = sourceCommitLogs;
    }
	
    #region Properties
    
    public SourceCommitLogs Collection
    {
      get { return _sourceCommitLogs; }
    }
        
    
    
    
    public int CommitID
    {
      get { return (int)Row["CommitID"]; }
    }
    

    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int? ProductID
    {
      get { return Row["ProductID"] != DBNull.Value ? (int?)Row["ProductID"] : null; }
      set { Row["ProductID"] = CheckValue("ProductID", value); }
    }
    
    public int? VersionID
    {
      get { return Row["VersionID"] != DBNull.Value ? (int?)Row["VersionID"] : null; }
      set { Row["VersionID"] = CheckValue("VersionID", value); }
    }
    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
      set { Row["UserName"] = CheckValue("UserName", value); }
    }
    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int? Revision
    {
      get { return Row["Revision"] != DBNull.Value ? (int?)Row["Revision"] : null; }
      set { Row["Revision"] = CheckValue("Revision", value); }
    }
    
    public string Tickets
    {
      get { return Row["Tickets"] != DBNull.Value ? (string)Row["Tickets"] : null; }
      set { Row["Tickets"] = CheckValue("Tickets", value); }
    }
    
    public string RawCommitText
    {
      get { return Row["RawCommitText"] != DBNull.Value ? (string)Row["RawCommitText"] : null; }
      set { Row["RawCommitText"] = CheckValue("RawCommitText", value); }
    }
    
    public string Status
    {
      get { return Row["Status"] != DBNull.Value ? (string)Row["Status"] : null; }
      set { Row["Status"] = CheckValue("Status", value); }
    }
    

    

    /* DateTime */
    
    
    

    
    public DateTime? CommitDateTime
    {
      get { return Row["CommitDateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["CommitDateTime"]) : null; }
      set { Row["CommitDateTime"] = CheckValue("CommitDateTime", value); }
    }

    public DateTime? CommitDateTimeUtc
    {
      get { return Row["CommitDateTime"] != DBNull.Value ? (DateTime?)Row["CommitDateTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class SourceCommitLogs : BaseCollection, IEnumerable<SourceCommitLog>
  {
    public SourceCommitLogs(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SourceCommitLog"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CommitID"; }
    }



    public SourceCommitLog this[int index]
    {
      get { return new SourceCommitLog(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SourceCommitLog sourceCommitLog);
    partial void AfterRowInsert(SourceCommitLog sourceCommitLog);
    partial void BeforeRowEdit(SourceCommitLog sourceCommitLog);
    partial void AfterRowEdit(SourceCommitLog sourceCommitLog);
    partial void BeforeRowDelete(int commitID);
    partial void AfterRowDelete(int commitID);    

    partial void BeforeDBDelete(int commitID);
    partial void AfterDBDelete(int commitID);    

    #endregion

    #region Public Methods

    public SourceCommitLogProxy[] GetSourceCommitLogProxies()
    {
      List<SourceCommitLogProxy> list = new List<SourceCommitLogProxy>();

      foreach (SourceCommitLog item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int commitID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SourceCommitLog] WHERE ([CommitID] = @CommitID);";
        deleteCommand.Parameters.Add("CommitID", SqlDbType.Int);
        deleteCommand.Parameters["CommitID"].Value = commitID;

        BeforeDBDelete(commitID);
        BeforeRowDelete(commitID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(commitID);
        AfterDBDelete(commitID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SourceCommitLogsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SourceCommitLog] SET     [CommitDateTime] = @CommitDateTime,    [OrganizationID] = @OrganizationID,    [ProductID] = @ProductID,    [VersionID] = @VersionID,    [UserName] = @UserName,    [Description] = @Description,    [Revision] = @Revision,    [Tickets] = @Tickets,    [RawCommitText] = @RawCommitText,    [Status] = @Status  WHERE ([CommitID] = @CommitID);";

		
		tempParameter = updateCommand.Parameters.Add("CommitID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CommitDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("VersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserName", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Revision", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Tickets", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RawCommitText", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Status", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SourceCommitLog] (    [CommitDateTime],    [OrganizationID],    [ProductID],    [VersionID],    [UserName],    [Description],    [Revision],    [Tickets],    [RawCommitText],    [Status]) VALUES ( @CommitDateTime, @OrganizationID, @ProductID, @VersionID, @UserName, @Description, @Revision, @Tickets, @RawCommitText, @Status); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Status", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RawCommitText", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Tickets", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Revision", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserName", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("VersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("CommitDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SourceCommitLog] WHERE ([CommitID] = @CommitID);";
		deleteCommand.Parameters.Add("CommitID", SqlDbType.Int);

		try
		{
		  foreach (SourceCommitLog sourceCommitLog in this)
		  {
			if (sourceCommitLog.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(sourceCommitLog);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = sourceCommitLog.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CommitID"].AutoIncrement = false;
			  Table.Columns["CommitID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				sourceCommitLog.Row["CommitID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(sourceCommitLog);
			}
			else if (sourceCommitLog.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(sourceCommitLog);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = sourceCommitLog.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(sourceCommitLog);
			}
			else if (sourceCommitLog.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)sourceCommitLog.Row["CommitID", DataRowVersion.Original];
			  deleteCommand.Parameters["CommitID"].Value = id;
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

      foreach (SourceCommitLog sourceCommitLog in this)
      {
        if (sourceCommitLog.Row.Table.Columns.Contains("CreatorID") && (int)sourceCommitLog.Row["CreatorID"] == 0) sourceCommitLog.Row["CreatorID"] = LoginUser.UserID;
        if (sourceCommitLog.Row.Table.Columns.Contains("ModifierID")) sourceCommitLog.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SourceCommitLog FindByCommitID(int commitID)
    {
      foreach (SourceCommitLog sourceCommitLog in this)
      {
        if (sourceCommitLog.CommitID == commitID)
        {
          return sourceCommitLog;
        }
      }
      return null;
    }

    public virtual SourceCommitLog AddNewSourceCommitLog()
    {
      if (Table.Columns.Count < 1) LoadColumns("SourceCommitLog");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SourceCommitLog(row, this);
    }
    
    public virtual void LoadByCommitID(int commitID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CommitID], [CommitDateTime], [OrganizationID], [ProductID], [VersionID], [UserName], [Description], [Revision], [Tickets], [RawCommitText], [Status] FROM [dbo].[SourceCommitLog] WHERE ([CommitID] = @CommitID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CommitID", commitID);
        Fill(command);
      }
    }
    
    public static SourceCommitLog GetSourceCommitLog(LoginUser loginUser, int commitID)
    {
      SourceCommitLogs sourceCommitLogs = new SourceCommitLogs(loginUser);
      sourceCommitLogs.LoadByCommitID(commitID);
      if (sourceCommitLogs.IsEmpty)
        return null;
      else
        return sourceCommitLogs[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SourceCommitLog> Members

    public IEnumerator<SourceCommitLog> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SourceCommitLog(row, this);
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
