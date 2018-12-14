using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class KnowledgeBaseCategory : BaseItem
  {
    private KnowledgeBaseCategories _knowledgeBaseCategories;
    
    public KnowledgeBaseCategory(DataRow row, KnowledgeBaseCategories knowledgeBaseCategories): base(row, knowledgeBaseCategories)
    {
      _knowledgeBaseCategories = knowledgeBaseCategories;
    }
	
    #region Properties
    
    public KnowledgeBaseCategories Collection
    {
      get { return _knowledgeBaseCategories; }
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
    
    public bool? VisibleOnPortal
    {
      get { return Row["VisibleOnPortal"] != DBNull.Value ? (bool?)Row["VisibleOnPortal"] : null; }
      set { Row["VisibleOnPortal"] = CheckValue("VisibleOnPortal", value); }
    }
    
    public int? ProductFamilyID
    {
      get { return Row["ProductFamilyID"] != DBNull.Value ? (int?)Row["ProductFamilyID"] : null; }
      set { Row["ProductFamilyID"] = CheckValue("ProductFamilyID", value); }
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

  public partial class KnowledgeBaseCategories : BaseCollection, IEnumerable<KnowledgeBaseCategory>
  {
    public KnowledgeBaseCategories(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "KnowledgeBaseCategories"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CategoryID"; }
    }



    public KnowledgeBaseCategory this[int index]
    {
      get { return new KnowledgeBaseCategory(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(KnowledgeBaseCategory knowledgeBaseCategory);
    partial void AfterRowInsert(KnowledgeBaseCategory knowledgeBaseCategory);
    partial void BeforeRowEdit(KnowledgeBaseCategory knowledgeBaseCategory);
    partial void AfterRowEdit(KnowledgeBaseCategory knowledgeBaseCategory);
    partial void BeforeRowDelete(int categoryID);
    partial void AfterRowDelete(int categoryID);    

    partial void BeforeDBDelete(int categoryID);
    partial void AfterDBDelete(int categoryID);    

    #endregion

    #region Public Methods

    public KnowledgeBaseCategoryProxy[] GetKnowledgeBaseCategoryProxies()
    {
      List<KnowledgeBaseCategoryProxy> list = new List<KnowledgeBaseCategoryProxy>();

      foreach (KnowledgeBaseCategory item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int categoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[KnowledgeBaseCategories] WHERE ([CategoryID] = @CategoryID);";
        deleteCommand.Parameters.Add("CategoryID", SqlDbType.Int);
        deleteCommand.Parameters["CategoryID"].Value = categoryID;

        BeforeDBDelete(categoryID);
        BeforeRowDelete(categoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(categoryID);
        AfterDBDelete(categoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("KnowledgeBaseCategoriesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[KnowledgeBaseCategories] SET     [ParentID] = @ParentID,    [CategoryName] = @CategoryName,    [CategoryDesc] = @CategoryDesc,    [OrganizationID] = @OrganizationID,    [Position] = @Position,    [VisibleOnPortal] = @VisibleOnPortal,    [ProductFamilyID] = @ProductFamilyID  WHERE ([CategoryID] = @CategoryID);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("VisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[KnowledgeBaseCategories] (    [ParentID],    [CategoryName],    [CategoryDesc],    [OrganizationID],    [Position],    [VisibleOnPortal],    [ProductFamilyID]) VALUES ( @ParentID, @CategoryName, @CategoryDesc, @OrganizationID, @Position, @VisibleOnPortal, @ProductFamilyID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("VisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[KnowledgeBaseCategories] WHERE ([CategoryID] = @CategoryID);";
		deleteCommand.Parameters.Add("CategoryID", SqlDbType.Int);

		try
		{
		  foreach (KnowledgeBaseCategory knowledgeBaseCategory in this)
		  {
			if (knowledgeBaseCategory.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(knowledgeBaseCategory);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = knowledgeBaseCategory.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CategoryID"].AutoIncrement = false;
			  Table.Columns["CategoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				knowledgeBaseCategory.Row["CategoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(knowledgeBaseCategory);
			}
			else if (knowledgeBaseCategory.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(knowledgeBaseCategory);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = knowledgeBaseCategory.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(knowledgeBaseCategory);
			}
			else if (knowledgeBaseCategory.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)knowledgeBaseCategory.Row["CategoryID", DataRowVersion.Original];
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

      foreach (KnowledgeBaseCategory knowledgeBaseCategory in this)
      {
        if (knowledgeBaseCategory.Row.Table.Columns.Contains("CreatorID") && (int)knowledgeBaseCategory.Row["CreatorID"] == 0) knowledgeBaseCategory.Row["CreatorID"] = LoginUser.UserID;
        if (knowledgeBaseCategory.Row.Table.Columns.Contains("ModifierID")) knowledgeBaseCategory.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public KnowledgeBaseCategory FindByCategoryID(int categoryID)
    {
      foreach (KnowledgeBaseCategory knowledgeBaseCategory in this)
      {
        if (knowledgeBaseCategory.CategoryID == categoryID)
        {
          return knowledgeBaseCategory;
        }
      }
      return null;
    }

    public virtual KnowledgeBaseCategory AddNewKnowledgeBaseCategory()
    {
      if (Table.Columns.Count < 1) LoadColumns("KnowledgeBaseCategories");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new KnowledgeBaseCategory(row, this);
    }
    
    public virtual void LoadByCategoryID(int categoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CategoryID], [ParentID], [CategoryName], [CategoryDesc], [OrganizationID], [Position], [VisibleOnPortal], [ProductFamilyID] FROM [dbo].[KnowledgeBaseCategories] WHERE ([CategoryID] = @CategoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CategoryID", categoryID);
        Fill(command);
      }
    }
    
    public static KnowledgeBaseCategory GetKnowledgeBaseCategory(LoginUser loginUser, int categoryID)
    {
      KnowledgeBaseCategories knowledgeBaseCategories = new KnowledgeBaseCategories(loginUser);
      knowledgeBaseCategories.LoadByCategoryID(categoryID);
      if (knowledgeBaseCategories.IsEmpty)
        return null;
      else
        return knowledgeBaseCategories[0];
    }
    
    
    

    #endregion

    #region IEnumerable<KnowledgeBaseCategory> Members

    public IEnumerator<KnowledgeBaseCategory> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new KnowledgeBaseCategory(row, this);
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
