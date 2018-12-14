using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CalendarEvent : BaseItem
  {
    private CalendarEvents _calendarEvents;
    
    public CalendarEvent(DataRow row, CalendarEvents calendarEvents): base(row, calendarEvents)
    {
      _calendarEvents = calendarEvents;
    }
	
    #region Properties
    
    public CalendarEvents Collection
    {
      get { return _calendarEvents; }
    }
        
    
    
    
    public int CalendarID
    {
      get { return (int)Row["CalendarID"]; }
    }
    

    
    public string Description
    {
      get { return Row["Description"] != DBNull.Value ? (string)Row["Description"] : null; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public bool? Repeat
    {
      get { return Row["Repeat"] != DBNull.Value ? (bool?)Row["Repeat"] : null; }
      set { Row["Repeat"] = CheckValue("Repeat", value); }
    }
    
    public int? RepeatFrequency
    {
      get { return Row["RepeatFrequency"] != DBNull.Value ? (int?)Row["RepeatFrequency"] : null; }
      set { Row["RepeatFrequency"] = CheckValue("RepeatFrequency", value); }
    }
    

    
    public bool IsHoliday
    {
      get { return (bool)Row["IsHoliday"]; }
      set { Row["IsHoliday"] = CheckValue("IsHoliday", value); }
    }
    
    public bool AllDay
    {
      get { return (bool)Row["AllDay"]; }
      set { Row["AllDay"] = CheckValue("AllDay", value); }
    }
    
    public int CreatorID
    {
      get { return (int)Row["CreatorID"]; }
      set { Row["CreatorID"] = CheckValue("CreatorID", value); }
    }
    
    public string Title
    {
      get { return (string)Row["Title"]; }
      set { Row["Title"] = CheckValue("Title", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? EndDate
    {
      get { return Row["EndDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["EndDate"]) : null; }
      set { Row["EndDate"] = CheckValue("EndDate", value); }
    }

    public DateTime? EndDateUtc
    {
      get { return Row["EndDate"] != DBNull.Value ? (DateTime?)Row["EndDate"] : null; }
    }
    
    public DateTime? StartDateUTC
    {
      get { return Row["StartDateUTC"] != DBNull.Value ? DateToLocal((DateTime?)Row["StartDateUTC"]) : null; }
      set { Row["StartDateUTC"] = CheckValue("StartDateUTC", value); }
    }

    public DateTime? StartDateUTCUtc
    {
      get { return Row["StartDateUTC"] != DBNull.Value ? (DateTime?)Row["StartDateUTC"] : null; }
    }
    
    public DateTime? EndDateUTC
    {
      get { return Row["EndDateUTC"] != DBNull.Value ? DateToLocal((DateTime?)Row["EndDateUTC"]) : null; }
      set { Row["EndDateUTC"] = CheckValue("EndDateUTC", value); }
    }

    public DateTime? EndDateUTCUtc
    {
      get { return Row["EndDateUTC"] != DBNull.Value ? (DateTime?)Row["EndDateUTC"] : null; }
    }
    

    
    public DateTime LastModified
    {
      get { return DateToLocal((DateTime)Row["LastModified"]); }
      set { Row["LastModified"] = CheckValue("LastModified", value); }
    }

    public DateTime LastModifiedUtc
    {
      get { return (DateTime)Row["LastModified"]; }
    }
    
    public DateTime StartDate
    {
      get { return DateToLocal((DateTime)Row["StartDate"]); }
      set { Row["StartDate"] = CheckValue("StartDate", value); }
    }

    public DateTime StartDateUtc
    {
      get { return (DateTime)Row["StartDate"]; }
    }
    

    #endregion
    
    
  }

  public partial class CalendarEvents : BaseCollection, IEnumerable<CalendarEvent>
  {
    public CalendarEvents(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CalendarEvents"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CalendarID"; }
    }



    public CalendarEvent this[int index]
    {
      get { return new CalendarEvent(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CalendarEvent calendarEvent);
    partial void AfterRowInsert(CalendarEvent calendarEvent);
    partial void BeforeRowEdit(CalendarEvent calendarEvent);
    partial void AfterRowEdit(CalendarEvent calendarEvent);
    partial void BeforeRowDelete(int calendarID);
    partial void AfterRowDelete(int calendarID);    

    partial void BeforeDBDelete(int calendarID);
    partial void AfterDBDelete(int calendarID);    

    #endregion

    #region Public Methods

    public CalendarEventProxy[] GetCalendarEventProxies()
    {
      List<CalendarEventProxy> list = new List<CalendarEventProxy>();

      foreach (CalendarEvent item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int calendarID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CalendarEvents] WHERE ([CalendarID] = @CalendarID);";
        deleteCommand.Parameters.Add("CalendarID", SqlDbType.Int);
        deleteCommand.Parameters["CalendarID"].Value = calendarID;

        BeforeDBDelete(calendarID);
        BeforeRowDelete(calendarID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(calendarID);
        AfterDBDelete(calendarID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CalendarEventsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CalendarEvents] SET     [OrganizationID] = @OrganizationID,    [StartDate] = @StartDate,    [EndDate] = @EndDate,    [Title] = @Title,    [Description] = @Description,    [Repeat] = @Repeat,    [RepeatFrequency] = @RepeatFrequency,    [LastModified] = @LastModified,    [AllDay] = @AllDay,    [StartDateUTC] = @StartDateUTC,    [EndDateUTC] = @EndDateUTC,    [IsHoliday] = @IsHoliday  WHERE ([CalendarID] = @CalendarID);";

		
		tempParameter = updateCommand.Parameters.Add("CalendarID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("StartDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("EndDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("Title", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Repeat", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("RepeatFrequency", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("AllDay", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("StartDateUTC", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("EndDateUTC", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsHoliday", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CalendarEvents] (    [OrganizationID],    [StartDate],    [EndDate],    [Title],    [Description],    [Repeat],    [RepeatFrequency],    [LastModified],    [CreatorID],    [AllDay],    [StartDateUTC],    [EndDateUTC],    [IsHoliday]) VALUES ( @OrganizationID, @StartDate, @EndDate, @Title, @Description, @Repeat, @RepeatFrequency, @LastModified, @CreatorID, @AllDay, @StartDateUTC, @EndDateUTC, @IsHoliday); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("IsHoliday", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EndDateUTC", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("StartDateUTC", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("AllDay", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("LastModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("RepeatFrequency", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Repeat", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Title", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EndDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("StartDate", SqlDbType.DateTime, 8);
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
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CalendarEvents] WHERE ([CalendarID] = @CalendarID);";
		deleteCommand.Parameters.Add("CalendarID", SqlDbType.Int);

		try
		{
		  foreach (CalendarEvent calendarEvent in this)
		  {
			if (calendarEvent.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(calendarEvent);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = calendarEvent.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CalendarID"].AutoIncrement = false;
			  Table.Columns["CalendarID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				calendarEvent.Row["CalendarID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(calendarEvent);
			}
			else if (calendarEvent.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(calendarEvent);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = calendarEvent.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(calendarEvent);
			}
			else if (calendarEvent.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)calendarEvent.Row["CalendarID", DataRowVersion.Original];
			  deleteCommand.Parameters["CalendarID"].Value = id;
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

      foreach (CalendarEvent calendarEvent in this)
      {
        if (calendarEvent.Row.Table.Columns.Contains("CreatorID") && (int)calendarEvent.Row["CreatorID"] == 0) calendarEvent.Row["CreatorID"] = LoginUser.UserID;
        if (calendarEvent.Row.Table.Columns.Contains("ModifierID")) calendarEvent.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CalendarEvent FindByCalendarID(int calendarID)
    {
      foreach (CalendarEvent calendarEvent in this)
      {
        if (calendarEvent.CalendarID == calendarID)
        {
          return calendarEvent;
        }
      }
      return null;
    }

    public virtual CalendarEvent AddNewCalendarEvent()
    {
      if (Table.Columns.Count < 1) LoadColumns("CalendarEvents");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CalendarEvent(row, this);
    }
    
    public virtual void LoadByCalendarID(int calendarID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CalendarID], [OrganizationID], [StartDate], [EndDate], [Title], [Description], [Repeat], [RepeatFrequency], [LastModified], [CreatorID], [AllDay], [StartDateUTC], [EndDateUTC], [IsHoliday] FROM [dbo].[CalendarEvents] WHERE ([CalendarID] = @CalendarID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CalendarID", calendarID);
        Fill(command);
      }
    }
    
    public static CalendarEvent GetCalendarEvent(LoginUser loginUser, int calendarID)
    {
      CalendarEvents calendarEvents = new CalendarEvents(loginUser);
      calendarEvents.LoadByCalendarID(calendarID);
      if (calendarEvents.IsEmpty)
        return null;
      else
        return calendarEvents[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CalendarEvent> Members

    public IEnumerator<CalendarEvent> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CalendarEvent(row, this);
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
