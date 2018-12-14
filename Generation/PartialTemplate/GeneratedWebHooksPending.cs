using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WebHooksPendingItem : BaseItem
  {
    private WebHooksPending _webHooksPending;
    
    public WebHooksPendingItem(DataRow row, WebHooksPending webHooksPending): base(row, webHooksPending)
    {
      _webHooksPending = webHooksPending;
    }
	
    #region Properties
    
    public WebHooksPending Collection
    {
      get { return _webHooksPending; }
    }
        
    
    
    
    public int Id
    {
      get { return (int)Row["Id"]; }
    }
    

    
    public int? RefId
    {
      get { return Row["RefId"] != DBNull.Value ? (int?)Row["RefId"] : null; }
      set { Row["RefId"] = CheckValue("RefId", value); }
    }
    
    public string Url
    {
      get { return Row["Url"] != DBNull.Value ? (string)Row["Url"] : null; }
      set { Row["Url"] = CheckValue("Url", value); }
    }
    
    public string BodyData
    {
      get { return Row["BodyData"] != DBNull.Value ? (string)Row["BodyData"] : null; }
      set { Row["BodyData"] = CheckValue("BodyData", value); }
    }
    
    public string Token
    {
      get { return Row["Token"] != DBNull.Value ? (string)Row["Token"] : null; }
      set { Row["Token"] = CheckValue("Token", value); }
    }
    
    public bool? IsProcessing
    {
      get { return Row["IsProcessing"] != DBNull.Value ? (bool?)Row["IsProcessing"] : null; }
      set { Row["IsProcessing"] = CheckValue("IsProcessing", value); }
    }
    

    
    public bool Inbound
    {
      get { return (bool)Row["Inbound"]; }
      set { Row["Inbound"] = CheckValue("Inbound", value); }
    }
    
    public short Type
    {
      get { return (short)Row["Type"]; }
      set { Row["Type"] = CheckValue("Type", value); }
    }
    
    public int RefType
    {
      get { return (int)Row["RefType"]; }
      set { Row["RefType"] = CheckValue("RefType", value); }
    }
    
    public int OrganizationId
    {
      get { return (int)Row["OrganizationId"]; }
      set { Row["OrganizationId"] = CheckValue("OrganizationId", value); }
    }
    

    /* DateTime */
    
    
    

    

    
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

  public partial class WebHooksPending : BaseCollection, IEnumerable<WebHooksPendingItem>
  {
    public WebHooksPending(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "WebHooksPending"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "Id"; }
    }



    public WebHooksPendingItem this[int index]
    {
      get { return new WebHooksPendingItem(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WebHooksPendingItem webHooksPendingItem);
    partial void AfterRowInsert(WebHooksPendingItem webHooksPendingItem);
    partial void BeforeRowEdit(WebHooksPendingItem webHooksPendingItem);
    partial void AfterRowEdit(WebHooksPendingItem webHooksPendingItem);
    partial void BeforeRowDelete(int id);
    partial void AfterRowDelete(int id);    

    partial void BeforeDBDelete(int id);
    partial void AfterDBDelete(int id);    

    #endregion

    #region Public Methods

    public WebHooksPendingItemProxy[] GetWebHooksPendingItemProxies()
    {
      List<WebHooksPendingItemProxy> list = new List<WebHooksPendingItemProxy>();

      foreach (WebHooksPendingItem item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int id)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WebHooksPending] WHERE ([Id] = @Id);";
        deleteCommand.Parameters.Add("Id", SqlDbType.Int);
        deleteCommand.Parameters["Id"].Value = id;

        BeforeDBDelete(id);
        BeforeRowDelete(id);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(id);
        AfterDBDelete(id);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("WebHooksPendingSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[WebHooksPending] SET     [OrganizationId] = @OrganizationId,    [RefType] = @RefType,    [RefId] = @RefId,    [Type] = @Type,    [Url] = @Url,    [BodyData] = @BodyData,    [Token] = @Token,    [Inbound] = @Inbound,    [IsProcessing] = @IsProcessing  WHERE ([Id] = @Id);";

		
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
		
		tempParameter = updateCommand.Parameters.Add("RefType", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("RefId", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("Type", SqlDbType.SmallInt, 2);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 5;
		  tempParameter.Scale = 5;
		}
		
		tempParameter = updateCommand.Parameters.Add("Url", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("BodyData", SqlDbType.VarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Token", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Inbound", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsProcessing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[WebHooksPending] (    [OrganizationId],    [RefType],    [RefId],    [Type],    [Url],    [BodyData],    [Token],    [Inbound],    [IsProcessing],    [DateCreated]) VALUES ( @OrganizationId, @RefType, @RefId, @Type, @Url, @BodyData, @Token, @Inbound, @IsProcessing, @DateCreated); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("DateCreated", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("IsProcessing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Inbound", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("BodyData", SqlDbType.VarChar, 4000);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Url", SqlDbType.VarChar, 150);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Type", SqlDbType.SmallInt, 2);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 5;
		  tempParameter.Scale = 5;
		}
		
		tempParameter = insertCommand.Parameters.Add("RefId", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WebHooksPending] WHERE ([Id] = @Id);";
		deleteCommand.Parameters.Add("Id", SqlDbType.Int);

		try
		{
		  foreach (WebHooksPendingItem webHooksPendingItem in this)
		  {
			if (webHooksPendingItem.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(webHooksPendingItem);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = webHooksPendingItem.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["Id"].AutoIncrement = false;
			  Table.Columns["Id"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				webHooksPendingItem.Row["Id"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(webHooksPendingItem);
			}
			else if (webHooksPendingItem.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(webHooksPendingItem);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = webHooksPendingItem.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(webHooksPendingItem);
			}
			else if (webHooksPendingItem.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)webHooksPendingItem.Row["Id", DataRowVersion.Original];
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

      foreach (WebHooksPendingItem webHooksPendingItem in this)
      {
        if (webHooksPendingItem.Row.Table.Columns.Contains("CreatorID") && (int)webHooksPendingItem.Row["CreatorID"] == 0) webHooksPendingItem.Row["CreatorID"] = LoginUser.UserID;
        if (webHooksPendingItem.Row.Table.Columns.Contains("ModifierID")) webHooksPendingItem.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WebHooksPendingItem FindById(int id)
    {
      foreach (WebHooksPendingItem webHooksPendingItem in this)
      {
        if (webHooksPendingItem.Id == id)
        {
          return webHooksPendingItem;
        }
      }
      return null;
    }

    public virtual WebHooksPendingItem AddNewWebHooksPendingItem()
    {
      if (Table.Columns.Count < 1) LoadColumns("WebHooksPending");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WebHooksPendingItem(row, this);
    }
    
    public virtual void LoadById(int id)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [Id], [OrganizationId], [RefType], [RefId], [Type], [Url], [BodyData], [Token], [Inbound], [IsProcessing], [DateCreated] FROM [dbo].[WebHooksPending] WHERE ([Id] = @Id);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("Id", id);
        Fill(command);
      }
    }
    
    public static WebHooksPendingItem GetWebHooksPendingItem(LoginUser loginUser, int id)
    {
      WebHooksPending webHooksPending = new WebHooksPending(loginUser);
      webHooksPending.LoadById(id);
      if (webHooksPending.IsEmpty)
        return null;
      else
        return webHooksPending[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WebHooksPendingItem> Members

    public IEnumerator<WebHooksPendingItem> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WebHooksPendingItem(row, this);
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
