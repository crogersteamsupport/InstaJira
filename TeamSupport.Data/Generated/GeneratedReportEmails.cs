using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportEmail : BaseItem
  {
    private ReportEmails _reportEmails;
    
    public ReportEmail(DataRow row, ReportEmails reportEmails): base(row, reportEmails)
    {
      _reportEmails = reportEmails;
    }
	
    #region Properties
    
    public ReportEmails Collection
    {
      get { return _reportEmails; }
    }
        
    
    
    
    public int ReportEmailID
    {
      get { return (int)Row["ReportEmailID"]; }
    }
    

    

    
    public bool IsWaiting
    {
      get { return (bool)Row["IsWaiting"]; }
      set { Row["IsWaiting"] = CheckValue("IsWaiting", value); }
    }
    
    public int ReportID
    {
      get { return (int)Row["ReportID"]; }
      set { Row["ReportID"] = CheckValue("ReportID", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateSent
    {
      get { return Row["DateSent"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateSent"]) : null; }
      set { Row["DateSent"] = CheckValue("DateSent", value); }
    }

    public DateTime? DateSentUtc
    {
      get { return Row["DateSent"] != DBNull.Value ? (DateTime?)Row["DateSent"] : null; }
    }
    
    public DateTime? DateFailed
    {
      get { return Row["DateFailed"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateFailed"]) : null; }
      set { Row["DateFailed"] = CheckValue("DateFailed", value); }
    }

    public DateTime? DateFailedUtc
    {
      get { return Row["DateFailed"] != DBNull.Value ? (DateTime?)Row["DateFailed"] : null; }
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

  public partial class ReportEmails : BaseCollection, IEnumerable<ReportEmail>
  {
    public ReportEmails(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportEmails"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReportEmailID"; }
    }



    public ReportEmail this[int index]
    {
      get { return new ReportEmail(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportEmail reportEmail);
    partial void AfterRowInsert(ReportEmail reportEmail);
    partial void BeforeRowEdit(ReportEmail reportEmail);
    partial void AfterRowEdit(ReportEmail reportEmail);
    partial void BeforeRowDelete(int reportEmailID);
    partial void AfterRowDelete(int reportEmailID);    

    partial void BeforeDBDelete(int reportEmailID);
    partial void AfterDBDelete(int reportEmailID);    

    #endregion

    #region Public Methods

    public ReportEmailProxy[] GetReportEmailProxies()
    {
      List<ReportEmailProxy> list = new List<ReportEmailProxy>();

      foreach (ReportEmail item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reportEmailID)
    {
      BeforeDBDelete(reportEmailID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportEmails] WHERE ([ReportEmailID] = @ReportEmailID);";
        deleteCommand.Parameters.Add("ReportEmailID", SqlDbType.Int);
        deleteCommand.Parameters["ReportEmailID"].Value = reportEmailID;

        BeforeRowDelete(reportEmailID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(reportEmailID);
      }
      AfterDBDelete(reportEmailID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportEmailsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportEmails] SET     [UserID] = @UserID,    [ReportID] = @ReportID,    [DateSent] = @DateSent,    [DateFailed] = @DateFailed,    [IsWaiting] = @IsWaiting  WHERE ([ReportEmailID] = @ReportEmailID);";

		
		tempParameter = updateCommand.Parameters.Add("ReportEmailID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ReportID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateSent", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateFailed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsWaiting", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportEmails] (    [UserID],    [ReportID],    [DateCreated],    [DateSent],    [DateFailed],    [IsWaiting]) VALUES ( @UserID, @ReportID, @DateCreated, @DateSent, @DateFailed, @IsWaiting); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsWaiting", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateFailed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateSent", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("ReportID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportEmails] WHERE ([ReportEmailID] = @ReportEmailID);";
		deleteCommand.Parameters.Add("ReportEmailID", SqlDbType.Int);

		try
		{
		  foreach (ReportEmail reportEmail in this)
		  {
			if (reportEmail.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportEmail);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportEmail.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReportEmailID"].AutoIncrement = false;
			  Table.Columns["ReportEmailID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportEmail.Row["ReportEmailID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportEmail);
			}
			else if (reportEmail.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportEmail);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportEmail.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportEmail);
			}
			else if (reportEmail.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportEmail.Row["ReportEmailID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReportEmailID"].Value = id;
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

      foreach (ReportEmail reportEmail in this)
      {
        if (reportEmail.Row.Table.Columns.Contains("CreatorID") && (int)reportEmail.Row["CreatorID"] == 0) reportEmail.Row["CreatorID"] = LoginUser.UserID;
        if (reportEmail.Row.Table.Columns.Contains("ModifierID")) reportEmail.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportEmail FindByReportEmailID(int reportEmailID)
    {
      foreach (ReportEmail reportEmail in this)
      {
        if (reportEmail.ReportEmailID == reportEmailID)
        {
          return reportEmail;
        }
      }
      return null;
    }

    public virtual ReportEmail AddNewReportEmail()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportEmails");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportEmail(row, this);
    }
    
    public virtual void LoadByReportEmailID(int reportEmailID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReportEmailID], [UserID], [ReportID], [DateCreated], [DateSent], [DateFailed], [IsWaiting] FROM [dbo].[ReportEmails] WHERE ([ReportEmailID] = @ReportEmailID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportEmailID", reportEmailID);
        Fill(command);
      }
    }
    
    public static ReportEmail GetReportEmail(LoginUser loginUser, int reportEmailID)
    {
      ReportEmails reportEmails = new ReportEmails(loginUser);
      reportEmails.LoadByReportEmailID(reportEmailID);
      if (reportEmails.IsEmpty)
        return null;
      else
        return reportEmails[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportEmail> Members

    public IEnumerator<ReportEmail> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportEmail(row, this);
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
