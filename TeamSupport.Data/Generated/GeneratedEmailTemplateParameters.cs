using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class EmailTemplateParameter : BaseItem
  {
    private EmailTemplateParameters _emailTemplateParameters;
    
    public EmailTemplateParameter(DataRow row, EmailTemplateParameters emailTemplateParameters): base(row, emailTemplateParameters)
    {
      _emailTemplateParameters = emailTemplateParameters;
    }
	
    #region Properties
    
    public EmailTemplateParameters Collection
    {
      get { return _emailTemplateParameters; }
    }
        
    
    
    
    public int EmailTemplateParameterID
    {
      get { return (int)Row["EmailTemplateParameterID"]; }
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
    
    public int EmailTemplateID
    {
      get { return (int)Row["EmailTemplateID"]; }
      set { Row["EmailTemplateID"] = CheckValue("EmailTemplateID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class EmailTemplateParameters : BaseCollection, IEnumerable<EmailTemplateParameter>
  {
    public EmailTemplateParameters(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "EmailTemplateParameters"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "EmailTemplateParameterID"; }
    }



    public EmailTemplateParameter this[int index]
    {
      get { return new EmailTemplateParameter(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(EmailTemplateParameter emailTemplateParameter);
    partial void AfterRowInsert(EmailTemplateParameter emailTemplateParameter);
    partial void BeforeRowEdit(EmailTemplateParameter emailTemplateParameter);
    partial void AfterRowEdit(EmailTemplateParameter emailTemplateParameter);
    partial void BeforeRowDelete(int emailTemplateParameterID);
    partial void AfterRowDelete(int emailTemplateParameterID);    

    partial void BeforeDBDelete(int emailTemplateParameterID);
    partial void AfterDBDelete(int emailTemplateParameterID);    

    #endregion

    #region Public Methods

    public EmailTemplateParameterProxy[] GetEmailTemplateParameterProxies()
    {
      List<EmailTemplateParameterProxy> list = new List<EmailTemplateParameterProxy>();

      foreach (EmailTemplateParameter item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int emailTemplateParameterID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EmailTemplateParameters] WHERE ([EmailTemplateParameterID] = @EmailTemplateParameterID);";
        deleteCommand.Parameters.Add("EmailTemplateParameterID", SqlDbType.Int);
        deleteCommand.Parameters["EmailTemplateParameterID"].Value = emailTemplateParameterID;

        BeforeDBDelete(emailTemplateParameterID);
        BeforeRowDelete(emailTemplateParameterID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(emailTemplateParameterID);
        AfterDBDelete(emailTemplateParameterID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("EmailTemplateParametersSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[EmailTemplateParameters] SET     [EmailTemplateID] = @EmailTemplateID,    [Name] = @Name,    [Description] = @Description  WHERE ([EmailTemplateParameterID] = @EmailTemplateParameterID);";

		
		tempParameter = updateCommand.Parameters.Add("EmailTemplateParameterID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("EmailTemplateID", SqlDbType.Int, 4);
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
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[EmailTemplateParameters] (    [EmailTemplateID],    [Name],    [Description]) VALUES ( @EmailTemplateID, @Name, @Description); SET @Identity = SCOPE_IDENTITY();";

		
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EmailTemplateParameters] WHERE ([EmailTemplateParameterID] = @EmailTemplateParameterID);";
		deleteCommand.Parameters.Add("EmailTemplateParameterID", SqlDbType.Int);

		try
		{
		  foreach (EmailTemplateParameter emailTemplateParameter in this)
		  {
			if (emailTemplateParameter.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(emailTemplateParameter);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = emailTemplateParameter.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["EmailTemplateParameterID"].AutoIncrement = false;
			  Table.Columns["EmailTemplateParameterID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				emailTemplateParameter.Row["EmailTemplateParameterID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(emailTemplateParameter);
			}
			else if (emailTemplateParameter.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(emailTemplateParameter);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = emailTemplateParameter.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(emailTemplateParameter);
			}
			else if (emailTemplateParameter.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)emailTemplateParameter.Row["EmailTemplateParameterID", DataRowVersion.Original];
			  deleteCommand.Parameters["EmailTemplateParameterID"].Value = id;
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

      foreach (EmailTemplateParameter emailTemplateParameter in this)
      {
        if (emailTemplateParameter.Row.Table.Columns.Contains("CreatorID") && (int)emailTemplateParameter.Row["CreatorID"] == 0) emailTemplateParameter.Row["CreatorID"] = LoginUser.UserID;
        if (emailTemplateParameter.Row.Table.Columns.Contains("ModifierID")) emailTemplateParameter.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public EmailTemplateParameter FindByEmailTemplateParameterID(int emailTemplateParameterID)
    {
      foreach (EmailTemplateParameter emailTemplateParameter in this)
      {
        if (emailTemplateParameter.EmailTemplateParameterID == emailTemplateParameterID)
        {
          return emailTemplateParameter;
        }
      }
      return null;
    }

    public virtual EmailTemplateParameter AddNewEmailTemplateParameter()
    {
      if (Table.Columns.Count < 1) LoadColumns("EmailTemplateParameters");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new EmailTemplateParameter(row, this);
    }
    
    public virtual void LoadByEmailTemplateParameterID(int emailTemplateParameterID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [EmailTemplateParameterID], [EmailTemplateID], [Name], [Description] FROM [dbo].[EmailTemplateParameters] WHERE ([EmailTemplateParameterID] = @EmailTemplateParameterID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("EmailTemplateParameterID", emailTemplateParameterID);
        Fill(command);
      }
    }
    
    public static EmailTemplateParameter GetEmailTemplateParameter(LoginUser loginUser, int emailTemplateParameterID)
    {
      EmailTemplateParameters emailTemplateParameters = new EmailTemplateParameters(loginUser);
      emailTemplateParameters.LoadByEmailTemplateParameterID(emailTemplateParameterID);
      if (emailTemplateParameters.IsEmpty)
        return null;
      else
        return emailTemplateParameters[0];
    }
    
    
    

    #endregion

    #region IEnumerable<EmailTemplateParameter> Members

    public IEnumerator<EmailTemplateParameter> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new EmailTemplateParameter(row, this);
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
