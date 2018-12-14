using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AgentRatingsOption : BaseItem
  {
    private AgentRatingsOptions _agentRatingsOptions;
    
    public AgentRatingsOption(DataRow row, AgentRatingsOptions agentRatingsOptions): base(row, agentRatingsOptions)
    {
      _agentRatingsOptions = agentRatingsOptions;
    }
	
    #region Properties
    
    public AgentRatingsOptions Collection
    {
      get { return _agentRatingsOptions; }
    }
        
    
    
    

    
    public string PositiveRatingText
    {
      get { return Row["PositiveRatingText"] != DBNull.Value ? (string)Row["PositiveRatingText"] : null; }
      set { Row["PositiveRatingText"] = CheckValue("PositiveRatingText", value); }
    }
    
    public string NeutralRatingText
    {
      get { return Row["NeutralRatingText"] != DBNull.Value ? (string)Row["NeutralRatingText"] : null; }
      set { Row["NeutralRatingText"] = CheckValue("NeutralRatingText", value); }
    }
    
    public string NegativeRatingText
    {
      get { return Row["NegativeRatingText"] != DBNull.Value ? (string)Row["NegativeRatingText"] : null; }
      set { Row["NegativeRatingText"] = CheckValue("NegativeRatingText", value); }
    }
    
    public string PositiveImage
    {
      get { return Row["PositiveImage"] != DBNull.Value ? (string)Row["PositiveImage"] : null; }
      set { Row["PositiveImage"] = CheckValue("PositiveImage", value); }
    }
    
    public string NeutralImage
    {
      get { return Row["NeutralImage"] != DBNull.Value ? (string)Row["NeutralImage"] : null; }
      set { Row["NeutralImage"] = CheckValue("NeutralImage", value); }
    }
    
    public string NegativeImage
    {
      get { return Row["NegativeImage"] != DBNull.Value ? (string)Row["NegativeImage"] : null; }
      set { Row["NegativeImage"] = CheckValue("NegativeImage", value); }
    }
    
    public string RedirectURL
    {
      get { return Row["RedirectURL"] != DBNull.Value ? (string)Row["RedirectURL"] : null; }
      set { Row["RedirectURL"] = CheckValue("RedirectURL", value); }
    }
    
    public string ExternalPageLink
    {
      get { return Row["ExternalPageLink"] != DBNull.Value ? (string)Row["ExternalPageLink"] : null; }
      set { Row["ExternalPageLink"] = CheckValue("ExternalPageLink", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class AgentRatingsOptions : BaseCollection, IEnumerable<AgentRatingsOption>
  {
    public AgentRatingsOptions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AgentRatingsOptions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public AgentRatingsOption this[int index]
    {
      get { return new AgentRatingsOption(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AgentRatingsOption agentRatingsOption);
    partial void AfterRowInsert(AgentRatingsOption agentRatingsOption);
    partial void BeforeRowEdit(AgentRatingsOption agentRatingsOption);
    partial void AfterRowEdit(AgentRatingsOption agentRatingsOption);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public AgentRatingsOptionProxy[] GetAgentRatingsOptionProxies()
    {
      List<AgentRatingsOptionProxy> list = new List<AgentRatingsOptionProxy>();

      foreach (AgentRatingsOption item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationID)
    {
      BeforeDBDelete(organizationID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AgentRatingsOptions] WHERE ([OrganizationID] = @OrganizationID);";
        deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationID"].Value = organizationID;

        BeforeRowDelete(organizationID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(organizationID);
      }
      AfterDBDelete(organizationID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AgentRatingsOptionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[AgentRatingsOptions] SET     [PositiveRatingText] = @PositiveRatingText,    [NeutralRatingText] = @NeutralRatingText,    [NegativeRatingText] = @NegativeRatingText,    [PositiveImage] = @PositiveImage,    [NeutralImage] = @NeutralImage,    [NegativeImage] = @NegativeImage,    [RedirectURL] = @RedirectURL,    [ExternalPageLink] = @ExternalPageLink  WHERE ([OrganizationID] = @OrganizationID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("PositiveRatingText", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeutralRatingText", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NegativeRatingText", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PositiveImage", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeutralImage", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NegativeImage", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RedirectURL", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExternalPageLink", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[AgentRatingsOptions] (    [OrganizationID],    [PositiveRatingText],    [NeutralRatingText],    [NegativeRatingText],    [PositiveImage],    [NeutralImage],    [NegativeImage],    [RedirectURL],    [ExternalPageLink]) VALUES ( @OrganizationID, @PositiveRatingText, @NeutralRatingText, @NegativeRatingText, @PositiveImage, @NeutralImage, @NegativeImage, @RedirectURL, @ExternalPageLink); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ExternalPageLink", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RedirectURL", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NegativeImage", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeutralImage", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PositiveImage", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NegativeRatingText", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeutralRatingText", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PositiveRatingText", SqlDbType.VarChar, -1);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AgentRatingsOptions] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (AgentRatingsOption agentRatingsOption in this)
		  {
			if (agentRatingsOption.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(agentRatingsOption);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = agentRatingsOption.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				agentRatingsOption.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(agentRatingsOption);
			}
			else if (agentRatingsOption.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(agentRatingsOption);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = agentRatingsOption.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(agentRatingsOption);
			}
			else if (agentRatingsOption.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)agentRatingsOption.Row["OrganizationID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationID"].Value = id;
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

      foreach (AgentRatingsOption agentRatingsOption in this)
      {
        if (agentRatingsOption.Row.Table.Columns.Contains("CreatorID") && (int)agentRatingsOption.Row["CreatorID"] == 0) agentRatingsOption.Row["CreatorID"] = LoginUser.UserID;
        if (agentRatingsOption.Row.Table.Columns.Contains("ModifierID")) agentRatingsOption.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AgentRatingsOption FindByOrganizationID(int organizationID)
    {
      foreach (AgentRatingsOption agentRatingsOption in this)
      {
        if (agentRatingsOption.OrganizationID == organizationID)
        {
          return agentRatingsOption;
        }
      }
      return null;
    }

    public virtual AgentRatingsOption AddNewAgentRatingsOption()
    {
      if (Table.Columns.Count < 1) LoadColumns("AgentRatingsOptions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AgentRatingsOption(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [PositiveRatingText], [NeutralRatingText], [NegativeRatingText], [PositiveImage], [NeutralImage], [NegativeImage], [RedirectURL], [ExternalPageLink] FROM [dbo].[AgentRatingsOptions] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static AgentRatingsOption GetAgentRatingsOption(LoginUser loginUser, int organizationID)
    {
      AgentRatingsOptions agentRatingsOptions = new AgentRatingsOptions(loginUser);
      agentRatingsOptions.LoadByOrganizationID(organizationID);
      if (agentRatingsOptions.IsEmpty)
        return null;
      else
        return agentRatingsOptions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AgentRatingsOption> Members

    public IEnumerator<AgentRatingsOption> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AgentRatingsOption(row, this);
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
