using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ChatClient : BaseItem
  {
    private ChatClients _chatClients;
    
    public ChatClient(DataRow row, ChatClients chatClients): base(row, chatClients)
    {
      _chatClients = chatClients;
    }
	
    #region Properties
    
    public ChatClients Collection
    {
      get { return _chatClients; }
    }
        
    
    
    
    public int ChatClientID
    {
      get { return (int)Row["ChatClientID"]; }
    }
    

    
    public int? LinkedUserID
    {
      get { return Row["LinkedUserID"] != DBNull.Value ? (int?)Row["LinkedUserID"] : null; }
      set { Row["LinkedUserID"] = CheckValue("LinkedUserID", value); }
    }
    

    
    public string CompanyName
    {
      get { return (string)Row["CompanyName"]; }
      set { Row["CompanyName"] = CheckValue("CompanyName", value); }
    }
    
    public string Email
    {
      get { return (string)Row["Email"]; }
      set { Row["Email"] = CheckValue("Email", value); }
    }
    
    public string LastName
    {
      get { return (string)Row["LastName"]; }
      set { Row["LastName"] = CheckValue("LastName", value); }
    }
    
    public string FirstName
    {
      get { return (string)Row["FirstName"]; }
      set { Row["FirstName"] = CheckValue("FirstName", value); }
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
    
    public DateTime LastPing
    {
      get { return DateToLocal((DateTime)Row["LastPing"]); }
      set { Row["LastPing"] = CheckValue("LastPing", value); }
    }

    public DateTime LastPingUtc
    {
      get { return (DateTime)Row["LastPing"]; }
    }
    

    #endregion
    
    
  }

  public partial class ChatClients : BaseCollection, IEnumerable<ChatClient>
  {
    public ChatClients(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ChatClients"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ChatClientID"; }
    }



    public ChatClient this[int index]
    {
      get { return new ChatClient(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ChatClient chatClient);
    partial void AfterRowInsert(ChatClient chatClient);
    partial void BeforeRowEdit(ChatClient chatClient);
    partial void AfterRowEdit(ChatClient chatClient);
    partial void BeforeRowDelete(int chatClientID);
    partial void AfterRowDelete(int chatClientID);    

    partial void BeforeDBDelete(int chatClientID);
    partial void AfterDBDelete(int chatClientID);    

    #endregion

    #region Public Methods

    public ChatClientProxy[] GetChatClientProxies()
    {
      List<ChatClientProxy> list = new List<ChatClientProxy>();

      foreach (ChatClient item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int chatClientID)
    {
      BeforeDBDelete(chatClientID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatClients] WHERE ([ChatClientID] = @ChatClientID);";
        deleteCommand.Parameters.Add("ChatClientID", SqlDbType.Int);
        deleteCommand.Parameters["ChatClientID"].Value = chatClientID;

        BeforeRowDelete(chatClientID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(chatClientID);
      }
      AfterDBDelete(chatClientID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ChatClientsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ChatClients] SET     [OrganizationID] = @OrganizationID,    [FirstName] = @FirstName,    [LastName] = @LastName,    [Email] = @Email,    [CompanyName] = @CompanyName,    [LastPing] = @LastPing,    [LinkedUserID] = @LinkedUserID  WHERE ([ChatClientID] = @ChatClientID);";

		
		tempParameter = updateCommand.Parameters.Add("ChatClientID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("FirstName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Email", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CompanyName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastPing", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LinkedUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ChatClients] (    [OrganizationID],    [FirstName],    [LastName],    [Email],    [CompanyName],    [LastPing],    [LinkedUserID],    [DateCreated]) VALUES ( @OrganizationID, @FirstName, @LastName, @Email, @CompanyName, @LastPing, @LinkedUserID, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LinkedUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastPing", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CompanyName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Email", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FirstName", SqlDbType.VarChar, 50);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ChatClients] WHERE ([ChatClientID] = @ChatClientID);";
		deleteCommand.Parameters.Add("ChatClientID", SqlDbType.Int);

		try
		{
		  foreach (ChatClient chatClient in this)
		  {
			if (chatClient.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(chatClient);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = chatClient.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ChatClientID"].AutoIncrement = false;
			  Table.Columns["ChatClientID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				chatClient.Row["ChatClientID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(chatClient);
			}
			else if (chatClient.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(chatClient);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = chatClient.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(chatClient);
			}
			else if (chatClient.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)chatClient.Row["ChatClientID", DataRowVersion.Original];
			  deleteCommand.Parameters["ChatClientID"].Value = id;
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

      foreach (ChatClient chatClient in this)
      {
        if (chatClient.Row.Table.Columns.Contains("CreatorID") && (int)chatClient.Row["CreatorID"] == 0) chatClient.Row["CreatorID"] = LoginUser.UserID;
        if (chatClient.Row.Table.Columns.Contains("ModifierID")) chatClient.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ChatClient FindByChatClientID(int chatClientID)
    {
      foreach (ChatClient chatClient in this)
      {
        if (chatClient.ChatClientID == chatClientID)
        {
          return chatClient;
        }
      }
      return null;
    }

    public virtual ChatClient AddNewChatClient()
    {
      if (Table.Columns.Count < 1) LoadColumns("ChatClients");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ChatClient(row, this);
    }
    
    public virtual void LoadByChatClientID(int chatClientID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ChatClientID], [OrganizationID], [FirstName], [LastName], [Email], [CompanyName], [LastPing], [LinkedUserID], [DateCreated] FROM [dbo].[ChatClients] WHERE ([ChatClientID] = @ChatClientID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ChatClientID", chatClientID);
        Fill(command);
      }
    }
    
    public static ChatClient GetChatClient(LoginUser loginUser, int chatClientID)
    {
      ChatClients chatClients = new ChatClients(loginUser);
      chatClients.LoadByChatClientID(chatClientID);
      if (chatClients.IsEmpty)
        return null;
      else
        return chatClients[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ChatClient> Members

    public IEnumerator<ChatClient> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ChatClient(row, this);
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
