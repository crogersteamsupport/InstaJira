using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AgentRating : BaseItem
  {
    private AgentRatings _agentRatings;
    
    public AgentRating(DataRow row, AgentRatings agentRatings): base(row, agentRatings)
    {
      _agentRatings = agentRatings;
    }
	
    #region Properties
    
    public AgentRatings Collection
    {
      get { return _agentRatings; }
    }
        
    
    
    
    public int AgentRatingID
    {
      get { return (int)Row["AgentRatingID"]; }
    }
    

    
    public string Comment
    {
      get { return Row["Comment"] != DBNull.Value ? (string)Row["Comment"] : null; }
      set { Row["Comment"] = CheckValue("Comment", value); }
    }
    

    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    
    public int Rating
    {
      get { return (int)Row["Rating"]; }
      set { Row["Rating"] = CheckValue("Rating", value); }
    }
    
    public int ContactID
    {
      get { return (int)Row["ContactID"]; }
      set { Row["ContactID"] = CheckValue("ContactID", value); }
    }
    
    public int CompanyID
    {
      get { return (int)Row["CompanyID"]; }
      set { Row["CompanyID"] = CheckValue("CompanyID", value); }
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

  public partial class AgentRatings : BaseCollection, IEnumerable<AgentRating>
  {
    public AgentRatings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AgentRatings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AgentRatingID"; }
    }



    public AgentRating this[int index]
    {
      get { return new AgentRating(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AgentRating agentRating);
    partial void AfterRowInsert(AgentRating agentRating);
    partial void BeforeRowEdit(AgentRating agentRating);
    partial void AfterRowEdit(AgentRating agentRating);
    partial void BeforeRowDelete(int agentRatingID);
    partial void AfterRowDelete(int agentRatingID);    

    partial void BeforeDBDelete(int agentRatingID);
    partial void AfterDBDelete(int agentRatingID);    

    #endregion

    #region Public Methods

    public AgentRatingProxy[] GetAgentRatingProxies()
    {
      List<AgentRatingProxy> list = new List<AgentRatingProxy>();

      foreach (AgentRating item in this)
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
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] = @AgentRatingID);";
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
		//SqlTransaction transaction = connection.BeginTransaction("AgentRatingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[AgentRatings] SET     [OrganizationID] = @OrganizationID,    [CompanyID] = @CompanyID,    [ContactID] = @ContactID,    [Rating] = @Rating,    [Comment] = @Comment,    [TicketID] = @TicketID  WHERE ([AgentRatingID] = @AgentRatingID);";

		
		tempParameter = updateCommand.Parameters.Add("AgentRatingID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("CompanyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ContactID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Rating", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Comment", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[AgentRatings] (    [OrganizationID],    [CompanyID],    [ContactID],    [Rating],    [Comment],    [DateCreated],    [TicketID]) VALUES ( @OrganizationID, @CompanyID, @ContactID, @Rating, @Comment, @DateCreated, @TicketID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("Comment", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Rating", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ContactID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CompanyID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] = @AgentRatingID);";
		deleteCommand.Parameters.Add("AgentRatingID", SqlDbType.Int);

		try
		{
		  foreach (AgentRating agentRating in this)
		  {
			if (agentRating.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(agentRating);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = agentRating.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AgentRatingID"].AutoIncrement = false;
			  Table.Columns["AgentRatingID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				agentRating.Row["AgentRatingID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(agentRating);
			}
			else if (agentRating.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(agentRating);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = agentRating.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(agentRating);
			}
			else if (agentRating.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)agentRating.Row["AgentRatingID", DataRowVersion.Original];
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

      foreach (AgentRating agentRating in this)
      {
        if (agentRating.Row.Table.Columns.Contains("CreatorID") && (int)agentRating.Row["CreatorID"] == 0) agentRating.Row["CreatorID"] = LoginUser.UserID;
        if (agentRating.Row.Table.Columns.Contains("ModifierID")) agentRating.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AgentRating FindByAgentRatingID(int agentRatingID)
    {
      foreach (AgentRating agentRating in this)
      {
        if (agentRating.AgentRatingID == agentRatingID)
        {
          return agentRating;
        }
      }
      return null;
    }

    public virtual AgentRating AddNewAgentRating()
    {
      if (Table.Columns.Count < 1) LoadColumns("AgentRatings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AgentRating(row, this);
    }
    
    public virtual void LoadByAgentRatingID(int agentRatingID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] = @AgentRatingID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AgentRatingID", agentRatingID);
        Fill(command);
      }
    }

    public virtual void LoadByAgentRatingIDFilter(int[] agentRatingIDs, string filter, int start)
    {
        int end = start + 50;
        using (SqlCommand command = new SqlCommand())
        {
            if (start != -1)
            {
                if (filter != "")
                    command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + ") and [Rating]=@filter)) as temp where rownum between @start and @end order by rownum asc;";
                else
                    command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] in (" + DataUtils.IntArrayToCommaString(agentRatingIDs)+ "))) as temp where rownum between @start and @end order by rownum asc;";
            }
            else
            {
                if (filter != "")
                    command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + ") and [Rating]=@filter);";
                else
                    command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([AgentRatingID] in (" + DataUtils.IntArrayToCommaString(agentRatingIDs) + "));";
            }
            command.CommandType = CommandType.Text;
            //command.Parameters.AddWithValue("AgentRatingID", );
            command.Parameters.AddWithValue("filter", filter);
            command.Parameters.AddWithValue("start", start);
            command.Parameters.AddWithValue("end", end);
                Fill(command);

        }
    }

    public virtual void LoadByOrganizationIDFilter(int organizationID, string filter, int start)
    {
        int end = start + 50;
        using (SqlCommand command = new SqlCommand())
        {

            if (start != -1)
            {
                if (filter != "")
                    command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([CompanyID] = @CompanyID and [Rating]=@filter)) as temp where rownum between @start and @end order by rownum asc;";
                else
                    command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([CompanyID] = @CompanyID)) as temp where rownum between @start and @end order by rownum asc;";
            }
            else
            {
                if (filter != "")
                    command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([CompanyID] = @CompanyID and [Rating]=@filter);";
                else
                    command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([CompanyID] = @CompanyID)";

            }


            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("CompanyID", organizationID);
            command.Parameters.AddWithValue("filter", filter);
            command.Parameters.AddWithValue("start", start);
            command.Parameters.AddWithValue("end", end);
            Fill(command);
        }
    }


    public virtual void LoadByContactIDFilter(int userID, string filter, int start)
    {
        int end = start + 50;
        using (SqlCommand command = new SqlCommand())
        {

            if (start != -1)
            {
                if (filter != "")
                    command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([ContactID] = @userID and [Rating]=@filter)) as temp where rownum between @start and @end order by rownum asc;";
                else
                    command.CommandText = "SET NOCOUNT OFF; select * from (SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID], ROW_NUMBER() OVER (ORDER BY DateCreated Desc) AS rownum FROM [dbo].[AgentRatings] WHERE ([ContactID] = @userID)) as temp where rownum between @start and @end order by rownum asc;";
            }
            else
            {
                if (filter != "")
                    command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([ContactID] = @userID and [Rating]=@filter);";
                else
                    command.CommandText = "SET NOCOUNT OFF; SELECT [AgentRatingID], [OrganizationID], [CompanyID], [ContactID], [Rating], [Comment], [DateCreated], [TicketID] FROM [dbo].[AgentRatings] WHERE ([ContactID] = @userID)";

            }


            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("userID", userID);
            command.Parameters.AddWithValue("filter", filter);
            command.Parameters.AddWithValue("start", start);
            command.Parameters.AddWithValue("end", end);
            Fill(command);
        }
    }

    public static AgentRating GetAgentRating(LoginUser loginUser, int agentRatingID)
    {
      AgentRatings agentRatings = new AgentRatings(loginUser);
      agentRatings.LoadByAgentRatingID(agentRatingID);
      if (agentRatings.IsEmpty)
        return null;
      else
        return agentRatings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AgentRating> Members

    public IEnumerator<AgentRating> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AgentRating(row, this);
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
