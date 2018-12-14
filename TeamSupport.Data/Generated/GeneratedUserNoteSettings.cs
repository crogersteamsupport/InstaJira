using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class UserNoteSetting : BaseItem
  {
    private UserNoteSettings _userNoteSettings;
    
    public UserNoteSetting(DataRow row, UserNoteSettings userNoteSettings): base(row, userNoteSettings)
    {
      _userNoteSettings = userNoteSettings;
    }
	
    #region Properties
    
    public UserNoteSettings Collection
    {
      get { return _userNoteSettings; }
    }
        
    
    
    

    

    
    public bool IsSnoozed
    {
      get { return (bool)Row["IsSnoozed"]; }
      set { Row["IsSnoozed"] = CheckValue("IsSnoozed", value); }
    }
    
    public bool IsDismissed
    {
      get { return (bool)Row["IsDismissed"]; }
      set { Row["IsDismissed"] = CheckValue("IsDismissed", value); }
    }
    
    public int RefID
    {
        get { return (int)Row["RefID"]; }
        set { Row["RefID"] = CheckValue("RefID", value); }
    }

    public ReferenceType RefType
    {
        get { return (ReferenceType)Row["RefType"]; }
        set { Row["RefType"] = CheckValue("RefType", value); }
    }

    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime SnoozeTime
    {
      get { return DateToLocal((DateTime)Row["SnoozeTime"]); }
      set { Row["SnoozeTime"] = CheckValue("SnoozeTime", value); }
    }

    public DateTime SnoozeTimeUtc
    {
      get { return (DateTime)Row["SnoozeTime"]; }
    }
    

    #endregion
    
    
  }

  public partial class UserNoteSettings : BaseCollection, IEnumerable<UserNoteSetting>
  {
    public UserNoteSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "UserNoteSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserID"; }
    }



    public UserNoteSetting this[int index]
    {
      get { return new UserNoteSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(UserNoteSetting userNoteSetting);
    partial void AfterRowInsert(UserNoteSetting userNoteSetting);
    partial void BeforeRowEdit(UserNoteSetting userNoteSetting);
    partial void AfterRowEdit(UserNoteSetting userNoteSetting);
    partial void BeforeRowDelete(int userID);
    partial void AfterRowDelete(int userID);    

    partial void BeforeDBDelete(int userID);
    partial void AfterDBDelete(int userID);    

    #endregion

    #region Public Methods

    public UserNoteSettingProxy[] GetUserNoteSettingProxies()
    {
      List<UserNoteSettingProxy> list = new List<UserNoteSettingProxy>();

      foreach (UserNoteSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userID)
    {
      BeforeDBDelete(userID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserNoteSettings] WHERE ([UserID] = @UserID);";
        deleteCommand.Parameters.Add("UserID", SqlDbType.Int);
        deleteCommand.Parameters["UserID"].Value = userID;

        BeforeRowDelete(userID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(userID);
      }
      AfterDBDelete(userID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("UserNoteSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
        updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[UserNoteSettings] SET     [RefID] = @RefID, [RefType] = @RefType,    [IsDismissed] = @IsDismissed,    [IsSnoozed] = @IsSnoozed,    [SnoozeTime] = @SnoozeTime  WHERE ([UserID] = @UserID and [RefID] = @RefID);";

		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("IsDismissed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsSnoozed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SnoozeTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
        insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[UserNoteSettings] (    [UserID],  [RefID],  [RefType],    [IsDismissed],    [IsSnoozed],    [SnoozeTime]) VALUES ( @UserID, @RefID, @RefType, @IsDismissed, @IsSnoozed, @SnoozeTime); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("SnoozeTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsSnoozed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsDismissed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserNoteSettings] WHERE ([UserID] = @UserID);";
		deleteCommand.Parameters.Add("UserID", SqlDbType.Int);

		try
		{
		  foreach (UserNoteSetting userNoteSetting in this)
		  {
			if (userNoteSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(userNoteSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = userNoteSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserID"].AutoIncrement = false;
			  Table.Columns["UserID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				userNoteSetting.Row["UserID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(userNoteSetting);
			}
			else if (userNoteSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(userNoteSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = userNoteSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(userNoteSetting);
			}
			else if (userNoteSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)userNoteSetting.Row["UserID", DataRowVersion.Original];
			  deleteCommand.Parameters["UserID"].Value = id;
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

      foreach (UserNoteSetting userNoteSetting in this)
      {
        if (userNoteSetting.Row.Table.Columns.Contains("CreatorID") && (int)userNoteSetting.Row["CreatorID"] == 0) userNoteSetting.Row["CreatorID"] = LoginUser.UserID;
        if (userNoteSetting.Row.Table.Columns.Contains("ModifierID")) userNoteSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public UserNoteSetting FindByUserID(int userID)
    {
      foreach (UserNoteSetting userNoteSetting in this)
      {
        if (userNoteSetting.UserID == userID)
        {
          return userNoteSetting;
        }
      }
      return null;
    }

    public virtual UserNoteSetting AddNewUserNoteSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("UserNoteSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new UserNoteSetting(row, this);
    }
    
    public virtual void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserID], [RefID], [RefType], [IsDismissed], [IsSnoozed], [SnoozeTime] FROM [dbo].[UserNoteSettings] WHERE ([UserID] = @UserID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserID", userID);
        Fill(command);
      }
    }

    public virtual void LoadByUserNoteID(int userID, int refID, ReferenceType refType)
    {   
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SET NOCOUNT OFF; SELECT [UserID], [RefID], [RefType], [IsDismissed], [IsSnoozed], [SnoozeTime] FROM [dbo].[UserNoteSettings] WHERE ([UserID] = @UserID and [RefID] = @RefID and [RefType] = @RefType);";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("UserID", userID);
            command.Parameters.AddWithValue("RefID", refID);
            command.Parameters.AddWithValue("RefType", refType);
            Fill(command);
        }
    }

    public virtual void LoadByIDType(int refID, ReferenceType refType)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SET NOCOUNT OFF; SELECT [UserID], [RefID], [RefType], [IsDismissed], [IsSnoozed], [SnoozeTime] FROM [dbo].[UserNoteSettings] WHERE ( [RefID] = @RefID and [RefType] = @RefType);";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("RefID", refID);
            command.Parameters.AddWithValue("RefType", refType);
            Fill(command);
        }
    }

    public static UserNoteSetting GetUserNoteSetting(LoginUser loginUser, int userID)
    {
      UserNoteSettings userNoteSettings = new UserNoteSettings(loginUser);
      userNoteSettings.LoadByUserID(userID);
      if (userNoteSettings.IsEmpty)
        return null;
      else
        return userNoteSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<UserNoteSetting> Members

    public IEnumerator<UserNoteSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new UserNoteSetting(row, this);
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
