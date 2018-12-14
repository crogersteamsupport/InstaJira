using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CRMLinkField : BaseItem
  {
    private CRMLinkFields _cRMLinkFields;
    
    public CRMLinkField(DataRow row, CRMLinkFields cRMLinkFields): base(row, cRMLinkFields)
    {
      _cRMLinkFields = cRMLinkFields;
    }
	
    #region Properties
    
    public CRMLinkFields Collection
    {
      get { return _cRMLinkFields; }
    }
        
    
    
    
    public int CRMFieldID
    {
      get { return (int)Row["CRMFieldID"]; }
    }
    

    
    public string CRMObjectName
    {
      get { return Row["CRMObjectName"] != DBNull.Value ? (string)Row["CRMObjectName"] : null; }
      set { Row["CRMObjectName"] = CheckValue("CRMObjectName", value); }
    }
    
    public int? CustomFieldID
    {
      get { return Row["CustomFieldID"] != DBNull.Value ? (int?)Row["CustomFieldID"] : null; }
      set { Row["CustomFieldID"] = CheckValue("CustomFieldID", value); }
    }
    
    public string TSFieldName
    {
      get { return Row["TSFieldName"] != DBNull.Value ? (string)Row["TSFieldName"] : null; }
      set { Row["TSFieldName"] = CheckValue("TSFieldName", value); }
    }
    

    
    public string CRMFieldName
    {
      get { return (string)Row["CRMFieldName"]; }
      set { Row["CRMFieldName"] = CheckValue("CRMFieldName", value); }
    }
    
    public int CRMLinkID
    {
      get { return (int)Row["CRMLinkID"]; }
      set { Row["CRMLinkID"] = CheckValue("CRMLinkID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CRMLinkFields : BaseCollection, IEnumerable<CRMLinkField>
  {
    public CRMLinkFields(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CRMLinkFields"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CRMFieldID"; }
    }



    public CRMLinkField this[int index]
    {
      get { return new CRMLinkField(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CRMLinkField cRMLinkField);
    partial void AfterRowInsert(CRMLinkField cRMLinkField);
    partial void BeforeRowEdit(CRMLinkField cRMLinkField);
    partial void AfterRowEdit(CRMLinkField cRMLinkField);
    partial void BeforeRowDelete(int cRMFieldID);
    partial void AfterRowDelete(int cRMFieldID);    

    partial void BeforeDBDelete(int cRMFieldID);
    partial void AfterDBDelete(int cRMFieldID);    

    #endregion

    #region Public Methods

    public CRMLinkFieldProxy[] GetCRMLinkFieldProxies()
    {
      List<CRMLinkFieldProxy> list = new List<CRMLinkFieldProxy>();

      foreach (CRMLinkField item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int cRMFieldID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkFields] WHERE ([CRMFieldID] = @CRMFieldID);";
        deleteCommand.Parameters.Add("CRMFieldID", SqlDbType.Int);
        deleteCommand.Parameters["CRMFieldID"].Value = cRMFieldID;

        BeforeDBDelete(cRMFieldID);
        BeforeRowDelete(cRMFieldID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(cRMFieldID);
        AfterDBDelete(cRMFieldID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CRMLinkFieldsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CRMLinkFields] SET     [CRMLinkID] = @CRMLinkID,    [CRMObjectName] = @CRMObjectName,    [CRMFieldName] = @CRMFieldName,    [CustomFieldID] = @CustomFieldID,    [TSFieldName] = @TSFieldName  WHERE ([CRMFieldID] = @CRMFieldID);";

		
		tempParameter = updateCommand.Parameters.Add("CRMFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMLinkID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMObjectName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMFieldName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TSFieldName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CRMLinkFields] (    [CRMLinkID],    [CRMObjectName],    [CRMFieldName],    [CustomFieldID],    [TSFieldName]) VALUES ( @CRMLinkID, @CRMObjectName, @CRMFieldName, @CustomFieldID, @TSFieldName); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("TSFieldName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CustomFieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMFieldName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMObjectName", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMLinkID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkFields] WHERE ([CRMFieldID] = @CRMFieldID);";
		deleteCommand.Parameters.Add("CRMFieldID", SqlDbType.Int);

		try
		{
		  foreach (CRMLinkField cRMLinkField in this)
		  {
			if (cRMLinkField.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(cRMLinkField);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = cRMLinkField.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CRMFieldID"].AutoIncrement = false;
			  Table.Columns["CRMFieldID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				cRMLinkField.Row["CRMFieldID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(cRMLinkField);
			}
			else if (cRMLinkField.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(cRMLinkField);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = cRMLinkField.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(cRMLinkField);
			}
			else if (cRMLinkField.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)cRMLinkField.Row["CRMFieldID", DataRowVersion.Original];
			  deleteCommand.Parameters["CRMFieldID"].Value = id;
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

      foreach (CRMLinkField cRMLinkField in this)
      {
        if (cRMLinkField.Row.Table.Columns.Contains("CreatorID") && (int)cRMLinkField.Row["CreatorID"] == 0) cRMLinkField.Row["CreatorID"] = LoginUser.UserID;
        if (cRMLinkField.Row.Table.Columns.Contains("ModifierID")) cRMLinkField.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CRMLinkField FindByCRMFieldID(int cRMFieldID)
    {
      foreach (CRMLinkField cRMLinkField in this)
      {
        if (cRMLinkField.CRMFieldID == cRMFieldID)
        {
          return cRMLinkField;
        }
      }
      return null;
    }

    public virtual CRMLinkField AddNewCRMLinkField()
    {
      if (Table.Columns.Count < 1) LoadColumns("CRMLinkFields");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CRMLinkField(row, this);
    }
    
    public virtual void LoadByCRMFieldID(int cRMFieldID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CRMFieldID], [CRMLinkID], [CRMObjectName], [CRMFieldName], [CustomFieldID], [TSFieldName] FROM [dbo].[CRMLinkFields] WHERE ([CRMFieldID] = @CRMFieldID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CRMFieldID", cRMFieldID);
        Fill(command);
      }
    }
    
    public static CRMLinkField GetCRMLinkField(LoginUser loginUser, int cRMFieldID)
    {
      CRMLinkFields cRMLinkFields = new CRMLinkFields(loginUser);
      cRMLinkFields.LoadByCRMFieldID(cRMFieldID);
      if (cRMLinkFields.IsEmpty)
        return null;
      else
        return cRMLinkFields[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CRMLinkField> Members

    public IEnumerator<CRMLinkField> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CRMLinkField(row, this);
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
