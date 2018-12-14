using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ChatUserSetting : BaseItem
  {
    private ChatUserSettings _chatUserSettings;
    
    public ChatUserSetting(DataRow row, ChatUserSettings chatUserSettings): base(row, chatUserSettings)
    {
      _chatUserSettings = chatUserSettings;
    }
	
    #region Properties
    
    public ChatUserSettings Collection
    {
      get { return _chatUserSettings; }
    }
        
    
    
    

    

    
    public int LastChatRequestID
    {
      get { return (int)Row["LastChatRequestID"]; }
      set { Row["LastChatRequestID"] = CheckValue("LastChatRequestID", value); }
    }
    
    public bool IsAvailable
    {
      get { return (bool)Row["IsAvailable"]; }
      set { Row["IsAvailable"] = CheckValue("IsAvailable", value); }
    }
    
    public int CurrentChatID
    {
      get { return (int)Row["CurrentChatID"]; }
      set { Row["CurrentChatID"] = CheckValue("CurrentChatID", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ChatUserSettings : BaseCollection, IEnumerable<ChatUserSetting>
  {
    public ChatUserSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ChatUserSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserID"; }
    }



    public ChatUserSetting this[int index]
    {
      get { return new ChatUserSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ChatUserSetting chatUserSetting);
    partial void AfterRowInsert(ChatUserSetting chatUserSetting);
    partial void BeforeRowEdit(ChatUserSetting chatUserSetting);
    partial void AfterRowEdit(ChatUserSetting chatUserSetting);
    partial void BeforeRowDelete(int userID);
    partial void AfterRowDelete(int userID);    

    partial void BeforeDBDelete(int userID);
    partial void AfterDBDelete(int userID);    

    #endregion

    #region Public Methods

    public ChatUserSettingProxy[] GetChatUserSettingProxies()
    {
      List<ChatUserSettingProxy> list = new List<ChatUserSettingProxy>();

      foreach (ChatUserSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatUserSettings] WHERE ([UserID] = @UserID);";
        deleteCommand.Parameters.Add("UserID", SqlDbType.Int);
        deleteCommand.Parameters["UserID"].Value = userID;

        BeforeDBDelete(userID);
        BeforeRowDelete(userID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(userID);
        AfterDBDelete(userID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ChatUserSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ChatUserSettings] SET     [CurrentChatID] = @CurrentChatID,    [IsAvailable] = @IsAvailable,    [LastChatRequestID] = @LastChatRequestID  WHERE ([UserID] = @UserID);";

		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CurrentChatID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsAvailable", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastChatRequestID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ChatUserSettings] (    [UserID],    [CurrentChatID],    [IsAvailable],    [LastChatRequestID]) VALUES ( @UserID, @CurrentChatID, @IsAvailable, @LastChatRequestID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("LastChatRequestID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsAvailable", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CurrentChatID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatUserSettings] WHERE ([UserID] = @UserID);";
		deleteCommand.Parameters.Add("UserID", SqlDbType.Int);

		try
		{
		  foreach (ChatUserSetting chatUserSetting in this)
		  {
			if (chatUserSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(chatUserSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = chatUserSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserID"].AutoIncrement = false;
			  Table.Columns["UserID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				chatUserSetting.Row["UserID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(chatUserSetting);
			}
			else if (chatUserSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(chatUserSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = chatUserSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(chatUserSetting);
			}
			else if (chatUserSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)chatUserSetting.Row["UserID", DataRowVersion.Original];
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

      foreach (ChatUserSetting chatUserSetting in this)
      {
        if (chatUserSetting.Row.Table.Columns.Contains("CreatorID") && (int)chatUserSetting.Row["CreatorID"] == 0) chatUserSetting.Row["CreatorID"] = LoginUser.UserID;
        if (chatUserSetting.Row.Table.Columns.Contains("ModifierID")) chatUserSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ChatUserSetting FindByUserID(int userID)
    {
      foreach (ChatUserSetting chatUserSetting in this)
      {
        if (chatUserSetting.UserID == userID)
        {
          return chatUserSetting;
        }
      }
      return null;
    }

    public virtual ChatUserSetting AddNewChatUserSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("ChatUserSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ChatUserSetting(row, this);
    }
    
    public virtual void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserID], [CurrentChatID], [IsAvailable], [LastChatRequestID] FROM [dbo].[ChatUserSettings] WHERE ([UserID] = @UserID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserID", userID);
        Fill(command);
      }
    }
    
    public static ChatUserSetting GetChatUserSetting(LoginUser loginUser, int userID)
    {
      ChatUserSettings chatUserSettings = new ChatUserSettings(loginUser);
      chatUserSettings.LoadByUserID(userID);
      if (chatUserSettings.IsEmpty)
        return null;
      else
        return chatUserSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ChatUserSetting> Members

    public IEnumerator<ChatUserSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ChatUserSetting(row, this);
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
