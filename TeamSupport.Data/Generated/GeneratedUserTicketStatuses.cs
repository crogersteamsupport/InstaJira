using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class UserTicketStatus : BaseItem
  {
    private UserTicketStatuses _userTicketStatuses;
    
    public UserTicketStatus(DataRow row, UserTicketStatuses userTicketStatuses): base(row, userTicketStatuses)
    {
      _userTicketStatuses = userTicketStatuses;
    }
	
    #region Properties
    
    public UserTicketStatuses Collection
    {
      get { return _userTicketStatuses; }
    }
        
    
    
    
    public int UserTicketStatusID
    {
      get { return (int)Row["UserTicketStatusID"]; }
    }
    

    

    
    public bool IsFlagged
    {
      get { return (bool)Row["IsFlagged"]; }
      set { Row["IsFlagged"] = CheckValue("IsFlagged", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateRead
    {
      get { return DateToLocal((DateTime)Row["DateRead"]); }
      set { Row["DateRead"] = CheckValue("DateRead", value); }
    }

    public DateTime DateReadUtc
    {
      get { return (DateTime)Row["DateRead"]; }
    }
    

    #endregion
    
    
  }

  public partial class UserTicketStatuses : BaseCollection, IEnumerable<UserTicketStatus>
  {
    public UserTicketStatuses(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "UserTicketStatuses"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserTicketStatusID"; }
    }



    public UserTicketStatus this[int index]
    {
      get { return new UserTicketStatus(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(UserTicketStatus userTicketStatus);
    partial void AfterRowInsert(UserTicketStatus userTicketStatus);
    partial void BeforeRowEdit(UserTicketStatus userTicketStatus);
    partial void AfterRowEdit(UserTicketStatus userTicketStatus);
    partial void BeforeRowDelete(int userTicketStatusID);
    partial void AfterRowDelete(int userTicketStatusID);    

    partial void BeforeDBDelete(int userTicketStatusID);
    partial void AfterDBDelete(int userTicketStatusID);    

    #endregion

    #region Public Methods

    public UserTicketStatusProxy[] GetUserTicketStatusProxies()
    {
      List<UserTicketStatusProxy> list = new List<UserTicketStatusProxy>();

      foreach (UserTicketStatus item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userTicketStatusID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserTicketStatuses] WHERE ([UserTicketStatusID] = @UserTicketStatusID);";
        deleteCommand.Parameters.Add("UserTicketStatusID", SqlDbType.Int);
        deleteCommand.Parameters["UserTicketStatusID"].Value = userTicketStatusID;

        BeforeDBDelete(userTicketStatusID);
        BeforeRowDelete(userTicketStatusID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(userTicketStatusID);
        AfterDBDelete(userTicketStatusID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("UserTicketStatusesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[UserTicketStatuses] SET     [TicketID] = @TicketID,    [UserID] = @UserID,    [IsFlagged] = @IsFlagged,    [DateRead] = @DateRead  WHERE ([UserTicketStatusID] = @UserTicketStatusID);";

		
		tempParameter = updateCommand.Parameters.Add("UserTicketStatusID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("IsFlagged", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateRead", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[UserTicketStatuses] (    [TicketID],    [UserID],    [IsFlagged],    [DateRead]) VALUES ( @TicketID, @UserID, @IsFlagged, @DateRead); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateRead", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsFlagged", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserTicketStatuses] WHERE ([UserTicketStatusID] = @UserTicketStatusID);";
		deleteCommand.Parameters.Add("UserTicketStatusID", SqlDbType.Int);

		try
		{
		  foreach (UserTicketStatus userTicketStatus in this)
		  {
			if (userTicketStatus.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(userTicketStatus);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = userTicketStatus.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserTicketStatusID"].AutoIncrement = false;
			  Table.Columns["UserTicketStatusID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				userTicketStatus.Row["UserTicketStatusID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(userTicketStatus);
			}
			else if (userTicketStatus.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(userTicketStatus);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = userTicketStatus.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(userTicketStatus);
			}
			else if (userTicketStatus.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)userTicketStatus.Row["UserTicketStatusID", DataRowVersion.Original];
			  deleteCommand.Parameters["UserTicketStatusID"].Value = id;
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

      foreach (UserTicketStatus userTicketStatus in this)
      {
        if (userTicketStatus.Row.Table.Columns.Contains("CreatorID") && (int)userTicketStatus.Row["CreatorID"] == 0) userTicketStatus.Row["CreatorID"] = LoginUser.UserID;
        if (userTicketStatus.Row.Table.Columns.Contains("ModifierID")) userTicketStatus.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public UserTicketStatus FindByUserTicketStatusID(int userTicketStatusID)
    {
      foreach (UserTicketStatus userTicketStatus in this)
      {
        if (userTicketStatus.UserTicketStatusID == userTicketStatusID)
        {
          return userTicketStatus;
        }
      }
      return null;
    }

    public virtual UserTicketStatus AddNewUserTicketStatus()
    {
      if (Table.Columns.Count < 1) LoadColumns("UserTicketStatuses");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new UserTicketStatus(row, this);
    }
    
    public virtual void LoadByUserTicketStatusID(int userTicketStatusID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserTicketStatusID], [TicketID], [UserID], [IsFlagged], [DateRead] FROM [dbo].[UserTicketStatuses] WHERE ([UserTicketStatusID] = @UserTicketStatusID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserTicketStatusID", userTicketStatusID);
        Fill(command);
      }
    }
    
    public static UserTicketStatus GetUserTicketStatus(LoginUser loginUser, int userTicketStatusID)
    {
      UserTicketStatuses userTicketStatuses = new UserTicketStatuses(loginUser);
      userTicketStatuses.LoadByUserTicketStatusID(userTicketStatusID);
      if (userTicketStatuses.IsEmpty)
        return null;
      else
        return userTicketStatuses[0];
    }
    
    
    

    #endregion

    #region IEnumerable<UserTicketStatus> Members

    public IEnumerator<UserTicketStatus> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new UserTicketStatus(row, this);
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
