using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class PortalLoginHistoryItem : BaseItem
  {
    private PortalLoginHistory _portalLoginHistory;
    
    public PortalLoginHistoryItem(DataRow row, PortalLoginHistory portalLoginHistory): base(row, portalLoginHistory)
    {
      _portalLoginHistory = portalLoginHistory;
    }
	
    #region Properties
    
    public PortalLoginHistory Collection
    {
      get { return _portalLoginHistory; }
    }
        
    
    
    
    public int PortalLoginID
    {
      get { return (int)Row["PortalLoginID"]; }
    }
    

    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
      set { Row["UserName"] = CheckValue("UserName", value); }
    }
    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public string OrganizationName
    {
      get { return Row["OrganizationName"] != DBNull.Value ? (string)Row["OrganizationName"] : null; }
      set { Row["OrganizationName"] = CheckValue("OrganizationName", value); }
    }
    
    public bool? Success
    {
      get { return Row["Success"] != DBNull.Value ? (bool?)Row["Success"] : null; }
      set { Row["Success"] = CheckValue("Success", value); }
    }
    
    public string IPAddress
    {
      get { return Row["IPAddress"] != DBNull.Value ? (string)Row["IPAddress"] : null; }
      set { Row["IPAddress"] = CheckValue("IPAddress", value); }
    }
    
    public string Browser
    {
      get { return Row["Browser"] != DBNull.Value ? (string)Row["Browser"] : null; }
      set { Row["Browser"] = CheckValue("Browser", value); }
    }
    

    
    public string Source
    {
      get { return (string)Row["Source"]; }
      set { Row["Source"] = CheckValue("Source", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? LoginDateTime
    {
      get { return Row["LoginDateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["LoginDateTime"]) : null; }
      set { Row["LoginDateTime"] = CheckValue("LoginDateTime", value); }
    }

    public DateTime? LoginDateTimeUtc
    {
      get { return Row["LoginDateTime"] != DBNull.Value ? (DateTime?)Row["LoginDateTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class PortalLoginHistory : BaseCollection, IEnumerable<PortalLoginHistoryItem>
  {
    public PortalLoginHistory(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "PortalLoginHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "PortalLoginID"; }
    }



    public PortalLoginHistoryItem this[int index]
    {
      get { return new PortalLoginHistoryItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(PortalLoginHistoryItem portalLoginHistoryItem);
    partial void AfterRowInsert(PortalLoginHistoryItem portalLoginHistoryItem);
    partial void BeforeRowEdit(PortalLoginHistoryItem portalLoginHistoryItem);
    partial void AfterRowEdit(PortalLoginHistoryItem portalLoginHistoryItem);
    partial void BeforeRowDelete(int portalLoginID);
    partial void AfterRowDelete(int portalLoginID);    

    partial void BeforeDBDelete(int portalLoginID);
    partial void AfterDBDelete(int portalLoginID);    

    #endregion

    #region Public Methods

    public PortalLoginHistoryItemProxy[] GetPortalLoginHistoryItemProxies()
    {
      List<PortalLoginHistoryItemProxy> list = new List<PortalLoginHistoryItemProxy>();

      foreach (PortalLoginHistoryItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int portalLoginID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PortalLoginHistory] WHERE ([PortalLoginID] = @PortalLoginID);";
        deleteCommand.Parameters.Add("PortalLoginID", SqlDbType.Int);
        deleteCommand.Parameters["PortalLoginID"].Value = portalLoginID;

        BeforeDBDelete(portalLoginID);
        BeforeRowDelete(portalLoginID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(portalLoginID);
        AfterDBDelete(portalLoginID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PortalLoginHistorySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[PortalLoginHistory] SET     [UserName] = @UserName,    [OrganizationID] = @OrganizationID,    [OrganizationName] = @OrganizationName,    [Success] = @Success,    [LoginDateTime] = @LoginDateTime,    [IPAddress] = @IPAddress,    [Browser] = @Browser,    [UserID] = @UserID,    [Source] = @Source  WHERE ([PortalLoginID] = @PortalLoginID);";

		
		tempParameter = updateCommand.Parameters.Add("PortalLoginID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserName", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationName", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Success", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LoginDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IPAddress", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Browser", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Source", SqlDbType.NVarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[PortalLoginHistory] (    [UserName],    [OrganizationID],    [OrganizationName],    [Success],    [LoginDateTime],    [IPAddress],    [Browser],    [UserID],    [Source]) VALUES ( @UserName, @OrganizationID, @OrganizationName, @Success, @LoginDateTime, @IPAddress, @Browser, @UserID, @Source); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Source", SqlDbType.NVarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Browser", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IPAddress", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LoginDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Success", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationName", SqlDbType.VarChar, 200);
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
		
		tempParameter = insertCommand.Parameters.Add("UserName", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PortalLoginHistory] WHERE ([PortalLoginID] = @PortalLoginID);";
		deleteCommand.Parameters.Add("PortalLoginID", SqlDbType.Int);

		try
		{
		  foreach (PortalLoginHistoryItem portalLoginHistoryItem in this)
		  {
			if (portalLoginHistoryItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(portalLoginHistoryItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = portalLoginHistoryItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["PortalLoginID"].AutoIncrement = false;
			  Table.Columns["PortalLoginID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				portalLoginHistoryItem.Row["PortalLoginID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(portalLoginHistoryItem);
			}
			else if (portalLoginHistoryItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(portalLoginHistoryItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = portalLoginHistoryItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(portalLoginHistoryItem);
			}
			else if (portalLoginHistoryItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)portalLoginHistoryItem.Row["PortalLoginID", DataRowVersion.Original];
			  deleteCommand.Parameters["PortalLoginID"].Value = id;
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

      foreach (PortalLoginHistoryItem portalLoginHistoryItem in this)
      {
        if (portalLoginHistoryItem.Row.Table.Columns.Contains("CreatorID") && (int)portalLoginHistoryItem.Row["CreatorID"] == 0) portalLoginHistoryItem.Row["CreatorID"] = LoginUser.UserID;
        if (portalLoginHistoryItem.Row.Table.Columns.Contains("ModifierID")) portalLoginHistoryItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public PortalLoginHistoryItem FindByPortalLoginID(int portalLoginID)
    {
      foreach (PortalLoginHistoryItem portalLoginHistoryItem in this)
      {
        if (portalLoginHistoryItem.PortalLoginID == portalLoginID)
        {
          return portalLoginHistoryItem;
        }
      }
      return null;
    }

    public virtual PortalLoginHistoryItem AddNewPortalLoginHistoryItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("PortalLoginHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new PortalLoginHistoryItem(row, this);
    }
    
    public virtual void LoadByPortalLoginID(int portalLoginID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [PortalLoginID], [UserName], [OrganizationID], [OrganizationName], [Success], [LoginDateTime], [IPAddress], [Browser], [UserID], [Source] FROM [dbo].[PortalLoginHistory] WHERE ([PortalLoginID] = @PortalLoginID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("PortalLoginID", portalLoginID);
        Fill(command);
      }
    }
    
    public static PortalLoginHistoryItem GetPortalLoginHistoryItem(LoginUser loginUser, int portalLoginID)
    {
      PortalLoginHistory portalLoginHistory = new PortalLoginHistory(loginUser);
      portalLoginHistory.LoadByPortalLoginID(portalLoginID);
      if (portalLoginHistory.IsEmpty)
        return null;
      else
        return portalLoginHistory[0];
    }
    
    
    

    #endregion

    #region IEnumerable<PortalLoginHistoryItem> Members

    public IEnumerator<PortalLoginHistoryItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new PortalLoginHistoryItem(row, this);
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
