using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WatercoolerMsgItem : BaseItem
  {
    private WatercoolerMsg _watercoolerMsg;
    
    public WatercoolerMsgItem(DataRow row, WatercoolerMsg watercoolerMsg): base(row, watercoolerMsg)
    {
      _watercoolerMsg = watercoolerMsg;
    }
	
    #region Properties
    
    public WatercoolerMsg Collection
    {
      get { return _watercoolerMsg; }
    }
        
    
    
    
    public int MessageID
    {
      get { return (int)Row["MessageID"]; }
    }
    

    
    public string Message
    {
      get { return Row["Message"] != DBNull.Value ? (string)Row["Message"] : null; }
      set { Row["Message"] = CheckValue("Message",value); }
    }
    
    public int? MessageParent
    {
      get { return Row["MessageParent"] != DBNull.Value ? (int?)Row["MessageParent"] : null; }
      set { Row["MessageParent"] = CheckValue("MessageParent",value); }
    }
    

    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }
    
    public bool IsDeleted
    {
      get { return (bool)Row["IsDeleted"]; }
      set { Row["IsDeleted"] = CheckValue("IsDeleted",value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID",value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID",value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime LastModified
    {
      get { return DateToLocal((DateTime)Row["LastModified"]); }
      set { Row["LastModified"] = CheckValue("LastModified",value); }
    }

    public DateTime LastModifiedUtc
    {
      get { return (DateTime)Row["LastModified"]; }
    }
    
    public DateTime TimeStamp
    {
      get { return DateToLocal((DateTime)Row["TimeStamp"]); }
      set { Row["TimeStamp"] = CheckValue("TimeStamp",value); }
    }

    public DateTime TimeStampUtc
    {
      get { return (DateTime)Row["TimeStamp"]; }
    }
    

    #endregion
    
    
  }

  public partial class WatercoolerMsg : BaseCollection, IEnumerable<WatercoolerMsgItem>
  {
    public WatercoolerMsg(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "WatercoolerMsg"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "MessageID"; }
    }



    public WatercoolerMsgItem this[int index]
    {
      get { return new WatercoolerMsgItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WatercoolerMsgItem watercoolerMsgItem);
    partial void AfterRowInsert(WatercoolerMsgItem watercoolerMsgItem);
    partial void BeforeRowEdit(WatercoolerMsgItem watercoolerMsgItem);
    partial void AfterRowEdit(WatercoolerMsgItem watercoolerMsgItem);
    partial void BeforeRowDelete(int messageID);
    partial void AfterRowDelete(int messageID);    

    partial void BeforeDBDelete(int messageID);
    partial void AfterDBDelete(int messageID);    

    #endregion

    #region Public Methods

    public WatercoolerMsgItemProxy[] GetWatercoolerMsgItemProxies()
    {
      List<WatercoolerMsgItemProxy> list = new List<WatercoolerMsgItemProxy>();

      foreach (WatercoolerMsgItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int messageID)
    {
      BeforeDBDelete(messageID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WatercoolerMsg] WHERE ([MessageID] = @MessageID);";
        deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);
        deleteCommand.Parameters["MessageID"].Value = messageID;

        BeforeRowDelete(messageID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(messageID);
      }
      AfterDBDelete(messageID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("WatercoolerMsgSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[WatercoolerMsg] SET     [UserID] = @UserID,    [OrganizationID] = @OrganizationID,    [TimeStamp] = @TimeStamp,    [Message] = @Message,    [MessageParent] = @MessageParent,    [IsDeleted] = @IsDeleted,    [LastModified] = @LastModified,    [NeedsIndexing] = @NeedsIndexing  WHERE ([MessageID] = @MessageID);";

		
		tempParameter = updateCommand.Parameters.Add("MessageID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeStamp", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("Message", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MessageParent", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[WatercoolerMsg] (    [UserID],    [OrganizationID],    [TimeStamp],    [Message],    [MessageParent],    [IsDeleted],    [LastModified],    [NeedsIndexing]) VALUES ( @UserID, @OrganizationID, @TimeStamp, @Message, @MessageParent, @IsDeleted, @LastModified, @NeedsIndexing); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MessageParent", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Message", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeStamp", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WatercoolerMsg] WHERE ([MessageID] = @MessageID);";
		deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);

		try
		{
		  foreach (WatercoolerMsgItem watercoolerMsgItem in this)
		  {
			if (watercoolerMsgItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(watercoolerMsgItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = watercoolerMsgItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["MessageID"].AutoIncrement = false;
			  Table.Columns["MessageID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				watercoolerMsgItem.Row["MessageID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(watercoolerMsgItem);
			}
			else if (watercoolerMsgItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(watercoolerMsgItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = watercoolerMsgItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(watercoolerMsgItem);
			}
			else if (watercoolerMsgItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)watercoolerMsgItem.Row["MessageID", DataRowVersion.Original];
			  deleteCommand.Parameters["MessageID"].Value = id;
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

      foreach (WatercoolerMsgItem watercoolerMsgItem in this)
      {
        if (watercoolerMsgItem.Row.Table.Columns.Contains("CreatorID") && (int)watercoolerMsgItem.Row["CreatorID"] == 0) watercoolerMsgItem.Row["CreatorID"] = LoginUser.UserID;
        if (watercoolerMsgItem.Row.Table.Columns.Contains("ModifierID")) watercoolerMsgItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WatercoolerMsgItem FindByMessageID(int messageID)
    {
      foreach (WatercoolerMsgItem watercoolerMsgItem in this)
      {
        if (watercoolerMsgItem.MessageID == messageID)
        {
          return watercoolerMsgItem;
        }
      }
      return null;
    }

    public virtual WatercoolerMsgItem AddNewWatercoolerMsgItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("WatercoolerMsg");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WatercoolerMsgItem(row, this);
    }
    
    public virtual void LoadByMessageID(int messageID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [MessageID], [UserID], [OrganizationID], [TimeStamp], [Message], [MessageParent], [IsDeleted], [LastModified], [NeedsIndexing] FROM [dbo].[WatercoolerMsg] WHERE ([MessageID] = @MessageID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("MessageID", messageID);
        Fill(command);
      }
    }
    
    public static WatercoolerMsgItem GetWatercoolerMsgItem(LoginUser loginUser, int messageID)
    {
      WatercoolerMsg watercoolerMsg = new WatercoolerMsg(loginUser);
      watercoolerMsg.LoadByMessageID(messageID);
      if (watercoolerMsg.IsEmpty)
        return null;
      else
        return watercoolerMsg[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WatercoolerMsgItem> Members

    public IEnumerator<WatercoolerMsgItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WatercoolerMsgItem(row, this);
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
