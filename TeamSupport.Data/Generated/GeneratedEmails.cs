using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class Email : BaseItem
  {
    private Emails _emails;
    
    public Email(DataRow row, Emails emails): base(row, emails)
    {
      _emails = emails;
    }
	
    #region Properties
    
    public Emails Collection
    {
      get { return _emails; }
    }
        
    
    
    
    public int EmailID
    {
      get { return (int)Row["EmailID"]; }
    }
    

    
    public string CCAddress
    {
      get { return Row["CCAddress"] != DBNull.Value ? (string)Row["CCAddress"] : null; }
      set { Row["CCAddress"] = CheckValue("CCAddress", value); }
    }
    
    public string BCCAddress
    {
      get { return Row["BCCAddress"] != DBNull.Value ? (string)Row["BCCAddress"] : null; }
      set { Row["BCCAddress"] = CheckValue("BCCAddress", value); }
    }
    
    public string Subject
    {
      get { return Row["Subject"] != DBNull.Value ? (string)Row["Subject"] : null; }
      set { Row["Subject"] = CheckValue("Subject", value); }
    }
    
    public string Body
    {
      get { return Row["Body"] != DBNull.Value ? (string)Row["Body"] : null; }
      set { Row["Body"] = CheckValue("Body", value); }
    }
    
    public string Attachments
    {
      get { return Row["Attachments"] != DBNull.Value ? (string)Row["Attachments"] : null; }
      set { Row["Attachments"] = CheckValue("Attachments", value); }
    }
    
    public string LastFailedReason
    {
      get { return Row["LastFailedReason"] != DBNull.Value ? (string)Row["LastFailedReason"] : null; }
      set { Row["LastFailedReason"] = CheckValue("LastFailedReason", value); }
    }
    
    public int? EmailPostID
    {
      get { return Row["EmailPostID"] != DBNull.Value ? (int?)Row["EmailPostID"] : null; }
      set { Row["EmailPostID"] = CheckValue("EmailPostID", value); }
    }
    
    public string LockProcessID
    {
      get { return Row["LockProcessID"] != DBNull.Value ? (string)Row["LockProcessID"] : null; }
      set { Row["LockProcessID"] = CheckValue("LockProcessID", value); }
    }
    

    
    public int Attempts
    {
      get { return (int)Row["Attempts"]; }
      set { Row["Attempts"] = CheckValue("Attempts", value); }
    }
    
    public bool IsHtml
    {
      get { return (bool)Row["IsHtml"]; }
      set { Row["IsHtml"] = CheckValue("IsHtml", value); }
    }
    
    public bool IsWaiting
    {
      get { return (bool)Row["IsWaiting"]; }
      set { Row["IsWaiting"] = CheckValue("IsWaiting", value); }
    }
    
    public bool IsSuccess
    {
      get { return (bool)Row["IsSuccess"]; }
      set { Row["IsSuccess"] = CheckValue("IsSuccess", value); }
    }
    
    public int Size
    {
      get { return (int)Row["Size"]; }
      set { Row["Size"] = CheckValue("Size", value); }
    }
    
    public string ToAddress
    {
      get { return (string)Row["ToAddress"]; }
      set { Row["ToAddress"] = CheckValue("ToAddress", value); }
    }
    
    public string FromAddress
    {
      get { return (string)Row["FromAddress"]; }
      set { Row["FromAddress"] = CheckValue("FromAddress", value); }
    }
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateSent
    {
      get { return Row["DateSent"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateSent"]) : null; }
      set { Row["DateSent"] = CheckValue("DateSent", value); }
    }

    public DateTime? DateSentUtc
    {
      get { return Row["DateSent"] != DBNull.Value ? (DateTime?)Row["DateSent"] : null; }
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
    
    public DateTime NextAttempt
    {
      get { return DateToLocal((DateTime)Row["NextAttempt"]); }
      set { Row["NextAttempt"] = CheckValue("NextAttempt", value); }
    }

    public DateTime NextAttemptUtc
    {
      get { return (DateTime)Row["NextAttempt"]; }
    }
    

    #endregion
    
    
  }

  public partial class Emails : BaseCollection, IEnumerable<Email>
  {
    public Emails(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "Emails"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "EmailID"; }
    }



    public Email this[int index]
    {
      get { return new Email(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(Email email);
    partial void AfterRowInsert(Email email);
    partial void BeforeRowEdit(Email email);
    partial void AfterRowEdit(Email email);
    partial void BeforeRowDelete(int emailID);
    partial void AfterRowDelete(int emailID);    

    partial void BeforeDBDelete(int emailID);
    partial void AfterDBDelete(int emailID);    

    #endregion

    #region Public Methods

    public EmailProxy[] GetEmailProxies()
    {
      List<EmailProxy> list = new List<EmailProxy>();

      foreach (Email item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int emailID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Emails] WHERE ([EmailID] = @EmailID);";
        deleteCommand.Parameters.Add("EmailID", SqlDbType.Int);
        deleteCommand.Parameters["EmailID"].Value = emailID;

        BeforeDBDelete(emailID);
        BeforeRowDelete(emailID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(emailID);
        AfterDBDelete(emailID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("EmailsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[Emails] SET     [OrganizationID] = @OrganizationID,    [Description] = @Description,    [FromAddress] = @FromAddress,    [ToAddress] = @ToAddress,    [CCAddress] = @CCAddress,    [BCCAddress] = @BCCAddress,    [Subject] = @Subject,    [Body] = @Body,    [Attachments] = @Attachments,    [Size] = @Size,    [IsSuccess] = @IsSuccess,    [IsWaiting] = @IsWaiting,    [IsHtml] = @IsHtml,    [Attempts] = @Attempts,    [NextAttempt] = @NextAttempt,    [DateSent] = @DateSent,    [LastFailedReason] = @LastFailedReason,    [EmailPostID] = @EmailPostID,    [LockProcessID] = @LockProcessID  WHERE ([EmailID] = @EmailID);";

		
		tempParameter = updateCommand.Parameters.Add("EmailID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("FromAddress", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("ToAddress", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CCAddress", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("BCCAddress", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Subject", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Body", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Attachments", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Size", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsSuccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsWaiting", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsHtml", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Attempts", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("NextAttempt", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("DateSent", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("LastFailedReason", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailPostID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("LockProcessID", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[Emails] (    [OrganizationID],    [Description],    [FromAddress],    [ToAddress],    [CCAddress],    [BCCAddress],    [Subject],    [Body],    [Attachments],    [Size],    [IsSuccess],    [IsWaiting],    [IsHtml],    [Attempts],    [NextAttempt],    [DateSent],    [LastFailedReason],    [EmailPostID],    [DateCreated],    [LockProcessID]) VALUES ( @OrganizationID, @Description, @FromAddress, @ToAddress, @CCAddress, @BCCAddress, @Subject, @Body, @Attachments, @Size, @IsSuccess, @IsWaiting, @IsHtml, @Attempts, @NextAttempt, @DateSent, @LastFailedReason, @EmailPostID, @DateCreated, @LockProcessID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("LockProcessID", SqlDbType.VarChar, 250);
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
		
		tempParameter = insertCommand.Parameters.Add("EmailPostID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("LastFailedReason", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateSent", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("NextAttempt", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("Attempts", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsHtml", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsWaiting", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsSuccess", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Size", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Attachments", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Body", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Subject", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("BCCAddress", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("CCAddress", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ToAddress", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("FromAddress", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 250);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[Emails] WHERE ([EmailID] = @EmailID);";
		deleteCommand.Parameters.Add("EmailID", SqlDbType.Int);

		try
		{
		  foreach (Email email in this)
		  {
			if (email.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(email);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = email.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["EmailID"].AutoIncrement = false;
			  Table.Columns["EmailID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				email.Row["EmailID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(email);
			}
			else if (email.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(email);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = email.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(email);
			}
			else if (email.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)email.Row["EmailID", DataRowVersion.Original];
			  deleteCommand.Parameters["EmailID"].Value = id;
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

      foreach (Email email in this)
      {
        if (email.Row.Table.Columns.Contains("CreatorID") && (int)email.Row["CreatorID"] == 0) email.Row["CreatorID"] = LoginUser.UserID;
        if (email.Row.Table.Columns.Contains("ModifierID")) email.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public Email FindByEmailID(int emailID)
    {
      foreach (Email email in this)
      {
        if (email.EmailID == emailID)
        {
          return email;
        }
      }
      return null;
    }

    public virtual Email AddNewEmail()
    {
      if (Table.Columns.Count < 1) LoadColumns("Emails");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new Email(row, this);
    }
    
    public virtual void LoadByEmailID(int emailID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [EmailID], [OrganizationID], [Description], [FromAddress], [ToAddress], [CCAddress], [BCCAddress], [Subject], [Body], [Attachments], [Size], [IsSuccess], [IsWaiting], [IsHtml], [Attempts], [NextAttempt], [DateSent], [LastFailedReason], [EmailPostID], [DateCreated], [LockProcessID] FROM [dbo].[Emails] WHERE ([EmailID] = @EmailID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("EmailID", emailID);
        Fill(command);
      }
    }
    
    public static Email GetEmail(LoginUser loginUser, int emailID)
    {
      Emails emails = new Emails(loginUser);
      emails.LoadByEmailID(emailID);
      if (emails.IsEmpty)
        return null;
      else
        return emails[0];
    }
    
    
    

    #endregion

    #region IEnumerable<Email> Members

    public IEnumerator<Email> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new Email(row, this);
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
