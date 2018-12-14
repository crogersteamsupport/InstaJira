using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AssetAssignmentsViewItem : BaseItem
  {
    private AssetAssignmentsView _assetAssignmentsView;
    
    public AssetAssignmentsViewItem(DataRow row, AssetAssignmentsView assetAssignmentsView): base(row, assetAssignmentsView)
    {
      _assetAssignmentsView = assetAssignmentsView;
    }
	
    #region Properties
    
    public AssetAssignmentsView Collection
    {
      get { return _assetAssignmentsView; }
    }
        
    
    public string NameAssignedFrom
    {
      get { return Row["NameAssignedFrom"] != DBNull.Value ? (string)Row["NameAssignedFrom"] : null; }
    }
    
    public string NameAssignedTo
    {
      get { return Row["NameAssignedTo"] != DBNull.Value ? (string)Row["NameAssignedTo"] : null; }
    }
    
    public string ActorName
    {
      get { return Row["ActorName"] != DBNull.Value ? (string)Row["ActorName"] : null; }
    }
    
    public string ModifierName
    {
      get { return Row["ModifierName"] != DBNull.Value ? (string)Row["ModifierName"] : null; }
    }
    
    
    

    
    public string ActionDescription
    {
      get { return Row["ActionDescription"] != DBNull.Value ? (string)Row["ActionDescription"] : null; }
      set { Row["ActionDescription"] = CheckValue("ActionDescription", value); }
    }
    
    public int? ShippedFrom
    {
      get { return Row["ShippedFrom"] != DBNull.Value ? (int?)Row["ShippedFrom"] : null; }
      set { Row["ShippedFrom"] = CheckValue("ShippedFrom", value); }
    }
    
    public int? ShippedTo
    {
      get { return Row["ShippedTo"] != DBNull.Value ? (int?)Row["ShippedTo"] : null; }
      set { Row["ShippedTo"] = CheckValue("ShippedTo", value); }
    }
    
    public string TrackingNumber
    {
      get { return Row["TrackingNumber"] != DBNull.Value ? (string)Row["TrackingNumber"] : null; }
      set { Row["TrackingNumber"] = CheckValue("TrackingNumber", value); }
    }
    
    public string ShippingMethod
    {
      get { return Row["ShippingMethod"] != DBNull.Value ? (string)Row["ShippingMethod"] : null; }
      set { Row["ShippingMethod"] = CheckValue("ShippingMethod", value); }
    }
    
    public string ReferenceNum
    {
      get { return Row["ReferenceNum"] != DBNull.Value ? (string)Row["ReferenceNum"] : null; }
      set { Row["ReferenceNum"] = CheckValue("ReferenceNum", value); }
    }
    
    public string Comments
    {
      get { return Row["Comments"] != DBNull.Value ? (string)Row["Comments"] : null; }
      set { Row["Comments"] = CheckValue("Comments", value); }
    }
    
    public int? Actor
    {
      get { return Row["Actor"] != DBNull.Value ? (int?)Row["Actor"] : null; }
      set { Row["Actor"] = CheckValue("Actor", value); }
    }
    
    public int? RefType
    {
      get { return Row["RefType"] != DBNull.Value ? (int?)Row["RefType"] : null; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int? ModifierID
    {
      get { return Row["ModifierID"] != DBNull.Value ? (int?)Row["ModifierID"] : null; }
      set { Row["ModifierID"] = CheckValue("ModifierID", value); }
    }
    
    public int? ShippedFromRefType
    {
      get { return Row["ShippedFromRefType"] != DBNull.Value ? (int?)Row["ShippedFromRefType"] : null; }
      set { Row["ShippedFromRefType"] = CheckValue("ShippedFromRefType", value); }
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
    
    public int HistoryID
    {
      get { return (int)Row["HistoryID"]; }
      set { Row["HistoryID"] = CheckValue("HistoryID", value); }
    }
    
    public int AssetAssignmentsID
    {
      get { return (int)Row["AssetAssignmentsID"]; }
      set { Row["AssetAssignmentsID"] = CheckValue("AssetAssignmentsID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? ActionTime
    {
      get { return Row["ActionTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["ActionTime"]) : null; }
      set { Row["ActionTime"] = CheckValue("ActionTime", value); }
    }

    public DateTime? ActionTimeUtc
    {
      get { return Row["ActionTime"] != DBNull.Value ? (DateTime?)Row["ActionTime"] : null; }
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

  public partial class AssetAssignmentsView : BaseCollection, IEnumerable<AssetAssignmentsViewItem>
  {
    public AssetAssignmentsView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AssetAssignmentsView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "AssetAssignmentsID"; }
    }



    public AssetAssignmentsViewItem this[int index]
    {
      get { return new AssetAssignmentsViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AssetAssignmentsViewItem assetAssignmentsViewItem);
    partial void AfterRowInsert(AssetAssignmentsViewItem assetAssignmentsViewItem);
    partial void BeforeRowEdit(AssetAssignmentsViewItem assetAssignmentsViewItem);
    partial void AfterRowEdit(AssetAssignmentsViewItem assetAssignmentsViewItem);
    partial void BeforeRowDelete(int assetAssignmentsID);
    partial void AfterRowDelete(int assetAssignmentsID);    

    partial void BeforeDBDelete(int assetAssignmentsID);
    partial void AfterDBDelete(int assetAssignmentsID);    

    #endregion

    #region Public Methods

    public AssetAssignmentsViewItemProxy[] GetAssetAssignmentsViewItemProxies()
    {
      List<AssetAssignmentsViewItemProxy> list = new List<AssetAssignmentsViewItemProxy>();

      foreach (AssetAssignmentsViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int assetAssignmentsID)
    {
      BeforeDBDelete(assetAssignmentsID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AssetAssignmentsView] WHERE ([AssetAssignmentsID] = @AssetAssignmentsID);";
        deleteCommand.Parameters.Add("AssetAssignmentsID", SqlDbType.Int);
        deleteCommand.Parameters["AssetAssignmentsID"].Value = assetAssignmentsID;

        BeforeRowDelete(assetAssignmentsID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(assetAssignmentsID);
      }
      AfterDBDelete(assetAssignmentsID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AssetAssignmentsViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[AssetAssignmentsView] SET     [HistoryID] = @HistoryID,    [AssetID] = @AssetID,    [OrganizationID] = @OrganizationID,    [ActionTime] = @ActionTime,    [ActionDescription] = @ActionDescription,    [ShippedFrom] = @ShippedFrom,    [NameAssignedFrom] = @NameAssignedFrom,    [ShippedTo] = @ShippedTo,    [NameAssignedTo] = @NameAssignedTo,    [TrackingNumber] = @TrackingNumber,    [ShippingMethod] = @ShippingMethod,    [ReferenceNum] = @ReferenceNum,    [Comments] = @Comments,    [Actor] = @Actor,    [ActorName] = @ActorName,    [RefType] = @RefType,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [ModifierName] = @ModifierName,    [ShippedFromRefType] = @ShippedFromRefType  WHERE ([AssetAssignmentsID] = @AssetAssignmentsID);";

		
		tempParameter = updateCommand.Parameters.Add("AssetAssignmentsID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("HistoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AssetID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ActionTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionDescription", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShippedFrom", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("NameAssignedFrom", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShippedTo", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("NameAssignedTo", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TrackingNumber", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShippingMethod", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReferenceNum", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Comments", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Actor", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActorName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ModifierName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ShippedFromRefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[AssetAssignmentsView] (    [AssetAssignmentsID],    [HistoryID],    [AssetID],    [OrganizationID],    [ActionTime],    [ActionDescription],    [ShippedFrom],    [NameAssignedFrom],    [ShippedTo],    [NameAssignedTo],    [TrackingNumber],    [ShippingMethod],    [ReferenceNum],    [Comments],    [DateCreated],    [Actor],    [ActorName],    [RefType],    [DateModified],    [ModifierID],    [ModifierName],    [ShippedFromRefType]) VALUES ( @AssetAssignmentsID, @HistoryID, @AssetID, @OrganizationID, @ActionTime, @ActionDescription, @ShippedFrom, @NameAssignedFrom, @ShippedTo, @NameAssignedTo, @TrackingNumber, @ShippingMethod, @ReferenceNum, @Comments, @DateCreated, @Actor, @ActorName, @RefType, @DateModified, @ModifierID, @ModifierName, @ShippedFromRefType); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ShippedFromRefType", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActorName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Actor", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Comments", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReferenceNum", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ShippingMethod", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TrackingNumber", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("NameAssignedTo", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ShippedTo", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("NameAssignedFrom", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ShippedFrom", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionDescription", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionTime", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("AssetID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("HistoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("AssetAssignmentsID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AssetAssignmentsView] WHERE ([AssetAssignmentsID] = @AssetAssignmentsID);";
		deleteCommand.Parameters.Add("AssetAssignmentsID", SqlDbType.Int);

		try
		{
		  foreach (AssetAssignmentsViewItem assetAssignmentsViewItem in this)
		  {
			if (assetAssignmentsViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(assetAssignmentsViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = assetAssignmentsViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["AssetAssignmentsID"].AutoIncrement = false;
			  Table.Columns["AssetAssignmentsID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				assetAssignmentsViewItem.Row["AssetAssignmentsID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(assetAssignmentsViewItem);
			}
			else if (assetAssignmentsViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(assetAssignmentsViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = assetAssignmentsViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(assetAssignmentsViewItem);
			}
			else if (assetAssignmentsViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)assetAssignmentsViewItem.Row["AssetAssignmentsID", DataRowVersion.Original];
			  deleteCommand.Parameters["AssetAssignmentsID"].Value = id;
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

      foreach (AssetAssignmentsViewItem assetAssignmentsViewItem in this)
      {
        if (assetAssignmentsViewItem.Row.Table.Columns.Contains("CreatorID") && (int)assetAssignmentsViewItem.Row["CreatorID"] == 0) assetAssignmentsViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (assetAssignmentsViewItem.Row.Table.Columns.Contains("ModifierID")) assetAssignmentsViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AssetAssignmentsViewItem FindByAssetAssignmentsID(int assetAssignmentsID)
    {
      foreach (AssetAssignmentsViewItem assetAssignmentsViewItem in this)
      {
        if (assetAssignmentsViewItem.AssetAssignmentsID == assetAssignmentsID)
        {
          return assetAssignmentsViewItem;
        }
      }
      return null;
    }

    public virtual AssetAssignmentsViewItem AddNewAssetAssignmentsViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("AssetAssignmentsView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AssetAssignmentsViewItem(row, this);
    }
    
    public virtual void LoadByAssetAssignmentsID(int assetAssignmentsID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [AssetAssignmentsID], [HistoryID], [AssetID], [OrganizationID], [ActionTime], [ActionDescription], [ShippedFrom], [NameAssignedFrom], [ShippedTo], [NameAssignedTo], [TrackingNumber], [ShippingMethod], [ReferenceNum], [Comments], [DateCreated], [Actor], [ActorName], [RefType], [DateModified], [ModifierID], [ModifierName], [ShippedFromRefType] FROM [dbo].[AssetAssignmentsView] WHERE ([AssetAssignmentsID] = @AssetAssignmentsID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("AssetAssignmentsID", assetAssignmentsID);
        Fill(command);
      }
    }
    
    public static AssetAssignmentsViewItem GetAssetAssignmentsViewItem(LoginUser loginUser, int assetAssignmentsID)
    {
      AssetAssignmentsView assetAssignmentsView = new AssetAssignmentsView(loginUser);
      assetAssignmentsView.LoadByAssetAssignmentsID(assetAssignmentsID);
      if (assetAssignmentsView.IsEmpty)
        return null;
      else
        return assetAssignmentsView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AssetAssignmentsViewItem> Members

    public IEnumerator<AssetAssignmentsViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AssetAssignmentsViewItem(row, this);
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
