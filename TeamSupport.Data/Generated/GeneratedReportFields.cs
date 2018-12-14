using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportField : BaseItem
  {
    private ReportFields _reportFields;
    
    public ReportField(DataRow row, ReportFields reportFields): base(row, reportFields)
    {
      _reportFields = reportFields;
    }
	
    #region Properties
    
    public ReportFields Collection
    {
      get { return _reportFields; }
    }
        
    
    
    
    public int ReportFieldID
    {
      get { return (int)Row["ReportFieldID"]; }
    }
    

    

    
    public bool IsCustomField
    {
      get { return (bool)Row["IsCustomField"]; }
      set { Row["IsCustomField"] = CheckValue("IsCustomField", value); }
    }
    
    public int LinkedFieldID
    {
      get { return (int)Row["LinkedFieldID"]; }
      set { Row["LinkedFieldID"] = CheckValue("LinkedFieldID", value); }
    }
    
    public int ReportID
    {
      get { return (int)Row["ReportID"]; }
      set { Row["ReportID"] = CheckValue("ReportID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ReportFields : BaseCollection, IEnumerable<ReportField>
  {
    public ReportFields(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportFields"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReportFieldID"; }
    }



    public ReportField this[int index]
    {
      get { return new ReportField(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportField reportField);
    partial void AfterRowInsert(ReportField reportField);
    partial void BeforeRowEdit(ReportField reportField);
    partial void AfterRowEdit(ReportField reportField);
    partial void BeforeRowDelete(int reportFieldID);
    partial void AfterRowDelete(int reportFieldID);    

    partial void BeforeDBDelete(int reportFieldID);
    partial void AfterDBDelete(int reportFieldID);    

    #endregion

    #region Public Methods

    public ReportFieldProxy[] GetReportFieldProxies()
    {
      List<ReportFieldProxy> list = new List<ReportFieldProxy>();

      foreach (ReportField item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reportFieldID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportFields] WHERE ([ReportFieldID] = @ReportFieldID);";
        deleteCommand.Parameters.Add("ReportFieldID", SqlDbType.Int);
        deleteCommand.Parameters["ReportFieldID"].Value = reportFieldID;

        BeforeDBDelete(reportFieldID);
        BeforeRowDelete(reportFieldID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(reportFieldID);
        AfterDBDelete(reportFieldID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportFieldsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportFields] SET     [ReportID] = @ReportID,    [LinkedFieldID] = @LinkedFieldID,    [IsCustomField] = @IsCustomField  WHERE ([ReportFieldID] = @ReportFieldID);";

		
		tempParameter = updateCommand.Parameters.Add("ReportFieldID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("LinkedFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsCustomField", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportFields] (    [ReportID],    [LinkedFieldID],    [IsCustomField]) VALUES ( @ReportID, @LinkedFieldID, @IsCustomField); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsCustomField", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LinkedFieldID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportFields] WHERE ([ReportFieldID] = @ReportFieldID);";
		deleteCommand.Parameters.Add("ReportFieldID", SqlDbType.Int);

		try
		{
		  foreach (ReportField reportField in this)
		  {
			if (reportField.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportField);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportField.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReportFieldID"].AutoIncrement = false;
			  Table.Columns["ReportFieldID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportField.Row["ReportFieldID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportField);
			}
			else if (reportField.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportField);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportField.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportField);
			}
			else if (reportField.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportField.Row["ReportFieldID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReportFieldID"].Value = id;
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

      foreach (ReportField reportField in this)
      {
        if (reportField.Row.Table.Columns.Contains("CreatorID") && (int)reportField.Row["CreatorID"] == 0) reportField.Row["CreatorID"] = LoginUser.UserID;
        if (reportField.Row.Table.Columns.Contains("ModifierID")) reportField.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportField FindByReportFieldID(int reportFieldID)
    {
      foreach (ReportField reportField in this)
      {
        if (reportField.ReportFieldID == reportFieldID)
        {
          return reportField;
        }
      }
      return null;
    }

    public virtual ReportField AddNewReportField()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportFields");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportField(row, this);
    }
    
    public virtual void LoadByReportFieldID(int reportFieldID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReportFieldID], [ReportID], [LinkedFieldID], [IsCustomField] FROM [dbo].[ReportFields] WHERE ([ReportFieldID] = @ReportFieldID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportFieldID", reportFieldID);
        Fill(command);
      }
    }
    
    public static ReportField GetReportField(LoginUser loginUser, int reportFieldID)
    {
      ReportFields reportFields = new ReportFields(loginUser);
      reportFields.LoadByReportFieldID(reportFieldID);
      if (reportFields.IsEmpty)
        return null;
      else
        return reportFields[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportField> Members

    public IEnumerator<ReportField> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportField(row, this);
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
