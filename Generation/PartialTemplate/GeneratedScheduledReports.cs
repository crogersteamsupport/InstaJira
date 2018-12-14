using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ScheduledReport : BaseItem
  {
    private ScheduledReports _scheduledReports;
    
    public ScheduledReport(DataRow row, ScheduledReports scheduledReports): base(row, scheduledReports)
    {
      _scheduledReports = scheduledReports;
    }
	
    #region Properties
    
    public ScheduledReports Collection
    {
      get { return _scheduledReports; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    
    public short? RunCount
    {
      get { return Row["RunCount"] != DBNull.Value ? (short?)Row["RunCount"] : null; }
      set { Row["RunCount"] = CheckValue("RunCount", value); }
    }
    
    public byte? Every
    {
      get { return Row["Every"] != DBNull.Value ? (byte?)Row["Every"] : null; }
      set { Row["Every"] = CheckValue("Every", value); }
    }
    
    public byte? Weekday
    {
      get { return Row["Weekday"] != DBNull.Value ? (byte?)Row["Weekday"] : null; }
      set { Row["Weekday"] = CheckValue("Weekday", value); }
    }
    
    public byte? Monthday
    {
      get { return Row["Monthday"] != DBNull.Value ? (byte?)Row["Monthday"] : null; }
      set { Row["Monthday"] = CheckValue("Monthday", value); }
    }
    
    public bool? IsSuccessful
    {
      get { return Row["IsSuccessful"] != DBNull.Value ? (bool?)Row["IsSuccessful"] : null; }
      set { Row["IsSuccessful"] = CheckValue("IsSuccessful", value); }
    }
    
    public int? ModifierId
    {
      get { return Row["ModifierId"] != DBNull.Value ? (int?)Row["ModifierId"] : null; }
      set { Row["ModifierId"] = CheckValue("ModifierId", value); }
    }
    
    public int? LockProcessId
    {
      get { return Row["LockProcessId"] != DBNull.Value ? (int?)Row["LockProcessId"] : null; }
      set { Row["LockProcessId"] = CheckValue("LockProcessId", value); }
    }
    

    
    public int CreatorId
    {
      get { return (int)Row["CreatorId"]; }
      set { Row["CreatorId"] = CheckValue("CreatorId", value); }
    }
    
    public byte RecurrencyId
    {
      get { return (byte)Row["RecurrencyId"]; }
      set { Row["RecurrencyId"] = CheckValue("RecurrencyId", value); }
    }
    
    public bool IsActive
    {
      get { return (bool)Row["IsActive"]; }
      set { Row["IsActive"] = CheckValue("IsActive", value); }
    }
    
    public int OrganizationId
    {
      get { return (int)Row["OrganizationId"]; }
      set { Row["OrganizationId"] = CheckValue("OrganizationId", value); }
    }
    
    public int ReportId
    {
      get { return (int)Row["ReportId"]; }
      set { Row["ReportId"] = CheckValue("ReportId", value); }
    }
    
    public string EmailRecipients
    {
      get { return (string)Row["EmailRecipients"]; }
      set { Row["EmailRecipients"] = CheckValue("EmailRecipients", value); }
    }
    
    public string EmailBody
    {
      get { return (string)Row["EmailBody"]; }
      set { Row["EmailBody"] = CheckValue("EmailBody", value); }
    }
    
    public string EmailSubject
    {
      get { return (string)Row["EmailSubject"]; }
      set { Row["EmailSubject"] = CheckValue("EmailSubject", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LastRun
    {
      get { return Row["LastRun"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastRun"]) : null; }
      set { Row["LastRun"] = CheckValue("LastRun", value); }
    }

    public DateTime? LastRunUtc
    {
      get { return Row["LastRun"] != DBNull.Value ? (DateTime?)Row["LastRun"] : null; }
    }
    
    public DateTime? NextRun
    {
      get { return Row["NextRun"] != DBNull.Value ? DateToLocal((DateTime?)Row["NextRun"]) : null; }
      set { Row["NextRun"] = CheckValue("NextRun", value); }
    }

    public DateTime? NextRunUtc
    {
      get { return Row["NextRun"] != DBNull.Value ? (DateTime?)Row["NextRun"] : null; }
    }
    
    public DateTime? DateModified
    {
      get { return Row["DateModified"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModified"]) : null; }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime? DateModifiedUtc
    {
      get { return Row["DateModified"] != DBNull.Value ? (DateTime?)Row["DateModified"] : null; }
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
    
    public DateTime StartDate
    {
      get { return DateToLocal((DateTime)Row["StartDate"]); }
      set { Row["StartDate"] = CheckValue("StartDate", value); }
    }

    public DateTime StartDateUtc
    {
      get { return (DateTime)Row["StartDate"]; }
    }
    

    #endregion
    
    
  }

  public partial class ScheduledReports : BaseCollection, IEnumerable<ScheduledReport>
  {
    public ScheduledReports(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ScheduledReports"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public ScheduledReport this[int index]
    {
      get { return new ScheduledReport(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ScheduledReport scheduledReport);
    partial void AfterRowInsert(ScheduledReport scheduledReport);
    partial void BeforeRowEdit(ScheduledReport scheduledReport);
    partial void AfterRowEdit(ScheduledReport scheduledReport);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public ScheduledReportProxy[] GetScheduledReportProxies()
    {
      List<ScheduledReportProxy> list = new List<ScheduledReportProxy>();

      foreach (ScheduledReport item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ScheduledReports] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ScheduledReportsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ScheduledReports] SET     [EmailSubject] = @EmailSubject,    [EmailBody] = @EmailBody,    [EmailRecipients] = @EmailRecipients,    [ReportId] = @ReportId,    [OrganizationId] = @OrganizationId,    [RunCount] = @RunCount,    [IsActive] = @IsActive,    [StartDate] = @StartDate,    [RecurrencyId] = @RecurrencyId,    [Every] = @Every,    [Weekday] = @Weekday,    [Monthday] = @Monthday,    [LastRun] = @LastRun,    [IsSuccessful] = @IsSuccessful,    [NextRun] = @NextRun,    [CreatorId] = @CreatorId,    [ModifierId] = @ModifierId,    [DateModified] = @DateModified,    [LockProcessId] = @LockProcessId  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailSubject", SqlDbType.NVarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailBody", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailRecipients", SqlDbType.NVarChar, 2000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReportId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RunCount", SqlDbType.SmallInt, 2);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 5;
		  tempParameter.Scale = 5;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("StartDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("RecurrencyId", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = updateCommand.Parameters.Add("Every", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = updateCommand.Parameters.Add("Weekday", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = updateCommand.Parameters.Add("Monthday", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastRun", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsSuccessful", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("NextRun", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatorId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierId", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("LockProcessId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ScheduledReports] (    [EmailSubject],    [EmailBody],    [EmailRecipients],    [ReportId],    [OrganizationId],    [RunCount],    [IsActive],    [StartDate],    [RecurrencyId],    [Every],    [Weekday],    [Monthday],    [LastRun],    [IsSuccessful],    [NextRun],    [CreatorId],    [ModifierId],    [DateCreated],    [DateModified],    [LockProcessId]) VALUES ( @EmailSubject, @EmailBody, @EmailRecipients, @ReportId, @OrganizationId, @RunCount, @IsActive, @StartDate, @RecurrencyId, @Every, @Weekday, @Monthday, @LastRun, @IsSuccessful, @NextRun, @CreatorId, @ModifierId, @DateCreated, @DateModified, @LockProcessId); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("LockProcessId", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("ModifierId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("NextRun", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsSuccessful", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastRun", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Monthday", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = insertCommand.Parameters.Add("Weekday", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = insertCommand.Parameters.Add("Every", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = insertCommand.Parameters.Add("RecurrencyId", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = insertCommand.Parameters.Add("StartDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsActive", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RunCount", SqlDbType.SmallInt, 2);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 5;
		  tempParameter.Scale = 5;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailRecipients", SqlDbType.NVarChar, 2000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailBody", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailSubject", SqlDbType.NVarChar, 200);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ScheduledReports] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (ScheduledReport scheduledReport in this)
		  {
			if (scheduledReport.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(scheduledReport);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = scheduledReport.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				scheduledReport.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(scheduledReport);
			}
			else if (scheduledReport.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(scheduledReport);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = scheduledReport.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(scheduledReport);
			}
			else if (scheduledReport.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)scheduledReport.Row["Id", DataRowVersion.Original];
			  deleteCommand.Parameters["Id"].Value = id;
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

      foreach (ScheduledReport scheduledReport in this)
      {
        if (scheduledReport.Row.Table.Columns.Contains("CreatorID") && (int)scheduledReport.Row["CreatorID"] == 0) scheduledReport.Row["CreatorID"] = LoginUser.UserID;
        if (scheduledReport.Row.Table.Columns.Contains("ModifierID")) scheduledReport.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ScheduledReport FindById(int id)
    {
      foreach (ScheduledReport scheduledReport in this)
      {
        if (scheduledReport.Id == id)
        {
          return scheduledReport;
        }
      }
      return null;
    }

    public virtual ScheduledReport AddNewScheduledReport()
    {
      if (Table.Columns.Count < 1) LoadColumns("ScheduledReports");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ScheduledReport(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [EmailSubject], [EmailBody], [EmailRecipients], [ReportId], [OrganizationId], [RunCount], [IsActive], [StartDate], [RecurrencyId], [Every], [Weekday], [Monthday], [LastRun], [IsSuccessful], [NextRun], [CreatorId], [ModifierId], [DateCreated], [DateModified], [LockProcessId] FROM [dbo].[ScheduledReports] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static ScheduledReport GetScheduledReport(LoginUser loginUser, int id)
    {
      ScheduledReports scheduledReports = new ScheduledReports(loginUser);
      scheduledReports.LoadById(id);
      if (scheduledReports.IsEmpty)
        return null;
      else
        return scheduledReports[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ScheduledReport> Members

    public IEnumerator<ScheduledReport> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ScheduledReport(row, this);
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
