using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class UserAssignmentHistoryItem : BaseItem
  {
    private UserAssignmentHistory _userAssignmentHistory;
    
    public UserAssignmentHistoryItem(DataRow row, UserAssignmentHistory userAssignmentHistory): base(row, userAssignmentHistory)
    {
      _userAssignmentHistory = userAssignmentHistory;
    }
	
    #region Properties
    
    public UserAssignmentHistory Collection
    {
      get { return _userAssignmentHistory; }
    }
        
    
    
    
    public int UserAssignmentHistoryID
    {
      get { return (int)Row["UserAssignmentHistoryID"]; }
    }
    

    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateAssigned
    {
      get { return DateToLocal((DateTime)Row["DateAssigned"]); }
      set { Row["DateAssigned"] = CheckValue("DateAssigned", value); }
    }

    public DateTime DateAssignedUtc
    {
      get { return (DateTime)Row["DateAssigned"]; }
    }
    

    #endregion
    
    
  }

  public partial class UserAssignmentHistory : BaseCollection, IEnumerable<UserAssignmentHistoryItem>
  {
    public UserAssignmentHistory(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "UserAssignmentHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserAssignmentHistoryID"; }
    }



    public UserAssignmentHistoryItem this[int index]
    {
      get { return new UserAssignmentHistoryItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(UserAssignmentHistoryItem userAssignmentHistoryItem);
    partial void AfterRowInsert(UserAssignmentHistoryItem userAssignmentHistoryItem);
    partial void BeforeRowEdit(UserAssignmentHistoryItem userAssignmentHistoryItem);
    partial void AfterRowEdit(UserAssignmentHistoryItem userAssignmentHistoryItem);
    partial void BeforeRowDelete(int userAssignmentHistoryID);
    partial void AfterRowDelete(int userAssignmentHistoryID);    

    partial void BeforeDBDelete(int userAssignmentHistoryID);
    partial void AfterDBDelete(int userAssignmentHistoryID);    

    #endregion

    #region Public Methods

    public UserAssignmentHistoryItemProxy[] GetUserAssignmentHistoryItemProxies()
    {
      List<UserAssignmentHistoryItemProxy> list = new List<UserAssignmentHistoryItemProxy>();

      foreach (UserAssignmentHistoryItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userAssignmentHistoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserAssignmentHistory] WHERE ([UserAssignmentHistoryID] = @UserAssignmentHistoryID);";
        deleteCommand.Parameters.Add("UserAssignmentHistoryID", SqlDbType.Int);
        deleteCommand.Parameters["UserAssignmentHistoryID"].Value = userAssignmentHistoryID;

        BeforeDBDelete(userAssignmentHistoryID);
        BeforeRowDelete(userAssignmentHistoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(userAssignmentHistoryID);
        AfterDBDelete(userAssignmentHistoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("UserAssignmentHistorySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[UserAssignmentHistory] SET     [TicketID] = @TicketID,    [UserID] = @UserID,    [DateAssigned] = @DateAssigned  WHERE ([UserAssignmentHistoryID] = @UserAssignmentHistoryID);";

		
		tempParameter = updateCommand.Parameters.Add("UserAssignmentHistoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("DateAssigned", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[UserAssignmentHistory] (    [TicketID],    [UserID],    [DateAssigned]) VALUES ( @TicketID, @UserID, @DateAssigned); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateAssigned", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserAssignmentHistory] WHERE ([UserAssignmentHistoryID] = @UserAssignmentHistoryID);";
		deleteCommand.Parameters.Add("UserAssignmentHistoryID", SqlDbType.Int);

		try
		{
		  foreach (UserAssignmentHistoryItem userAssignmentHistoryItem in this)
		  {
			if (userAssignmentHistoryItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(userAssignmentHistoryItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = userAssignmentHistoryItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserAssignmentHistoryID"].AutoIncrement = false;
			  Table.Columns["UserAssignmentHistoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				userAssignmentHistoryItem.Row["UserAssignmentHistoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(userAssignmentHistoryItem);
			}
			else if (userAssignmentHistoryItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(userAssignmentHistoryItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = userAssignmentHistoryItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(userAssignmentHistoryItem);
			}
			else if (userAssignmentHistoryItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)userAssignmentHistoryItem.Row["UserAssignmentHistoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["UserAssignmentHistoryID"].Value = id;
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

      foreach (UserAssignmentHistoryItem userAssignmentHistoryItem in this)
      {
        if (userAssignmentHistoryItem.Row.Table.Columns.Contains("CreatorID") && (int)userAssignmentHistoryItem.Row["CreatorID"] == 0) userAssignmentHistoryItem.Row["CreatorID"] = LoginUser.UserID;
        if (userAssignmentHistoryItem.Row.Table.Columns.Contains("ModifierID")) userAssignmentHistoryItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public UserAssignmentHistoryItem FindByUserAssignmentHistoryID(int userAssignmentHistoryID)
    {
      foreach (UserAssignmentHistoryItem userAssignmentHistoryItem in this)
      {
        if (userAssignmentHistoryItem.UserAssignmentHistoryID == userAssignmentHistoryID)
        {
          return userAssignmentHistoryItem;
        }
      }
      return null;
    }

    public virtual UserAssignmentHistoryItem AddNewUserAssignmentHistoryItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("UserAssignmentHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new UserAssignmentHistoryItem(row, this);
    }
    
    public virtual void LoadByUserAssignmentHistoryID(int userAssignmentHistoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserAssignmentHistoryID], [TicketID], [UserID], [DateAssigned] FROM [dbo].[UserAssignmentHistory] WHERE ([UserAssignmentHistoryID] = @UserAssignmentHistoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserAssignmentHistoryID", userAssignmentHistoryID);
        Fill(command);
      }
    }
    
    public static UserAssignmentHistoryItem GetUserAssignmentHistoryItem(LoginUser loginUser, int userAssignmentHistoryID)
    {
      UserAssignmentHistory userAssignmentHistory = new UserAssignmentHistory(loginUser);
      userAssignmentHistory.LoadByUserAssignmentHistoryID(userAssignmentHistoryID);
      if (userAssignmentHistory.IsEmpty)
        return null;
      else
        return userAssignmentHistory[0];
    }
    
    
    

    #endregion

    #region IEnumerable<UserAssignmentHistoryItem> Members

    public IEnumerator<UserAssignmentHistoryItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new UserAssignmentHistoryItem(row, this);
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
