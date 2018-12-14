using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ForumPermission : BaseItem
  {
    private ForumPermissions _forumPermissions;
    
    public ForumPermission(DataRow row, ForumPermissions forumPermissions): base(row, forumPermissions)
    {
      _forumPermissions = forumPermissions;
    }
	
    #region Properties
    
    public ForumPermissions Collection
    {
      get { return _forumPermissions; }
    }
        
    
    
    

    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int? FilterUserID
    {
      get { return Row["FilterUserID"] != DBNull.Value ? (int?)Row["FilterUserID"] : null; }
      set { Row["FilterUserID"] = CheckValue("FilterUserID", value); }
    }
    
    public int? FilterOrgID
    {
      get { return Row["FilterOrgID"] != DBNull.Value ? (int?)Row["FilterOrgID"] : null; }
      set { Row["FilterOrgID"] = CheckValue("FilterOrgID", value); }
    }
    

    
    public int CategoryID
    {
      get { return (int)Row["CategoryID"]; }
      set { Row["CategoryID"] = CheckValue("CategoryID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ForumPermissions : BaseCollection, IEnumerable<ForumPermission>
  {
    public ForumPermissions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ForumPermissions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CategoryID"; }
    }



    public ForumPermission this[int index]
    {
      get { return new ForumPermission(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ForumPermission forumPermission);
    partial void AfterRowInsert(ForumPermission forumPermission);
    partial void BeforeRowEdit(ForumPermission forumPermission);
    partial void AfterRowEdit(ForumPermission forumPermission);
    partial void BeforeRowDelete(int categoryID);
    partial void AfterRowDelete(int categoryID);    

    partial void BeforeDBDelete(int categoryID);
    partial void AfterDBDelete(int categoryID);    

    #endregion

    #region Public Methods

    public ForumPermissionProxy[] GetForumPermissionProxies()
    {
      List<ForumPermissionProxy> list = new List<ForumPermissionProxy>();

      foreach (ForumPermission item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int categoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ForumPermissions] WHERE ([CategoryID] = @CategoryID);";
        deleteCommand.Parameters.Add("CategoryID", SqlDbType.Int);
        deleteCommand.Parameters["CategoryID"].Value = categoryID;

        BeforeDBDelete(categoryID);
        BeforeRowDelete(categoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(categoryID);
        AfterDBDelete(categoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ForumPermissionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ForumPermissions] SET     [OrganizationID] = @OrganizationID,    [FilterUserID] = @FilterUserID,    [FilterOrgID] = @FilterOrgID  WHERE ([CategoryID] = @CategoryID);";

		
		tempParameter = updateCommand.Parameters.Add("CategoryID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("FilterUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FilterOrgID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ForumPermissions] (    [CategoryID],    [OrganizationID],    [FilterUserID],    [FilterOrgID]) VALUES ( @CategoryID, @OrganizationID, @FilterUserID, @FilterOrgID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("FilterOrgID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("FilterUserID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("CategoryID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ForumPermissions] WHERE ([CategoryID] = @CategoryID);";
		deleteCommand.Parameters.Add("CategoryID", SqlDbType.Int);

		try
		{
		  foreach (ForumPermission forumPermission in this)
		  {
			if (forumPermission.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(forumPermission);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = forumPermission.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CategoryID"].AutoIncrement = false;
			  Table.Columns["CategoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				forumPermission.Row["CategoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(forumPermission);
			}
			else if (forumPermission.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(forumPermission);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = forumPermission.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(forumPermission);
			}
			else if (forumPermission.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)forumPermission.Row["CategoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["CategoryID"].Value = id;
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

      foreach (ForumPermission forumPermission in this)
      {
        if (forumPermission.Row.Table.Columns.Contains("CreatorID") && (int)forumPermission.Row["CreatorID"] == 0) forumPermission.Row["CreatorID"] = LoginUser.UserID;
        if (forumPermission.Row.Table.Columns.Contains("ModifierID")) forumPermission.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ForumPermission FindByCategoryID(int categoryID)
    {
      foreach (ForumPermission forumPermission in this)
      {
        if (forumPermission.CategoryID == categoryID)
        {
          return forumPermission;
        }
      }
      return null;
    }

    public virtual ForumPermission AddNewForumPermission()
    {
      if (Table.Columns.Count < 1) LoadColumns("ForumPermissions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ForumPermission(row, this);
    }
    
    public virtual void LoadByCategoryID(int categoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CategoryID], [OrganizationID], [FilterUserID], [FilterOrgID] FROM [dbo].[ForumPermissions] WHERE ([CategoryID] = @CategoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CategoryID", categoryID);
        Fill(command);
      }
    }
    
    public static ForumPermission GetForumPermission(LoginUser loginUser, int categoryID)
    {
      ForumPermissions forumPermissions = new ForumPermissions(loginUser);
      forumPermissions.LoadByCategoryID(categoryID);
      if (forumPermissions.IsEmpty)
        return null;
      else
        return forumPermissions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ForumPermission> Members

    public IEnumerator<ForumPermission> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ForumPermission(row, this);
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
