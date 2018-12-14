using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CRMLinkSynchedOrganization : BaseItem
  {
    private CRMLinkSynchedOrganizations _cRMLinkSynchedOrganizations;
    
    public CRMLinkSynchedOrganization(DataRow row, CRMLinkSynchedOrganizations cRMLinkSynchedOrganizations): base(row, cRMLinkSynchedOrganizations)
    {
      _cRMLinkSynchedOrganizations = cRMLinkSynchedOrganizations;
    }
	
    #region Properties
    
    public CRMLinkSynchedOrganizations Collection
    {
      get { return _cRMLinkSynchedOrganizations; }
    }
        
    
    
    
    public int CRMLinkSynchedOrganizationsID
    {
      get { return (int)Row["CRMLinkSynchedOrganizationsID"]; }
    }
    

    

    
    public string OrganizationCRMID
    {
      get { return (string)Row["OrganizationCRMID"]; }
      set { Row["OrganizationCRMID"] = CheckValue("OrganizationCRMID", value); }
    }
    
    public int CRMLinkTableID
    {
      get { return (int)Row["CRMLinkTableID"]; }
      set { Row["CRMLinkTableID"] = CheckValue("CRMLinkTableID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CRMLinkSynchedOrganizations : BaseCollection, IEnumerable<CRMLinkSynchedOrganization>
  {
    public CRMLinkSynchedOrganizations(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CRMLinkSynchedOrganizations"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CRMLinkSynchedOrganizationsID"; }
    }



    public CRMLinkSynchedOrganization this[int index]
    {
      get { return new CRMLinkSynchedOrganization(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CRMLinkSynchedOrganization cRMLinkSynchedOrganization);
    partial void AfterRowInsert(CRMLinkSynchedOrganization cRMLinkSynchedOrganization);
    partial void BeforeRowEdit(CRMLinkSynchedOrganization cRMLinkSynchedOrganization);
    partial void AfterRowEdit(CRMLinkSynchedOrganization cRMLinkSynchedOrganization);
    partial void BeforeRowDelete(int cRMLinkSynchedOrganizationsID);
    partial void AfterRowDelete(int cRMLinkSynchedOrganizationsID);    

    partial void BeforeDBDelete(int cRMLinkSynchedOrganizationsID);
    partial void AfterDBDelete(int cRMLinkSynchedOrganizationsID);    

    #endregion

    #region Public Methods

    public CRMLinkSynchedOrganizationProxy[] GetCRMLinkSynchedOrganizationProxies()
    {
      List<CRMLinkSynchedOrganizationProxy> list = new List<CRMLinkSynchedOrganizationProxy>();

      foreach (CRMLinkSynchedOrganization item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int cRMLinkSynchedOrganizationsID)
    {
      BeforeDBDelete(cRMLinkSynchedOrganizationsID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkSynchedOrganizations] WHERE ([CRMLinkSynchedOrganizationsID] = @CRMLinkSynchedOrganizationsID);";
        deleteCommand.Parameters.Add("CRMLinkSynchedOrganizationsID", SqlDbType.Int);
        deleteCommand.Parameters["CRMLinkSynchedOrganizationsID"].Value = cRMLinkSynchedOrganizationsID;

        BeforeRowDelete(cRMLinkSynchedOrganizationsID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(cRMLinkSynchedOrganizationsID);
      }
      AfterDBDelete(cRMLinkSynchedOrganizationsID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CRMLinkSynchedOrganizationsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CRMLinkSynchedOrganizations] SET     [CRMLinkTableID] = @CRMLinkTableID,    [OrganizationCRMID] = @OrganizationCRMID  WHERE ([CRMLinkSynchedOrganizationsID] = @CRMLinkSynchedOrganizationsID);";

		
		tempParameter = updateCommand.Parameters.Add("CRMLinkSynchedOrganizationsID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CRMLinkTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationCRMID", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CRMLinkSynchedOrganizations] (    [CRMLinkTableID],    [OrganizationCRMID]) VALUES ( @CRMLinkTableID, @OrganizationCRMID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OrganizationCRMID", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CRMLinkTableID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CRMLinkSynchedOrganizations] WHERE ([CRMLinkSynchedOrganizationsID] = @CRMLinkSynchedOrganizationsID);";
		deleteCommand.Parameters.Add("CRMLinkSynchedOrganizationsID", SqlDbType.Int);

		try
		{
		  foreach (CRMLinkSynchedOrganization cRMLinkSynchedOrganization in this)
		  {
			if (cRMLinkSynchedOrganization.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(cRMLinkSynchedOrganization);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = cRMLinkSynchedOrganization.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CRMLinkSynchedOrganizationsID"].AutoIncrement = false;
			  Table.Columns["CRMLinkSynchedOrganizationsID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				cRMLinkSynchedOrganization.Row["CRMLinkSynchedOrganizationsID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(cRMLinkSynchedOrganization);
			}
			else if (cRMLinkSynchedOrganization.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(cRMLinkSynchedOrganization);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = cRMLinkSynchedOrganization.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(cRMLinkSynchedOrganization);
			}
			else if (cRMLinkSynchedOrganization.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)cRMLinkSynchedOrganization.Row["CRMLinkSynchedOrganizationsID", DataRowVersion.Original];
			  deleteCommand.Parameters["CRMLinkSynchedOrganizationsID"].Value = id;
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

      foreach (CRMLinkSynchedOrganization cRMLinkSynchedOrganization in this)
      {
        if (cRMLinkSynchedOrganization.Row.Table.Columns.Contains("CreatorID") && (int)cRMLinkSynchedOrganization.Row["CreatorID"] == 0) cRMLinkSynchedOrganization.Row["CreatorID"] = LoginUser.UserID;
        if (cRMLinkSynchedOrganization.Row.Table.Columns.Contains("ModifierID")) cRMLinkSynchedOrganization.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CRMLinkSynchedOrganization FindByCRMLinkSynchedOrganizationsID(int cRMLinkSynchedOrganizationsID)
    {
      foreach (CRMLinkSynchedOrganization cRMLinkSynchedOrganization in this)
      {
        if (cRMLinkSynchedOrganization.CRMLinkSynchedOrganizationsID == cRMLinkSynchedOrganizationsID)
        {
          return cRMLinkSynchedOrganization;
        }
      }
      return null;
    }

    public virtual CRMLinkSynchedOrganization AddNewCRMLinkSynchedOrganization()
    {
      if (Table.Columns.Count < 1) LoadColumns("CRMLinkSynchedOrganizations");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CRMLinkSynchedOrganization(row, this);
    }
    
    public virtual void LoadByCRMLinkSynchedOrganizationsID(int cRMLinkSynchedOrganizationsID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CRMLinkSynchedOrganizationsID], [CRMLinkTableID], [OrganizationCRMID] FROM [dbo].[CRMLinkSynchedOrganizations] WHERE ([CRMLinkSynchedOrganizationsID] = @CRMLinkSynchedOrganizationsID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CRMLinkSynchedOrganizationsID", cRMLinkSynchedOrganizationsID);
        Fill(command);
      }
    }
    
    public static CRMLinkSynchedOrganization GetCRMLinkSynchedOrganization(LoginUser loginUser, int cRMLinkSynchedOrganizationsID)
    {
      CRMLinkSynchedOrganizations cRMLinkSynchedOrganizations = new CRMLinkSynchedOrganizations(loginUser);
      cRMLinkSynchedOrganizations.LoadByCRMLinkSynchedOrganizationsID(cRMLinkSynchedOrganizationsID);
      if (cRMLinkSynchedOrganizations.IsEmpty)
        return null;
      else
        return cRMLinkSynchedOrganizations[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CRMLinkSynchedOrganization> Members

    public IEnumerator<CRMLinkSynchedOrganization> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CRMLinkSynchedOrganization(row, this);
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
