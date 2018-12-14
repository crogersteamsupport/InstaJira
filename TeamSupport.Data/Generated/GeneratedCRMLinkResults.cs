using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CRMLinkResult : BaseItem
  {
    private CRMLinkResults _cRMLinkResults;
    
    public CRMLinkResult(DataRow row, CRMLinkResults cRMLinkResults): base(row, cRMLinkResults)
    {
      _cRMLinkResults = cRMLinkResults;
    }
	
    #region Properties
    
    public CRMLinkResults Collection
    {
      get { return _cRMLinkResults; }
    }
        
    
    
    
    public int CRMResultsID
    {
      get { return (int)Row["CRMResultsID"]; }
    }
    

    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public string AttemptResult
    {
      get { return Row["AttemptResult"] != DBNull.Value ? (string)Row["AttemptResult"] : null; }
      set { Row["AttemptResult"] = CheckValue("AttemptResult", value); }
    }
    

    

    /* DateTime */
    
    
    

    
    public DateTime? AttemptDateTime
    {
      get { return Row["AttemptDateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["AttemptDateTime"]) : null; }
      set { Row["AttemptDateTime"] = CheckValue("AttemptDateTime", value); }
    }

    public DateTime? AttemptDateTimeUtc
    {
      get { return Row["AttemptDateTime"] != DBNull.Value ? (DateTime?)Row["AttemptDateTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class CRMLinkResults : BaseCollection, IEnumerable<CRMLinkResult>
  {
    public CRMLinkResults(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CRMLinkResults"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CRMResultsID"; }
    }



    public CRMLinkResult this[int index]
    {
      get { return new CRMLinkResult(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CRMLinkResult cRMLinkResult);
    partial void AfterRowInsert(CRMLinkResult cRMLinkResult);
    partial void BeforeRowEdit(CRMLinkResult cRMLinkResult);
    partial void AfterRowEdit(CRMLinkResult cRMLinkResult);
    partial void BeforeRowDelete(int cRMResultsID);
    partial void AfterRowDelete(int cRMResultsID);    

    partial void BeforeDBDelete(int cRMResultsID);
    partial void AfterDBDelete(int cRMResultsID);    

    #endregion

    #region Public Methods

    public CRMLinkResultProxy[] GetCRMLinkResultProxies()
    {
      List<CRMLinkResultProxy> list = new List<CRMLinkResultProxy>();

      foreach (CRMLinkResult item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int cRMResultsID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkResults] WHERE ([CRMResultsID] = @CRMResultsID);";
        deleteCommand.Parameters.Add("CRMResultsID", SqlDbType.Int);
        deleteCommand.Parameters["CRMResultsID"].Value = cRMResultsID;

        BeforeDBDelete(cRMResultsID);
        BeforeRowDelete(cRMResultsID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(cRMResultsID);
        AfterDBDelete(cRMResultsID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CRMLinkResultsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CRMLinkResults] SET     [OrganizationID] = @OrganizationID,    [AttemptDateTime] = @AttemptDateTime,    [AttemptResult] = @AttemptResult  WHERE ([CRMResultsID] = @CRMResultsID);";

		
		tempParameter = updateCommand.Parameters.Add("CRMResultsID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("AttemptDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("AttemptResult", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CRMLinkResults] (    [OrganizationID],    [AttemptDateTime],    [AttemptResult]) VALUES ( @OrganizationID, @AttemptDateTime, @AttemptResult); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("AttemptResult", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AttemptDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkResults] WHERE ([CRMResultsID] = @CRMResultsID);";
		deleteCommand.Parameters.Add("CRMResultsID", SqlDbType.Int);

		try
		{
		  foreach (CRMLinkResult cRMLinkResult in this)
		  {
			if (cRMLinkResult.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(cRMLinkResult);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = cRMLinkResult.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CRMResultsID"].AutoIncrement = false;
			  Table.Columns["CRMResultsID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				cRMLinkResult.Row["CRMResultsID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(cRMLinkResult);
			}
			else if (cRMLinkResult.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(cRMLinkResult);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = cRMLinkResult.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(cRMLinkResult);
			}
			else if (cRMLinkResult.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)cRMLinkResult.Row["CRMResultsID", DataRowVersion.Original];
			  deleteCommand.Parameters["CRMResultsID"].Value = id;
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

      foreach (CRMLinkResult cRMLinkResult in this)
      {
        if (cRMLinkResult.Row.Table.Columns.Contains("CreatorID") && (int)cRMLinkResult.Row["CreatorID"] == 0) cRMLinkResult.Row["CreatorID"] = LoginUser.UserID;
        if (cRMLinkResult.Row.Table.Columns.Contains("ModifierID")) cRMLinkResult.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CRMLinkResult FindByCRMResultsID(int cRMResultsID)
    {
      foreach (CRMLinkResult cRMLinkResult in this)
      {
        if (cRMLinkResult.CRMResultsID == cRMResultsID)
        {
          return cRMLinkResult;
        }
      }
      return null;
    }

    public virtual CRMLinkResult AddNewCRMLinkResult()
    {
      if (Table.Columns.Count < 1) LoadColumns("CRMLinkResults");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CRMLinkResult(row, this);
    }
    
    public virtual void LoadByCRMResultsID(int cRMResultsID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CRMResultsID], [OrganizationID], [AttemptDateTime], [AttemptResult] FROM [dbo].[CRMLinkResults] WHERE ([CRMResultsID] = @CRMResultsID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CRMResultsID", cRMResultsID);
        Fill(command);
      }
    }
    
    public static CRMLinkResult GetCRMLinkResult(LoginUser loginUser, int cRMResultsID)
    {
      CRMLinkResults cRMLinkResults = new CRMLinkResults(loginUser);
      cRMLinkResults.LoadByCRMResultsID(cRMResultsID);
      if (cRMLinkResults.IsEmpty)
        return null;
      else
        return cRMLinkResults[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CRMLinkResult> Members

    public IEnumerator<CRMLinkResult> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CRMLinkResult(row, this);
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
