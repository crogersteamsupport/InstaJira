using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class RemindersViewItem : BaseItem
  {
    private RemindersView _remindersView;
    
    public RemindersViewItem(DataRow row, RemindersView remindersView): base(row, remindersView)
    {
      _remindersView = remindersView;
    }
	
    #region Properties
    
    public RemindersView Collection
    {
      get { return _remindersView; }
    }
        
    
    public string UserName
    {
      get { return Row["UserName"] != DBNull.Value ? (string)Row["UserName"] : null; }
    }
    
    public string Creator
    {
      get { return Row["Creator"] != DBNull.Value ? (string)Row["Creator"] : null; }
    }
    
    public string ReminderTarget
    {
      get { return Row["ReminderTarget"] != DBNull.Value ? (string)Row["ReminderTarget"] : null; }
    }
    
    
    
    public string ReminderType
    {
      get { return (string)Row["ReminderType"]; }
    }
    

    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
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
    
    public int ReminderID
    {
      get { return (int)Row["ReminderID"]; }
      set { Row["ReminderID"] = CheckValue("ReminderID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DueDate
    {
      get { return Row["DueDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["DueDate"]) : null; }
      set { Row["DueDate"] = CheckValue("DueDate", value); }
    }

    public DateTime? DueDateUtc
    {
      get { return Row["DueDate"] != DBNull.Value ? (DateTime?)Row["DueDate"] : null; }
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

  public partial class RemindersView : BaseCollection, IEnumerable<RemindersViewItem>
  {
    public RemindersView(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "RemindersView"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ReminderID"; }
    }



    public RemindersViewItem this[int index]
    {
      get { return new RemindersViewItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(RemindersViewItem remindersViewItem);
    partial void AfterRowInsert(RemindersViewItem remindersViewItem);
    partial void BeforeRowEdit(RemindersViewItem remindersViewItem);
    partial void AfterRowEdit(RemindersViewItem remindersViewItem);
    partial void BeforeRowDelete(int reminderID);
    partial void AfterRowDelete(int reminderID);    

    partial void BeforeDBDelete(int reminderID);
    partial void AfterDBDelete(int reminderID);    

    #endregion

    #region Public Methods

    public RemindersViewItemProxy[] GetRemindersViewItemProxies()
    {
      List<RemindersViewItemProxy> list = new List<RemindersViewItemProxy>();

      foreach (RemindersViewItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int reminderID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[RemindersView] WHERE ([ReminderID] = @ReminderID);";
        deleteCommand.Parameters.Add("ReminderID", SqlDbType.Int);
        deleteCommand.Parameters["ReminderID"].Value = reminderID;

        BeforeDBDelete(reminderID);
        BeforeRowDelete(reminderID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(reminderID);
        AfterDBDelete(reminderID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("RemindersViewSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[RemindersView] SET     [OrganizationID] = @OrganizationID,    [RefType] = @RefType,    [RefID] = @RefID,    [Description] = @Description,    [DueDate] = @DueDate,    [UserID] = @UserID,    [IsDismissed] = @IsDismissed,    [HasEmailSent] = @HasEmailSent,    [UserName] = @UserName,    [Creator] = @Creator,    [ReminderType] = @ReminderType,    [ReminderTarget] = @ReminderTarget  WHERE ([ReminderID] = @ReminderID);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("UserName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Creator", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReminderType", SqlDbType.VarChar, 7);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ReminderTarget", SqlDbType.NVarChar, 459);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[RemindersView] (    [ReminderID],    [OrganizationID],    [RefType],    [RefID],    [Description],    [DueDate],    [UserID],    [IsDismissed],    [HasEmailSent],    [CreatorID],    [DateCreated],    [UserName],    [Creator],    [ReminderType],    [ReminderTarget]) VALUES ( @ReminderID, @OrganizationID, @RefType, @RefID, @Description, @DueDate, @UserID, @IsDismissed, @HasEmailSent, @CreatorID, @DateCreated, @UserName, @Creator, @ReminderType, @ReminderTarget); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ReminderTarget", SqlDbType.NVarChar, 459);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReminderType", SqlDbType.VarChar, 7);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Creator", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UserName", SqlDbType.NVarChar, 201);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
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
		
		tempParameter = insertCommand.Parameters.Add("ReminderID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[RemindersView] WHERE ([ReminderID] = @ReminderID);";
		deleteCommand.Parameters.Add("ReminderID", SqlDbType.Int);

		try
		{
		  foreach (RemindersViewItem remindersViewItem in this)
		  {
			if (remindersViewItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(remindersViewItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = remindersViewItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ReminderID"].AutoIncrement = false;
			  Table.Columns["ReminderID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				remindersViewItem.Row["ReminderID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(remindersViewItem);
			}
			else if (remindersViewItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(remindersViewItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = remindersViewItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(remindersViewItem);
			}
			else if (remindersViewItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)remindersViewItem.Row["ReminderID", DataRowVersion.Original];
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

      foreach (RemindersViewItem remindersViewItem in this)
      {
        if (remindersViewItem.Row.Table.Columns.Contains("CreatorID") && (int)remindersViewItem.Row["CreatorID"] == 0) remindersViewItem.Row["CreatorID"] = LoginUser.UserID;
        if (remindersViewItem.Row.Table.Columns.Contains("ModifierID")) remindersViewItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public RemindersViewItem FindByReminderID(int reminderID)
    {
      foreach (RemindersViewItem remindersViewItem in this)
      {
        if (remindersViewItem.ReminderID == reminderID)
        {
          return remindersViewItem;
        }
      }
      return null;
    }

    public virtual RemindersViewItem AddNewRemindersViewItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("RemindersView");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new RemindersViewItem(row, this);
    }
    
    public virtual void LoadByReminderID(int reminderID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ReminderID], [OrganizationID], [RefType], [RefID], [Description], [DueDate], [UserID], [IsDismissed], [HasEmailSent], [CreatorID], [DateCreated], [UserName], [Creator], [ReminderType], [ReminderTarget] FROM [dbo].[RemindersView] WHERE ([ReminderID] = @ReminderID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ReminderID", reminderID);
        Fill(command);
      }
    }
    
    public static RemindersViewItem GetRemindersViewItem(LoginUser loginUser, int reminderID)
    {
      RemindersView remindersView = new RemindersView(loginUser);
      remindersView.LoadByReminderID(reminderID);
      if (remindersView.IsEmpty)
        return null;
      else
        return remindersView[0];
    }
    
    
    

    #endregion

    #region IEnumerable<RemindersViewItem> Members

    public IEnumerator<RemindersViewItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new RemindersViewItem(row, this);
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
