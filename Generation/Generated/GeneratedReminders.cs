using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Reminder : BaseItem
  {
    private Reminders _reminders;
    
    public Reminder(DataRow row, Reminders reminders): base(row, reminders)
    {
      _reminders = reminders;
    }
	
    #region Properties
    
    public Reminders Collection
    {
      get { return _reminders; }
    }
        
    
    
    
    public int ReminderID
    {
      get { return (int)Row["ReminderID"]; }
    }
    

    

    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public bool HasEmailSent
    {
      get { return (bool)Row["HasEmailSent"]; }
      set { Row["HasEmailSent"] = CheckValue("HasEmailSent", value); }
    }
    
    public bool IsDismissed
    {
      get { return (bool)Row["IsDismissed"]; }
      set { Row["IsDismissed"] = CheckValue("IsDismissed", value); }
    }
    
    public int UserID
    {
      get { return (int)Row["UserID"]; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateCreated
    {
      get { return DateToLocal((DateTime)Row["DateCreated"]); }
      set { Row["DateCreated"] = CheckValue("DateCreated", value); }
    }

    public DateTime DateCreatedUtc
    {
      get { return (DateTime)Row["DateCreated"]; }
    }
    
    public DateTime DueDate
    {
      get { return DateToLocal((DateTime)Row["DueDate"]); }
      set { Row["DueDate"] = CheckValue("DueDate", value); }
    }

    public DateTime DueDateUtc
    {
      get { return (DateTime)Row["DueDate"]; }
    }
    

    #endregion
    
    
  }

  public partial class Reminders : BaseCollection, IEnumerable<Reminder>
  {
    public Reminders(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Reminders"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReminderID"; }
    }



    public Reminder this[int index]
    {
      get { return new Reminder(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Reminder reminder);
    partial void AfterRowInsert(Reminder reminder);
    partial void BeforeRowEdit(Reminder reminder);
    partial void AfterRowEdit(Reminder reminder);
    partial void BeforeRowDelete(int reminderID);
    partial void AfterRowDelete(int reminderID);    

    partial void BeforeDBDelete(int reminderID);
    partial void AfterDBDelete(int reminderID);    

    #endregion

    #region Public Methods

    public ReminderProxy[] GetReminderProxies()
    {
      List<ReminderProxy> list = new List<ReminderProxy>();

      foreach (Reminder item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reminderID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Reminders] WHERE ([ReminderID] = @ReminderID);";
        deleteCommand.Parameters.Add("ReminderID", SqlDbType.Int);
        deleteCommand.Parameters["ReminderID"].Value = reminderID;

        BeforeDBDelete(reminderID);
        BeforeRowDelete(reminderID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(reminderID);
        AfterDBDelete(reminderID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("RemindersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Reminders] SET     [OrganizationID] = @OrganizationID,    [RefType] = @RefType,    [RefID] = @RefID,    [Description] = @Description,    [DueDate] = @DueDate,    [UserID] = @UserID,    [IsDismissed] = @IsDismissed,    [HasEmailSent] = @HasEmailSent  WHERE ([ReminderID] = @ReminderID);";

		
		tempParameter = updateCommand.Parameters.Add("ReminderID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.NVarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsDismissed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("HasEmailSent", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Reminders] (    [OrganizationID],    [RefType],    [RefID],    [Description],    [DueDate],    [UserID],    [IsDismissed],    [HasEmailSent],    [CreatorID],    [DateCreated]) VALUES ( @OrganizationID, @RefType, @RefID, @Description, @DueDate, @UserID, @IsDismissed, @HasEmailSent, @CreatorID, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("HasEmailSent", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsDismissed", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DueDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.NVarChar, 4000);
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
		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Reminders] WHERE ([ReminderID] = @ReminderID);";
		deleteCommand.Parameters.Add("ReminderID", SqlDbType.Int);

		try
		{
		  foreach (Reminder reminder in this)
		  {
			if (reminder.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(reminder);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = reminder.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReminderID"].AutoIncrement = false;
			  Table.Columns["ReminderID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				reminder.Row["ReminderID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(reminder);
			}
			else if (reminder.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(reminder);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = reminder.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(reminder);
			}
			else if (reminder.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)reminder.Row["ReminderID", DataRowVersion.Original];
			  deleteCommand.Parameters["ReminderID"].Value = id;
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

      foreach (Reminder reminder in this)
      {
        if (reminder.Row.Table.Columns.Contains("CreatorID") && (int)reminder.Row["CreatorID"] == 0) reminder.Row["CreatorID"] = LoginUser.UserID;
        if (reminder.Row.Table.Columns.Contains("ModifierID")) reminder.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Reminder FindByReminderID(int reminderID)
    {
      foreach (Reminder reminder in this)
      {
        if (reminder.ReminderID == reminderID)
        {
          return reminder;
        }
      }
      return null;
    }

    public virtual Reminder AddNewReminder()
    {
      if (Table.Columns.Count < 1) LoadColumns("Reminders");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Reminder(row, this);
    }
    
    public virtual void LoadByReminderID(int reminderID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReminderID], [OrganizationID], [RefType], [RefID], [Description], [DueDate], [UserID], [IsDismissed], [HasEmailSent], [CreatorID], [DateCreated] FROM [dbo].[Reminders] WHERE ([ReminderID] = @ReminderID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReminderID", reminderID);
        Fill(command);
      }
    }
    
    public static Reminder GetReminder(LoginUser loginUser, int reminderID)
    {
      Reminders reminders = new Reminders(loginUser);
      reminders.LoadByReminderID(reminderID);
      if (reminders.IsEmpty)
        return null;
      else
        return reminders[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Reminder> Members

    public IEnumerator<Reminder> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Reminder(row, this);
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
