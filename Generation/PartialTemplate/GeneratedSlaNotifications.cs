using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SlaNotification : BaseItem
  {
    private SlaNotifications _slaNotifications;
    
    public SlaNotification(DataRow row, SlaNotifications slaNotifications): base(row, slaNotifications)
    {
      _slaNotifications = slaNotifications;
    }
	
    #region Properties
    
    public SlaNotifications Collection
    {
      get { return _slaNotifications; }
    }
        
    
    
    

    

    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? TimeClosedViolationDate
    {
      get { return Row["TimeClosedViolationDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["TimeClosedViolationDate"]) : null; }
      set { Row["TimeClosedViolationDate"] = CheckValue("TimeClosedViolationDate", value); }
    }

    public DateTime? TimeClosedViolationDateUtc
    {
      get { return Row["TimeClosedViolationDate"] != DBNull.Value ? (DateTime?)Row["TimeClosedViolationDate"] : null; }
    }
    
    public DateTime? LastActionViolationDate
    {
      get { return Row["LastActionViolationDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastActionViolationDate"]) : null; }
      set { Row["LastActionViolationDate"] = CheckValue("LastActionViolationDate", value); }
    }

    public DateTime? LastActionViolationDateUtc
    {
      get { return Row["LastActionViolationDate"] != DBNull.Value ? (DateTime?)Row["LastActionViolationDate"] : null; }
    }
    
    public DateTime? InitialResponseViolationDate
    {
      get { return Row["InitialResponseViolationDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["InitialResponseViolationDate"]) : null; }
      set { Row["InitialResponseViolationDate"] = CheckValue("InitialResponseViolationDate", value); }
    }

    public DateTime? InitialResponseViolationDateUtc
    {
      get { return Row["InitialResponseViolationDate"] != DBNull.Value ? (DateTime?)Row["InitialResponseViolationDate"] : null; }
    }
    
    public DateTime? TimeClosedWarningDate
    {
      get { return Row["TimeClosedWarningDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["TimeClosedWarningDate"]) : null; }
      set { Row["TimeClosedWarningDate"] = CheckValue("TimeClosedWarningDate", value); }
    }

    public DateTime? TimeClosedWarningDateUtc
    {
      get { return Row["TimeClosedWarningDate"] != DBNull.Value ? (DateTime?)Row["TimeClosedWarningDate"] : null; }
    }
    
    public DateTime? LastActionWarningDate
    {
      get { return Row["LastActionWarningDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastActionWarningDate"]) : null; }
      set { Row["LastActionWarningDate"] = CheckValue("LastActionWarningDate", value); }
    }

    public DateTime? LastActionWarningDateUtc
    {
      get { return Row["LastActionWarningDate"] != DBNull.Value ? (DateTime?)Row["LastActionWarningDate"] : null; }
    }
    
    public DateTime? InitialResponseWarningDate
    {
      get { return Row["InitialResponseWarningDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["InitialResponseWarningDate"]) : null; }
      set { Row["InitialResponseWarningDate"] = CheckValue("InitialResponseWarningDate", value); }
    }

    public DateTime? InitialResponseWarningDateUtc
    {
      get { return Row["InitialResponseWarningDate"] != DBNull.Value ? (DateTime?)Row["InitialResponseWarningDate"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class SlaNotifications : BaseCollection, IEnumerable<SlaNotification>
  {
    public SlaNotifications(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SlaNotifications"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "TicketID"; }
    }



    public SlaNotification this[int index]
    {
      get { return new SlaNotification(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SlaNotification slaNotification);
    partial void AfterRowInsert(SlaNotification slaNotification);
    partial void BeforeRowEdit(SlaNotification slaNotification);
    partial void AfterRowEdit(SlaNotification slaNotification);
    partial void BeforeRowDelete(int ticketID);
    partial void AfterRowDelete(int ticketID);    

    partial void BeforeDBDelete(int ticketID);
    partial void AfterDBDelete(int ticketID);    

    #endregion

    #region Public Methods

    public SlaNotificationProxy[] GetSlaNotificationProxies()
    {
      List<SlaNotificationProxy> list = new List<SlaNotificationProxy>();

      foreach (SlaNotification item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int ticketID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaNotifications] WHERE ([TicketID] = @TicketID);";
        deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);
        deleteCommand.Parameters["TicketID"].Value = ticketID;

        BeforeDBDelete(ticketID);
        BeforeRowDelete(ticketID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(ticketID);
        AfterDBDelete(ticketID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SlaNotificationsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SlaNotifications] SET     [TimeClosedViolationDate] = @TimeClosedViolationDate,    [LastActionViolationDate] = @LastActionViolationDate,    [InitialResponseViolationDate] = @InitialResponseViolationDate,    [TimeClosedWarningDate] = @TimeClosedWarningDate,    [LastActionWarningDate] = @LastActionWarningDate,    [InitialResponseWarningDate] = @InitialResponseWarningDate  WHERE ([TicketID] = @TicketID);";

		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeClosedViolationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastActionViolationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("InitialResponseViolationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("TimeClosedWarningDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastActionWarningDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("InitialResponseWarningDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SlaNotifications] (    [TicketID],    [TimeClosedViolationDate],    [LastActionViolationDate],    [InitialResponseViolationDate],    [TimeClosedWarningDate],    [LastActionWarningDate],    [InitialResponseWarningDate]) VALUES ( @TicketID, @TimeClosedViolationDate, @LastActionViolationDate, @InitialResponseViolationDate, @TimeClosedWarningDate, @LastActionWarningDate, @InitialResponseWarningDate); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("InitialResponseWarningDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastActionWarningDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeClosedWarningDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("InitialResponseViolationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastActionViolationDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("TimeClosedViolationDate", SqlDbType.DateTime, 8);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaNotifications] WHERE ([TicketID] = @TicketID);";
		deleteCommand.Parameters.Add("TicketID", SqlDbType.Int);

		try
		{
		  foreach (SlaNotification slaNotification in this)
		  {
			if (slaNotification.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(slaNotification);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = slaNotification.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["TicketID"].AutoIncrement = false;
			  Table.Columns["TicketID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				slaNotification.Row["TicketID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(slaNotification);
			}
			else if (slaNotification.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(slaNotification);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = slaNotification.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(slaNotification);
			}
			else if (slaNotification.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)slaNotification.Row["TicketID", DataRowVersion.Original];
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

      foreach (SlaNotification slaNotification in this)
      {
        if (slaNotification.Row.Table.Columns.Contains("CreatorID") && (int)slaNotification.Row["CreatorID"] == 0) slaNotification.Row["CreatorID"] = LoginUser.UserID;
        if (slaNotification.Row.Table.Columns.Contains("ModifierID")) slaNotification.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SlaNotification FindByTicketID(int ticketID)
    {
      foreach (SlaNotification slaNotification in this)
      {
        if (slaNotification.TicketID == ticketID)
        {
          return slaNotification;
        }
      }
      return null;
    }

    public virtual SlaNotification AddNewSlaNotification()
    {
      if (Table.Columns.Count < 1) LoadColumns("SlaNotifications");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SlaNotification(row, this);
    }
    
    public virtual void LoadByTicketID(int ticketID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [TicketID], [TimeClosedViolationDate], [LastActionViolationDate], [InitialResponseViolationDate], [TimeClosedWarningDate], [LastActionWarningDate], [InitialResponseWarningDate] FROM [dbo].[SlaNotifications] WHERE ([TicketID] = @TicketID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("TicketID", ticketID);
        Fill(command);
      }
    }
    
    public static SlaNotification GetSlaNotification(LoginUser loginUser, int ticketID)
    {
      SlaNotifications slaNotifications = new SlaNotifications(loginUser);
      slaNotifications.LoadByTicketID(ticketID);
      if (slaNotifications.IsEmpty)
        return null;
      else
        return slaNotifications[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SlaNotification> Members

    public IEnumerator<SlaNotification> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SlaNotification(row, this);
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
