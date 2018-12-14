using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CustomFieldCategory : BaseItem
  {
    private CustomFieldCategories _customFieldCategories;
    
    public CustomFieldCategory(DataRow row, CustomFieldCategories customFieldCategories): base(row, customFieldCategories)
    {
      _customFieldCategories = customFieldCategories;
    }
	
    #region Properties
    
    public CustomFieldCategories Collection
    {
      get { return _customFieldCategories; }
    }
        
    
    
    
    public int CustomFieldCategoryID
    {
      get { return (int)Row["CustomFieldCategoryID"]; }
    }
    

    
    public int? AuxID
    {
      get { return Row["AuxID"] != DBNull.Value ? (int?)Row["AuxID"] : null; }
      set { Row["AuxID"] = CheckValue("AuxID", value); }
    }
    

    
    public ReferenceType RefType
    {
      get { return (ReferenceType)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public string Category
    {
      get { return (string)Row["Category"]; }
      set { Row["Category"] = CheckValue("Category", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CustomFieldCategories : BaseCollection, IEnumerable<CustomFieldCategory>
  {
    public CustomFieldCategories(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CustomFieldCategories"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CustomFieldCategoryID"; }
    }



    public CustomFieldCategory this[int index]
    {
      get { return new CustomFieldCategory(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CustomFieldCategory customFieldCategory);
    partial void AfterRowInsert(CustomFieldCategory customFieldCategory);
    partial void BeforeRowEdit(CustomFieldCategory customFieldCategory);
    partial void AfterRowEdit(CustomFieldCategory customFieldCategory);
    partial void BeforeRowDelete(int customFieldCategoryID);
    partial void AfterRowDelete(int customFieldCategoryID);    

    partial void BeforeDBDelete(int customFieldCategoryID);
    partial void AfterDBDelete(int customFieldCategoryID);    

    #endregion

    #region Public Methods

    public CustomFieldCategoryProxy[] GetCustomFieldCategoryProxies()
    {
      List<CustomFieldCategoryProxy> list = new List<CustomFieldCategoryProxy>();

      foreach (CustomFieldCategory item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int customFieldCategoryID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomFieldCategories] WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID);";
        deleteCommand.Parameters.Add("CustomFieldCategoryID", SqlDbType.Int);
        deleteCommand.Parameters["CustomFieldCategoryID"].Value = customFieldCategoryID;

        BeforeDBDelete(customFieldCategoryID);
        BeforeRowDelete(customFieldCategoryID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(customFieldCategoryID);
        AfterDBDelete(customFieldCategoryID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CustomFieldCategoriesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CustomFieldCategories] SET     [OrganizationID] = @OrganizationID,    [Category] = @Category,    [Position] = @Position,    [RefType] = @RefType,    [AuxID] = @AuxID  WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID);";

		
		tempParameter = updateCommand.Parameters.Add("CustomFieldCategoryID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Category", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("AuxID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CustomFieldCategories] (    [OrganizationID],    [Category],    [Position],    [RefType],    [AuxID]) VALUES ( @OrganizationID, @Category, @Position, @RefType, @AuxID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("AuxID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Category", SqlDbType.VarChar, 250);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CustomFieldCategories] WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID);";
		deleteCommand.Parameters.Add("CustomFieldCategoryID", SqlDbType.Int);

		try
		{
		  foreach (CustomFieldCategory customFieldCategory in this)
		  {
			if (customFieldCategory.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(customFieldCategory);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = customFieldCategory.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CustomFieldCategoryID"].AutoIncrement = false;
			  Table.Columns["CustomFieldCategoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				customFieldCategory.Row["CustomFieldCategoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(customFieldCategory);
			}
			else if (customFieldCategory.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(customFieldCategory);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = customFieldCategory.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(customFieldCategory);
			}
			else if (customFieldCategory.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)customFieldCategory.Row["CustomFieldCategoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["CustomFieldCategoryID"].Value = id;
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

      foreach (CustomFieldCategory customFieldCategory in this)
      {
        if (customFieldCategory.Row.Table.Columns.Contains("CreatorID") && (int)customFieldCategory.Row["CreatorID"] == 0) customFieldCategory.Row["CreatorID"] = LoginUser.UserID;
        if (customFieldCategory.Row.Table.Columns.Contains("ModifierID")) customFieldCategory.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CustomFieldCategory FindByCustomFieldCategoryID(int customFieldCategoryID)
    {
      foreach (CustomFieldCategory customFieldCategory in this)
      {
        if (customFieldCategory.CustomFieldCategoryID == customFieldCategoryID)
        {
          return customFieldCategory;
        }
      }
      return null;
    }

    public virtual CustomFieldCategory AddNewCustomFieldCategory()
    {
      if (Table.Columns.Count < 1) LoadColumns("CustomFieldCategories");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CustomFieldCategory(row, this);
    }
    
    public virtual void LoadByCustomFieldCategoryID(int customFieldCategoryID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CustomFieldCategoryID], [OrganizationID], [Category], [Position], [RefType], [AuxID] FROM [dbo].[CustomFieldCategories] WHERE ([CustomFieldCategoryID] = @CustomFieldCategoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CustomFieldCategoryID", customFieldCategoryID);
        Fill(command);
      }
    }
    
    public static CustomFieldCategory GetCustomFieldCategory(LoginUser loginUser, int customFieldCategoryID)
    {
      CustomFieldCategories customFieldCategories = new CustomFieldCategories(loginUser);
      customFieldCategories.LoadByCustomFieldCategoryID(customFieldCategoryID);
      if (customFieldCategories.IsEmpty)
        return null;
      else
        return customFieldCategories[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CustomFieldCategory> Members

    public IEnumerator<CustomFieldCategory> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CustomFieldCategory(row, this);
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
