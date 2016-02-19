using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class KBRating : BaseItem
  {
    private KBRatings _kBRatings;
    
    public KBRating(DataRow row, KBRatings kBRatings): base(row, kBRatings)
    {
      _kBRatings = kBRatings;
    }
	
    #region Properties
    
    public KBRatings Collection
    {
      get { return _kBRatings; }
    }
        
    
    
    

    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public string IP
    {
      get { return Row["IP"] != DBNull.Value ? (string)Row["IP"] : null; }
      set { Row["IP"] = CheckValue("IP", value); }
    }
    
    public string Comment
    {
      get { return Row["Comment"] != DBNull.Value ? (string)Row["Comment"] : null; }
      set { Row["Comment"] = CheckValue("Comment", value); }
    }
    

    
    public bool Rating
    {
      get { return (bool)Row["Rating"]; }
      set { Row["Rating"] = CheckValue("Rating", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateUpdated
    {
      get { return Row["DateUpdated"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateUpdated"]) : null; }
      set { Row["DateUpdated"] = CheckValue("DateUpdated", value); }
    }

    public DateTime? DateUpdatedUtc
    {
      get { return Row["DateUpdated"] != DBNull.Value ? (DateTime?)Row["DateUpdated"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class KBRatings : BaseCollection, IEnumerable<KBRating>
  {
    public KBRatings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "KBRatings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public KBRating this[int index]
    {
      get { return new KBRating(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(KBRating kBRating);
    partial void AfterRowInsert(KBRating kBRating);
    partial void BeforeRowEdit(KBRating kBRating);
    partial void AfterRowEdit(KBRating kBRating);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public KBRatingProxy[] GetKBRatingProxies()
    {
      List<KBRatingProxy> list = new List<KBRatingProxy>();

      foreach (KBRating item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
      BeforeDBDelete(ticketID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[KBRatings] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeRowDelete(ticketID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(ticketID);
      }
      AfterDBDelete(ticketID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("KBRatingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[KBRatings] SET     [UserID] = @UserID,    [IP] = @IP,    [Rating] = @Rating,    [DateUpdated] = @DateUpdated,    [Comment] = @Comment  WHERE ([TicketID] = @TicketID);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("IP", SqlDbType.NVarChar, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Rating", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateUpdated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("Comment", SqlDbType.NVarChar, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[KBRatings] (    [TicketID],    [UserID],    [IP],    [Rating],    [DateUpdated],    [Comment]) VALUES ( @TicketID, @UserID, @IP, @Rating, @DateUpdated, @Comment); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Comment", SqlDbType.NVarChar, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateUpdated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Rating", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IP", SqlDbType.NVarChar, 1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[KBRatings] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (KBRating kBRating in this)
		  {
			if (kBRating.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(kBRating);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = kBRating.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				kBRating.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(kBRating);
			}
			else if (kBRating.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(kBRating);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = kBRating.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(kBRating);
			}
			else if (kBRating.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)kBRating.Row["TicketID", DataRowVersion.Original];
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

      foreach (KBRating kBRating in this)
      {
        if (kBRating.Row.Table.Columns.Contains("CreatorID") && (int)kBRating.Row["CreatorID"] == 0) kBRating.Row["CreatorID"] = LoginUser.UserID;
        if (kBRating.Row.Table.Columns.Contains("ModifierID")) kBRating.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public KBRating FindByTicketID(int ticketID)
    {
      foreach (KBRating kBRating in this)
      {
        if (kBRating.TicketID == ticketID)
        {
          return kBRating;
        }
      }
      return null;
    }

    public virtual KBRating AddNewKBRating()
    {
      if (Table.Columns.Count < 1) LoadColumns("KBRatings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new KBRating(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [UserID], [IP], [Rating], [DateUpdated], [Comment] FROM [dbo].[KBRatings] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static KBRating GetKBRating(LoginUser loginUser, int ticketID)
    {
      KBRatings kBRatings = new KBRatings(loginUser);
      kBRatings.LoadByTicketID(ticketID);
      if (kBRatings.IsEmpty)
        return null;
      else
        return kBRatings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<KBRating> Members

    public IEnumerator<KBRating> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new KBRating(row, this);
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
