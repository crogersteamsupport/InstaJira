using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class LoginHistoryItem : BaseItem
  {
    private LoginHistory _loginHistory;
    
    public LoginHistoryItem(DataRow row, LoginHistory loginHistory): base(row, loginHistory)
    {
      _loginHistory = loginHistory;
    }
	
    #region Properties
    
    public LoginHistory Collection
    {
      get { return _loginHistory; }
    }
        
    
    
    
    public int LoginHistoryID
    {
      get { return (int)Row["LoginHistoryID"]; }
    }
    

    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
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
    
    public string Language
    {
      get { return Row["Language"] != DBNull.Value ? (string)Row["Language"] : null; }
      set { Row["Language"] = CheckValue("Language", value); }
    }
    
    public string PixelDepth
    {
      get { return Row["PixelDepth"] != DBNull.Value ? (string)Row["PixelDepth"] : null; }
      set { Row["PixelDepth"] = CheckValue("PixelDepth", value); }
    }
    
    public string ScreenHeight
    {
      get { return Row["ScreenHeight"] != DBNull.Value ? (string)Row["ScreenHeight"] : null; }
      set { Row["ScreenHeight"] = CheckValue("ScreenHeight", value); }
    }
    
    public string ScreenWidth
    {
      get { return Row["ScreenWidth"] != DBNull.Value ? (string)Row["ScreenWidth"] : null; }
      set { Row["ScreenWidth"] = CheckValue("ScreenWidth", value); }
    }
    
    public string URL
    {
      get { return Row["URL"] != DBNull.Value ? (string)Row["URL"] : null; }
      set { Row["URL"] = CheckValue("URL", value); }
    }
    
    public string DeviceID
    {
      get { return Row["DeviceID"] != DBNull.Value ? (string)Row["DeviceID"] : null; }
      set { Row["DeviceID"] = CheckValue("DeviceID", value); }
    }
    

    
    public bool IsSupport
    {
      get { return (bool)Row["IsSupport"]; }
      set { Row["IsSupport"] = CheckValue("IsSupport", value); }
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

  public partial class LoginHistory : BaseCollection, IEnumerable<LoginHistoryItem>
  {
    public LoginHistory(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "LoginHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "LoginHistoryID"; }
    }



    public LoginHistoryItem this[int index]
    {
      get { return new LoginHistoryItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(LoginHistoryItem loginHistoryItem);
    partial void AfterRowInsert(LoginHistoryItem loginHistoryItem);
    partial void BeforeRowEdit(LoginHistoryItem loginHistoryItem);
    partial void AfterRowEdit(LoginHistoryItem loginHistoryItem);
    partial void BeforeRowDelete(int loginHistoryID);
    partial void AfterRowDelete(int loginHistoryID);    

    partial void BeforeDBDelete(int loginHistoryID);
    partial void AfterDBDelete(int loginHistoryID);    

    #endregion

    #region Public Methods

    public LoginHistoryItemProxy[] GetLoginHistoryItemProxies()
    {
      List<LoginHistoryItemProxy> list = new List<LoginHistoryItemProxy>();

      foreach (LoginHistoryItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int loginHistoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[LoginHistory] WHERE ([LoginHistoryID] = @LoginHistoryID);";
        deleteCommand.Parameters.Add("LoginHistoryID", SqlDbType.Int);
        deleteCommand.Parameters["LoginHistoryID"].Value = loginHistoryID;

        BeforeDBDelete(loginHistoryID);
        BeforeRowDelete(loginHistoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(loginHistoryID);
        AfterDBDelete(loginHistoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("LoginHistorySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[LoginHistory] SET     [UserID] = @UserID,    [IPAddress] = @IPAddress,    [Browser] = @Browser,    [Version] = @Version,    [MajorVersion] = @MajorVersion,    [CookiesEnabled] = @CookiesEnabled,    [Platform] = @Platform,    [UserAgent] = @UserAgent,    [Language] = @Language,    [PixelDepth] = @PixelDepth,    [ScreenHeight] = @ScreenHeight,    [ScreenWidth] = @ScreenWidth,    [URL] = @URL,    [IsSupport] = @IsSupport,    [DeviceID] = @DeviceID  WHERE ([LoginHistoryID] = @LoginHistoryID);";

		
		tempParameter = updateCommand.Parameters.Add("LoginHistoryID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Language", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PixelDepth", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ScreenHeight", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ScreenWidth", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("URL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsSupport", SqlDbType.Bit, 1);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[LoginHistory] (    [UserID],    [IPAddress],    [Browser],    [Version],    [MajorVersion],    [CookiesEnabled],    [Platform],    [UserAgent],    [Language],    [PixelDepth],    [ScreenHeight],    [ScreenWidth],    [URL],    [DateCreated],    [IsSupport],    [DeviceID]) VALUES ( @UserID, @IPAddress, @Browser, @Version, @MajorVersion, @CookiesEnabled, @Platform, @UserAgent, @Language, @PixelDepth, @ScreenHeight, @ScreenWidth, @URL, @DateCreated, @IsSupport, @DeviceID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DeviceID", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsSupport", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("URL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ScreenWidth", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ScreenHeight", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PixelDepth", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Language", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[LoginHistory] WHERE ([LoginHistoryID] = @LoginHistoryID);";
		deleteCommand.Parameters.Add("LoginHistoryID", SqlDbType.Int);

		try
		{
		  foreach (LoginHistoryItem loginHistoryItem in this)
		  {
			if (loginHistoryItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(loginHistoryItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = loginHistoryItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["LoginHistoryID"].AutoIncrement = false;
			  Table.Columns["LoginHistoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				loginHistoryItem.Row["LoginHistoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(loginHistoryItem);
			}
			else if (loginHistoryItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(loginHistoryItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = loginHistoryItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(loginHistoryItem);
			}
			else if (loginHistoryItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)loginHistoryItem.Row["LoginHistoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["LoginHistoryID"].Value = id;
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

      foreach (LoginHistoryItem loginHistoryItem in this)
      {
        if (loginHistoryItem.Row.Table.Columns.Contains("CreatorID") && (int)loginHistoryItem.Row["CreatorID"] == 0) loginHistoryItem.Row["CreatorID"] = LoginUser.UserID;
        if (loginHistoryItem.Row.Table.Columns.Contains("ModifierID")) loginHistoryItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public LoginHistoryItem FindByLoginHistoryID(int loginHistoryID)
    {
      foreach (LoginHistoryItem loginHistoryItem in this)
      {
        if (loginHistoryItem.LoginHistoryID == loginHistoryID)
        {
          return loginHistoryItem;
        }
      }
      return null;
    }

    public virtual LoginHistoryItem AddNewLoginHistoryItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("LoginHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new LoginHistoryItem(row, this);
    }
    
    public virtual void LoadByLoginHistoryID(int loginHistoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [LoginHistoryID], [UserID], [IPAddress], [Browser], [Version], [MajorVersion], [CookiesEnabled], [Platform], [UserAgent], [Language], [PixelDepth], [ScreenHeight], [ScreenWidth], [URL], [DateCreated], [IsSupport], [DeviceID] FROM [dbo].[LoginHistory] WHERE ([LoginHistoryID] = @LoginHistoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("LoginHistoryID", loginHistoryID);
        Fill(command);
      }
    }
    
    public static LoginHistoryItem GetLoginHistoryItem(LoginUser loginUser, int loginHistoryID)
    {
      LoginHistory loginHistory = new LoginHistory(loginUser);
      loginHistory.LoadByLoginHistoryID(loginHistoryID);
      if (loginHistory.IsEmpty)
        return null;
      else
        return loginHistory[0];
    }
    
    
    

    #endregion

    #region IEnumerable<LoginHistoryItem> Members

    public IEnumerator<LoginHistoryItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new LoginHistoryItem(row, this);
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
