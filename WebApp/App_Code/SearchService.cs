using Microsoft.SqlServer.Server;
using dtSearch.Engine;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Diagnostics;
using NewRelic.Api;
using System.Dynamic;
using Microsoft.Security.Application;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SearchService : System.Web.Services.WebService
    {
        public SearchService()
        {

        }

        // The fields available to filter are the TicketsView fields.
        // Nevertheless some of these fields also exists in the WikiArticlesView (e.g. ParentID)
        // If a field that does not exists in or have no equivalent in the WikiArticlesView is selected as a filter,
        // the wiki index will not be included in the search and only tickets results will be displayed.
        [WebMethod]
        public SearchResultsProxy GetSearchResults(string searchTerm, int firstItemIndex, int pageSize)
        {
            SearchResultsProxy result = new SearchResultsProxy();
            List<SearchResultsItemProxy> resultItems = new List<SearchResultsItemProxy>();

            LoginUser loginUser = TSAuthentication.GetLoginUser();

            SearchStandardFilter searchStandardFilter = GetSearchStandardFilter(loginUser);

            bool hasNonWikiFilters = false;
            bool hasNonNotesFilters = false;
            bool hasNonProductVersionsFilters = false;
            bool hasNonTasksFilters = false;
            bool hasNonWaterCoolerFilters = false;

            bool hasIndexesToSearch = GetHasIndexesToSearch(
                                        searchStandardFilter,
                                        loginUser,
                                        ref hasNonWikiFilters,
                                        ref hasNonNotesFilters,
                                        ref hasNonProductVersionsFilters,
                                        ref hasNonTasksFilters,
                                        ref hasNonWaterCoolerFilters);

            if (hasIndexesToSearch)
            {
                string tempItemsTableFieldsDefinition = string.Empty;
                string tempItemsTableFields = string.Empty;

                string ticketsQuery = string.Empty;
                string wikisQuery = string.Empty;
                string notesQuery = string.Empty;
                string productVersionsQuery = string.Empty;
                string tasksQuery = string.Empty;
                string waterCoolerQuery = string.Empty;

                string orderByClause = string.Empty;

                string fieldsList = string.Empty;

                string selectTicketsFields = string.Empty;
                string selectWikisFields = string.Empty;
                string selectNotesFields = string.Empty;
                string selectProductVersionsFields = string.Empty;
                string selectTasksFields = string.Empty;
                string selectWaterCoolerFields = string.Empty;

                orderByClause = GetOrderByClause(loginUser,
                                                 ref tempItemsTableFieldsDefinition,
                                                 ref tempItemsTableFields,
                                                 ref selectTicketsFields,
                                                 ref selectWikisFields,
                                                 ref selectNotesFields,
                                                 ref selectProductVersionsFields,
                                                 ref selectTasksFields,
                                                 ref selectWaterCoolerFields);

                SqlCommand command = new SqlCommand();
                TeamSupport.Data.User user = Users.GetUser(loginUser, loginUser.UserID);

                if (searchStandardFilter.Tickets || searchStandardFilter.KnowledgeBase)
                {
                    List<SqlDataRecord> dtSearchTicketsResultsList = TicketsView.GetSearchResultsList(searchTerm, loginUser);

                    if (dtSearchTicketsResultsList.Count > 0)
                    {
                        if ((ProductFamiliesRightType)user.ProductFamiliesRights != ProductFamiliesRightType.AllFamilies)
                        {
                            ticketsQuery = GetProductFamilyTicketsQuery(searchTerm, loginUser, selectTicketsFields);
                        }
                        else
                        {
                            ticketsQuery = GetTicketsQuery(searchTerm, loginUser, selectTicketsFields);
                        }

                        SqlParameter dtSearchTicketsResultsTable = new SqlParameter("@dtSearchTicketsResultsTable", SqlDbType.Structured);
                        dtSearchTicketsResultsTable.TypeName = "dtSearch_results_tbltype";
                        dtSearchTicketsResultsTable.Value = dtSearchTicketsResultsList;
                        command.Parameters.Add(dtSearchTicketsResultsTable);
                    }
                }

                if (searchStandardFilter.Wikis && !hasNonWikiFilters)
                {
                    List<SqlDataRecord> dtSearchWikisResultsList = WikiArticlesView.GetSearchResultsList(searchTerm, loginUser);

                    if (dtSearchWikisResultsList.Count > 0)
                    {
                        bool includesTickets = false;
                        if (ticketsQuery.Length > 0)
                        {
                            includesTickets = true;
                        }

                        wikisQuery = GetWikisQuery(includesTickets, loginUser, selectWikisFields);

                        SqlParameter dtSearchWikisResultsTable = new SqlParameter("@dtSearchWikisResultsTable", SqlDbType.Structured);
                        dtSearchWikisResultsTable.TypeName = "dtSearch_results_tbltype";
                        dtSearchWikisResultsTable.Value = dtSearchWikisResultsList;
                        command.Parameters.Add(dtSearchWikisResultsTable);
                    }
                }

                if (searchStandardFilter.Notes && !hasNonNotesFilters)
                {
                    List<SqlDataRecord> dtSearchNotesResultsList = NotesView.GetSearchResultsList(searchTerm, loginUser);

                    if (dtSearchNotesResultsList.Count > 0)
                    {
                        bool includesPreviousQuery = false;
                        if (ticketsQuery.Length > 0 || wikisQuery.Length > 0)
                        {
                            includesPreviousQuery = true;
                        }

                        notesQuery = GetNotesQuery(includesPreviousQuery, loginUser, selectNotesFields);

                        SqlParameter dtSearchNotesResultsTable = new SqlParameter("@dtSearchNotesResultsTable", SqlDbType.Structured);
                        dtSearchNotesResultsTable.TypeName = "dtSearch_results_tbltype";
                        dtSearchNotesResultsTable.Value = dtSearchNotesResultsList;
                        command.Parameters.Add(dtSearchNotesResultsTable);
                    }
                }

                if (searchStandardFilter.ProductVersions && !hasNonProductVersionsFilters)
                {
                    List<SqlDataRecord> dtSearchProductVersionsResultsList = ProductVersionsView.GetSearchResultsList(searchTerm, loginUser);

                    if (dtSearchProductVersionsResultsList.Count > 0)
                    {
                        bool includesPreviousQuery = false;
                        if (ticketsQuery.Length > 0 || wikisQuery.Length > 0 || notesQuery.Length > 0)
                        {
                            includesPreviousQuery = true;
                        }

                        if ((ProductFamiliesRightType)user.ProductFamiliesRights != ProductFamiliesRightType.AllFamilies)
                        {
                            productVersionsQuery = GetProductVersionsWithinProductFamilyQuery(includesPreviousQuery, loginUser, selectProductVersionsFields);
                        }
                        else
                        {
                            productVersionsQuery = GetProductVersionsQuery(includesPreviousQuery, loginUser, selectProductVersionsFields);
                        }

                        SqlParameter dtSearchProductVersionsResultsTable = new SqlParameter("@dtSearchProductVersionsResultsTable", SqlDbType.Structured);
                        dtSearchProductVersionsResultsTable.TypeName = "dtSearch_results_tbltype";
                        dtSearchProductVersionsResultsTable.Value = dtSearchProductVersionsResultsList;
                        command.Parameters.Add(dtSearchProductVersionsResultsTable);
                    }
                }

                if (searchStandardFilter.Tasks && !hasNonTasksFilters)
                {
                    List<SqlDataRecord> dtSearchTasksResultsList = TasksView.GetSearchResultsList(searchTerm, loginUser);

                    if (dtSearchTasksResultsList.Count > 0)
                    {
                        bool includesPreviousQuery = false;
                        if (ticketsQuery.Length > 0 || wikisQuery.Length > 0 || notesQuery.Length > 0 || productVersionsQuery.Length > 0)
                        {
                            includesPreviousQuery = true;
                        }

                        tasksQuery = GetTasksQuery(includesPreviousQuery, loginUser, selectTasksFields);

                        SqlParameter dtSearchTasksResultsTable = new SqlParameter("@dtSearchTasksResultsTable", SqlDbType.Structured);
                        dtSearchTasksResultsTable.TypeName = "dtSearch_results_tbltype";
                        dtSearchTasksResultsTable.Value = dtSearchTasksResultsList;
                        command.Parameters.Add(dtSearchTasksResultsTable);
                    }
                }

                if (searchStandardFilter.WaterCooler && !hasNonWaterCoolerFilters)
                {
                    List<SqlDataRecord> dtSearchWaterCoolerResultsList = WaterCoolerView.GetSearchResultsList(searchTerm, loginUser);

                    if (dtSearchWaterCoolerResultsList.Count > 0)
                    {
                        bool includesPreviousQuery = false;
                        if (ticketsQuery.Length > 0 || wikisQuery.Length > 0 || notesQuery.Length > 0 || productVersionsQuery.Length > 0 || tasksQuery.Length > 0)
                        {
                            includesPreviousQuery = true;
                        }

                        waterCoolerQuery = GetWaterCoolerQuery(includesPreviousQuery, loginUser, selectWaterCoolerFields);

                        SqlParameter dtSearchWaterCoolerResultsTable = new SqlParameter("@dtSearchWaterCoolerResultsTable", SqlDbType.Structured);
                        dtSearchWaterCoolerResultsTable.TypeName = "dtSearch_results_tbltype";
                        dtSearchWaterCoolerResultsTable.Value = dtSearchWaterCoolerResultsList;
                        command.Parameters.Add(dtSearchWaterCoolerResultsTable);
                    }
                }

                fieldsList = GetFieldsList();

                string query = @"
        DECLARE @TempItems
        TABLE
        (
          ID            int IDENTITY,
          recordID      int,
          source        int,
          relevance     int,
          DateModified  datetime
          {0}
        )

        INSERT INTO @TempItems
        (
          recordID,
          source,
          relevance,
          DateModified
          {1}
        )
        {2}
        {3}
        {4}
        {5}
        {6}
        {7}
        {8}

        SET @resultsCount = @@RowCount

        SELECT
          {9}
        FROM
          @TempItems ti
          LEFT JOIN dbo.TicketsView tv
            ON ti.source = 17
            AND ti.recordID = tv.TicketID
          LEFT JOIN dbo.WikiArticlesView wv
            ON ti.source = 39
            AND ti.recordID = wv.ArticleID
          LEFT JOIN dbo.NotesView nv
            ON ti.source = 40
            AND ti.recordID = nv.NoteID
          LEFT JOIN dbo.ProductVersionsView pvv
            ON ti.source = 14
            AND ti.recordID = pvv.ProductVersionID
          LEFT JOIN dbo.TasksView tsk
            ON ti.source = 61
            AND ti.recordID = tsk.TaskID
          LEFT JOIN dbo.NewWaterCoolerView wcv
            ON ti.source = 38
            AND ti.recordID = wcv.MessageID
        WHERE
          ti.ID BETWEEN @FromIndex AND @toIndex
        ORDER BY
          ti.ID
        ";

                command.CommandText = string.Format(
                  query,
                  tempItemsTableFieldsDefinition,
                  tempItemsTableFields,
                  ticketsQuery,
                  wikisQuery,
                  notesQuery,
                  productVersionsQuery,
                  tasksQuery,
                  waterCoolerQuery,
                  orderByClause,
                  fieldsList);
                command.CommandType = CommandType.Text;

                SqlParameter resultsCount = new SqlParameter("@resultsCount", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(resultsCount);

                command.Parameters.AddWithValue("@FromIndex", firstItemIndex + 1);
                command.Parameters.AddWithValue("@ToIndex", firstItemIndex + pageSize);

                command.Parameters.AddWithValue("@userID", loginUser.UserID);

                DataTable table = new DataTable();
                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    command.Connection = connection;
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        try
                        {
                            adapter.Fill(table);
                            if (resultsCount.Value != DBNull.Value)
                            {
                                result.Count = (int)resultsCount.Value;
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }

                foreach (DataRow row in table.Rows)
                {
                    SearchResultsItemProxy resultItem = new SearchResultsItemProxy();

                    resultItem.ScorePercent = (int)row["relevance"];

                    switch ((ReferenceType)row["source"])
                    {
                        case ReferenceType.Tickets:
                            resultItem.ID = (int)row["TicketID"];
                            resultItem.DisplayName = string.Format("{0}: {1}", row["TicketNumber"].ToString(), row["Name"].ToString());
                            resultItem.Number = (int)row["TicketNumber"];
                            resultItem.Status = row["Status"].ToString();
                            resultItem.Severity = row["Severity"].ToString();

                            if ((bool)row["IsKnowledgeBase"])
                            {
                                resultItem.TypeID = 2;
                            }
                            else
                            {
                                resultItem.TypeID = 1;
                            }

                            break;
                        case ReferenceType.Wikis:
                            resultItem.ID = (int)row["ArticleID"];
                            resultItem.DisplayName = row["ArticleName"].ToString();
                            resultItem.Creator = row["Creator"].ToString();
                            resultItem.Modifier = row["Modifier"].ToString();
                            resultItem.TypeID = 3;

                            break;
                        case ReferenceType.Notes:
                            resultItem.ID = (int)row["NoteID"];
                            resultItem.CustomerID = (int)row["RefID"];
                            resultItem.Creator = row["CreatorName"].ToString();
                            resultItem.DateModified = row["DateModified"].ToString();
                            switch ((ReferenceType)row["NoteRefType"])
                            {
                                case ReferenceType.Organizations:
                                    resultItem.DisplayName = string.Format("{0}'s activity: {1}", row["OrganizationName"].ToString(), row["Title"].ToString());
                                    resultItem.TypeID = 4;
                                    break;
                                case ReferenceType.Users:
                                    resultItem.DisplayName = string.Format("{0}'s activity: {1}", row["ContactName"].ToString(), row["Title"].ToString());
                                    resultItem.TypeID = 7;
                                    break;
                            }

                            break;
                        case ReferenceType.ProductVersions:
                            resultItem.ID = (int)row["ProductVersionID"];
                            resultItem.ProductID = (int)row["ProductID"];
                            resultItem.DisplayName = string.Format("{0}'s version {1}", row["ProductName"].ToString(), row["VersionNumber"].ToString());
                            resultItem.Status = row["VersionStatus"].ToString();
                            resultItem.DateModified = row["ProductVersionDateModified"].ToString();
                            resultItem.TypeID = 5;

                            break;
                        case ReferenceType.Tasks:
                            resultItem.ID = (int)row["TaskID"];
                            resultItem.IsComplete = Convert.ToBoolean(row["IsComplete"]);
                            resultItem.UserName = row["UserName"].ToString();
                            resultItem.IsPastDue = Convert.ToBoolean(row["IsPastDue"]);
                            resultItem.TypeID = 8;

                            if (string.IsNullOrEmpty(row["TaskParentName"].ToString()))
                            {
                                resultItem.DisplayName = row["TaskName"].ToString();
                            }
                            else
                            {
                                resultItem.DisplayName = row["TaskParentName"].ToString() + " > " + row["TaskName"].ToString();
                            }

                            if (!string.IsNullOrEmpty(row["DateCompleted"].ToString()))
                            {
                                resultItem.DateCompleted = DataUtils.DateToLocal(loginUser, Convert.ToDateTime(row["DateCompleted"]));
                            }

                            if (string.IsNullOrEmpty(resultItem.UserName))
                            {
                                resultItem.UserName = "Unassigned";
                            }

                            if (!string.IsNullOrEmpty(row["DueDate"].ToString()))
                            {
                                resultItem.DueDate = DataUtils.DateToLocal(loginUser, Convert.ToDateTime(row["DueDate"]));
                            }
                            break;
                        case ReferenceType.WaterCooler:
                            int messageParent = (int)row["MessageParent"];
                            resultItem.ID = messageParent == -1 ? (int)row["MessageID"] : messageParent;
                            resultItem.DisplayName = row["Message"].ToString();
                            if (resultItem.DisplayName.Length > 100)
                            {
                                resultItem.DisplayName = resultItem.DisplayName.Substring(0, 100) + "...";
                            }
                            resultItem.Creator = row["UserName"].ToString();
                            resultItem.DateModified = row["WaterCoolerLastModified"].ToString();
                            resultItem.TypeID = 6;
                            //resultItem.RefType = row["RefType"] is System.DBNull ? -1 : (int?)row["RefType"];
                            //resultItem.AttachmentID = row["AttachmentID"] is System.DBNull ? -1 : (int?)row["AttachmentID"];
                            resultItem.RefType = -1;
                            resultItem.AttachmentID = -1;


                            break;

                    }

                    resultItems.Add(resultItem);
                }
            }

            result.Items = resultItems.ToArray();

            return result;
        }

        private SearchStandardFilter GetSearchStandardFilter(LoginUser loginUser)
        {
            SearchStandardFilter result = null;

            SearchStandardFilters searchStandardFilters = new SearchStandardFilters(loginUser);
            searchStandardFilters.LoadByUserID(loginUser.UserID);

            if (searchStandardFilters.Count == 0)
            {
                // Calling AddNewSearchStandardFilter after LoadByUserID generates a 'UserID doesn't allow null' exception.
                searchStandardFilters = new SearchStandardFilters(loginUser);
                result = searchStandardFilters.AddNewSearchStandardFilter();
                result.Tickets = true;
                result.KnowledgeBase = true;
                result.Wikis = true;
            }
            else
            {
                result = searchStandardFilters[0];
            }

            return result;
        }

        private bool GetHasIndexesToSearch(
          SearchStandardFilter searchStandardFilter,
          LoginUser loginUser,
          ref bool hasNonWikiFilters,
          ref bool hasNonNotesFilters,
          ref bool hasNonProductVersionsFilters,
          ref bool hasNonTasksFilters,
          ref bool hasNonWaterCoolerFilters)
        {
            bool result = false;

            //The NonTable filters allows us to exclude searching objects that will never match a filter
            //as it is bein applied in fields that do not belong to them.
            GetHasNonTableFilters(
              searchStandardFilter,
              loginUser,
              ref hasNonWikiFilters,
              ref hasNonNotesFilters,
              ref hasNonProductVersionsFilters,
              ref hasNonTasksFilters,
              ref hasNonWaterCoolerFilters);

            if (
              searchStandardFilter.Tickets
              || searchStandardFilter.KnowledgeBase
              || (searchStandardFilter.Wikis && !hasNonWikiFilters)
              || (searchStandardFilter.Notes && !hasNonNotesFilters)
              || (searchStandardFilter.ProductVersions && !hasNonProductVersionsFilters)
              || (searchStandardFilter.Tasks && !hasNonTasksFilters)
              || (searchStandardFilter.WaterCooler && !hasNonWaterCoolerFilters)
            )
            {
                result = true;
            }

            return result;
        }

        // This method is written under the assumption the user.ProductFamiliesRights is not AllFamilies.
        private string GetProductFamilyTicketsQuery(string searchTerm, LoginUser loginUser, string selectTicketsFields)
        {
            string result = @"
        SELECT
          tv.TicketID
          , 17
          , dtrt.relevance
          , tv.DateModified
          {0}
        FROM
          dbo.TicketsView tv
          JOIN Organizations o
            ON tv.OrganizationID = o.OrganizationID
          LEFT JOIN Products p
            ON tv.ProductID = p.ProductID
          LEFT JOIN UserRightsProductFamilies urpf
            ON p.ProductFamilyID = urpf.ProductFamilyID
          JOIN @dtSearchTicketsResultsTable dtrt
            ON tv.TicketID = dtrt.recordID
	        {1}
        WHERE
          (
            o.UseProductFamilies = 0
            OR tv.UserID = " + loginUser.UserID + @"
            OR urpf.UserID = " + loginUser.UserID + @"
            OR tv.ProductID IS NULL
          )
          {2}
      ";

            string joinClause = GetJoinClause(loginUser);

            string whereClause = TicketsView.GetSearchResultsWhereClause(loginUser);

            return string.Format(result, selectTicketsFields, joinClause, whereClause);
        }

        private string GetTicketsQuery(string searchTerm, LoginUser loginUser, string selectTicketsFields)
        {
            string result = @"
        SELECT
          tv.TicketID
          , 17
          , dtrt.relevance
          , tv.DateModified
          {0}
        FROM
          dbo.TicketsView tv
          JOIN @dtSearchTicketsResultsTable dtrt
            ON tv.TicketID = dtrt.recordID
	        {1}
        WHERE
          1 = 1
          {2}
      ";

            string joinClause = GetJoinClause(loginUser);

            string whereClause = TicketsView.GetSearchResultsWhereClause(loginUser);

            return string.Format(result, selectTicketsFields, joinClause, whereClause);
        }

        private string GetJoinClause(LoginUser loginUser)
        {
            SearchCustomFilters searchCustomFilters = new SearchCustomFilters(loginUser);
            searchCustomFilters.LoadByUserID(loginUser.UserID);
            return searchCustomFilters.GetJoinClause();
        }

        private string GetWikisQuery(bool includeTickets, LoginUser loginUser, string selectWikisFields)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (includeTickets)
            {
                resultBuilder.Append(" UNION ");
            }

            resultBuilder.Append(@"
        SELECT
          wav.ArticleID
          , 39
          , dwrt.relevance
          , wav.ModifiedDate AS DateModified");
            resultBuilder.Append(selectWikisFields);

            resultBuilder.Append(@"
        FROM
          dbo.WikiArticlesView wav
          JOIN @dtSearchWikisResultsTable dwrt
            ON wav.ArticleID = dwrt.RecordID
        WHERE
          1 = 1
      ");

            resultBuilder.Append(WikiArticlesView.GetSearchResultsWhereClause(loginUser));

            return resultBuilder.ToString();
        }

        private string GetNotesQuery(bool includePreviousQuery, LoginUser loginUser, string selectNotesFields)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (includePreviousQuery)
            {
                resultBuilder.Append(" UNION ");
            }

            resultBuilder.Append(@"
        SELECT
          nv.NoteID
          , 40
          , dnrt.relevance
          , nv.DateModified");
            resultBuilder.Append(selectNotesFields);

            resultBuilder.Append(@"
        FROM
          dbo.NotesView nv
          JOIN @dtSearchNotesResultsTable dnrt
            ON nv.NoteID = dnrt.RecordID
        WHERE
          1 = 1
      ");

            resultBuilder.Append(NotesView.GetSearchResultsWhereClause(loginUser));

            return resultBuilder.ToString();
        }

        private string GetProductVersionsWithinProductFamilyQuery(bool includePreviousQuery, LoginUser loginUser, string selectProductVersionsFields)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (includePreviousQuery)
            {
                resultBuilder.Append(" UNION ");
            }

            resultBuilder.Append(@"
        SELECT
          pvv.ProductVersionID
          , 14
          , dpvrt.relevance
          , pvv.DateModified");
            resultBuilder.Append(selectProductVersionsFields);

            resultBuilder.Append(@"
        FROM
          dbo.ProductVersionsView pvv
          JOIN Organizations o
            ON pvv.OrganizationID = o.OrganizationID
          LEFT JOIN Products p
            ON pvv.ProductID = p.ProductID
          LEFT JOIN UserRightsProductFamilies urpf
            ON p.ProductFamilyID = urpf.ProductFamilyID
          JOIN @dtSearchProductVersionsResultsTable dpvrt
            ON pvv.ProductVersionID = dpvrt.RecordID
        WHERE
          (
            o.UseProductFamilies = 0
            OR urpf.UserID = " + loginUser.UserID + @"
          )
      ");

            resultBuilder.Append(ProductVersionsView.GetSearchResultsWhereClause(loginUser));

            return resultBuilder.ToString();
        }

        private string GetProductVersionsQuery(bool includePreviousQuery, LoginUser loginUser, string selectProductVersionsFields)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (includePreviousQuery)
            {
                resultBuilder.Append(" UNION ");
            }

            resultBuilder.Append(@"
        SELECT
          pvv.ProductVersionID
          , 14
          , dpvrt.relevance
          , pvv.DateModified");
            resultBuilder.Append(selectProductVersionsFields);

            resultBuilder.Append(@"
        FROM
          dbo.ProductVersionsView pvv
          JOIN @dtSearchProductVersionsResultsTable dpvrt
            ON pvv.ProductVersionID = dpvrt.RecordID
        WHERE
          1 = 1
      ");

            resultBuilder.Append(ProductVersionsView.GetSearchResultsWhereClause(loginUser));

            return resultBuilder.ToString();
        }

        private string GetTasksQuery(bool includePreviousQuery, LoginUser loginUser, string selectTasksFields)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (includePreviousQuery)
            {
                resultBuilder.Append(" UNION ");
            }

            resultBuilder.Append(@"
        SELECT
          tsk.TaskID
          , 61
          , dtskrt.relevance
          , tsk.DateModified");
            resultBuilder.Append(selectTasksFields);

            resultBuilder.Append(@"
        FROM
          dbo.TasksView tsk
          JOIN @dtSearchTasksResultsTable dtskrt
            ON tsk.TaskID = dtskrt.RecordID
        WHERE
          1 = 1
      ");

            resultBuilder.Append(TasksView.GetSearchResultsWhereClause(loginUser));

            return resultBuilder.ToString();
        }

        private string GetWaterCoolerQuery(bool includePreviousQuery, LoginUser loginUser, string selectWaterCoolerFields)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (includePreviousQuery)
            {
                resultBuilder.Append(" UNION ");
            }

            resultBuilder.Append(@"
        SELECT
          wcv.MessageID
          , 38
          , dpvrt.relevance
          , wcv.LastModified AS DateModified");
            resultBuilder.Append(selectWaterCoolerFields);

            resultBuilder.Append(@"
        FROM
          dbo.NewWaterCoolerView wcv
          JOIN @dtSearchWaterCoolerResultsTable dpvrt
            ON wcv.MessageID = dpvrt.RecordID
          LEFT JOIN dbo.NewWaterCoolerView pwcv
            ON wcv.MessageParent = pwcv.MessageID
          LEFT JOIN (SELECT COUNT(*) AS Count, x.MessageID FROM WatercoolerAttachments x WHERE x.RefType IN (3,4) GROUP BY x.MessageID) userOrGroupAttachments
            ON (wcv.MessageParent < 0 AND wcv.MessageID = userOrGroupAttachments.MessageID)
            OR (wcv.MessageParent > 0 AND wcv.MessageParent = userOrGroupAttachments.MessageID)
          LEFT JOIN (SELECT COUNT(*) AS Count, x.MessageID FROM WatercoolerAttachments x WHERE x.RefType = 3 AND x.AttachmentID = @userID GROUP BY x.MessageID) userAttachments
            ON (wcv.MessageParent < 0 AND wcv.MessageID = userAttachments.MessageID)
            OR (wcv.MessageParent > 0 AND wcv.MessageParent = userAttachments.MessageID)
          LEFT JOIN (SELECT COUNT(*) AS Count, x.MessageID FROM WatercoolerAttachments x WHERE x.RefType = 4 AND x.AttachmentID IN (SELECT gu.GroupID FROM GroupUsers gu WHERE gu.UserID = @UserID) GROUP BY x.MessageID) groupAttachments
            ON (wcv.MessageParent < 0 AND wcv.MessageID = groupAttachments.MessageID)
            OR (wcv.MessageParent > 0 AND wcv.MessageParent = groupAttachments.MessageID)
        WHERE
          wcv.IsDeleted = 0
          AND
          (
            (wcv.MessageParent < 0 AND wcv.UserID = @userID) OR (wcv.MessageParent > 0 AND pwcv.UserID = @userID)
            OR userOrGroupAttachments.Count IS NULL
            OR userAttachments.Count > 0
            OR groupAttachments.Count > 0
          )
      ");

            resultBuilder.Append(WaterCoolerView.GetSearchResultsWhereClause(loginUser));

            return resultBuilder.ToString();
        }

        private string GetOrderByClause(LoginUser loginUser,
                                        ref string tempItemsTableFieldsDefinition,
                                        ref string tempItemsTableFields,
                                        ref string selectTicketsFields,
                                        ref string selectWikisFields,
                                        ref string selectNotesFields,
                                        ref string selectProductVersionsFields,
                                        ref string selectTasksFields,
                                        ref string selectWaterCoolerFields)
        {
            SearchSorters searchSorters = new SearchSorters(loginUser);
            searchSorters.LoadByUserID(loginUser.UserID);
            return searchSorters.ConvertToOrderByClause(ref tempItemsTableFieldsDefinition,
                                                        ref tempItemsTableFields,
                                                        ref selectTicketsFields,
                                                        ref selectWikisFields,
                                                        ref selectNotesFields,
                                                        ref selectProductVersionsFields,
                                                        ref selectTasksFields,
                                                        ref selectWaterCoolerFields);
        }

        private string GetFieldsList()
        {
            return @"
        ti.ID
        , ti.source
        , ti.relevance
        , tv.TicketID
        , tv.TicketNumber
        , tv.Name
        , tv.Status
        , tv.Severity
        , tv.IsKnowledgeBase
        , wv.ArticleID
        , wv.ArticleName
        , wv.Creator
        , wv.Modifier
        , nv.NoteID
        , nv.RefID
        , nv.OrganizationName
        , nv.Title
        , nv.CreatorName
        , nv.DateModified
        , nv.RefType AS NoteRefType
        , nv.ContactName
        , pvv.ProductVersionID
        , pvv.ProductID
        , pvv.ProductName
        , pvv.VersionNumber
        , pvv.VersionStatus
        , pvv.DateModified AS ProductVersionDateModified
        , tsk.TaskID
        , tsk.Name AS TaskName
        , tsk.IsComplete
        , tsk.DateCompleted
        , tsk.UserName
        , tsk.DueDate
        , tsk.TaskParentName
        , IIF(tsk.DueDate < GETUTCDATE() , 1 , 0) AS IsPastDue
        , wcv.MessageID
        , wcv.Message
        , wcv.UserName
        , wcv.LastModified AS WaterCoolerLastModified
        , wcv.RefType
        , wcv.AttachmentID
        , wcv.MessageParent
      ";
        }

        private void GetHasNonTableFilters(
          SearchStandardFilter searchStandardFilter,
          LoginUser loginUser,
          ref bool hasNonWikiFilters,
          ref bool hasNonNotesFilters,
          ref bool hasNonProductVersionsFilters,
          ref bool hasNonTasksFilters,
          ref bool hasNonWaterCoolerFilters)
        {
            Organizations organizations = new Organizations(loginUser);
            organizations.LoadByOrganizationID(loginUser.OrganizationID);
            if (!organizations.IsEmpty && organizations[0].ProductType == ProductType.Express)
            {
                hasNonWikiFilters = true;
            }

            ReportTableFields ticketsViewFields = new ReportTableFields(loginUser);
            ticketsViewFields.LoadByReportTableID(10);

            SearchCustomFilters filters = new SearchCustomFilters(loginUser);
            filters.LoadByUserID(loginUser.UserID);

            WikiArticlesViewItem wikiArticlesViewItem = null;
            NotesViewItem notesViewItem = null;
            ProductVersionsViewItem productVersionsViewItem = null;
            TasksViewItem tasksViewItem = null;
            WaterCoolerViewItem waterCoolerViewItem = null;

            bool isFirstIteration = true;

            foreach (SearchCustomFilter filter in filters)
            {
                ReportTableField field = ticketsViewFields.FindByReportTableFieldID(filter.FieldID);
                if (field == null)
                {
                    //Filter is not in the tickets view therefore it is a custom field filter.
                    //If it has a custom field filter then exclude all other objects as this apply to tickets only
                    hasNonWikiFilters = true;
                    hasNonNotesFilters = true;
                    hasNonProductVersionsFilters = true;
                    hasNonTasksFilters = true;
                    hasNonWaterCoolerFilters = true;
                }
                else
                {
                    string fieldName = field.FieldName;

                    if (!hasNonWikiFilters && searchStandardFilter.Wikis)
                    {
                        if (isFirstIteration)
                        {
                            WikiArticlesView wikiArticlesView = new WikiArticlesView(loginUser);
                            wikiArticlesViewItem = wikiArticlesView.AddNewWikiArticlesViewItem();
                        }

                        string wikiEquivalentFieldName = DataUtils.GetWikiEquivalentFieldName(fieldName);
                        if (!DataUtils.GetIsColumnInBaseCollection(wikiArticlesViewItem.Collection, wikiEquivalentFieldName))
                        {
                            hasNonWikiFilters = true;
                        }
                    }

                    if (!hasNonNotesFilters && searchStandardFilter.Notes)
                    {
                        if (isFirstIteration)
                        {
                            NotesView notesView = new NotesView(loginUser);
                            notesViewItem = notesView.AddNewNotesViewItem();
                        }

                        string notesEquivalentFieldName = DataUtils.GetNotesEquivalentFieldName(fieldName);
                        if (!DataUtils.GetIsColumnInBaseCollection(notesViewItem.Collection, notesEquivalentFieldName))
                        {
                            hasNonNotesFilters = true;
                        }
                    }

                    if (!hasNonProductVersionsFilters && searchStandardFilter.ProductVersions)
                    {
                        if (isFirstIteration)
                        {
                            ProductVersionsView productVersionsView = new ProductVersionsView(loginUser);
                            productVersionsViewItem = productVersionsView.AddNewProductVersionsViewItem();
                        }

                        string productVersionEquivalentFieldName = DataUtils.GetProductVersionsEquivalentFieldName(fieldName);
                        if (!DataUtils.GetIsColumnInBaseCollection(productVersionsViewItem.Collection, productVersionEquivalentFieldName))
                        {
                            hasNonProductVersionsFilters = true;
                        }
                    }

                    if (!hasNonTasksFilters && searchStandardFilter.Tasks)
                    {
                        if (isFirstIteration)
                        {
                            TasksView tasksView = new TasksView(loginUser);
                            tasksViewItem = tasksView.AddNewTasksViewItem();
                        }

                        string taskEquivalentFieldName = DataUtils.GetTasksEquivalentFieldName(fieldName);
                        if (!DataUtils.GetIsColumnInBaseCollection(tasksViewItem.Collection, taskEquivalentFieldName))
                        {
                            hasNonTasksFilters = true;
                        }
                    }

                    if (!hasNonWaterCoolerFilters && searchStandardFilter.WaterCooler)
                    {
                        if (isFirstIteration)
                        {
                            WaterCoolerView waterCoolerView = new WaterCoolerView(loginUser);
                            waterCoolerViewItem = waterCoolerView.AddNewWaterCoolerViewItem();
                        }

                        string waterCoolerEquivalentFieldName = DataUtils.GetWaterCoolerEquivalentFieldName(fieldName);
                        if (!DataUtils.GetIsColumnInBaseCollection(waterCoolerViewItem.Collection, waterCoolerEquivalentFieldName))
                        {
                            hasNonWaterCoolerFilters = true;
                        }
                    }
                }
            }
        }

        [WebMethod]
        public int AddStandardFilters(
          bool includeTickets,
          bool includeKnowledgeBase,
          bool includeWikis,
          bool includeNotes,
          bool includeProductVersions,
          bool includeTasks,
          bool includeWaterCooler)
        {
            SearchStandardFilters filters = new SearchStandardFilters(TSAuthentication.GetLoginUser());
            SearchStandardFilter filter = filters.AddNewSearchStandardFilter();

            filter.UserID = filters.LoginUser.UserID;
            filter.Tickets = includeTickets;
            filter.KnowledgeBase = includeKnowledgeBase;
            filter.Wikis = includeWikis;
            filter.Notes = includeNotes;
            filter.ProductVersions = includeProductVersions;
            filter.Tasks = includeTasks;
            filter.WaterCooler = includeWaterCooler;

            filters.Save();

            return filter.StandardFilterID;
        }

        [WebMethod]
        public void UpdateStandardFilters(
          int standardFilterID,
          bool includeTickets,
          bool includeKnowledgeBase,
          bool includeWikis,
          bool includeNotes,
          bool includeProductVersions,
          bool includeTasks,
          bool includeWaterCooler)
        {
            SearchStandardFilters filters = new SearchStandardFilters(TSAuthentication.GetLoginUser());
            filters.LoadByStandardFilterID((int)standardFilterID);
            SearchStandardFilter filter = filters.FindByStandardFilterID((int)standardFilterID);

            filter.Tickets = includeTickets;
            filter.KnowledgeBase = includeKnowledgeBase;
            filter.Wikis = includeWikis;
            filter.Notes = includeNotes;
            filter.ProductVersions = includeProductVersions;
            filter.Tasks = includeTasks;
            filter.WaterCooler = includeWaterCooler;
            filters.Save();
        }

        [WebMethod]
        public int AddCustomFilter(int fieldID, string measure, string value, bool isCustom)
        {
            SearchCustomFilters filters = new SearchCustomFilters(TSAuthentication.GetLoginUser());
            SearchCustomFilter filter = filters.AddNewSearchCustomFilter();

            filter.TableID = isCustom ? -1 : 10;
            filter.FieldID = fieldID;
            filter.Measure = measure;
            filter.TestValue = value;
            filter.UserID = filters.LoginUser.UserID;
            filter.MatchAll = true;

            filters.Save();

            return (int)filters.Table.Rows[0]["CustomFilterID"];
        }

        [WebMethod]
        public void UpdateCustomFilter(int customFilterID, int fieldID, string measure, string value, bool isCustom)
        {
            SearchCustomFilters filters = new SearchCustomFilters(TSAuthentication.GetLoginUser());
            filters.LoadByCustomFilterID(customFilterID);
            SearchCustomFilter filter = filters.FindByCustomFilterID(customFilterID);

            filter.TableID = isCustom ? -1 : 10;
            filter.FieldID = fieldID;
            filter.Measure = measure;
            filter.TestValue = value;

            filters.Save();
        }

        [WebMethod]
        public void DeleteCustomFilter(int filterID)
        {
            SearchCustomFilters filters = new SearchCustomFilters(TSAuthentication.GetLoginUser());
            filters.DeleteFromDB(filterID);
        }

        [WebMethod]
        public int AddSorter(int fieldID, bool descending, bool isCustom)
        {
            SearchSorters sorters = new SearchSorters(TSAuthentication.GetLoginUser());
            SearchSorter sorter = sorters.AddNewSearchSorter();

            sorter.TableID = isCustom ? -1 : 10;
            sorter.FieldID = fieldID;
            sorter.Descending = descending;
            sorter.UserID = sorters.LoginUser.UserID;

            sorters.Save();

            return (int)sorters.Table.Rows[0]["SorterID"];
        }

        [WebMethod]
        public void UpdateSorter(int sorterID, int fieldID, bool descending, bool isCustom)
        {
            SearchSorters sorters = new SearchSorters(TSAuthentication.GetLoginUser());
            sorters.LoadBySorterID(sorterID);
            SearchSorter sorter = sorters.FindBySorterID(sorterID);

            sorter.TableID = isCustom ? -1 : 10;
            sorter.FieldID = fieldID;
            sorter.Descending = descending;

            sorters.Save();
        }

        [WebMethod]
        public void DeleteSorter(int sorterID)
        {
            SearchSorters sorters = new SearchSorters(TSAuthentication.GetLoginUser());
            sorters.DeleteFromDB(sorterID);
        }

        [WebMethod]
        public AdvancedSearchOptions GetAdvancedSearchOptions()
        {
            AdvancedSearchOptions result = new AdvancedSearchOptions();

            LoginUser loginUser = TSAuthentication.GetLoginUser();

            // GetStandardFilters
            SearchStandardFilters standardFilters = new SearchStandardFilters(loginUser);
            standardFilters.LoadByUserID(loginUser.UserID);
            if (standardFilters.Table.Rows.Count > 0)
            {
                result.StandardFilterID = (int)standardFilters.Table.Rows[0]["StandardFilterID"];
                result.Tickets = (bool)standardFilters.Table.Rows[0]["Tickets"];
                result.KnowledgeBase = (bool)standardFilters.Table.Rows[0]["KnowledgeBase"];
                result.Wikis = (bool)standardFilters.Table.Rows[0]["Wikis"];
                result.Notes = (bool)standardFilters.Table.Rows[0]["Notes"];
                result.ProductVersions = (bool)standardFilters.Table.Rows[0]["ProductVersions"];
                result.Tasks = (bool)standardFilters.Table.Rows[0]["Tasks"];
                result.WaterCooler = (bool)standardFilters.Table.Rows[0]["WaterCooler"];

            }

            // GetFields
            List<AutoFieldItem> fieldItems = new List<AutoFieldItem>();
            ReportTableFields fields = new ReportTableFields(loginUser);
            fields.LoadByReportTableID(10);

            CustomFields customs = new CustomFields(fields.LoginUser);
            customs.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets);

            foreach (ReportTableField field in fields)
            {
                fieldItems.Add(new AutoFieldItem(field));
            }

            TicketTypes ticketTypes = new TicketTypes(fields.LoginUser);
            ticketTypes.LoadAllPositions(TSAuthentication.OrganizationID);

            foreach (CustomField custom in customs)
            {
                TicketType ticketType = ticketTypes.FindByTicketTypeID(custom.AuxID);
                if (ticketType == null)
                {
                    fieldItems.Add(new AutoFieldItem(custom));
                }
                else
                {
                    fieldItems.Add(new AutoFieldItem(custom, string.Format("{0} ({1})", custom.Name, ticketType.Name)));
                }
            }

            result.Fields = fieldItems.ToArray();

            // GetCustomFilters
            SearchCustomFilters customFilters = new SearchCustomFilters(loginUser);
            customFilters.LoadByUserID(loginUser.UserID);
            result.CustomFilters = customFilters.GetSearchCustomFilterProxies();

            // GetCustomSorters
            SearchSorters sorters = new SearchSorters(loginUser);
            sorters.LoadByUserID(loginUser.UserID);
            result.Sorters = sorters.GetSearchSorterProxies();

            return result;
        }

        [WebMethod]
        public string[] CustomerSearchTest(string searchTerm, int organizationID)
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            LoginUser loginUser = new LoginUser(TSAuthentication.GetLoginUser().ConnectionString, TSAuthentication.UserID, organizationID, null);
            SearchResults results = GetCustomerSearchResults(loginUser, searchTerm, true, true, 50, null);
            return GetSearchReport(results);
        }

        public string[] GetSearchReport(SearchResults results)
        {
            SearchReportJob report = results.NewSearchReportJob();
            report.SelectAll();
            report.Flags = ReportFlags.dtsReportStoreInResults | ReportFlags.dtsReportWholeFile | ReportFlags.dtsReportGetFromCache | ReportFlags.dtsReportIncludeAll;
            report.OutputFormat = OutputFormats.itUnformattedHTML;
            report.MaxContextBlocks = 3;
            report.MaxWordsToRead = 50000;
            report.OutputStringMaxSize = 50000;
            //report.WordsOfContext = 10;
            report.BeforeHit = "123456789";// "<strong style=\"color:red;\">";
            report.AfterHit = "987654321"; //"</strong>";
            report.OutputToString = true;
            report.Execute();
            report.ContextSeparator = "<br/><br/>";
            List<string> result = new List<string>();


            for (int i = 0; i < results.Count; i++)
            {
                result.Add(GetSearchItemReport(results, i));
            }
            return result.ToArray();
        }

        public string GetSearchItemReport(SearchResults results, int index)
        {
            results.GetNthDoc(index);

            using (FileConverter fc = new FileConverter())
            {

                // This sets up FileConverter with the input file, index location, and hits
                fc.SetInputItem(results, index);

                fc.OutputToString = true;
                fc.OutputStringMaxSize = 2000000;
                fc.OutputFormat = OutputFormats.itHTML;

                fc.Header = "<A NAME=hit0></A>"
                  + "<TABLE border=0>"
                  + "<TR><TD bgcolor=#003399 color=#ffffff>"
                  + "<font face=verdana color=#ffffff size=2></TD></TR>"
                  + "<TR><TD bgcolor=#eeeeee color=#000000>"
                  + "<font face=verdana color=#003399 size=2>"
                  + "<B>" + results.get_DocDetailItem("_shortName") + "   </B><BR>"
                  + "<font face=verdana color=#000000 size=1>"
                  + results.DocHitCount + " Hits."
                  + "  <B>Location: </B>  <B>Date: </B>"
                  + results.get_DocDetailItem("_date") + "<BR>"
                  + "</FONT>"
                  + "</TD></TR><TR><TD bgcolor=#003399 color=#ffffff>"
                  + "<font face=verdana color=#ffffff size=2></TD></TR></TABLE>";


                fc.BeforeHit = "<span style=\"background-color: #FF0000\">";
                fc.AfterHit = "</span>";
                fc.Flags = (fc.Flags | ConvertFlags.dtsConvertGetFromCache);
                fc.Execute();
                return fc.OutputString;
            }
        }

        public SearchResults GetCustomerSearchResults(LoginUser loginUser, string searchTerm, bool searchCompanies, bool searchContacts, int max, bool? active, bool? parentsOnly = false)
        {
            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;
            using (SearchJob job = new SearchJob())
            {

                job.Request = searchTerm;
                job.FieldWeights = "Name:20,Email:10";
                job.MaxFilesToRetrieve = max;
                job.AutoStopLimit = 10000000;
                job.TimeoutSeconds = 30;
                job.Fuzziness = 0;

                job.Request = Regex.Replace(job.Request, "[,.]+", "");
                if (job.Request.IndexOf('"') < 0)
                {
                    job.Request = job.Request + "*";
                }

                User user = Users.GetUser(loginUser, loginUser.UserID);
                if (user.TicketRights == TicketRightType.Customers)
                {
                    StringBuilder conditions = new StringBuilder();
                    Organizations orgs = new Organizations(loginUser);
                    orgs.LoadByUserRights(user.UserID);

                    for (int i = 0; i < orgs.Count; i++)
                    {
                        if (i > 0) conditions.Append(" OR ");
                        conditions.Append("(OrganizationID::" + orgs[i].OrganizationID.ToString() + ") ");
                    }

                    job.BooleanConditions = conditions.ToString();
                }

                if (active != null)
                {
                    StringBuilder activeCondition = new StringBuilder();
                    if ((bool)active)
                    {
                        activeCondition.Append("(IsActive::true) ");
                    }
                    else
                    {
                        activeCondition.Append("(IsActive::false) ");
                    }

                    if (job.BooleanConditions != null)
                    {
                        job.BooleanConditions = job.BooleanConditions + " AND " + activeCondition.ToString();
                    }
                    else
                    {
                        job.BooleanConditions = activeCondition.ToString();
                    }
                }

                if (parentsOnly != null)
                {
                    StringBuilder parentsOnlyCondition = new StringBuilder();
                    if ((bool)parentsOnly)
                    {
                        parentsOnlyCondition.Append("(IsParent::true) ");

                        if (job.BooleanConditions != null)
                        {
                            job.BooleanConditions = job.BooleanConditions + " AND " + parentsOnlyCondition.ToString();
                        }
                        else
                        {
                            job.BooleanConditions = parentsOnlyCondition.ToString();
                        }
                    }
                }

                job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchDelayDocInfo;

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;

                string companiesIndexPath = DataUtils.GetCompaniesIndexPath(loginUser);

                if (searchCompanies)
                {
                    job.IndexesToSearch.Add(companiesIndexPath);
                }

                string contactsIndexPath = DataUtils.GetContactsIndexPath(loginUser);
                if (searchContacts)
                {
                    job.IndexesToSearch.Add(contactsIndexPath);
                }
                job.Execute();
                job.Results.Sort(SortFlags.dtsSortByRelevanceScore | SortFlags.dtsSortDescending, "");
                return job.Results;
            }
        }

        public SearchResults GetAssetsSearchResults(LoginUser loginUser, string searchTerm, bool searchAssigned, bool searchWarehouse, bool searchJunkyard, int max)
        {
            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;
            using (SearchJob job = new SearchJob())
            {

                job.Request = searchTerm;
                job.FieldWeights = "Name:20,SerialNumber:10";
                job.MaxFilesToRetrieve = max;
                job.AutoStopLimit = 10000000;
                job.TimeoutSeconds = 30;

                job.Fuzziness = 0;
                job.Request = job.Request + "*";

                if (!searchAssigned || !searchWarehouse || !searchJunkyard)
                {
                    StringBuilder conditions = new StringBuilder();
                    if (searchAssigned)
                    {
                        conditions.Append("(Location::Assigned) ");
                        if (searchWarehouse) conditions.Append("OR (Location::Warehouse) ");
                        if (searchJunkyard) conditions.Append("OR (Location::Junkyard) ");
                    }
                    else if (searchWarehouse)
                    {
                        conditions.Append("(Location::Warehouse) ");
                        if (searchJunkyard) conditions.Append("OR (Location::Junkyard) ");
                    }
                    else if (searchJunkyard)
                    {
                        conditions.Append("(Location::Junkyard) ");
                    }
                    job.BooleanConditions = conditions.ToString();
                }

                job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchDelayDocInfo;

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;

                job.IndexesToSearch.Add(DataUtils.GetAssetsIndexPath(loginUser));
                job.Execute();
                job.Results.Sort(SortFlags.dtsSortByRelevanceScore | SortFlags.dtsSortDescending, "");
                return job.Results;
            }
        }

        public SearchResults GetProductsSearchResults(LoginUser loginUser, string searchTerm, int max, bool searchProducts, bool searchProductVersions)
        {
            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;
            using (SearchJob job = new SearchJob())
            {

                job.Request = searchTerm;
                job.FieldWeights = "Name:20,Description:10";
                job.MaxFilesToRetrieve = max;
                job.AutoStopLimit = 10000000;
                job.TimeoutSeconds = 30;

                job.Fuzziness = 0;
                job.Request = job.Request + "*";

                job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchDelayDocInfo;

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;

                if (searchProducts)
                {
                    job.IndexesToSearch.Add(DataUtils.GetProductsIndexPath(loginUser));
                }

                if (searchProductVersions)
                {
                    job.IndexesToSearch.Add(DataUtils.GetProductVersionsIndexPath(loginUser));
                }

                User user = Users.GetUser(loginUser, loginUser.UserID);
                if ((ProductFamiliesRightType)user.ProductFamiliesRights != ProductFamiliesRightType.AllFamilies)
                {
                    Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
                    if (organization.UseProductFamilies)
                    {
                        ProductFamilies userRightsProductFamilies = new ProductFamilies(loginUser);
                        userRightsProductFamilies.LoadByUserRights(loginUser.UserID);
                        if (userRightsProductFamilies.Count > 0)
                        {
                            StringBuilder conditions = new StringBuilder();
                            //conditions.Append(" (");
                            //conditions.Append(" (OrganizationID::" + user.OrganizationID.ToString() + ") ");
                            for (int i = 0; i < userRightsProductFamilies.Count; i++)
                            {
                                if (i > 0)
                                {
                                    conditions.Append("OR ");
                                }
                                conditions.Append("(ProductFamilyID::" + userRightsProductFamilies[i].ProductFamilyID.ToString() + ") ");
                            }
                            //conditions.Append(") ");
                            job.BooleanConditions = conditions.ToString();
                        }
                        else
                        {
                            return job.Results;
                        }
                    }
                }


                job.Execute();
                if (searchTerm == "xfirstword")
                {
                    if (searchProducts)
                    {
                        job.Results.Sort(SortFlags.dtsSortByField | SortFlags.dtsSortAscending, "Name");
                    }
                    else
                    {
                        job.Results.Sort(SortFlags.dtsSortByName | SortFlags.dtsSortDescending, "");
                    }
                }
                else
                {
                    job.Results.Sort(SortFlags.dtsSortByRelevanceScore | SortFlags.dtsSortDescending, "");
                }
                return job.Results;
            }
        }

        [WebMethod]
        public string[] SearchCompaniesAndContacts(string searchTerm, int from, int count, bool searchCompanies, bool searchContacts, bool? active)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> resultItems = new List<string>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                //return GetAllCompaniesAndContacts(from, count, searchCompanies, searchContacts, active);
                return sendResults(GetSearchForAll(from, count, "ContactCustomerList", active, false));
            }

            if (searchCompanies || searchContacts)
            {
                Stopwatch stopWatch = Stopwatch.StartNew();
                SearchResults results = GetCustomerSearchResults(loginUser, searchTerm, searchCompanies, searchContacts, 0, active);
                stopWatch.Stop();
                NewRelic.Api.Agent.NewRelic.RecordMetric("Custom/SearchCompaniesAndContacts", stopWatch.ElapsedMilliseconds);
                //Only record the custom parameter in NR if the search took longer than 3 seconds (I'm using this arbitrarily, seems appropiate)
                if (stopWatch.ElapsedMilliseconds > 500)
                {
                    NewRelic.Api.Agent.NewRelic.AddCustomParameter("SearchCompaniesAndContacts-OrgId", TSAuthentication.GetOrganization(loginUser).OrganizationID);
                    NewRelic.Api.Agent.NewRelic.AddCustomParameter("SearchCompaniesAndContacts-Term", searchTerm);
                }

                int topLimit = from + count;
                if (topLimit > results.Count)
                {
                    topLimit = results.Count;
                }

                stopWatch.Restart();
                for (int i = from; i < topLimit; i++)
                {
                    results.GetNthDoc(i);
                    if (results.CurrentItem.UserFields != null && results.CurrentItem.UserFields["JSON"] != null)
                        resultItems.Add(results.CurrentItem.UserFields["JSON"].ToString());
                }
                stopWatch.Stop();
                NewRelic.Api.Agent.NewRelic.RecordMetric("Custom/SearchCompaniesAndContactsPullData", stopWatch.ElapsedMilliseconds);
            }

            return resultItems.ToArray();
        }


        [WebMethod]
        public string[] SearchCompaniesAndContactsDt(string searchTerm, int from, int count, bool searchCompanies, bool searchContacts, bool? active, bool? parentsOnly)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> resultItems = new List<string>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllCompaniesAndContactsDt(from, count, searchCompanies, searchContacts, active, parentsOnly);
            }

            if (searchCompanies || searchContacts)
            {
                Stopwatch stopWatch = Stopwatch.StartNew();
                SearchResults results = GetCustomerSearchResults(loginUser, searchTerm, searchCompanies, searchContacts, 0, active, parentsOnly);
                stopWatch.Stop();
                NewRelic.Api.Agent.NewRelic.RecordMetric("Custom/SearchCompaniesAndContacts", stopWatch.ElapsedMilliseconds);
                //Only record the custom parameter in NR if the search took longer than 3 seconds (I'm using this arbitrarily, seems appropiate)
                if (stopWatch.ElapsedMilliseconds > 500)
                {
                    NewRelic.Api.Agent.NewRelic.AddCustomParameter("SearchCompaniesAndContacts-OrgId", TSAuthentication.GetOrganization(loginUser).OrganizationID);
                    NewRelic.Api.Agent.NewRelic.AddCustomParameter("SearchCompaniesAndContacts-Term", searchTerm);
                }

                int topLimit = from + count;
                if (topLimit > results.Count)
                {
                    topLimit = results.Count;
                }

                stopWatch.Restart();
                for (int i = from; i < topLimit; i++)
                {
                    results.GetNthDoc(i);
                    if (results.CurrentItem.UserFields != null && results.CurrentItem.UserFields["JSON"] != null)
                        resultItems.Add(results.CurrentItem.UserFields["JSON"].ToString());
                }
                stopWatch.Stop();
                NewRelic.Api.Agent.NewRelic.RecordMetric("Custom/SearchCompaniesAndContactsPullData", stopWatch.ElapsedMilliseconds);
            }

            return resultItems.ToArray();
        }

        private string[] GetAllCompaniesAndContactsDt(int from, int count, bool searchCompanies, bool searchContacts, bool? active, bool? parentsOnly = false)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> results = new List<string>();
            SqlCommand command = new SqlCommand();

            string pageQuery = @"
WITH 
q AS ({0}),
r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY 
    CASE 
        WHEN [NAME] IS NULL THEN 1 
        WHEN [NAME] = ''    THEN 2 
        ELSE 3 
    END DESC, 
    [NAME] ASC) AS 'RowNum' FROM q)
SELECT * INTO #X FROM r
WHERE RowNum BETWEEN @From AND @To

SELECT 
	o.Name AS Organization, 
	o.OrganizationID, 
	u.UserID, 
	o.Website, 
	u.IsPortalUser, 
	o.HasPortalAccess, 
	u.FirstName, 
	u.LastName, 
	u.Email, 
	u.Title,
	(SELECT COUNT(*) FROM TicketsView t LEFT JOIN OrganizationTickets ot ON ot.TicketID = t.TicketID WHERE ot.OrganizationID = o.OrganizationID AND t.IsClosed = 0) AS OrgOpenTickets,
	(SELECT COUNT(*) FROM TicketsView t LEFT JOIN UserTickets ut ON ut.TicketID = t.TicketID WHERE ut.UserID = u.UserID AND t.IsClosed = 0) AS ContactOpenTickets
FROM #X AS x
LEFT JOIN Organizations o ON o.OrganizationID = x.OrganizationID
LEFT JOIN Users u ON u.UserID = x.UserID";

            string companyQuery = @"
SELECT 
  LTRIM(o.Name) AS Name, 
  o.OrganizationID,
  NULL AS UserID
  FROM Organizations o WHERE o.ParentID = @OrganizationID
";

            if (Convert.ToBoolean(parentsOnly))
            {
                companyQuery += " AND (1 < (SELECT COUNT(*) FROM GetCompanyFamilyIDs(o.OrganizationID, 1))) ";
            }

            string contactQuery = @"
SELECT 
  LTRIM(u.LastName + ' ' + u.FirstName) AS Name, 
  u.OrganizationID,
  u.UserID
  FROM Users u
  LEFT JOIN Organizations o ON u.OrganizationID = o.OrganizationID
  WHERE o.ParentID = @OrganizationID AND u.MarkDeleted=0
";
            User user = Users.GetUser(loginUser, loginUser.UserID);
            if (user.TicketRights == TicketRightType.Customers)
            {
                companyQuery = companyQuery + " AND o.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
                contactQuery = contactQuery + " AND u.OrganizationID IN (SELECT OrganizationID FROM UserRightsOrganizations WHERE UserID = " + user.UserID.ToString() + ")";
            }

            if (active != null)
            {
                companyQuery = companyQuery + " AND o.IsActive = @IsActive";
                contactQuery = contactQuery + " AND u.IsActive = @IsActive";
                command.Parameters.AddWithValue("@IsActive", (bool)active);
            }

            if (searchContacts && searchCompanies)
            {
                command.CommandText = string.Format(pageQuery, companyQuery + " UNION ALL " + contactQuery);
            }
            else if (searchCompanies)
            {
                command.CommandText = string.Format(pageQuery, companyQuery);
            }
            else if (searchContacts)
            {
                command.CommandText = string.Format(pageQuery, contactQuery);
            }
            else
            {
                return results.ToArray();
            }


            command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
            command.Parameters.AddWithValue("@From", from + 1);
            command.Parameters.AddWithValue("@To", from + count);

            DataTable table = SqlExecutor.ExecuteQuery(loginUser, command);

            foreach (DataRow row in table.Rows)
            {
                if (row["UserID"] == DBNull.Value)
                {
                    CustomerSearchCompany company = new CustomerSearchCompany();
                    company.name = (string)row["Organization"];
                    company.organizationID = (int)row["OrganizationID"];
                    company.isPortal = (bool)row["HasPortalAccess"];
                    company.openTicketCount = (int)row["OrgOpenTickets"];
                    company.website = GetDBString(row["Website"]);

                    List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
                    PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
                    //phoneNumbers.LoadByID(company.organizationID, ReferenceType.Organizations);
                    foreach (PhoneNumber number in phoneNumbers)
                    {
                        phones.Add(new CustomerSearchPhone(number));
                    }
                    company.phones = phones.ToArray();

                    results.Add(JsonConvert.SerializeObject(company));
                }
                else
                {
                    CustomerSearchContact contact = new CustomerSearchContact();
                    contact.organizationID = (int)row["OrganizationID"];
                    contact.isPortal = (bool)row["IsPortalUser"];
                    contact.openTicketCount = (int)row["ContactOpenTickets"];

                    contact.userID = (int)row["UserID"];
                    contact.fName = GetDBString(row["FirstName"]);
                    contact.lName = GetDBString(row["LastName"]);
                    contact.email = GetDBString(row["Email"]);
                    contact.title = GetDBString(row["Title"]);
                    contact.organization = GetDBString(row["Organization"]);

                    List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
                    PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
                    //phoneNumbers.LoadByID(contact.userID, ReferenceType.Contacts);
                    foreach (PhoneNumber number in phoneNumbers)
                    {
                        phones.Add(new CustomerSearchPhone(number));
                    }
                    contact.phones = phones.ToArray();
                    results.Add(JsonConvert.SerializeObject(contact));
                }

            }

            return results.ToArray();

        }


        [WebMethod]
        public string[] SearchCompaniesAndContacts2(string searchTerm, int from, int count, bool searchCompanies, bool searchContacts, bool? active, bool? parentsOnly)
        {
            searchTerm = AntiXss.HtmlEncode(searchTerm);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string[] resultItems = new string[] { };

            Stopwatch stopWatch = Stopwatch.StartNew();
            if (!String.IsNullOrEmpty(searchTerm))
                resultItems = sendResults(GetAllCompaniesAndContactsWithTerm(searchTerm, from, count, searchCompanies, searchContacts, active, parentsOnly));
            else
            {
                if (searchCompanies && !searchContacts)
                    resultItems = sendResults(GetSearchForAll(from, count, "CustomerList", active, parentsOnly));
                if (searchContacts && !searchCompanies)
                    resultItems = sendResults(GetSearchForAll(from, count, "ContactList", active, parentsOnly));
                if (searchContacts && searchCompanies)
                    resultItems = sendResults(GetSearchForAll(from, count, "ContactCustomerList", active, parentsOnly));
            }
            stopWatch.Stop();

            NewRelic.Api.Agent.NewRelic.RecordMetric("Custom/SearchCompaniesAndContacts", stopWatch.ElapsedMilliseconds);
            //Only record the custom parameter in NR if the search took longer than 3 seconds (I'm using this arbitrarily, seems appropiate)
            if (stopWatch.ElapsedMilliseconds > 500)
            {
                NewRelic.Api.Agent.NewRelic.AddCustomParameter("SearchCompaniesAndContacts-OrgId", TSAuthentication.GetOrganization(loginUser).OrganizationID);
                NewRelic.Api.Agent.NewRelic.AddCustomParameter("SearchCompaniesAndContacts-Term", searchTerm);
            }

            return resultItems;
        }

        private DataTable GetAllCompaniesAndContactsWithTerm(string searchTerm, int from, int count, bool searchCompanies, bool searchContacts, bool? active, bool? parentsOnly = false)
        {

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> results = new List<string>();

            //Clean searchterm
            searchTerm = searchTerm.Replace("\"", "").Replace("'", "");
            if (!String.IsNullOrEmpty(searchTerm))
                searchTerm = "\"" + searchTerm + "\"";

            SqlCommand command = new SqlCommand();

            command.CommandText = "CustomerSearch_New";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FromIndex", from + 1);
            command.Parameters.AddWithValue("@ToIndex", from + count);
            command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", loginUser.UserID);
            command.Parameters.AddWithValue("@SearchTerm", searchTerm);
            command.Parameters.AddWithValue("@IncludeCompanies", searchCompanies);
            command.Parameters.AddWithValue("@IncludeContacts", searchContacts);
            command.Parameters.AddWithValue("@ActiveOnly", active);
            command.Parameters.AddWithValue("@ParentsOnly", parentsOnly);


            return SqlExecutor.ExecuteQuery(loginUser, command);


        }
        private DataTable GetSearchForAll(int from, int count, string sprocName, bool? active, bool? parentsOnly = false)
        {

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> results = new List<string>();

            //Clean searchterm
            SqlCommand command = new SqlCommand();

            command.CommandText = sprocName;
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FromIndex", from + 1);
            command.Parameters.AddWithValue("@ToIndex", from + count);
            command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
            command.Parameters.AddWithValue("@UserID", loginUser.UserID);
            command.Parameters.AddWithValue("@ActiveOnly", active);
            command.Parameters.AddWithValue("@ParentsOnly", parentsOnly);


            return SqlExecutor.ExecuteQuery(loginUser, command);

        }

        private string[] sendResults(DataTable table)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> results = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                if (row["UserID"] == DBNull.Value)
                {
                    CustomerSearchCompany company = new CustomerSearchCompany();
                    company.name = (string)row["Organization"];
                    company.organizationID = (int)row["OrganizationID"];
                    company.isPortal = (bool)row["HasPortalAccess"];
                    company.openTicketCount = 0;// (int)row["OrgOpenTickets"];
                    company.website = GetDBString(row["Website"]);

                    List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
                    PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
                    foreach (PhoneNumber number in phoneNumbers)
                    {
                        phones.Add(new CustomerSearchPhone(number));
                    }
                    company.phones = phones.ToArray();

                    results.Add(JsonConvert.SerializeObject(company));
                }
                else
                {
                    CustomerSearchContact contact = new CustomerSearchContact();
                    contact.organizationID = (int)row["OrganizationID"];
                    contact.isPortal = (bool)row["IsPortalUser"];
                    contact.openTicketCount = 0;// (int)row["ContactOpenTickets"];

                    contact.userID = (int)row["UserID"];
                    contact.fName = GetDBString(row["FirstName"]);
                    contact.lName = GetDBString(row["LastName"]);
                    contact.email = GetDBString(row["Email"]);
                    contact.title = GetDBString(row["Title"]);
                    contact.organization = GetDBString(row["Organization"]);

                    List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
                    PhoneNumbers phoneNumbers = new PhoneNumbers(loginUser);
                    foreach (PhoneNumber number in phoneNumbers)
                    {
                        phones.Add(new CustomerSearchPhone(number));
                    }
                    contact.phones = phones.ToArray();
                    results.Add(JsonConvert.SerializeObject(contact));
                }
            }
            return results.ToArray();
        }


        public static string GetDBString(object o)
        {
            if (o == null || o == DBNull.Value) return "";
            return o.ToString();
        }

        [WebMethod]
        public string[] SearchAssetsOLD(string searchTerm, int from, int count, bool searchAssigned, bool searchWarehouse, bool searchJunkyard)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> resultItems = new List<string>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                //return GetAllAssets(from, count, searchAssigned, searchWarehouse, searchJunkyard);
                searchTerm = "xfirstword";
            }

            //if (searchCompanies || searchContacts)
            //{
            SearchResults results = GetAssetsSearchResults(loginUser, searchTerm, searchAssigned, searchWarehouse, searchJunkyard, 0);
            int topLimit = from + count;
            if (topLimit > results.Count)
            {
                topLimit = results.Count;
            }

            for (int i = from; i < topLimit; i++)
            {
                results.GetNthDoc(i);
                if (results.CurrentItem.UserFields["JSON"] != null)
                    resultItems.Add(results.CurrentItem.UserFields["JSON"].ToString());
            }

            //}

            return resultItems.ToArray();
        }

        [WebMethod]
        public string[] SearchAssets(string searchTerm, int from, int count, bool searchAssigned, bool searchWarehouse, bool searchJunkyard)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> results = new List<string>();
            SqlCommand command = new SqlCommand();

            //Clean searchterm
            searchTerm = searchTerm.Replace("\"", "").Replace("'", "");
            if (!String.IsNullOrEmpty(searchTerm))
                searchTerm = "\"" + searchTerm + "\"";


            command.CommandText = "AssetsSearch";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FromIndex", from + 1);
            command.Parameters.AddWithValue("@ToIndex", from + count);
            command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
            command.Parameters.AddWithValue("@SearchTerm", searchTerm);
            command.Parameters.AddWithValue("@IncludeAssigned", searchAssigned);
            command.Parameters.AddWithValue("@IncludeWarehouse", searchWarehouse);
            command.Parameters.AddWithValue("@IncludeJunkyard", searchJunkyard);


            DataTable table = SqlExecutor.ExecuteQuery(loginUser, command);
            foreach (DataRow row in table.Rows)
            {
                dynamic assetSearch = new ExpandoObject();
                assetSearch.assetID = row["assetID"];
                assetSearch.serialNumber = row["serialNumber"];
                assetSearch.notes = row["notes"];
                assetSearch.name = row["name"];
                assetSearch.location = row["location"];
                assetSearch.warrantyExpiration = row["warrantyExpiration"];
                assetSearch.productName = row["productName"];
                assetSearch.productVersionNumber = row["VersionNumber"];
                results.Add(JsonConvert.SerializeObject(assetSearch));
            }
            return results.ToArray();

        }

        //[WebMethod]
        public string[] SearchAssets_OLD(string searchTerm, int from, int count, bool searchAssigned, bool searchWarehouse, bool searchJunkyard)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<string> results = new List<string>();
            SqlCommand command = new SqlCommand();

            string pageQuery = @"
WITH
q AS ({0}),
r AS (SELECT q.*, ROW_NUMBER() OVER (ORDER BY
    CASE
        WHEN [NAME] IS NULL THEN 1
        WHEN [NAME] = ''    THEN 2
        ELSE 3
    END DESC,
    [NAME] ASC) AS 'RowNum' FROM q)
SELECT * INTO #X FROM r
WHERE RowNum BETWEEN @From AND @To

select  a.AssetID,
        a.OrganizationID,
        a.SerialNumber,
        a.notes,
        a.Name,
        a.Location,
        a.WarrantyExpiration,
        p.Name as productName,
        pv.VersionNumber
FROM #X AS x
LEFT JOIN Assets a on a.AssetID = x.AssetID
LEFT JOIN Products p ON p.ProductID = x.ProductID
Left JOIN ProductVersions pv on pv.ProductVersionID = x.ProductVersionID";

            string assetQuery = @"
SELECT
  a.Name as Name,
  a.AssetID,
  a.ProductID,
  a.ProductVersionID
  FROM Assets a WHERE a.OrganizationID = @OrganizationID
";
            if (!string.IsNullOrEmpty(searchTerm))
            {
                assetQuery += " and (CONTAINS(Name,@SearchTerm) or CONTAINS(a.notes,@SearchTerm) or CONTAINS(a.serialnumber,@SearchTerm) ) ";
                command.Parameters.AddWithValue("@SearchTerm", string.Format("\"{0}*\"", searchTerm));
            }

            if (!searchAssigned || !searchWarehouse || !searchJunkyard)
            {
                if (searchAssigned)
                {
                    assetQuery += " and (Location=1)";
                    if (searchWarehouse) assetQuery += " OR (Location=2) ";
                    if (searchJunkyard) assetQuery += " OR (Location=3) ";
                }
                else if (searchWarehouse)
                {
                    assetQuery += " and (Location=2) ";
                    if (searchJunkyard) assetQuery += "  or (Location=3) ";
                }
                else if (searchJunkyard)
                {
                    assetQuery += " and (Location=3) ";
                }
            }

            command.CommandText = string.Format(pageQuery, assetQuery);
            command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
            command.Parameters.AddWithValue("@From", from + 1);
            command.Parameters.AddWithValue("@To", from + count);

            DataTable table = SqlExecutor.ExecuteQuery(loginUser, command);

            foreach (DataRow row in table.Rows)
            {
                dynamic assetSearch = new ExpandoObject();
                assetSearch.assetID = row["assetID"];
                assetSearch.serialNumber = row["serialNumber"];
                assetSearch.notes = row["notes"];
                assetSearch.name = row["name"];
                assetSearch.location = row["location"];
                assetSearch.warrantyExpiration = row["warrantyExpiration"];
                assetSearch.productName = row["productName"];
                assetSearch.productVersionNumber = row["VersionNumber"];
                results.Add(JsonConvert.SerializeObject(assetSearch));
            }
            return results.ToArray();
        }

        [WebMethod]
        public string[] SearchProducts(string searchTerm, int from, int count, bool searchProducts, bool searchProductVersions)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            try
            {
                List<string> resultItems = new List<string>();
                string engine = "FTS";
                switch (engine)
                {
                    //https://dtsearch.com/
                    case "DTS":
                        if (string.IsNullOrWhiteSpace(searchTerm))
                        {
                            searchTerm = "xfirstword";
                        }

                        SearchResults results = GetProductsSearchResults(loginUser, searchTerm, 0, searchProducts, searchProductVersions);
                        int topLimit = from + count;
                        if (topLimit > results.Count)
                        {
                            topLimit = results.Count;
                        }

                        for (int i = from; i < topLimit; i++)
                        {
                            results.GetNthDoc(i);
                            if (results.CurrentItem.UserFields["JSON"] != null)
                                resultItems.Add(results.CurrentItem.UserFields["JSON"].ToString());
                        }
                        break;
                    //https://docs.microsoft.com/en-us/sql/relational-databases/search/full-text-search
                    case "FTS":
                        StringBuilder productsQuery = new StringBuilder();
                        StringBuilder versionsQuery = new StringBuilder();
                        SqlCommand command = new SqlCommand();

                        if (searchProducts)
                        {
                            List<SqlDataRecord> tfsProductsResultsList = Products.GetSearchResultsList(searchTerm, loginUser);

                            if (tfsProductsResultsList.Count > 0)
                            {
                                productsQuery.Append(@"
                                    SELECT
                                        p.ProductID
                                        , 13
                                        , fprt.relevance
                                        , p.DateModified
                                    FROM
                                        dbo.Products p
                                        JOIN @ftsProductsResultsTable fprt
                                            ON p.ProductID = fprt.recordID
                                ");

                                SqlParameter ftsProductsResultsTable = new SqlParameter("@ftsProductsResultsTable", SqlDbType.Structured);
                                ftsProductsResultsTable.TypeName = "dtSearch_results_tbltype";
                                ftsProductsResultsTable.Value = tfsProductsResultsList;
                                command.Parameters.Add(ftsProductsResultsTable);
                            }
                        }

                        if (searchProductVersions)
                        {
                            List<SqlDataRecord> tfsProductVersionsResultsList = ProductVersions.GetSearchResultsList(searchTerm, loginUser);

                            if (tfsProductVersionsResultsList.Count > 0)
                            {
                                versionsQuery.Append(@"
                                    SELECT
                                        pv.ProductVersionID
                                        , 14
                                        , fpvrt.relevance
                                        , pv.DateModified
                                    FROM
                                        dbo.ProductVersions pv
                                        JOIN @ftsProductVersionsResultsTable fpvrt
                                            ON pv.ProductVersionID = fpvrt.recordID
                                ");

                                if (productsQuery.Length > 0)
                                {
                                    versionsQuery.AppendLine("UNION");
                                }

                                SqlParameter ftsProductVersionsResultsTable = new SqlParameter("@ftsProductVersionsResultsTable", SqlDbType.Structured);
                                ftsProductVersionsResultsTable.TypeName = "dtSearch_results_tbltype";
                                ftsProductVersionsResultsTable.Value = tfsProductVersionsResultsList;
                                command.Parameters.Add(ftsProductVersionsResultsTable);
                            }
                        }

                        if (productsQuery.Length > 0 || versionsQuery.Length > 0)
                        {
                            StringBuilder orderByClause = new StringBuilder();
                            if (!string.IsNullOrWhiteSpace(searchTerm))
                            {
                                orderByClause.Append("ti.relevance DESC");
                            }
                            else
                            {
                                orderByClause.Append("p.name, pv.versionNumber");
                            }

                            string query = @"
                                DECLARE @TempItems
                                TABLE
                                (
                                  ID            int IDENTITY,
                                  recordID      int,
                                  source        int,
                                  relevance     int,
                                  DateModified  datetime
                                )

                                INSERT INTO @TempItems
                                (
                                  recordID,
                                  source,
                                  relevance,
                                  DateModified
                                )
                                {0}
                                {1}

                                SET @resultsCount = @@RowCount

                                SELECT
                                  p.productID,
                                  p.name,
                                  p.Name AS productName,
                                  p.description,
                                  pv.productVersionID,
                                  pv.versionNumber,
                                  pv.releaseDate,
                                  pv.isReleased,
                                  pvs.Name AS versionStatus
                                FROM
                                  @TempItems ti
                                  LEFT JOIN dbo.Products p
                                    ON ti.source = 13
                                    AND ti.recordID = p.ProductID
                                  LEFT JOIN dbo.ProductVersions pv
                                    ON ti.source = 14
                                    AND ti.recordID = pv.ProductVersionID
                                  LEFT JOIN dbo.ProductVersionStatuses pvs
                                    ON pv.ProductVersionStatusID = pvs.ProductVersionStatusID
                                WHERE
                                  ti.ID BETWEEN @FromIndex AND @toIndex
                                ORDER BY
                                  " + orderByClause.ToString() + @"
                                FOR JSON PATH";

                            command.CommandText = string.Format(query, versionsQuery.ToString(), productsQuery.ToString());
                            command.CommandType = CommandType.Text;

                            SqlParameter resultsCount = new SqlParameter("@resultsCount", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            command.Parameters.Add(resultsCount);

                            command.Parameters.AddWithValue("@FromIndex", from + 1);
                            command.Parameters.AddWithValue("@ToIndex", from + count);

                            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                            {
                                command.Connection = connection;
                                connection.Open();
                                var reader = command.ExecuteReader();
                                StringBuilder resultItemsBuilder = new StringBuilder();
                                while (reader.Read())
                                {
                                    resultItemsBuilder.Append(reader.GetValue(0).ToString());
                                }
                                Newtonsoft.Json.Linq.JArray resultItemsJArray = Newtonsoft.Json.Linq.JArray.Parse(resultItemsBuilder.ToString());
                                for (int j = 0; j < resultItemsJArray.Count; j++)
                                {
                                    resultItems.Add(resultItemsJArray[j].ToString());
                                }
                            }
                        }
                        break;
                }
                return resultItems.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(loginUser, ex, "SearchService.SearchProducts");
            }
            return null;
        }

        [WebMethod]
        public ProductFamilyProxy[] SearchProductFamilies(string searchTerm, int from)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ProductFamilies productFamilies = new ProductFamilies(TSAuthentication.GetLoginUser());
            productFamilies.LoadBySearchTerm(searchTerm, from, loginUser.OrganizationID, loginUser.UserID);

            return productFamilies.GetProductFamilyProxies();
        }

    }



    [DataContract(Namespace = "http://teamsupport.com/")]
    public class AdvancedSearchOptions
    {
        [DataMember]
        public int? StandardFilterID { get; set; }
        [DataMember]
        public bool? Tickets { get; set; }
        [DataMember]
        public bool? KnowledgeBase { get; set; }
        [DataMember]
        public bool? Wikis { get; set; }
        [DataMember]
        public bool? Notes { get; set; }
        [DataMember]
        public bool? ProductVersions { get; set; }
        [DataMember]
        public bool? Tasks { get; set; }
        [DataMember]
        public bool? WaterCooler { get; set; }
        [DataMember]
        public AutoFieldItem[] Fields { get; set; }
        [DataMember]
        public SearchCustomFilterProxy[] CustomFilters { get; set; }
        [DataMember]
        public SearchSorterProxy[] Sorters { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class CompaniesAndContactsSearchResults
    {
        [DataMember]
        public string SearchTerm { get; set; }
        [DataMember]
        public int From { get; set; }
        [DataMember]
        public int To { get; set; }
        [DataMember]
        public CompanyOrContact[] Items { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class CompanyOrContact
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public ReferenceType ReferenceType { get; set; }
    }
}
