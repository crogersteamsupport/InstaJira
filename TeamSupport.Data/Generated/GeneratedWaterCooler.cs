using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WaterCoolerItem : BaseItem
  {
    private WaterCooler _waterCooler;
    
    public WaterCoolerItem(DataRow row, WaterCooler waterCooler): base(row, waterCooler)
    {
      _waterCooler = waterCooler;
    }
	
    #region Properties
    
    public WaterCooler Collection
    {
      get { return _waterCooler; }
    }
        
    
    
    
    public int MessageID
    {
      get { return (int)Row["MessageID"]; }
    }
    

    
    public int? GroupFor
    {
      get { return Row["GroupFor"] != DBNull.Value ? (int?)Row["GroupFor"] : null; }
      set { Row["GroupFor"] = CheckValue("GroupFor", value); }
    }
    
    public int? ReplyTo
    {
      get { return Row["ReplyTo"] != DBNull.Value ? (int?)Row["ReplyTo"] : null; }
      set { Row["ReplyTo"] = CheckValue("ReplyTo", value); }
    }
    
    public string Message
    {
      get { return Row["Message"] != DBNull.Value ? (string)Row["Message"] : null; }
      set { Row["Message"] = CheckValue("Message", value); }
    }
    
    public string MessageType
    {
      get { return Row["MessageType"] != DBNull.Value ? (string)Row["MessageType"] : null; }
      set { Row["MessageType"] = CheckValue("MessageType", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime TimeStamp
    {
      get { return DateToLocal((DateTime)Row["TimeStamp"]); }
      set { Row["TimeStamp"] = CheckValue("TimeStamp", value); }
    }

    public DateTime TimeStampUtc
    {
      get { return (DateTime)Row["TimeStamp"]; }
    }
    

    #endregion
    
    
  }

  public partial class WaterCooler : BaseCollection, IEnumerable<WaterCoolerItem>
  {
    public WaterCooler(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "WaterCooler"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "MessageID"; }
    }



    public WaterCoolerItem this[int index]
    {
      get { return new WaterCoolerItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WaterCoolerItem waterCoolerItem);
    partial void AfterRowInsert(WaterCoolerItem waterCoolerItem);
    partial void BeforeRowEdit(WaterCoolerItem waterCoolerItem);
    partial void AfterRowEdit(WaterCoolerItem waterCoolerItem);
    partial void BeforeRowDelete(int messageID);
    partial void AfterRowDelete(int messageID);    

    partial void BeforeDBDelete(int messageID);
    partial void AfterDBDelete(int messageID);    

    #endregion

    #region Public Methods

    public WaterCoolerItemProxy[] GetWaterCoolerItemProxies()
    {
      List<WaterCoolerItemProxy> list = new List<WaterCoolerItemProxy>();

      foreach (WaterCoolerItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int messageID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WaterCooler] WHERE ([MessageID] = @MessageID);";
        deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);
        deleteCommand.Parameters["MessageID"].Value = messageID;

        BeforeDBDelete(messageID);
        BeforeRowDelete(messageID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(messageID);
        AfterDBDelete(messageID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("WaterCoolerSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[WaterCooler] SET     [UserID] = @UserID,    [OrganizationID] = @OrganizationID,    [TimeStamp] = @TimeStamp,    [GroupFor] = @GroupFor,    [ReplyTo] = @ReplyTo,    [Message] = @Message,    [MessageType] = @MessageType  WHERE ([MessageID] = @MessageID);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("GroupFor", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReplyTo", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Message", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MessageType", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[WaterCooler] (    [UserID],    [OrganizationID],    [TimeStamp],    [GroupFor],    [ReplyTo],    [Message],    [MessageType]) VALUES ( @UserID, @OrganizationID, @TimeStamp, @GroupFor, @ReplyTo, @Message, @MessageType); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("MessageType", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Message", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReplyTo", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("GroupFor", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WaterCooler] WHERE ([MessageID] = @MessageID);";
		deleteCommand.Parameters.Add("MessageID", SqlDbType.Int);

		try
		{
		  foreach (WaterCoolerItem waterCoolerItem in this)
		  {
			if (waterCoolerItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(waterCoolerItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = waterCoolerItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["MessageID"].AutoIncrement = false;
			  Table.Columns["MessageID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				waterCoolerItem.Row["MessageID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(waterCoolerItem);
			}
			else if (waterCoolerItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(waterCoolerItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = waterCoolerItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(waterCoolerItem);
			}
			else if (waterCoolerItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)waterCoolerItem.Row["MessageID", DataRowVersion.Original];
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

      foreach (WaterCoolerItem waterCoolerItem in this)
      {
        if (waterCoolerItem.Row.Table.Columns.Contains("CreatorID") && (int)waterCoolerItem.Row["CreatorID"] == 0) waterCoolerItem.Row["CreatorID"] = LoginUser.UserID;
        if (waterCoolerItem.Row.Table.Columns.Contains("ModifierID")) waterCoolerItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WaterCoolerItem FindByMessageID(int messageID)
    {
      foreach (WaterCoolerItem waterCoolerItem in this)
      {
        if (waterCoolerItem.MessageID == messageID)
        {
          return waterCoolerItem;
        }
      }
      return null;
    }

    public virtual WaterCoolerItem AddNewWaterCoolerItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("WaterCooler");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WaterCoolerItem(row, this);
    }
    
    public virtual void LoadByMessageID(int messageID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [MessageID], [UserID], [OrganizationID], [TimeStamp], [GroupFor], [ReplyTo], [Message], [MessageType] FROM [dbo].[WaterCooler] WHERE ([MessageID] = @MessageID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("MessageID", messageID);
        Fill(command);
      }
    }
    
    public static WaterCoolerItem GetWaterCoolerItem(LoginUser loginUser, int messageID)
    {
      WaterCooler waterCooler = new WaterCooler(loginUser);
      waterCooler.LoadByMessageID(messageID);
      if (waterCooler.IsEmpty)
        return null;
      else
        return waterCooler[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WaterCoolerItem> Members

    public IEnumerator<WaterCoolerItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WaterCoolerItem(row, this);
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
