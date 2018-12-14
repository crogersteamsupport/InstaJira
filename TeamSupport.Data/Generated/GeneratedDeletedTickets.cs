using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class DeletedTicket : BaseItem
  {
    private DeletedTickets _deletedTickets;
    
    public DeletedTicket(DataRow row, DeletedTickets deletedTickets): base(row, deletedTickets)
    {
      _deletedTickets = deletedTickets;
    }
	
    #region Properties
    
    public DeletedTickets Collection
    {
      get { return _deletedTickets; }
    }
        
    
    
    
    public int ID
    {
      get { return (int)Row["ID"]; }
    }
    

    
    public string Name
    {
      get { return Row["Name"] != DBNull.Value ? (string)Row["Name"] : null; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    

    
    public int DeleterID
    {
      get { return (int)Row["DeleterID"]; }
      set { Row["DeleterID"] = CheckValue("DeleterID", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int TicketNumber
    {
      get { return (int)Row["TicketNumber"]; }
      set { Row["TicketNumber"] = CheckValue("TicketNumber", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateDeleted
    {
      get { return DateToLocal((DateTime)Row["DateDeleted"]); }
      set { Row["DateDeleted"] = CheckValue("DateDeleted", value); }
    }

    public DateTime DateDeletedUtc
    {
      get { return (DateTime)Row["DateDeleted"]; }
    }
    

    #endregion
    
    
  }

  public partial class DeletedTickets : BaseCollection, IEnumerable<DeletedTicket>
  {
    public DeletedTickets(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "DeletedTickets"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ID"; }
    }



    public DeletedTicket this[int index]
    {
      get { return new DeletedTicket(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(DeletedTicket deletedTicket);
    partial void AfterRowInsert(DeletedTicket deletedTicket);
    partial void BeforeRowEdit(DeletedTicket deletedTicket);
    partial void AfterRowEdit(DeletedTicket deletedTicket);
    partial void BeforeRowDelete(int iD);
    partial void AfterRowDelete(int iD);    

    partial void BeforeDBDelete(int iD);
    partial void AfterDBDelete(int iD);    

    #endregion

    #region Public Methods

    public DeletedTicketProxy[] GetDeletedTicketProxies()
    {
      List<DeletedTicketProxy> list = new List<DeletedTicketProxy>();

      foreach (DeletedTicket item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int iD)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[DeletedTickets] WHERE ([ID] = @ID);";
        deleteCommand.Parameters.Add("ID", SqlDbType.Int);
        deleteCommand.Parameters["ID"].Value = iD;

        BeforeDBDelete(iD);
        BeforeRowDelete(iD);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(iD);
        AfterDBDelete(iD);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("DeletedTicketsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[DeletedTickets] SET     [TicketID] = @TicketID,    [TicketNumber] = @TicketNumber,    [OrganizationID] = @OrganizationID,    [Name] = @Name,    [DateDeleted] = @DateDeleted,    [DeleterID] = @DeleterID  WHERE ([ID] = @ID);";

		
		tempParameter = updateCommand.Parameters.Add("ID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateDeleted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DeleterID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[DeletedTickets] (    [TicketID],    [TicketNumber],    [OrganizationID],    [Name],    [DateDeleted],    [DeleterID]) VALUES ( @TicketID, @TicketNumber, @OrganizationID, @Name, @DateDeleted, @DeleterID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DeleterID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateDeleted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[DeletedTickets] WHERE ([ID] = @ID);";
		deleteCommand.Parameters.Add("ID", SqlDbType.Int);

		try
		{
		  foreach (DeletedTicket deletedTicket in this)
		  {
			if (deletedTicket.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(deletedTicket);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = deletedTicket.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ID"].AutoIncrement = false;
			  Table.Columns["ID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				deletedTicket.Row["ID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(deletedTicket);
			}
			else if (deletedTicket.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(deletedTicket);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = deletedTicket.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(deletedTicket);
			}
			else if (deletedTicket.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)deletedTicket.Row["ID", DataRowVersion.Original];
			  deleteCommand.Parameters["ID"].Value = id;
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

      foreach (DeletedTicket deletedTicket in this)
      {
        if (deletedTicket.Row.Table.Columns.Contains("CreatorID") && (int)deletedTicket.Row["CreatorID"] == 0) deletedTicket.Row["CreatorID"] = LoginUser.UserID;
        if (deletedTicket.Row.Table.Columns.Contains("ModifierID")) deletedTicket.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public DeletedTicket FindByID(int iD)
    {
      foreach (DeletedTicket deletedTicket in this)
      {
        if (deletedTicket.ID == iD)
        {
          return deletedTicket;
        }
      }
      return null;
    }

    public virtual DeletedTicket AddNewDeletedTicket()
    {
      if (Table.Columns.Count < 1) LoadColumns("DeletedTickets");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new DeletedTicket(row, this);
    }
    
    public virtual void LoadByID(int iD)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ID], [TicketID], [TicketNumber], [OrganizationID], [Name], [DateDeleted], [DeleterID] FROM [dbo].[DeletedTickets] WHERE ([ID] = @ID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ID", iD);
        Fill(command);
      }
    }
    
    public static DeletedTicket GetDeletedTicket(LoginUser loginUser, int iD)
    {
      DeletedTickets deletedTickets = new DeletedTickets(loginUser);
      deletedTickets.LoadByID(iD);
      if (deletedTickets.IsEmpty)
        return null;
      else
        return deletedTickets[0];
    }
    
    
    

    #endregion

    #region IEnumerable<DeletedTicket> Members

    public IEnumerator<DeletedTicket> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new DeletedTicket(row, this);
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
