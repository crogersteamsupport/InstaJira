using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ChatSetting : BaseItem
  {
    private ChatSettings _chatSettings;
    
    public ChatSetting(DataRow row, ChatSettings chatSettings): base(row, chatSettings)
    {
      _chatSettings = chatSettings;
    }
	
    #region Properties
    
    public ChatSettings Collection
    {
      get { return _chatSettings; }
    }
        
    
    
    

    

    
    public string ClientCss
    {
      get { return (string)Row["ClientCss"]; }
      set { Row["ClientCss"] = CheckValue("ClientCss", value); }
    }
    
    public bool UseCss
    {
      get { return (bool)Row["UseCss"]; }
      set { Row["UseCss"] = CheckValue("UseCss", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ChatSettings : BaseCollection, IEnumerable<ChatSetting>
  {
    public ChatSettings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ChatSettings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public ChatSetting this[int index]
    {
      get { return new ChatSetting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ChatSetting chatSetting);
    partial void AfterRowInsert(ChatSetting chatSetting);
    partial void BeforeRowEdit(ChatSetting chatSetting);
    partial void AfterRowEdit(ChatSetting chatSetting);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public ChatSettingProxy[] GetChatSettingProxies()
    {
      List<ChatSettingProxy> list = new List<ChatSettingProxy>();

      foreach (ChatSetting item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatSettings] WHERE ([OrganizationID] = @OrganizationID);";
        deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationID"].Value = organizationID;

        BeforeDBDelete(organizationID);
        BeforeRowDelete(organizationID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(organizationID);
        AfterDBDelete(organizationID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ChatSettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ChatSettings] SET     [UseCss] = @UseCss,    [ClientCss] = @ClientCss  WHERE ([OrganizationID] = @OrganizationID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseCss", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ClientCss", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ChatSettings] (    [OrganizationID],    [UseCss],    [ClientCss]) VALUES ( @OrganizationID, @UseCss, @ClientCss); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ClientCss", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseCss", SqlDbType.Bit, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatSettings] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (ChatSetting chatSetting in this)
		  {
			if (chatSetting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(chatSetting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = chatSetting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				chatSetting.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(chatSetting);
			}
			else if (chatSetting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(chatSetting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = chatSetting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(chatSetting);
			}
			else if (chatSetting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)chatSetting.Row["OrganizationID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationID"].Value = id;
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

      foreach (ChatSetting chatSetting in this)
      {
        if (chatSetting.Row.Table.Columns.Contains("CreatorID") && (int)chatSetting.Row["CreatorID"] == 0) chatSetting.Row["CreatorID"] = LoginUser.UserID;
        if (chatSetting.Row.Table.Columns.Contains("ModifierID")) chatSetting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ChatSetting FindByOrganizationID(int organizationID)
    {
      foreach (ChatSetting chatSetting in this)
      {
        if (chatSetting.OrganizationID == organizationID)
        {
          return chatSetting;
        }
      }
      return null;
    }

    public virtual ChatSetting AddNewChatSetting()
    {
      if (Table.Columns.Count < 1) LoadColumns("ChatSettings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ChatSetting(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [UseCss], [ClientCss] FROM [dbo].[ChatSettings] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static ChatSetting GetChatSetting(LoginUser loginUser, int organizationID)
    {
      ChatSettings chatSettings = new ChatSettings(loginUser);
      chatSettings.LoadByOrganizationID(organizationID);
      if (chatSettings.IsEmpty)
        return null;
      else
        return chatSettings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ChatSetting> Members

    public IEnumerator<ChatSetting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ChatSetting(row, this);
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
