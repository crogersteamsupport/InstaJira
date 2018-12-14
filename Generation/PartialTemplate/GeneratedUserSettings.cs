using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class UserSetting : BaseItem
  {
    private UserSettings _userSettings;
    
    public UserSetting(DataRow row, UserSettings userSettings): base(row, userSettings)
    {
      _userSettings = userSettings;
    }
	
    #region Properties
    
    public UserSettings Collection
    {
      get { return _userSettings; }
    }
        
    
    
    
    public int UserSettingID
    {
      get { return (int)Row["UserSettingID"]; }
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
    
    public string SettingValue
    {
      get { return (string)Row["SettingValue"]; }
      set { Row["SettingValue"] = CheckValue("SettingValue", value); }
    }
    
    public string SettingKey
    {
      get { return (string)Row["SettingKey"]; }
      set { Row["SettingKey"] = CheckValue("SettingKey", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
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

  public partial class UserSettings : BaseCollection, IEnumerable<UserSetting>
  {
    public UserSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "UserSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserSettingID"; }
    }



    public UserSetting this[int index]
    {
      get { return new UserSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(UserSetting userSetting);
    partial void AfterRowInsert(UserSetting userSetting);
    partial void BeforeRowEdit(UserSetting userSetting);
    partial void AfterRowEdit(UserSetting userSetting);
    partial void BeforeRowDelete(int userSettingID);
    partial void AfterRowDelete(int userSettingID);    

    partial void BeforeDBDelete(int userSettingID);
    partial void AfterDBDelete(int userSettingID);    

    #endregion

    #region Public Methods

    public UserSettingProxy[] GetUserSettingProxies()
    {
      List<UserSettingProxy> list = new List<UserSettingProxy>();

      foreach (UserSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userSettingID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserSettings] WHERE ([UserSettingID] = @UserSettingID);";
        deleteCommand.Parameters.Add("UserSettingID", SqlDbType.Int);
        deleteCommand.Parameters["UserSettingID"].Value = userSettingID;

        BeforeDBDelete(userSettingID);
        BeforeRowDelete(userSettingID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(userSettingID);
        AfterDBDelete(userSettingID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("UserSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[UserSettings] SET     [UserID] = @UserID,    [SettingKey] = @SettingKey,    [SettingValue] = @SettingValue,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([UserSettingID] = @UserSettingID);";

		
		tempParameter = updateCommand.Parameters.Add("UserSettingID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("SettingKey", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SettingValue", SqlDbType.VarChar, -1);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[UserSettings] (    [UserID],    [SettingKey],    [SettingValue],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @UserID, @SettingKey, @SettingValue, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("SettingValue", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SettingKey", SqlDbType.VarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserSettings] WHERE ([UserSettingID] = @UserSettingID);";
		deleteCommand.Parameters.Add("UserSettingID", SqlDbType.Int);

		try
		{
		  foreach (UserSetting userSetting in this)
		  {
			if (userSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(userSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = userSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserSettingID"].AutoIncrement = false;
			  Table.Columns["UserSettingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				userSetting.Row["UserSettingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(userSetting);
			}
			else if (userSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(userSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = userSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(userSetting);
			}
			else if (userSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)userSetting.Row["UserSettingID", DataRowVersion.Original];
			  deleteCommand.Parameters["UserSettingID"].Value = id;
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

      foreach (UserSetting userSetting in this)
      {
        if (userSetting.Row.Table.Columns.Contains("CreatorID") && (int)userSetting.Row["CreatorID"] == 0) userSetting.Row["CreatorID"] = LoginUser.UserID;
        if (userSetting.Row.Table.Columns.Contains("ModifierID")) userSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public UserSetting FindByUserSettingID(int userSettingID)
    {
      foreach (UserSetting userSetting in this)
      {
        if (userSetting.UserSettingID == userSettingID)
        {
          return userSetting;
        }
      }
      return null;
    }

    public virtual UserSetting AddNewUserSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("UserSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new UserSetting(row, this);
    }
    
    public virtual void LoadByUserSettingID(int userSettingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserSettingID], [UserID], [SettingKey], [SettingValue], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[UserSettings] WHERE ([UserSettingID] = @UserSettingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserSettingID", userSettingID);
        Fill(command);
      }
    }
    
    public static UserSetting GetUserSetting(LoginUser loginUser, int userSettingID)
    {
      UserSettings userSettings = new UserSettings(loginUser);
      userSettings.LoadByUserSettingID(userSettingID);
      if (userSettings.IsEmpty)
        return null;
      else
        return userSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<UserSetting> Members

    public IEnumerator<UserSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new UserSetting(row, this);
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
