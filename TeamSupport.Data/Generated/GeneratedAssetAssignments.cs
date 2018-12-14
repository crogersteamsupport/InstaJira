using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AssetAssignment : BaseItem
  {
    private AssetAssignments _assetAssignments;
    
    public AssetAssignment(DataRow row, AssetAssignments assetAssignments): base(row, assetAssignments)
    {
      _assetAssignments = assetAssignments;
    }
	
    #region Properties
    
    public AssetAssignments Collection
    {
      get { return _assetAssignments; }
    }
        
    
    
    
    public int AssetAssignmentsID
    {
      get { return (int)Row["AssetAssignmentsID"]; }
    }
    

    
    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
    }
    

    
    public int HistoryID
    {
      get { return (int)Row["HistoryID"]; }
      set { Row["HistoryID"] = CheckValue("HistoryID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class AssetAssignments : BaseCollection, IEnumerable<AssetAssignment>
  {
    public AssetAssignments(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AssetAssignments"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AssetAssignmentsID"; }
    }



    public AssetAssignment this[int index]
    {
      get { return new AssetAssignment(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AssetAssignment assetAssignment);
    partial void AfterRowInsert(AssetAssignment assetAssignment);
    partial void BeforeRowEdit(AssetAssignment assetAssignment);
    partial void AfterRowEdit(AssetAssignment assetAssignment);
    partial void BeforeRowDelete(int assetAssignmentsID);
    partial void AfterRowDelete(int assetAssignmentsID);    

    partial void BeforeDBDelete(int assetAssignmentsID);
    partial void AfterDBDelete(int assetAssignmentsID);    

    #endregion

    #region Public Methods

    public AssetAssignmentProxy[] GetAssetAssignmentProxies()
    {
      List<AssetAssignmentProxy> list = new List<AssetAssignmentProxy>();

      foreach (AssetAssignment item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int assetAssignmentsID)
    {
      BeforeDBDelete(assetAssignmentsID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AssetAssignments] WHERE ([AssetAssignmentsID] = @AssetAssignmentsID);";
        deleteCommand.Parameters.Add("AssetAssignmentsID", SqlDbType.Int);
        deleteCommand.Parameters["AssetAssignmentsID"].Value = assetAssignmentsID;

        BeforeRowDelete(assetAssignmentsID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(assetAssignmentsID);
      }
      AfterDBDelete(assetAssignmentsID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AssetAssignmentsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[AssetAssignments] SET     [HistoryID] = @HistoryID,    [ImportFileID] = @ImportFileID  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID);";

		
		tempParameter = updateCommand.Parameters.Add("AssetAssignmentsID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("HistoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[AssetAssignments] (    [HistoryID],    [ImportFileID]) VALUES ( @HistoryID, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("HistoryID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AssetAssignments] WHERE ([AssetAssignmentsID] = @AssetAssignmentsID);";
		deleteCommand.Parameters.Add("AssetAssignmentsID", SqlDbType.Int);

		try
		{
		  foreach (AssetAssignment assetAssignment in this)
		  {
			if (assetAssignment.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(assetAssignment);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = assetAssignment.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AssetAssignmentsID"].AutoIncrement = false;
			  Table.Columns["AssetAssignmentsID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				assetAssignment.Row["AssetAssignmentsID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(assetAssignment);
			}
			else if (assetAssignment.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(assetAssignment);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = assetAssignment.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(assetAssignment);
			}
			else if (assetAssignment.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)assetAssignment.Row["AssetAssignmentsID", DataRowVersion.Original];
			  deleteCommand.Parameters["AssetAssignmentsID"].Value = id;
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

      foreach (AssetAssignment assetAssignment in this)
      {
        if (assetAssignment.Row.Table.Columns.Contains("CreatorID") && (int)assetAssignment.Row["CreatorID"] == 0) assetAssignment.Row["CreatorID"] = LoginUser.UserID;
        if (assetAssignment.Row.Table.Columns.Contains("ModifierID")) assetAssignment.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AssetAssignment FindByAssetAssignmentsID(int assetAssignmentsID)
    {
      foreach (AssetAssignment assetAssignment in this)
      {
        if (assetAssignment.AssetAssignmentsID == assetAssignmentsID)
        {
          return assetAssignment;
        }
      }
      return null;
    }

    public virtual AssetAssignment AddNewAssetAssignment()
    {
      if (Table.Columns.Count < 1) LoadColumns("AssetAssignments");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AssetAssignment(row, this);
    }
    
    public virtual void LoadByAssetAssignmentsID(int assetAssignmentsID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AssetAssignmentsID], [HistoryID], [ImportFileID] FROM [dbo].[AssetAssignments] WHERE ([AssetAssignmentsID] = @AssetAssignmentsID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AssetAssignmentsID", assetAssignmentsID);
        Fill(command);
      }
    }
    
    public static AssetAssignment GetAssetAssignment(LoginUser loginUser, int assetAssignmentsID)
    {
      AssetAssignments assetAssignments = new AssetAssignments(loginUser);
      assetAssignments.LoadByAssetAssignmentsID(assetAssignmentsID);
      if (assetAssignments.IsEmpty)
        return null;
      else
        return assetAssignments[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AssetAssignment> Members

    public IEnumerator<AssetAssignment> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AssetAssignment(row, this);
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
