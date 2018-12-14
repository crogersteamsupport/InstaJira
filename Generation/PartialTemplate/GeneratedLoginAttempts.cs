using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class LoginAttempt : BaseItem
  {
    private LoginAttempts _loginAttempts;
    
    public LoginAttempt(DataRow row, LoginAttempts loginAttempts): base(row, loginAttempts)
    {
      _loginAttempts = loginAttempts;
    }
	
    #region Properties
    
    public LoginAttempts Collection
    {
      get { return _loginAttempts; }
    }
        
    
    
    
    public int LoginAttemptID
    {
      get { return (int)Row["LoginAttemptID"]; }
    }
    

    
    public string IPAddress
    {
      get { return Row["IPAddress"] != DBNull.Value ? (string)Row["IPAddress"] : null; }
      set { Row["IPAddress"] = CheckValue("IPAddress", value); }
    }
    
    public string Browser
    {
      get { return Row["Browser"] != DBNull.Value ? (string)Row["Browser"] : null; }
      set { Row["Browser"] = CheckValue("Browser", value); }
    }
    
    public string Version
    {
      get { return Row["Version"] != DBNull.Value ? (string)Row["Version"] : null; }
      set { Row["Version"] = CheckValue("Version", value); }
    }
    
    public string MajorVersion
    {
      get { return Row["MajorVersion"] != DBNull.Value ? (string)Row["MajorVersion"] : null; }
      set { Row["MajorVersion"] = CheckValue("MajorVersion", value); }
    }
    
    public bool? CookiesEnabled
    {
      get { return Row["CookiesEnabled"] != DBNull.Value ? (bool?)Row["CookiesEnabled"] : null; }
      set { Row["CookiesEnabled"] = CheckValue("CookiesEnabled", value); }
    }
    
    public string Platform
    {
      get { return Row["Platform"] != DBNull.Value ? (string)Row["Platform"] : null; }
      set { Row["Platform"] = CheckValue("Platform", value); }
    }
    
    public string UserAgent
    {
      get { return Row["UserAgent"] != DBNull.Value ? (string)Row["UserAgent"] : null; }
      set { Row["UserAgent"] = CheckValue("UserAgent", value); }
    }
    
    public string DeviceID
    {
      get { return Row["DeviceID"] != DBNull.Value ? (string)Row["DeviceID"] : null; }
      set { Row["DeviceID"] = CheckValue("DeviceID", value); }
    }
    

    
    public bool Successful
    {
      get { return (bool)Row["Successful"]; }
      set { Row["Successful"] = CheckValue("Successful", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
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

  public partial class LoginAttempts : BaseCollection, IEnumerable<LoginAttempt>
  {
    public LoginAttempts(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "LoginAttempts"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "LoginAttemptID"; }
    }



    public LoginAttempt this[int index]
    {
      get { return new LoginAttempt(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(LoginAttempt loginAttempt);
    partial void AfterRowInsert(LoginAttempt loginAttempt);
    partial void BeforeRowEdit(LoginAttempt loginAttempt);
    partial void AfterRowEdit(LoginAttempt loginAttempt);
    partial void BeforeRowDelete(int loginAttemptID);
    partial void AfterRowDelete(int loginAttemptID);    

    partial void BeforeDBDelete(int loginAttemptID);
    partial void AfterDBDelete(int loginAttemptID);    

    #endregion

    #region Public Methods

    public LoginAttemptProxy[] GetLoginAttemptProxies()
    {
      List<LoginAttemptProxy> list = new List<LoginAttemptProxy>();

      foreach (LoginAttempt item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int loginAttemptID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[LoginAttempts] WHERE ([LoginAttemptID] = @LoginAttemptID);";
        deleteCommand.Parameters.Add("LoginAttemptID", SqlDbType.Int);
        deleteCommand.Parameters["LoginAttemptID"].Value = loginAttemptID;

        BeforeDBDelete(loginAttemptID);
        BeforeRowDelete(loginAttemptID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(loginAttemptID);
        AfterDBDelete(loginAttemptID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("LoginAttemptsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[LoginAttempts] SET     [UserID] = @UserID,    [Successful] = @Successful,    [IPAddress] = @IPAddress,    [Browser] = @Browser,    [Version] = @Version,    [MajorVersion] = @MajorVersion,    [CookiesEnabled] = @CookiesEnabled,    [Platform] = @Platform,    [UserAgent] = @UserAgent,    [DeviceID] = @DeviceID  WHERE ([LoginAttemptID] = @LoginAttemptID);";

		
		tempParameter = updateCommand.Parameters.Add("LoginAttemptID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Successful", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IPAddress", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Browser", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Version", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MajorVersion", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CookiesEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Platform", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserAgent", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DeviceID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[LoginAttempts] (    [UserID],    [Successful],    [IPAddress],    [Browser],    [Version],    [MajorVersion],    [CookiesEnabled],    [Platform],    [UserAgent],    [DateCreated],    [DeviceID]) VALUES ( @UserID, @Successful, @IPAddress, @Browser, @Version, @MajorVersion, @CookiesEnabled, @Platform, @UserAgent, @DateCreated, @DeviceID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DeviceID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserAgent", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Platform", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CookiesEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MajorVersion", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Version", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Browser", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IPAddress", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Successful", SqlDbType.Bit, 1);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[LoginAttempts] WHERE ([LoginAttemptID] = @LoginAttemptID);";
		deleteCommand.Parameters.Add("LoginAttemptID", SqlDbType.Int);

		try
		{
		  foreach (LoginAttempt loginAttempt in this)
		  {
			if (loginAttempt.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(loginAttempt);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = loginAttempt.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["LoginAttemptID"].AutoIncrement = false;
			  Table.Columns["LoginAttemptID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				loginAttempt.Row["LoginAttemptID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(loginAttempt);
			}
			else if (loginAttempt.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(loginAttempt);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = loginAttempt.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(loginAttempt);
			}
			else if (loginAttempt.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)loginAttempt.Row["LoginAttemptID", DataRowVersion.Original];
			  deleteCommand.Parameters["LoginAttemptID"].Value = id;
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

      foreach (LoginAttempt loginAttempt in this)
      {
        if (loginAttempt.Row.Table.Columns.Contains("CreatorID") && (int)loginAttempt.Row["CreatorID"] == 0) loginAttempt.Row["CreatorID"] = LoginUser.UserID;
        if (loginAttempt.Row.Table.Columns.Contains("ModifierID")) loginAttempt.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public LoginAttempt FindByLoginAttemptID(int loginAttemptID)
    {
      foreach (LoginAttempt loginAttempt in this)
      {
        if (loginAttempt.LoginAttemptID == loginAttemptID)
        {
          return loginAttempt;
        }
      }
      return null;
    }

    public virtual LoginAttempt AddNewLoginAttempt()
    {
      if (Table.Columns.Count < 1) LoadColumns("LoginAttempts");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new LoginAttempt(row, this);
    }
    
    public virtual void LoadByLoginAttemptID(int loginAttemptID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [LoginAttemptID], [UserID], [Successful], [IPAddress], [Browser], [Version], [MajorVersion], [CookiesEnabled], [Platform], [UserAgent], [DateCreated], [DeviceID] FROM [dbo].[LoginAttempts] WHERE ([LoginAttemptID] = @LoginAttemptID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("LoginAttemptID", loginAttemptID);
        Fill(command);
      }
    }
    
    public static LoginAttempt GetLoginAttempt(LoginUser loginUser, int loginAttemptID)
    {
      LoginAttempts loginAttempts = new LoginAttempts(loginUser);
      loginAttempts.LoadByLoginAttemptID(loginAttemptID);
      if (loginAttempts.IsEmpty)
        return null;
      else
        return loginAttempts[0];
    }
    
    
    

    #endregion

    #region IEnumerable<LoginAttempt> Members

    public IEnumerator<LoginAttempt> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new LoginAttempt(row, this);
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
