using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SlaPausedDay : BaseItem
  {
    private SlaPausedDays _slaPausedDays;
    
    public SlaPausedDay(DataRow row, SlaPausedDays slaPausedDays): base(row, slaPausedDays)
    {
      _slaPausedDays = slaPausedDays;
    }
	
    #region Properties
    
    public SlaPausedDays Collection
    {
      get { return _slaPausedDays; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    

    
    public int SlaTriggerId
    {
      get { return (int)Row["SlaTriggerId"]; }
      set { Row["SlaTriggerId"] = CheckValue("SlaTriggerId", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime DateToPause
    {
      get { return DateToLocal((DateTime)Row["DateToPause"]); }
      set { Row["DateToPause"] = CheckValue("DateToPause", value); }
    }

    public DateTime DateToPauseUtc
    {
      get { return (DateTime)Row["DateToPause"]; }
    }
    

    #endregion
    
    
  }

  public partial class SlaPausedDays : BaseCollection, IEnumerable<SlaPausedDay>
  {
    public SlaPausedDays(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SlaPausedDays"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public SlaPausedDay this[int index]
    {
      get { return new SlaPausedDay(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SlaPausedDay slaPausedDay);
    partial void AfterRowInsert(SlaPausedDay slaPausedDay);
    partial void BeforeRowEdit(SlaPausedDay slaPausedDay);
    partial void AfterRowEdit(SlaPausedDay slaPausedDay);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public SlaPausedDayProxy[] GetSlaPausedDayProxies()
    {
      List<SlaPausedDayProxy> list = new List<SlaPausedDayProxy>();

      foreach (SlaPausedDay item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaPausedDays] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SlaPausedDaysSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SlaPausedDays] SET     [SlaTriggerId] = @SlaTriggerId,    [DateToPause] = @DateToPause  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("SlaTriggerId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateToPause", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SlaPausedDays] (    [SlaTriggerId],    [DateToPause]) VALUES ( @SlaTriggerId, @DateToPause); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateToPause", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("SlaTriggerId", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaPausedDays] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (SlaPausedDay slaPausedDay in this)
		  {
			if (slaPausedDay.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(slaPausedDay);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = slaPausedDay.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				slaPausedDay.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(slaPausedDay);
			}
			else if (slaPausedDay.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(slaPausedDay);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = slaPausedDay.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(slaPausedDay);
			}
			else if (slaPausedDay.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)slaPausedDay.Row["Id", DataRowVersion.Original];
			  deleteCommand.Parameters["Id"].Value = id;
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

      foreach (SlaPausedDay slaPausedDay in this)
      {
        if (slaPausedDay.Row.Table.Columns.Contains("CreatorID") && (int)slaPausedDay.Row["CreatorID"] == 0) slaPausedDay.Row["CreatorID"] = LoginUser.UserID;
        if (slaPausedDay.Row.Table.Columns.Contains("ModifierID")) slaPausedDay.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SlaPausedDay FindById(int id)
    {
      foreach (SlaPausedDay slaPausedDay in this)
      {
        if (slaPausedDay.Id == id)
        {
          return slaPausedDay;
        }
      }
      return null;
    }

    public virtual SlaPausedDay AddNewSlaPausedDay()
    {
      if (Table.Columns.Count < 1) LoadColumns("SlaPausedDays");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SlaPausedDay(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [SlaTriggerId], [DateToPause] FROM [dbo].[SlaPausedDays] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static SlaPausedDay GetSlaPausedDay(LoginUser loginUser, int id)
    {
      SlaPausedDays slaPausedDays = new SlaPausedDays(loginUser);
      slaPausedDays.LoadById(id);
      if (slaPausedDays.IsEmpty)
        return null;
      else
        return slaPausedDays[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SlaPausedDay> Members

    public IEnumerator<SlaPausedDay> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SlaPausedDay(row, this);
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
