using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ChatMessage : BaseItem
  {
    private ChatMessages _chatMessages;
    
    public ChatMessage(DataRow row, ChatMessages chatMessages): base(row, chatMessages)
    {
      _chatMessages = chatMessages;
    }
	
    #region Properties
    
    public ChatMessages Collection
    {
      get { return _chatMessages; }
    }
        
    
    
    
    public int ChatMessageID
    {
      get { return (int)Row["ChatMessageID"]; }
    }
    

    

    
    public ChatParticipantType PosterType
    {
      get { return (ChatParticipantType)Row["PosterType"]; }
      set { Row["PosterType"] = CheckValue("PosterType", value); }
    }
    
    public int PosterID
    {
      get { return (int)Row["PosterID"]; }
      set { Row["PosterID"] = CheckValue("PosterID", value); }
    }
    
    public string Message
    {
      get { return (string)Row["Message"]; }
      set { Row["Message"] = CheckValue("Message", value); }
    }
    
    public bool IsNotification
    {
      get { return (bool)Row["IsNotification"]; }
      set { Row["IsNotification"] = CheckValue("IsNotification", value); }
    }
    
    public int ChatID
    {
      get { return (int)Row["ChatID"]; }
      set { Row["ChatID"] = CheckValue("ChatID", value); }
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

  public partial class ChatMessages : BaseCollection, IEnumerable<ChatMessage>
  {
    public ChatMessages(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ChatMessages"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ChatMessageID"; }
    }



    public ChatMessage this[int index]
    {
      get { return new ChatMessage(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ChatMessage chatMessage);
    partial void AfterRowInsert(ChatMessage chatMessage);
    partial void BeforeRowEdit(ChatMessage chatMessage);
    partial void AfterRowEdit(ChatMessage chatMessage);
    partial void BeforeRowDelete(int chatMessageID);
    partial void AfterRowDelete(int chatMessageID);    

    partial void BeforeDBDelete(int chatMessageID);
    partial void AfterDBDelete(int chatMessageID);    

    #endregion

    #region Public Methods

    public ChatMessageProxy[] GetChatMessageProxies()
    {
      List<ChatMessageProxy> list = new List<ChatMessageProxy>();

      foreach (ChatMessage item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int chatMessageID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatMessages] WHERE ([ChatMessageID] = @ChatMessageID);";
        deleteCommand.Parameters.Add("ChatMessageID", SqlDbType.Int);
        deleteCommand.Parameters["ChatMessageID"].Value = chatMessageID;

        BeforeDBDelete(chatMessageID);
        BeforeRowDelete(chatMessageID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(chatMessageID);
        AfterDBDelete(chatMessageID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ChatMessagesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ChatMessages] SET     [ChatID] = @ChatID,    [IsNotification] = @IsNotification,    [Message] = @Message,    [PosterID] = @PosterID,    [PosterType] = @PosterType  WHERE ([ChatMessageID] = @ChatMessageID);";

		
		tempParameter = updateCommand.Parameters.Add("ChatMessageID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ChatID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsNotification", SqlDbType.Bit, 1);
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
		
		tempParameter = updateCommand.Parameters.Add("PosterID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PosterType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ChatMessages] (    [ChatID],    [IsNotification],    [Message],    [PosterID],    [PosterType],    [DateCreated]) VALUES ( @ChatID, @IsNotification, @Message, @PosterID, @PosterType, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("PosterType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("PosterID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Message", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsNotification", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ChatID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatMessages] WHERE ([ChatMessageID] = @ChatMessageID);";
		deleteCommand.Parameters.Add("ChatMessageID", SqlDbType.Int);

		try
		{
		  foreach (ChatMessage chatMessage in this)
		  {
			if (chatMessage.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(chatMessage);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = chatMessage.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ChatMessageID"].AutoIncrement = false;
			  Table.Columns["ChatMessageID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				chatMessage.Row["ChatMessageID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(chatMessage);
			}
			else if (chatMessage.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(chatMessage);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = chatMessage.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(chatMessage);
			}
			else if (chatMessage.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)chatMessage.Row["ChatMessageID", DataRowVersion.Original];
			  deleteCommand.Parameters["ChatMessageID"].Value = id;
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

      foreach (ChatMessage chatMessage in this)
      {
        if (chatMessage.Row.Table.Columns.Contains("CreatorID") && (int)chatMessage.Row["CreatorID"] == 0) chatMessage.Row["CreatorID"] = LoginUser.UserID;
        if (chatMessage.Row.Table.Columns.Contains("ModifierID")) chatMessage.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ChatMessage FindByChatMessageID(int chatMessageID)
    {
      foreach (ChatMessage chatMessage in this)
      {
        if (chatMessage.ChatMessageID == chatMessageID)
        {
          return chatMessage;
        }
      }
      return null;
    }

    public virtual ChatMessage AddNewChatMessage()
    {
      if (Table.Columns.Count < 1) LoadColumns("ChatMessages");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ChatMessage(row, this);
    }
    
    public virtual void LoadByChatMessageID(int chatMessageID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ChatMessageID], [ChatID], [IsNotification], [Message], [PosterID], [PosterType], [DateCreated] FROM [dbo].[ChatMessages] WHERE ([ChatMessageID] = @ChatMessageID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ChatMessageID", chatMessageID);
        Fill(command);
      }
    }
    
    public static ChatMessage GetChatMessage(LoginUser loginUser, int chatMessageID)
    {
      ChatMessages chatMessages = new ChatMessages(loginUser);
      chatMessages.LoadByChatMessageID(chatMessageID);
      if (chatMessages.IsEmpty)
        return null;
      else
        return chatMessages[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ChatMessage> Members

    public IEnumerator<ChatMessage> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ChatMessage(row, this);
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
