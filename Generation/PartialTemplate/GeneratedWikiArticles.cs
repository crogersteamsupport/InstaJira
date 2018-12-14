using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class WikiArticle : BaseItem
  {
    private WikiArticles _wikiArticles;
    
    public WikiArticle(DataRow row, WikiArticles wikiArticles): base(row, wikiArticles)
    {
      _wikiArticles = wikiArticles;
    }
	
    #region Properties
    
    public WikiArticles Collection
    {
      get { return _wikiArticles; }
    }
        
    
    
    
    public int ArticleID
    {
      get { return (int)Row["ArticleID"]; }
    }
    

    
    public int? ParentID
    {
      get { return Row["ParentID"] != DBNull.Value ? (int?)Row["ParentID"] : null; }
      set { Row["ParentID"] = CheckValue("ParentID", value); }
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
    
    public bool? PublicView
    {
      get { return Row["PublicView"] != DBNull.Value ? (bool?)Row["PublicView"] : null; }
      set { Row["PublicView"] = CheckValue("PublicView", value); }
    }
    
    public bool? PublicEdit
    {
      get { return Row["PublicEdit"] != DBNull.Value ? (bool?)Row["PublicEdit"] : null; }
      set { Row["PublicEdit"] = CheckValue("PublicEdit", value); }
    }
    
    public bool? PortalView
    {
      get { return Row["PortalView"] != DBNull.Value ? (bool?)Row["PortalView"] : null; }
      set { Row["PortalView"] = CheckValue("PortalView", value); }
    }
    
    public bool? PortalEdit
    {
      get { return Row["PortalEdit"] != DBNull.Value ? (bool?)Row["PortalEdit"] : null; }
      set { Row["PortalEdit"] = CheckValue("PortalEdit", value); }
    }
    
    public bool? Private
    {
      get { return Row["Private"] != DBNull.Value ? (bool?)Row["Private"] : null; }
      set { Row["Private"] = CheckValue("Private", value); }
    }
    
    public bool? IsDeleted
    {
      get { return Row["IsDeleted"] != DBNull.Value ? (bool?)Row["IsDeleted"] : null; }
      set { Row["IsDeleted"] = CheckValue("IsDeleted", value); }
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
    

    
    public bool NeedsIndexing
    {
      get { return (bool)Row["NeedsIndexing"]; }
      set { Row["NeedsIndexing"] = CheckValue("NeedsIndexing", value); }
    }
    
    public int OrganizationID
    {
      get { return (int)Row["OrganizationID"]; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
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

  public partial class WikiArticles : BaseCollection, IEnumerable<WikiArticle>
  {
    public WikiArticles(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "WikiArticles"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ArticleID"; }
    }



    public WikiArticle this[int index]
    {
      get { return new WikiArticle(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(WikiArticle wikiArticle);
    partial void AfterRowInsert(WikiArticle wikiArticle);
    partial void BeforeRowEdit(WikiArticle wikiArticle);
    partial void AfterRowEdit(WikiArticle wikiArticle);
    partial void BeforeRowDelete(int articleID);
    partial void AfterRowDelete(int articleID);    

    partial void BeforeDBDelete(int articleID);
    partial void AfterDBDelete(int articleID);    

    #endregion

    #region Public Methods

    public WikiArticleProxy[] GetWikiArticleProxies()
    {
      List<WikiArticleProxy> list = new List<WikiArticleProxy>();

      foreach (WikiArticle item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int articleID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WikiArticles] WHERE ([ArticleID] = @ArticleID);";
        deleteCommand.Parameters.Add("ArticleID", SqlDbType.Int);
        deleteCommand.Parameters["ArticleID"].Value = articleID;

        BeforeDBDelete(articleID);
        BeforeRowDelete(articleID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(articleID);
        AfterDBDelete(articleID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("WikiArticlesSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[WikiArticles] SET     [ParentID] = @ParentID,    [OrganizationID] = @OrganizationID,    [ArticleName] = @ArticleName,    [Body] = @Body,    [Version] = @Version,    [PublicView] = @PublicView,    [PublicEdit] = @PublicEdit,    [PortalView] = @PortalView,    [PortalEdit] = @PortalEdit,    [Private] = @Private,    [IsDeleted] = @IsDeleted,    [CreatedBy] = @CreatedBy,    [CreatedDate] = @CreatedDate,    [ModifiedBy] = @ModifiedBy,    [ModifiedDate] = @ModifiedDate,    [NeedsIndexing] = @NeedsIndexing  WHERE ([ArticleID] = @ArticleID);";

		
		tempParameter = updateCommand.Parameters.Add("ArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("Body", SqlDbType.VarChar, -1);
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
		
		tempParameter = updateCommand.Parameters.Add("PublicView", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PublicEdit", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalView", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("PortalEdit", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("Private", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("IsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
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
		
		tempParameter = updateCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[WikiArticles] (    [ParentID],    [OrganizationID],    [ArticleName],    [Body],    [Version],    [PublicView],    [PublicEdit],    [PortalView],    [PortalEdit],    [Private],    [IsDeleted],    [CreatedBy],    [CreatedDate],    [ModifiedBy],    [ModifiedDate],    [NeedsIndexing]) VALUES ( @ParentID, @OrganizationID, @ArticleName, @Body, @Version, @PublicView, @PublicEdit, @PortalView, @PortalEdit, @Private, @IsDeleted, @CreatedBy, @CreatedDate, @ModifiedBy, @ModifiedDate, @NeedsIndexing); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("NeedsIndexing", SqlDbType.Bit, 1);
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
		
		tempParameter = insertCommand.Parameters.Add("IsDeleted", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Private", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalEdit", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PortalView", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PublicEdit", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("PublicView", SqlDbType.Bit, 1);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("Version", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("Body", SqlDbType.VarChar, -1);
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
		
		tempParameter = insertCommand.Parameters.Add("ParentID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[WikiArticles] WHERE ([ArticleID] = @ArticleID);";
		deleteCommand.Parameters.Add("ArticleID", SqlDbType.Int);

		try
		{
		  foreach (WikiArticle wikiArticle in this)
		  {
			if (wikiArticle.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(wikiArticle);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = wikiArticle.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ArticleID"].AutoIncrement = false;
			  Table.Columns["ArticleID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				wikiArticle.Row["ArticleID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(wikiArticle);
			}
			else if (wikiArticle.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(wikiArticle);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = wikiArticle.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(wikiArticle);
			}
			else if (wikiArticle.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)wikiArticle.Row["ArticleID", DataRowVersion.Original];
			  deleteCommand.Parameters["ArticleID"].Value = id;
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

      foreach (WikiArticle wikiArticle in this)
      {
        if (wikiArticle.Row.Table.Columns.Contains("CreatorID") && (int)wikiArticle.Row["CreatorID"] == 0) wikiArticle.Row["CreatorID"] = LoginUser.UserID;
        if (wikiArticle.Row.Table.Columns.Contains("ModifierID")) wikiArticle.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public WikiArticle FindByArticleID(int articleID)
    {
      foreach (WikiArticle wikiArticle in this)
      {
        if (wikiArticle.ArticleID == articleID)
        {
          return wikiArticle;
        }
      }
      return null;
    }

    public virtual WikiArticle AddNewWikiArticle()
    {
      if (Table.Columns.Count < 1) LoadColumns("WikiArticles");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new WikiArticle(row, this);
    }
    
    public virtual void LoadByArticleID(int articleID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ArticleID], [ParentID], [OrganizationID], [ArticleName], [Body], [Version], [PublicView], [PublicEdit], [PortalView], [PortalEdit], [Private], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate], [NeedsIndexing] FROM [dbo].[WikiArticles] WHERE ([ArticleID] = @ArticleID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ArticleID", articleID);
        Fill(command);
      }
    }
    
    public static WikiArticle GetWikiArticle(LoginUser loginUser, int articleID)
    {
      WikiArticles wikiArticles = new WikiArticles(loginUser);
      wikiArticles.LoadByArticleID(articleID);
      if (wikiArticles.IsEmpty)
        return null;
      else
        return wikiArticles[0];
    }
    
    
    

    #endregion

    #region IEnumerable<WikiArticle> Members

    public IEnumerator<WikiArticle> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new WikiArticle(row, this);
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
