using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class SlaPausedTim : BaseItem
  {
    private SlaPausedTimes _slaPausedTimes;
    
    public SlaPausedTim(DataRow row, SlaPausedTimes slaPausedTimes): base(row, slaPausedTimes)
    {
      _slaPausedTimes = slaPausedTimes;
    }
	
    #region Properties
    
    public SlaPausedTimes Collection
    {
      get { return _slaPausedTimes; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    
    public int? BusinessPausedTime
    {
      get { return Row["BusinessPausedTime"] != DBNull.Value ? (int?)Row["BusinessPausedTime"] : null; }
      set { Row["BusinessPausedTime"] = CheckValue("BusinessPausedTime", value); }
    }
    

    
    public int SlaTriggerId
    {
      get { return (int)Row["SlaTriggerId"]; }
      set { Row["SlaTriggerId"] = CheckValue("SlaTriggerId", value); }
    }
    
    public int TicketStatusId
    {
      get { return (int)Row["TicketStatusId"]; }
      set { Row["TicketStatusId"] = CheckValue("TicketStatusId", value); }
    }
    
    public int TicketId
    {
      get { return (int)Row["TicketId"]; }
      set { Row["TicketId"] = CheckValue("TicketId", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? ResumedOn
    {
      get { return Row["ResumedOn"] != DBNull.Value ? DateToLocal((DateTime?)Row["ResumedOn"]) : null; }
      set { Row["ResumedOn"] = CheckValue("ResumedOn", value); }
    }

    public DateTime? ResumedOnUtc
    {
      get { return Row["ResumedOn"] != DBNull.Value ? (DateTime?)Row["ResumedOn"] : null; }
    }
    

    
    public DateTime PausedOn
    {
      get { return DateToLocal((DateTime)Row["PausedOn"]); }
      set { Row["PausedOn"] = CheckValue("PausedOn", value); }
    }

    public DateTime PausedOnUtc
    {
      get { return (DateTime)Row["PausedOn"]; }
    }
    

    #endregion
    
    
  }

  public partial class SlaPausedTimes : BaseCollection, IEnumerable<SlaPausedTim>
  {
    public SlaPausedTimes(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "SlaPausedTimes"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public SlaPausedTim this[int index]
    {
      get { return new SlaPausedTim(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(SlaPausedTim slaPausedTim);
    partial void AfterRowInsert(SlaPausedTim slaPausedTim);
    partial void BeforeRowEdit(SlaPausedTim slaPausedTim);
    partial void AfterRowEdit(SlaPausedTim slaPausedTim);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public SlaPausedTimProxy[] GetSlaPausedTimProxies()
    {
      List<SlaPausedTimProxy> list = new List<SlaPausedTimProxy>();

      foreach (SlaPausedTim item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaPausedTimes] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("SlaPausedTimesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[SlaPausedTimes] SET     [TicketId] = @TicketId,    [TicketStatusId] = @TicketStatusId,    [SlaTriggerId] = @SlaTriggerId,    [PausedOn] = @PausedOn,    [ResumedOn] = @ResumedOn,    [BusinessPausedTime] = @BusinessPausedTime  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketStatusId", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("PausedOn", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ResumedOn", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("BusinessPausedTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[SlaPausedTimes] (    [TicketId],    [TicketStatusId],    [SlaTriggerId],    [PausedOn],    [ResumedOn],    [BusinessPausedTime]) VALUES ( @TicketId, @TicketStatusId, @SlaTriggerId, @PausedOn, @ResumedOn, @BusinessPausedTime); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("BusinessPausedTime", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ResumedOn", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("PausedOn", SqlDbType.DateTime, 8);
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
		
		tempParameter = insertCommand.Parameters.Add("TicketStatusId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketId", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[SlaPausedTimes] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (SlaPausedTim slaPausedTim in this)
		  {
			if (slaPausedTim.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(slaPausedTim);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = slaPausedTim.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				slaPausedTim.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(slaPausedTim);
			}
			else if (slaPausedTim.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(slaPausedTim);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = slaPausedTim.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(slaPausedTim);
			}
			else if (slaPausedTim.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)slaPausedTim.Row["Id", DataRowVersion.Original];
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

      foreach (SlaPausedTim slaPausedTim in this)
      {
        if (slaPausedTim.Row.Table.Columns.Contains("CreatorID") && (int)slaPausedTim.Row["CreatorID"] == 0) slaPausedTim.Row["CreatorID"] = LoginUser.UserID;
        if (slaPausedTim.Row.Table.Columns.Contains("ModifierID")) slaPausedTim.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public SlaPausedTim FindById(int id)
    {
      foreach (SlaPausedTim slaPausedTim in this)
      {
        if (slaPausedTim.Id == id)
        {
          return slaPausedTim;
        }
      }
      return null;
    }

    public virtual SlaPausedTim AddNewSlaPausedTim()
    {
      if (Table.Columns.Count < 1) LoadColumns("SlaPausedTimes");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new SlaPausedTim(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [TicketId], [TicketStatusId], [SlaTriggerId], [PausedOn], [ResumedOn], [BusinessPausedTime] FROM [dbo].[SlaPausedTimes] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static SlaPausedTim GetSlaPausedTim(LoginUser loginUser, int id)
    {
      SlaPausedTimes slaPausedTimes = new SlaPausedTimes(loginUser);
      slaPausedTimes.LoadById(id);
      if (slaPausedTimes.IsEmpty)
        return null;
      else
        return slaPausedTimes[0];
    }
    
    
    

    #endregion

    #region IEnumerable<SlaPausedTim> Members

    public IEnumerator<SlaPausedTim> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new SlaPausedTim(row, this);
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
