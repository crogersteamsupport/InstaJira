using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class RecentlyViewedItem : BaseItem
  {
    private RecentlyViewedItems _recentlyViewedItems;
    
    public RecentlyViewedItem(DataRow row, RecentlyViewedItems recentlyViewedItems): base(row, recentlyViewedItems)
    {
      _recentlyViewedItems = recentlyViewedItems;
    }
	
    #region Properties
    
    public RecentlyViewedItems Collection
    {
      get { return _recentlyViewedItems; }
    }
        
    
    
    

    

    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateViewed
    {
      get { return DateToLocal((DateTime)Row["DateViewed"]); }
      set { Row["DateViewed"] = CheckValue("DateViewed", value); }
    }

    public DateTime DateViewedUtc
    {
      get { return (DateTime)Row["DateViewed"]; }
    }
    

    #endregion
    
    
  }

  public partial class RecentlyViewedItems : BaseCollection, IEnumerable<RecentlyViewedItem>
  {
    public RecentlyViewedItems(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "RecentlyViewedItems"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserID"; }
    }



    public RecentlyViewedItem this[int index]
    {
      get { return new RecentlyViewedItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(RecentlyViewedItem recentlyViewedItem);
    partial void AfterRowInsert(RecentlyViewedItem recentlyViewedItem);
    partial void BeforeRowEdit(RecentlyViewedItem recentlyViewedItem);
    partial void AfterRowEdit(RecentlyViewedItem recentlyViewedItem);
    partial void BeforeRowDelete(int userID);
    partial void AfterRowDelete(int userID);    

    partial void BeforeDBDelete(int userID);
    partial void AfterDBDelete(int userID);    

    #endregion

    #region Public Methods

    public RecentlyViewedItemProxy[] GetRecentlyViewedItemProxies()
    {
      List<RecentlyViewedItemProxy> list = new List<RecentlyViewedItemProxy>();

      foreach (RecentlyViewedItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userID)
    {
      BeforeDBDelete(userID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[RecentlyViewedItems] WHERE ([UserID] = @UserID AND [RefType] = @RefType AND [RefID] = @RefID);";
        deleteCommand.Parameters.Add("UserID", SqlDbType.Int);
        deleteCommand.Parameters["UserID"].Value = userID;

        BeforeRowDelete(userID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(userID);
      }
      AfterDBDelete(userID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("RecentlyViewedItemsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[RecentlyViewedItems] SET     [DateViewed] = @DateViewed  WHERE ([UserID] = @UserID AND [RefType] = @RefType AND [RefID] = @RefID);";

		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateViewed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; IF EXISTS (SELECT * FROM RecentlyViewedItems WHERE UserID=@UserID AND RefType=@RefType AND RefID=@RefID) UPDATE RecentlyViewedItems SET DateViewed=GETUTCDATE() WHERE UserID=@UserID AND RefType=@RefType AND RefID=@RefID ELSE  INSERT INTO RecentlyViewedItems (UserID, RefType, RefID, DateViewed) VALUES (@UserID, @RefType, @RefID, GETUTCDATE())";

		
		tempParameter = insertCommand.Parameters.Add("DateViewed", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[RecentlyViewedItems] WHERE ([UserID] = @UserID AND [RefType] = @RefType AND [RefID] = @RefID);";
		deleteCommand.Parameters.Add("UserID", SqlDbType.Int);

		try
		{
		  foreach (RecentlyViewedItem recentlyViewedItem in this)
		  {
			if (recentlyViewedItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(recentlyViewedItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = recentlyViewedItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserID"].AutoIncrement = false;
			  Table.Columns["UserID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				recentlyViewedItem.Row["UserID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(recentlyViewedItem);
			}
			else if (recentlyViewedItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(recentlyViewedItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = recentlyViewedItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(recentlyViewedItem);
			}
			else if (recentlyViewedItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)recentlyViewedItem.Row["UserID", DataRowVersion.Original];
			  deleteCommand.Parameters["UserID"].Value = id;
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

      foreach (RecentlyViewedItem recentlyViewedItem in this)
      {
        if (recentlyViewedItem.Row.Table.Columns.Contains("CreatorID") && (int)recentlyViewedItem.Row["CreatorID"] == 0) recentlyViewedItem.Row["CreatorID"] = LoginUser.UserID;
        if (recentlyViewedItem.Row.Table.Columns.Contains("ModifierID")) recentlyViewedItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public RecentlyViewedItem FindByUserID(int userID)
    {
      foreach (RecentlyViewedItem recentlyViewedItem in this)
      {
        if (recentlyViewedItem.UserID == userID)
        {
          return recentlyViewedItem;
        }
      }
      return null;
    }

    public virtual RecentlyViewedItem AddNewRecentlyViewedItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("RecentlyViewedItems");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new RecentlyViewedItem(row, this);
    }
    
    public virtual void LoadByUserID(int userID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserID], [RefType], [RefID], [DateViewed] FROM [dbo].[RecentlyViewedItems] WHERE ([UserID] = @UserID AND [RefType] = @RefType AND [RefID] = @RefID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserID", userID);
        Fill(command);
      }
    }
    
    public static RecentlyViewedItem GetRecentlyViewedItem(LoginUser loginUser, int userID)
    {
      RecentlyViewedItems recentlyViewedItems = new RecentlyViewedItems(loginUser);
      recentlyViewedItems.LoadByUserID(userID);
      if (recentlyViewedItems.IsEmpty)
        return null;
      else
        return recentlyViewedItems[0];
    }
    
    
    

    #endregion

    #region IEnumerable<RecentlyViewedItem> Members

    public IEnumerator<RecentlyViewedItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new RecentlyViewedItem(row, this);
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
