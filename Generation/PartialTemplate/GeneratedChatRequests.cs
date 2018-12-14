using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ChatRequest : BaseItem
  {
    private ChatRequests _chatRequests;
    
    public ChatRequest(DataRow row, ChatRequests chatRequests): base(row, chatRequests)
    {
      _chatRequests = chatRequests;
    }
	
    #region Properties
    
    public ChatRequests Collection
    {
      get { return _chatRequests; }
    }
        
    
    
    
    public int ChatRequestID
    {
      get { return (int)Row["ChatRequestID"]; }
    }
    

    
    public int? TargetUserID
    {
      get { return Row["TargetUserID"] != DBNull.Value ? (int?)Row["TargetUserID"] : null; }
      set { Row["TargetUserID"] = CheckValue("TargetUserID", value); }
    }
    
    public int? GroupID
    {
      get { return Row["GroupID"] != DBNull.Value ? (int?)Row["GroupID"] : null; }
      set { Row["GroupID"] = CheckValue("GroupID", value); }
    }
    

    
    public bool IsAccepted
    {
      get { return (bool)Row["IsAccepted"]; }
      set { Row["IsAccepted"] = CheckValue("IsAccepted", value); }
    }
    
    public ChatRequestType RequestType
    {
      get { return (ChatRequestType)Row["RequestType"]; }
      set { Row["RequestType"] = CheckValue("RequestType", value); }
    }
    
    public string Message
    {
      get { return (string)Row["Message"]; }
      set { Row["Message"] = CheckValue("Message", value); }
    }
    
    public ChatParticipantType RequestorType
    {
      get { return (ChatParticipantType)Row["RequestorType"]; }
      set { Row["RequestorType"] = CheckValue("RequestorType", value); }
    }
    
    public int RequestorID
    {
      get { return (int)Row["RequestorID"]; }
      set { Row["RequestorID"] = CheckValue("RequestorID", value); }
    }
    
    public int ChatID
    {
      get { return (int)Row["ChatID"]; }
      set { Row["ChatID"] = CheckValue("ChatID", value); }
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

  public partial class ChatRequests : BaseCollection, IEnumerable<ChatRequest>
  {
    public ChatRequests(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ChatRequests"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ChatRequestID"; }
    }



    public ChatRequest this[int index]
    {
      get { return new ChatRequest(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ChatRequest chatRequest);
    partial void AfterRowInsert(ChatRequest chatRequest);
    partial void BeforeRowEdit(ChatRequest chatRequest);
    partial void AfterRowEdit(ChatRequest chatRequest);
    partial void BeforeRowDelete(int chatRequestID);
    partial void AfterRowDelete(int chatRequestID);    

    partial void BeforeDBDelete(int chatRequestID);
    partial void AfterDBDelete(int chatRequestID);    

    #endregion

    #region Public Methods

    public ChatRequestProxy[] GetChatRequestProxies()
    {
      List<ChatRequestProxy> list = new List<ChatRequestProxy>();

      foreach (ChatRequest item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int chatRequestID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatRequests] WHERE ([ChatRequestID] = @ChatRequestID);";
        deleteCommand.Parameters.Add("ChatRequestID", SqlDbType.Int);
        deleteCommand.Parameters["ChatRequestID"].Value = chatRequestID;

        BeforeDBDelete(chatRequestID);
        BeforeRowDelete(chatRequestID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(chatRequestID);
        AfterDBDelete(chatRequestID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ChatRequestsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ChatRequests] SET     [OrganizationID] = @OrganizationID,    [ChatID] = @ChatID,    [RequestorID] = @RequestorID,    [RequestorType] = @RequestorType,    [TargetUserID] = @TargetUserID,    [Message] = @Message,    [GroupID] = @GroupID,    [RequestType] = @RequestType,    [IsAccepted] = @IsAccepted  WHERE ([ChatRequestID] = @ChatRequestID);";

		
		tempParameter = updateCommand.Parameters.Add("ChatRequestID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ChatID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequestorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequestorType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TargetUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Message", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RequestType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsAccepted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ChatRequests] (    [OrganizationID],    [ChatID],    [RequestorID],    [RequestorType],    [TargetUserID],    [Message],    [GroupID],    [RequestType],    [IsAccepted],    [DateCreated]) VALUES ( @OrganizationID, @ChatID, @RequestorID, @RequestorType, @TargetUserID, @Message, @GroupID, @RequestType, @IsAccepted, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsAccepted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequestType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("TargetUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequestorType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RequestorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ChatID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatRequests] WHERE ([ChatRequestID] = @ChatRequestID);";
		deleteCommand.Parameters.Add("ChatRequestID", SqlDbType.Int);

		try
		{
		  foreach (ChatRequest chatRequest in this)
		  {
			if (chatRequest.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(chatRequest);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = chatRequest.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ChatRequestID"].AutoIncrement = false;
			  Table.Columns["ChatRequestID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				chatRequest.Row["ChatRequestID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(chatRequest);
			}
			else if (chatRequest.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(chatRequest);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = chatRequest.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(chatRequest);
			}
			else if (chatRequest.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)chatRequest.Row["ChatRequestID", DataRowVersion.Original];
			  deleteCommand.Parameters["ChatRequestID"].Value = id;
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

      foreach (ChatRequest chatRequest in this)
      {
        if (chatRequest.Row.Table.Columns.Contains("CreatorID") && (int)chatRequest.Row["CreatorID"] == 0) chatRequest.Row["CreatorID"] = LoginUser.UserID;
        if (chatRequest.Row.Table.Columns.Contains("ModifierID")) chatRequest.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ChatRequest FindByChatRequestID(int chatRequestID)
    {
      foreach (ChatRequest chatRequest in this)
      {
        if (chatRequest.ChatRequestID == chatRequestID)
        {
          return chatRequest;
        }
      }
      return null;
    }

    public virtual ChatRequest AddNewChatRequest()
    {
      if (Table.Columns.Count < 1) LoadColumns("ChatRequests");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ChatRequest(row, this);
    }
    
    public virtual void LoadByChatRequestID(int chatRequestID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ChatRequestID], [OrganizationID], [ChatID], [RequestorID], [RequestorType], [TargetUserID], [Message], [GroupID], [RequestType], [IsAccepted], [DateCreated] FROM [dbo].[ChatRequests] WHERE ([ChatRequestID] = @ChatRequestID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ChatRequestID", chatRequestID);
        Fill(command);
      }
    }
    
    public static ChatRequest GetChatRequest(LoginUser loginUser, int chatRequestID)
    {
      ChatRequests chatRequests = new ChatRequests(loginUser);
      chatRequests.LoadByChatRequestID(chatRequestID);
      if (chatRequests.IsEmpty)
        return null;
      else
        return chatRequests[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ChatRequest> Members

    public IEnumerator<ChatRequest> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ChatRequest(row, this);
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
