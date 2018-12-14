using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CRMLinkError : BaseItem
  {
    private CRMLinkErrors _cRMLinkErrors;
    
    public CRMLinkError(DataRow row, CRMLinkErrors cRMLinkErrors): base(row, cRMLinkErrors)
    {
      _cRMLinkErrors = cRMLinkErrors;
    }
	
    #region Properties
    
    public CRMLinkErrors Collection
    {
      get { return _cRMLinkErrors; }
    }
        
    
    
    
    public int CRMLinkErrorID
    {
      get { return (int)Row["CRMLinkErrorID"]; }
    }
    

    
    public string ObjectData
    {
      get { return Row["ObjectData"] != DBNull.Value ? (string)Row["ObjectData"] : null; }
      set { Row["ObjectData"] = CheckValue("ObjectData", value); }
    }
    
    public string Exception
    {
      get { return Row["Exception"] != DBNull.Value ? (string)Row["Exception"] : null; }
      set { Row["Exception"] = CheckValue("Exception", value); }
    }
    
    public string ErrorMessage
    {
      get { return Row["ErrorMessage"] != DBNull.Value ? (string)Row["ErrorMessage"] : null; }
      set { Row["ErrorMessage"] = CheckValue("ErrorMessage", value); }
    }
    

    
    public bool IsCleared
    {
      get { return (bool)Row["IsCleared"]; }
      set { Row["IsCleared"] = CheckValue("IsCleared", value); }
    }
    
    public int ErrorCount
    {
      get { return (int)Row["ErrorCount"]; }
      set { Row["ErrorCount"] = CheckValue("ErrorCount", value); }
    }
    
    public string OperationType
    {
      get { return (string)Row["OperationType"]; }
      set { Row["OperationType"] = CheckValue("OperationType", value); }
    }
    
    public string ObjectFieldName
    {
      get { return (string)Row["ObjectFieldName"]; }
      set { Row["ObjectFieldName"] = CheckValue("ObjectFieldName", value); }
    }
    
    public string ObjectID
    {
      get { return (string)Row["ObjectID"]; }
      set { Row["ObjectID"] = CheckValue("ObjectID", value); }
    }
    
    public string ObjectType
    {
      get { return (string)Row["ObjectType"]; }
      set { Row["ObjectType"] = CheckValue("ObjectType", value); }
    }
    
    public string Orientation
    {
      get { return (string)Row["Orientation"]; }
      set { Row["Orientation"] = CheckValue("Orientation", value); }
    }
    
    public string CRMType
    {
      get { return (string)Row["CRMType"]; }
      set { Row["CRMType"] = CheckValue("CRMType", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
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

  public partial class CRMLinkErrors : BaseCollection, IEnumerable<CRMLinkError>
  {
    public CRMLinkErrors(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CRMLinkErrors"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CRMLinkErrorID"; }
    }



    public CRMLinkError this[int index]
    {
      get { return new CRMLinkError(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CRMLinkError cRMLinkError);
    partial void AfterRowInsert(CRMLinkError cRMLinkError);
    partial void BeforeRowEdit(CRMLinkError cRMLinkError);
    partial void AfterRowEdit(CRMLinkError cRMLinkError);
    partial void BeforeRowDelete(int cRMLinkErrorID);
    partial void AfterRowDelete(int cRMLinkErrorID);    

    partial void BeforeDBDelete(int cRMLinkErrorID);
    partial void AfterDBDelete(int cRMLinkErrorID);    

    #endregion

    #region Public Methods

    public CRMLinkErrorProxy[] GetCRMLinkErrorProxies()
    {
      List<CRMLinkErrorProxy> list = new List<CRMLinkErrorProxy>();

      foreach (CRMLinkError item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int cRMLinkErrorID)
    {
      BeforeDBDelete(cRMLinkErrorID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkErrors] WHERE ([CRMLinkErrorID] = @CRMLinkErrorID);";
        deleteCommand.Parameters.Add("CRMLinkErrorID", SqlDbType.Int);
        deleteCommand.Parameters["CRMLinkErrorID"].Value = cRMLinkErrorID;

        BeforeRowDelete(cRMLinkErrorID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(cRMLinkErrorID);
      }
      AfterDBDelete(cRMLinkErrorID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CRMLinkErrorsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CRMLinkErrors] SET     [OrganizationID] = @OrganizationID,    [CRMType] = @CRMType,    [Orientation] = @Orientation,    [ObjectType] = @ObjectType,    [ObjectID] = @ObjectID,    [ObjectFieldName] = @ObjectFieldName,    [ObjectData] = @ObjectData,    [Exception] = @Exception,    [OperationType] = @OperationType,    [DateModified] = @DateModified,    [ErrorMessage] = @ErrorMessage,    [ErrorCount] = @ErrorCount,    [IsCleared] = @IsCleared  WHERE ([CRMLinkErrorID] = @CRMLinkErrorID);";

		
		tempParameter = updateCommand.Parameters.Add("CRMLinkErrorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Orientation", SqlDbType.VarChar, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ObjectType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ObjectID", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ObjectFieldName", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ObjectData", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Exception", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OperationType", SqlDbType.VarChar, 10);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ErrorMessage", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ErrorCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsCleared", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CRMLinkErrors] (    [OrganizationID],    [CRMType],    [Orientation],    [ObjectType],    [ObjectID],    [ObjectFieldName],    [ObjectData],    [Exception],    [OperationType],    [DateCreated],    [DateModified],    [ErrorMessage],    [ErrorCount],    [IsCleared]) VALUES ( @OrganizationID, @CRMType, @Orientation, @ObjectType, @ObjectID, @ObjectFieldName, @ObjectData, @Exception, @OperationType, @DateCreated, @DateModified, @ErrorMessage, @ErrorCount, @IsCleared); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsCleared", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ErrorCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ErrorMessage", SqlDbType.VarChar, 500);
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
		
		tempParameter = insertCommand.Parameters.Add("OperationType", SqlDbType.VarChar, 10);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Exception", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ObjectData", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ObjectFieldName", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ObjectID", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ObjectType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Orientation", SqlDbType.VarChar, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMType", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkErrors] WHERE ([CRMLinkErrorID] = @CRMLinkErrorID);";
		deleteCommand.Parameters.Add("CRMLinkErrorID", SqlDbType.Int);

		try
		{
		  foreach (CRMLinkError cRMLinkError in this)
		  {
			if (cRMLinkError.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(cRMLinkError);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = cRMLinkError.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CRMLinkErrorID"].AutoIncrement = false;
			  Table.Columns["CRMLinkErrorID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				cRMLinkError.Row["CRMLinkErrorID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(cRMLinkError);
			}
			else if (cRMLinkError.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(cRMLinkError);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = cRMLinkError.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(cRMLinkError);
			}
			else if (cRMLinkError.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)cRMLinkError.Row["CRMLinkErrorID", DataRowVersion.Original];
			  deleteCommand.Parameters["CRMLinkErrorID"].Value = id;
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

      foreach (CRMLinkError cRMLinkError in this)
      {
        if (cRMLinkError.Row.Table.Columns.Contains("CreatorID") && (int)cRMLinkError.Row["CreatorID"] == 0) cRMLinkError.Row["CreatorID"] = LoginUser.UserID;
        if (cRMLinkError.Row.Table.Columns.Contains("ModifierID")) cRMLinkError.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CRMLinkError FindByCRMLinkErrorID(int cRMLinkErrorID)
    {
      foreach (CRMLinkError cRMLinkError in this)
      {
        if (cRMLinkError.CRMLinkErrorID == cRMLinkErrorID)
        {
          return cRMLinkError;
        }
      }
      return null;
    }

    public virtual CRMLinkError AddNewCRMLinkError()
    {
      if (Table.Columns.Count < 1) LoadColumns("CRMLinkErrors");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CRMLinkError(row, this);
    }
    
    public virtual void LoadByCRMLinkErrorID(int cRMLinkErrorID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CRMLinkErrorID], [OrganizationID], [CRMType], [Orientation], [ObjectType], [ObjectID], [ObjectFieldName], [ObjectData], [Exception], [OperationType], [DateCreated], [DateModified], [ErrorMessage], [ErrorCount], [IsCleared] FROM [dbo].[CRMLinkErrors] WHERE ([CRMLinkErrorID] = @CRMLinkErrorID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CRMLinkErrorID", cRMLinkErrorID);
        Fill(command);
      }
    }
    
    public static CRMLinkError GetCRMLinkError(LoginUser loginUser, int cRMLinkErrorID)
    {
      CRMLinkErrors cRMLinkErrors = new CRMLinkErrors(loginUser);
      cRMLinkErrors.LoadByCRMLinkErrorID(cRMLinkErrorID);
      if (cRMLinkErrors.IsEmpty)
        return null;
      else
        return cRMLinkErrors[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CRMLinkError> Members

    public IEnumerator<CRMLinkError> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CRMLinkError(row, this);
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
