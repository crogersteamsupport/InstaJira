using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ProductVersion : BaseItem
  {
    private ProductVersions _productVersions;
    
    public ProductVersion(DataRow row, ProductVersions productVersions): base(row, productVersions)
    {
      _productVersions = productVersions;
    }
	
    #region Properties
    
    public ProductVersions Collection
    {
      get { return _productVersions; }
    }
        
    
    
    
    public int ProductVersionID
    {
      get { return (int)Row["ProductVersionID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }
    
    public string JiraProjectKey
    {
      get { return Row["JiraProjectKey"] != DBNull.Value ? (string)Row["JiraProjectKey"] : null; }
      set { Row["JiraProjectKey"] = CheckValue("JiraProjectKey", value); }
    }
    
    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
    }
    

    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
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
    
    public bool IsReleased
    {
      get { return (bool)Row["IsReleased"]; }
      set { Row["IsReleased"] = CheckValue("IsReleased", value); }
    }
    
    public string VersionNumber
    {
      get { return (string)Row["VersionNumber"]; }
      set { Row["VersionNumber"] = CheckValue("VersionNumber", value); }
    }
    
    public int ProductVersionStatusID
    {
      get { return (int)Row["ProductVersionStatusID"]; }
      set { Row["ProductVersionStatusID"] = CheckValue("ProductVersionStatusID", value); }
    }
    
    public int ProductID
    {
      get { return (int)Row["ProductID"]; }
      set { Row["ProductID"] = CheckValue("ProductID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? ReleaseDate
    {
      get { return Row["ReleaseDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["ReleaseDate"]) : null; }
      set { Row["ReleaseDate"] = CheckValue("ReleaseDate", value); }
    }

    public DateTime? ReleaseDateUtc
    {
      get { return Row["ReleaseDate"] != DBNull.Value ? (DateTime?)Row["ReleaseDate"] : null; }
    }
    

    
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

  public partial class ProductVersions : BaseCollection, IEnumerable<ProductVersion>
  {
    public ProductVersions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ProductVersions"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ProductVersionID"; }
    }



    public ProductVersion this[int index]
    {
      get { return new ProductVersion(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ProductVersion productVersion);
    partial void AfterRowInsert(ProductVersion productVersion);
    partial void BeforeRowEdit(ProductVersion productVersion);
    partial void AfterRowEdit(ProductVersion productVersion);
    partial void BeforeRowDelete(int productVersionID);
    partial void AfterRowDelete(int productVersionID);    

    partial void BeforeDBDelete(int productVersionID);
    partial void AfterDBDelete(int productVersionID);    

    #endregion

    #region Public Methods

    public ProductVersionProxy[] GetProductVersionProxies()
    {
      List<ProductVersionProxy> list = new List<ProductVersionProxy>();

      foreach (ProductVersion item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int productVersionID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ProductVersions] WHERE ([ProductVersionID] = @ProductVersionID);";
        deleteCommand.Parameters.Add("ProductVersionID", SqlDbType.Int);
        deleteCommand.Parameters["ProductVersionID"].Value = productVersionID;

        BeforeDBDelete(productVersionID);
        BeforeRowDelete(productVersionID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(productVersionID);
        AfterDBDelete(productVersionID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ProductVersionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ProductVersions] SET     [ProductID] = @ProductID,    [ProductVersionStatusID] = @ProductVersionStatusID,    [VersionNumber] = @VersionNumber,    [ReleaseDate] = @ReleaseDate,    [IsReleased] = @IsReleased,    [Description] = @Description,    [ImportID] = @ImportID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [NeedsIndexing] = @NeedsIndexing,    [JiraProjectKey] = @JiraProjectKey,    [ImportFileID] = @ImportFileID  WHERE ([ProductVersionID] = @ProductVersionID);";

		
		tempParameter = updateCommand.Parameters.Add("ProductVersionID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ProductVersionStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("VersionNumber", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReleaseDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsReleased", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
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
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraProjectKey", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ProductVersions] (    [ProductID],    [ProductVersionStatusID],    [VersionNumber],    [ReleaseDate],    [IsReleased],    [Description],    [ImportID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [NeedsIndexing],    [JiraProjectKey],    [ImportFileID]) VALUES ( @ProductID, @ProductVersionStatusID, @VersionNumber, @ReleaseDate, @IsReleased, @Description, @ImportID, @DateCreated, @DateModified, @CreatorID, @ModifierID, @NeedsIndexing, @JiraProjectKey, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraProjectKey", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsReleased", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReleaseDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("VersionNumber", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductVersionStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ProductVersions] WHERE ([ProductVersionID] = @ProductVersionID);";
		deleteCommand.Parameters.Add("ProductVersionID", SqlDbType.Int);

		try
		{
		  foreach (ProductVersion productVersion in this)
		  {
			if (productVersion.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(productVersion);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = productVersion.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ProductVersionID"].AutoIncrement = false;
			  Table.Columns["ProductVersionID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				productVersion.Row["ProductVersionID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(productVersion);
			}
			else if (productVersion.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(productVersion);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = productVersion.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(productVersion);
			}
			else if (productVersion.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)productVersion.Row["ProductVersionID", DataRowVersion.Original];
			  deleteCommand.Parameters["ProductVersionID"].Value = id;
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

      foreach (ProductVersion productVersion in this)
      {
        if (productVersion.Row.Table.Columns.Contains("CreatorID") && (int)productVersion.Row["CreatorID"] == 0) productVersion.Row["CreatorID"] = LoginUser.UserID;
        if (productVersion.Row.Table.Columns.Contains("ModifierID")) productVersion.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ProductVersion FindByProductVersionID(int productVersionID)
    {
      foreach (ProductVersion productVersion in this)
      {
        if (productVersion.ProductVersionID == productVersionID)
        {
          return productVersion;
        }
      }
      return null;
    }

    public virtual ProductVersion AddNewProductVersion()
    {
      if (Table.Columns.Count < 1) LoadColumns("ProductVersions");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ProductVersion(row, this);
    }
    
    public virtual void LoadByProductVersionID(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ProductVersionID], [ProductID], [ProductVersionStatusID], [VersionNumber], [ReleaseDate], [IsReleased], [Description], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [NeedsIndexing], [JiraProjectKey], [ImportFileID] FROM [dbo].[ProductVersions] WHERE ([ProductVersionID] = @ProductVersionID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ProductVersionID", productVersionID);
        Fill(command);
      }
    }
    
    public static ProductVersion GetProductVersion(LoginUser loginUser, int productVersionID)
    {
      ProductVersions productVersions = new ProductVersions(loginUser);
      productVersions.LoadByProductVersionID(productVersionID);
      if (productVersions.IsEmpty)
        return null;
      else
        return productVersions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ProductVersion> Members

    public IEnumerator<ProductVersion> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ProductVersion(row, this);
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
