using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketStatus : BaseItem
  {
    private TicketStatuses _ticketStatuses;
    
    public TicketStatus(DataRow row, TicketStatuses ticketStatuses): base(row, ticketStatuses)
    {
      _ticketStatuses = ticketStatuses;
    }
	
    #region Properties
    
    public TicketStatuses Collection
    {
      get { return _ticketStatuses; }
    }
        
    
    
    
    public int TicketStatusID
    {
      get { return (int)Row["TicketStatusID"]; }
    }
    

    

    
    public bool PauseSLA
    {
      get { return (bool)Row["PauseSLA"]; }
      set { Row["PauseSLA"] = CheckValue("PauseSLA", value); }
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
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public bool IsEmailResponse
    {
      get { return (bool)Row["IsEmailResponse"]; }
      set { Row["IsEmailResponse"] = CheckValue("IsEmailResponse", value); }
    }
    
    public bool IsClosedEmail
    {
      get { return (bool)Row["IsClosedEmail"]; }
      set { Row["IsClosedEmail"] = CheckValue("IsClosedEmail", value); }
    }
    
    public bool IsClosed
    {
      get { return (bool)Row["IsClosed"]; }
      set { Row["IsClosed"] = CheckValue("IsClosed", value); }
    }
    
    public int TicketTypeID
    {
      get { return (int)Row["TicketTypeID"]; }
      set { Row["TicketTypeID"] = CheckValue("TicketTypeID", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
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

  public partial class TicketStatuses : BaseCollection, IEnumerable<TicketStatus>
  {
    public TicketStatuses(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketStatuses"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketStatusID"; }
    }



    public TicketStatus this[int index]
    {
      get { return new TicketStatus(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketStatus ticketStatus);
    partial void AfterRowInsert(TicketStatus ticketStatus);
    partial void BeforeRowEdit(TicketStatus ticketStatus);
    partial void AfterRowEdit(TicketStatus ticketStatus);
    partial void BeforeRowDelete(int ticketStatusID);
    partial void AfterRowDelete(int ticketStatusID);    

    partial void BeforeDBDelete(int ticketStatusID);
    partial void AfterDBDelete(int ticketStatusID);    

    #endregion

    #region Public Methods

    public TicketStatusProxy[] GetTicketStatusProxies()
    {
      List<TicketStatusProxy> list = new List<TicketStatusProxy>();

      foreach (TicketStatus item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketStatusID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketStatuses] WHERE ([TicketStatusID] = @TicketStatusID);";
        deleteCommand.Parameters.Add("TicketStatusID", SqlDbType.Int);
        deleteCommand.Parameters["TicketStatusID"].Value = ticketStatusID;

        BeforeDBDelete(ticketStatusID);
        BeforeRowDelete(ticketStatusID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketStatusID);
        AfterDBDelete(ticketStatusID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketStatusesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketStatuses] SET     [Name] = @Name,    [Description] = @Description,    [Position] = @Position,    [TicketTypeID] = @TicketTypeID,    [IsClosed] = @IsClosed,    [IsClosedEmail] = @IsClosedEmail,    [IsEmailResponse] = @IsEmailResponse,    [OrganizationID] = @OrganizationID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [PauseSLA] = @PauseSLA  WHERE ([TicketStatusID] = @TicketStatusID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsClosed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsClosedEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsEmailResponse", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("PauseSLA", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketStatuses] (    [Name],    [Description],    [Position],    [TicketTypeID],    [IsClosed],    [IsClosedEmail],    [IsEmailResponse],    [OrganizationID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [PauseSLA]) VALUES ( @Name, @Description, @Position, @TicketTypeID, @IsClosed, @IsClosedEmail, @IsEmailResponse, @OrganizationID, @DateCreated, @DateModified, @CreatorID, @ModifierID, @PauseSLA); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("PauseSLA", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
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
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsEmailResponse", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsClosedEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsClosed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketTypeID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketStatuses] WHERE ([TicketStatusID] = @TicketStatusID);";
		deleteCommand.Parameters.Add("TicketStatusID", SqlDbType.Int);

		try
		{
		  foreach (TicketStatus ticketStatus in this)
		  {
			if (ticketStatus.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketStatus);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketStatus.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketStatusID"].AutoIncrement = false;
			  Table.Columns["TicketStatusID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketStatus.Row["TicketStatusID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketStatus);
			}
			else if (ticketStatus.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketStatus);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketStatus.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketStatus);
			}
			else if (ticketStatus.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketStatus.Row["TicketStatusID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketStatusID"].Value = id;
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

      foreach (TicketStatus ticketStatus in this)
      {
        if (ticketStatus.Row.Table.Columns.Contains("CreatorID") && (int)ticketStatus.Row["CreatorID"] == 0) ticketStatus.Row["CreatorID"] = LoginUser.UserID;
        if (ticketStatus.Row.Table.Columns.Contains("ModifierID")) ticketStatus.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketStatus FindByTicketStatusID(int ticketStatusID)
    {
      foreach (TicketStatus ticketStatus in this)
      {
        if (ticketStatus.TicketStatusID == ticketStatusID)
        {
          return ticketStatus;
        }
      }
      return null;
    }

    public virtual TicketStatus AddNewTicketStatus()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketStatuses");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketStatus(row, this);
    }
    
    public virtual void LoadByTicketStatusID(int ticketStatusID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketStatusID], [Name], [Description], [Position], [TicketTypeID], [IsClosed], [IsClosedEmail], [IsEmailResponse], [OrganizationID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [PauseSLA] FROM [dbo].[TicketStatuses] WHERE ([TicketStatusID] = @TicketStatusID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketStatusID", ticketStatusID);
        Fill(command);
      }
    }
    
    public static TicketStatus GetTicketStatus(LoginUser loginUser, int ticketStatusID)
    {
      TicketStatuses ticketStatuses = new TicketStatuses(loginUser);
      ticketStatuses.LoadByTicketStatusID(ticketStatusID);
      if (ticketStatuses.IsEmpty)
        return null;
      else
        return ticketStatuses[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketStatus> Members

    public IEnumerator<TicketStatus> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketStatus(row, this);
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
