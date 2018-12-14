using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketAutomationTriggersViewItem : BaseItem
  {
    private TicketAutomationTriggersView _ticketAutomationTriggersView;
    
    public TicketAutomationTriggersViewItem(DataRow row, TicketAutomationTriggersView ticketAutomationTriggersView): base(row, ticketAutomationTriggersView)
    {
      _ticketAutomationTriggersView = ticketAutomationTriggersView;
    }
	
    #region Properties
    
    public TicketAutomationTriggersView Collection
    {
      get { return _ticketAutomationTriggersView; }
    }
        
    
    public int? ExecutionsCount
    {
      get { return Row["ExecutionsCount"] != DBNull.Value ? (int?)Row["ExecutionsCount"] : null; }
    }
    
    
    
    public int TriggerID
    {
      get { return (int)Row["TriggerID"]; }
    }
    

    
    public string CustomSQL
    {
      get { return Row["CustomSQL"] != DBNull.Value ? (string)Row["CustomSQL"] : null; }
      set { Row["CustomSQL"] = CheckValue("CustomSQL", value); }
    }
    
    public string LastSQLExecuted
    {
      get { return Row["LastSQLExecuted"] != DBNull.Value ? (string)Row["LastSQLExecuted"] : null; }
      set { Row["LastSQLExecuted"] = CheckValue("LastSQLExecuted", value); }
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
    
    public bool UseCustomSQL
    {
      get { return (bool)Row["UseCustomSQL"]; }
      set { Row["UseCustomSQL"] = CheckValue("UseCustomSQL", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public bool Active
    {
      get { return (bool)Row["Active"]; }
      set { Row["Active"] = CheckValue("Active", value); }
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

  public partial class TicketAutomationTriggersView : BaseCollection, IEnumerable<TicketAutomationTriggersViewItem>
  {
    public TicketAutomationTriggersView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketAutomationTriggersView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TriggerID"; }
    }



    public TicketAutomationTriggersViewItem this[int index]
    {
      get { return new TicketAutomationTriggersViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketAutomationTriggersViewItem ticketAutomationTriggersViewItem);
    partial void AfterRowInsert(TicketAutomationTriggersViewItem ticketAutomationTriggersViewItem);
    partial void BeforeRowEdit(TicketAutomationTriggersViewItem ticketAutomationTriggersViewItem);
    partial void AfterRowEdit(TicketAutomationTriggersViewItem ticketAutomationTriggersViewItem);
    partial void BeforeRowDelete(int triggerID);
    partial void AfterRowDelete(int triggerID);    

    partial void BeforeDBDelete(int triggerID);
    partial void AfterDBDelete(int triggerID);    

    #endregion

    #region Public Methods

    public TicketAutomationTriggersViewItemProxy[] GetTicketAutomationTriggersViewItemProxies()
    {
      List<TicketAutomationTriggersViewItemProxy> list = new List<TicketAutomationTriggersViewItemProxy>();

      foreach (TicketAutomationTriggersViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int triggerID)
    {
      BeforeDBDelete(triggerID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationTriggersView] WHERE ([TriggerID] = @TriggerID);";
        deleteCommand.Parameters.Add("TriggerID", SqlDbType.Int);
        deleteCommand.Parameters["TriggerID"].Value = triggerID;

        BeforeRowDelete(triggerID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(triggerID);
      }
      AfterDBDelete(triggerID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketAutomationTriggersViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketAutomationTriggersView] SET     [Name] = @Name,    [Active] = @Active,    [Position] = @Position,    [OrganizationID] = @OrganizationID,    [UseCustomSQL] = @UseCustomSQL,    [CustomSQL] = @CustomSQL,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [LastSQLExecuted] = @LastSQLExecuted,    [ExecutionsCount] = @ExecutionsCount  WHERE ([TriggerID] = @TriggerID);";

		
		tempParameter = updateCommand.Parameters.Add("TriggerID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Active", SqlDbType.Bit, 1);
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
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseCustomSQL", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CustomSQL", SqlDbType.VarChar, 5000);
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
		
		tempParameter = updateCommand.Parameters.Add("LastSQLExecuted", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ExecutionsCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketAutomationTriggersView] (    [Name],    [Active],    [Position],    [OrganizationID],    [UseCustomSQL],    [CustomSQL],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [LastSQLExecuted],    [ExecutionsCount]) VALUES ( @Name, @Active, @Position, @OrganizationID, @UseCustomSQL, @CustomSQL, @DateCreated, @DateModified, @CreatorID, @ModifierID, @LastSQLExecuted, @ExecutionsCount); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ExecutionsCount", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastSQLExecuted", SqlDbType.VarChar, -1);
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
		
		tempParameter = insertCommand.Parameters.Add("CustomSQL", SqlDbType.VarChar, 5000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseCustomSQL", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Active", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 200);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationTriggersView] WHERE ([TriggerID] = @TriggerID);";
		deleteCommand.Parameters.Add("TriggerID", SqlDbType.Int);

		try
		{
		  foreach (TicketAutomationTriggersViewItem ticketAutomationTriggersViewItem in this)
		  {
			if (ticketAutomationTriggersViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketAutomationTriggersViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketAutomationTriggersViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TriggerID"].AutoIncrement = false;
			  Table.Columns["TriggerID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketAutomationTriggersViewItem.Row["TriggerID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketAutomationTriggersViewItem);
			}
			else if (ticketAutomationTriggersViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketAutomationTriggersViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketAutomationTriggersViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketAutomationTriggersViewItem);
			}
			else if (ticketAutomationTriggersViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketAutomationTriggersViewItem.Row["TriggerID", DataRowVersion.Original];
			  deleteCommand.Parameters["TriggerID"].Value = id;
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

      foreach (TicketAutomationTriggersViewItem ticketAutomationTriggersViewItem in this)
      {
        if (ticketAutomationTriggersViewItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketAutomationTriggersViewItem.Row["CreatorID"] == 0) ticketAutomationTriggersViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketAutomationTriggersViewItem.Row.Table.Columns.Contains("ModifierID")) ticketAutomationTriggersViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketAutomationTriggersViewItem FindByTriggerID(int triggerID)
    {
      foreach (TicketAutomationTriggersViewItem ticketAutomationTriggersViewItem in this)
      {
        if (ticketAutomationTriggersViewItem.TriggerID == triggerID)
        {
          return ticketAutomationTriggersViewItem;
        }
      }
      return null;
    }

    public virtual TicketAutomationTriggersViewItem AddNewTicketAutomationTriggersViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketAutomationTriggersView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketAutomationTriggersViewItem(row, this);
    }
    
    public virtual void LoadByTriggerID(int triggerID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TriggerID], [Name], [Active], [Position], [OrganizationID], [UseCustomSQL], [CustomSQL], [DateCreated], [DateModified], [CreatorID], [ModifierID], [LastSQLExecuted], [ExecutionsCount] FROM [dbo].[TicketAutomationTriggersView] WHERE ([TriggerID] = @TriggerID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TriggerID", triggerID);
        Fill(command);
      }
    }
    
    public static TicketAutomationTriggersViewItem GetTicketAutomationTriggersViewItem(LoginUser loginUser, int triggerID)
    {
      TicketAutomationTriggersView ticketAutomationTriggersView = new TicketAutomationTriggersView(loginUser);
      ticketAutomationTriggersView.LoadByTriggerID(triggerID);
      if (ticketAutomationTriggersView.IsEmpty)
        return null;
      else
        return ticketAutomationTriggersView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketAutomationTriggersViewItem> Members

    public IEnumerator<TicketAutomationTriggersViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketAutomationTriggersViewItem(row, this);
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
