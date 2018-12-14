using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class EmailTemplate : BaseItem
  {
    private EmailTemplates _emailTemplates;
    
    public EmailTemplate(DataRow row, EmailTemplates emailTemplates): base(row, emailTemplates)
    {
      _emailTemplates = emailTemplates;
    }
	
    #region Properties
    
    public EmailTemplates Collection
    {
      get { return _emailTemplates; }
    }
        
    
    
    

    

    
    public string Body
    {
      get { return (string)Row["Body"]; }
      set { Row["Body"] = CheckValue("Body", value); }
    }
    
    public bool UseGlobalTemplate
    {
      get { return (bool)Row["UseGlobalTemplate"]; }
      set { Row["UseGlobalTemplate"] = CheckValue("UseGlobalTemplate", value); }
    }
    
    public bool IsEmail
    {
      get { return (bool)Row["IsEmail"]; }
      set { Row["IsEmail"] = CheckValue("IsEmail", value); }
    }
    
    public bool IncludeDelimiter
    {
      get { return (bool)Row["IncludeDelimiter"]; }
      set { Row["IncludeDelimiter"] = CheckValue("IncludeDelimiter", value); }
    }
    
    public bool IsHtml
    {
      get { return (bool)Row["IsHtml"]; }
      set { Row["IsHtml"] = CheckValue("IsHtml", value); }
    }
    
    public string Footer
    {
      get { return (string)Row["Footer"]; }
      set { Row["Footer"] = CheckValue("Footer", value); }
    }
    
    public string Header
    {
      get { return (string)Row["Header"]; }
      set { Row["Header"] = CheckValue("Header", value); }
    }
    
    public string Subject
    {
      get { return (string)Row["Subject"]; }
      set { Row["Subject"] = CheckValue("Subject", value); }
    }
    
    public bool IsTSOnly
    {
      get { return (bool)Row["IsTSOnly"]; }
      set { Row["IsTSOnly"] = CheckValue("IsTSOnly", value); }
    }
    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string Name
    {
      get { return (string)Row["Name"]; }
      set { Row["Name"] = CheckValue("Name", value); }
    }
    
    public int Position
    {
      get { return (int)Row["Position"]; }
      set { Row["Position"] = CheckValue("Position", value); }
    }
    
    public int EmailTemplateID
    {
      get { return (int)Row["EmailTemplateID"]; }
      set { Row["EmailTemplateID"] = CheckValue("EmailTemplateID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class EmailTemplates : BaseCollection, IEnumerable<EmailTemplate>
  {
    public EmailTemplates(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "EmailTemplates"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "EmailTemplateID"; }
    }



    public EmailTemplate this[int index]
    {
      get { return new EmailTemplate(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(EmailTemplate emailTemplate);
    partial void AfterRowInsert(EmailTemplate emailTemplate);
    partial void BeforeRowEdit(EmailTemplate emailTemplate);
    partial void AfterRowEdit(EmailTemplate emailTemplate);
    partial void BeforeRowDelete(int emailTemplateID);
    partial void AfterRowDelete(int emailTemplateID);    

    partial void BeforeDBDelete(int emailTemplateID);
    partial void AfterDBDelete(int emailTemplateID);    

    #endregion

    #region Public Methods

    public EmailTemplateProxy[] GetEmailTemplateProxies()
    {
      List<EmailTemplateProxy> list = new List<EmailTemplateProxy>();

      foreach (EmailTemplate item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int emailTemplateID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EmailTemplates] WHERE ([EmailTemplateID] = @EmailTemplateID);";
        deleteCommand.Parameters.Add("EmailTemplateID", SqlDbType.Int);
        deleteCommand.Parameters["EmailTemplateID"].Value = emailTemplateID;

        BeforeDBDelete(emailTemplateID);
        BeforeRowDelete(emailTemplateID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(emailTemplateID);
        AfterDBDelete(emailTemplateID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("EmailTemplatesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[EmailTemplates] SET     [Position] = @Position,    [Name] = @Name,    [Description] = @Description,    [IsTSOnly] = @IsTSOnly,    [Subject] = @Subject,    [Header] = @Header,    [Footer] = @Footer,    [IsHtml] = @IsHtml,    [IncludeDelimiter] = @IncludeDelimiter,    [IsEmail] = @IsEmail,    [UseGlobalTemplate] = @UseGlobalTemplate,    [Body] = @Body  WHERE ([EmailTemplateID] = @EmailTemplateID);";

		
		tempParameter = updateCommand.Parameters.Add("EmailTemplateID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Name", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsTSOnly", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Subject", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Header", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Footer", SqlDbType.VarChar, 1000);
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
		
		tempParameter = updateCommand.Parameters.Add("IncludeDelimiter", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UseGlobalTemplate", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Body", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[EmailTemplates] (    [EmailTemplateID],    [Position],    [Name],    [Description],    [IsTSOnly],    [Subject],    [Header],    [Footer],    [IsHtml],    [IncludeDelimiter],    [IsEmail],    [UseGlobalTemplate],    [Body]) VALUES ( @EmailTemplateID, @Position, @Name, @Description, @IsTSOnly, @Subject, @Header, @Footer, @IsHtml, @IncludeDelimiter, @IsEmail, @UseGlobalTemplate, @Body); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Body", SqlDbType.VarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseGlobalTemplate", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsEmail", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IncludeDelimiter", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsHtml", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Footer", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Header", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Subject", SqlDbType.VarChar, 8000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsTSOnly", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Name", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Position", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("EmailTemplateID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EmailTemplates] WHERE ([EmailTemplateID] = @EmailTemplateID);";
		deleteCommand.Parameters.Add("EmailTemplateID", SqlDbType.Int);

		try
		{
		  foreach (EmailTemplate emailTemplate in this)
		  {
			if (emailTemplate.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(emailTemplate);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = emailTemplate.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["EmailTemplateID"].AutoIncrement = false;
			  Table.Columns["EmailTemplateID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				emailTemplate.Row["EmailTemplateID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(emailTemplate);
			}
			else if (emailTemplate.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(emailTemplate);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = emailTemplate.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(emailTemplate);
			}
			else if (emailTemplate.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)emailTemplate.Row["EmailTemplateID", DataRowVersion.Original];
			  deleteCommand.Parameters["EmailTemplateID"].Value = id;
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

      foreach (EmailTemplate emailTemplate in this)
      {
        if (emailTemplate.Row.Table.Columns.Contains("CreatorID") && (int)emailTemplate.Row["CreatorID"] == 0) emailTemplate.Row["CreatorID"] = LoginUser.UserID;
        if (emailTemplate.Row.Table.Columns.Contains("ModifierID")) emailTemplate.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public EmailTemplate FindByEmailTemplateID(int emailTemplateID)
    {
      foreach (EmailTemplate emailTemplate in this)
      {
        if (emailTemplate.EmailTemplateID == emailTemplateID)
        {
          return emailTemplate;
        }
      }
      return null;
    }

    public virtual EmailTemplate AddNewEmailTemplate()
    {
      if (Table.Columns.Count < 1) LoadColumns("EmailTemplates");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new EmailTemplate(row, this);
    }
    
    public virtual void LoadByEmailTemplateID(int emailTemplateID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [EmailTemplateID], [Position], [Name], [Description], [IsTSOnly], [Subject], [Header], [Footer], [IsHtml], [IncludeDelimiter], [IsEmail], [UseGlobalTemplate], [Body] FROM [dbo].[EmailTemplates] WHERE ([EmailTemplateID] = @EmailTemplateID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("EmailTemplateID", emailTemplateID);
        Fill(command);
      }
    }
    
    public static EmailTemplate GetEmailTemplate(LoginUser loginUser, int emailTemplateID)
    {
      EmailTemplates emailTemplates = new EmailTemplates(loginUser);
      emailTemplates.LoadByEmailTemplateID(emailTemplateID);
      if (emailTemplates.IsEmpty)
        return null;
      else
        return emailTemplates[0];
    }
    
    
    

    #endregion

    #region IEnumerable<EmailTemplate> Members

    public IEnumerator<EmailTemplate> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new EmailTemplate(row, this);
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
