using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class AssetHistoryViewItem : BaseItem
  {
    private AssetHistoryView _assetHistoryView;
    
    public AssetHistoryViewItem(DataRow row, AssetHistoryView assetHistoryView): base(row, assetHistoryView)
    {
      _assetHistoryView = assetHistoryView;
    }
	
    #region Properties
    
    public AssetHistoryView Collection
    {
      get { return _assetHistoryView; }
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

  public partial class AssetHistoryView : BaseCollection, IEnumerable<AssetHistoryViewItem>
  {
    public AssetHistoryView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "AssetHistoryView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "HistoryID"; }
    }



    public AssetHistoryViewItem this[int index]
    {
      get { return new AssetHistoryViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(AssetHistoryViewItem assetHistoryViewItem);
    partial void AfterRowInsert(AssetHistoryViewItem assetHistoryViewItem);
    partial void BeforeRowEdit(AssetHistoryViewItem assetHistoryViewItem);
    partial void AfterRowEdit(AssetHistoryViewItem assetHistoryViewItem);
    partial void BeforeRowDelete(int historyID);
    partial void AfterRowDelete(int historyID);    

    partial void BeforeDBDelete(int historyID);
    partial void AfterDBDelete(int historyID);    

    #endregion

    #region Public Methods

    public AssetHistoryViewItemProxy[] GetAssetHistoryViewItemProxies()
    {
      List<AssetHistoryViewItemProxy> list = new List<AssetHistoryViewItemProxy>();

      foreach (AssetHistoryViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int historyID)
    {
      BeforeDBDelete(historyID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AssetHistoryView] WHERE ([HistoryID] = @HistoryID);";
        deleteCommand.Parameters.Add("HistoryID", SqlDbType.Int);
        deleteCommand.Parameters["HistoryID"].Value = historyID;

        BeforeRowDelete(historyID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(historyID);
      }
      AfterDBDelete(historyID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("AssetHistoryViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[AssetHistoryView] SET     [AssetID] = @AssetID,    [OrganizationID] = @OrganizationID,    [ActionTime] = @ActionTime,    [ActionDescription] = @ActionDescription,    [ShippedFrom] = @ShippedFrom,    [NameAssignedFrom] = @NameAssignedFrom,    [ShippedTo] = @ShippedTo,    [NameAssignedTo] = @NameAssignedTo,    [TrackingNumber] = @TrackingNumber,    [ShippingMethod] = @ShippingMethod,    [ReferenceNum] = @ReferenceNum,    [Comments] = @Comments,    [Actor] = @Actor,    [ActorName] = @ActorName,    [RefType] = @RefType,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [ModifierName] = @ModifierName,    [ShippedFromRefType] = @ShippedFromRefType  WHERE ([HistoryID] = @HistoryID);";

		
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[AssetHistoryView] (    [HistoryID],    [AssetID],    [OrganizationID],    [ActionTime],    [ActionDescription],    [ShippedFrom],    [NameAssignedFrom],    [ShippedTo],    [NameAssignedTo],    [TrackingNumber],    [ShippingMethod],    [ReferenceNum],    [Comments],    [DateCreated],    [Actor],    [ActorName],    [RefType],    [DateModified],    [ModifierID],    [ModifierName],    [ShippedFromRefType]) VALUES ( @HistoryID, @AssetID, @OrganizationID, @ActionTime, @ActionDescription, @ShippedFrom, @NameAssignedFrom, @ShippedTo, @NameAssignedTo, @TrackingNumber, @ShippingMethod, @ReferenceNum, @Comments, @DateCreated, @Actor, @ActorName, @RefType, @DateModified, @ModifierID, @ModifierName, @ShippedFromRefType); SET @Identity = SCOPE_IDENTITY();";

		
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[AssetHistoryView] WHERE ([HistoryID] = @HistoryID);";
		deleteCommand.Parameters.Add("HistoryID", SqlDbType.Int);

		try
		{
		  foreach (AssetHistoryViewItem assetHistoryViewItem in this)
		  {
			if (assetHistoryViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(assetHistoryViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = assetHistoryViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["HistoryID"].AutoIncrement = false;
			  Table.Columns["HistoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				assetHistoryViewItem.Row["HistoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(assetHistoryViewItem);
			}
			else if (assetHistoryViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(assetHistoryViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = assetHistoryViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(assetHistoryViewItem);
			}
			else if (assetHistoryViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)assetHistoryViewItem.Row["HistoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["HistoryID"].Value = id;
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

      foreach (AssetHistoryViewItem assetHistoryViewItem in this)
      {
        if (assetHistoryViewItem.Row.Table.Columns.Contains("CreatorID") && (int)assetHistoryViewItem.Row["CreatorID"] == 0) assetHistoryViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (assetHistoryViewItem.Row.Table.Columns.Contains("ModifierID")) assetHistoryViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public AssetHistoryViewItem FindByHistoryID(int historyID)
    {
      foreach (AssetHistoryViewItem assetHistoryViewItem in this)
      {
        if (assetHistoryViewItem.HistoryID == historyID)
        {
          return assetHistoryViewItem;
        }
      }
      return null;
    }

    public virtual AssetHistoryViewItem AddNewAssetHistoryViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("AssetHistoryView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new AssetHistoryViewItem(row, this);
    }
    
    public virtual void LoadByHistoryID(int historyID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [HistoryID], [AssetID], [OrganizationID], [ActionTime], [ActionDescription], [ShippedFrom], [NameAssignedFrom], [ShippedTo], [NameAssignedTo], [TrackingNumber], [ShippingMethod], [ReferenceNum], [Comments], [DateCreated], [Actor], [ActorName], [RefType], [DateModified], [ModifierID], [ModifierName], [ShippedFromRefType] FROM [dbo].[AssetHistoryView] WHERE ([HistoryID] = @HistoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("HistoryID", historyID);
        Fill(command);
      }
    }
    
    public static AssetHistoryViewItem GetAssetHistoryViewItem(LoginUser loginUser, int historyID)
    {
      AssetHistoryView assetHistoryView = new AssetHistoryView(loginUser);
      assetHistoryView.LoadByHistoryID(historyID);
      if (assetHistoryView.IsEmpty)
        return null;
      else
        return assetHistoryView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<AssetHistoryViewItem> Members

    public IEnumerator<AssetHistoryViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new AssetHistoryViewItem(row, this);
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
