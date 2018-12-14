using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class OrganizationEmail : BaseItem
  {
    private OrganizationEmails _organizationEmails;
    
    public OrganizationEmail(DataRow row, OrganizationEmails organizationEmails): base(row, organizationEmails)
    {
      _organizationEmails = organizationEmails;
    }
	
    #region Properties
    
    public OrganizationEmails Collection
    {
      get { return _organizationEmails; }
    }
        
    
    
    
    public int OrganizationEmailID
    {
      get { return (int)Row["OrganizationEmailID"]; }
    }
    

    
    public int? ProductFamilyID
    {
      get { return Row["ProductFamilyID"] != DBNull.Value ? (int?)Row["ProductFamilyID"] : null; }
      set { Row["ProductFamilyID"] = CheckValue("ProductFamilyID", value); }
    }
    

    
    public bool UseGlobalTemplate
    {
      get { return (bool)Row["UseGlobalTemplate"]; }
      set { Row["UseGlobalTemplate"] = CheckValue("UseGlobalTemplate", value); }
    }
    
    public bool IsHtml
    {
      get { return (bool)Row["IsHtml"]; }
      set { Row["IsHtml"] = CheckValue("IsHtml", value); }
    }
    
    public string Body
    {
      get { return (string)Row["Body"]; }
      set { Row["Body"] = CheckValue("Body", value); }
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
    
    public int EmailTemplateID
    {
      get { return (int)Row["EmailTemplateID"]; }
      set { Row["EmailTemplateID"] = CheckValue("EmailTemplateID", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    

    /* DateTime */
    
    
    

    

    

    #endregion
    
    
  }

  public partial class OrganizationEmails : BaseCollection, IEnumerable<OrganizationEmail>
  {
    public OrganizationEmails(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "OrganizationEmails"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "OrganizationEmailID"; }
    }



    public OrganizationEmail this[int index]
    {
      get { return new OrganizationEmail(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(OrganizationEmail organizationEmail);
    partial void AfterRowInsert(OrganizationEmail organizationEmail);
    partial void BeforeRowEdit(OrganizationEmail organizationEmail);
    partial void AfterRowEdit(OrganizationEmail organizationEmail);
    partial void BeforeRowDelete(int organizationEmailID);
    partial void AfterRowDelete(int organizationEmailID);    

    partial void BeforeDBDelete(int organizationEmailID);
    partial void AfterDBDelete(int organizationEmailID);    

    #endregion

    #region Public Methods

    public OrganizationEmailProxy[] GetOrganizationEmailProxies()
    {
      List<OrganizationEmailProxy> list = new List<OrganizationEmailProxy>();

      foreach (OrganizationEmail item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int organizationEmailID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationEmails] WHERE ([OrganizationEmailID] = @OrganizationEmailID);";
        deleteCommand.Parameters.Add("OrganizationEmailID", SqlDbType.Int);
        deleteCommand.Parameters["OrganizationEmailID"].Value = organizationEmailID;

        BeforeDBDelete(organizationEmailID);
        BeforeRowDelete(organizationEmailID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(organizationEmailID);
        AfterDBDelete(organizationEmailID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("OrganizationEmailsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[OrganizationEmails] SET     [OrganizationID] = @OrganizationID,    [EmailTemplateID] = @EmailTemplateID,    [Subject] = @Subject,    [Header] = @Header,    [Footer] = @Footer,    [Body] = @Body,    [IsHtml] = @IsHtml,    [UseGlobalTemplate] = @UseGlobalTemplate,    [ProductFamilyID] = @ProductFamilyID  WHERE ([OrganizationEmailID] = @OrganizationEmailID);";

		
		tempParameter = updateCommand.Parameters.Add("OrganizationEmailID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("EmailTemplateID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Subject", SqlDbType.NVarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Header", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Footer", SqlDbType.NVarChar, 1000);
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
		
		tempParameter = updateCommand.Parameters.Add("IsHtml", SqlDbType.Bit, 1);
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
		
		tempParameter = updateCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[OrganizationEmails] (    [OrganizationID],    [EmailTemplateID],    [Subject],    [Header],    [Footer],    [Body],    [IsHtml],    [UseGlobalTemplate],    [ProductFamilyID]) VALUES ( @OrganizationID, @EmailTemplateID, @Subject, @Header, @Footer, @Body, @IsHtml, @UseGlobalTemplate, @ProductFamilyID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ProductFamilyID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("UseGlobalTemplate", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("Body", SqlDbType.NVarChar, -1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Footer", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Header", SqlDbType.NVarChar, 1000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Subject", SqlDbType.NVarChar, 4000);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[OrganizationEmails] WHERE ([OrganizationEmailID] = @OrganizationEmailID);";
		deleteCommand.Parameters.Add("OrganizationEmailID", SqlDbType.Int);

		try
		{
		  foreach (OrganizationEmail organizationEmail in this)
		  {
			if (organizationEmail.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(organizationEmail);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = organizationEmail.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["OrganizationEmailID"].AutoIncrement = false;
			  Table.Columns["OrganizationEmailID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				organizationEmail.Row["OrganizationEmailID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(organizationEmail);
			}
			else if (organizationEmail.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(organizationEmail);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = organizationEmail.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(organizationEmail);
			}
			else if (organizationEmail.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)organizationEmail.Row["OrganizationEmailID", DataRowVersion.Original];
			  deleteCommand.Parameters["OrganizationEmailID"].Value = id;
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

      foreach (OrganizationEmail organizationEmail in this)
      {
        if (organizationEmail.Row.Table.Columns.Contains("CreatorID") && (int)organizationEmail.Row["CreatorID"] == 0) organizationEmail.Row["CreatorID"] = LoginUser.UserID;
        if (organizationEmail.Row.Table.Columns.Contains("ModifierID")) organizationEmail.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public OrganizationEmail FindByOrganizationEmailID(int organizationEmailID)
    {
      foreach (OrganizationEmail organizationEmail in this)
      {
        if (organizationEmail.OrganizationEmailID == organizationEmailID)
        {
          return organizationEmail;
        }
      }
      return null;
    }

    public virtual OrganizationEmail AddNewOrganizationEmail()
    {
      if (Table.Columns.Count < 1) LoadColumns("OrganizationEmails");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new OrganizationEmail(row, this);
    }
    
    public virtual void LoadByOrganizationEmailID(int organizationEmailID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [OrganizationEmailID], [OrganizationID], [EmailTemplateID], [Subject], [Header], [Footer], [Body], [IsHtml], [UseGlobalTemplate], [ProductFamilyID] FROM [dbo].[OrganizationEmails] WHERE ([OrganizationEmailID] = @OrganizationEmailID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("OrganizationEmailID", organizationEmailID);
        Fill(command);
      }
    }
    
    public static OrganizationEmail GetOrganizationEmail(LoginUser loginUser, int organizationEmailID)
    {
      OrganizationEmails organizationEmails = new OrganizationEmails(loginUser);
      organizationEmails.LoadByOrganizationEmailID(organizationEmailID);
      if (organizationEmails.IsEmpty)
        return null;
      else
        return organizationEmails[0];
    }
    
    
    

    #endregion

    #region IEnumerable<OrganizationEmail> Members

    public IEnumerator<OrganizationEmail> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new OrganizationEmail(row, this);
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
