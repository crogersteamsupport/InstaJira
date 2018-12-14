using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketTimeLineViewItem : BaseItem
  {
    private TicketTimeLineView _ticketTimeLineView;
    
    public TicketTimeLineViewItem(DataRow row, TicketTimeLineView ticketTimeLineView): base(row, ticketTimeLineView)
    {
      _ticketTimeLineView = ticketTimeLineView;
    }
	
    #region Properties
    
    public TicketTimeLineView Collection
    {
      get { return _ticketTimeLineView; }
    }
        
    
    
    

    
    public int? TicketID
    {
      get { return Row["TicketID"] != DBNull.Value ? (int?)Row["TicketID"] : null; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    
    public int? TicketNumber
    {
      get { return Row["TicketNumber"] != DBNull.Value ? (int?)Row["TicketNumber"] : null; }
      set { Row["TicketNumber"] = CheckValue("TicketNumber", value); }
    }
    
    public string MessageType
    {
      get { return Row["MessageType"] != DBNull.Value ? (string)Row["MessageType"] : null; }
      set { Row["MessageType"] = CheckValue("MessageType", value); }
    }

    public int? ActionTypeID
    {
      get { return Row["ActionTypeID"] != DBNull.Value ? (int?)Row["ActionTypeID"] : null; }
      set { Row["ActionTypeID"] = CheckValue("ActionTypeID", value); }
    }
    
    public string Message
    {
      get { return Row["Message"] != DBNull.Value ? (string)Row["Message"] : null; }
      set { Row["Message"] = CheckValue("Message", value); }
    }
    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public string CreatorName
    {
      get { return Row["CreatorName"] != DBNull.Value ? (string)Row["CreatorName"] : null; }
      set { Row["CreatorName"] = CheckValue("CreatorName", value); }
    }
    
    public int? WCUserID
    {
      get { return Row["WCUserID"] != DBNull.Value ? (int?)Row["WCUserID"] : null; }
      set { Row["WCUserID"] = CheckValue("WCUserID", value); }
    }

    public int? TimeSpent
    {
      get { return Row["TimeSpent"] != DBNull.Value ? (int?)Row["TimeSpent"] : null; }
      set { Row["TimeSpent"] = CheckValue("TimeSpent", value); }
    }
    

    
    public bool IsPinned
    {
      get { return (bool)Row["IsPinned"]; }
      set { Row["IsPinned"] = CheckValue("IsPinned", value); }
    }
    
    public bool IsKnowledgeBase
    {
      get { return (bool)Row["IsKnowledgeBase"]; }
      set { Row["IsKnowledgeBase"] = CheckValue("IsKnowledgeBase", value); }
    }
    
    public bool IsVisibleOnPortal
    {
      get { return (bool)Row["IsVisibleOnPortal"]; }
      set { Row["IsVisibleOnPortal"] = CheckValue("IsVisibleOnPortal", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public bool IsWC
    {
      get { return (bool)Row["IsWC"]; }
      set { Row["IsWC"] = CheckValue("IsWC", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    

    /* DateTime */


    public DateTime? DateStarted
    {
      get { return Row["DateStarted"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateStarted"]) : null; }
      set { Row["DateStarted"] = CheckValue("DateStarted", value); }
    }

    public DateTime? DateStartedUtc
    {
      get { return Row["DateStarted"] != DBNull.Value ? (DateTime?)Row["DateStarted"] : null; }
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

  public partial class TicketTimeLineView : BaseCollection, IEnumerable<TicketTimeLineViewItem>
  {
    public TicketTimeLineView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketTimeLineView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public TicketTimeLineViewItem this[int index]
    {
      get { return new TicketTimeLineViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketTimeLineViewItem ticketTimeLineViewItem);
    partial void AfterRowInsert(TicketTimeLineViewItem ticketTimeLineViewItem);
    partial void BeforeRowEdit(TicketTimeLineViewItem ticketTimeLineViewItem);
    partial void AfterRowEdit(TicketTimeLineViewItem ticketTimeLineViewItem);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public TicketTimeLineViewItemProxy[] GetTicketTimeLineViewItemProxies()
    {
      List<TicketTimeLineViewItemProxy> list = new List<TicketTimeLineViewItemProxy>();

      foreach (TicketTimeLineViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
      BeforeDBDelete(ticketID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketTimeLineView] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeRowDelete(ticketID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(ticketID);
      }
      AfterDBDelete(ticketID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketTimeLineViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketTimeLineView] SET     [TicketNumber] = @TicketNumber,    [RefID] = @RefID,    [IsWC] = @IsWC,    [MessageType] = @MessageType,    [Message] = @Message,    [OrganizationID] = @OrganizationID,    [CreatorName] = @CreatorName,    [IsVisibleOnPortal] = @IsVisibleOnPortal,    [IsKnowledgeBase] = @IsKnowledgeBase,    [IsPinned] = @IsPinned,    [WCUserID] = @WCUserID  WHERE ([TicketID] = @TicketID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsWC", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("MessageType", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Message", SqlDbType.Text, 2147483647);
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
		
		tempParameter = updateCommand.Parameters.Add("CreatorName", SqlDbType.VarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsPinned", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("WCUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketTimeLineView] (    [TicketID],    [TicketNumber],    [RefID],    [IsWC],    [MessageType],    [Message],    [DateCreated],    [OrganizationID],    [CreatorID],    [CreatorName],    [IsVisibleOnPortal],    [IsKnowledgeBase],    [IsPinned],    [WCUserID]) VALUES ( @TicketID, @TicketNumber, @RefID, @IsWC, @MessageType, @Message, @DateCreated, @OrganizationID, @CreatorID, @CreatorName, @IsVisibleOnPortal, @IsKnowledgeBase, @IsPinned, @WCUserID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("WCUserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsPinned", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsKnowledgeBase", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsVisibleOnPortal", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
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
		
		tempParameter = insertCommand.Parameters.Add("Message", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("MessageType", SqlDbType.VarChar, 255);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsWC", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketNumber", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketTimeLineView] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (TicketTimeLineViewItem ticketTimeLineViewItem in this)
		  {
			if (ticketTimeLineViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketTimeLineViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketTimeLineViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketTimeLineViewItem.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketTimeLineViewItem);
			}
			else if (ticketTimeLineViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketTimeLineViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketTimeLineViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketTimeLineViewItem);
			}
			else if (ticketTimeLineViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketTimeLineViewItem.Row["TicketID", DataRowVersion.Original];
			  deleteCommand.Parameters["TicketID"].Value = id;
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

      foreach (TicketTimeLineViewItem ticketTimeLineViewItem in this)
      {
        if (ticketTimeLineViewItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketTimeLineViewItem.Row["CreatorID"] == 0) ticketTimeLineViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketTimeLineViewItem.Row.Table.Columns.Contains("ModifierID")) ticketTimeLineViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketTimeLineViewItem FindByTicketID(int ticketID)
    {
      foreach (TicketTimeLineViewItem ticketTimeLineViewItem in this)
      {
        if (ticketTimeLineViewItem.TicketID == ticketID)
        {
          return ticketTimeLineViewItem;
        }
      }
      return null;
    }

    public virtual TicketTimeLineViewItem AddNewTicketTimeLineViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketTimeLineView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketTimeLineViewItem(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [TicketNumber], [RefID], [IsWC], [MessageType], [Message], [DateCreated], [OrganizationID], [CreatorID], [CreatorName], [IsVisibleOnPortal], [IsKnowledgeBase], [IsPinned], [WCUserID] FROM [dbo].[TicketTimeLineView] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static TicketTimeLineViewItem GetTicketTimeLineViewItem(LoginUser loginUser, int ticketID)
    {
      TicketTimeLineView ticketTimeLineView = new TicketTimeLineView(loginUser);
      ticketTimeLineView.LoadByTicketID(ticketID);
      if (ticketTimeLineView.IsEmpty)
        return null;
      else
        return ticketTimeLineView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketTimeLineViewItem> Members

    public IEnumerator<TicketTimeLineViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketTimeLineViewItem(row, this);
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
