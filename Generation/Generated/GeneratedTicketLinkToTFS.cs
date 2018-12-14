using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketLinkToTFSItem : BaseItem
  {
    private TicketLinkToTFS _ticketLinkToTFS;
    
    public TicketLinkToTFSItem(DataRow row, TicketLinkToTFS ticketLinkToTFS): base(row, ticketLinkToTFS)
    {
      _ticketLinkToTFS = ticketLinkToTFS;
    }
	
    #region Properties
    
    public TicketLinkToTFS Collection
    {
      get { return _ticketLinkToTFS; }
    }
        
    
    
    
    public int id
    {
      get { return (int)Row["id"]; }
    }
    

    
    public int? TFSID
    {
      get { return Row["TFSID"] != DBNull.Value ? (int?)Row["TFSID"] : null; }
      set { Row["TFSID"] = CheckValue("TFSID", value); }
    }
    
    public string TFSTitle
    {
      get { return Row["TFSTitle"] != DBNull.Value ? (string)Row["TFSTitle"] : null; }
      set { Row["TFSTitle"] = CheckValue("TFSTitle", value); }
    }
    
    public string TFSURL
    {
      get { return Row["TFSURL"] != DBNull.Value ? (string)Row["TFSURL"] : null; }
      set { Row["TFSURL"] = CheckValue("TFSURL", value); }
    }
    
    public string TFSState
    {
      get { return Row["TFSState"] != DBNull.Value ? (string)Row["TFSState"] : null; }
      set { Row["TFSState"] = CheckValue("TFSState", value); }
    }
    
    public int? CreatorID
    {
      get { return Row["CreatorID"] != DBNull.Value ? (int?)Row["CreatorID"] : null; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public int? CrmLinkID
    {
      get { return Row["CrmLinkID"] != DBNull.Value ? (int?)Row["CrmLinkID"] : null; }
      set { Row["CrmLinkID"] = CheckValue("CrmLinkID", value); }
    }
    

    
    public bool SyncWithTFS
    {
      get { return (bool)Row["SyncWithTFS"]; }
      set { Row["SyncWithTFS"] = CheckValue("SyncWithTFS", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateModifiedByTFSSync
    {
      get { return Row["DateModifiedByTFSSync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedByTFSSync"]) : null; }
      set { Row["DateModifiedByTFSSync"] = CheckValue("DateModifiedByTFSSync", value); }
    }

    public DateTime? DateModifiedByTFSSyncUtc
    {
      get { return Row["DateModifiedByTFSSync"] != DBNull.Value ? (DateTime?)Row["DateModifiedByTFSSync"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class TicketLinkToTFS : BaseCollection, IEnumerable<TicketLinkToTFSItem>
  {
    public TicketLinkToTFS(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketLinkToTFS"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "id"; }
    }



    public TicketLinkToTFSItem this[int index]
    {
      get { return new TicketLinkToTFSItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketLinkToTFSItem ticketLinkToTFSItem);
    partial void AfterRowInsert(TicketLinkToTFSItem ticketLinkToTFSItem);
    partial void BeforeRowEdit(TicketLinkToTFSItem ticketLinkToTFSItem);
    partial void AfterRowEdit(TicketLinkToTFSItem ticketLinkToTFSItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public TicketLinkToTFSItemProxy[] GetTicketLinkToTFSItemProxies()
    {
      List<TicketLinkToTFSItemProxy> list = new List<TicketLinkToTFSItemProxy>();

      foreach (TicketLinkToTFSItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketLinkToTFS] WHERE ([id] = @id);";
        deleteCommand.Parameters.Add("id", SqlDbType.Int);
        deleteCommand.Parameters["id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketLinkToTFSSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketLinkToTFS] SET     [TicketID] = @TicketID,    [DateModifiedByTFSSync] = @DateModifiedByTFSSync,    [SyncWithTFS] = @SyncWithTFS,    [TFSID] = @TFSID,    [TFSTitle] = @TFSTitle,    [TFSURL] = @TFSURL,    [TFSState] = @TFSState,    [CrmLinkID] = @CrmLinkID  WHERE ([id] = @id);";

		
		tempParameter = updateCommand.Parameters.Add("id", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedByTFSSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SyncWithTFS", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TFSID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TFSTitle", SqlDbType.NVarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TFSURL", SqlDbType.NVarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TFSState", SqlDbType.NVarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CrmLinkID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketLinkToTFS] (    [TicketID],    [DateModifiedByTFSSync],    [SyncWithTFS],    [TFSID],    [TFSTitle],    [TFSURL],    [TFSState],    [CreatorID],    [CrmLinkID]) VALUES ( @TicketID, @DateModifiedByTFSSync, @SyncWithTFS, @TFSID, @TFSTitle, @TFSURL, @TFSState, @CreatorID, @CrmLinkID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("CrmLinkID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("TFSState", SqlDbType.NVarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TFSURL", SqlDbType.NVarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TFSTitle", SqlDbType.NVarChar, 256);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TFSID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SyncWithTFS", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedByTFSSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketLinkToTFS] WHERE ([id] = @id);";
		deleteCommand.Parameters.Add("id", SqlDbType.Int);

		try
		{
		  foreach (TicketLinkToTFSItem ticketLinkToTFSItem in this)
		  {
			if (ticketLinkToTFSItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketLinkToTFSItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketLinkToTFSItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["id"].AutoIncrement = false;
			  Table.Columns["id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketLinkToTFSItem.Row["id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketLinkToTFSItem);
			}
			else if (ticketLinkToTFSItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketLinkToTFSItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketLinkToTFSItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketLinkToTFSItem);
			}
			else if (ticketLinkToTFSItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketLinkToTFSItem.Row["id", DataRowVersion.Original];
			  deleteCommand.Parameters["id"].Value = id;
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

      foreach (TicketLinkToTFSItem ticketLinkToTFSItem in this)
      {
        if (ticketLinkToTFSItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketLinkToTFSItem.Row["CreatorID"] == 0) ticketLinkToTFSItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketLinkToTFSItem.Row.Table.Columns.Contains("ModifierID")) ticketLinkToTFSItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketLinkToTFSItem FindByid(int id)
    {
      foreach (TicketLinkToTFSItem ticketLinkToTFSItem in this)
      {
        if (ticketLinkToTFSItem.id == id)
        {
          return ticketLinkToTFSItem;
        }
      }
      return null;
    }

    public virtual TicketLinkToTFSItem AddNewTicketLinkToTFSItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketLinkToTFS");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketLinkToTFSItem(row, this);
    }
    
    public virtual void LoadByid(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [id], [TicketID], [DateModifiedByTFSSync], [SyncWithTFS], [TFSID], [TFSTitle], [TFSURL], [TFSState], [CreatorID], [CrmLinkID] FROM [dbo].[TicketLinkToTFS] WHERE ([id] = @id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("id", id);
        Fill(command);
      }
    }
    
    public static TicketLinkToTFSItem GetTicketLinkToTFSItem(LoginUser loginUser, int id)
    {
      TicketLinkToTFS ticketLinkToTFS = new TicketLinkToTFS(loginUser);
      ticketLinkToTFS.LoadByid(id);
      if (ticketLinkToTFS.IsEmpty)
        return null;
      else
        return ticketLinkToTFS[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketLinkToTFSItem> Members

    public IEnumerator<TicketLinkToTFSItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketLinkToTFSItem(row, this);
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
