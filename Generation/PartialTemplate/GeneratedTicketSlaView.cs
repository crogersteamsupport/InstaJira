using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class TicketSlaViewItem : BaseItem
  {
    private TicketSlaView _ticketSlaView;
    
    public TicketSlaViewItem(DataRow row, TicketSlaView ticketSlaView): base(row, ticketSlaView)
    {
      _ticketSlaView = ticketSlaView;
    }
	
    #region Properties
    
    public TicketSlaView Collection
    {
      get { return _ticketSlaView; }
    }
        
    
    
    

    
    public int? ViolationTimeClosed
    {
      get { return Row["ViolationTimeClosed"] != DBNull.Value ? (int?)Row["ViolationTimeClosed"] : null; }
      set { Row["ViolationTimeClosed"] = CheckValue("ViolationTimeClosed", value); }
    }
    
    public int? WarningTimeClosed
    {
      get { return Row["WarningTimeClosed"] != DBNull.Value ? (int?)Row["WarningTimeClosed"] : null; }
      set { Row["WarningTimeClosed"] = CheckValue("WarningTimeClosed", value); }
    }
    
    public int? ViolationLastAction
    {
      get { return Row["ViolationLastAction"] != DBNull.Value ? (int?)Row["ViolationLastAction"] : null; }
      set { Row["ViolationLastAction"] = CheckValue("ViolationLastAction", value); }
    }
    
    public int? WarningLastAction
    {
      get { return Row["WarningLastAction"] != DBNull.Value ? (int?)Row["WarningLastAction"] : null; }
      set { Row["WarningLastAction"] = CheckValue("WarningLastAction", value); }
    }
    
    public int? ViolationInitialResponse
    {
      get { return Row["ViolationInitialResponse"] != DBNull.Value ? (int?)Row["ViolationInitialResponse"] : null; }
      set { Row["ViolationInitialResponse"] = CheckValue("ViolationInitialResponse", value); }
    }
    
    public int? WarningInitialResponse
    {
      get { return Row["WarningInitialResponse"] != DBNull.Value ? (int?)Row["WarningInitialResponse"] : null; }
      set { Row["WarningInitialResponse"] = CheckValue("WarningInitialResponse", value); }
    }
    

    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class TicketSlaView : BaseCollection, IEnumerable<TicketSlaViewItem>
  {
    public TicketSlaView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "TicketSlaView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public TicketSlaViewItem this[int index]
    {
      get { return new TicketSlaViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(TicketSlaViewItem ticketSlaViewItem);
    partial void AfterRowInsert(TicketSlaViewItem ticketSlaViewItem);
    partial void BeforeRowEdit(TicketSlaViewItem ticketSlaViewItem);
    partial void AfterRowEdit(TicketSlaViewItem ticketSlaViewItem);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public TicketSlaViewItemProxy[] GetTicketSlaViewItemProxies()
    {
      List<TicketSlaViewItemProxy> list = new List<TicketSlaViewItemProxy>();

      foreach (TicketSlaViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketSlaView] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeDBDelete(ticketID);
        BeforeRowDelete(ticketID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketID);
        AfterDBDelete(ticketID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("TicketSlaViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[TicketSlaView] SET     [ViolationTimeClosed] = @ViolationTimeClosed,    [WarningTimeClosed] = @WarningTimeClosed,    [ViolationLastAction] = @ViolationLastAction,    [WarningLastAction] = @WarningLastAction,    [ViolationInitialResponse] = @ViolationInitialResponse,    [WarningInitialResponse] = @WarningInitialResponse  WHERE ([TicketID] = @TicketID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViolationTimeClosed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("WarningTimeClosed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViolationLastAction", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("WarningLastAction", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViolationInitialResponse", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("WarningInitialResponse", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[TicketSlaView] (    [TicketID],    [ViolationTimeClosed],    [WarningTimeClosed],    [ViolationLastAction],    [WarningLastAction],    [ViolationInitialResponse],    [WarningInitialResponse]) VALUES ( @TicketID, @ViolationTimeClosed, @WarningTimeClosed, @ViolationLastAction, @WarningLastAction, @ViolationInitialResponse, @WarningInitialResponse); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("WarningInitialResponse", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ViolationInitialResponse", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("WarningLastAction", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ViolationLastAction", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("WarningTimeClosed", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ViolationTimeClosed", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[TicketSlaView] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (TicketSlaViewItem ticketSlaViewItem in this)
		  {
			if (ticketSlaViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(ticketSlaViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = ticketSlaViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				ticketSlaViewItem.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(ticketSlaViewItem);
			}
			else if (ticketSlaViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(ticketSlaViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = ticketSlaViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(ticketSlaViewItem);
			}
			else if (ticketSlaViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)ticketSlaViewItem.Row["TicketID", DataRowVersion.Original];
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

      foreach (TicketSlaViewItem ticketSlaViewItem in this)
      {
        if (ticketSlaViewItem.Row.Table.Columns.Contains("CreatorID") && (int)ticketSlaViewItem.Row["CreatorID"] == 0) ticketSlaViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (ticketSlaViewItem.Row.Table.Columns.Contains("ModifierID")) ticketSlaViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public TicketSlaViewItem FindByTicketID(int ticketID)
    {
      foreach (TicketSlaViewItem ticketSlaViewItem in this)
      {
        if (ticketSlaViewItem.TicketID == ticketID)
        {
          return ticketSlaViewItem;
        }
      }
      return null;
    }

    public virtual TicketSlaViewItem AddNewTicketSlaViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("TicketSlaView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new TicketSlaViewItem(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [ViolationTimeClosed], [WarningTimeClosed], [ViolationLastAction], [WarningLastAction], [ViolationInitialResponse], [WarningInitialResponse] FROM [dbo].[TicketSlaView] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static TicketSlaViewItem GetTicketSlaViewItem(LoginUser loginUser, int ticketID)
    {
      TicketSlaView ticketSlaView = new TicketSlaView(loginUser);
      ticketSlaView.LoadByTicketID(ticketID);
      if (ticketSlaView.IsEmpty)
        return null;
      else
        return ticketSlaView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<TicketSlaViewItem> Members

    public IEnumerator<TicketSlaViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new TicketSlaViewItem(row, this);
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
