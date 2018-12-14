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
  public partial class NotesViewItem
  {
  }
  
  public partial class NotesView
  {
		public void LoadbyCustomerID(int customerID)
	  {
		  using (SqlCommand command = new SqlCommand())
		  {
			  command.CommandText = "SET NOCOUNT OFF; SELECT TOP 5 [NoteID], [RefType], [RefID], [Title], [Description], [CreatorID], [ModifierID], [DateModified], [DateCreated], [NeedsIndexing], [CreatorName], [ModifierName], [ParentOrganizationID], [OrganizationName], [ContactName] FROM [dbo].[NotesView] WHERE ([RefID] = @RefID) and Reftype=9 order by DateModified desc;";
			  command.CommandType = CommandType.Text;
			  command.Parameters.AddWithValue("RefID", customerID);
			  Fill(command);
		  }
	  }

		public void LoadbyContactID(int customerID)
		{
			using (SqlCommand command = new SqlCommand())
			{
				command.CommandText = "SET NOCOUNT OFF; SELECT TOP 5 [NoteID], [RefType], [RefID], [Title], [Description], [CreatorID], [ModifierID], [DateModified], [DateCreated], [NeedsIndexing], [CreatorName], [ModifierName], [ParentOrganizationID], [OrganizationName], [ContactName] FROM [dbo].[NotesView] WHERE ([RefID] = @RefID) and Reftype=22 order by DateModified desc;";
				command.CommandType = CommandType.Text;
				command.Parameters.AddWithValue("RefID", customerID);
				Fill(command);
			}
		}

    public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
    {
      using (SqlCommand command = new SqlCommand())
      {
        string text = @"
        SELECT 
          TOP {0} 
          NoteID
        FROM 
          NotesView nv WITH(NOLOCK)
        WHERE 
          nv.NeedsIndexing = 1
          AND nv.RefType IN (9, 22)
          AND nv.ParentOrganizationID = @OrganizationID
        ORDER BY 
          nv.DateModified DESC";

        if (isRebuilding)
        {
          text = @"
          SELECT 
            NoteID
          FROM 
            NotesView nv WITH(NOLOCK)
          WHERE 
            nv.RefType IN (9, 22)
            AND nv.ParentOrganizationID = @OrganizationID
          ORDER BY 
            nv.DateModified DESC";
        }

        command.CommandText = string.Format(text, max.ToString());
        command.CommandType = CommandType.Text;
        command.Parameters.AddWithValue("@OrganizationID", organizationID);
        Fill(command);
      }
    }

    public static List<SqlDataRecord> GetSearchResultsList(string searchTerm, LoginUser loginUser)
    {
      SqlMetaData recordIDColumn = new SqlMetaData("recordID", SqlDbType.Int);
      SqlMetaData relevanceColumn = new SqlMetaData("relevance", SqlDbType.Int);

      SqlMetaData[] columns = new SqlMetaData[] { recordIDColumn, relevanceColumn };

      List<SqlDataRecord> result = new List<SqlDataRecord>();

      SearchResults results = GetSearchNotesResults(searchTerm, loginUser);

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

    public static SearchResults GetSearchNotesResults(string searchTerm, LoginUser loginUser)
    {
      Options options = new Options();
      options.TextFlags = TextFlags.dtsoTfRecognizeDates;
      using (SearchJob job = new SearchJob())
      {

        searchTerm = searchTerm.Trim();
        job.Request = searchTerm;
        job.FieldWeights = "Title: 1000";

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
        job.IndexesToSearch.Add(DataUtils.GetNotesIndexPath(loginUser));
        job.Execute();

        return job.Results;
      }

    }

    public static string GetSearchResultsWhereClause(LoginUser loginUser)
    {
      StringBuilder resultBuilder = new StringBuilder();

      SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
      searchCustomFilters.LoadByUserID(loginUser.UserID);
      resultBuilder.Append(searchCustomFilters.ConvertToNotesEquivalentWhereClause());

      return resultBuilder.ToString();
    }

  }
  
}
