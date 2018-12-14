using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ProductVersionsViewItem : BaseItem
  {
    private ProductVersionsView _productVersionsView;
    
    public ProductVersionsViewItem(DataRow row, ProductVersionsView productVersionsView): base(row, productVersionsView)
    {
      _productVersionsView = productVersionsView;
    }
	
    #region Properties
    
    public ProductVersionsView Collection
    {
      get { return _productVersionsView; }
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
    
    public int? ProductFamilyID
    {
      get { return Row["ProductFamilyID"] != DBNull.Value ? (int?)Row["ProductFamilyID"] : null; }
      set { Row["ProductFamilyID"] = CheckValue("ProductFamilyID", value); }
    }
    
    public string JiraProjectKey
    {
      get { return Row["JiraProjectKey"] != DBNull.Value ? (string)Row["JiraProjectKey"] : null; }
      set { Row["JiraProjectKey"] = CheckValue("JiraProjectKey", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public string ProductName
    {
      get { return (string)Row["ProductName"]; }
      set { Row["ProductName"] = CheckValue("ProductName", value); }
    }
    
    public string VersionStatus
    {
      get { return (string)Row["VersionStatus"]; }
      set { Row["VersionStatus"] = CheckValue("VersionStatus", value); }
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
    
    public int ProductVersionID
    {
      get { return (int)Row["ProductVersionID"]; }
      set { Row["ProductVersionID"] = CheckValue("ProductVersionID", value); }
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

  public partial class ProductVersionsView : BaseCollection, IEnumerable<ProductVersionsViewItem>
  {
    public ProductVersionsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ProductVersionsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ProductVersionID"; }
    }



    public ProductVersionsViewItem this[int index]
    {
      get { return new ProductVersionsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ProductVersionsViewItem productVersionsViewItem);
    partial void AfterRowInsert(ProductVersionsViewItem productVersionsViewItem);
    partial void BeforeRowEdit(ProductVersionsViewItem productVersionsViewItem);
    partial void AfterRowEdit(ProductVersionsViewItem productVersionsViewItem);
    partial void BeforeRowDelete(int productVersionID);
    partial void AfterRowDelete(int productVersionID);    

    partial void BeforeDBDelete(int productVersionID);
    partial void AfterDBDelete(int productVersionID);    

    #endregion

    #region Public Methods

    public ProductVersionsViewItemProxy[] GetProductVersionsViewItemProxies()
    {
      List<ProductVersionsViewItemProxy> list = new List<ProductVersionsViewItemProxy>();

      foreach (ProductVersionsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int productVersionID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ProductVersionsView] WHERE ([ProductVersionID] = @ProductVersionID);";
        deleteCommand.Parameters.Add("ProductVersionID", SqlDbType.Int);
        deleteCommand.Parameters["ProductVersionID"].Value = productVersionID;

        BeforeDBDelete(productVersionID);
        BeforeRowDelete(productVersionID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(productVersionID);
        AfterDBDelete(productVersionID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ProductVersionsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ProductVersionsView] SET     [ProductID] = @ProductID,    [ProductVersionStatusID] = @ProductVersionStatusID,    [VersionNumber] = @VersionNumber,    [ReleaseDate] = @ReleaseDate,    [IsReleased] = @IsReleased,    [Description] = @Description,    [ImportID] = @ImportID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [NeedsIndexing] = @NeedsIndexing,    [VersionStatus] = @VersionStatus,    [ProductName] = @ProductName,    [OrganizationID] = @OrganizationID,    [ProductFamilyID] = @ProductFamilyID,    [JiraProjectKey] = @JiraProjectKey  WHERE ([ProductVersionID] = @ProductVersionID);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("VersionStatus", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductName", SqlDbType.VarChar, 255);
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
		
		tempParameter = updateCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraProjectKey", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ProductVersionsView] (    [ProductVersionID],    [ProductID],    [ProductVersionStatusID],    [VersionNumber],    [ReleaseDate],    [IsReleased],    [Description],    [ImportID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [NeedsIndexing],    [VersionStatus],    [ProductName],    [OrganizationID],    [ProductFamilyID],    [JiraProjectKey]) VALUES ( @ProductVersionID, @ProductID, @ProductVersionStatusID, @VersionNumber, @ReleaseDate, @IsReleased, @Description, @ImportID, @DateCreated, @DateModified, @CreatorID, @ModifierID, @NeedsIndexing, @VersionStatus, @ProductName, @OrganizationID, @ProductFamilyID, @JiraProjectKey); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("JiraProjectKey", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("ProductName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("VersionStatus", SqlDbType.VarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("ProductVersionID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ProductVersionsView] WHERE ([ProductVersionID] = @ProductVersionID);";
		deleteCommand.Parameters.Add("ProductVersionID", SqlDbType.Int);

		try
		{
		  foreach (ProductVersionsViewItem productVersionsViewItem in this)
		  {
			if (productVersionsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(productVersionsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = productVersionsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ProductVersionID"].AutoIncrement = false;
			  Table.Columns["ProductVersionID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				productVersionsViewItem.Row["ProductVersionID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(productVersionsViewItem);
			}
			else if (productVersionsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(productVersionsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = productVersionsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(productVersionsViewItem);
			}
			else if (productVersionsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)productVersionsViewItem.Row["ProductVersionID", DataRowVersion.Original];
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

      foreach (ProductVersionsViewItem productVersionsViewItem in this)
      {
        if (productVersionsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)productVersionsViewItem.Row["CreatorID"] == 0) productVersionsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (productVersionsViewItem.Row.Table.Columns.Contains("ModifierID")) productVersionsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ProductVersionsViewItem FindByProductVersionID(int productVersionID)
    {
      foreach (ProductVersionsViewItem productVersionsViewItem in this)
      {
        if (productVersionsViewItem.ProductVersionID == productVersionID)
        {
          return productVersionsViewItem;
        }
      }
      return null;
    }

    public virtual ProductVersionsViewItem AddNewProductVersionsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ProductVersionsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ProductVersionsViewItem(row, this);
    }
    
    public virtual void LoadByProductVersionID(int productVersionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ProductVersionID], [ProductID], [ProductVersionStatusID], [VersionNumber], [ReleaseDate], [IsReleased], [Description], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [NeedsIndexing], [VersionStatus], [ProductName], [OrganizationID], [ProductFamilyID], [JiraProjectKey] FROM [dbo].[ProductVersionsView] WHERE ([ProductVersionID] = @ProductVersionID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ProductVersionID", productVersionID);
        Fill(command);
      }
    }
    
    public static ProductVersionsViewItem GetProductVersionsViewItem(LoginUser loginUser, int productVersionID)
    {
      ProductVersionsView productVersionsView = new ProductVersionsView(loginUser);
      productVersionsView.LoadByProductVersionID(productVersionID);
      if (productVersionsView.IsEmpty)
        return null;
      else
        return productVersionsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ProductVersionsViewItem> Members

    public IEnumerator<ProductVersionsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ProductVersionsViewItem(row, this);
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
