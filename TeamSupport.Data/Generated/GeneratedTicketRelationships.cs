using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketRelationship : BaseItem
  {
    private TicketRelationships _ticketRelationships;
    
    public TicketRelationship(DataRow row, TicketRelationships ticketRelationships): base(row, ticketRelationships)
    {
      _ticketRelationships = ticketRelationships;
    }
	
    #region Properties
    
    public TicketRelationships Collection
    {
      get { return _ticketRelationships; }
    }
        
    
    
    
    public int TicketRelationshipID
    {
      get { return (int)Row["TicketRelationshipID"]; }
    }
    

    
    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
    }
    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int Ticket2ID
    {
      get { return (int)Row["Ticket2ID"]; }
      set { Row["Ticket2ID"] = CheckValue("Ticket2ID", value); }
    }
    
    public int Ticket1ID
    {
      get { return (int)Row["Ticket1ID"]; }
      set { Row["Ticket1ID"] = CheckValue("Ticket1ID", value); }
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

  public partial class TicketRelationships : BaseCollection, IEnumerable<TicketRelationship>
  {
    public TicketRelationships(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketRelationships"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketRelationshipID"; }
    }



    public TicketRelationship this[int index]
    {
      get { return new TicketRelationship(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketRelationship ticketRelationship);
    partial void AfterRowInsert(TicketRelationship ticketRelationship);
    partial void BeforeRowEdit(TicketRelationship ticketRelationship);
    partial void AfterRowEdit(TicketRelationship ticketRelationship);
    partial void BeforeRowDelete(int ticketRelationshipID);
    partial void AfterRowDelete(int ticketRelationshipID);    

    partial void BeforeDBDelete(int ticketRelationshipID);
    partial void AfterDBDelete(int ticketRelationshipID);    

    #endregion

    #region Public Methods

    public TicketRelationshipProxy[] GetTicketRelationshipProxies()
    {
      List<TicketRelationshipProxy> list = new List<TicketRelationshipProxy>();

      foreach (TicketRelationship item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketRelationshipID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketRelationships] WHERE ([TicketRelationshipID] = @TicketRelationshipID);";
        deleteCommand.Parameters.Add("TicketRelationshipID", SqlDbType.Int);
        deleteCommand.Parameters["TicketRelationshipID"].Value = ticketRelationshipID;

        BeforeDBDelete(ticketRelationshipID);
        BeforeRowDelete(ticketRelationshipID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketRelationshipID);
        AfterDBDelete(ticketRelationshipID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketRelationshipsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketRelationships] SET     [OrganizationID] = @OrganizationID,    [Ticket1ID] = @Ticket1ID,    [Ticket2ID] = @Ticket2ID,    [ImportFileID] = @ImportFileID  WHERE ([TicketRelationshipID] = @TicketRelationshipID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketRelationshipID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Ticket1ID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Ticket2ID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketRelationships] (    [OrganizationID],    [Ticket1ID],    [Ticket2ID],    [CreatorID],    [DateCreated],    [ImportFileID]) VALUES ( @OrganizationID, @Ticket1ID, @Ticket2ID, @CreatorID, @DateCreated, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Ticket2ID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Ticket1ID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketRelationships] WHERE ([TicketRelationshipID] = @TicketRelationshipID);";
		deleteCommand.Parameters.Add("TicketRelationshipID", SqlDbType.Int);

		try
		{
		  foreach (TicketRelationship ticketRelationship in this)
		  {
			if (ticketRelationship.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketRelationship);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketRelationship.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketRelationshipID"].AutoIncrement = false;
			  Table.Columns["TicketRelationshipID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketRelationship.Row["TicketRelationshipID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketRelationship);
			}
			else if (ticketRelationship.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketRelationship);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketRelationship.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketRelationship);
			}
			else if (ticketRelationship.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketRelationship.Row["TicketRelationshipID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketRelationshipID"].Value = id;
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

      foreach (TicketRelationship ticketRelationship in this)
      {
        if (ticketRelationship.Row.Table.Columns.Contains("CreatorID") && (int)ticketRelationship.Row["CreatorID"] == 0) ticketRelationship.Row["CreatorID"] = LoginUser.UserID;
        if (ticketRelationship.Row.Table.Columns.Contains("ModifierID")) ticketRelationship.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketRelationship FindByTicketRelationshipID(int ticketRelationshipID)
    {
      foreach (TicketRelationship ticketRelationship in this)
      {
        if (ticketRelationship.TicketRelationshipID == ticketRelationshipID)
        {
          return ticketRelationship;
        }
      }
      return null;
    }

    public virtual TicketRelationship AddNewTicketRelationship()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketRelationships");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketRelationship(row, this);
    }
    
    public virtual void LoadByTicketRelationshipID(int ticketRelationshipID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketRelationshipID], [OrganizationID], [Ticket1ID], [Ticket2ID], [CreatorID], [DateCreated], [ImportFileID] FROM [dbo].[TicketRelationships] WHERE ([TicketRelationshipID] = @TicketRelationshipID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketRelationshipID", ticketRelationshipID);
        Fill(command);
      }
    }
    
    public static TicketRelationship GetTicketRelationship(LoginUser loginUser, int ticketRelationshipID)
    {
      TicketRelationships ticketRelationships = new TicketRelationships(loginUser);
      ticketRelationships.LoadByTicketRelationshipID(ticketRelationshipID);
      if (ticketRelationships.IsEmpty)
        return null;
      else
        return ticketRelationships[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketRelationship> Members

    public IEnumerator<TicketRelationship> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketRelationship(row, this);
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
