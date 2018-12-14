using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class DeletedIndexItem : BaseItem
  {
    private DeletedIndexItems _deletedIndexItems;
    
    public DeletedIndexItem(DataRow row, DeletedIndexItems deletedIndexItems): base(row, deletedIndexItems)
    {
      _deletedIndexItems = deletedIndexItems;
    }
	
    #region Properties
    
    public DeletedIndexItems Collection
    {
      get { return _deletedIndexItems; }
    }
        
    
    
    
    public int DeletedIndexID
    {
      get { return (int)Row["DeletedIndexID"]; }
    }
    

    

    
    public ReferenceType RefType
    {
      get { return (ReferenceType)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateDeleted
    {
      get { return DateToLocal((DateTime)Row["DateDeleted"]); }
      set { Row["DateDeleted"] = CheckValue("DateDeleted", value); }
    }

    public DateTime DateDeletedUtc
    {
      get { return (DateTime)Row["DateDeleted"]; }
    }
    

    #endregion
    
    
  }

  public partial class DeletedIndexItems : BaseCollection, IEnumerable<DeletedIndexItem>
  {
    public DeletedIndexItems(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "DeletedIndexItems"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "DeletedIndexID"; }
    }



    public DeletedIndexItem this[int index]
    {
      get { return new DeletedIndexItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(DeletedIndexItem deletedIndexItem);
    partial void AfterRowInsert(DeletedIndexItem deletedIndexItem);
    partial void BeforeRowEdit(DeletedIndexItem deletedIndexItem);
    partial void AfterRowEdit(DeletedIndexItem deletedIndexItem);
    partial void BeforeRowDelete(int deletedIndexID);
    partial void AfterRowDelete(int deletedIndexID);    

    partial void BeforeDBDelete(int deletedIndexID);
    partial void AfterDBDelete(int deletedIndexID);    

    #endregion

    #region Public Methods

    public DeletedIndexItemProxy[] GetDeletedIndexItemProxies()
    {
      List<DeletedIndexItemProxy> list = new List<DeletedIndexItemProxy>();

      foreach (DeletedIndexItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int deletedIndexID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[DeletedIndexItems] WHERE ([DeletedIndexID] = @DeletedIndexID);";
        deleteCommand.Parameters.Add("DeletedIndexID", SqlDbType.Int);
        deleteCommand.Parameters["DeletedIndexID"].Value = deletedIndexID;

        BeforeDBDelete(deletedIndexID);
        BeforeRowDelete(deletedIndexID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(deletedIndexID);
        AfterDBDelete(deletedIndexID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("DeletedIndexItemsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[DeletedIndexItems] SET     [OrganizationID] = @OrganizationID,    [RefID] = @RefID,    [RefType] = @RefType,    [DateDeleted] = @DateDeleted  WHERE ([DeletedIndexID] = @DeletedIndexID);";

		
		tempParameter = updateCommand.Parameters.Add("DeletedIndexID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("DateDeleted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[DeletedIndexItems] (    [OrganizationID],    [RefID],    [RefType],    [DateDeleted]) VALUES ( @OrganizationID, @RefID, @RefType, @DateDeleted); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateDeleted", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[DeletedIndexItems] WHERE ([DeletedIndexID] = @DeletedIndexID);";
		deleteCommand.Parameters.Add("DeletedIndexID", SqlDbType.Int);

		try
		{
		  foreach (DeletedIndexItem deletedIndexItem in this)
		  {
			if (deletedIndexItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(deletedIndexItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = deletedIndexItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["DeletedIndexID"].AutoIncrement = false;
			  Table.Columns["DeletedIndexID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				deletedIndexItem.Row["DeletedIndexID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(deletedIndexItem);
			}
			else if (deletedIndexItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(deletedIndexItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = deletedIndexItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(deletedIndexItem);
			}
			else if (deletedIndexItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)deletedIndexItem.Row["DeletedIndexID", DataRowVersion.Original];
			  deleteCommand.Parameters["DeletedIndexID"].Value = id;
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

      foreach (DeletedIndexItem deletedIndexItem in this)
      {
        if (deletedIndexItem.Row.Table.Columns.Contains("CreatorID") && (int)deletedIndexItem.Row["CreatorID"] == 0) deletedIndexItem.Row["CreatorID"] = LoginUser.UserID;
        if (deletedIndexItem.Row.Table.Columns.Contains("ModifierID")) deletedIndexItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public DeletedIndexItem FindByDeletedIndexID(int deletedIndexID)
    {
      foreach (DeletedIndexItem deletedIndexItem in this)
      {
        if (deletedIndexItem.DeletedIndexID == deletedIndexID)
        {
          return deletedIndexItem;
        }
      }
      return null;
    }

    public virtual DeletedIndexItem AddNewDeletedIndexItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("DeletedIndexItems");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new DeletedIndexItem(row, this);
    }
    
    public virtual void LoadByDeletedIndexID(int deletedIndexID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [DeletedIndexID], [OrganizationID], [RefID], [RefType], [DateDeleted] FROM [dbo].[DeletedIndexItems] WHERE ([DeletedIndexID] = @DeletedIndexID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("DeletedIndexID", deletedIndexID);
        Fill(command);
      }
    }
    
    public static DeletedIndexItem GetDeletedIndexItem(LoginUser loginUser, int deletedIndexID)
    {
      DeletedIndexItems deletedIndexItems = new DeletedIndexItems(loginUser);
      deletedIndexItems.LoadByDeletedIndexID(deletedIndexID);
      if (deletedIndexItems.IsEmpty)
        return null;
      else
        return deletedIndexItems[0];
    }
    
    
    

    #endregion

    #region IEnumerable<DeletedIndexItem> Members

    public IEnumerator<DeletedIndexItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new DeletedIndexItem(row, this);
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
