using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TSEMailIgnoreListItem : BaseItem
  {
    private TSEMailIgnoreList _tSEMailIgnoreList;
    
    public TSEMailIgnoreListItem(DataRow row, TSEMailIgnoreList tSEMailIgnoreList): base(row, tSEMailIgnoreList)
    {
      _tSEMailIgnoreList = tSEMailIgnoreList;
    }
	
    #region Properties
    
    public TSEMailIgnoreList Collection
    {
      get { return _tSEMailIgnoreList; }
    }
        
    
    
    
    public int IgnoreID
    {
      get { return (int)Row["IgnoreID"]; }
    }
    

    
    public string FromAddress
    {
      get { return Row["FromAddress"] != DBNull.Value ? (string)Row["FromAddress"] : null; }
      set { Row["FromAddress"] = CheckValue("FromAddress", value); }
    }
    
    public string ToAddress
    {
      get { return Row["ToAddress"] != DBNull.Value ? (string)Row["ToAddress"] : null; }
      set { Row["ToAddress"] = CheckValue("ToAddress", value); }
    }
    

    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class TSEMailIgnoreList : BaseCollection, IEnumerable<TSEMailIgnoreListItem>
  {
    public TSEMailIgnoreList(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TSEMailIgnoreList"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "IgnoreID"; }
    }



    public TSEMailIgnoreListItem this[int index]
    {
      get { return new TSEMailIgnoreListItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TSEMailIgnoreListItem tSEMailIgnoreListItem);
    partial void AfterRowInsert(TSEMailIgnoreListItem tSEMailIgnoreListItem);
    partial void BeforeRowEdit(TSEMailIgnoreListItem tSEMailIgnoreListItem);
    partial void AfterRowEdit(TSEMailIgnoreListItem tSEMailIgnoreListItem);
    partial void BeforeRowDelete(int ignoreID);
    partial void AfterRowDelete(int ignoreID);    

    partial void BeforeDBDelete(int ignoreID);
    partial void AfterDBDelete(int ignoreID);    

    #endregion

    #region Public Methods

    public TSEMailIgnoreListItemProxy[] GetTSEMailIgnoreListItemProxies()
    {
      List<TSEMailIgnoreListItemProxy> list = new List<TSEMailIgnoreListItemProxy>();

      foreach (TSEMailIgnoreListItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ignoreID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TSEMailIgnoreList] WHERE ([IgnoreID] = @IgnoreID);";
        deleteCommand.Parameters.Add("IgnoreID", SqlDbType.Int);
        deleteCommand.Parameters["IgnoreID"].Value = ignoreID;

        BeforeDBDelete(ignoreID);
        BeforeRowDelete(ignoreID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ignoreID);
        AfterDBDelete(ignoreID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TSEMailIgnoreListSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TSEMailIgnoreList] SET     [FromAddress] = @FromAddress,    [ToAddress] = @ToAddress  WHERE ([IgnoreID] = @IgnoreID);";

		
		tempParameter = updateCommand.Parameters.Add("IgnoreID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FromAddress", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ToAddress", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TSEMailIgnoreList] (    [FromAddress],    [ToAddress]) VALUES ( @FromAddress, @ToAddress); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ToAddress", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FromAddress", SqlDbType.VarChar, 500);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TSEMailIgnoreList] WHERE ([IgnoreID] = @IgnoreID);";
		deleteCommand.Parameters.Add("IgnoreID", SqlDbType.Int);

		try
		{
		  foreach (TSEMailIgnoreListItem tSEMailIgnoreListItem in this)
		  {
			if (tSEMailIgnoreListItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(tSEMailIgnoreListItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = tSEMailIgnoreListItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["IgnoreID"].AutoIncrement = false;
			  Table.Columns["IgnoreID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				tSEMailIgnoreListItem.Row["IgnoreID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(tSEMailIgnoreListItem);
			}
			else if (tSEMailIgnoreListItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(tSEMailIgnoreListItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = tSEMailIgnoreListItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(tSEMailIgnoreListItem);
			}
			else if (tSEMailIgnoreListItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)tSEMailIgnoreListItem.Row["IgnoreID", DataRowVersion.Original];
			  deleteCommand.Parameters["IgnoreID"].Value = id;
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

      foreach (TSEMailIgnoreListItem tSEMailIgnoreListItem in this)
      {
        if (tSEMailIgnoreListItem.Row.Table.Columns.Contains("CreatorID") && (int)tSEMailIgnoreListItem.Row["CreatorID"] == 0) tSEMailIgnoreListItem.Row["CreatorID"] = LoginUser.UserID;
        if (tSEMailIgnoreListItem.Row.Table.Columns.Contains("ModifierID")) tSEMailIgnoreListItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TSEMailIgnoreListItem FindByIgnoreID(int ignoreID)
    {
      foreach (TSEMailIgnoreListItem tSEMailIgnoreListItem in this)
      {
        if (tSEMailIgnoreListItem.IgnoreID == ignoreID)
        {
          return tSEMailIgnoreListItem;
        }
      }
      return null;
    }

    public virtual TSEMailIgnoreListItem AddNewTSEMailIgnoreListItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TSEMailIgnoreList");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TSEMailIgnoreListItem(row, this);
    }
    
    public virtual void LoadByIgnoreID(int ignoreID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [IgnoreID], [FromAddress], [ToAddress] FROM [dbo].[TSEMailIgnoreList] WHERE ([IgnoreID] = @IgnoreID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("IgnoreID", ignoreID);
        Fill(command);
      }
    }
    
    public static TSEMailIgnoreListItem GetTSEMailIgnoreListItem(LoginUser loginUser, int ignoreID)
    {
      TSEMailIgnoreList tSEMailIgnoreList = new TSEMailIgnoreList(loginUser);
      tSEMailIgnoreList.LoadByIgnoreID(ignoreID);
      if (tSEMailIgnoreList.IsEmpty)
        return null;
      else
        return tSEMailIgnoreList[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TSEMailIgnoreListItem> Members

    public IEnumerator<TSEMailIgnoreListItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TSEMailIgnoreListItem(row, this);
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
