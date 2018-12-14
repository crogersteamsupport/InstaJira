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
  public partial class WaterCoolerViewItem
  {
  }


  public partial class WaterCoolerView
  {
      /// <summary>
      /// This loads the top 25 threads in the WC.  The logic is not complete.
      /// It needs a selection based on users and groups
      /// </summary>
      public void LoadTop10Threads(int pageID, int itemID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "WCLoadTop10";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("PageID", pageID);
              command.Parameters.AddWithValue("AttID", itemID);
              Fill(command);
          }
      }

      public void SearchLoadByMessageID(int pageID, int itemID, int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "WCLoadMessage";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("PageID", pageID);
              command.Parameters.AddWithValue("AttID", itemID);
              command.Parameters.AddWithValue("MsgID", messageID);
              Fill(command);
          }


      }

      public void CheckMessage(int pageID, int itemID, int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "WCCheckMessage";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("MessageID", messageID);
              command.Parameters.AddWithValue("PageID", pageID);
              command.Parameters.AddWithValue("AttID", itemID);
              Fill(command);
          }
      }

      public void LoadMoreThreads(int pageID, int itemID, int msgcount)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "WCLoadMoreThreads";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@Start", msgcount + 1);
              command.Parameters.AddWithValue("@End", msgcount + 5);
              command.Parameters.AddWithValue("PageID", pageID);
              command.Parameters.AddWithValue("AttID", itemID);
              Fill(command);
          }
      }

      public void LoadMoreThreadsNoCountFilter(int pageID, int itemID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = "WCLoadMoreThreadsNoCountFilter";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("PageID", pageID);
              command.Parameters.AddWithValue("AttID", itemID);
              Fill(command);
          }
      }

      public int GetTicketWaterCoolerCount(int ticketID)
      {
          Object returnValue;

          using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
          {
              connection.Open();
              SqlCommand command = connection.CreateCommand();
              command.CommandText = "WCTicketCount";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("PageID", 0);
              command.Parameters.AddWithValue("AttID", ticketID);

              returnValue = command.ExecuteScalar();

              connection.Close();
              return (int)returnValue;

          }

      }

      public int GetLatestWatercoolerCount(string lastping)
      {
          Object returnValue;

          using (SqlConnection connection = new SqlConnection(LoginUser.ConnectionString))
          {
              connection.Open();
              SqlCommand command = connection.CreateCommand();
              command.CommandText = "WCCountNew";
              command.CommandType = CommandType.StoredProcedure;
              command.Parameters.AddWithValue("UserID", LoginUser.UserID);
              command.Parameters.AddWithValue("OrgID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("PageID", -1);
              command.Parameters.AddWithValue("AttID", -1);
              command.Parameters.AddWithValue("UserTime", lastping);

              returnValue = command.ExecuteScalar();

              connection.Close();
              return (int)returnValue;

          }

      }

      public void LoadMessage(int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              // This query isn't right just a sample to pull the top 25.
              command.CommandText = @"SELECT * FROM NewWaterCoolerView WHERE OrganizationID = @OrganizationID AND MessageID = @MsgID";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@MsgID", messageID);
              Fill(command);
          }
      }

      /// <summary>
      /// This loads replies to a WC message.
      /// </summary>
      /// <param name="messageID"></param>
      public void LoadReplies(int messageID)
      {
          using (SqlCommand command = new SqlCommand())
          {
              command.CommandText = @"SELECT * FROM WaterCoolerMsg WHERE MessageParent = @ReplyTo AND isdeleted=0 AND OrganizationID = @OrganizationID ORDER BY TimeStamp ASC";
              command.CommandType = CommandType.Text;
              command.Parameters.AddWithValue("@OrganizationID", LoginUser.OrganizationID);
              command.Parameters.AddWithValue("@ReplyTo", messageID);
              Fill(command);
          }
      }

      public void LoadForIndexing(int organizationID, int max, bool isRebuilding)
      {
        using (SqlCommand command = new SqlCommand())
        {
          string text = @"
            SELECT TOP {0} *
            FROM 
              NewWaterCoolerView wcv WITH(NOLOCK)
            WHERE 
              wcv.NeedsIndexing = 1
              AND wcv.OrganizationID = @OrganizationID
            ORDER BY 
              wcv.LastModified DESC";
          if (isRebuilding)
          {
            text = @"
            SELECT *
            FROM NewWaterCoolerView wcv WITH(NOLOCK)
            WHERE wcv.OrganizationID = @OrganizationID
            ORDER BY wcv.LastModified DESC";
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

        SearchResults results = GetSearchWaterCoolerResults(searchTerm, loginUser);

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

      public static SearchResults GetSearchWaterCoolerResults(string searchTerm, LoginUser loginUser)
      {
        Options options = new Options();
        options.TextFlags = TextFlags.dtsoTfRecognizeDates;
        using (SearchJob job = new SearchJob())
        {

          searchTerm = searchTerm.Trim();
          job.Request = searchTerm;
          //job.FieldWeights = "ArticleName: 1000";

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
          job.IndexesToSearch.Add(DataUtils.GetWaterCoolerIndexPath(loginUser));
          job.Execute();

          return job.Results;
        }

      }

      public static string GetSearchResultsWhereClause(LoginUser loginUser)
      {
        StringBuilder resultBuilder = new StringBuilder();

        SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
        searchCustomFilters.LoadByUserID(loginUser.UserID);
        resultBuilder.Append(searchCustomFilters.ConvertToWaterCoolerEquivalentWhereClause());

        return resultBuilder.ToString();
      }

  }
  
}
