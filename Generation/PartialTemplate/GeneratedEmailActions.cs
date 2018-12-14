using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class EmailAction : BaseItem
  {
    private EmailActions _emailActions;
    
    public EmailAction(DataRow row, EmailActions emailActions): base(row, emailActions)
    {
      _emailActions = emailActions;
    }
	
    #region Properties
    
    public EmailActions Collection
    {
      get { return _emailActions; }
    }
        
    
    
    
    public int EMailActionID
    {
      get { return (int)Row["EMailActionID"]; }
    }
    

    
    public string EMailFrom
    {
      get { return Row["EMailFrom"] != DBNull.Value ? (string)Row["EMailFrom"] : null; }
      set { Row["EMailFrom"] = CheckValue("EMailFrom", value); }
    }
    
    public string EMailTo
    {
      get { return Row["EMailTo"] != DBNull.Value ? (string)Row["EMailTo"] : null; }
      set { Row["EMailTo"] = CheckValue("EMailTo", value); }
    }
    
    public string EMailSubject
    {
      get { return Row["EMailSubject"] != DBNull.Value ? (string)Row["EMailSubject"] : null; }
      set { Row["EMailSubject"] = CheckValue("EMailSubject", value); }
    }
    
    public string EMailBody
    {
      get { return Row["EMailBody"] != DBNull.Value ? (string)Row["EMailBody"] : null; }
      set { Row["EMailBody"] = CheckValue("EMailBody", value); }
    }
    
    public string OrganizationGUID
    {
      get { return Row["OrganizationGUID"] != DBNull.Value ? (string)Row["OrganizationGUID"] : null; }
      set { Row["OrganizationGUID"] = CheckValue("OrganizationGUID", value); }
    }
    
    public bool? ActionAdded
    {
      get { return Row["ActionAdded"] != DBNull.Value ? (bool?)Row["ActionAdded"] : null; }
      set { Row["ActionAdded"] = CheckValue("ActionAdded", value); }
    }
    
    public string Status
    {
      get { return Row["Status"] != DBNull.Value ? (string)Row["Status"] : null; }
      set { Row["Status"] = CheckValue("Status", value); }
    }
    
    public int? TicketID
    {
      get { return Row["TicketID"] != DBNull.Value ? (int?)Row["TicketID"] : null; }
      set { Row["TicketID"] = CheckValue("TicketID", value); }
    }
    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    

    /* DateTime */
    
    
    

    
    public DateTime? DateTime
    {
      get { return Row["DateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateTime"]) : null; }
      set { Row["DateTime"] = CheckValue("DateTime", value); }
    }

    public DateTime? DateTimeUtc
    {
      get { return Row["DateTime"] != DBNull.Value ? (DateTime?)Row["DateTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class EmailActions : BaseCollection, IEnumerable<EmailAction>
  {
    public EmailActions(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "EMailActionTable"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "EMailActionID"; }
    }



    public EmailAction this[int index]
    {
      get { return new EmailAction(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(EmailAction emailAction);
    partial void AfterRowInsert(EmailAction emailAction);
    partial void BeforeRowEdit(EmailAction emailAction);
    partial void AfterRowEdit(EmailAction emailAction);
    partial void BeforeRowDelete(int eMailActionID);
    partial void AfterRowDelete(int eMailActionID);    

    partial void BeforeDBDelete(int eMailActionID);
    partial void AfterDBDelete(int eMailActionID);    

    #endregion

    #region Public Methods

    public EmailActionProxy[] GetEmailActionProxies()
    {
      List<EmailActionProxy> list = new List<EmailActionProxy>();

      foreach (EmailAction item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int eMailActionID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EMailActionTable] WHERE ([EMailActionID] = @EMailActionID);";
        deleteCommand.Parameters.Add("EMailActionID", SqlDbType.Int);
        deleteCommand.Parameters["EMailActionID"].Value = eMailActionID;

        BeforeDBDelete(eMailActionID);
        BeforeRowDelete(eMailActionID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(eMailActionID);
        AfterDBDelete(eMailActionID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("EmailActionsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[EMailActionTable] SET     [DateTime] = @DateTime,    [EMailFrom] = @EMailFrom,    [EMailTo] = @EMailTo,    [EMailSubject] = @EMailSubject,    [EMailBody] = @EMailBody,    [OrganizationGUID] = @OrganizationGUID,    [ActionAdded] = @ActionAdded,    [Status] = @Status,    [TicketID] = @TicketID,    [OrganizationID] = @OrganizationID  WHERE ([EMailActionID] = @EMailActionID);";

		
		tempParameter = updateCommand.Parameters.Add("EMailActionID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("EMailFrom", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EMailTo", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EMailSubject", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EMailBody", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationGUID", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ActionAdded", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Status", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[EMailActionTable] (    [DateTime],    [EMailFrom],    [EMailTo],    [EMailSubject],    [EMailBody],    [OrganizationGUID],    [ActionAdded],    [Status],    [TicketID],    [OrganizationID]) VALUES ( @DateTime, @EMailFrom, @EMailTo, @EMailSubject, @EMailBody, @OrganizationGUID, @ActionAdded, @Status, @TicketID, @OrganizationID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("OrganizationID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("TicketID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Status", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ActionAdded", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationGUID", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EMailBody", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EMailSubject", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EMailTo", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("EMailFrom", SqlDbType.VarChar, 200);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		

		insertCommand.Parameters.Add("Identity", SqlDbType.Int).Direction = ParameterDirection.Output;
		SqlCommand deleteCommand = connection.CreateCommand();
		deleteCommand.Connection = connection;
		//deleteCommand.Transaction = transaction;
		deleteCommand.CommandType = CommandType.Text;
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EMailActionTable] WHERE ([EMailActionID] = @EMailActionID);";
		deleteCommand.Parameters.Add("EMailActionID", SqlDbType.Int);

		try
		{
		  foreach (EmailAction emailAction in this)
		  {
			if (emailAction.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(emailAction);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = emailAction.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["EMailActionID"].AutoIncrement = false;
			  Table.Columns["EMailActionID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				emailAction.Row["EMailActionID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(emailAction);
			}
			else if (emailAction.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(emailAction);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = emailAction.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(emailAction);
			}
			else if (emailAction.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)emailAction.Row["EMailActionID", DataRowVersion.Original];
			  deleteCommand.Parameters["EMailActionID"].Value = id;
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

      foreach (EmailAction emailAction in this)
      {
        if (emailAction.Row.Table.Columns.Contains("CreatorID") && (int)emailAction.Row["CreatorID"] == 0) emailAction.Row["CreatorID"] = LoginUser.UserID;
        if (emailAction.Row.Table.Columns.Contains("ModifierID")) emailAction.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public EmailAction FindByEMailActionID(int eMailActionID)
    {
      foreach (EmailAction emailAction in this)
      {
        if (emailAction.EMailActionID == eMailActionID)
        {
          return emailAction;
        }
      }
      return null;
    }

    public virtual EmailAction AddNewEmailAction()
    {
      if (Table.Columns.Count < 1) LoadColumns("EMailActionTable");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new EmailAction(row, this);
    }
    
    public virtual void LoadByEMailActionID(int eMailActionID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [EMailActionID], [DateTime], [EMailFrom], [EMailTo], [EMailSubject], [EMailBody], [OrganizationGUID], [ActionAdded], [Status], [TicketID], [OrganizationID] FROM [dbo].[EMailActionTable] WHERE ([EMailActionID] = @EMailActionID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("EMailActionID", eMailActionID);
        Fill(command);
      }
    }
    
    public static EmailAction GetEmailAction(LoginUser loginUser, int eMailActionID)
    {
      EmailActions emailActions = new EmailActions(loginUser);
      emailActions.LoadByEMailActionID(eMailActionID);
      if (emailActions.IsEmpty)
        return null;
      else
        return emailActions[0];
    }
    
    
    

    #endregion

    #region IEnumerable<EmailAction> Members

    public IEnumerator<EmailAction> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new EmailAction(row, this);
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
