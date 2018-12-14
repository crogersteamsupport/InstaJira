using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class EmailTemplateTable : BaseItem
  {
    private EmailTemplateTables _emailTemplateTables;
    
    public EmailTemplateTable(DataRow row, EmailTemplateTables emailTemplateTables): base(row, emailTemplateTables)
    {
      _emailTemplateTables = emailTemplateTables;
    }
	
    #region Properties
    
    public EmailTemplateTables Collection
    {
      get { return _emailTemplateTables; }
    }
        
    
    
    
    public int EmailTemplateTableID
    {
      get { return (int)Row["EmailTemplateTableID"]; }
    }
    

    

    
    public string Description
    {
      get { return (string)Row["Description"]; }
      set { Row["Description"] = CheckValue("Description", value); }
    }
    
    public string Alias
    {
      get { return (string)Row["Alias"]; }
      set { Row["Alias"] = CheckValue("Alias", value); }
    }
    
    public int ReportTableID
    {
      get { return (int)Row["ReportTableID"]; }
      set { Row["ReportTableID"] = CheckValue("ReportTableID", value); }
    }
    
    public int EmailTemplateID
    {
      get { return (int)Row["EmailTemplateID"]; }
      set { Row["EmailTemplateID"] = CheckValue("EmailTemplateID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class EmailTemplateTables : BaseCollection, IEnumerable<EmailTemplateTable>
  {
    public EmailTemplateTables(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "EmailTemplateTables"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "EmailTemplateTableID"; }
    }



    public EmailTemplateTable this[int index]
    {
      get { return new EmailTemplateTable(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(EmailTemplateTable emailTemplateTable);
    partial void AfterRowInsert(EmailTemplateTable emailTemplateTable);
    partial void BeforeRowEdit(EmailTemplateTable emailTemplateTable);
    partial void AfterRowEdit(EmailTemplateTable emailTemplateTable);
    partial void BeforeRowDelete(int emailTemplateTableID);
    partial void AfterRowDelete(int emailTemplateTableID);    

    partial void BeforeDBDelete(int emailTemplateTableID);
    partial void AfterDBDelete(int emailTemplateTableID);    

    #endregion

    #region Public Methods

    public EmailTemplateTableProxy[] GetEmailTemplateTableProxies()
    {
      List<EmailTemplateTableProxy> list = new List<EmailTemplateTableProxy>();

      foreach (EmailTemplateTable item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int emailTemplateTableID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EmailTemplateTables] WHERE ([EmailTemplateTableID] = @EmailTemplateTableID);";
        deleteCommand.Parameters.Add("EmailTemplateTableID", SqlDbType.Int);
        deleteCommand.Parameters["EmailTemplateTableID"].Value = emailTemplateTableID;

        BeforeDBDelete(emailTemplateTableID);
        BeforeRowDelete(emailTemplateTableID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(emailTemplateTableID);
        AfterDBDelete(emailTemplateTableID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("EmailTemplateTablesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[EmailTemplateTables] SET     [EmailTemplateID] = @EmailTemplateID,    [ReportTableID] = @ReportTableID,    [Alias] = @Alias,    [Description] = @Description  WHERE ([EmailTemplateTableID] = @EmailTemplateTableID);";

		
		tempParameter = updateCommand.Parameters.Add("EmailTemplateTableID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ReportTableID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Alias", SqlDbType.VarChar, 100);
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
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[EmailTemplateTables] (    [EmailTemplateID],    [ReportTableID],    [Alias],    [Description]) VALUES ( @EmailTemplateID, @ReportTableID, @Alias, @Description); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Description", SqlDbType.VarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Alias", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ReportTableID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[EmailTemplateTables] WHERE ([EmailTemplateTableID] = @EmailTemplateTableID);";
		deleteCommand.Parameters.Add("EmailTemplateTableID", SqlDbType.Int);

		try
		{
		  foreach (EmailTemplateTable emailTemplateTable in this)
		  {
			if (emailTemplateTable.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(emailTemplateTable);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = emailTemplateTable.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["EmailTemplateTableID"].AutoIncrement = false;
			  Table.Columns["EmailTemplateTableID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				emailTemplateTable.Row["EmailTemplateTableID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(emailTemplateTable);
			}
			else if (emailTemplateTable.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(emailTemplateTable);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = emailTemplateTable.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(emailTemplateTable);
			}
			else if (emailTemplateTable.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)emailTemplateTable.Row["EmailTemplateTableID", DataRowVersion.Original];
			  deleteCommand.Parameters["EmailTemplateTableID"].Value = id;
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

      foreach (EmailTemplateTable emailTemplateTable in this)
      {
        if (emailTemplateTable.Row.Table.Columns.Contains("CreatorID") && (int)emailTemplateTable.Row["CreatorID"] == 0) emailTemplateTable.Row["CreatorID"] = LoginUser.UserID;
        if (emailTemplateTable.Row.Table.Columns.Contains("ModifierID")) emailTemplateTable.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public EmailTemplateTable FindByEmailTemplateTableID(int emailTemplateTableID)
    {
      foreach (EmailTemplateTable emailTemplateTable in this)
      {
        if (emailTemplateTable.EmailTemplateTableID == emailTemplateTableID)
        {
          return emailTemplateTable;
        }
      }
      return null;
    }

    public virtual EmailTemplateTable AddNewEmailTemplateTable()
    {
      if (Table.Columns.Count < 1) LoadColumns("EmailTemplateTables");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new EmailTemplateTable(row, this);
    }
    
    public virtual void LoadByEmailTemplateTableID(int emailTemplateTableID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [EmailTemplateTableID], [EmailTemplateID], [ReportTableID], [Alias], [Description] FROM [dbo].[EmailTemplateTables] WHERE ([EmailTemplateTableID] = @EmailTemplateTableID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("EmailTemplateTableID", emailTemplateTableID);
        Fill(command);
      }
    }
    
    public static EmailTemplateTable GetEmailTemplateTable(LoginUser loginUser, int emailTemplateTableID)
    {
      EmailTemplateTables emailTemplateTables = new EmailTemplateTables(loginUser);
      emailTemplateTables.LoadByEmailTemplateTableID(emailTemplateTableID);
      if (emailTemplateTables.IsEmpty)
        return null;
      else
        return emailTemplateTables[0];
    }
    
    
    

    #endregion

    #region IEnumerable<EmailTemplateTable> Members

    public IEnumerator<EmailTemplateTable> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new EmailTemplateTable(row, this);
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
