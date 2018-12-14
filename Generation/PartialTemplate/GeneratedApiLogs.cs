using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ApiLog : BaseItem
  {
    private ApiLogs _apiLogs;
    
    public ApiLog(DataRow row, ApiLogs apiLogs): base(row, apiLogs)
    {
      _apiLogs = apiLogs;
    }
	
    #region Properties
    
    public ApiLogs Collection
    {
      get { return _apiLogs; }
    }
        
    
    
    
    public int ApiLogID
    {
      get { return (int)Row["ApiLogID"]; }
    }
    

    
    public string RequestBody
    {
      get { return Row["RequestBody"] != DBNull.Value ? (string)Row["RequestBody"] : null; }
      set { Row["RequestBody"] = CheckValue("RequestBody", value); }
    }
    

    
    public int StatusCode
    {
      get { return (int)Row["StatusCode"]; }
      set { Row["StatusCode"] = CheckValue("StatusCode", value); }
    }
    
    public string Verb
    {
      get { return (string)Row["Verb"]; }
      set { Row["Verb"] = CheckValue("Verb", value); }
    }
    
    public string Url
    {
      get { return (string)Row["Url"]; }
      set { Row["Url"] = CheckValue("Url", value); }
    }
    
    public string IPAddress
    {
      get { return (string)Row["IPAddress"]; }
      set { Row["IPAddress"] = CheckValue("IPAddress", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
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

  public partial class ApiLogs : BaseCollection, IEnumerable<ApiLog>
  {
    public ApiLogs(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ApiLogs"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ApiLogID"; }
    }



    public ApiLog this[int index]
    {
      get { return new ApiLog(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ApiLog apiLog);
    partial void AfterRowInsert(ApiLog apiLog);
    partial void BeforeRowEdit(ApiLog apiLog);
    partial void AfterRowEdit(ApiLog apiLog);
    partial void BeforeRowDelete(int apiLogID);
    partial void AfterRowDelete(int apiLogID);    

    partial void BeforeDBDelete(int apiLogID);
    partial void AfterDBDelete(int apiLogID);    

    #endregion

    #region Public Methods

    public ApiLogProxy[] GetApiLogProxies()
    {
      List<ApiLogProxy> list = new List<ApiLogProxy>();

      foreach (ApiLog item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int apiLogID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ApiLogs] WHERE ([ApiLogID] = @ApiLogID);";
        deleteCommand.Parameters.Add("ApiLogID", SqlDbType.Int);
        deleteCommand.Parameters["ApiLogID"].Value = apiLogID;

        BeforeDBDelete(apiLogID);
        BeforeRowDelete(apiLogID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(apiLogID);
        AfterDBDelete(apiLogID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ApiLogsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ApiLogs] SET     [OrganizationID] = @OrganizationID,    [IPAddress] = @IPAddress,    [Url] = @Url,    [Verb] = @Verb,    [StatusCode] = @StatusCode,    [RequestBody] = @RequestBody  WHERE ([ApiLogID] = @ApiLogID);";

		
		tempParameter = updateCommand.Parameters.Add("ApiLogID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("IPAddress", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Url", SqlDbType.VarChar, 2100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Verb", SqlDbType.VarChar, 20);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("StatusCode", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequestBody", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ApiLogs] (    [OrganizationID],    [IPAddress],    [Url],    [Verb],    [StatusCode],    [RequestBody],    [DateCreated]) VALUES ( @OrganizationID, @IPAddress, @Url, @Verb, @StatusCode, @RequestBody, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequestBody", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("StatusCode", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Verb", SqlDbType.VarChar, 20);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Url", SqlDbType.VarChar, 2100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IPAddress", SqlDbType.VarChar, 50);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ApiLogs] WHERE ([ApiLogID] = @ApiLogID);";
		deleteCommand.Parameters.Add("ApiLogID", SqlDbType.Int);

		try
		{
		  foreach (ApiLog apiLog in this)
		  {
			if (apiLog.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(apiLog);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = apiLog.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ApiLogID"].AutoIncrement = false;
			  Table.Columns["ApiLogID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				apiLog.Row["ApiLogID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(apiLog);
			}
			else if (apiLog.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(apiLog);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = apiLog.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(apiLog);
			}
			else if (apiLog.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)apiLog.Row["ApiLogID", DataRowVersion.Original];
			  deleteCommand.Parameters["ApiLogID"].Value = id;
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

      foreach (ApiLog apiLog in this)
      {
        if (apiLog.Row.Table.Columns.Contains("CreatorID") && (int)apiLog.Row["CreatorID"] == 0) apiLog.Row["CreatorID"] = LoginUser.UserID;
        if (apiLog.Row.Table.Columns.Contains("ModifierID")) apiLog.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ApiLog FindByApiLogID(int apiLogID)
    {
      foreach (ApiLog apiLog in this)
      {
        if (apiLog.ApiLogID == apiLogID)
        {
          return apiLog;
        }
      }
      return null;
    }

    public virtual ApiLog AddNewApiLog()
    {
      if (Table.Columns.Count < 1) LoadColumns("ApiLogs");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ApiLog(row, this);
    }
    
    public virtual void LoadByApiLogID(int apiLogID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ApiLogID], [OrganizationID], [IPAddress], [Url], [Verb], [StatusCode], [RequestBody], [DateCreated] FROM [dbo].[ApiLogs] WHERE ([ApiLogID] = @ApiLogID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ApiLogID", apiLogID);
        Fill(command);
      }
    }
    
    public static ApiLog GetApiLog(LoginUser loginUser, int apiLogID)
    {
      ApiLogs apiLogs = new ApiLogs(loginUser);
      apiLogs.LoadByApiLogID(apiLogID);
      if (apiLogs.IsEmpty)
        return null;
      else
        return apiLogs[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ApiLog> Members

    public IEnumerator<ApiLog> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ApiLog(row, this);
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
