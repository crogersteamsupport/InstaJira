using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportView : BaseItem
  {
    private ReportViews _reportViews;
    
    public ReportView(DataRow row, ReportViews reportViews): base(row, reportViews)
    {
      _reportViews = reportViews;
    }
	
    #region Properties
    
    public ReportViews Collection
    {
      get { return _reportViews; }
    }
        
    
    
    
    public int ReportViewID
    {
      get { return (int)Row["ReportViewID"]; }
    }
    

    
    public string SQLExecuted
    {
      get { return Row["SQLExecuted"] != DBNull.Value ? (string)Row["SQLExecuted"] : null; }
      set { Row["SQLExecuted"] = CheckValue("SQLExecuted", value); }
    }
    
    public string ErrorMessage
    {
      get { return Row["ErrorMessage"] != DBNull.Value ? (string)Row["ErrorMessage"] : null; }
      set { Row["ErrorMessage"] = CheckValue("ErrorMessage", value); }
    }
    

    
    public double DurationToLoad
    {
      get { return (double)Row["DurationToLoad"]; }
      set { Row["DurationToLoad"] = CheckValue("DurationToLoad", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int ReportID
    {
      get { return (int)Row["ReportID"]; }
      set { Row["ReportID"] = CheckValue("ReportID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateViewed
    {
      get { return DateToLocal((DateTime)Row["DateViewed"]); }
      set { Row["DateViewed"] = CheckValue("DateViewed", value); }
    }

    public DateTime DateViewedUtc
    {
      get { return (DateTime)Row["DateViewed"]; }
    }
    

    #endregion
    
    
  }

  public partial class ReportViews : BaseCollection, IEnumerable<ReportView>
  {
    public ReportViews(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportViews"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReportViewID"; }
    }



    public ReportView this[int index]
    {
      get { return new ReportView(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportView reportView);
    partial void AfterRowInsert(ReportView reportView);
    partial void BeforeRowEdit(ReportView reportView);
    partial void AfterRowEdit(ReportView reportView);
    partial void BeforeRowDelete(int reportViewID);
    partial void AfterRowDelete(int reportViewID);    

    partial void BeforeDBDelete(int reportViewID);
    partial void AfterDBDelete(int reportViewID);    

    #endregion

    #region Public Methods

    public ReportViewProxy[] GetReportViewProxies()
    {
      List<ReportViewProxy> list = new List<ReportViewProxy>();

      foreach (ReportView item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reportViewID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportViews] WHERE ([ReportViewID] = @ReportViewID);";
        deleteCommand.Parameters.Add("ReportViewID", SqlDbType.Int);
        deleteCommand.Parameters["ReportViewID"].Value = reportViewID;

        BeforeDBDelete(reportViewID);
        BeforeRowDelete(reportViewID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(reportViewID);
        AfterDBDelete(reportViewID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportViewsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportViews] SET     [ReportID] = @ReportID,    [UserID] = @UserID,    [DateViewed] = @DateViewed,    [DurationToLoad] = @DurationToLoad,    [SQLExecuted] = @SQLExecuted,    [ErrorMessage] = @ErrorMessage  WHERE ([ReportViewID] = @ReportViewID);";

		
		tempParameter = updateCommand.Parameters.Add("ReportViewID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateViewed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DurationToLoad", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = updateCommand.Parameters.Add("SQLExecuted", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ErrorMessage", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportViews] (    [ReportID],    [UserID],    [DateViewed],    [DurationToLoad],    [SQLExecuted],    [ErrorMessage]) VALUES ( @ReportID, @UserID, @DateViewed, @DurationToLoad, @SQLExecuted, @ErrorMessage); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ErrorMessage", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SQLExecuted", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DurationToLoad", SqlDbType.Float, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 15;
		  tempParameter.Scale = 15;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateViewed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportViews] WHERE ([ReportViewID] = @ReportViewID);";
		deleteCommand.Parameters.Add("ReportViewID", SqlDbType.Int);

		try
		{
		  foreach (ReportView reportView in this)
		  {
			if (reportView.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportView);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportView.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReportViewID"].AutoIncrement = false;
			  Table.Columns["ReportViewID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportView.Row["ReportViewID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportView);
			}
			else if (reportView.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportView);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportView.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportView);
			}
			else if (reportView.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportView.Row["ReportViewID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReportViewID"].Value = id;
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

      foreach (ReportView reportView in this)
      {
        if (reportView.Row.Table.Columns.Contains("CreatorID") && (int)reportView.Row["CreatorID"] == 0) reportView.Row["CreatorID"] = LoginUser.UserID;
        if (reportView.Row.Table.Columns.Contains("ModifierID")) reportView.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportView FindByReportViewID(int reportViewID)
    {
      foreach (ReportView reportView in this)
      {
        if (reportView.ReportViewID == reportViewID)
        {
          return reportView;
        }
      }
      return null;
    }

    public virtual ReportView AddNewReportView()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportViews");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportView(row, this);
    }
    
    public virtual void LoadByReportViewID(int reportViewID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReportViewID], [ReportID], [UserID], [DateViewed], [DurationToLoad], [SQLExecuted], [ErrorMessage] FROM [dbo].[ReportViews] WHERE ([ReportViewID] = @ReportViewID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportViewID", reportViewID);
        Fill(command);
      }
    }
    
    public static ReportView GetReportView(LoginUser loginUser, int reportViewID)
    {
      ReportViews reportViews = new ReportViews(loginUser);
      reportViews.LoadByReportViewID(reportViewID);
      if (reportViews.IsEmpty)
        return null;
      else
        return reportViews[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportView> Members

    public IEnumerator<ReportView> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportView(row, this);
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
