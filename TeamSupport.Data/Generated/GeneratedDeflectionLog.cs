using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class DeflectionLogItem : BaseItem
  {
    private DeflectionLog _deflectionLog;
    
    public DeflectionLogItem(DataRow row, DeflectionLog deflectionLog): base(row, deflectionLog)
    {
      _deflectionLog = deflectionLog;
    }
	
    #region Properties
    
    public DeflectionLog Collection
    {
      get { return _deflectionLog; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    
    public int? OrgID
    {
      get { return Row["OrgID"] != DBNull.Value ? (int?)Row["OrgID"] : null; }
      set { Row["OrgID"] = CheckValue("OrgID", value); }
    }
    

    
    public bool Helpful
    {
      get { return (bool)Row["Helpful"]; }
      set { Row["Helpful"] = CheckValue("Helpful", value); }
    }
    
    public string Source
    {
      get { return (string)Row["Source"]; }
      set { Row["Source"] = CheckValue("Source", value); }
    }
    
    public int TicketID
    {
      get { return (int)Row["TicketID"]; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    

    /* DateTime */
    
    
    

    

    
    public DateTime Date
    {
      get { return DateToLocal((DateTime)Row["Date"]); }
      set { Row["Date"] = CheckValue("Date", value); }
    }

    public DateTime DateUtc
    {
      get { return (DateTime)Row["Date"]; }
    }
    

    #endregion
    
    
  }

  public partial class DeflectionLog : BaseCollection, IEnumerable<DeflectionLogItem>
  {
    public DeflectionLog(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "DeflectionLog"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public DeflectionLogItem this[int index]
    {
      get { return new DeflectionLogItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(DeflectionLogItem deflectionLogItem);
    partial void AfterRowInsert(DeflectionLogItem deflectionLogItem);
    partial void BeforeRowEdit(DeflectionLogItem deflectionLogItem);
    partial void AfterRowEdit(DeflectionLogItem deflectionLogItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public DeflectionLogItemProxy[] GetDeflectionLogItemProxies()
    {
      List<DeflectionLogItemProxy> list = new List<DeflectionLogItemProxy>();

      foreach (DeflectionLogItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[DeflectionLog] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("DeflectionLogSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[DeflectionLog] SET     [TicketID] = @TicketID,    [Source] = @Source,    [Helpful] = @Helpful,    [Date] = @Date,    [UserID] = @UserID,    [OrgID] = @OrgID  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Source", SqlDbType.VarChar, 20);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Helpful", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Date", SqlDbType.DateTime, 8);
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
		
		tempParameter = updateCommand.Parameters.Add("OrgID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[DeflectionLog] (    [TicketID],    [Source],    [Helpful],    [Date],    [UserID],    [OrgID]) VALUES ( @TicketID, @Source, @Helpful, @Date, @UserID, @OrgID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OrgID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Date", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Helpful", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Source", SqlDbType.VarChar, 20);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[DeflectionLog] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (DeflectionLogItem deflectionLogItem in this)
		  {
			if (deflectionLogItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(deflectionLogItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = deflectionLogItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				deflectionLogItem.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(deflectionLogItem);
			}
			else if (deflectionLogItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(deflectionLogItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = deflectionLogItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(deflectionLogItem);
			}
			else if (deflectionLogItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)deflectionLogItem.Row["Id", DataRowVersion.Original];
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

      foreach (DeflectionLogItem deflectionLogItem in this)
      {
        if (deflectionLogItem.Row.Table.Columns.Contains("CreatorID") && (int)deflectionLogItem.Row["CreatorID"] == 0) deflectionLogItem.Row["CreatorID"] = LoginUser.UserID;
        if (deflectionLogItem.Row.Table.Columns.Contains("ModifierID")) deflectionLogItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public DeflectionLogItem FindById(int id)
    {
      foreach (DeflectionLogItem deflectionLogItem in this)
      {
        if (deflectionLogItem.Id == id)
        {
          return deflectionLogItem;
        }
      }
      return null;
    }

    public virtual DeflectionLogItem AddNewDeflectionLogItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("DeflectionLog");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new DeflectionLogItem(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [TicketID], [Source], [Helpful], [Date], [UserID], [OrgID] FROM [dbo].[DeflectionLog] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static DeflectionLogItem GetDeflectionLogItem(LoginUser loginUser, int id)
    {
      DeflectionLog deflectionLog = new DeflectionLog(loginUser);
      deflectionLog.LoadById(id);
      if (deflectionLog.IsEmpty)
        return null;
      else
        return deflectionLog[0];
    }
    
    
    

    #endregion

    #region IEnumerable<DeflectionLogItem> Members

    public IEnumerator<DeflectionLogItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new DeflectionLogItem(row, this);
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
