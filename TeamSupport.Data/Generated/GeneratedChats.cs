using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Chat : BaseItem
  {
    private Chats _chats;
    
    public Chat(DataRow row, Chats chats): base(row, chats)
    {
      _chats = chats;
    }
	
    #region Properties
    
    public Chats Collection
    {
      get { return _chats; }
    }
        
    
    
    
    public int ChatID
    {
      get { return (int)Row["ChatID"]; }
    }
    

    
    public int? ActionID
    {
      get { return Row["ActionID"] != DBNull.Value ? (int?)Row["ActionID"] : null; }
      set { Row["ActionID"] = CheckValue("ActionID", value); }
    }
    

    
    public ChatParticipantType InitiatorType
    {
      get { return (ChatParticipantType)Row["InitiatorType"]; }
      set { Row["InitiatorType"] = CheckValue("InitiatorType", value); }
    }
    
    public int InitiatorID
    {
      get { return (int)Row["InitiatorID"]; }
      set { Row["InitiatorID"] = CheckValue("InitiatorID", value); }
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

  public partial class Chats : BaseCollection, IEnumerable<Chat>
  {
    public Chats(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Chats"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ChatID"; }
    }



    public Chat this[int index]
    {
      get { return new Chat(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Chat chat);
    partial void AfterRowInsert(Chat chat);
    partial void BeforeRowEdit(Chat chat);
    partial void AfterRowEdit(Chat chat);
    partial void BeforeRowDelete(int chatID);
    partial void AfterRowDelete(int chatID);    

    partial void BeforeDBDelete(int chatID);
    partial void AfterDBDelete(int chatID);    

    #endregion

    #region Public Methods

    public ChatProxy[] GetChatProxies()
    {
      List<ChatProxy> list = new List<ChatProxy>();

      foreach (Chat item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int chatID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Chats] WHERE ([ChatID] = @ChatID);";
        deleteCommand.Parameters.Add("ChatID", SqlDbType.Int);
        deleteCommand.Parameters["ChatID"].Value = chatID;

        BeforeDBDelete(chatID);
        BeforeRowDelete(chatID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(chatID);
        AfterDBDelete(chatID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ChatsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Chats] SET     [OrganizationID] = @OrganizationID,    [InitiatorID] = @InitiatorID,    [InitiatorType] = @InitiatorType,    [ActionID] = @ActionID  WHERE ([ChatID] = @ChatID);";

		
		tempParameter = updateCommand.Parameters.Add("ChatID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("InitiatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("InitiatorType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Chats] (    [OrganizationID],    [InitiatorID],    [InitiatorType],    [ActionID],    [DateCreated]) VALUES ( @OrganizationID, @InitiatorID, @InitiatorType, @ActionID, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("InitiatorType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("InitiatorID", SqlDbType.Int, 4);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Chats] WHERE ([ChatID] = @ChatID);";
		deleteCommand.Parameters.Add("ChatID", SqlDbType.Int);

		try
		{
		  foreach (Chat chat in this)
		  {
			if (chat.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(chat);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = chat.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ChatID"].AutoIncrement = false;
			  Table.Columns["ChatID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				chat.Row["ChatID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(chat);
			}
			else if (chat.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(chat);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = chat.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(chat);
			}
			else if (chat.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)chat.Row["ChatID", DataRowVersion.Original];
			  deleteCommand.Parameters["ChatID"].Value = id;
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

      foreach (Chat chat in this)
      {
        if (chat.Row.Table.Columns.Contains("CreatorID") && (int)chat.Row["CreatorID"] == 0) chat.Row["CreatorID"] = LoginUser.UserID;
        if (chat.Row.Table.Columns.Contains("ModifierID")) chat.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Chat FindByChatID(int chatID)
    {
      foreach (Chat chat in this)
      {
        if (chat.ChatID == chatID)
        {
          return chat;
        }
      }
      return null;
    }

    public virtual Chat AddNewChat()
    {
      if (Table.Columns.Count < 1) LoadColumns("Chats");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Chat(row, this);
    }
    
    public virtual void LoadByChatID(int chatID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ChatID], [OrganizationID], [InitiatorID], [InitiatorType], [ActionID], [DateCreated] FROM [dbo].[Chats] WHERE ([ChatID] = @ChatID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ChatID", chatID);
        Fill(command);
      }
    }
    
    public static Chat GetChat(LoginUser loginUser, int chatID)
    {
      Chats chats = new Chats(loginUser);
      chats.LoadByChatID(chatID);
      if (chats.IsEmpty)
        return null;
      else
        return chats[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Chat> Members

    public IEnumerator<Chat> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Chat(row, this);
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
