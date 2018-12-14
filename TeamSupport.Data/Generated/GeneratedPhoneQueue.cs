using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class PhoneQueueItem : BaseItem
  {
    private PhoneQueue _phoneQueue;
    
    public PhoneQueueItem(DataRow row, PhoneQueue phoneQueue): base(row, phoneQueue)
    {
      _phoneQueue = phoneQueue;
    }
	
    #region Properties
    
    public PhoneQueue Collection
    {
      get { return _phoneQueue; }
    }
        
    
    
    
    public int PhoneQueueID
    {
      get { return (int)Row["PhoneQueueID"]; }
    }
    

    
    public string ActionValue
    {
      get { return Row["ActionValue"] != DBNull.Value ? (string)Row["ActionValue"] : null; }
      set { Row["ActionValue"] = CheckValue("ActionValue", value); }
    }
    

    
    public string Status
    {
      get { return (string)Row["Status"]; }
      set { Row["Status"] = CheckValue("Status", value); }
    }
    
    public string CallFrom
    {
      get { return (string)Row["CallFrom"]; }
      set { Row["CallFrom"] = CheckValue("CallFrom", value); }
    }
    
    public string CallTo
    {
      get { return (string)Row["CallTo"]; }
      set { Row["CallTo"] = CheckValue("CallTo", value); }
    }
    
    public string AccountSID
    {
      get { return (string)Row["AccountSID"]; }
      set { Row["AccountSID"] = CheckValue("AccountSID", value); }
    }
    
    public string CallSID
    {
      get { return (string)Row["CallSID"]; }
      set { Row["CallSID"] = CheckValue("CallSID", value); }
    }
    
    public string OrganizationID
    {
      get { return (string)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? CallDateTime
    {
      get { return Row["CallDateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["CallDateTime"]) : null; }
      set { Row["CallDateTime"] = CheckValue("CallDateTime", value); }
    }

    public DateTime? CallDateTimeUtc
    {
      get { return Row["CallDateTime"] != DBNull.Value ? (DateTime?)Row["CallDateTime"] : null; }
    }
    
    public DateTime? LastActionDateTime
    {
      get { return Row["LastActionDateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["LastActionDateTime"]) : null; }
      set { Row["LastActionDateTime"] = CheckValue("LastActionDateTime", value); }
    }

    public DateTime? LastActionDateTimeUtc
    {
      get { return Row["LastActionDateTime"] != DBNull.Value ? (DateTime?)Row["LastActionDateTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class PhoneQueue : BaseCollection, IEnumerable<PhoneQueueItem>
  {
    public PhoneQueue(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "PhoneQueue"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "PhoneQueueID"; }
    }



    public PhoneQueueItem this[int index]
    {
      get { return new PhoneQueueItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(PhoneQueueItem phoneQueueItem);
    partial void AfterRowInsert(PhoneQueueItem phoneQueueItem);
    partial void BeforeRowEdit(PhoneQueueItem phoneQueueItem);
    partial void AfterRowEdit(PhoneQueueItem phoneQueueItem);
    partial void BeforeRowDelete(int phoneQueueID);
    partial void AfterRowDelete(int phoneQueueID);    

    partial void BeforeDBDelete(int phoneQueueID);
    partial void AfterDBDelete(int phoneQueueID);    

    #endregion

    #region Public Methods

    public PhoneQueueItemProxy[] GetPhoneQueueItemProxies()
    {
      List<PhoneQueueItemProxy> list = new List<PhoneQueueItemProxy>();

      foreach (PhoneQueueItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int phoneQueueID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneQueue] WHERE ([PhoneQueueID] = @PhoneQueueID);";
        deleteCommand.Parameters.Add("PhoneQueueID", SqlDbType.Int);
        deleteCommand.Parameters["PhoneQueueID"].Value = phoneQueueID;

        BeforeDBDelete(phoneQueueID);
        BeforeRowDelete(phoneQueueID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(phoneQueueID);
        AfterDBDelete(phoneQueueID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("PhoneQueueSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[PhoneQueue] SET     [OrganizationID] = @OrganizationID,    [CallSID] = @CallSID,    [AccountSID] = @AccountSID,    [CallTo] = @CallTo,    [CallFrom] = @CallFrom,    [Status] = @Status,    [CallDateTime] = @CallDateTime,    [LastActionDateTime] = @LastActionDateTime,    [ActionValue] = @ActionValue  WHERE ([PhoneQueueID] = @PhoneQueueID);";

		
		tempParameter = updateCommand.Parameters.Add("PhoneQueueID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CallSID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("AccountSID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CallTo", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CallFrom", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Status", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CallDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastActionDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionValue", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[PhoneQueue] (    [OrganizationID],    [CallSID],    [AccountSID],    [CallTo],    [CallFrom],    [Status],    [CallDateTime],    [LastActionDateTime],    [ActionValue]) VALUES ( @OrganizationID, @CallSID, @AccountSID, @CallTo, @CallFrom, @Status, @CallDateTime, @LastActionDateTime, @ActionValue); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ActionValue", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastActionDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CallDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Status", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CallFrom", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CallTo", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("AccountSID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CallSID", SqlDbType.VarChar, 50);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.VarChar, 50);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[PhoneQueue] WHERE ([PhoneQueueID] = @PhoneQueueID);";
		deleteCommand.Parameters.Add("PhoneQueueID", SqlDbType.Int);

		try
		{
		  foreach (PhoneQueueItem phoneQueueItem in this)
		  {
			if (phoneQueueItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(phoneQueueItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = phoneQueueItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["PhoneQueueID"].AutoIncrement = false;
			  Table.Columns["PhoneQueueID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				phoneQueueItem.Row["PhoneQueueID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(phoneQueueItem);
			}
			else if (phoneQueueItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(phoneQueueItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = phoneQueueItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(phoneQueueItem);
			}
			else if (phoneQueueItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)phoneQueueItem.Row["PhoneQueueID", DataRowVersion.Original];
			  deleteCommand.Parameters["PhoneQueueID"].Value = id;
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

      foreach (PhoneQueueItem phoneQueueItem in this)
      {
        if (phoneQueueItem.Row.Table.Columns.Contains("CreatorID") && (int)phoneQueueItem.Row["CreatorID"] == 0) phoneQueueItem.Row["CreatorID"] = LoginUser.UserID;
        if (phoneQueueItem.Row.Table.Columns.Contains("ModifierID")) phoneQueueItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public PhoneQueueItem FindByPhoneQueueID(int phoneQueueID)
    {
      foreach (PhoneQueueItem phoneQueueItem in this)
      {
        if (phoneQueueItem.PhoneQueueID == phoneQueueID)
        {
          return phoneQueueItem;
        }
      }
      return null;
    }

    public virtual PhoneQueueItem AddNewPhoneQueueItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("PhoneQueue");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new PhoneQueueItem(row, this);
    }
    
    public virtual void LoadByPhoneQueueID(int phoneQueueID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [PhoneQueueID], [OrganizationID], [CallSID], [AccountSID], [CallTo], [CallFrom], [Status], [CallDateTime], [LastActionDateTime], [ActionValue] FROM [dbo].[PhoneQueue] WHERE ([PhoneQueueID] = @PhoneQueueID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("PhoneQueueID", phoneQueueID);
        Fill(command);
      }
    }
    
    public static PhoneQueueItem GetPhoneQueueItem(LoginUser loginUser, int phoneQueueID)
    {
      PhoneQueue phoneQueue = new PhoneQueue(loginUser);
      phoneQueue.LoadByPhoneQueueID(phoneQueueID);
      if (phoneQueue.IsEmpty)
        return null;
      else
        return phoneQueue[0];
    }
    
    
    

    #endregion

    #region IEnumerable<PhoneQueueItem> Members

    public IEnumerator<PhoneQueueItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new PhoneQueueItem(row, this);
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
