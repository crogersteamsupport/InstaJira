using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WikiHistory : BaseItem
  {
    private WikiHistoryCollection _wikiHistoryCollection;
    
    public WikiHistory(DataRow row, WikiHistoryCollection wikiHistoryCollection): base(row, wikiHistoryCollection)
    {
      _wikiHistoryCollection = wikiHistoryCollection;
    }
	
    #region Properties
    
    public WikiHistoryCollection Collection
    {
      get { return _wikiHistoryCollection; }
    }
        
    
    
    
    public int HistoryID
    {
      get { return (int)Row["HistoryID"]; }
    }
    

    
    public string ArticleName
    {
      get { return Row["ArticleName"] != DBNull.Value ? (string)Row["ArticleName"] : null; }
      set { Row["ArticleName"] = CheckValue("ArticleName", value); }
    }
    
    public string Body
    {
      get { return Row["Body"] != DBNull.Value ? (string)Row["Body"] : null; }
      set { Row["Body"] = CheckValue("Body", value); }
    }
    
    public int? Version
    {
      get { return Row["Version"] != DBNull.Value ? (int?)Row["Version"] : null; }
      set { Row["Version"] = CheckValue("Version", value); }
    }
    
    public int? CreatedBy
    {
      get { return Row["CreatedBy"] != DBNull.Value ? (int?)Row["CreatedBy"] : null; }
      set { Row["CreatedBy"] = CheckValue("CreatedBy", value); }
    }
    
    public int? ModifiedBy
    {
      get { return Row["ModifiedBy"] != DBNull.Value ? (int?)Row["ModifiedBy"] : null; }
      set { Row["ModifiedBy"] = CheckValue("ModifiedBy", value); }
    }
    
    public string Comment
    {
      get { return Row["Comment"] != DBNull.Value ? (string)Row["Comment"] : null; }
      set { Row["Comment"] = CheckValue("Comment", value); }
    }
    

    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int ArticleID
    {
      get { return (int)Row["ArticleID"]; }
      set { Row["ArticleID"] = CheckValue("ArticleID", value); }
    }
    

    /* DateTime */
    
    
    

    
    public DateTime? CreatedDate
    {
      get { return Row["CreatedDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["CreatedDate"]) : null; }
      set { Row["CreatedDate"] = CheckValue("CreatedDate", value); }
    }

    public DateTime? CreatedDateUtc
    {
      get { return Row["CreatedDate"] != DBNull.Value ? (DateTime?)Row["CreatedDate"] : null; }
    }
    
    public DateTime? ModifiedDate
    {
      get { return Row["ModifiedDate"] != DBNull.Value ? DateToLocal((DateTime?)Row["ModifiedDate"]) : null; }
      set { Row["ModifiedDate"] = CheckValue("ModifiedDate", value); }
    }

    public DateTime? ModifiedDateUtc
    {
      get { return Row["ModifiedDate"] != DBNull.Value ? (DateTime?)Row["ModifiedDate"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class WikiHistoryCollection : BaseCollection, IEnumerable<WikiHistory>
  {
    public WikiHistoryCollection(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "WikiHistory"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "HistoryID"; }
    }



    public WikiHistory this[int index]
    {
      get { return new WikiHistory(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WikiHistory wikiHistory);
    partial void AfterRowInsert(WikiHistory wikiHistory);
    partial void BeforeRowEdit(WikiHistory wikiHistory);
    partial void AfterRowEdit(WikiHistory wikiHistory);
    partial void BeforeRowDelete(int historyID);
    partial void AfterRowDelete(int historyID);    

    partial void BeforeDBDelete(int historyID);
    partial void AfterDBDelete(int historyID);    

    #endregion

    #region Public Methods

    public WikiHistoryProxy[] GetWikiHistoryProxies()
    {
      List<WikiHistoryProxy> list = new List<WikiHistoryProxy>();

      foreach (WikiHistory item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int historyID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WikiHistory] WHERE ([HistoryID] = @HistoryID);";
        deleteCommand.Parameters.Add("HistoryID", SqlDbType.Int);
        deleteCommand.Parameters["HistoryID"].Value = historyID;

        BeforeDBDelete(historyID);
        BeforeRowDelete(historyID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(historyID);
        AfterDBDelete(historyID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("WikiHistoryCollectionSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[WikiHistory] SET     [ArticleID] = @ArticleID,    [OrganizationID] = @OrganizationID,    [ArticleName] = @ArticleName,    [Body] = @Body,    [Version] = @Version,    [CreatedBy] = @CreatedBy,    [CreatedDate] = @CreatedDate,    [ModifiedBy] = @ModifiedBy,    [ModifiedDate] = @ModifiedDate,    [Comment] = @Comment  WHERE ([HistoryID] = @HistoryID);";

		
		tempParameter = updateCommand.Parameters.Add("HistoryID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ArticleID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ArticleName", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Body", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Version", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatedBy", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("CreatedDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifiedBy", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ModifiedDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("Comment", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[WikiHistory] (    [ArticleID],    [OrganizationID],    [ArticleName],    [Body],    [Version],    [CreatedBy],    [CreatedDate],    [ModifiedBy],    [ModifiedDate],    [Comment]) VALUES ( @ArticleID, @OrganizationID, @ArticleName, @Body, @Version, @CreatedBy, @CreatedDate, @ModifiedBy, @ModifiedDate, @Comment); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("Comment", SqlDbType.VarChar, 500);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifiedDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ModifiedBy", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatedDate", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("CreatedBy", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Version", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Body", SqlDbType.Text, 2147483647);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ArticleName", SqlDbType.VarChar, 500);
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
		
		tempParameter = insertCommand.Parameters.Add("ArticleID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WikiHistory] WHERE ([HistoryID] = @HistoryID);";
		deleteCommand.Parameters.Add("HistoryID", SqlDbType.Int);

		try
		{
		  foreach (WikiHistory wikiHistory in this)
		  {
			if (wikiHistory.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(wikiHistory);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = wikiHistory.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["HistoryID"].AutoIncrement = false;
			  Table.Columns["HistoryID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				wikiHistory.Row["HistoryID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(wikiHistory);
			}
			else if (wikiHistory.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(wikiHistory);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = wikiHistory.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(wikiHistory);
			}
			else if (wikiHistory.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)wikiHistory.Row["HistoryID", DataRowVersion.Original];
			  deleteCommand.Parameters["HistoryID"].Value = id;
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

      foreach (WikiHistory wikiHistory in this)
      {
        if (wikiHistory.Row.Table.Columns.Contains("CreatorID") && (int)wikiHistory.Row["CreatorID"] == 0) wikiHistory.Row["CreatorID"] = LoginUser.UserID;
        if (wikiHistory.Row.Table.Columns.Contains("ModifierID")) wikiHistory.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WikiHistory FindByHistoryID(int historyID)
    {
      foreach (WikiHistory wikiHistory in this)
      {
        if (wikiHistory.HistoryID == historyID)
        {
          return wikiHistory;
        }
      }
      return null;
    }

    public virtual WikiHistory AddNewWikiHistory()
    {
      if (Table.Columns.Count < 1) LoadColumns("WikiHistory");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WikiHistory(row, this);
    }
    
    public virtual void LoadByHistoryID(int historyID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [HistoryID], [ArticleID], [OrganizationID], [ArticleName], [Body], [Version], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [Comment] FROM [dbo].[WikiHistory] WHERE ([HistoryID] = @HistoryID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("HistoryID", historyID);
        Fill(command);
      }
    }
    
    public static WikiHistory GetWikiHistory(LoginUser loginUser, int historyID)
    {
      WikiHistoryCollection wikiHistoryCollection = new WikiHistoryCollection(loginUser);
      wikiHistoryCollection.LoadByHistoryID(historyID);
      if (wikiHistoryCollection.IsEmpty)
        return null;
      else
        return wikiHistoryCollection[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WikiHistory> Members

    public IEnumerator<WikiHistory> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WikiHistory(row, this);
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
