using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportTable : BaseItem
  {
    private ReportTables _reportTables;
    
    public ReportTable(DataRow row, ReportTables reportTables): base(row, reportTables)
    {
      _reportTables = reportTables;
    }
	
    #region Properties
    
    public ReportTables Collection
    {
      get { return _reportTables; }
    }
        
    
    
    

    
    public string OrganizationIDFieldName
    {
      get { return Row["OrganizationIDFieldName"] != DBNull.Value ? (string)Row["OrganizationIDFieldName"] : null; }
      set { Row["OrganizationIDFieldName"] = CheckValue("OrganizationIDFieldName", value); }
    }
    
    public string LookupKeyFieldName
    {
      get { return Row["LookupKeyFieldName"] != DBNull.Value ? (string)Row["LookupKeyFieldName"] : null; }
      set { Row["LookupKeyFieldName"] = CheckValue("LookupKeyFieldName", value); }
    }
    
    public string LookupDisplayClause
    {
      get { return Row["LookupDisplayClause"] != DBNull.Value ? (string)Row["LookupDisplayClause"] : null; }
      set { Row["LookupDisplayClause"] = CheckValue("LookupDisplayClause", value); }
    }
    
    public string LookupOrderBy
    {
      get { return Row["LookupOrderBy"] != DBNull.Value ? (string)Row["LookupOrderBy"] : null; }
      set { Row["LookupOrderBy"] = CheckValue("LookupOrderBy", value); }
    }
    

    
    public bool UseTicketRights
    {
      get { return (bool)Row["UseTicketRights"]; }
      set { Row["UseTicketRights"] = CheckValue("UseTicketRights", value); }
    }
    
    public bool IsCategory
    {
      get { return (bool)Row["IsCategory"]; }
      set { Row["IsCategory"] = CheckValue("IsCategory", value); }
    }
    
    public ReferenceType CustomFieldRefType
    {
      get { return (ReferenceType)Row["CustomFieldRefType"]; }
      set { Row["CustomFieldRefType"] = CheckValue("CustomFieldRefType", value); }
    }
    
    public string Alias
    {
      get { return (string)Row["Alias"]; }
      set { Row["Alias"] = CheckValue("Alias", value); }
    }
    
    public string TableName
    {
      get { return (string)Row["TableName"]; }
      set { Row["TableName"] = CheckValue("TableName", value); }
    }
    
    public int ReportTableID
    {
      get { return (int)Row["ReportTableID"]; }
      set { Row["ReportTableID"] = CheckValue("ReportTableID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ReportTables : BaseCollection, IEnumerable<ReportTable>
  {
    public ReportTables(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportTables"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReportTableID"; }
    }



    public ReportTable this[int index]
    {
      get { return new ReportTable(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportTable reportTable);
    partial void AfterRowInsert(ReportTable reportTable);
    partial void BeforeRowEdit(ReportTable reportTable);
    partial void AfterRowEdit(ReportTable reportTable);
    partial void BeforeRowDelete(int reportTableID);
    partial void AfterRowDelete(int reportTableID);    

    partial void BeforeDBDelete(int reportTableID);
    partial void AfterDBDelete(int reportTableID);    

    #endregion

    #region Public Methods

    public ReportTableProxy[] GetReportTableProxies()
    {
      List<ReportTableProxy> list = new List<ReportTableProxy>();

      foreach (ReportTable item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reportTableID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportTables] WHERE ([ReportTableID] = @ReportTableID);";
        deleteCommand.Parameters.Add("ReportTableID", SqlDbType.Int);
        deleteCommand.Parameters["ReportTableID"].Value = reportTableID;

        BeforeDBDelete(reportTableID);
        BeforeRowDelete(reportTableID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(reportTableID);
        AfterDBDelete(reportTableID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportTablesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportTables] SET     [TableName] = @TableName,    [Alias] = @Alias,    [CustomFieldRefType] = @CustomFieldRefType,    [IsCategory] = @IsCategory,    [OrganizationIDFieldName] = @OrganizationIDFieldName,    [LookupKeyFieldName] = @LookupKeyFieldName,    [LookupDisplayClause] = @LookupDisplayClause,    [LookupOrderBy] = @LookupOrderBy,    [UseTicketRights] = @UseTicketRights  WHERE ([ReportTableID] = @ReportTableID);";

		
		tempParameter = updateCommand.Parameters.Add("ReportTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TableName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Alias", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomFieldRefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsCategory", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationIDFieldName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LookupKeyFieldName", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LookupDisplayClause", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LookupOrderBy", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseTicketRights", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportTables] (    [ReportTableID],    [TableName],    [Alias],    [CustomFieldRefType],    [IsCategory],    [OrganizationIDFieldName],    [LookupKeyFieldName],    [LookupDisplayClause],    [LookupOrderBy],    [UseTicketRights]) VALUES ( @ReportTableID, @TableName, @Alias, @CustomFieldRefType, @IsCategory, @OrganizationIDFieldName, @LookupKeyFieldName, @LookupDisplayClause, @LookupOrderBy, @UseTicketRights); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("UseTicketRights", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LookupOrderBy", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LookupDisplayClause", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LookupKeyFieldName", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationIDFieldName", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsCategory", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomFieldRefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Alias", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TableName", SqlDbType.VarChar, 50);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportTables] WHERE ([ReportTableID] = @ReportTableID);";
		deleteCommand.Parameters.Add("ReportTableID", SqlDbType.Int);

		try
		{
		  foreach (ReportTable reportTable in this)
		  {
			if (reportTable.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportTable);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportTable.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReportTableID"].AutoIncrement = false;
			  Table.Columns["ReportTableID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportTable.Row["ReportTableID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportTable);
			}
			else if (reportTable.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportTable);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportTable.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportTable);
			}
			else if (reportTable.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportTable.Row["ReportTableID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReportTableID"].Value = id;
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

      foreach (ReportTable reportTable in this)
      {
        if (reportTable.Row.Table.Columns.Contains("CreatorID") && (int)reportTable.Row["CreatorID"] == 0) reportTable.Row["CreatorID"] = LoginUser.UserID;
        if (reportTable.Row.Table.Columns.Contains("ModifierID")) reportTable.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportTable FindByReportTableID(int reportTableID)
    {
      foreach (ReportTable reportTable in this)
      {
        if (reportTable.ReportTableID == reportTableID)
        {
          return reportTable;
        }
      }
      return null;
    }

    public virtual ReportTable AddNewReportTable()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportTables");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportTable(row, this);
    }
    
    public virtual void LoadByReportTableID(int reportTableID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReportTableID], [TableName], [Alias], [CustomFieldRefType], [IsCategory], [OrganizationIDFieldName], [LookupKeyFieldName], [LookupDisplayClause], [LookupOrderBy], [UseTicketRights] FROM [dbo].[ReportTables] WHERE ([ReportTableID] = @ReportTableID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportTableID", reportTableID);
        Fill(command);
      }
    }
    
    public static ReportTable GetReportTable(LoginUser loginUser, int reportTableID)
    {
      ReportTables reportTables = new ReportTables(loginUser);
      reportTables.LoadByReportTableID(reportTableID);
      if (reportTables.IsEmpty)
        return null;
      else
        return reportTables[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportTable> Members

    public IEnumerator<ReportTable> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportTable(row, this);
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
