using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketAutomationTriggerLogicItem : BaseItem
  {
    private TicketAutomationTriggerLogic _ticketAutomationTriggerLogic;
    
    public TicketAutomationTriggerLogicItem(DataRow row, TicketAutomationTriggerLogic ticketAutomationTriggerLogic): base(row, ticketAutomationTriggerLogic)
    {
      _ticketAutomationTriggerLogic = ticketAutomationTriggerLogic;
    }
	
    #region Properties
    
    public TicketAutomationTriggerLogic Collection
    {
      get { return _ticketAutomationTriggerLogic; }
    }
        
    
    
    
    public int TriggerLogicID
    {
      get { return (int)Row["TriggerLogicID"]; }
    }
    

    
    public string OtherTrigger
    {
      get { return Row["OtherTrigger"] != DBNull.Value ? (string)Row["OtherTrigger"] : null; }
      set { Row["OtherTrigger"] = CheckValue("OtherTrigger", value); }
    }
    

    
    public bool MatchAll
    {
      get { return (bool)Row["MatchAll"]; }
      set { Row["MatchAll"] = CheckValue("MatchAll", value); }
    }
    
    public string TestValue
    {
      get { return (string)Row["TestValue"]; }
      set { Row["TestValue"] = CheckValue("TestValue", value); }
    }
    
    public string Measure
    {
      get { return (string)Row["Measure"]; }
      set { Row["Measure"] = CheckValue("Measure", value); }
    }
    
    public int FieldID
    {
      get { return (int)Row["FieldID"]; }
      set { Row["FieldID"] = CheckValue("FieldID", value); }
    }
    
    public int TableID
    {
      get { return (int)Row["TableID"]; }
      set { Row["TableID"] = CheckValue("TableID", value); }
    }
    
    public int TriggerID
    {
      get { return (int)Row["TriggerID"]; }
      set { Row["TriggerID"] = CheckValue("TriggerID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class TicketAutomationTriggerLogic : BaseCollection, IEnumerable<TicketAutomationTriggerLogicItem>
  {
    public TicketAutomationTriggerLogic(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketAutomationTriggerLogic"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TriggerLogicID"; }
    }



    public TicketAutomationTriggerLogicItem this[int index]
    {
      get { return new TicketAutomationTriggerLogicItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketAutomationTriggerLogicItem ticketAutomationTriggerLogicItem);
    partial void AfterRowInsert(TicketAutomationTriggerLogicItem ticketAutomationTriggerLogicItem);
    partial void BeforeRowEdit(TicketAutomationTriggerLogicItem ticketAutomationTriggerLogicItem);
    partial void AfterRowEdit(TicketAutomationTriggerLogicItem ticketAutomationTriggerLogicItem);
    partial void BeforeRowDelete(int triggerLogicID);
    partial void AfterRowDelete(int triggerLogicID);    

    partial void BeforeDBDelete(int triggerLogicID);
    partial void AfterDBDelete(int triggerLogicID);    

    #endregion

    #region Public Methods

    public TicketAutomationTriggerLogicItemProxy[] GetTicketAutomationTriggerLogicItemProxies()
    {
      List<TicketAutomationTriggerLogicItemProxy> list = new List<TicketAutomationTriggerLogicItemProxy>();

      foreach (TicketAutomationTriggerLogicItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int triggerLogicID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationTriggerLogic] WHERE ([TriggerLogicID] = @TriggerLogicID);";
        deleteCommand.Parameters.Add("TriggerLogicID", SqlDbType.Int);
        deleteCommand.Parameters["TriggerLogicID"].Value = triggerLogicID;

        BeforeDBDelete(triggerLogicID);
        BeforeRowDelete(triggerLogicID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(triggerLogicID);
        AfterDBDelete(triggerLogicID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketAutomationTriggerLogicSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketAutomationTriggerLogic] SET     [TriggerID] = @TriggerID,    [TableID] = @TableID,    [FieldID] = @FieldID,    [Measure] = @Measure,    [TestValue] = @TestValue,    [MatchAll] = @MatchAll,    [OtherTrigger] = @OtherTrigger  WHERE ([TriggerLogicID] = @TriggerLogicID);";

		
		tempParameter = updateCommand.Parameters.Add("TriggerLogicID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TriggerID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("FieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Measure", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TestValue", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MatchAll", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OtherTrigger", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketAutomationTriggerLogic] (    [TriggerID],    [TableID],    [FieldID],    [Measure],    [TestValue],    [MatchAll],    [OtherTrigger]) VALUES ( @TriggerID, @TableID, @FieldID, @Measure, @TestValue, @MatchAll, @OtherTrigger); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OtherTrigger", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MatchAll", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("TestValue", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Measure", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FieldID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TriggerID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketAutomationTriggerLogic] WHERE ([TriggerLogicID] = @TriggerLogicID);";
		deleteCommand.Parameters.Add("TriggerLogicID", SqlDbType.Int);

		try
		{
		  foreach (TicketAutomationTriggerLogicItem ticketAutomationTriggerLogicItem in this)
		  {
			if (ticketAutomationTriggerLogicItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketAutomationTriggerLogicItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketAutomationTriggerLogicItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TriggerLogicID"].AutoIncrement = false;
			  Table.Columns["TriggerLogicID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketAutomationTriggerLogicItem.Row["TriggerLogicID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketAutomationTriggerLogicItem);
			}
			else if (ticketAutomationTriggerLogicItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketAutomationTriggerLogicItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketAutomationTriggerLogicItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketAutomationTriggerLogicItem);
			}
			else if (ticketAutomationTriggerLogicItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketAutomationTriggerLogicItem.Row["TriggerLogicID", DataRowVersion.Original];
			  deleteCommand.Parameters["TriggerLogicID"].Value = id;
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

      foreach (TicketAutomationTriggerLogicItem ticketAutomationTriggerLogicItem in this)
      {
        if (ticketAutomationTriggerLogicItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketAutomationTriggerLogicItem.Row["CreatorID"] == 0) ticketAutomationTriggerLogicItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketAutomationTriggerLogicItem.Row.Table.Columns.Contains("ModifierID")) ticketAutomationTriggerLogicItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketAutomationTriggerLogicItem FindByTriggerLogicID(int triggerLogicID)
    {
      foreach (TicketAutomationTriggerLogicItem ticketAutomationTriggerLogicItem in this)
      {
        if (ticketAutomationTriggerLogicItem.TriggerLogicID == triggerLogicID)
        {
          return ticketAutomationTriggerLogicItem;
        }
      }
      return null;
    }

    public virtual TicketAutomationTriggerLogicItem AddNewTicketAutomationTriggerLogicItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketAutomationTriggerLogic");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketAutomationTriggerLogicItem(row, this);
    }
    
    public virtual void LoadByTriggerLogicID(int triggerLogicID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TriggerLogicID], [TriggerID], [TableID], [FieldID], [Measure], [TestValue], [MatchAll], [OtherTrigger] FROM [dbo].[TicketAutomationTriggerLogic] WHERE ([TriggerLogicID] = @TriggerLogicID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TriggerLogicID", triggerLogicID);
        Fill(command);
      }
    }
    
    public static TicketAutomationTriggerLogicItem GetTicketAutomationTriggerLogicItem(LoginUser loginUser, int triggerLogicID)
    {
      TicketAutomationTriggerLogic ticketAutomationTriggerLogic = new TicketAutomationTriggerLogic(loginUser);
      ticketAutomationTriggerLogic.LoadByTriggerLogicID(triggerLogicID);
      if (ticketAutomationTriggerLogic.IsEmpty)
        return null;
      else
        return ticketAutomationTriggerLogic[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketAutomationTriggerLogicItem> Members

    public IEnumerator<TicketAutomationTriggerLogicItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketAutomationTriggerLogicItem(row, this);
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
