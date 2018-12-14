using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SlaTicket : BaseItem
  {
    private SlaTickets _slaTickets;
    
    public SlaTicket(DataRow row, SlaTickets slaTickets): base(row, slaTickets)
    {
      _slaTickets = slaTickets;
    }
	
    #region Properties
    
    public SlaTickets Collection
    {
      get { return _slaTickets; }
    }
        
    
    
    

    

    
    public bool IsPending
    {
      get { return (bool)Row["IsPending"]; }
      set { Row["IsPending"] = CheckValue("IsPending", value); }
    }
    
    public int SlaTriggerId
    {
      get { return (int)Row["SlaTriggerId"]; }
      set { Row["SlaTriggerId"] = CheckValue("SlaTriggerId", value); }
    }
    
    public int TicketId
    {
      get { return (int)Row["TicketId"]; }
      set { Row["TicketId"] = CheckValue("TicketId", value); }
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

  public partial class SlaTickets : BaseCollection, IEnumerable<SlaTicket>
  {
    public SlaTickets(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SlaTickets"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketId"; }
    }



    public SlaTicket this[int index]
    {
      get { return new SlaTicket(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SlaTicket slaTicket);
    partial void AfterRowInsert(SlaTicket slaTicket);
    partial void BeforeRowEdit(SlaTicket slaTicket);
    partial void AfterRowEdit(SlaTicket slaTicket);
    partial void BeforeRowDelete(int ticketId);
    partial void AfterRowDelete(int ticketId);    

    partial void BeforeDBDelete(int ticketId);
    partial void AfterDBDelete(int ticketId);    

    #endregion

    #region Public Methods

    public SlaTicketProxy[] GetSlaTicketProxies()
    {
      List<SlaTicketProxy> list = new List<SlaTicketProxy>();

      foreach (SlaTicket item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketId)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaTickets] WHERE ([TicketId] = @TicketId);";
        deleteCommand.Parameters.Add("TicketId", SqlDbType.Int);
        deleteCommand.Parameters["TicketId"].Value = ticketId;

        BeforeDBDelete(ticketId);
        BeforeRowDelete(ticketId);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketId);
        AfterDBDelete(ticketId);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SlaTicketsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SlaTickets] SET     [SlaTriggerId] = @SlaTriggerId,    [IsPending] = @IsPending,    [DateModified] = @DateModified  WHERE ([TicketId] = @TicketId);";

		
		tempParameter = updateCommand.Parameters.Add("TicketId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaTriggerId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsPending", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SlaTickets] (    [TicketId],    [SlaTriggerId],    [IsPending],    [DateCreated],    [DateModified]) VALUES ( @TicketId, @SlaTriggerId, @IsPending, @DateCreated, @DateModified); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("IsPending", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaTriggerId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketId", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaTickets] WHERE ([TicketId] = @TicketId);";
		deleteCommand.Parameters.Add("TicketId", SqlDbType.Int);

		try
		{
		  foreach (SlaTicket slaTicket in this)
		  {
			if (slaTicket.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(slaTicket);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = slaTicket.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketId"].AutoIncrement = false;
			  Table.Columns["TicketId"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				slaTicket.Row["TicketId"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(slaTicket);
			}
			else if (slaTicket.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(slaTicket);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = slaTicket.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(slaTicket);
			}
			else if (slaTicket.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)slaTicket.Row["TicketId", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketId"].Value = id;
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

      foreach (SlaTicket slaTicket in this)
      {
        if (slaTicket.Row.Table.Columns.Contains("CreatorID") && (int)slaTicket.Row["CreatorID"] == 0) slaTicket.Row["CreatorID"] = LoginUser.UserID;
        if (slaTicket.Row.Table.Columns.Contains("ModifierID")) slaTicket.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SlaTicket FindByTicketId(int ticketId)
    {
      foreach (SlaTicket slaTicket in this)
      {
        if (slaTicket.TicketId == ticketId)
        {
          return slaTicket;
        }
      }
      return null;
    }

    public virtual SlaTicket AddNewSlaTicket()
    {
      if (Table.Columns.Count < 1) LoadColumns("SlaTickets");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SlaTicket(row, this);
    }
    
    public virtual void LoadByTicketId(int ticketId)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketId], [SlaTriggerId], [IsPending], [DateCreated], [DateModified] FROM [dbo].[SlaTickets] WHERE ([TicketId] = @TicketId);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketId", ticketId);
        Fill(command);
      }
    }
    
    public static SlaTicket GetSlaTicket(LoginUser loginUser, int ticketId)
    {
      SlaTickets slaTickets = new SlaTickets(loginUser);
      slaTickets.LoadByTicketId(ticketId);
      if (slaTickets.IsEmpty)
        return null;
      else
        return slaTickets[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SlaTicket> Members

    public IEnumerator<SlaTicket> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SlaTicket(row, this);
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
