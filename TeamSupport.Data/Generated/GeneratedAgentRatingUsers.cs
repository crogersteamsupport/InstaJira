using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AgentRatingUser : BaseItem
  {
    private AgentRatingUsers _agentRatingUsers;
    
    public AgentRatingUser(DataRow row, AgentRatingUsers agentRatingUsers): base(row, agentRatingUsers)
    {
      _agentRatingUsers = agentRatingUsers;
    }
	
    #region Properties
    
    public AgentRatingUsers Collection
    {
      get { return _agentRatingUsers; }
    }
        
    
    
    

    

    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int AgentRatingID
    {
      get { return (int)Row["AgentRatingID"]; }
      set { Row["AgentRatingID"] = CheckValue("AgentRatingID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class AgentRatingUsers : BaseCollection, IEnumerable<AgentRatingUser>
  {
    public AgentRatingUsers(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AgentRatingUsers"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AgentRatingID"; }
    }



    public AgentRatingUser this[int index]
    {
      get { return new AgentRatingUser(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AgentRatingUser agentRatingUser);
    partial void AfterRowInsert(AgentRatingUser agentRatingUser);
    partial void BeforeRowEdit(AgentRatingUser agentRatingUser);
    partial void AfterRowEdit(AgentRatingUser agentRatingUser);
    partial void BeforeRowDelete(int agentRatingID);
    partial void AfterRowDelete(int agentRatingID);    

    partial void BeforeDBDelete(int agentRatingID);
    partial void AfterDBDelete(int agentRatingID);    

    #endregion

    #region Public Methods

    public AgentRatingUserProxy[] GetAgentRatingUserProxies()
    {
      List<AgentRatingUserProxy> list = new List<AgentRatingUserProxy>();

      foreach (AgentRatingUser item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int agentRatingID)
    {
      BeforeDBDelete(agentRatingID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AgentRatingUsers] WHERE ([AgentRatingID] = @AgentRatingID AND [UserID] = @UserID);";
        deleteCommand.Parameters.Add("AgentRatingID", SqlDbType.Int);
        deleteCommand.Parameters["AgentRatingID"].Value = agentRatingID;

        BeforeRowDelete(agentRatingID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(agentRatingID);
      }
      AfterDBDelete(agentRatingID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AgentRatingUsersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[AgentRatingUsers] SET   WHERE ([AgentRatingID] = @AgentRatingID AND [UserID] = @UserID);";

		
		tempParameter = updateCommand.Parameters.Add("AgentRatingID", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[AgentRatingUsers] (    [AgentRatingID],    [UserID]) VALUES ( @AgentRatingID, @UserID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("AgentRatingID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AgentRatingUsers] WHERE ([AgentRatingID] = @AgentRatingID);";
		deleteCommand.Parameters.Add("AgentRatingID", SqlDbType.Int);

		try
		{
		  foreach (AgentRatingUser agentRatingUser in this)
		  {
			if (agentRatingUser.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(agentRatingUser);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = agentRatingUser.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AgentRatingID"].AutoIncrement = false;
			  Table.Columns["AgentRatingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				agentRatingUser.Row["AgentRatingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(agentRatingUser);
			}
			else if (agentRatingUser.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(agentRatingUser);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = agentRatingUser.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(agentRatingUser);
			}
			else if (agentRatingUser.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)agentRatingUser.Row["AgentRatingID", DataRowVersion.Original];
			  deleteCommand.Parameters["AgentRatingID"].Value = id;
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

      foreach (AgentRatingUser agentRatingUser in this)
      {
        if (agentRatingUser.Row.Table.Columns.Contains("CreatorID") && (int)agentRatingUser.Row["CreatorID"] == 0) agentRatingUser.Row["CreatorID"] = LoginUser.UserID;
        if (agentRatingUser.Row.Table.Columns.Contains("ModifierID")) agentRatingUser.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AgentRatingUser FindByAgentRatingID(int agentRatingID)
    {
      foreach (AgentRatingUser agentRatingUser in this)
      {
        if (agentRatingUser.AgentRatingID == agentRatingID)
        {
          return agentRatingUser;
        }
      }
      return null;
    }

    public virtual AgentRatingUser AddNewAgentRatingUser()
    {
      if (Table.Columns.Count < 1) LoadColumns("AgentRatingUsers");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AgentRatingUser(row, this);
    }
    
    public virtual void LoadByAgentRatingID(int agentRatingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [UserID] FROM [dbo].[AgentRatingUsers] WHERE ([AgentRatingID] = @AgentRatingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AgentRatingID", agentRatingID);
        Fill(command);
      }
    }

    public virtual void LoadByUserID(int UserID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [UserID] FROM [dbo].[AgentRatingUsers] WHERE ([UserID] = @UserID);";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("UserID", UserID);
            Fill(command);
        }
    }

    public static AgentRatingUser GetAgentRatingUser(LoginUser loginUser, int agentRatingID)
    {
      AgentRatingUsers agentRatingUsers = new AgentRatingUsers(loginUser);
      agentRatingUsers.LoadByAgentRatingID(agentRatingID);
      if (agentRatingUsers.IsEmpty)
        return null;
      else
        return agentRatingUsers[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AgentRatingUser> Members

    public IEnumerator<AgentRatingUser> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AgentRatingUser(row, this);
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
