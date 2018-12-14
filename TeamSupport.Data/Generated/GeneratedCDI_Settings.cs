using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CDI_Setting : BaseItem
  {
    private CDI_Settings _cDI_Settings;
    
    public CDI_Setting(DataRow row, CDI_Settings cDI_Settings): base(row, cDI_Settings)
    {
      _cDI_Settings = cDI_Settings;
    }
	
    #region Properties
    
    public CDI_Settings Collection
    {
      get { return _cDI_Settings; }
    }
        
    public float? TotalTicketsWeight
    {
      get { return Row["TotalTicketsWeight"] != DBNull.Value ? (float?)Row["TotalTicketsWeight"] : null; }
      set { Row["TotalTicketsWeight"] = CheckValue("TotalTicketsWeight", value); }
    }    
    
    public float? AverageActionCountWeight
        {
      get { return Row["AverageActionCountWeight"] != DBNull.Value ? (float?)Row["AverageActionCountWeight"] : null; }
      set { Row["AverageActionCountWeight"] = CheckValue("AverageActionCountWeight", value); }
    }    

    public float? AverageSentimentScoreWeight
        {
      get { return Row["AverageSentimentScoreWeight"] != DBNull.Value ? (float?)Row["AverageSentimentScoreWeight"] : null; }
      set { Row["AverageSentimentScoreWeight"] = CheckValue("AverageSentimentScoreWeight", value); }
    }
    
    public float? AverageSeverityWeight
        {
      get { return Row["AverageSeverityWeight"] != DBNull.Value ? (float?)Row["AverageSeverityWeight"] : null; }
      set { Row["AverageSeverityWeight"] = CheckValue("AverageSeverityWeight", value); }
    }
    
    public float? AgentRatingsWeight
        {
      get { return Row["AgentRatingsWeight"] != DBNull.Value ? (float?)Row["AgentRatingsWeight"] : null; }
      set { Row["AgentRatingsWeight"] = CheckValue("AgentRatingsWeight", value); }
    }
    
    public float? OpenTicketsWeight
    {
      get { return Row["OpenTicketsWeight"] != DBNull.Value ? (float?)Row["OpenTicketsWeight"] : null; }
      set { Row["OpenTicketsWeight"] = CheckValue("OpenTicketsWeight", value); }
    }
    
    public float? Last30Weight
    {
      get { return Row["Last30Weight"] != DBNull.Value ? (float?)Row["Last30Weight"] : null; }
      set { Row["Last30Weight"] = CheckValue("Last30Weight", value); }
    }
    
    public float? AvgDaysOpenWeight
    {
      get { return Row["AvgDaysOpenWeight"] != DBNull.Value ? (float?)Row["AvgDaysOpenWeight"] : null; }
      set { Row["AvgDaysOpenWeight"] = CheckValue("AvgDaysOpenWeight", value); }
    }

    public float? AvgDaysToCloseWeight
    {
        get { return Row["AvgDaysToCloseWeight"] != DBNull.Value ? (float?)Row["AvgDaysToCloseWeight"] : null; }
        set { Row["AvgDaysToCloseWeight"] = CheckValue("AvgDaysToCloseWeight", value); }
    }

    public int? GreenUpperRange
    {
      get { return Row["GreenUpperRange"] != DBNull.Value ? (int?)Row["GreenUpperRange"] : null; }
      set { Row["GreenUpperRange"] = CheckValue("GreenUpperRange", value); }
    }
    
    public int? YellowUpperRange
    {
      get { return Row["YellowUpperRange"] != DBNull.Value ? (int?)Row["YellowUpperRange"] : null; }
      set { Row["YellowUpperRange"] = CheckValue("YellowUpperRange", value); }
    }
    
    public bool? NeedCompute
    {
      get { return Row["NeedCompute"] != DBNull.Value ? (bool?)Row["NeedCompute"] : null; }
      set { Row["NeedCompute"] = CheckValue("NeedCompute", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LastCompute
    {
      get { return Row["LastCompute"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastCompute"]) : null; }
      set { Row["LastCompute"] = CheckValue("LastCompute", value); }
    }

    public DateTime? LastComputeUtc
    {
      get { return Row["LastCompute"] != DBNull.Value ? (DateTime?)Row["LastCompute"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class CDI_Settings : BaseCollection, IEnumerable<CDI_Setting>
  {
    public CDI_Settings(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CDI_Settings"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationID"; }
    }



    public CDI_Setting this[int index]
    {
      get { return new CDI_Setting(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CDI_Setting cDI_Setting);
    partial void AfterRowInsert(CDI_Setting cDI_Setting);
    partial void BeforeRowEdit(CDI_Setting cDI_Setting);
    partial void AfterRowEdit(CDI_Setting cDI_Setting);
    partial void BeforeRowDelete(int organizationID);
    partial void AfterRowDelete(int organizationID);    

    partial void BeforeDBDelete(int organizationID);
    partial void AfterDBDelete(int organizationID);    

    #endregion

    #region Public Methods

    public CDI_SettingProxy[] GetCDI_SettingProxies()
    {
      List<CDI_SettingProxy> list = new List<CDI_SettingProxy>();

      foreach (CDI_Setting item in this)
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
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CDI_Settings] WHERE ([OrganizationID] = @OrganizationID);";
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
		//SqlTransaction transaction = connection.BeginTransaction("CDI_SettingsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CDI_Settings] SET     [TotalTicketsWeight] = @TotalTicketsWeight,    [OpenTicketsWeight] = @OpenTicketsWeight,    [Last30Weight] = @Last30Weight,    [AvgDaysOpenWeight] = @AvgDaysOpenWeight,    [AvgDaysToCloseWeight] = @AvgDaysToCloseWeight,    [GreenUpperRange] = @GreenUpperRange,    [YellowUpperRange] = @YellowUpperRange,    [LastCompute] = @LastCompute,    [NeedCompute] = @NeedCompute, [AverageActionCountWeight] = @AverageActionCountWeight, [AverageSentimentScoreWeight]=@AverageSentimentScoreWeight, [AverageSeverityWeight]=@AverageSeverityWeight, [AgentRatingsWeight]=@AgentRatingsWeight WHERE ([OrganizationID] = @OrganizationID);";
		
        tempParameter = updateCommand.Parameters.Add("AverageActionCountWeight", SqlDbType.Real, 4);
        if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}

		tempParameter = updateCommand.Parameters.Add("AverageSentimentScoreWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}		

		tempParameter = updateCommand.Parameters.Add("AverageSeverityWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}

		tempParameter = updateCommand.Parameters.Add("AgentRatingsWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}

		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TotalTicketsWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = updateCommand.Parameters.Add("OpenTicketsWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = updateCommand.Parameters.Add("Last30Weight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = updateCommand.Parameters.Add("AvgDaysOpenWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = updateCommand.Parameters.Add("AvgDaysToCloseWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = updateCommand.Parameters.Add("GreenUpperRange", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("YellowUpperRange", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastCompute", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedCompute", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CDI_Settings] (    [OrganizationID],    [TotalTicketsWeight],    [OpenTicketsWeight],    [Last30Weight],    [AvgDaysOpenWeight],    [AvgDaysToCloseWeight],    [GreenUpperRange],    [YellowUpperRange],    [LastCompute],    [NeedCompute], [AverageActionCountWeight], [AverageSentimentScoreWeight], [AverageSeverityWeight], [AgentRatingsWeight]) VALUES ( @OrganizationID, @TotalTicketsWeight, @OpenTicketsWeight, @Last30Weight, @AvgDaysOpenWeight, @AvgDaysToCloseWeight, @GreenUpperRange, @YellowUpperRange, @LastCompute, @NeedCompute, @AverageActionCountWeight, @AverageSentimentScoreWeight, @AverageSeverityWeight, @AgentRatingsWeight); SET @Identity = SCOPE_IDENTITY();";

        tempParameter = insertCommand.Parameters.Add("AverageActionCountWeight", SqlDbType.Real, 4);
        if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}

		tempParameter = insertCommand.Parameters.Add("AverageSentimentScoreWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}		

		tempParameter = insertCommand.Parameters.Add("AverageSeverityWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = insertCommand.Parameters.Add("AgentRatingsWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedCompute", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastCompute", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("YellowUpperRange", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("GreenUpperRange", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("AvgDaysToCloseWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = insertCommand.Parameters.Add("AvgDaysOpenWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = insertCommand.Parameters.Add("Last30Weight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = insertCommand.Parameters.Add("OpenTicketsWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
		}
		
		tempParameter = insertCommand.Parameters.Add("TotalTicketsWeight", SqlDbType.Real, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 7;
		  tempParameter.Scale = 7;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CDI_Settings] WHERE ([OrganizationID] = @OrganizationID);";
		deleteCommand.Parameters.Add("OrganizationID", SqlDbType.Int);

		try
		{
		  foreach (CDI_Setting cDI_Setting in this)
		  {
			if (cDI_Setting.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(cDI_Setting);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = cDI_Setting.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationID"].AutoIncrement = false;
			  Table.Columns["OrganizationID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				cDI_Setting.Row["OrganizationID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(cDI_Setting);
			}
			else if (cDI_Setting.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(cDI_Setting);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = cDI_Setting.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(cDI_Setting);
			}
			else if (cDI_Setting.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)cDI_Setting.Row["OrganizationID", DataRowVersion.Original];
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

      foreach (CDI_Setting cDI_Setting in this)
      {
        if (cDI_Setting.Row.Table.Columns.Contains("CreatorID") && (int)cDI_Setting.Row["CreatorID"] == 0) cDI_Setting.Row["CreatorID"] = LoginUser.UserID;
        if (cDI_Setting.Row.Table.Columns.Contains("ModifierID")) cDI_Setting.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CDI_Setting FindByOrganizationID(int organizationID)
    {
      foreach (CDI_Setting cDI_Setting in this)
      {
        if (cDI_Setting.OrganizationID == organizationID)
        {
          return cDI_Setting;
        }
      }
      return null;
    }

    public virtual CDI_Setting AddNewCDI_Setting()
    {
      if (Table.Columns.Count < 1) LoadColumns("CDI_Settings");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CDI_Setting(row, this);
    }
    
    public virtual void LoadByOrganizationID(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationID], [TotalTicketsWeight], [OpenTicketsWeight], [Last30Weight], [AvgDaysOpenWeight], [AvgDaysToCloseWeight], [GreenUpperRange], [YellowUpperRange], [LastCompute], [NeedCompute], [AverageActionCountWeight], [AverageSentimentScoreWeight], [AverageSeverityWeight], [AgentRatingsWeight] FROM [dbo].[CDI_Settings] WHERE ([OrganizationID] = @OrganizationID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }
    
    public static CDI_Setting GetCDI_Setting(LoginUser loginUser, int organizationID)
    {
      CDI_Settings cDI_Settings = new CDI_Settings(loginUser);
      cDI_Settings.LoadByOrganizationID(organizationID);
      if (cDI_Settings.IsEmpty)
        return null;
      else
        return cDI_Settings[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CDI_Setting> Members

    public IEnumerator<CDI_Setting> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CDI_Setting(row, this);
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
