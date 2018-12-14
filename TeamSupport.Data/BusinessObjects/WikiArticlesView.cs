using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using dtSearch.Engine;

namespace TeamSupport.Data
{
  public partial class WikiArticlesViewItem
  {
  }
  
  public partial class WikiArticlesView
  {
    public void LoadByOrganizationID(int organizationID, string orderBy = "ArticleName", int? limitNumber = null)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string limit = string.Empty;
        if (limitNumber != null)
        {
          limit = "TOP " + limitNumber.ToString();
        }
        command.CommandText = "SELECT " + limit + " * FROM WikiArticlesView WHERE OrganizationID = @OrganizationID AND Private <> 1 ORDER BY " + orderBy;
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public int LoadForGridCount(int organizationID, int userID,  string search)
    {
      if (search.Trim() == "") search = "\"\"";
      using (SqlCommand command = new SqlCommand())
      {
        StringBuilder builder = new StringBuilder();
        builder.Append(@" SELECT COUNT(*)
                          FROM WikiArticlesView wav LEFT JOIN WikiArticles wa ON wav.ArticleID = wa.ArticleID
                          WHERE (wav.OrganizationID = @OrganizationID)
                                  AND (ISNULL(wa.IsDeleted, 0) <> 1)
                                  AND (wa.Private = 0 OR wa.CreatedBy = @UserID)
                                  AND (
                                   (@Search = '""""') OR 
                                   (CONTAINS((wa.[Body]), @Search)) OR 
                                   (CONTAINS((wa.[ArticleName]), @Search))
                                  )
                        ");

        command.CommandText = builder.ToString();
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@Search", search);

        return (int)ExecuteScalar(command, "WikiArticleView");
      }
    }

    public void LoadForGrid(int pageIndex, int pageSize, int organizationID, int userID, string search, string sortColumn, bool sortAsc)
    {
      if (search.Trim() == "") search = @"""""";

      using (SqlCommand command = new SqlCommand())
      {
        string sort = sortColumn;

        StringBuilder builder = new StringBuilder();
        builder.Append("WITH WikiRows AS (SELECT ROW_NUMBER() OVER (ORDER BY wav.");
        builder.Append(sort);
        if (sortAsc) builder.Append(" ASC");
        else builder.Append(" DESC");
        builder.Append(") AS RowNumber, wav.*");
        builder.Append(@" 
                              	  
                                  
                                  FROM WikiArticlesView wav LEFT JOIN WikiArticles wa ON wav.ArticleID = wa.ArticleID
                                  WHERE (wav.OrganizationID = @OrganizationID)
                                  AND (wa.Private = 0 OR wa.CreatedBy = @UserID)
                                  AND (ISNULL(wa.IsDeleted, 0) <> 1)
                                  AND (
                                   (@Search = '""""') OR 
                                   (CONTAINS((wa.[Body]), @Search)) OR 
                                   (CONTAINS((wa.[ArticleName]), @Search))
                                  ))
                              	  
                                  SELECT * FROM WikiRows 
                                  WHERE RowNumber BETWEEN @PageIndex*@PageSize+1 AND @PageIndex*@PageSize+@PageSize
                                  ORDER BY RowNumber ASC");

        command.CommandText = builder.ToString();
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@PageIndex", pageIndex);
        command.Parameters.AddWithValue("@PageSize", pageSize);
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        command.Parameters.AddWithValue("@UserID", userID);
        command.Parameters.AddWithValue("@Search", search);

        Fill(command);
      }

    }

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT TOP {0} ArticleID
        FROM WikiArticlesView w WITH(NOLOCK)
        WHERE w.NeedsIndexing = 1
        AND w.OrganizationID= @OrganizationID
        ORDER BY ModifiedDate DESC";

        if (isRebuilding)
        {
          text = @"
          SELECT ArticleID
          FROM WikiArticlesView w WITH(NOLOCK)
          WHERE w.OrganizationID= @OrganizationID
          ORDER BY ModifiedDate DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public void LoadColumnNames()
    {
      using (SqlCommand command = new SqlCommand())
      {
        command.CommandText = @"
        SELECT *
        FROM WikiArticlesView w WITH(NOLOCK)
        WHERE 1 = 2";
        command.CommandType = CommandType.Text;
        Fill(command);
      }
    }

    public static string GetSearchResultsWhereClause(LoginUser loginUser)
    {
      StringBuilder resultBuilder = new StringBuilder();

      resultBuilder.Append(" AND (wav.Private = 0 OR CreatedBy = " + loginUser.UserID.ToString() + ")");

      SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
      searchCustomFilters.LoadByUserID(loginUser.UserID);
      resultBuilder.Append(searchCustomFilters.ConvertToWikiEquivalentWhereClause());

      return resultBuilder.ToString();
    }

    public static List<SqlDataRecord> GetSearchResultsList(string searchTerm, LoginUser loginUser)
    {
      SqlMetaData recordIDColumn = new SqlMetaData("recordID", SqlDbType.Int);
      SqlMetaData relevanceColumn = new SqlMetaData("relevance", SqlDbType.Int);

      SqlMetaData[] columns = new SqlMetaData[] { recordIDColumn, relevanceColumn };

      List<SqlDataRecord> result = new List<SqlDataRecord>();

      SearchResults results = GetSearchWikiResults(searchTerm, loginUser);

      List<int> items = new List<int>();
      for (int i = 0; i < results.Count; i++)
      {
        results.GetNthDoc(i);

        SqlDataRecord record = new SqlDataRecord(columns);
        record.SetInt32(0, int.Parse(results.CurrentItem.Filename));
        record.SetInt32(1, results.CurrentItem.ScorePercent);

        result.Add(record);
      }

      return result;
    }

    public static SearchResults GetSearchWikiResults(string searchTerm, LoginUser loginUser)
    {
      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;
      using (SearchJob job = new SearchJob())
      {

        searchTerm = searchTerm.Trim();
        job.Request = searchTerm;
        job.FieldWeights = "ArticleName: 1000";

        //StringBuilder conditions = new StringBuilder();
        //job.BooleanConditions = conditions.ToString();


        //job.MaxFilesToRetrieve = 1000;
        //job.AutoStopLimit = 1000000;
        job.TimeoutSeconds = 30;
        job.SearchFlags =
          //SearchFlags.dtsSearchSelectMostRecent |
          SearchFlags.dtsSearchDelayDocInfo;

        int num = 0;
        if (!int.TryParse(searchTerm, out num))
        {
          //job.Fuzziness = 1;
          job.SearchFlags = job.SearchFlags |
            //SearchFlags.dtsSearchFuzzy |
            //SearchFlags.dtsSearchStemming |
            SearchFlags.dtsSearchPositionalScoring |
            SearchFlags.dtsSearchAutoTermWeight;
        }

        if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
        job.IndexesToSearch.Add(DataUtils.GetWikiIndexPath(loginUser));
        job.Execute();

        return job.Results;
      }

    }

		public static SearchResults GetPortalSearchWikiResults(string searchTerm, LoginUser loginUser, int parentOrgID)
		{
			Options options = new Options();
			options.TextFlags = TextFlags.dtsoTfRecognizeDates;
			using (SearchJob job = new SearchJob())
			{
				StringBuilder conditions = new StringBuilder();
				conditions.Append(" ((PortalView::True) AND (IsDeleted::false))");

				job.Request = searchTerm;
				job.FieldWeights = "ArticleName: 1000";
				job.BooleanConditions = conditions.ToString();
				job.TimeoutSeconds = 30;
				job.SearchFlags = SearchFlags.dtsSearchDelayDocInfo;

				int num = 0;
				if (!int.TryParse(searchTerm, out num))
				{
					job.SearchFlags = job.SearchFlags |
						SearchFlags.dtsSearchPositionalScoring |
						SearchFlags.dtsSearchAutoTermWeight;
				}

				if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
				job.IndexesToSearch.Add(DataUtils.GetPortalWikiIndexPath(loginUser, parentOrgID));
				job.Execute();

				return job.Results;
			}
		}
	}
  
}
