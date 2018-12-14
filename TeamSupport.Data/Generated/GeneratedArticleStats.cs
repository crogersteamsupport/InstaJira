using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
  [Serializable]
  public partial class ArticleStat : BaseItem
  {
    private ArticleStats _articleStats;
    
    public ArticleStat(DataRow row, ArticleStats articleStats): base(row, articleStats)
    {
      _articleStats = articleStats;
    }
	
    #region Properties
    
    public ArticleStats Collection
    {
      get { return _articleStats; }
    }
        
    
    
    
    public int ArticleViewID
    {
      get { return (int)Row["ArticleViewID"]; }
    }
    

    
    public int? OrganizationID
    {
      get { return Row["OrganizationID"] != DBNull.Value ? (int?)Row["OrganizationID"] : null; }
      set { Row["OrganizationID"] = CheckValue("OrganizationID", value); }
    }
    
    public int? ArticleID
    {
      get { return Row["ArticleID"] != DBNull.Value ? (int?)Row["ArticleID"] : null; }
      set { Row["ArticleID"] = CheckValue("ArticleID", value); }
    }
    
    public string ViewIP
    {
      get { return Row["ViewIP"] != DBNull.Value ? (string)Row["ViewIP"] : null; }
      set { Row["ViewIP"] = CheckValue("ViewIP", value); }
    }
    
    public int? UserID
    {
      get { return Row["UserID"] != DBNull.Value ? (int?)Row["UserID"] : null; }
      set { Row["UserID"] = CheckValue("UserID", value); }
    }
    

    

    /* DateTime */
    
    
    

    
    public DateTime? ViewDateTime
    {
      get { return Row["ViewDateTime"] != DBNull.Value ? DateToLocal((DateTime?)Row["ViewDateTime"]) : null; }
      set { Row["ViewDateTime"] = CheckValue("ViewDateTime", value); }
    }

    public DateTime? ViewDateTimeUtc
    {
      get { return Row["ViewDateTime"] != DBNull.Value ? (DateTime?)Row["ViewDateTime"] : null; }
    }
    

    

    #endregion
    
    
  }

  public partial class ArticleStats : BaseCollection, IEnumerable<ArticleStat>
  {
    public ArticleStats(LoginUser loginUser): base (loginUser)
    {
    }

    #region Properties

    public override string TableName
    {
      get { return "ArticleStats"; }
    }
    
    public override string PrimaryKeyFieldName
    {
      get { return "ArticleViewID"; }
    }



    public ArticleStat this[int index]
    {
      get { return new ArticleStat(Table.Rows[index], this); }
    }
    

    #endregion

    #region Protected Members
    
    partial void BeforeRowInsert(ArticleStat articleStat);
    partial void AfterRowInsert(ArticleStat articleStat);
    partial void BeforeRowEdit(ArticleStat articleStat);
    partial void AfterRowEdit(ArticleStat articleStat);
    partial void BeforeRowDelete(int articleViewID);
    partial void AfterRowDelete(int articleViewID);    

    partial void BeforeDBDelete(int articleViewID);
    partial void AfterDBDelete(int articleViewID);    

    #endregion

    #region Public Methods

    public ArticleStatProxy[] GetArticleStatProxies()
    {
      List<ArticleStatProxy> list = new List<ArticleStatProxy>();

      foreach (ArticleStat item in this)
      {
        list.Add(item.GetProxy()); 
      }

      return list.ToArray();
    }	
	
    public virtual void DeleteFromDB(int articleViewID)
    {
        SqlCommand deleteCommand = new SqlCommand();
        deleteCommand.CommandType = CommandType.Text;
        deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ArticleStats] WHERE ([ArticleViewID] = @ArticleViewID);";
        deleteCommand.Parameters.Add("ArticleViewID", SqlDbType.Int);
        deleteCommand.Parameters["ArticleViewID"].Value = articleViewID;

        BeforeDBDelete(articleViewID);
        BeforeRowDelete(articleViewID);
        TryDeleteFromDB(deleteCommand);
        AfterRowDelete(articleViewID);
        AfterDBDelete(articleViewID);
	}

    public override void Save(SqlConnection connection)    {
		//SqlTransaction transaction = connection.BeginTransaction("ArticleStatsSave");
		SqlParameter tempParameter;
		SqlCommand updateCommand = connection.CreateCommand();
		updateCommand.Connection = connection;
		//updateCommand.Transaction = transaction;
		updateCommand.CommandType = CommandType.Text;
		updateCommand.CommandText = "SET NOCOUNT OFF; UPDATE [dbo].[ArticleStats] SET     [OrganizationID] = @OrganizationID,    [ArticleID] = @ArticleID,    [ViewDateTime] = @ViewDateTime,    [ViewIP] = @ViewIP,    [UserID] = @UserID  WHERE ([ArticleViewID] = @ArticleViewID);";

		
		tempParameter = updateCommand.Parameters.Add("ArticleViewID", SqlDbType.Int, 4);
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
		
		tempParameter = updateCommand.Parameters.Add("ArticleID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViewDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = updateCommand.Parameters.Add("ViewIP", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = updateCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		

		SqlCommand insertCommand = connection.CreateCommand();
		insertCommand.Connection = connection;
		//insertCommand.Transaction = transaction;
		insertCommand.CommandType = CommandType.Text;
		insertCommand.CommandText = "SET NOCOUNT OFF; INSERT INTO [dbo].[ArticleStats] (    [OrganizationID],    [ArticleID],    [ViewDateTime],    [ViewIP],    [UserID]) VALUES ( @OrganizationID, @ArticleID, @ViewDateTime, @ViewIP, @UserID); SET @Identity = SCOPE_IDENTITY();";

		
		tempParameter = insertCommand.Parameters.Add("UserID", SqlDbType.Int, 4);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 10;
		  tempParameter.Scale = 10;
		}
		
		tempParameter = insertCommand.Parameters.Add("ViewIP", SqlDbType.VarChar, 100);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 255;
		  tempParameter.Scale = 255;
		}
		
		tempParameter = insertCommand.Parameters.Add("ViewDateTime", SqlDbType.DateTime, 8);
		if (tempParameter.SqlDbType == SqlDbType.Float)
		{
		  tempParameter.Precision = 23;
		  tempParameter.Scale = 23;
		}
		
		tempParameter = insertCommand.Parameters.Add("ArticleID", SqlDbType.Int, 4);
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
		deleteCommand.CommandText = "SET NOCOUNT OFF;  DELETE FROM [dbo].[ArticleStats] WHERE ([ArticleViewID] = @ArticleViewID);";
		deleteCommand.Parameters.Add("ArticleViewID", SqlDbType.Int);

		try
		{
		  foreach (ArticleStat articleStat in this)
		  {
			if (articleStat.Row.RowState == DataRowState.Added)
			{
			  BeforeRowInsert(articleStat);
			  for (int i = 0; i < insertCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = insertCommand.Parameters[i];
				if (parameter.Direction != ParameterDirection.Output)
				{
				  parameter.Value = articleStat.Row[parameter.ParameterName];
				}
			  }

			  if (insertCommand.Parameters.Contains("ModifierID")) insertCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (insertCommand.Parameters.Contains("CreatorID") && (int)insertCommand.Parameters["CreatorID"].Value == 0) insertCommand.Parameters["CreatorID"].Value = LoginUser.UserID;

			  insertCommand.ExecuteNonQuery();
			  Table.Columns["ArticleViewID"].AutoIncrement = false;
			  Table.Columns["ArticleViewID"].ReadOnly = false;
			  if (insertCommand.Parameters["Identity"].Value != DBNull.Value)
				articleStat.Row["ArticleViewID"] = (int)insertCommand.Parameters["Identity"].Value;
			  AfterRowInsert(articleStat);
			}
			else if (articleStat.Row.RowState == DataRowState.Modified)
			{
			  BeforeRowEdit(articleStat);
			  for (int i = 0; i < updateCommand.Parameters.Count; i++)
			  {
				SqlParameter parameter = updateCommand.Parameters[i];
				parameter.Value = articleStat.Row[parameter.ParameterName];
			  }
			  if (updateCommand.Parameters.Contains("ModifierID")) updateCommand.Parameters["ModifierID"].Value = LoginUser.UserID;
			  if (updateCommand.Parameters.Contains("DateModified")) updateCommand.Parameters["DateModified"].Value = DateTime.UtcNow;

			  updateCommand.ExecuteNonQuery();
			  AfterRowEdit(articleStat);
			}
			else if (articleStat.Row.RowState == DataRowState.Deleted)
			{
			  int id = (int)articleStat.Row["ArticleViewID", DataRowVersion.Original];
			  deleteCommand.Parameters["ArticleViewID"].Value = id;
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

      foreach (ArticleStat articleStat in this)
      {
        if (articleStat.Row.Table.Columns.Contains("CreatorID") && (int)articleStat.Row["CreatorID"] == 0) articleStat.Row["CreatorID"] = LoginUser.UserID;
        if (articleStat.Row.Table.Columns.Contains("ModifierID")) articleStat.Row["ModifierID"] = LoginUser.UserID;
      }
    
      SqlBulkCopy copy = new SqlBulkCopy(LoginUser.ConnectionString);
      copy.BulkCopyTimeout = 0;
      copy.DestinationTableName = TableName;
      copy.WriteToServer(Table);

      Table.AcceptChanges();
     
      if (DataCache != null) DataCache.InvalidateItem(TableName, LoginUser.OrganizationID);
    }

    public ArticleStat FindByArticleViewID(int articleViewID)
    {
      foreach (ArticleStat articleStat in this)
      {
        if (articleStat.ArticleViewID == articleViewID)
        {
          return articleStat;
        }
      }
      return null;
    }

    public virtual ArticleStat AddNewArticleStat()
    {
      if (Table.Columns.Count < 1) LoadColumns("ArticleStats");
      DataRow row = Table.NewRow();
      Table.Rows.Add(row);
      return new ArticleStat(row, this);
    }
    
    public virtual void LoadByArticleViewID(int articleViewID)
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = "SET NOCOUNT OFF; SELECT [ArticleViewID], [OrganizationID], [ArticleID], [ViewDateTime], [ViewIP], [UserID] FROM [dbo].[ArticleStats] WHERE ([ArticleViewID] = @ArticleViewID);";
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("ArticleViewID", articleViewID);
        Fill(command);
      }
    }
    
    public static ArticleStat GetArticleStat(LoginUser loginUser, int articleViewID)
    {
      ArticleStats articleStats = new ArticleStats(loginUser);
      articleStats.LoadByArticleViewID(articleViewID);
      if (articleStats.IsEmpty)
        return null;
      else
        return articleStats[0];
    }
    
    
    

    #endregion

    #region IEnumerable<ArticleStat> Members

    public IEnumerator<ArticleStat> GetEnumerator()
    {
      foreach (DataRow row in Table.Rows)
      {
        yield return new ArticleStat(row, this);
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
