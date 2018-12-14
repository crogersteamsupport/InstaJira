using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketRating : BaseItem
  {
    private TicketRatings _ticketRatings;
    
    public TicketRating(DataRow row, TicketRatings ticketRatings): base(row, ticketRatings)
    {
      _ticketRatings = ticketRatings;
    }
	
    #region Properties
    
    public TicketRatings Collection
    {
      get { return _ticketRatings; }
    }
        
    
    
    

    
    public int? TicketType
    {
      get { return Row["TicketType"] != DBNull.Value ? (int?)Row["TicketType"] : null; }
      set { Row["TicketType"] = CheckValue("TicketType", value); }
    }
    
    public int? Votes
    {
      get { return Row["Votes"] != DBNull.Value ? (int?)Row["Votes"] : null; }
      set { Row["Votes"] = CheckValue("Votes", value); }
    }
    
    public float? Rating
    {
      get { return Row["Rating"] != DBNull.Value ? (float?)Row["Rating"] : null; }
      set { Row["Rating"] = CheckValue("Rating", value); }
    }
    
    public int? Views
    {
      get { return Row["Views"] != DBNull.Value ? (int?)Row["Views"] : null; }
      set { Row["Views"] = CheckValue("Views", value); }
    }
    
    public int? ThumbsUp
    {
      get { return Row["ThumbsUp"] != DBNull.Value ? (int?)Row["ThumbsUp"] : null; }
      set { Row["ThumbsUp"] = CheckValue("ThumbsUp", value); }
    }
    
    public int? ThumbsDown
    {
      get { return Row["ThumbsDown"] != DBNull.Value ? (int?)Row["ThumbsDown"] : null; }
      set { Row["ThumbsDown"] = CheckValue("ThumbsDown", value); }
    }
    

    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LastViewed
    {
      get { return Row["LastViewed"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastViewed"]) : null; }
      set { Row["LastViewed"] = CheckValue("LastViewed", value); }
    }

    public DateTime? LastViewedUtc
    {
      get { return Row["LastViewed"] != DBNull.Value ? (DateTime?)Row["LastViewed"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class TicketRatings : BaseCollection, IEnumerable<TicketRating>
  {
    public TicketRatings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketRatings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public TicketRating this[int index]
    {
      get { return new TicketRating(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketRating ticketRating);
    partial void AfterRowInsert(TicketRating ticketRating);
    partial void BeforeRowEdit(TicketRating ticketRating);
    partial void AfterRowEdit(TicketRating ticketRating);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public TicketRatingProxy[] GetTicketRatingProxies()
    {
      List<TicketRatingProxy> list = new List<TicketRatingProxy>();

      foreach (TicketRating item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketRatings] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeDBDelete(ticketID);
        BeforeRowDelete(ticketID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketID);
        AfterDBDelete(ticketID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketRatingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketRatings] SET     [TicketType] = @TicketType,    [Votes] = @Votes,    [Rating] = @Rating,    [Views] = @Views,    [ThumbsUp] = @ThumbsUp,    [ThumbsDown] = @ThumbsDown,    [LastViewed] = @LastViewed  WHERE ([TicketID] = @TicketID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Votes", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Rating", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = updateCommand.Parameters.Add("Views", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ThumbsUp", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ThumbsDown", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastViewed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketRatings] (    [TicketID],    [TicketType],    [Votes],    [Rating],    [Views],    [ThumbsUp],    [ThumbsDown],    [LastViewed]) VALUES ( @TicketID, @TicketType, @Votes, @Rating, @Views, @ThumbsUp, @ThumbsDown, @LastViewed); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("LastViewed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ThumbsDown", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ThumbsUp", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Views", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Rating", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = insertCommand.Parameters.Add("Votes", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketType", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketRatings] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (TicketRating ticketRating in this)
		  {
			if (ticketRating.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketRating);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketRating.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketRating.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketRating);
			}
			else if (ticketRating.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketRating);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketRating.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketRating);
			}
			else if (ticketRating.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketRating.Row["TicketID", DataRowVersion.Original];
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

      foreach (TicketRating ticketRating in this)
      {
        if (ticketRating.Row.Table.Columns.Contains("CreatorID") && (int)ticketRating.Row["CreatorID"] == 0) ticketRating.Row["CreatorID"] = LoginUser.UserID;
        if (ticketRating.Row.Table.Columns.Contains("ModifierID")) ticketRating.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketRating FindByTicketID(int ticketID)
    {
      foreach (TicketRating ticketRating in this)
      {
        if (ticketRating.TicketID == ticketID)
        {
          return ticketRating;
        }
      }
      return null;
    }

    public virtual TicketRating AddNewTicketRating()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketRatings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketRating(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [TicketType], [Votes], [Rating], [Views], [ThumbsUp], [ThumbsDown], [LastViewed] FROM [dbo].[TicketRatings] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static TicketRating GetTicketRating(LoginUser loginUser, int ticketID)
    {
      TicketRatings ticketRatings = new TicketRatings(loginUser);
      ticketRatings.LoadByTicketID(ticketID);
      if (ticketRatings.IsEmpty)
        return null;
      else
        return ticketRatings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketRating> Members

    public IEnumerator<TicketRating> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketRating(row, this);
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
