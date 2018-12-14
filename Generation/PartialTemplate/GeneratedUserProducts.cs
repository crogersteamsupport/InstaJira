using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class UserProduct : BaseItem
  {
    private UserProducts _userProducts;
    
    public UserProduct(DataRow row, UserProducts userProducts): base(row, userProducts)
    {
      _userProducts = userProducts;
    }
	
    #region Properties
    
    public UserProducts Collection
    {
      get { return _userProducts; }
    }
        
    
    
    
    public int UserProductID
    {
      get { return (int)Row["UserProductID"]; }
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
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
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

  public partial class UserProducts : BaseCollection, IEnumerable<UserProduct>
  {
    public UserProducts(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "UserProducts"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "UserProductID"; }
    }



    public UserProduct this[int index]
    {
      get { return new UserProduct(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(UserProduct userProduct);
    partial void AfterRowInsert(UserProduct userProduct);
    partial void BeforeRowEdit(UserProduct userProduct);
    partial void AfterRowEdit(UserProduct userProduct);
    partial void BeforeRowDelete(int userProductID);
    partial void AfterRowDelete(int userProductID);    

    partial void BeforeDBDelete(int userProductID);
    partial void AfterDBDelete(int userProductID);    

    #endregion

    #region Public Methods

    public UserProductProxy[] GetUserProductProxies()
    {
      List<UserProductProxy> list = new List<UserProductProxy>();

      foreach (UserProduct item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int userProductID)
    {
      BeforeDBDelete(userProductID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserProducts] WHERE ([UserProductID] = @UserProductID);";
        deleteCommand.Parameters.Add("UserProductID", SqlDbType.Int);
        deleteCommand.Parameters["UserProductID"].Value = userProductID;

        BeforeRowDelete(userProductID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(userProductID);
      }
      AfterDBDelete(userProductID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("UserProductsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[UserProducts] SET     [UserID] = @UserID,    [ProductID] = @ProductID,    [ProductVersionID] = @ProductVersionID,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [SupportExpiration] = @SupportExpiration,    [ImportID] = @ImportID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([UserProductID] = @UserProductID);";

		
		tempParameter = updateCommand.Parameters.Add("UserProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[UserProducts] (    [UserID],    [ProductID],    [ProductVersionID],    [IsVisibleOnPortal],    [SupportExpiration],    [ImportID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @UserID, @ProductID, @ProductVersionID, @IsVisibleOnPortal, @SupportExpiration, @ImportID, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[UserProducts] WHERE ([UserProductID] = @UserProductID);";
		deleteCommand.Parameters.Add("UserProductID", SqlDbType.Int);

		try
		{
		  foreach (UserProduct userProduct in this)
		  {
			if (userProduct.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(userProduct);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = userProduct.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["UserProductID"].AutoIncrement = false;
			  Table.Columns["UserProductID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				userProduct.Row["UserProductID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(userProduct);
			}
			else if (userProduct.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(userProduct);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = userProduct.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(userProduct);
			}
			else if (userProduct.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)userProduct.Row["UserProductID", DataRowVersion.Original];
			  deleteCommand.Parameters["UserProductID"].Value = id;
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

      foreach (UserProduct userProduct in this)
      {
        if (userProduct.Row.Table.Columns.Contains("CreatorID") && (int)userProduct.Row["CreatorID"] == 0) userProduct.Row["CreatorID"] = LoginUser.UserID;
        if (userProduct.Row.Table.Columns.Contains("ModifierID")) userProduct.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public UserProduct FindByUserProductID(int userProductID)
    {
      foreach (UserProduct userProduct in this)
      {
        if (userProduct.UserProductID == userProductID)
        {
          return userProduct;
        }
      }
      return null;
    }

    public virtual UserProduct AddNewUserProduct()
    {
      if (Table.Columns.Count < 1) LoadColumns("UserProducts");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new UserProduct(row, this);
    }
    
    public virtual void LoadByUserProductID(int userProductID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [UserProductID], [UserID], [ProductID], [ProductVersionID], [IsVisibleOnPortal], [SupportExpiration], [ImportID], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[UserProducts] WHERE ([UserProductID] = @UserProductID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("UserProductID", userProductID);
        Fill(command);
      }
    }
    
    public static UserProduct GetUserProduct(LoginUser loginUser, int userProductID)
    {
      UserProducts userProducts = new UserProducts(loginUser);
      userProducts.LoadByUserProductID(userProductID);
      if (userProducts.IsEmpty)
        return null;
      else
        return userProducts[0];
    }
    
    
    

    #endregion

    #region IEnumerable<UserProduct> Members

    public IEnumerator<UserProduct> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new UserProduct(row, this);
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
