using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportDataItem : BaseItem
  {
    private ReportData _reportData;
    
    public ReportDataItem(DataRow row, ReportData reportData): base(row, reportData)
    {
      _reportData = reportData;
    }
	
    #region Properties
    
    public ReportData Collection
    {
      get { return _reportData; }
    }
        
    
    
    
    public int ReportDataID
    {
      get { return (int)Row["ReportDataID"]; }
    }
    

    
    public string ReportData
    {
      get { return Row["ReportData"] != DBNull.Value ? (string)Row["ReportData"] : null; }
      set { Row["ReportData"] = CheckValue("ReportData", value); }
    }
    
    public string QueryObject
    {
      get { return Row["QueryObject"] != DBNull.Value ? (string)Row["QueryObject"] : null; }
      set { Row["QueryObject"] = CheckValue("QueryObject", value); }
    }
    
    public string OrderByClause
    {
      get { return Row["OrderByClause"] != DBNull.Value ? (string)Row["OrderByClause"] : null; }
      set { Row["OrderByClause"] = CheckValue("OrderByClause", value); }
    }
    

    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
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
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
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

  public partial class ReportData : BaseCollection, IEnumerable<ReportDataItem>
  {
    public ReportData(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportData"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReportDataID"; }
    }



    public ReportDataItem this[int index]
    {
      get { return new ReportDataItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportDataItem reportDataItem);
    partial void AfterRowInsert(ReportDataItem reportDataItem);
    partial void BeforeRowEdit(ReportDataItem reportDataItem);
    partial void AfterRowEdit(ReportDataItem reportDataItem);
    partial void BeforeRowDelete(int reportDataID);
    partial void AfterRowDelete(int reportDataID);    

    partial void BeforeDBDelete(int reportDataID);
    partial void AfterDBDelete(int reportDataID);    

    #endregion

    #region Public Methods

    public ReportDataItemProxy[] GetReportDataItemProxies()
    {
      List<ReportDataItemProxy> list = new List<ReportDataItemProxy>();

      foreach (ReportDataItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reportDataID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportData] WHERE ([ReportDataID] = @ReportDataID);";
        deleteCommand.Parameters.Add("ReportDataID", SqlDbType.Int);
        deleteCommand.Parameters["ReportDataID"].Value = reportDataID;

        BeforeDBDelete(reportDataID);
        BeforeRowDelete(reportDataID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(reportDataID);
        AfterDBDelete(reportDataID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportDataSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportData] SET     [UserID] = @UserID,    [ReportID] = @ReportID,    [ReportData] = @ReportData,    [QueryObject] = @QueryObject,    [ModifierID] = @ModifierID,    [DateModified] = @DateModified,    [OrderByClause] = @OrderByClause  WHERE ([ReportDataID] = @ReportDataID);";

		
		tempParameter = updateCommand.Parameters.Add("ReportDataID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ReportData", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("QueryObject", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("OrderByClause", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportData] (    [UserID],    [ReportID],    [ReportData],    [QueryObject],    [CreatorID],    [ModifierID],    [DateCreated],    [DateModified],    [OrderByClause]) VALUES ( @UserID, @ReportID, @ReportData, @QueryObject, @CreatorID, @ModifierID, @DateCreated, @DateModified, @OrderByClause); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OrderByClause", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("QueryObject", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportData", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportData] WHERE ([ReportDataID] = @ReportDataID);";
		deleteCommand.Parameters.Add("ReportDataID", SqlDbType.Int);

		try
		{
		  foreach (ReportDataItem reportDataItem in this)
		  {
			if (reportDataItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportDataItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportDataItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReportDataID"].AutoIncrement = false;
			  Table.Columns["ReportDataID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportDataItem.Row["ReportDataID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportDataItem);
			}
			else if (reportDataItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportDataItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportDataItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportDataItem);
			}
			else if (reportDataItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportDataItem.Row["ReportDataID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReportDataID"].Value = id;
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

      foreach (ReportDataItem reportDataItem in this)
      {
        if (reportDataItem.Row.Table.Columns.Contains("CreatorID") && (int)reportDataItem.Row["CreatorID"] == 0) reportDataItem.Row["CreatorID"] = LoginUser.UserID;
        if (reportDataItem.Row.Table.Columns.Contains("ModifierID")) reportDataItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportDataItem FindByReportDataID(int reportDataID)
    {
      foreach (ReportDataItem reportDataItem in this)
      {
        if (reportDataItem.ReportDataID == reportDataID)
        {
          return reportDataItem;
        }
      }
      return null;
    }

    public virtual ReportDataItem AddNewReportDataItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportData");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportDataItem(row, this);
    }
    
    public virtual void LoadByReportDataID(int reportDataID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReportDataID], [UserID], [ReportID], [ReportData], [QueryObject], [CreatorID], [ModifierID], [DateCreated], [DateModified], [OrderByClause] FROM [dbo].[ReportData] WHERE ([ReportDataID] = @ReportDataID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportDataID", reportDataID);
        Fill(command);
      }
    }
    
    public static ReportDataItem GetReportDataItem(LoginUser loginUser, int reportDataID)
    {
      ReportData reportData = new ReportData(loginUser);
      reportData.LoadByReportDataID(reportDataID);
      if (reportData.IsEmpty)
        return null;
      else
        return reportData[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportDataItem> Members

    public IEnumerator<ReportDataItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportDataItem(row, this);
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
