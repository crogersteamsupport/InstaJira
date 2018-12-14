using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ForumTicket : BaseItem
  {
    private ForumTickets _forumTickets;
    
    public ForumTicket(DataRow row, ForumTickets forumTickets): base(row, forumTickets)
    {
      _forumTickets = forumTickets;
    }
	
    #region Properties
    
    public ForumTickets Collection
    {
      get { return _forumTickets; }
    }
        
    
    
    

    
    public int? ForumCategory
    {
      get { return Row["ForumCategory"] != DBNull.Value ? (int?)Row["ForumCategory"] : null; }
      set { Row["ForumCategory"] = CheckValue("ForumCategory", value); }
    }
    
    public int? ViewCount
    {
      get { return Row["ViewCount"] != DBNull.Value ? (int?)Row["ViewCount"] : null; }
      set { Row["ViewCount"] = CheckValue("ViewCount", value); }
    }
    

    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ForumTickets : BaseCollection, IEnumerable<ForumTicket>
  {
    public ForumTickets(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ForumTickets"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public ForumTicket this[int index]
    {
      get { return new ForumTicket(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ForumTicket forumTicket);
    partial void AfterRowInsert(ForumTicket forumTicket);
    partial void BeforeRowEdit(ForumTicket forumTicket);
    partial void AfterRowEdit(ForumTicket forumTicket);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public ForumTicketProxy[] GetForumTicketProxies()
    {
      List<ForumTicketProxy> list = new List<ForumTicketProxy>();

      foreach (ForumTicket item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ForumTickets] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeDBDelete(ticketID);
        BeforeRowDelete(ticketID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketID);
        AfterDBDelete(ticketID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ForumTicketsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ForumTickets] SET     [ForumCategory] = @ForumCategory,    [ViewCount] = @ViewCount  WHERE ([TicketID] = @TicketID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ForumCategory", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViewCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ForumTickets] (    [TicketID],    [ForumCategory],    [ViewCount]) VALUES ( @TicketID, @ForumCategory, @ViewCount); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ViewCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ForumCategory", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ForumTickets] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (ForumTicket forumTicket in this)
		  {
			if (forumTicket.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(forumTicket);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = forumTicket.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				forumTicket.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(forumTicket);
			}
			else if (forumTicket.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(forumTicket);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = forumTicket.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(forumTicket);
			}
			else if (forumTicket.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)forumTicket.Row["TicketID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketID"].Value = id;
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

      foreach (ForumTicket forumTicket in this)
      {
        if (forumTicket.Row.Table.Columns.Contains("CreatorID") && (int)forumTicket.Row["CreatorID"] == 0) forumTicket.Row["CreatorID"] = LoginUser.UserID;
        if (forumTicket.Row.Table.Columns.Contains("ModifierID")) forumTicket.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ForumTicket FindByTicketID(int ticketID)
    {
      foreach (ForumTicket forumTicket in this)
      {
        if (forumTicket.TicketID == ticketID)
        {
          return forumTicket;
        }
      }
      return null;
    }

    public virtual ForumTicket AddNewForumTicket()
    {
      if (Table.Columns.Count < 1) LoadColumns("ForumTickets");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ForumTicket(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [ForumCategory], [ViewCount] FROM [dbo].[ForumTickets] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static ForumTicket GetForumTicket(LoginUser loginUser, int ticketID)
    {
      ForumTickets forumTickets = new ForumTickets(loginUser);
      forumTickets.LoadByTicketID(ticketID);
      if (forumTickets.IsEmpty)
        return null;
      else
        return forumTickets[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ForumTicket> Members

    public IEnumerator<ForumTicket> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ForumTicket(row, this);
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
