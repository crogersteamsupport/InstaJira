using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class CalendarRefItem : BaseItem
  {
    private CalendarRef _calendarRef;
    
    public CalendarRefItem(DataRow row, CalendarRef calendarRef): base(row, calendarRef)
    {
      _calendarRef = calendarRef;
    }
	
    #region Properties
    
    public CalendarRef Collection
    {
      get { return _calendarRef; }
    }
        
    
    
    

    
    public int RefType
    {
        get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    

    
    public int RefID
    {
      get { return (int)Row["RefID"]; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public int CalendarID
    {
      get { return (int)Row["CalendarID"]; }
      set { Row["CalendarID"] = CheckValue("CalendarID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class CalendarRef : BaseCollection, IEnumerable<CalendarRefItem>
  {
    public CalendarRef(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "CalendarRef"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "CalendarID"; }
    }



    public CalendarRefItem this[int index]
    {
      get { return new CalendarRefItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(CalendarRefItem calendarRefItem);
    partial void AfterRowInsert(CalendarRefItem calendarRefItem);
    partial void BeforeRowEdit(CalendarRefItem calendarRefItem);
    partial void AfterRowEdit(CalendarRefItem calendarRefItem);
    partial void BeforeRowDelete(int calendarID);
    partial void AfterRowDelete(int calendarID);    

    partial void BeforeDBDelete(int calendarID);
    partial void AfterDBDelete(int calendarID);    

    #endregion

    #region Public Methods

    public CalendarRefItemProxy[] GetCalendarRefItemProxies()
    {
      List<CalendarRefItemProxy> list = new List<CalendarRefItemProxy>();

      foreach (CalendarRefItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int calendarID)
    {
      BeforeDBDelete(calendarID);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CalendarRef] WHERE ([CalendarID] = @CalendarID);";
        deleteCommand.Parameters.Add("CalendarID", SqlDbType.Int);
        deleteCommand.Parameters["CalendarID"].Value = calendarID;

        BeforeRowDelete(calendarID);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(calendarID);
      }
      AfterDBDelete(calendarID);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("CalendarRefSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[CalendarRef] SET     [RefID] = @RefID,    [RefType] = @RefType  WHERE ([CalendarID] = @CalendarID);";

		
		tempParameter = updateCommand.Parameters.Add("CalendarID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[CalendarRef] (    [CalendarID],    [RefID],    [RefType]) VALUES ( @CalendarID, @RefID, @RefType); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CalendarID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[CalendarRef] WHERE ([CalendarID] = @CalendarID);";
		deleteCommand.Parameters.Add("CalendarID", SqlDbType.Int);

		try
		{
		  foreach (CalendarRefItem calendarRefItem in this)
		  {
			if (calendarRefItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(calendarRefItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = calendarRefItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["CalendarID"].AutoIncrement = false;
			  Table.Columns["CalendarID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				calendarRefItem.Row["CalendarID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(calendarRefItem);
			}
			else if (calendarRefItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(calendarRefItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = calendarRefItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(calendarRefItem);
			}
			else if (calendarRefItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)calendarRefItem.Row["CalendarID", DataRowVersion.Original];
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

      foreach (CalendarRefItem calendarRefItem in this)
      {
        if (calendarRefItem.Row.Table.Columns.Contains("CreatorID") && (int)calendarRefItem.Row["CreatorID"] == 0) calendarRefItem.Row["CreatorID"] = LoginUser.UserID;
        if (calendarRefItem.Row.Table.Columns.Contains("ModifierID")) calendarRefItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public CalendarRefItem FindByCalendarID(int calendarID)
    {
      foreach (CalendarRefItem calendarRefItem in this)
      {
        if (calendarRefItem.CalendarID == calendarID)
        {
          return calendarRefItem;
        }
      }
      return null;
    }

    public virtual CalendarRefItem AddNewCalendarRefItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("CalendarRef");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new CalendarRefItem(row, this);
    }
    
    public virtual void LoadByCalendarID(int calendarID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [CalendarID], [RefID], [RefType] FROM [dbo].[CalendarRef] WHERE ([CalendarID] = @CalendarID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("CalendarID", calendarID);
        Fill(command);
      }
    }
    
    public static CalendarRefItem GetCalendarRefItem(LoginUser loginUser, int calendarID)
    {
      CalendarRef calendarRef = new CalendarRef(loginUser);
      calendarRef.LoadByCalendarID(calendarID);
      if (calendarRef.IsEmpty)
        return null;
      else
        return calendarRef[0];
    }
    
    
    

    #endregion

    #region IEnumerable<CalendarRefItem> Members

    public IEnumerator<CalendarRefItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new CalendarRefItem(row, this);
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
