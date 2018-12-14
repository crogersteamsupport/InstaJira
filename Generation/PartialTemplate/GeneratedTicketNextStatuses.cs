using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketNextStatus : BaseItem
  {
    private TicketNextStatuses _ticketNextStatuses;
    
    public TicketNextStatus(DataRow row, TicketNextStatuses ticketNextStatuses): base(row, ticketNextStatuses)
    {
      _ticketNextStatuses = ticketNextStatuses;
    }
	
    #region Properties
    
    public TicketNextStatuses Collection
    {
      get { return _ticketNextStatuses; }
    }
        
    
    
    
    public int TicketNextStatusID
    {
      get { return (int)Row["TicketNextStatusID"]; }
    }
    

    

    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public int NextStatusID
    {
      get { return (int)Row["NextStatusID"]; }
      set { Row["NextStatusID"] = CheckValue("NextStatusID", value); }
    }
    
    public int CurrentStatusID
    {
      get { return (int)Row["CurrentStatusID"]; }
      set { Row["CurrentStatusID"] = CheckValue("CurrentStatusID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
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
    

    #endregion
    
    
  }

  public partial class TicketNextStatuses : BaseCollection, IEnumerable<TicketNextStatus>
  {
    public TicketNextStatuses(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketNextStatuses"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketNextStatusID"; }
    }



    public TicketNextStatus this[int index]
    {
      get { return new TicketNextStatus(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketNextStatus ticketNextStatus);
    partial void AfterRowInsert(TicketNextStatus ticketNextStatus);
    partial void BeforeRowEdit(TicketNextStatus ticketNextStatus);
    partial void AfterRowEdit(TicketNextStatus ticketNextStatus);
    partial void BeforeRowDelete(int ticketNextStatusID);
    partial void AfterRowDelete(int ticketNextStatusID);    

    partial void BeforeDBDelete(int ticketNextStatusID);
    partial void AfterDBDelete(int ticketNextStatusID);    

    #endregion

    #region Public Methods

    public TicketNextStatusProxy[] GetTicketNextStatusProxies()
    {
      List<TicketNextStatusProxy> list = new List<TicketNextStatusProxy>();

      foreach (TicketNextStatus item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketNextStatusID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketNextStatuses] WHERE ([TicketNextStatusID] = @TicketNextStatusID);";
        deleteCommand.Parameters.Add("TicketNextStatusID", SqlDbType.Int);
        deleteCommand.Parameters["TicketNextStatusID"].Value = ticketNextStatusID;

        BeforeDBDelete(ticketNextStatusID);
        BeforeRowDelete(ticketNextStatusID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketNextStatusID);
        AfterDBDelete(ticketNextStatusID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketNextStatusesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketNextStatuses] SET     [CurrentStatusID] = @CurrentStatusID,    [NextStatusID] = @NextStatusID,    [Position] = @Position,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([TicketNextStatusID] = @TicketNextStatusID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketNextStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CurrentStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("NextStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketNextStatuses] (    [CurrentStatusID],    [NextStatusID],    [Position],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @CurrentStatusID, @NextStatusID, @Position, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("NextStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CurrentStatusID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketNextStatuses] WHERE ([TicketNextStatusID] = @TicketNextStatusID);";
		deleteCommand.Parameters.Add("TicketNextStatusID", SqlDbType.Int);

		try
		{
		  foreach (TicketNextStatus ticketNextStatus in this)
		  {
			if (ticketNextStatus.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketNextStatus);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketNextStatus.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketNextStatusID"].AutoIncrement = false;
			  Table.Columns["TicketNextStatusID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketNextStatus.Row["TicketNextStatusID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketNextStatus);
			}
			else if (ticketNextStatus.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketNextStatus);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketNextStatus.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketNextStatus);
			}
			else if (ticketNextStatus.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketNextStatus.Row["TicketNextStatusID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketNextStatusID"].Value = id;
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

      foreach (TicketNextStatus ticketNextStatus in this)
      {
        if (ticketNextStatus.Row.Table.Columns.Contains("CreatorID") && (int)ticketNextStatus.Row["CreatorID"] == 0) ticketNextStatus.Row["CreatorID"] = LoginUser.UserID;
        if (ticketNextStatus.Row.Table.Columns.Contains("ModifierID")) ticketNextStatus.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketNextStatus FindByTicketNextStatusID(int ticketNextStatusID)
    {
      foreach (TicketNextStatus ticketNextStatus in this)
      {
        if (ticketNextStatus.TicketNextStatusID == ticketNextStatusID)
        {
          return ticketNextStatus;
        }
      }
      return null;
    }

    public virtual TicketNextStatus AddNewTicketNextStatus()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketNextStatuses");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketNextStatus(row, this);
    }
    
    public virtual void LoadByTicketNextStatusID(int ticketNextStatusID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketNextStatusID], [CurrentStatusID], [NextStatusID], [Position], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[TicketNextStatuses] WHERE ([TicketNextStatusID] = @TicketNextStatusID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketNextStatusID", ticketNextStatusID);
        Fill(command);
      }
    }
    
    public static TicketNextStatus GetTicketNextStatus(LoginUser loginUser, int ticketNextStatusID)
    {
      TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(loginUser);
      ticketNextStatuses.LoadByTicketNextStatusID(ticketNextStatusID);
      if (ticketNextStatuses.IsEmpty)
        return null;
      else
        return ticketNextStatuses[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketNextStatus> Members

    public IEnumerator<TicketNextStatus> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketNextStatus(row, this);
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
