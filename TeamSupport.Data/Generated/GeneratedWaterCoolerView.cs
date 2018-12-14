using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WaterCoolerViewItem : BaseItem
  {
    private WaterCoolerView _waterCoolerView;
    
    public WaterCoolerViewItem(DataRow row, WaterCoolerView waterCoolerView): base(row, waterCoolerView)
    {
      _waterCoolerView = waterCoolerView;
    }
	
    #region Properties
    
    public WaterCoolerView Collection
    {
      get { return _waterCoolerView; }
    }
        
    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
    }
    
    
    

    
    public string Message
    {
      get { return Row["Message"] != DBNull.Value ? (string)Row["Message"] : null; }
      set { Row["Message"] = CheckValue("Message",value); }
    }
    
    public int? MessageParent
    {
      get { return Row["MessageParent"] != DBNull.Value ? (int?)Row["MessageParent"] : null; }
      set { Row["MessageParent"] = CheckValue("MessageParent",value); }
    }
    
    public int? RefType
    {
      get { return Row["RefType"] != DBNull.Value ? (int?)Row["RefType"] : null; }
      set { Row["RefType"] = CheckValue("RefType",value); }
    }
    
    public int? AttachmentID
    {
      get { return Row["AttachmentID"] != DBNull.Value ? (int?)Row["AttachmentID"] : null; }
      set { Row["AttachmentID"] = CheckValue("AttachmentID",value); }
    }
    

    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }
    
    public bool IsDeleted
    {
      get { return (bool)Row["IsDeleted"]; }
      set { Row["IsDeleted"] = CheckValue("IsDeleted",value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID",value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID",value); }
    }
    
    public int MessageID
    {
      get { return (int)Row["MessageID"]; }
      set { Row["MessageID"] = CheckValue("MessageID",value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime LastModified
    {
      get { return DateToLocal((DateTime)Row["LastModified"]); }
      set { Row["LastModified"] = CheckValue("LastModified",value); }
    }

    public DateTime LastModifiedUtc
    {
      get { return (DateTime)Row["LastModified"]; }
    }
    
    public DateTime TimeStamp
    {
      get { return DateToLocal((DateTime)Row["TimeStamp"]); }
      set { Row["TimeStamp"] = CheckValue("TimeStamp",value); }
    }

    public DateTime TimeStampUtc
    {
      get { return (DateTime)Row["TimeStamp"]; }
    }
    

    #endregion
    
    
  }

  public partial class WaterCoolerView : BaseCollection, IEnumerable<WaterCoolerViewItem>
  {
    public WaterCoolerView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
        get { return "NewWaterCoolerView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "MessageID"; }
    }



    public WaterCoolerViewItem this[int index]
    {
      get { return new WaterCoolerViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WaterCoolerViewItem waterCoolerViewItem);
    partial void AfterRowInsert(WaterCoolerViewItem waterCoolerViewItem);
    partial void BeforeRowEdit(WaterCoolerViewItem waterCoolerViewItem);
    partial void AfterRowEdit(WaterCoolerViewItem waterCoolerViewItem);
    partial void BeforeRowDelete(int messageID);
    partial void AfterRowDelete(int messageID);    

    partial void BeforeDBDelete(int messageID);
    partial void AfterDBDelete(int messageID);    

    #endregion

    #region Public Methods

    public WaterCoolerViewItemProxy[] GetWaterCoolerViewItemProxies()
    {
      List<WaterCoolerViewItemProxy> list = new List<WaterCoolerViewItemProxy>();

      foreach (WaterCoolerViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int messageID)
    {
      BeforeDBDelete(messageID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[NewWaterCoolerView] WHERE ([MessageID] = @MessageID);";
        deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);
        deleteCommand.Parameters["MessageID"].Value = messageID;

        BeforeRowDelete(messageID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(messageID);
      }
      AfterDBDelete(messageID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("WaterCoolerViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
        updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[NewWaterCoolerView] SET     [UserID] = @UserID,    [OrganizationID] = @OrganizationID,    [TimeStamp] = @TimeStamp,    [Message] = @Message,    [MessageParent] = @MessageParent,    [IsDeleted] = @IsDeleted,    [LastModified] = @LastModified,    [RefType] = @RefType,    [AttachmentID] = @AttachmentID,    [NeedsIndexing] = @NeedsIndexing,    [UserName] = @UserName  WHERE ([MessageID] = @MessageID);";

		
		tempParameter = updateCommand.Parameters.Add("MessageID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeStamp", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("Message", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MessageParent", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("AttachmentID", SqlDbType.Int, 4);
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

		tempParameter = updateCommand.Parameters.Add("UserName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[NewWaterCoolerView] (    [MessageID],    [UserID],    [OrganizationID],    [TimeStamp],    [Message],    [MessageParent],    [IsDeleted],    [LastModified],    [RefType],    [AttachmentID],    [NeedsIndexing],    [UserName]) VALUES ( @MessageID, @UserID, @OrganizationID, @TimeStamp, @Message, @MessageParent, @IsDeleted, @LastModified, @RefType, @AttachmentID, @NeedsIndexing, @UserName); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("UserName", SqlDbType.VarChar, 201);
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
		
		tempParameter = insertCommand.Parameters.Add("AttachmentID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MessageParent", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Message", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeStamp", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("MessageID", SqlDbType.Int, 4);
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
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[NewWaterCoolerView] WHERE ([MessageID] = @MessageID);";
		deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);

		try
		{
		  foreach (WaterCoolerViewItem waterCoolerViewItem in this)
		  {
			if (waterCoolerViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(waterCoolerViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = waterCoolerViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["MessageID"].AutoIncrement = false;
			  Table.Columns["MessageID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				waterCoolerViewItem.Row["MessageID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(waterCoolerViewItem);
			}
			else if (waterCoolerViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(waterCoolerViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = waterCoolerViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(waterCoolerViewItem);
			}
			else if (waterCoolerViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)waterCoolerViewItem.Row["MessageID", DataRowVersion.Original];
			  deleteCommand.Parameters["MessageID"].Value = id;
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

      foreach (WaterCoolerViewItem waterCoolerViewItem in this)
      {
        if (waterCoolerViewItem.Row.Table.Columns.Contains("CreatorID") && (int)waterCoolerViewItem.Row["CreatorID"] == 0) waterCoolerViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (waterCoolerViewItem.Row.Table.Columns.Contains("ModifierID")) waterCoolerViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WaterCoolerViewItem FindByMessageID(int messageID)
    {
      foreach (WaterCoolerViewItem waterCoolerViewItem in this)
      {
        if (waterCoolerViewItem.MessageID == messageID)
        {
          return waterCoolerViewItem;
        }
      }
      return null;
    }

    public virtual WaterCoolerViewItem AddNewWaterCoolerViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("WaterCoolerView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WaterCoolerViewItem(row, this);
    }

    public virtual void LoadByMessageID(int messageID)
    {
        using (SqlCommand command = new SqlCommand())
        {
            command.CommandText = "SET NOCOUNT OFF; SELECT [MessageID], [UserID], [OrganizationID], [TimeStamp], [Message], [MessageParent], [IsDeleted], [LastModified], [RefType], [AttachmentID], [NeedsIndexing], [UserName] FROM [dbo].[NewWaterCoolerView] WHERE ([MessageID] = @MessageID);";
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("MessageID", messageID);
            Fill(command);
        }
    }
    
    public static WaterCoolerViewItem GetWaterCoolerViewItem(LoginUser loginUser, int messageID)
    {
      WaterCoolerView waterCoolerView = new WaterCoolerView(loginUser);
      waterCoolerView.LoadByMessageID(messageID);
      if (waterCoolerView.IsEmpty)
        return null;
      else
        return waterCoolerView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WaterCoolerViewItem> Members

    public IEnumerator<WaterCoolerViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WaterCoolerViewItem(row, this);
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
