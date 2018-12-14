using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportSubcategory : BaseItem
  {
    private ReportSubcategories _reportSubcategories;
    
    public ReportSubcategory(DataRow row, ReportSubcategories reportSubcategories): base(row, reportSubcategories)
    {
      _reportSubcategories = reportSubcategories;
    }
	
    #region Properties
    
    public ReportSubcategories Collection
    {
      get { return _reportSubcategories; }
    }
        
    
    
    

    
    public int? ReportTableID
    {
      get { return Row["ReportTableID"] != DBNull.Value ? (int?)Row["ReportTableID"] : null; }
      set { Row["ReportTableID"] = CheckValue("ReportTableID", value); }
    }
    

    
    public string BaseQuery
    {
      get { return (string)Row["BaseQuery"]; }
      set { Row["BaseQuery"] = CheckValue("BaseQuery", value); }
    }
    
    public int ReportCategoryTableID
    {
      get { return (int)Row["ReportCategoryTableID"]; }
      set { Row["ReportCategoryTableID"] = CheckValue("ReportCategoryTableID", value); }
    }
    
    public int ReportSubcategoryID
    {
      get { return (int)Row["ReportSubcategoryID"]; }
      set { Row["ReportSubcategoryID"] = CheckValue("ReportSubcategoryID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ReportSubcategories : BaseCollection, IEnumerable<ReportSubcategory>
  {
    public ReportSubcategories(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportSubcategories"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReportSubcategoryID"; }
    }



    public ReportSubcategory this[int index]
    {
      get { return new ReportSubcategory(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportSubcategory reportSubcategory);
    partial void AfterRowInsert(ReportSubcategory reportSubcategory);
    partial void BeforeRowEdit(ReportSubcategory reportSubcategory);
    partial void AfterRowEdit(ReportSubcategory reportSubcategory);
    partial void BeforeRowDelete(int reportSubcategoryID);
    partial void AfterRowDelete(int reportSubcategoryID);    

    partial void BeforeDBDelete(int reportSubcategoryID);
    partial void AfterDBDelete(int reportSubcategoryID);    

    #endregion

    #region Public Methods

    public ReportSubcategoryProxy[] GetReportSubcategoryProxies()
    {
      List<ReportSubcategoryProxy> list = new List<ReportSubcategoryProxy>();

      foreach (ReportSubcategory item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reportSubcategoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportSubcategories] WHERE ([ReportSubcategoryID] = @ReportSubcategoryID);";
        deleteCommand.Parameters.Add("ReportSubcategoryID", SqlDbType.Int);
        deleteCommand.Parameters["ReportSubcategoryID"].Value = reportSubcategoryID;

        BeforeDBDelete(reportSubcategoryID);
        BeforeRowDelete(reportSubcategoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(reportSubcategoryID);
        AfterDBDelete(reportSubcategoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportSubcategoriesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportSubcategories] SET     [ReportCategoryTableID] = @ReportCategoryTableID,    [ReportTableID] = @ReportTableID,    [BaseQuery] = @BaseQuery  WHERE ([ReportSubcategoryID] = @ReportSubcategoryID);";

		
		tempParameter = updateCommand.Parameters.Add("ReportSubcategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReportCategoryTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReportTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("BaseQuery", SqlDbType.VarChar, 3000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportSubcategories] (    [ReportSubcategoryID],    [ReportCategoryTableID],    [ReportTableID],    [BaseQuery]) VALUES ( @ReportSubcategoryID, @ReportCategoryTableID, @ReportTableID, @BaseQuery); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("BaseQuery", SqlDbType.VarChar, 3000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportCategoryTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportSubcategoryID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportSubcategories] WHERE ([ReportSubcategoryID] = @ReportSubcategoryID);";
		deleteCommand.Parameters.Add("ReportSubcategoryID", SqlDbType.Int);

		try
		{
		  foreach (ReportSubcategory reportSubcategory in this)
		  {
			if (reportSubcategory.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportSubcategory);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportSubcategory.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReportSubcategoryID"].AutoIncrement = false;
			  Table.Columns["ReportSubcategoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportSubcategory.Row["ReportSubcategoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportSubcategory);
			}
			else if (reportSubcategory.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportSubcategory);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportSubcategory.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportSubcategory);
			}
			else if (reportSubcategory.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportSubcategory.Row["ReportSubcategoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReportSubcategoryID"].Value = id;
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

      foreach (ReportSubcategory reportSubcategory in this)
      {
        if (reportSubcategory.Row.Table.Columns.Contains("CreatorID") && (int)reportSubcategory.Row["CreatorID"] == 0) reportSubcategory.Row["CreatorID"] = LoginUser.UserID;
        if (reportSubcategory.Row.Table.Columns.Contains("ModifierID")) reportSubcategory.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportSubcategory FindByReportSubcategoryID(int reportSubcategoryID)
    {
      foreach (ReportSubcategory reportSubcategory in this)
      {
        if (reportSubcategory.ReportSubcategoryID == reportSubcategoryID)
        {
          return reportSubcategory;
        }
      }
      return null;
    }

    public virtual ReportSubcategory AddNewReportSubcategory()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportSubcategories");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportSubcategory(row, this);
    }
    
    public virtual void LoadByReportSubcategoryID(int reportSubcategoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReportSubcategoryID], [ReportCategoryTableID], [ReportTableID], [BaseQuery] FROM [dbo].[ReportSubcategories] WHERE ([ReportSubcategoryID] = @ReportSubcategoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportSubcategoryID", reportSubcategoryID);
        Fill(command);
      }
    }
    
    public static ReportSubcategory GetReportSubcategory(LoginUser loginUser, int reportSubcategoryID)
    {
      ReportSubcategories reportSubcategories = new ReportSubcategories(loginUser);
      reportSubcategories.LoadByReportSubcategoryID(reportSubcategoryID);
      if (reportSubcategories.IsEmpty)
        return null;
      else
        return reportSubcategories[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportSubcategory> Members

    public IEnumerator<ReportSubcategory> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportSubcategory(row, this);
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
