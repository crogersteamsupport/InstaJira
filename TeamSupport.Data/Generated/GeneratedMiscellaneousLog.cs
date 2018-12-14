using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class MiscellaneousLogItem : BaseItem
  {
    private MiscellaneousLog _miscellaneousLog;
    
    public MiscellaneousLogItem(DataRow row, MiscellaneousLog miscellaneousLog): base(row, miscellaneousLog)
    {
      _miscellaneousLog = miscellaneousLog;
    }
	
    #region Properties
    
    public MiscellaneousLog Collection
    {
      get { return _miscellaneousLog; }
    }
        
    
    
    
    public int id
    {
      get { return (int)Row["id"]; }
    }
    

    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int? RefType
    {
      get { return Row["RefType"] != DBNull.Value ? (int?)Row["RefType"] : null; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int? RefID
    {
      get { return Row["RefID"] != DBNull.Value ? (int?)Row["RefID"] : null; }
      set { Row["RefID"] = CheckValue("RefID", value); }
    }
    
    public int? RefProcess
    {
      get { return Row["RefProcess"] != DBNull.Value ? (int?)Row["RefProcess"] : null; }
      set { Row["RefProcess"] = CheckValue("RefProcess", value); }
    }
    

    

    /* DateTime */
    
    
    

    

    
    public DateTime DateModified
    {
      get { return DateToLocal((DateTime)Row["DateModified"]); }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime DateModifiedUtc
    {
      get { return (DateTime)Row["DateModified"]; }
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

  public partial class MiscellaneousLog : BaseCollection, IEnumerable<MiscellaneousLogItem>
  {
    public MiscellaneousLog(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "MiscellaneousLog"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "id"; }
    }



    public MiscellaneousLogItem this[int index]
    {
      get { return new MiscellaneousLogItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(MiscellaneousLogItem miscellaneousLogItem);
    partial void AfterRowInsert(MiscellaneousLogItem miscellaneousLogItem);
    partial void BeforeRowEdit(MiscellaneousLogItem miscellaneousLogItem);
    partial void AfterRowEdit(MiscellaneousLogItem miscellaneousLogItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public MiscellaneousLogItemProxy[] GetMiscellaneousLogItemProxies()
    {
      List<MiscellaneousLogItemProxy> list = new List<MiscellaneousLogItemProxy>();

      foreach (MiscellaneousLogItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
      BeforeDBDelete(id);
      using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
      {
        connection.Open();

        SqlCommand deleteCommand = connection.CreateCommand();

        deleteCommand.Connection = connection;
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[MiscellaneousLog] WHERE ([id] = @id);";
        deleteCommand.Parameters.Add("id", SqlDbType.Int);
        deleteCommand.Parameters["id"].Value = id;

        BeforeRowDelete(id);
        deleteCommand.ExecuteNonQuery();
		connection.Close();
        if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
        AfterRowDelete(id);
      }
      AfterDBDelete(id);
      
    }

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("MiscellaneousLogSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[MiscellaneousLog] SET     [OrganizationID] = @OrganizationID,    [RefType] = @RefType,    [RefID] = @RefID,    [RefProcess] = @RefProcess,    [DateModified] = @DateModified  WHERE ([id] = @id);";

		
		tempParameter = updateCommand.Parameters.Add("id", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("RefProcess", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[MiscellaneousLog] (    [OrganizationID],    [RefType],    [RefID],    [RefProcess],    [DateCreated],    [DateModified]) VALUES ( @OrganizationID, @RefType, @RefID, @RefProcess, @DateCreated, @DateModified); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefProcess", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[MiscellaneousLog] WHERE ([id] = @id);";
		deleteCommand.Parameters.Add("id", SqlDbType.Int);

		try
		{
		  foreach (MiscellaneousLogItem miscellaneousLogItem in this)
		  {
			if (miscellaneousLogItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(miscellaneousLogItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = miscellaneousLogItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["id"].AutoIncrement = false;
			  Table.Columns["id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				miscellaneousLogItem.Row["id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(miscellaneousLogItem);
			}
			else if (miscellaneousLogItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(miscellaneousLogItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = miscellaneousLogItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(miscellaneousLogItem);
			}
			else if (miscellaneousLogItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)miscellaneousLogItem.Row["id", DataRowVersion.Original];
			  deleteCommand.Parameters["id"].Value = id;
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

      foreach (MiscellaneousLogItem miscellaneousLogItem in this)
      {
        if (miscellaneousLogItem.Row.Table.Columns.Contains("CreatorID") && (int)miscellaneousLogItem.Row["CreatorID"] == 0) miscellaneousLogItem.Row["CreatorID"] = LoginUser.UserID;
        if (miscellaneousLogItem.Row.Table.Columns.Contains("ModifierID")) miscellaneousLogItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public MiscellaneousLogItem FindByid(int id)
    {
      foreach (MiscellaneousLogItem miscellaneousLogItem in this)
      {
        if (miscellaneousLogItem.id == id)
        {
          return miscellaneousLogItem;
        }
      }
      return null;
    }

    public virtual MiscellaneousLogItem AddNewMiscellaneousLogItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("MiscellaneousLog");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new MiscellaneousLogItem(row, this);
    }
    
    public virtual void LoadByid(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [id], [OrganizationID], [RefType], [RefID], [RefProcess], [DateCreated], [DateModified] FROM [dbo].[MiscellaneousLog] WHERE ([id] = @id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("id", id);
        Fill(command);
      }
    }
    
    public static MiscellaneousLogItem GetMiscellaneousLogItem(LoginUser loginUser, int id)
    {
      MiscellaneousLog miscellaneousLog = new MiscellaneousLog(loginUser);
      miscellaneousLog.LoadByid(id);
      if (miscellaneousLog.IsEmpty)
        return null;
      else
        return miscellaneousLog[0];
    }
    
    
    

    #endregion

    #region IEnumerable<MiscellaneousLogItem> Members

    public IEnumerator<MiscellaneousLogItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new MiscellaneousLogItem(row, this);
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
