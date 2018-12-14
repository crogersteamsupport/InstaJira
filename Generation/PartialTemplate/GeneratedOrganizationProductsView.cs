using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class OrganizationProductsViewItem : BaseItem
  {
    private OrganizationProductsView _organizationProductsView;
    
    public OrganizationProductsViewItem(DataRow row, OrganizationProductsView organizationProductsView): base(row, organizationProductsView)
    {
      _organizationProductsView = organizationProductsView;
    }
	
    #region Properties
    
    public OrganizationProductsView Collection
    {
      get { return _organizationProductsView; }
    }
        
    
    
    

    
    public string Product
    {
      get { return Row["Product"] != DBNull.Value ? (string)Row["Product"] : null; }
      set { Row["Product"] = CheckValue("Product", value); }
    }
    
    public string VersionStatus
    {
      get { return Row["VersionStatus"] != DBNull.Value ? (string)Row["VersionStatus"] : null; }
      set { Row["VersionStatus"] = CheckValue("VersionStatus", value); }
    }
    
    public bool? IsShipping
    {
      get { return Row["IsShipping"] != DBNull.Value ? (bool?)Row["IsShipping"] : null; }
      set { Row["IsShipping"] = CheckValue("IsShipping", value); }
    }
    
    public bool? IsDiscontinued
    {
      get { return Row["IsDiscontinued"] != DBNull.Value ? (bool?)Row["IsDiscontinued"] : null; }
      set { Row["IsDiscontinued"] = CheckValue("IsDiscontinued", value); }
    }
    
    public string VersionNumber
    {
      get { return Row["VersionNumber"] != DBNull.Value ? (string)Row["VersionNumber"] : null; }
      set { Row["VersionNumber"] = CheckValue("VersionNumber", value); }
    }
    
    public int? ProductVersionStatusID
    {
      get { return Row["ProductVersionStatusID"] != DBNull.Value ? (int?)Row["ProductVersionStatusID"] : null; }
      set { Row["ProductVersionStatusID"] = CheckValue("ProductVersionStatusID", value); }
    }
    
    public bool? IsReleased
    {
      get { return Row["IsReleased"] != DBNull.Value ? (bool?)Row["IsReleased"] : null; }
      set { Row["IsReleased"] = CheckValue("IsReleased", value); }
    }
    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string OrganizationName
    {
      get { return Row["OrganizationName"] != DBNull.Value ? (string)Row["OrganizationName"] : null; }
      set { Row["OrganizationName"] = CheckValue("OrganizationName", value); }
    }
    
    public int? ProductVersionID
    {
      get { return Row["ProductVersionID"] != DBNull.Value ? (int?)Row["ProductVersionID"] : null; }
      set { Row["ProductVersionID"] = CheckValue("ProductVersionID", value); }
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
    
    public int OrganizationProductID
    {
      get { return (int)Row["OrganizationProductID"]; }
      set { Row["OrganizationProductID"] = CheckValue("OrganizationProductID", value); }
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

  public partial class OrganizationProductsView : BaseCollection, IEnumerable<OrganizationProductsViewItem>
  {
    public OrganizationProductsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "OrganizationProductsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationProductID"; }
    }



    public OrganizationProductsViewItem this[int index]
    {
      get { return new OrganizationProductsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(OrganizationProductsViewItem organizationProductsViewItem);
    partial void AfterRowInsert(OrganizationProductsViewItem organizationProductsViewItem);
    partial void BeforeRowEdit(OrganizationProductsViewItem organizationProductsViewItem);
    partial void AfterRowEdit(OrganizationProductsViewItem organizationProductsViewItem);
    partial void BeforeRowDelete(int organizationProductID);
    partial void AfterRowDelete(int organizationProductID);    

    partial void BeforeDBDelete(int organizationProductID);
    partial void AfterDBDelete(int organizationProductID);    

    #endregion

    #region Public Methods

    public OrganizationProductsViewItemProxy[] GetOrganizationProductsViewItemProxies()
    {
      List<OrganizationProductsViewItemProxy> list = new List<OrganizationProductsViewItemProxy>();

      foreach (OrganizationProductsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationProductID)
    {
      BeforeDBDelete(organizationProductID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationProductsView] WHERE ([OrganizationProductID] = @OrganizationProductID);";
        deleteCommand.Parameters.Add("OrganizationProductID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationProductID"].Value = organizationProductID;

        BeforeRowDelete(organizationProductID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(organizationProductID);
      }
      AfterDBDelete(organizationProductID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("OrganizationProductsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[OrganizationProductsView] SET     [Product] = @Product,    [VersionStatus] = @VersionStatus,    [IsShipping] = @IsShipping,    [IsDiscontinued] = @IsDiscontinued,    [VersionNumber] = @VersionNumber,    [ProductVersionStatusID] = @ProductVersionStatusID,    [ReleaseDate] = @ReleaseDate,    [IsReleased] = @IsReleased,    [Description] = @Description,    [OrganizationID] = @OrganizationID,    [OrganizationName] = @OrganizationName,    [ProductID] = @ProductID,    [ProductVersionID] = @ProductVersionID,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [SupportExpiration] = @SupportExpiration,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID  WHERE ([OrganizationProductID] = @OrganizationProductID);";

		
		tempParameter = updateCommand.Parameters.Add("Product", SqlDbType.VarChar, 255);
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
		
		tempParameter = updateCommand.Parameters.Add("VersionNumber", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductVersionStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		
		tempParameter = updateCommand.Parameters.Add("OrganizationName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[OrganizationProductsView] (    [Product],    [VersionStatus],    [IsShipping],    [IsDiscontinued],    [VersionNumber],    [ProductVersionStatusID],    [ReleaseDate],    [IsReleased],    [Description],    [OrganizationProductID],    [OrganizationID],    [OrganizationName],    [ProductID],    [ProductVersionID],    [IsVisibleOnPortal],    [SupportExpiration],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID]) VALUES ( @Product, @VersionStatus, @IsShipping, @IsDiscontinued, @VersionNumber, @ProductVersionStatusID, @ReleaseDate, @IsReleased, @Description, @OrganizationProductID, @OrganizationID, @OrganizationName, @ProductID, @ProductVersionID, @IsVisibleOnPortal, @SupportExpiration, @DateCreated, @DateModified, @CreatorID, @ModifierID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("OrganizationName", SqlDbType.VarChar, 255);
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
		
		tempParameter = insertCommand.Parameters.Add("OrganizationProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		
		tempParameter = insertCommand.Parameters.Add("ProductVersionStatusID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("VersionNumber", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("VersionStatus", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Product", SqlDbType.VarChar, 255);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationProductsView] WHERE ([OrganizationProductID] = @OrganizationProductID);";
		deleteCommand.Parameters.Add("OrganizationProductID", SqlDbType.Int);

		try
		{
		  foreach (OrganizationProductsViewItem organizationProductsViewItem in this)
		  {
			if (organizationProductsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(organizationProductsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = organizationProductsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationProductID"].AutoIncrement = false;
			  Table.Columns["OrganizationProductID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				organizationProductsViewItem.Row["OrganizationProductID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(organizationProductsViewItem);
			}
			else if (organizationProductsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(organizationProductsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = organizationProductsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(organizationProductsViewItem);
			}
			else if (organizationProductsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)organizationProductsViewItem.Row["OrganizationProductID", DataRowVersion.Original];
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

      foreach (OrganizationProductsViewItem organizationProductsViewItem in this)
      {
        if (organizationProductsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)organizationProductsViewItem.Row["CreatorID"] == 0) organizationProductsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (organizationProductsViewItem.Row.Table.Columns.Contains("ModifierID")) organizationProductsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public OrganizationProductsViewItem FindByOrganizationProductID(int organizationProductID)
    {
      foreach (OrganizationProductsViewItem organizationProductsViewItem in this)
      {
        if (organizationProductsViewItem.OrganizationProductID == organizationProductID)
        {
          return organizationProductsViewItem;
        }
      }
      return null;
    }

    public virtual OrganizationProductsViewItem AddNewOrganizationProductsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("OrganizationProductsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new OrganizationProductsViewItem(row, this);
    }
    
    public virtual void LoadByOrganizationProductID(int organizationProductID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Product], [VersionStatus], [IsShipping], [IsDiscontinued], [VersionNumber], [ProductVersionStatusID], [ReleaseDate], [IsReleased], [Description], [OrganizationProductID], [OrganizationID], [OrganizationName], [ProductID], [ProductVersionID], [IsVisibleOnPortal], [SupportExpiration], [DateCreated], [DateModified], [CreatorID], [ModifierID] FROM [dbo].[OrganizationProductsView] WHERE ([OrganizationProductID] = @OrganizationProductID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationProductID", organizationProductID);
        Fill(command);
      }
    }
    
    public static OrganizationProductsViewItem GetOrganizationProductsViewItem(LoginUser loginUser, int organizationProductID)
    {
      OrganizationProductsView organizationProductsView = new OrganizationProductsView(loginUser);
      organizationProductsView.LoadByOrganizationProductID(organizationProductID);
      if (organizationProductsView.IsEmpty)
        return null;
      else
        return organizationProductsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<OrganizationProductsViewItem> Members

    public IEnumerator<OrganizationProductsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new OrganizationProductsViewItem(row, this);
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
