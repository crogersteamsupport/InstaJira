using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class EmailPostHistoryItem : BaseItem
  {
    private EmailPostHistory _emailPostHistory;
    
    public EmailPostHistoryItem(DataRow row, EmailPostHistory emailPostHistory): base(row, emailPostHistory)
    {
      _emailPostHistory = emailPostHistory;
    }
	
    #region Properties
    
    public EmailPostHistory Collection
    {
      get { return _emailPostHistory; }
    }
        
    
    
    

    
    public string Param1
    {
      get { return Row["Param1"] != DBNull.Value ? (string)Row["Param1"] : null; }
      set { Row["Param1"] = CheckValue("Param1", value); }
    }
    
    public string Param2
    {
      get { return Row["Param2"] != DBNull.Value ? (string)Row["Param2"] : null; }
      set { Row["Param2"] = CheckValue("Param2", value); }
    }
    
    public string Param3
    {
      get { return Row["Param3"] != DBNull.Value ? (string)Row["Param3"] : null; }
      set { Row["Param3"] = CheckValue("Param3", value); }
    }
    
    public string Param4
    {
      get { return Row["Param4"] != DBNull.Value ? (string)Row["Param4"] : null; }
      set { Row["Param4"] = CheckValue("Param4", value); }
    }
    
    public string Param5
    {
      get { return Row["Param5"] != DBNull.Value ? (string)Row["Param5"] : null; }
      set { Row["Param5"] = CheckValue("Param5", value); }
    }
    
    public string Param6
    {
      get { return Row["Param6"] != DBNull.Value ? (string)Row["Param6"] : null; }
      set { Row["Param6"] = CheckValue("Param6", value); }
    }
    
    public string Param7
    {
      get { return Row["Param7"] != DBNull.Value ? (string)Row["Param7"] : null; }
      set { Row["Param7"] = CheckValue("Param7", value); }
    }
    
    public string Param8
    {
      get { return Row["Param8"] != DBNull.Value ? (string)Row["Param8"] : null; }
      set { Row["Param8"] = CheckValue("Param8", value); }
    }
    
    public string Param9
    {
      get { return Row["Param9"] != DBNull.Value ? (string)Row["Param9"] : null; }
      set { Row["Param9"] = CheckValue("Param9", value); }
    }
    
    public string Param10
    {
      get { return Row["Param10"] != DBNull.Value ? (string)Row["Param10"] : null; }
      set { Row["Param10"] = CheckValue("Param10", value); }
    }
    
    public string Text1
    {
      get { return Row["Text1"] != DBNull.Value ? (string)Row["Text1"] : null; }
      set { Row["Text1"] = CheckValue("Text1", value); }
    }
    
    public string Text2
    {
      get { return Row["Text2"] != DBNull.Value ? (string)Row["Text2"] : null; }
      set { Row["Text2"] = CheckValue("Text2", value); }
    }
    
    public string Text3
    {
      get { return Row["Text3"] != DBNull.Value ? (string)Row["Text3"] : null; }
      set { Row["Text3"] = CheckValue("Text3", value); }
    }
    
    public string LockProcessID
    {
      get { return Row["LockProcessID"] != DBNull.Value ? (string)Row["LockProcessID"] : null; }
      set { Row["LockProcessID"] = CheckValue("LockProcessID", value); }
    }
    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int HoldTime
    {
      get { return (int)Row["HoldTime"]; }
      set { Row["HoldTime"] = CheckValue("HoldTime", value); }
    }
    
    public int EmailPostType
    {
      get { return (int)Row["EmailPostType"]; }
      set { Row["EmailPostType"] = CheckValue("EmailPostType", value); }
    }
    
    public int EmailPostID
    {
      get { return (int)Row["EmailPostID"]; }
      set { Row["EmailPostID"] = CheckValue("EmailPostID", value); }
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

  public partial class EmailPostHistory : BaseCollection, IEnumerable<EmailPostHistoryItem>
  {
    public EmailPostHistory(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "EmailPostHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "EmailPostID"; }
    }



    public EmailPostHistoryItem this[int index]
    {
      get { return new EmailPostHistoryItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(EmailPostHistoryItem emailPostHistoryItem);
    partial void AfterRowInsert(EmailPostHistoryItem emailPostHistoryItem);
    partial void BeforeRowEdit(EmailPostHistoryItem emailPostHistoryItem);
    partial void AfterRowEdit(EmailPostHistoryItem emailPostHistoryItem);
    partial void BeforeRowDelete(int emailPostID);
    partial void AfterRowDelete(int emailPostID);    

    partial void BeforeDBDelete(int emailPostID);
    partial void AfterDBDelete(int emailPostID);    

    #endregion

    #region Public Methods

    public EmailPostHistoryItemProxy[] GetEmailPostHistoryItemProxies()
    {
      List<EmailPostHistoryItemProxy> list = new List<EmailPostHistoryItemProxy>();

      foreach (EmailPostHistoryItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int emailPostID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EmailPostHistory] WHERE ([EmailPostID] = @EmailPostID);";
        deleteCommand.Parameters.Add("EmailPostID", SqlDbType.Int);
        deleteCommand.Parameters["EmailPostID"].Value = emailPostID;

        BeforeDBDelete(emailPostID);
        BeforeRowDelete(emailPostID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(emailPostID);
        AfterDBDelete(emailPostID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("EmailPostHistorySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[EmailPostHistory] SET     [EmailPostType] = @EmailPostType,    [HoldTime] = @HoldTime,    [Param1] = @Param1,    [Param2] = @Param2,    [Param3] = @Param3,    [Param4] = @Param4,    [Param5] = @Param5,    [Param6] = @Param6,    [Param7] = @Param7,    [Param8] = @Param8,    [Param9] = @Param9,    [Param10] = @Param10,    [Text1] = @Text1,    [Text2] = @Text2,    [Text3] = @Text3,    [LockProcessID] = @LockProcessID  WHERE ([EmailPostID] = @EmailPostID);";

		
		tempParameter = updateCommand.Parameters.Add("EmailPostID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailPostType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("HoldTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param1", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param2", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param3", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param4", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param5", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param6", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param7", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param8", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param9", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Param10", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Text1", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Text2", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Text3", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LockProcessID", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[EmailPostHistory] (    [EmailPostID],    [EmailPostType],    [HoldTime],    [DateCreated],    [CreatorID],    [Param1],    [Param2],    [Param3],    [Param4],    [Param5],    [Param6],    [Param7],    [Param8],    [Param9],    [Param10],    [Text1],    [Text2],    [Text3],    [LockProcessID]) VALUES ( @EmailPostID, @EmailPostType, @HoldTime, @DateCreated, @CreatorID, @Param1, @Param2, @Param3, @Param4, @Param5, @Param6, @Param7, @Param8, @Param9, @Param10, @Text1, @Text2, @Text3, @LockProcessID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("LockProcessID", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Text3", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Text2", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Text1", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param10", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param9", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param8", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param7", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param6", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param5", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param4", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param3", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param2", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Param1", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("HoldTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailPostType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailPostID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EmailPostHistory] WHERE ([EmailPostID] = @EmailPostID);";
		deleteCommand.Parameters.Add("EmailPostID", SqlDbType.Int);

		try
		{
		  foreach (EmailPostHistoryItem emailPostHistoryItem in this)
		  {
			if (emailPostHistoryItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(emailPostHistoryItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = emailPostHistoryItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["EmailPostID"].AutoIncrement = false;
			  Table.Columns["EmailPostID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				emailPostHistoryItem.Row["EmailPostID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(emailPostHistoryItem);
			}
			else if (emailPostHistoryItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(emailPostHistoryItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = emailPostHistoryItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(emailPostHistoryItem);
			}
			else if (emailPostHistoryItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)emailPostHistoryItem.Row["EmailPostID", DataRowVersion.Original];
			  deleteCommand.Parameters["EmailPostID"].Value = id;
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

      foreach (EmailPostHistoryItem emailPostHistoryItem in this)
      {
        if (emailPostHistoryItem.Row.Table.Columns.Contains("CreatorID") && (int)emailPostHistoryItem.Row["CreatorID"] == 0) emailPostHistoryItem.Row["CreatorID"] = LoginUser.UserID;
        if (emailPostHistoryItem.Row.Table.Columns.Contains("ModifierID")) emailPostHistoryItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public EmailPostHistoryItem FindByEmailPostID(int emailPostID)
    {
      foreach (EmailPostHistoryItem emailPostHistoryItem in this)
      {
        if (emailPostHistoryItem.EmailPostID == emailPostID)
        {
          return emailPostHistoryItem;
        }
      }
      return null;
    }

    public virtual EmailPostHistoryItem AddNewEmailPostHistoryItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("EmailPostHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new EmailPostHistoryItem(row, this);
    }
    
    public virtual void LoadByEmailPostID(int emailPostID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [EmailPostID], [EmailPostType], [HoldTime], [DateCreated], [CreatorID], [Param1], [Param2], [Param3], [Param4], [Param5], [Param6], [Param7], [Param8], [Param9], [Param10], [Text1], [Text2], [Text3], [LockProcessID] FROM [dbo].[EmailPostHistory] WHERE ([EmailPostID] = @EmailPostID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("EmailPostID", emailPostID);
        Fill(command);
      }
    }
    
    public static EmailPostHistoryItem GetEmailPostHistoryItem(LoginUser loginUser, int emailPostID)
    {
      EmailPostHistory emailPostHistory = new EmailPostHistory(loginUser);
      emailPostHistory.LoadByEmailPostID(emailPostID);
      if (emailPostHistory.IsEmpty)
        return null;
      else
        return emailPostHistory[0];
    }
    
    
    

    #endregion

    #region IEnumerable<EmailPostHistoryItem> Members

    public IEnumerator<EmailPostHistoryItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new EmailPostHistoryItem(row, this);
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
