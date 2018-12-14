using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class OrganizationProduct : BaseItem
  {
    private OrganizationProducts _organizationProducts;
    
    public OrganizationProduct(DataRow row, OrganizationProducts organizationProducts): base(row, organizationProducts)
    {
      _organizationProducts = organizationProducts;
    }
	
    #region Properties
    
    public OrganizationProducts Collection
    {
      get { return _organizationProducts; }
    }
        
    
    
    
    public int OrganizationProductID
    {
      get { return (int)Row["OrganizationProductID"]; }
    }
    

    
    public int? ProductVersionID
    {
      get { return Row["ProductVersionID"] != DBNull.Value ? (int?)Row["ProductVersionID"] : null; }
      set { Row["ProductVersionID"] = CheckValue("ProductVersionID", value); }
    }
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }
    
    public int? ImportFileID
    {
      get { return Row["ImportFileID"] != DBNull.Value ? (int?)Row["ImportFileID"] : null; }
      set { Row["ImportFileID"] = CheckValue("ImportFileID", value); }
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
    
    public bool IsVisibleOnPortal
    {
      get { return (bool)Row["IsVisibleOnPortal"]; }
      set { Row["IsVisibleOnPortal"] = CheckValue("IsVisibleOnPortal", value); }
    }
    
    public int ProductID
    {
      get { return (int)Row["ProductID"]; }
      set { Row["ProductID"] = CheckValue("ProductID", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? SupportExpiration
    {
      get { return Row["SupportExpiration"] != DBNull.Value ? DateToLocal((DateTime?)Row["SupportExpiration"]) : null; }
      set { Row["SupportExpiration"] = CheckValue("SupportExpiration", value); }
    }

    public DateTime? SupportExpirationUtc
    {
      get { return Row["SupportExpiration"] != DBNull.Value ? (DateTime?)Row["SupportExpiration"] : null; }
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

  public partial class OrganizationProducts : BaseCollection, IEnumerable<OrganizationProduct>
  {
    public OrganizationProducts(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "OrganizationProducts"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationProductID"; }
    }



    public OrganizationProduct this[int index]
    {
      get { return new OrganizationProduct(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(OrganizationProduct organizationProduct);
    partial void AfterRowInsert(OrganizationProduct organizationProduct);
    partial void BeforeRowEdit(OrganizationProduct organizationProduct);
    partial void AfterRowEdit(OrganizationProduct organizationProduct);
    partial void BeforeRowDelete(int organizationProductID);
    partial void AfterRowDelete(int organizationProductID);    

    partial void BeforeDBDelete(int organizationProductID);
    partial void AfterDBDelete(int organizationProductID);    

    #endregion

    #region Public Methods

    public OrganizationProductProxy[] GetOrganizationProductProxies()
    {
      List<OrganizationProductProxy> list = new List<OrganizationProductProxy>();

      foreach (OrganizationProduct item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationProductID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationProducts] WHERE ([OrganizationProductID] = @OrganizationProductID);";
        deleteCommand.Parameters.Add("OrganizationProductID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationProductID"].Value = organizationProductID;

        BeforeDBDelete(organizationProductID);
        BeforeRowDelete(organizationProductID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(organizationProductID);
        AfterDBDelete(organizationProductID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("OrganizationProductsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[OrganizationProducts] SET     [OrganizationID] = @OrganizationID,    [ProductID] = @ProductID,    [ProductVersionID] = @ProductVersionID,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [SupportExpiration] = @SupportExpiration,    [ImportID] = @ImportID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [ImportFileID] = @ImportFileID  WHERE ([OrganizationProductID] = @OrganizationProductID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationProductID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SupportExpiration", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[OrganizationProducts] (    [OrganizationID],    [ProductID],    [ProductVersionID],    [IsVisibleOnPortal],    [SupportExpiration],    [ImportID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [ImportFileID]) VALUES ( @OrganizationID, @ProductID, @ProductVersionID, @IsVisibleOnPortal, @SupportExpiration, @ImportID, @DateCreated, @DateModified, @CreatorID, @ModifierID, @ImportFileID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportFileID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SupportExpiration", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductVersionID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationProducts] WHERE ([OrganizationProductID] = @OrganizationProductID);";
		deleteCommand.Parameters.Add("OrganizationProductID", SqlDbType.Int);

		try
		{
		  foreach (OrganizationProduct organizationProduct in this)
		  {
			if (organizationProduct.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(organizationProduct);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = organizationProduct.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationProductID"].AutoIncrement = false;
			  Table.Columns["OrganizationProductID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				organizationProduct.Row["OrganizationProductID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(organizationProduct);
			}
			else if (organizationProduct.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(organizationProduct);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = organizationProduct.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(organizationProduct);
			}
			else if (organizationProduct.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)organizationProduct.Row["OrganizationProductID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationProductID"].Value = id;
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

      foreach (OrganizationProduct organizationProduct in this)
      {
        if (organizationProduct.Row.Table.Columns.Contains("CreatorID") && (int)organizationProduct.Row["CreatorID"] == 0) organizationProduct.Row["CreatorID"] = LoginUser.UserID;
        if (organizationProduct.Row.Table.Columns.Contains("ModifierID")) organizationProduct.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public OrganizationProduct FindByOrganizationProductID(int organizationProductID)
    {
      foreach (OrganizationProduct organizationProduct in this)
      {
        if (organizationProduct.OrganizationProductID == organizationProductID)
        {
          return organizationProduct;
        }
      }
      return null;
    }

    public virtual OrganizationProduct AddNewOrganizationProduct()
    {
      if (Table.Columns.Count < 1) LoadColumns("OrganizationProducts");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new OrganizationProduct(row, this);
    }
    
    public virtual void LoadByOrganizationProductID(int organizationProductID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationProductID], [OrganizationID], [ProductID], [ProductVersionID], [IsVisibleOnPortal], [SupportExpiration], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID], [ImportFileID] FROM [dbo].[OrganizationProducts] WHERE ([OrganizationProductID] = @OrganizationProductID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationProductID", organizationProductID);
        Fill(command);
      }
    }
    
    public static OrganizationProduct GetOrganizationProduct(LoginUser loginUser, int organizationProductID)
    {
      OrganizationProducts organizationProducts = new OrganizationProducts(loginUser);
      organizationProducts.LoadByOrganizationProductID(organizationProductID);
      if (organizationProducts.IsEmpty)
        return null;
      else
        return organizationProducts[0];
    }
    
    
    

    #endregion

    #region IEnumerable<OrganizationProduct> Members

    public IEnumerator<OrganizationProduct> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new OrganizationProduct(row, this);
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
