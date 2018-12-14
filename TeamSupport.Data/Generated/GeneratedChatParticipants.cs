using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ChatParticipant : BaseItem
  {
    private ChatParticipants _chatParticipants;
    
    public ChatParticipant(DataRow row, ChatParticipants chatParticipants): base(row, chatParticipants)
    {
      _chatParticipants = chatParticipants;
    }
	
    #region Properties
    
    public ChatParticipants Collection
    {
      get { return _chatParticipants; }
    }
        
    
    
    
    public int ChatParticipantID
    {
      get { return (int)Row["ChatParticipantID"]; }
    }
    

    

    
    public int LastPreviewedMessageID
    {
      get { return (int)Row["LastPreviewedMessageID"]; }
      set { Row["LastPreviewedMessageID"] = CheckValue("LastPreviewedMessageID", value); }
    }
    
    public int LastMessageID
    {
      get { return (int)Row["LastMessageID"]; }
      set { Row["LastMessageID"] = CheckValue("LastMessageID", value); }
    }
    
    public string IPAddress
    {
      get { return (string)Row["IPAddress"]; }
      set { Row["IPAddress"] = CheckValue("IPAddress", value); }
    }
    
    public ChatParticipantType ParticipantType
    {
      get { return (ChatParticipantType)Row["ParticipantType"]; }
      set { Row["ParticipantType"] = CheckValue("ParticipantType", value); }
    }
    
    public int ParticipantID
    {
      get { return (int)Row["ParticipantID"]; }
      set { Row["ParticipantID"] = CheckValue("ParticipantID", value); }
    }
    
    public int ChatID
    {
      get { return (int)Row["ChatID"]; }
      set { Row["ChatID"] = CheckValue("ChatID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateLeft
    {
      get { return Row["DateLeft"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateLeft"]) : null; }
      set { Row["DateLeft"] = CheckValue("DateLeft", value); }
    }

    public DateTime? DateLeftUtc
    {
      get { return Row["DateLeft"] != DBNull.Value ? (DateTime?)Row["DateLeft"] : null; }
    }
    

    
    public DateTime DateJoined
    {
      get { return DateToLocal((DateTime)Row["DateJoined"]); }
      set { Row["DateJoined"] = CheckValue("DateJoined", value); }
    }

    public DateTime DateJoinedUtc
    {
      get { return (DateTime)Row["DateJoined"]; }
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
    
    public DateTime LastTyped
    {
      get { return DateToLocal((DateTime)Row["LastTyped"]); }
      set { Row["LastTyped"] = CheckValue("LastTyped", value); }
    }

    public DateTime LastTypedUtc
    {
      get { return (DateTime)Row["LastTyped"]; }
    }
    

    #endregion
    
    
  }

  public partial class ChatParticipants : BaseCollection, IEnumerable<ChatParticipant>
  {
    public ChatParticipants(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ChatParticipants"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ChatParticipantID"; }
    }



    public ChatParticipant this[int index]
    {
      get { return new ChatParticipant(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ChatParticipant chatParticipant);
    partial void AfterRowInsert(ChatParticipant chatParticipant);
    partial void BeforeRowEdit(ChatParticipant chatParticipant);
    partial void AfterRowEdit(ChatParticipant chatParticipant);
    partial void BeforeRowDelete(int chatParticipantID);
    partial void AfterRowDelete(int chatParticipantID);    

    partial void BeforeDBDelete(int chatParticipantID);
    partial void AfterDBDelete(int chatParticipantID);    

    #endregion

    #region Public Methods

    public ChatParticipantProxy[] GetChatParticipantProxies()
    {
      List<ChatParticipantProxy> list = new List<ChatParticipantProxy>();

      foreach (ChatParticipant item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int chatParticipantID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatParticipants] WHERE ([ChatParticipantID] = @ChatParticipantID);";
        deleteCommand.Parameters.Add("ChatParticipantID", SqlDbType.Int);
        deleteCommand.Parameters["ChatParticipantID"].Value = chatParticipantID;

        BeforeDBDelete(chatParticipantID);
        BeforeRowDelete(chatParticipantID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(chatParticipantID);
        AfterDBDelete(chatParticipantID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ChatParticipantsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ChatParticipants] SET     [ChatID] = @ChatID,    [ParticipantID] = @ParticipantID,    [ParticipantType] = @ParticipantType,    [IPAddress] = @IPAddress,    [LastMessageID] = @LastMessageID,    [LastPreviewedMessageID] = @LastPreviewedMessageID,    [LastTyped] = @LastTyped,    [DateJoined] = @DateJoined,    [DateLeft] = @DateLeft  WHERE ([ChatParticipantID] = @ChatParticipantID);";

		
		tempParameter = updateCommand.Parameters.Add("ChatParticipantID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ParticipantID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParticipantType", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("LastMessageID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastPreviewedMessageID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastTyped", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateJoined", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateLeft", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ChatParticipants] (    [ChatID],    [ParticipantID],    [ParticipantType],    [IPAddress],    [LastMessageID],    [LastPreviewedMessageID],    [LastTyped],    [DateCreated],    [DateJoined],    [DateLeft]) VALUES ( @ChatID, @ParticipantID, @ParticipantType, @IPAddress, @LastMessageID, @LastPreviewedMessageID, @LastTyped, @DateCreated, @DateJoined, @DateLeft); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateLeft", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateJoined", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("LastTyped", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastPreviewedMessageID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastMessageID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IPAddress", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParticipantType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParticipantID", SqlDbType.Int, 4);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatParticipants] WHERE ([ChatParticipantID] = @ChatParticipantID);";
		deleteCommand.Parameters.Add("ChatParticipantID", SqlDbType.Int);

		try
		{
		  foreach (ChatParticipant chatParticipant in this)
		  {
			if (chatParticipant.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(chatParticipant);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = chatParticipant.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ChatParticipantID"].AutoIncrement = false;
			  Table.Columns["ChatParticipantID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				chatParticipant.Row["ChatParticipantID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(chatParticipant);
			}
			else if (chatParticipant.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(chatParticipant);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = chatParticipant.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(chatParticipant);
			}
			else if (chatParticipant.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)chatParticipant.Row["ChatParticipantID", DataRowVersion.Original];
			  deleteCommand.Parameters["ChatParticipantID"].Value = id;
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

      foreach (ChatParticipant chatParticipant in this)
      {
        if (chatParticipant.Row.Table.Columns.Contains("CreatorID") && (int)chatParticipant.Row["CreatorID"] == 0) chatParticipant.Row["CreatorID"] = LoginUser.UserID;
        if (chatParticipant.Row.Table.Columns.Contains("ModifierID")) chatParticipant.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ChatParticipant FindByChatParticipantID(int chatParticipantID)
    {
      foreach (ChatParticipant chatParticipant in this)
      {
        if (chatParticipant.ChatParticipantID == chatParticipantID)
        {
          return chatParticipant;
        }
      }
      return null;
    }

    public virtual ChatParticipant AddNewChatParticipant()
    {
      if (Table.Columns.Count < 1) LoadColumns("ChatParticipants");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ChatParticipant(row, this);
    }
    
    public virtual void LoadByChatParticipantID(int chatParticipantID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ChatParticipantID], [ChatID], [ParticipantID], [ParticipantType], [IPAddress], [LastMessageID], [LastPreviewedMessageID], [LastTyped], [DateCreated], [DateJoined], [DateLeft] FROM [dbo].[ChatParticipants] WHERE ([ChatParticipantID] = @ChatParticipantID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ChatParticipantID", chatParticipantID);
        Fill(command);
      }
    }
    
    public static ChatParticipant GetChatParticipant(LoginUser loginUser, int chatParticipantID)
    {
      ChatParticipants chatParticipants = new ChatParticipants(loginUser);
      chatParticipants.LoadByChatParticipantID(chatParticipantID);
      if (chatParticipants.IsEmpty)
        return null;
      else
        return chatParticipants[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ChatParticipant> Members

    public IEnumerator<ChatParticipant> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ChatParticipant(row, this);
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
