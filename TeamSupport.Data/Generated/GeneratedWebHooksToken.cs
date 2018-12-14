using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WebHooksTokenItem : BaseItem
  {
    private WebHooksToken _webHooksToken;
    
    public WebHooksTokenItem(DataRow row, WebHooksToken webHooksToken): base(row, webHooksToken)
    {
      _webHooksToken = webHooksToken;
    }
	
    #region Properties
    
    public WebHooksToken Collection
    {
      get { return _webHooksToken; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    
    public int? ModifierId
    {
      get { return Row["ModifierId"] != DBNull.Value ? (int?)Row["ModifierId"] : null; }
      set { Row["ModifierId"] = CheckValue("ModifierId", value); }
    }
    

    
    public int CreatorId
    {
      get { return (int)Row["CreatorId"]; }
      set { Row["CreatorId"] = CheckValue("CreatorId", value); }
    }
    
    public bool IsEnabled
    {
      get { return (bool)Row["IsEnabled"]; }
      set { Row["IsEnabled"] = CheckValue("IsEnabled", value); }
    }
    
    public string Token
    {
      get { return (string)Row["Token"]; }
      set { Row["Token"] = CheckValue("Token", value); }
    }
    
    public int OrganizationId
    {
      get { return (int)Row["OrganizationId"]; }
      set { Row["OrganizationId"] = CheckValue("OrganizationId", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? DateModified
    {
      get { return Row["DateModified"] != DBNull.Value ? DateToLocal((DateTime?)Row["DateModified"]) : null; }
      set { Row["DateModified"] = CheckValue("DateModified", value); }
    }

    public DateTime? DateModifiedUtc
    {
      get { return Row["DateModified"] != DBNull.Value ? (DateTime?)Row["DateModified"] : null; }
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

  public partial class WebHooksToken : BaseCollection, IEnumerable<WebHooksTokenItem>
  {
    public WebHooksToken(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "WebHooksToken"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public WebHooksTokenItem this[int index]
    {
      get { return new WebHooksTokenItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WebHooksTokenItem webHooksTokenItem);
    partial void AfterRowInsert(WebHooksTokenItem webHooksTokenItem);
    partial void BeforeRowEdit(WebHooksTokenItem webHooksTokenItem);
    partial void AfterRowEdit(WebHooksTokenItem webHooksTokenItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public WebHooksTokenItemProxy[] GetWebHooksTokenItemProxies()
    {
      List<WebHooksTokenItemProxy> list = new List<WebHooksTokenItemProxy>();

      foreach (WebHooksTokenItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WebHooksToken] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("WebHooksTokenSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[WebHooksToken] SET     [OrganizationId] = @OrganizationId,    [Token] = @Token,    [IsEnabled] = @IsEnabled,    [CreatorId] = @CreatorId,    [DateModified] = @DateModified,    [ModifierId] = @ModifierId  WHERE ([Id] = @Id);";

		
		tempParameter = updateCommand.Parameters.Add("Id", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("OrganizationId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Token", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatorId", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ModifierId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[WebHooksToken] (    [OrganizationId],    [Token],    [IsEnabled],    [DateCreated],    [CreatorId],    [DateModified],    [ModifierId]) VALUES ( @OrganizationId, @Token, @IsEnabled, @DateCreated, @CreatorId, @DateModified, @ModifierId); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("ModifierId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateModified", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatorId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsEnabled", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Token", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("OrganizationId", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WebHooksToken] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (WebHooksTokenItem webHooksTokenItem in this)
		  {
			if (webHooksTokenItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(webHooksTokenItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = webHooksTokenItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				webHooksTokenItem.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(webHooksTokenItem);
			}
			else if (webHooksTokenItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(webHooksTokenItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = webHooksTokenItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(webHooksTokenItem);
			}
			else if (webHooksTokenItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)webHooksTokenItem.Row["Id", DataRowVersion.Original];
			  deleteCommand.Parameters["Id"].Value = id;
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

      foreach (WebHooksTokenItem webHooksTokenItem in this)
      {
        if (webHooksTokenItem.Row.Table.Columns.Contains("CreatorID") && (int)webHooksTokenItem.Row["CreatorID"] == 0) webHooksTokenItem.Row["CreatorID"] = LoginUser.UserID;
        if (webHooksTokenItem.Row.Table.Columns.Contains("ModifierID")) webHooksTokenItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WebHooksTokenItem FindById(int id)
    {
      foreach (WebHooksTokenItem webHooksTokenItem in this)
      {
        if (webHooksTokenItem.Id == id)
        {
          return webHooksTokenItem;
        }
      }
      return null;
    }

    public virtual WebHooksTokenItem AddNewWebHooksTokenItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("WebHooksToken");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WebHooksTokenItem(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [OrganizationId], [Token], [IsEnabled], [DateCreated], [CreatorId], [DateModified], [ModifierId] FROM [dbo].[WebHooksToken] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static WebHooksTokenItem GetWebHooksTokenItem(LoginUser loginUser, int id)
    {
      WebHooksToken webHooksToken = new WebHooksToken(loginUser);
      webHooksToken.LoadById(id);
      if (webHooksToken.IsEmpty)
        return null;
      else
        return webHooksToken[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WebHooksTokenItem> Members

    public IEnumerator<WebHooksTokenItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WebHooksTokenItem(row, this);
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
