using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AssetsViewItem : BaseItem
  {
    private AssetsView _assetsView;
    
    public AssetsViewItem(DataRow row, AssetsView assetsView): base(row, assetsView)
    {
      _assetsView = assetsView;
    }
	
    #region Properties
    
    public AssetsView Collection
    {
      get { return _assetsView; }
    }
        
    
    public string CreatorName
    {
      get { return Row["CreatorName"] != DBNull.Value ? (string)Row["CreatorName"] : null; }
    }
    
    public string ModifierName
    {
      get { return Row["ModifierName"] != DBNull.Value ? (string)Row["ModifierName"] : null; }
    }
    
    
    

    
    public int? ProductID
    {
      get { return Row["ProductID"] != DBNull.Value ? (int?)Row["ProductID"] : null; }
      set { Row["ProductID"] = CheckValue("ProductID", value); }
    }
    
    public string ProductName
    {
      get { return Row["ProductName"] != DBNull.Value ? (string)Row["ProductName"] : null; }
      set { Row["ProductName"] = CheckValue("ProductName", value); }
    }
    
    public int? ProductVersionID
    {
      get { return Row["ProductVersionID"] != DBNull.Value ? (int?)Row["ProductVersionID"] : null; }
      set { Row["ProductVersionID"] = CheckValue("ProductVersionID", value); }
    }
    
    public string ProductVersionNumber
    {
      get { return Row["ProductVersionNumber"] != DBNull.Value ? (string)Row["ProductVersionNumber"] : null; }
      set { Row["ProductVersionNumber"] = CheckValue("ProductVersionNumber", value); }
    }
    
    public string SerialNumber
    {
      get { return Row["SerialNumber"] != DBNull.Value ? (string)Row["SerialNumber"] : null; }
      set { Row["SerialNumber"] = CheckValue("SerialNumber", value); }
    }
    
    public string Name
    {
      get { return Row["Name"] != DBNull.Value ? (string)Row["Name"] : null; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public string Location
    {
      get { return Row["Location"] != DBNull.Value ? (string)Row["Location"] : null; }
      set { Row["Location"] = CheckValue("Location", value); }
    }
    
    public string Notes
    {
      get { return Row["Notes"] != DBNull.Value ? (string)Row["Notes"] : null; }
      set { Row["Notes"] = CheckValue("Notes", value); }
    }
    
    public int? CreatorID
    {
      get { return Row["CreatorID"] != DBNull.Value ? (int?)Row["CreatorID"] : null; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int? ModifierID
    {
      get { return Row["ModifierID"] != DBNull.Value ? (int?)Row["ModifierID"] : null; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int? SubPartOf
    {
      get { return Row["SubPartOf"] != DBNull.Value ? (int?)Row["SubPartOf"] : null; }
      set { Row["SubPartOf"] = CheckValue("SubPartOf", value); }
    }
    
    public string Status
    {
      get { return Row["Status"] != DBNull.Value ? (string)Row["Status"] : null; }
      set { Row["Status"] = CheckValue("Status", value); }
    }
    
    public string ImportID
    {
      get { return Row["ImportID"] != DBNull.Value ? (string)Row["ImportID"] : null; }
      set { Row["ImportID"] = CheckValue("ImportID", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int AssetID
    {
      get { return (int)Row["AssetID"]; }
      set { Row["AssetID"] = CheckValue("AssetID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? WarrantyExpiration
    {
      get { return Row["WarrantyExpiration"] != DBNull.Value ? DateToLocal((DateTime?)Row["WarrantyExpiration"]) : null; }
      set { Row["WarrantyExpiration"] = CheckValue("WarrantyExpiration", value); }
    }

    public DateTime? WarrantyExpirationUtc
    {
      get { return Row["WarrantyExpiration"] != DBNull.Value ? (DateTime?)Row["WarrantyExpiration"] : null; }
    }
    
    public DateTime? DateCreated
    {
      get { return Row["DateCreated"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateCreated"]) : null; }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime? DateCreatedUtc
    {
      get { return Row["DateCreated"] != DBNull.Value ? (DateTime?)Row["DateCreated"] : null; }
    }
    
    public DateTime? DateModified
    {
      get { return Row["DateModified"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModified"]) : null; }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime? DateModifiedUtc
    {
      get { return Row["DateModified"] != DBNull.Value ? (DateTime?)Row["DateModified"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class AssetsView : BaseCollection, IEnumerable<AssetsViewItem>
  {
    public AssetsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AssetsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AssetID"; }
    }



    public AssetsViewItem this[int index]
    {
      get { return new AssetsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AssetsViewItem assetsViewItem);
    partial void AfterRowInsert(AssetsViewItem assetsViewItem);
    partial void BeforeRowEdit(AssetsViewItem assetsViewItem);
    partial void AfterRowEdit(AssetsViewItem assetsViewItem);
    partial void BeforeRowDelete(int assetID);
    partial void AfterRowDelete(int assetID);    

    partial void BeforeDBDelete(int assetID);
    partial void AfterDBDelete(int assetID);    

    #endregion

    #region Public Methods

    public AssetsViewItemProxy[] GetAssetsViewItemProxies()
    {
      List<AssetsViewItemProxy> list = new List<AssetsViewItemProxy>();

      foreach (AssetsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int assetID)
    {
      BeforeDBDelete(assetID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AssetsView] WHERE ([AssetID] = @AssetID);";
        deleteCommand.Parameters.Add("AssetID", SqlDbType.Int);
        deleteCommand.Parameters["AssetID"].Value = assetID;

        BeforeRowDelete(assetID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(assetID);
      }
      AfterDBDelete(assetID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AssetsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[AssetsView] SET     [ProductID] = @ProductID,    [ProductName] = @ProductName,    [ProductVersionID] = @ProductVersionID,    [ProductVersionNumber] = @ProductVersionNumber,    [OrganizationID] = @OrganizationID,    [SerialNumber] = @SerialNumber,    [Name] = @Name,    [Location] = @Location,    [Notes] = @Notes,    [WarrantyExpiration] = @WarrantyExpiration,    [DateModified] = @DateModified,    [CreatorName] = @CreatorName,    [ModifierID] = @ModifierID,    [ModifierName] = @ModifierName,    [SubPartOf] = @SubPartOf,    [Status] = @Status,    [ImportID] = @ImportID  WHERE ([AssetID] = @AssetID);";

		
		tempParameter = updateCommand.Parameters.Add("AssetID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ProductName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductVersionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ProductVersionNumber", SqlDbType.VarChar, 50);
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
		
		tempParameter = updateCommand.Parameters.Add("SerialNumber", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Location", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Notes", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("WarrantyExpiration", SqlDbType.DateTime, 8);
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
		
		tempParameter = updateCommand.Parameters.Add("CreatorName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifierName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("SubPartOf", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Status", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[AssetsView] (    [AssetID],    [ProductID],    [ProductName],    [ProductVersionID],    [ProductVersionNumber],    [OrganizationID],    [SerialNumber],    [Name],    [Location],    [Notes],    [WarrantyExpiration],    [DateCreated],    [DateModified],    [CreatorID],    [CreatorName],    [ModifierID],    [ModifierName],    [SubPartOf],    [Status],    [ImportID]) VALUES ( @AssetID, @ProductID, @ProductName, @ProductVersionID, @ProductVersionNumber, @OrganizationID, @SerialNumber, @Name, @Location, @Notes, @WarrantyExpiration, @DateCreated, @DateModified, @CreatorID, @CreatorName, @ModifierID, @ModifierName, @SubPartOf, @Status, @ImportID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ImportID", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Status", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SubPartOf", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifierName", SqlDbType.VarChar, 201);
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
		
		tempParameter = insertCommand.Parameters.Add("CreatorName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("WarrantyExpiration", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Notes", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Location", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("SerialNumber", SqlDbType.VarChar, 500);
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
		
		tempParameter = insertCommand.Parameters.Add("ProductVersionNumber", SqlDbType.VarChar, 50);
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
		
		tempParameter = insertCommand.Parameters.Add("ProductName", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ProductID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("AssetID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AssetsView] WHERE ([AssetID] = @AssetID);";
		deleteCommand.Parameters.Add("AssetID", SqlDbType.Int);

		try
		{
		  foreach (AssetsViewItem assetsViewItem in this)
		  {
			if (assetsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(assetsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = assetsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AssetID"].AutoIncrement = false;
			  Table.Columns["AssetID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				assetsViewItem.Row["AssetID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(assetsViewItem);
			}
			else if (assetsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(assetsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = assetsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(assetsViewItem);
			}
			else if (assetsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)assetsViewItem.Row["AssetID", DataRowVersion.Original];
			  deleteCommand.Parameters["AssetID"].Value = id;
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

      foreach (AssetsViewItem assetsViewItem in this)
      {
        if (assetsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)assetsViewItem.Row["CreatorID"] == 0) assetsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (assetsViewItem.Row.Table.Columns.Contains("ModifierID")) assetsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AssetsViewItem FindByAssetID(int assetID)
    {
      foreach (AssetsViewItem assetsViewItem in this)
      {
        if (assetsViewItem.AssetID == assetID)
        {
          return assetsViewItem;
        }
      }
      return null;
    }

    public virtual AssetsViewItem AddNewAssetsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("AssetsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AssetsViewItem(row, this);
    }
    
    public virtual void LoadByAssetID(int assetID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AssetID], [ProductID], [ProductName], [ProductVersionID], [ProductVersionNumber], [OrganizationID], [SerialNumber], [Name], [Location], [Notes], [WarrantyExpiration], [DateCreated], [DateModified], [CreatorID], [CreatorName], [ModifierID], [ModifierName], [SubPartOf], [Status], [ImportID] FROM [dbo].[AssetsView] WHERE ([AssetID] = @AssetID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AssetID", assetID);
        Fill(command);
      }
    }
    
    public static AssetsViewItem GetAssetsViewItem(LoginUser loginUser, int assetID)
    {
      AssetsView assetsView = new AssetsView(loginUser);
      assetsView.LoadByAssetID(assetID);
      if (assetsView.IsEmpty)
        return null;
      else
        return assetsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AssetsViewItem> Members

    public IEnumerator<AssetsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AssetsViewItem(row, this);
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
