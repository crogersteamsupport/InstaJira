using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ScheduledReportsRecurrencyItem : BaseItem
  {
    private ScheduledReportsRecurrency _scheduledReportsRecurrency;
    
    public ScheduledReportsRecurrencyItem(DataRow row, ScheduledReportsRecurrency scheduledReportsRecurrency): base(row, scheduledReportsRecurrency)
    {
      _scheduledReportsRecurrency = scheduledReportsRecurrency;
    }
	
    #region Properties
    
    public ScheduledReportsRecurrency Collection
    {
      get { return _scheduledReportsRecurrency; }
    }
        
    
    
    
    public byte id
    {
      get { return (byte)Row["id"]; }
    }
    

    
    public string recurrency
    {
      get { return Row["recurrency"] != DBNull.Value ? (string)Row["recurrency"] : null; }
      set { Row["recurrency"] = CheckValue("recurrency", value); }
    }
    

    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class ScheduledReportsRecurrency : BaseCollection, IEnumerable<ScheduledReportsRecurrencyItem>
  {
    public ScheduledReportsRecurrency(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ScheduledReportsRecurrency"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "id"; }
    }



    public ScheduledReportsRecurrencyItem this[int index]
    {
      get { return new ScheduledReportsRecurrencyItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ScheduledReportsRecurrencyItem scheduledReportsRecurrencyItem);
    partial void AfterRowInsert(ScheduledReportsRecurrencyItem scheduledReportsRecurrencyItem);
    partial void BeforeRowEdit(ScheduledReportsRecurrencyItem scheduledReportsRecurrencyItem);
    partial void AfterRowEdit(ScheduledReportsRecurrencyItem scheduledReportsRecurrencyItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public ScheduledReportsRecurrencyItemProxy[] GetScheduledReportsRecurrencyItemProxies()
    {
      List<ScheduledReportsRecurrencyItemProxy> list = new List<ScheduledReportsRecurrencyItemProxy>();

      foreach (ScheduledReportsRecurrencyItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ScheduledReportsRecurrency] WHERE ([id] = @id);";
        deleteCommand.Parameters.Add("id", SqlDbType.Int);
        deleteCommand.Parameters["id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ScheduledReportsRecurrencySave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ScheduledReportsRecurrency] SET     [recurrency] = @recurrency  WHERE ([id] = @id);";

		
		tempParameter = updateCommand.Parameters.Add("id", SqlDbType.TinyInt, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 3;
		  tempParameter.Scale = 3;
		}
		
		tempParameter = updateCommand.Parameters.Add("recurrency", SqlDbType.VarChar, 30);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ScheduledReportsRecurrency] (    [recurrency]) VALUES ( @recurrency); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("recurrency", SqlDbType.VarChar, 30);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ScheduledReportsRecurrency] WHERE ([id] = @id);";
		deleteCommand.Parameters.Add("id", SqlDbType.Int);

		try
		{
		  foreach (ScheduledReportsRecurrencyItem scheduledReportsRecurrencyItem in this)
		  {
			if (scheduledReportsRecurrencyItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(scheduledReportsRecurrencyItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = scheduledReportsRecurrencyItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["id"].AutoIncrement = false;
			  Table.Columns["id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				scheduledReportsRecurrencyItem.Row["id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(scheduledReportsRecurrencyItem);
			}
			else if (scheduledReportsRecurrencyItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(scheduledReportsRecurrencyItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = scheduledReportsRecurrencyItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(scheduledReportsRecurrencyItem);
			}
			else if (scheduledReportsRecurrencyItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)scheduledReportsRecurrencyItem.Row["id", DataRowVersion.Original];
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

      foreach (ScheduledReportsRecurrencyItem scheduledReportsRecurrencyItem in this)
      {
        if (scheduledReportsRecurrencyItem.Row.Table.Columns.Contains("CreatorID") && (int)scheduledReportsRecurrencyItem.Row["CreatorID"] == 0) scheduledReportsRecurrencyItem.Row["CreatorID"] = LoginUser.UserID;
        if (scheduledReportsRecurrencyItem.Row.Table.Columns.Contains("ModifierID")) scheduledReportsRecurrencyItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ScheduledReportsRecurrencyItem FindByid(int id)
    {
      foreach (ScheduledReportsRecurrencyItem scheduledReportsRecurrencyItem in this)
      {
        if (scheduledReportsRecurrencyItem.id == id)
        {
          return scheduledReportsRecurrencyItem;
        }
      }
      return null;
    }

    public virtual ScheduledReportsRecurrencyItem AddNewScheduledReportsRecurrencyItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("ScheduledReportsRecurrency");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ScheduledReportsRecurrencyItem(row, this);
    }
    
    public virtual void LoadByid(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [id], [recurrency] FROM [dbo].[ScheduledReportsRecurrency] WHERE ([id] = @id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("id", id);
        Fill(command);
      }
    }
    
    public static ScheduledReportsRecurrencyItem GetScheduledReportsRecurrencyItem(LoginUser loginUser, int id)
    {
      ScheduledReportsRecurrency scheduledReportsRecurrency = new ScheduledReportsRecurrency(loginUser);
      scheduledReportsRecurrency.LoadByid(id);
      if (scheduledReportsRecurrency.IsEmpty)
        return null;
      else
        return scheduledReportsRecurrency[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ScheduledReportsRecurrencyItem> Members

    public IEnumerator<ScheduledReportsRecurrencyItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ScheduledReportsRecurrencyItem(row, this);
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
