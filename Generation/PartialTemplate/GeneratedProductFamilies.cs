using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ProductFamily : BaseItem
  {
    private ProductFamilies _productFamilies;
    
    public ProductFamily(DataRow row, ProductFamilies productFamilies): base(row, productFamilies)
    {
      _productFamilies = productFamilies;
    }
	
    #region Properties
    
    public ProductFamilies Collection
    {
      get { return _productFamilies; }
    }
        
    
    
    
    public int ProductFamilyID
    {
      get { return (int)Row["ProductFamilyID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    

    
    public int NeedsIndexing
    {
      get { return (int)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }
    
    public int ModifierID
    {
      get { return (int)Row["ModifierID"]; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
    }
    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    

    #endregion
    
    
  }

  public partial class ProductFamilies : BaseCollection, IEnumerable<ProductFamily>
  {
    public ProductFamilies(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ProductFamilies"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ProductFamilyID"; }
    }



    public ProductFamily this[int index]
    {
      get { return new ProductFamily(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ProductFamily productFamily);
    partial void AfterRowInsert(ProductFamily productFamily);
    partial void BeforeRowEdit(ProductFamily productFamily);
    partial void AfterRowEdit(ProductFamily productFamily);
    partial void BeforeRowDelete(int productFamilyID);
    partial void AfterRowDelete(int productFamilyID);    

    partial void BeforeDBDelete(int productFamilyID);
    partial void AfterDBDelete(int productFamilyID);    

    #endregion

    #region Public Methods

    public ProductFamilyProxy[] GetProductFamilyProxies()
    {
      List<ProductFamilyProxy> list = new List<ProductFamilyProxy>();

      foreach (ProductFamily item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int productFamilyID)
    {
      BeforeDBDelete(productFamilyID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ProductFamilies] WHERE ([ProductFamilyID] = @ProductFamilyID);";
        deleteCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int);
        deleteCommand.Parameters["ProductFamilyID"].Value = productFamilyID;

        BeforeRowDelete(productFamilyID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(productFamilyID);
      }
      AfterDBDelete(productFamilyID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ProductFamiliesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ProductFamilies] SET     [OrganizationID] = @OrganizationID,    [Name] = @Name,    [Description] = @Description,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [NeedsIndexing] = @NeedsIndexing  WHERE ([ProductFamilyID] = @ProductFamilyID);";

		
		tempParameter = updateCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ProductFamilies] (    [OrganizationID],    [Name],    [Description],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [NeedsIndexing]) VALUES ( @OrganizationID, @Name, @Description, @DateCreated, @DateModified, @CreatorID, @ModifierID, @NeedsIndexing); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.NVarChar, -1);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ProductFamilies] WHERE ([ProductFamilyID] = @ProductFamilyID);";
		deleteCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int);

		try
		{
		  foreach (ProductFamily productFamily in this)
		  {
			if (productFamily.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(productFamily);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = productFamily.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ProductFamilyID"].AutoIncrement = false;
			  Table.Columns["ProductFamilyID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				productFamily.Row["ProductFamilyID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(productFamily);
			}
			else if (productFamily.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(productFamily);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = productFamily.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(productFamily);
			}
			else if (productFamily.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)productFamily.Row["ProductFamilyID", DataRowVersion.Original];
			  deleteCommand.Parameters["ProductFamilyID"].Value = id;
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

      foreach (ProductFamily productFamily in this)
      {
        if (productFamily.Row.Table.Columns.Contains("CreatorID") && (int)productFamily.Row["CreatorID"] == 0) productFamily.Row["CreatorID"] = LoginUser.UserID;
        if (productFamily.Row.Table.Columns.Contains("ModifierID")) productFamily.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ProductFamily FindByProductFamilyID(int productFamilyID)
    {
      foreach (ProductFamily productFamily in this)
      {
        if (productFamily.ProductFamilyID == productFamilyID)
        {
          return productFamily;
        }
      }
      return null;
    }

    public virtual ProductFamily AddNewProductFamily()
    {
      if (Table.Columns.Count < 1) LoadColumns("ProductFamilies");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ProductFamily(row, this);
    }
    
    public virtual void LoadByProductFamilyID(int productFamilyID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ProductFamilyID], [OrganizationID], [Name], [Description], [DateCreated], [DateModified], [CreatorID], [ModifierID], [NeedsIndexing] FROM [dbo].[ProductFamilies] WHERE ([ProductFamilyID] = @ProductFamilyID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ProductFamilyID", productFamilyID);
        Fill(command);
      }
    }
    
    public static ProductFamily GetProductFamily(LoginUser loginUser, int productFamilyID)
    {
      ProductFamilies productFamilies = new ProductFamilies(loginUser);
      productFamilies.LoadByProductFamilyID(productFamilyID);
      if (productFamilies.IsEmpty)
        return null;
      else
        return productFamilies[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ProductFamily> Members

    public IEnumerator<ProductFamily> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ProductFamily(row, this);
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
