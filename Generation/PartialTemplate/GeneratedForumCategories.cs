using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ForumCategory : BaseItem
  {
    private ForumCategories _forumCategories;
    
    public ForumCategory(DataRow row, ForumCategories forumCategories): base(row, forumCategories)
    {
      _forumCategories = forumCategories;
    }
	
    #region Properties
    
    public ForumCategories Collection
    {
      get { return _forumCategories; }
    }
        
    
    
    
    public int CategoryID
    {
      get { return (int)Row["CategoryID"]; }
    }
    

    
    public string CategoryDesc
    {
      get { return Row["CategoryDesc"] != DBNull.Value ? (string)Row["CategoryDesc"] : null; }
      set { Row["CategoryDesc"] = CheckValue("CategoryDesc", value); }
    }
    
    public int? Position
    {
      get { return Row["Position"] != DBNull.Value ? (int?)Row["Position"] : null; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public int? TicketType
    {
      get { return Row["TicketType"] != DBNull.Value ? (int?)Row["TicketType"] : null; }
      set { Row["TicketType"] = CheckValue("TicketType", value); }
    }
    
    public int? GroupID
    {
      get { return Row["GroupID"] != DBNull.Value ? (int?)Row["GroupID"] : null; }
      set { Row["GroupID"] = CheckValue("GroupID", value); }
    }
    
    public int? ProductID
    {
      get { return Row["ProductID"] != DBNull.Value ? (int?)Row["ProductID"] : null; }
      set { Row["ProductID"] = CheckValue("ProductID", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public string CategoryName
    {
      get { return (string)Row["CategoryName"]; }
      set { Row["CategoryName"] = CheckValue("CategoryName", value); }
    }
    
    public int ParentID
    {
      get { return (int)Row["ParentID"]; }
      set { Row["ParentID"] = CheckValue("ParentID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ForumCategories : BaseCollection, IEnumerable<ForumCategory>
  {
    public ForumCategories(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ForumCategories"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CategoryID"; }
    }



    public ForumCategory this[int index]
    {
      get { return new ForumCategory(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ForumCategory forumCategory);
    partial void AfterRowInsert(ForumCategory forumCategory);
    partial void BeforeRowEdit(ForumCategory forumCategory);
    partial void AfterRowEdit(ForumCategory forumCategory);
    partial void BeforeRowDelete(int categoryID);
    partial void AfterRowDelete(int categoryID);    

    partial void BeforeDBDelete(int categoryID);
    partial void AfterDBDelete(int categoryID);    

    #endregion

    #region Public Methods

    public ForumCategoryProxy[] GetForumCategoryProxies()
    {
      List<ForumCategoryProxy> list = new List<ForumCategoryProxy>();

      foreach (ForumCategory item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int categoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ForumCategories] WHERE ([CategoryID] = @CategoryID);";
        deleteCommand.Parameters.Add("CategoryID", SqlDbType.Int);
        deleteCommand.Parameters["CategoryID"].Value = categoryID;

        BeforeDBDelete(categoryID);
        BeforeRowDelete(categoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(categoryID);
        AfterDBDelete(categoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ForumCategoriesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ForumCategories] SET     [ParentID] = @ParentID,    [CategoryName] = @CategoryName,    [CategoryDesc] = @CategoryDesc,    [OrganizationID] = @OrganizationID,    [Position] = @Position,    [TicketType] = @TicketType,    [GroupID] = @GroupID,    [ProductID] = @ProductID  WHERE ([CategoryID] = @CategoryID);";

		
		tempParameter = updateCommand.Parameters.Add("CategoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CategoryName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CategoryDesc", SqlDbType.NVarChar, 250);
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
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ForumCategories] (    [ParentID],    [CategoryName],    [CategoryDesc],    [OrganizationID],    [Position],    [TicketType],    [GroupID],    [ProductID]) VALUES ( @ParentID, @CategoryName, @CategoryDesc, @OrganizationID, @Position, @TicketType, @GroupID, @ProductID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("GroupID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("CategoryDesc", SqlDbType.NVarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CategoryName", SqlDbType.NVarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ForumCategories] WHERE ([CategoryID] = @CategoryID);";
		deleteCommand.Parameters.Add("CategoryID", SqlDbType.Int);

		try
		{
		  foreach (ForumCategory forumCategory in this)
		  {
			if (forumCategory.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(forumCategory);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = forumCategory.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CategoryID"].AutoIncrement = false;
			  Table.Columns["CategoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				forumCategory.Row["CategoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(forumCategory);
			}
			else if (forumCategory.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(forumCategory);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = forumCategory.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(forumCategory);
			}
			else if (forumCategory.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)forumCategory.Row["CategoryID", DataRowVersion.Original];
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

      foreach (ForumCategory forumCategory in this)
      {
        if (forumCategory.Row.Table.Columns.Contains("CreatorID") && (int)forumCategory.Row["CreatorID"] == 0) forumCategory.Row["CreatorID"] = LoginUser.UserID;
        if (forumCategory.Row.Table.Columns.Contains("ModifierID")) forumCategory.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ForumCategory FindByCategoryID(int categoryID)
    {
      foreach (ForumCategory forumCategory in this)
      {
        if (forumCategory.CategoryID == categoryID)
        {
          return forumCategory;
        }
      }
      return null;
    }

    public virtual ForumCategory AddNewForumCategory()
    {
      if (Table.Columns.Count < 1) LoadColumns("ForumCategories");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ForumCategory(row, this);
    }
    
    public virtual void LoadByCategoryID(int categoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CategoryID], [ParentID], [CategoryName], [CategoryDesc], [OrganizationID], [Position], [TicketType], [GroupID], [ProductID] FROM [dbo].[ForumCategories] WHERE ([CategoryID] = @CategoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CategoryID", categoryID);
        Fill(command);
      }
    }
    
    public static ForumCategory GetForumCategory(LoginUser loginUser, int categoryID)
    {
      ForumCategories forumCategories = new ForumCategories(loginUser);
      forumCategories.LoadByCategoryID(categoryID);
      if (forumCategories.IsEmpty)
        return null;
      else
        return forumCategories[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ForumCategory> Members

    public IEnumerator<ForumCategory> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ForumCategory(row, this);
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
