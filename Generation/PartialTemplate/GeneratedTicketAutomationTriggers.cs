using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketAutomationTrigger : BaseItem
  {
    private TicketAutomationTriggers _ticketAutomationTriggers;
    
    public TicketAutomationTrigger(DataRow row, TicketAutomationTriggers ticketAutomationTriggers): base(row, ticketAutomationTriggers)
    {
      _ticketAutomationTriggers = ticketAutomationTriggers;
    }
	
    #region Properties
    
    public TicketAutomationTriggers Collection
    {
      get { return _ticketAutomationTriggers; }
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

  public partial class TicketAutomationTriggers : BaseCollection, IEnumerable<TicketAutomationTrigger>
  {
    public TicketAutomationTriggers(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketAutomationTriggers"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TriggerID"; }
    }



    public TicketAutomationTrigger this[int index]
    {
      get { return new TicketAutomationTrigger(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketAutomationTrigger ticketAutomationTrigger);
    partial void AfterRowInsert(TicketAutomationTrigger ticketAutomationTrigger);
    partial void BeforeRowEdit(TicketAutomationTrigger ticketAutomationTrigger);
    partial void AfterRowEdit(TicketAutomationTrigger ticketAutomationTrigger);
    partial void BeforeRowDelete(int triggerID);
    partial void AfterRowDelete(int triggerID);    

    partial void BeforeDBDelete(int triggerID);
    partial void AfterDBDelete(int triggerID);    

    #endregion

    #region Public Methods

    public TicketAutomationTriggerProxy[] GetTicketAutomationTriggerProxies()
    {
      List<TicketAutomationTriggerProxy> list = new List<TicketAutomationTriggerProxy>();

      foreach (TicketAutomationTrigger item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int triggerID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationTriggers] WHERE ([TriggerID] = @TriggerID);";
        deleteCommand.Parameters.Add("TriggerID", SqlDbType.Int);
        deleteCommand.Parameters["TriggerID"].Value = triggerID;

        BeforeDBDelete(triggerID);
        BeforeRowDelete(triggerID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(triggerID);
        AfterDBDelete(triggerID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketAutomationTriggersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketAutomationTriggers] SET     [Name] = @Name,    [Active] = @Active,    [Position] = @Position,    [OrganizationID] = @OrganizationID,    [UseCustomSQL] = @UseCustomSQL,    [CustomSQL] = @CustomSQL,    [DateModified] = @DateModified,    [ModifierID] = @ModifierID,    [LastSQLExecuted] = @LastSQLExecuted  WHERE ([TriggerID] = @TriggerID);";

		
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketAutomationTriggers] (    [Name],    [Active],    [Position],    [OrganizationID],    [UseCustomSQL],    [CustomSQL],    [DateCreated],    [DateModified],    [CreatorID],    [ModifierID],    [LastSQLExecuted]) VALUES ( @Name, @Active, @Position, @OrganizationID, @UseCustomSQL, @CustomSQL, @DateCreated, @DateModified, @CreatorID, @ModifierID, @LastSQLExecuted); SET @Identity = SCOPE_IDENTITY();";

		
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationTriggers] WHERE ([TriggerID] = @TriggerID);";
		deleteCommand.Parameters.Add("TriggerID", SqlDbType.Int);

		try
		{
		  foreach (TicketAutomationTrigger ticketAutomationTrigger in this)
		  {
			if (ticketAutomationTrigger.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketAutomationTrigger);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketAutomationTrigger.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TriggerID"].AutoIncrement = false;
			  Table.Columns["TriggerID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketAutomationTrigger.Row["TriggerID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketAutomationTrigger);
			}
			else if (ticketAutomationTrigger.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketAutomationTrigger);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketAutomationTrigger.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketAutomationTrigger);
			}
			else if (ticketAutomationTrigger.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketAutomationTrigger.Row["TriggerID", DataRowVersion.Original];
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

      foreach (TicketAutomationTrigger ticketAutomationTrigger in this)
      {
        if (ticketAutomationTrigger.Row.Table.Columns.Contains("CreatorID") && (int)ticketAutomationTrigger.Row["CreatorID"] == 0) ticketAutomationTrigger.Row["CreatorID"] = LoginUser.UserID;
        if (ticketAutomationTrigger.Row.Table.Columns.Contains("ModifierID")) ticketAutomationTrigger.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketAutomationTrigger FindByTriggerID(int triggerID)
    {
      foreach (TicketAutomationTrigger ticketAutomationTrigger in this)
      {
        if (ticketAutomationTrigger.TriggerID == triggerID)
        {
          return ticketAutomationTrigger;
        }
      }
      return null;
    }

    public virtual TicketAutomationTrigger AddNewTicketAutomationTrigger()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketAutomationTriggers");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketAutomationTrigger(row, this);
    }
    
    public virtual void LoadByTriggerID(int triggerID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TriggerID], [Name], [Active], [Position], [OrganizationID], [UseCustomSQL], [CustomSQL], [DateCreated], [DateModified], [CreatorID], [ModifierID], [LastSQLExecuted] FROM [dbo].[TicketAutomationTriggers] WHERE ([TriggerID] = @TriggerID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TriggerID", triggerID);
        Fill(command);
      }
    }
    
    public static TicketAutomationTrigger GetTicketAutomationTrigger(LoginUser loginUser, int triggerID)
    {
      TicketAutomationTriggers ticketAutomationTriggers = new TicketAutomationTriggers(loginUser);
      ticketAutomationTriggers.LoadByTriggerID(triggerID);
      if (ticketAutomationTriggers.IsEmpty)
        return null;
      else
        return ticketAutomationTriggers[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketAutomationTrigger> Members

    public IEnumerator<TicketAutomationTrigger> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketAutomationTrigger(row, this);
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
