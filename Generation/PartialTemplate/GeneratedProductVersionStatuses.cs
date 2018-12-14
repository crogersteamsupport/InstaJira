using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ProductVersionStatus : BaseItem
  {
    private ProductVersionStatuses _productVersionStatuses;
    
    public ProductVersionStatus(DataRow row, ProductVersionStatuses productVersionStatuses): base(row, productVersionStatuses)
    {
      _productVersionStatuses = productVersionStatuses;
    }
	
    #region Properties
    
    public ProductVersionStatuses Collection
    {
      get { return _productVersionStatuses; }
    }
        
    
    
    
    public int ProductVersionStatusID
    {
      get { return (int)Row["ProductVersionStatusID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
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
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public bool IsDiscontinued
    {
      get { return (bool)Row["IsDiscontinued"]; }
      set { Row["IsDiscontinued"] = CheckValue("IsDiscontinued", value); }
    }
    
    public bool IsShipping
    {
      get { return (bool)Row["IsShipping"]; }
      set { Row["IsShipping"] = CheckValue("IsShipping", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
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

  public partial class ProductVersionStatuses : BaseCollection, IEnumerable<ProductVersionStatus>
  {
    public ProductVersionStatuses(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ProductVersionStatuses"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ProductVersionStatusID"; }
    }



    public ProductVersionStatus this[int index]
    {
      get { return new ProductVersionStatus(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ProductVersionStatus productVersionStatus);
    partial void AfterRowInsert(ProductVersionStatus productVersionStatus);
    partial void BeforeRowEdit(ProductVersionStatus productVersionStatus);
    partial void AfterRowEdit(ProductVersionStatus productVersionStatus);
    partial void BeforeRowDelete(int productVersionStatusID);
    partial void AfterRowDelete(int productVersionStatusID);    

    partial void BeforeDBDelete(int productVersionStatusID);
    partial void AfterDBDelete(int productVersionStatusID);    

    #endregion

    #region Public Methods

    public ProductVersionStatusProxy[] GetProductVersionStatusProxies()
    {
      List<ProductVersionStatusProxy> list = new List<ProductVersionStatusProxy>();

      foreach (ProductVersionStatus item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int productVersionStatusID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ProductVersionStatuses] WHERE ([ProductVersionStatusID] = @ProductVersionStatusID);";
        deleteCommand.Parameters.Add("ProductVersionStatusID", SqlDbType.Int);
        deleteCommand.Parameters["ProductVersionStatusID"].Value = productVersionStatusID;

        BeforeDBDelete(productVersionStatusID);
        BeforeRowDelete(productVersionStatusID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(productVersionStatusID);
        AfterDBDelete(productVersionStatusID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ProductVersionStatusesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ProductVersionStatuses] SET     [Name] = @Name,    [Description] = @Description,    [Position] = @Position,    [IsShipping] = @IsShipping,    [IsDiscontinued] = @IsDiscontinued,    [OrganizationID] = @OrganizationID,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([ProductVersionStatusID] = @ProductVersionStatusID);";

		
		tempParameter = updateCommand.Parameters.Add("ProductVersionStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
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
		
		tempParameter = updateCommand.Parameters.Add("IsShipping", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsDiscontinued", SqlDbType.Bit, 1);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ProductVersionStatuses] (    [Name],    [Description],    [Position],    [IsShipping],    [IsDiscontinued],    [OrganizationID],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @Name, @Description, @Position, @IsShipping, @IsDiscontinued, @OrganizationID, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsDiscontinued", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsShipping", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1024);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ProductVersionStatuses] WHERE ([ProductVersionStatusID] = @ProductVersionStatusID);";
		deleteCommand.Parameters.Add("ProductVersionStatusID", SqlDbType.Int);

		try
		{
		  foreach (ProductVersionStatus productVersionStatus in this)
		  {
			if (productVersionStatus.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(productVersionStatus);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = productVersionStatus.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ProductVersionStatusID"].AutoIncrement = false;
			  Table.Columns["ProductVersionStatusID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				productVersionStatus.Row["ProductVersionStatusID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(productVersionStatus);
			}
			else if (productVersionStatus.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(productVersionStatus);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = productVersionStatus.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(productVersionStatus);
			}
			else if (productVersionStatus.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)productVersionStatus.Row["ProductVersionStatusID", DataRowVersion.Original];
			  deleteCommand.Parameters["ProductVersionStatusID"].Value = id;
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

      foreach (ProductVersionStatus productVersionStatus in this)
      {
        if (productVersionStatus.Row.Table.Columns.Contains("CreatorID") && (int)productVersionStatus.Row["CreatorID"] == 0) productVersionStatus.Row["CreatorID"] = LoginUser.UserID;
        if (productVersionStatus.Row.Table.Columns.Contains("ModifierID")) productVersionStatus.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ProductVersionStatus FindByProductVersionStatusID(int productVersionStatusID)
    {
      foreach (ProductVersionStatus productVersionStatus in this)
      {
        if (productVersionStatus.ProductVersionStatusID == productVersionStatusID)
        {
          return productVersionStatus;
        }
      }
      return null;
    }

    public virtual ProductVersionStatus AddNewProductVersionStatus()
    {
      if (Table.Columns.Count < 1) LoadColumns("ProductVersionStatuses");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ProductVersionStatus(row, this);
    }
    
    public virtual void LoadByProductVersionStatusID(int productVersionStatusID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ProductVersionStatusID], [Name], [Description], [Position], [IsShipping], [IsDiscontinued], [OrganizationID], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[ProductVersionStatuses] WHERE ([ProductVersionStatusID] = @ProductVersionStatusID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ProductVersionStatusID", productVersionStatusID);
        Fill(command);
      }
    }
    
    public static ProductVersionStatus GetProductVersionStatus(LoginUser loginUser, int productVersionStatusID)
    {
      ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(loginUser);
      productVersionStatuses.LoadByProductVersionStatusID(productVersionStatusID);
      if (productVersionStatuses.IsEmpty)
        return null;
      else
        return productVersionStatuses[0];
    }
    
    
     

    public void LoadByPosition(int organizationID, int position)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ProductVersionStatuses WHERE (OrganizationID = @OrganizationID) AND (Position = @Position)";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        command.Parameters.AddWithValue("Position", position);
        Fill(command);
      }
    }
    
    public void LoadAllPositions(int organizationID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT * FROM ProductVersionStatuses WHERE (OrganizationID = @OrganizationID) ORDER BY Position";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void ValidatePositions(int organizationID)
    {
      ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(LoginUser);
      productVersionStatuses.LoadAllPositions(organizationID);
      int i = 0;
      foreach (ProductVersionStatus productVersionStatus in productVersionStatuses)
      {
        productVersionStatus.Position = i;
        i++;
      }
      productVersionStatuses.Save();
    }    

    public void MovePositionUp(int productVersionStatusID)
    {
      ProductVersionStatuses types1 = new ProductVersionStatuses(LoginUser);
      types1.LoadByProductVersionStatusID(productVersionStatusID);
      if (types1.IsEmpty || types1[0].Position < 1) return;

      ProductVersionStatuses types2 = new ProductVersionStatuses(LoginUser);
      types2.LoadByPosition(types1[0].OrganizationID, types1[0].Position - 1);
      if (!types2.IsEmpty)
      {
        types2[0].Position = types2[0].Position + 1;
        types2.Save();
      }

      types1[0].Position = types1[0].Position - 1;
      types1.Save();
      ValidatePositions(LoginUser.OrganizationID);
    }
    
    public void MovePositionDown(int productVersionStatusID)
    {
      ProductVersionStatuses types1 = new ProductVersionStatuses(LoginUser);
      types1.LoadByProductVersionStatusID(productVersionStatusID);
      if (types1.IsEmpty || types1[0].Position >= GetMaxPosition(types1[0].OrganizationID)) return;

      ProductVersionStatuses types2 = new ProductVersionStatuses(LoginUser);
      types2.LoadByPosition(types1[0].OrganizationID, types1[0].Position + 1);
      if (!types2.IsEmpty)
      {
        types2[0].Position = types2[0].Position - 1;
        types2.Save();
      }

      types1[0].Position = types1[0].Position + 1;
      types1.Save();
	  
      ValidatePositions(LoginUser.OrganizationID);
    }


    public virtual int GetMaxPosition(int organizationID)
    {
      int position = -1;
      
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SELECT MAX(Position) FROM ProductVersionStatuses WHERE OrganizationID = @OrganizationID";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationID", organizationID);
        object o = ExecuteScalar(command);
        if (o == DBNull.Value) return -1;
        position = (int)o;
      }
      return position;
    }
    
    

    #endregion

    #region IEnumerable<ProductVersionStatus> Members

    public IEnumerator<ProductVersionStatus> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ProductVersionStatus(row, this);
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
