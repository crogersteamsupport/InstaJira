using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ReportTableField : BaseItem
  {
    private ReportTableFields _reportTableFields;
    
    public ReportTableField(DataRow row, ReportTableFields reportTableFields): base(row, reportTableFields)
    {
      _reportTableFields = reportTableFields;
    }
	
    #region Properties
    
    public ReportTableFields Collection
    {
      get { return _reportTableFields; }
    }
        
    
    
    
    public int ReportTableFieldID
    {
      get { return (int)Row["ReportTableFieldID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int? LookupTableID
    {
      get { return Row["LookupTableID"] != DBNull.Value ? (int?)Row["LookupTableID"] : null; }
      set { Row["LookupTableID"] = CheckValue("LookupTableID", value); }
    }
    

    
    public bool IsSortable
    {
      get { return (bool)Row["IsSortable"]; }
      set { Row["IsSortable"] = CheckValue("IsSortable", value); }
    }
    
    public bool IsLink
    {
      get { return (bool)Row["IsLink"]; }
      set { Row["IsLink"] = CheckValue("IsLink", value); }
    }
    
    public bool IsEmail
    {
      get { return (bool)Row["IsEmail"]; }
      set { Row["IsEmail"] = CheckValue("IsEmail", value); }
    }
    
    public bool IsOpenable
    {
      get { return (bool)Row["IsOpenable"]; }
      set { Row["IsOpenable"] = CheckValue("IsOpenable", value); }
    }
    
    public bool IsReadOnly
    {
      get { return (bool)Row["IsReadOnly"]; }
      set { Row["IsReadOnly"] = CheckValue("IsReadOnly", value); }
    }
    
    public bool IsVisible
    {
      get { return (bool)Row["IsVisible"]; }
      set { Row["IsVisible"] = CheckValue("IsVisible", value); }
    }
    
    public int Size
    {
      get { return (int)Row["Size"]; }
      set { Row["Size"] = CheckValue("Size", value); }
    }
    
    public string DataType
    {
      get { return (string)Row["DataType"]; }
      set { Row["DataType"] = CheckValue("DataType", value); }
    }
    
    public string Alias
    {
      get { return (string)Row["Alias"]; }
      set { Row["Alias"] = CheckValue("Alias", value); }
    }
    
    public string FieldName
    {
      get { return (string)Row["FieldName"]; }
      set { Row["FieldName"] = CheckValue("FieldName", value); }
    }
    
    public int ReportTableID
    {
      get { return (int)Row["ReportTableID"]; }
      set { Row["ReportTableID"] = CheckValue("ReportTableID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ReportTableFields : BaseCollection, IEnumerable<ReportTableField>
  {
    public ReportTableFields(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ReportTableFields"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReportTableFieldID"; }
    }



    public ReportTableField this[int index]
    {
      get { return new ReportTableField(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ReportTableField reportTableField);
    partial void AfterRowInsert(ReportTableField reportTableField);
    partial void BeforeRowEdit(ReportTableField reportTableField);
    partial void AfterRowEdit(ReportTableField reportTableField);
    partial void BeforeRowDelete(int reportTableFieldID);
    partial void AfterRowDelete(int reportTableFieldID);    

    partial void BeforeDBDelete(int reportTableFieldID);
    partial void AfterDBDelete(int reportTableFieldID);    

    #endregion

    #region Public Methods

    public ReportTableFieldProxy[] GetReportTableFieldProxies()
    {
      List<ReportTableFieldProxy> list = new List<ReportTableFieldProxy>();

      foreach (ReportTableField item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reportTableFieldID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportTableFields] WHERE ([ReportTableFieldID] = @ReportTableFieldID);";
        deleteCommand.Parameters.Add("ReportTableFieldID", SqlDbType.Int);
        deleteCommand.Parameters["ReportTableFieldID"].Value = reportTableFieldID;

        BeforeDBDelete(reportTableFieldID);
        BeforeRowDelete(reportTableFieldID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(reportTableFieldID);
        AfterDBDelete(reportTableFieldID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ReportTableFieldsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ReportTableFields] SET     [ReportTableID] = @ReportTableID,    [FieldName] = @FieldName,    [Alias] = @Alias,    [DataType] = @DataType,    [Size] = @Size,    [IsVisible] = @IsVisible,    [Description] = @Description,    [LookupTableID] = @LookupTableID,    [IsReadOnly] = @IsReadOnly,    [IsOpenable] = @IsOpenable,    [IsEmail] = @IsEmail,    [IsLink] = @IsLink,    [IsSortable] = @IsSortable  WHERE ([ReportTableFieldID] = @ReportTableFieldID);";

		
		tempParameter = updateCommand.Parameters.Add("ReportTableFieldID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("FieldName", SqlDbType.VarChar, 1000);
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
		
		tempParameter = updateCommand.Parameters.Add("DataType", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Size", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisible", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LookupTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsReadOnly", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsOpenable", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsLink", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsSortable", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ReportTableFields] (    [ReportTableID],    [FieldName],    [Alias],    [DataType],    [Size],    [IsVisible],    [Description],    [LookupTableID],    [IsReadOnly],    [IsOpenable],    [IsEmail],    [IsLink],    [IsSortable]) VALUES ( @ReportTableID, @FieldName, @Alias, @DataType, @Size, @IsVisible, @Description, @LookupTableID, @IsReadOnly, @IsOpenable, @IsEmail, @IsLink, @IsSortable); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsSortable", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsLink", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsOpenable", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsReadOnly", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LookupTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisible", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Size", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DataType", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Alias", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FieldName", SqlDbType.VarChar, 1000);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ReportTableFields] WHERE ([ReportTableFieldID] = @ReportTableFieldID);";
		deleteCommand.Parameters.Add("ReportTableFieldID", SqlDbType.Int);

		try
		{
		  foreach (ReportTableField reportTableField in this)
		  {
			if (reportTableField.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reportTableField);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reportTableField.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReportTableFieldID"].AutoIncrement = false;
			  Table.Columns["ReportTableFieldID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reportTableField.Row["ReportTableFieldID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reportTableField);
			}
			else if (reportTableField.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reportTableField);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reportTableField.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reportTableField);
			}
			else if (reportTableField.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reportTableField.Row["ReportTableFieldID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReportTableFieldID"].Value = id;
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

      foreach (ReportTableField reportTableField in this)
      {
        if (reportTableField.Row.Table.Columns.Contains("CreatorID") && (int)reportTableField.Row["CreatorID"] == 0) reportTableField.Row["CreatorID"] = LoginUser.UserID;
        if (reportTableField.Row.Table.Columns.Contains("ModifierID")) reportTableField.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ReportTableField FindByReportTableFieldID(int reportTableFieldID)
    {
      foreach (ReportTableField reportTableField in this)
      {
        if (reportTableField.ReportTableFieldID == reportTableFieldID)
        {
          return reportTableField;
        }
      }
      return null;
    }

    public virtual ReportTableField AddNewReportTableField()
    {
      if (Table.Columns.Count < 1) LoadColumns("ReportTableFields");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ReportTableField(row, this);
    }
    
    public virtual void LoadByReportTableFieldID(int reportTableFieldID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReportTableFieldID], [ReportTableID], [FieldName], [Alias], [DataType], [Size], [IsVisible], [Description], [LookupTableID], [IsReadOnly], [IsOpenable], [IsEmail], [IsLink], [IsSortable] FROM [dbo].[ReportTableFields] WHERE ([ReportTableFieldID] = @ReportTableFieldID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReportTableFieldID", reportTableFieldID);
        Fill(command);
      }
    }
    
    public static ReportTableField GetReportTableField(LoginUser loginUser, int reportTableFieldID)
    {
      ReportTableFields reportTableFields = new ReportTableFields(loginUser);
      reportTableFields.LoadByReportTableFieldID(reportTableFieldID);
      if (reportTableFields.IsEmpty)
        return null;
      else
        return reportTableFields[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ReportTableField> Members

    public IEnumerator<ReportTableField> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ReportTableField(row, this);
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
