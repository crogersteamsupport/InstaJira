using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketLinkToJiraItem : BaseItem
  {
    private TicketLinkToJira _ticketLinkToJira;
    
    public TicketLinkToJiraItem(DataRow row, TicketLinkToJira ticketLinkToJira): base(row, ticketLinkToJira)
    {
      _ticketLinkToJira = ticketLinkToJira;
    }
	
    #region Properties
    
    public TicketLinkToJira Collection
    {
      get { return _ticketLinkToJira; }
    }
        
    
    
    
    public int id
    {
      get { return (int)Row["id"]; }
    }
    

    
    public int? TicketID
    {
      get { return Row["TicketID"] != DBNull.Value ? (int?)Row["TicketID"] : null; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    
    public bool? SyncWithJira
    {
      get { return Row["SyncWithJira"] != DBNull.Value ? (bool?)Row["SyncWithJira"] : null; }
      set { Row["SyncWithJira"] = CheckValue("SyncWithJira", value); }
    }
    
    public int? JiraID
    {
      get { return Row["JiraID"] != DBNull.Value ? (int?)Row["JiraID"] : null; }
      set { Row["JiraID"] = CheckValue("JiraID", value); }
    }
    
    public string JiraKey
    {
      get { return Row["JiraKey"] != DBNull.Value ? (string)Row["JiraKey"] : null; }
      set { Row["JiraKey"] = CheckValue("JiraKey", value); }
    }
    
    public string JiraLinkURL
    {
      get { return Row["JiraLinkURL"] != DBNull.Value ? (string)Row["JiraLinkURL"] : null; }
      set { Row["JiraLinkURL"] = CheckValue("JiraLinkURL", value); }
    }
    
    public string JiraStatus
    {
      get { return Row["JiraStatus"] != DBNull.Value ? (string)Row["JiraStatus"] : null; }
      set { Row["JiraStatus"] = CheckValue("JiraStatus", value); }
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
    

    

    /* DateTime */
    
    
    

    
    public DateTime? DateModifiedByJiraSync
    {
      get { return Row["DateModifiedByJiraSync"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModifiedByJiraSync"]) : null; }
      set { Row["DateModifiedByJiraSync"] = CheckValue("DateModifiedByJiraSync", value); }
    }

    public DateTime? DateModifiedByJiraSyncUtc
    {
      get { return Row["DateModifiedByJiraSync"] != DBNull.Value ? (DateTime?)Row["DateModifiedByJiraSync"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class TicketLinkToJira : BaseCollection, IEnumerable<TicketLinkToJiraItem>
  {
    public TicketLinkToJira(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketLinkToJira"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "id"; }
    }



    public TicketLinkToJiraItem this[int index]
    {
      get { return new TicketLinkToJiraItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketLinkToJiraItem ticketLinkToJiraItem);
    partial void AfterRowInsert(TicketLinkToJiraItem ticketLinkToJiraItem);
    partial void BeforeRowEdit(TicketLinkToJiraItem ticketLinkToJiraItem);
    partial void AfterRowEdit(TicketLinkToJiraItem ticketLinkToJiraItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public TicketLinkToJiraItemProxy[] GetTicketLinkToJiraItemProxies()
    {
      List<TicketLinkToJiraItemProxy> list = new List<TicketLinkToJiraItemProxy>();

      foreach (TicketLinkToJiraItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
      BeforeDBDelete(id);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketLinkToJira] WHERE ([id] = @id);";
        deleteCommand.Parameters.Add("id", SqlDbType.Int);
        deleteCommand.Parameters["id"].Value = id;

        BeforeRowDelete(id);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(id);
      }
      AfterDBDelete(id);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketLinkToJiraSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketLinkToJira] SET     [TicketID] = @TicketID,    [DateModifiedByJiraSync] = @DateModifiedByJiraSync,    [SyncWithJira] = @SyncWithJira,    [JiraID] = @JiraID,    [JiraKey] = @JiraKey,    [JiraLinkURL] = @JiraLinkURL,    [JiraStatus] = @JiraStatus,    [CrmLinkID] = @CrmLinkID  WHERE ([id] = @id);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("DateModifiedByJiraSync", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("SyncWithJira", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraKey", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraLinkURL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("JiraStatus", SqlDbType.VarChar, 8000);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketLinkToJira] (    [TicketID],    [DateModifiedByJiraSync],    [SyncWithJira],    [JiraID],    [JiraKey],    [JiraLinkURL],    [JiraStatus],    [CreatorID],    [CrmLinkID]) VALUES ( @TicketID, @DateModifiedByJiraSync, @SyncWithJira, @JiraID, @JiraKey, @JiraLinkURL, @JiraStatus, @CreatorID, @CrmLinkID); SET @Identity = SCOPE_IDENTITY();";

		
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
		
		tempParameter = insertCommand.Parameters.Add("JiraStatus", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraLinkURL", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraKey", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("JiraID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("SyncWithJira", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModifiedByJiraSync", SqlDbType.DateTime, 8);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketLinkToJira] WHERE ([id] = @id);";
		deleteCommand.Parameters.Add("id", SqlDbType.Int);

		try
		{
		  foreach (TicketLinkToJiraItem ticketLinkToJiraItem in this)
		  {
			if (ticketLinkToJiraItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketLinkToJiraItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketLinkToJiraItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["id"].AutoIncrement = false;
			  Table.Columns["id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketLinkToJiraItem.Row["id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketLinkToJiraItem);
			}
			else if (ticketLinkToJiraItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketLinkToJiraItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketLinkToJiraItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketLinkToJiraItem);
			}
			else if (ticketLinkToJiraItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketLinkToJiraItem.Row["id", DataRowVersion.Original];
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

      foreach (TicketLinkToJiraItem ticketLinkToJiraItem in this)
      {
        if (ticketLinkToJiraItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketLinkToJiraItem.Row["CreatorID"] == 0) ticketLinkToJiraItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketLinkToJiraItem.Row.Table.Columns.Contains("ModifierID")) ticketLinkToJiraItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketLinkToJiraItem FindByid(int id)
    {
      foreach (TicketLinkToJiraItem ticketLinkToJiraItem in this)
      {
        if (ticketLinkToJiraItem.id == id)
        {
          return ticketLinkToJiraItem;
        }
      }
      return null;
    }

    public virtual TicketLinkToJiraItem AddNewTicketLinkToJiraItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketLinkToJira");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketLinkToJiraItem(row, this);
    }
    
    public virtual void LoadByid(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [id], [TicketID], [DateModifiedByJiraSync], [SyncWithJira], [JiraID], [JiraKey], [JiraLinkURL], [JiraStatus], [CreatorID], [CrmLinkID] FROM [dbo].[TicketLinkToJira] WHERE ([id] = @id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("id", id);
        Fill(command);
      }
    }
    
    public static TicketLinkToJiraItem GetTicketLinkToJiraItem(LoginUser loginUser, int id)
    {
      TicketLinkToJira ticketLinkToJira = new TicketLinkToJira(loginUser);
      ticketLinkToJira.LoadByid(id);
      if (ticketLinkToJira.IsEmpty)
        return null;
      else
        return ticketLinkToJira[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketLinkToJiraItem> Members

    public IEnumerator<TicketLinkToJiraItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketLinkToJiraItem(row, this);
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
