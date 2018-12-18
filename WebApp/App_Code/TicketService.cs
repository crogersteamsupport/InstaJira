using Microsoft.SqlServer.Server;
using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using dtSearch.Engine;
using HtmlAgilityPack;
using System.IO;
using TidyNet;
using ImageResizer;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;
using OpenTokSDK;
using Jira = TeamSupport.JIRA;
using NR = NewRelic.Api;
using TeamSupport.ModelAPI;
using System.Threading;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TicketService : System.Web.Services.WebService
    {

        public TicketService()
        {

            //Uncomment the following line if using designed components
            //InitializeComponent();
        }

        [WebMethod]
        public TicketPage GetTicketPage(int pageIndex, int pageSize, TicketLoadFilter filter)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            if (filter == null) filter = new TicketLoadFilter();
            tickets.LoadByFilter(pageIndex, pageSize, filter);

            int totalRecords = 0;

            if (tickets.Any() && tickets.Count > 0)
            {
                totalRecords = tickets[0].TotalRecords;
            }

            return new TicketPage(pageIndex, pageSize, totalRecords, tickets.GetTicketsViewItemProxies(), filter);
        }

        [WebMethod]
        public GridResult GetTicketRange(int from, int to, TicketLoadFilter filter)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            GridResult result = new GridResult();
            string totalRecords = string.Empty;
            result.From = from;
            result.To = to;
            result.Total = 0;
            List<TicketsViewItemProxy> list = new List<TicketsViewItemProxy>();
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                connection.Open();
                if (filter == null) filter = new TicketLoadFilter();
                SqlCommand command = TicketsView.GetLoadRangeCommand(loginUser, from, to, filter);
                command.Connection = connection;
                SqlDataReader reader = command.ExecuteReader();

                try
                {

                    while (reader.Read())
                    {
                        TicketsViewItemProxy item = new TicketsViewItemProxy();
                        item.SalesForceID = reader["SalesForceID"] as string;
                        item.KnowledgeBaseCategoryName = reader["KnowledgeBaseCategoryName"] as string;
                        item.KnowledgeBaseCategoryID = reader["KnowledgeBaseCategoryID"] as int?;
                        item.CategoryName = reader["CategoryName"] as string;
                        item.ForumCategory = reader["ForumCategory"] as int?;
                        item.TicketSource = reader["TicketSource"] as string;
                        item.Customers = reader["Customers"] as string;
                        item.Contacts = reader["Contacts"] as string;
                        item.SlaWarningHours = reader["SlaWarningHours"] as decimal?;
                        item.SlaViolationHours = reader["SlaViolationHours"] as decimal?;
                        item.SlaWarningTime = reader["SlaWarningTime"] as int?;
                        item.SlaViolationTime = reader["SlaViolationTime"] as int?;
                        item.Tags = reader["Tags"] as string;
                        item.HoursSpent = reader["HoursSpent"] as decimal?;
                        item.ModifierName = reader["ModifierName"] as string;
                        item.CreatorName = reader["CreatorName"] as string;
                        item.CloserName = reader["CloserName"] as string;
                        item.DaysOpened = reader["DaysOpened"] as int?;
                        item.DaysClosed = reader["DaysClosed"] as int? ?? 0;
                        item.CloserID = reader["CloserID"] as int?;
                        item.CreatorID = reader["CreatorID"] as int? ?? -1;
                        item.ModifierID = reader["ModifierID"] as int? ?? -1;
                        item.ParentID = reader["ParentID"] as int?;
                        item.Name = reader["Name"] as string;
                        item.OrganizationID = reader["OrganizationID"] as int? ?? loginUser.OrganizationID;
                        item.TicketSeverityID = reader["TicketSeverityID"] as int? ?? -1;
                        item.TicketTypeID = reader["TicketTypeID"] as int? ?? -1;
                        item.TicketStatusID = reader["TicketStatusID"] as int? ?? -1;
                        item.UserID = reader["UserID"] as int? ?? -1;
                        item.GroupID = reader["GroupID"] as int? ?? -1;
                        item.ProductID = reader["ProductID"] as int? ?? -1;
                        item.SolvedVersionID = reader["SolvedVersionID"] as int?;
                        item.ReportedVersionID = reader["ReportedVersionID"] as int?;
                        item.IsKnowledgeBase = reader["IsKnowledgeBase"] as bool? ?? false;
                        item.IsVisibleOnPortal = reader["IsVisibleOnPortal"] as bool? ?? false;
                        item.TicketNumber = reader["TicketNumber"] as int? ?? -1;
                        item.Severity = reader["Severity"] as string;
                        item.IsClosed = reader["IsClosed"] as bool? ?? false;
                        item.SeverityPosition = reader["SeverityPosition"] as int?;
                        item.StatusPosition = reader["StatusPosition"] as int?;
                        item.Status = reader["Status"] as string;
                        item.UserName = reader["UserName"] as string;
                        item.TicketTypeName = reader["TicketTypeName"] as string;
						item.TicketTypeID = reader["TicketTypeID"] as int? ?? -1;
						item.GroupName = reader["GroupName"] as string;
                        item.SolvedVersion = reader["SolvedVersion"] as string;
                        item.ReportedVersion = reader["ReportedVersion"] as string;
                        item.ProductName = reader["ProductName"] as string;
                        item.TicketID = reader["TicketID"] as int? ?? -1;

                        item.DateModified = DateTime.SpecifyKind(reader["DateModified"] as DateTime? ?? DateTime.MinValue, DateTimeKind.Utc);
                        item.DateCreated = DateTime.SpecifyKind(reader["DateCreated"] as DateTime? ?? DateTime.MinValue, DateTimeKind.Utc);

                        item.DateModifiedBySalesForceSync = GetReaderNullableDate(reader["DateModifiedBySalesForceSync"]);
                        item.SlaWarningDate = GetReaderNullableDate(reader["SlaWarningDate"]);
                        item.SlaViolationDate = GetReaderNullableDate(reader["SlaViolationDate"]);
                        item.DateClosed = GetReaderNullableDate(reader["DateClosed"]);
                        item.DueDate = GetReaderNullableDate(reader["DueDate"]);

                        item.IsRead = reader["IsRead"] as bool? ?? false;
                        item.IsFlagged = reader["IsFlagged"] as bool? ?? false;
                        item.IsSubscribed = reader["IsSubscribed"] as bool? ?? false;
                        item.IsEnqueued = reader["IsEnqueued"] as bool? ?? false;
                        item.ViewerID = reader["ViewerID"] as int?;
                        list.Add(item);

                        totalRecords = Convert.ToString(reader["TotalRecords"]);
                    }
                }
                finally
                {
                    reader.Close();
                }

                result.Data = list;

                if (!string.IsNullOrEmpty(totalRecords))
                {
                    result.Total = int.Parse(totalRecords);
                }

                return result;
            }
        }

        private DateTime? GetReaderNullableDate(object o)
        {
            return o == null || o == DBNull.Value ? null : (DateTime?)DateTime.SpecifyKind(o as DateTime? ?? DateTime.MinValue, DateTimeKind.Utc);

        }

        [WebMethod]
        public int GetMyOpenReadCount()
        {
            return Tickets.GetMyOpenUnreadTicketCount(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
        }

        [WebMethod]
        public KnowledgeBaseCategoryTicketsProxy GetKnowledgeBaseCategoryTickets(
          int? categoryID,
          string categoryName,
          string parentCategoryName,
          int firstItemIndex,
          int pageSize)
        {
            string categoryIdClause = "AND utv.KnowledgeBaseCategoryID IS NULL ";
            string orderByClause = "utv.DateModified DESC";
            if (categoryID != null)
            {
                categoryIdClause = "AND utv.KnowledgeBaseCategoryID = " + categoryID + " ";
            }
            else if (categoryName != null && categoryName != string.Empty)
            {
                categoryIdClause = string.Empty;
                if (categoryName == "New Articles")
                {
                    orderByClause = "utv.DateCreated DESC";
                }
            }

            string query = @"SELECT utv.TicketID, utv.Name, COUNT(*) OVER() as resultsCount FROM Tickets utv
                WHERE utv.OrganizationID = @OrganizationID AND utv.IsKnowledgeBase = 1 " +
                categoryIdClause +
                @" ORDER BY " + orderByClause +
                @" OFFSET @FromIndex -1 ROWS FETCH NEXT @PageSize ROWS ONLY";

            SqlCommand command = new SqlCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            command.Parameters.AddWithValue("@FromIndex", firstItemIndex + 1);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@OrganizationID", TSAuthentication.OrganizationID);
            command.Parameters.AddWithValue("@ViewerID", TSAuthentication.GetLoginUser().UserID);

            KnowledgeBaseCategoryTicketsProxy result = new KnowledgeBaseCategoryTicketsProxy();
            result.CategoryID = categoryID;
            result.CategoryName = categoryName;
            result.ParentCategoryName = parentCategoryName;

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(TSAuthentication.GetLoginUser().ConnectionString))
            {
                command.Connection = connection;
                connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    try
                    {
                        adapter.Fill(table);
                        result.Count = (table.Rows.Count > 0) ? (int)table.Rows[0]["resultsCount"] : 0;
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            List<KnowledgeBaseCategoryTicketsItemProxy> resultItems = new List<KnowledgeBaseCategoryTicketsItemProxy>();

            foreach (DataRow row in table.Rows)
            {
                KnowledgeBaseCategoryTicketsItemProxy resultItem = new KnowledgeBaseCategoryTicketsItemProxy();

                resultItem.ID = (int)row["TicketID"];
                string fullLengthName = row["Name"].ToString();
                if (pageSize == 5 && categoryID != null && fullLengthName.Length > 45)
                {
                    resultItem.Name = fullLengthName.Substring(0, 45) + "...";
                }
                else
                {
                    resultItem.Name = fullLengthName;
                }

                resultItems.Add(resultItem);
            }

            result.Items = resultItems.ToArray();
            return result;
        }

        [WebMethod]
        public KnowledgeBaseCategoryTicketsProxy GetNewKnowledgeBaseArticles(
          int firstItemIndex,
          int pageSize)
        {
            string query = @"
        DECLARE @TempItems
        TABLE
        (
          ID        int IDENTITY,
          TicketID  int
        )

        INSERT INTO @TempItems
        (
          TicketID
        )
        SELECT
          TOP 20
          utv.TicketID
        FROM
          UserTicketsView utv
        WHERE
          utv.OrganizationID              = @OrganizationID
          AND utv.IsKnowledgeBase         = 1
          AND utv.ViewerID                = @ViewerID
        ORDER BY
          utv.DateCreated DESC

        SET @resultsCount = @@RowCount

        SELECT
          t.TicketID
          , t.Name
          , t.DateCreated
        FROM
          @TempItems ti
          JOIN dbo.Tickets t
            ON ti.TicketID = t.TicketID
        WHERE
          ti.ID BETWEEN @FromIndex AND @toIndex
        ORDER BY
          ti.ID
      ";

            SqlCommand command = new SqlCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            SqlParameter resultsCount = new SqlParameter("@resultsCount", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(resultsCount);
            command.Parameters.AddWithValue("@FromIndex", firstItemIndex + 1);
            command.Parameters.AddWithValue("@ToIndex", firstItemIndex + pageSize);
            command.Parameters.AddWithValue("@OrganizationID", TSAuthentication.OrganizationID);
            command.Parameters.AddWithValue("@ViewerID", TSAuthentication.GetLoginUser().UserID);

            KnowledgeBaseCategoryTicketsProxy result = new KnowledgeBaseCategoryTicketsProxy();

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(TSAuthentication.GetLoginUser().ConnectionString))
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

            List<KnowledgeBaseCategoryTicketsItemProxy> resultItems = new List<KnowledgeBaseCategoryTicketsItemProxy>();

            foreach (DataRow row in table.Rows)
            {
                KnowledgeBaseCategoryTicketsItemProxy resultItem = new KnowledgeBaseCategoryTicketsItemProxy();

                resultItem.ID = (int)row["TicketID"];

                string fullLengthName = row["Name"].ToString();
                if (pageSize == 5 && fullLengthName.Length > 37)
                {
                    resultItem.Name = fullLengthName.Substring(0, 37) + "...";
                }
                else
                {
                    resultItem.Name = fullLengthName;
                }

                resultItem.DateCreated = row["DateCreated"].ToString();

                resultItems.Add(resultItem);
            }

            result.Items = resultItems.ToArray();
            return result;
        }

        [WebMethod]
        public KnowledgeBaseCategoryTicketsProxy GetRecentlyModifiedKnowledgeBaseArticles(
          int firstItemIndex,
          int pageSize)
        {
            string query = @"
        DECLARE @TempItems
        TABLE
        (
          ID        int IDENTITY,
          TicketID  int
        )

        INSERT INTO @TempItems
        (
          TicketID
        )
        SELECT
          TOP 20
          utv.TicketID
        FROM
          UserTicketsView utv
        WHERE
          utv.OrganizationID              = @OrganizationID
          AND utv.IsKnowledgeBase         = 1
          AND utv.ViewerID                = @ViewerID
        ORDER BY
          utv.DateModified DESC

        SET @resultsCount = @@RowCount

        SELECT
          t.TicketID
          , t.Name
          , t.DateModified
        FROM
          @TempItems ti
          JOIN dbo.Tickets t
            ON ti.TicketID = t.TicketID
        WHERE
          ti.ID BETWEEN @FromIndex AND @toIndex
        ORDER BY
          ti.ID
      ";

            SqlCommand command = new SqlCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            SqlParameter resultsCount = new SqlParameter("@resultsCount", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(resultsCount);
            command.Parameters.AddWithValue("@FromIndex", firstItemIndex + 1);
            command.Parameters.AddWithValue("@ToIndex", firstItemIndex + pageSize);
            command.Parameters.AddWithValue("@OrganizationID", TSAuthentication.OrganizationID);
            command.Parameters.AddWithValue("@ViewerID", TSAuthentication.GetLoginUser().UserID);

            KnowledgeBaseCategoryTicketsProxy result = new KnowledgeBaseCategoryTicketsProxy();

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(TSAuthentication.GetLoginUser().ConnectionString))
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

            List<KnowledgeBaseCategoryTicketsItemProxy> resultItems = new List<KnowledgeBaseCategoryTicketsItemProxy>();

            foreach (DataRow row in table.Rows)
            {
                KnowledgeBaseCategoryTicketsItemProxy resultItem = new KnowledgeBaseCategoryTicketsItemProxy();

                resultItem.ID = (int)row["TicketID"];

                string fullLengthName = row["Name"].ToString();
                if (pageSize == 5 && fullLengthName.Length > 37)
                {
                    resultItem.Name = fullLengthName.Substring(0, 37) + "...";
                }
                else
                {
                    resultItem.Name = fullLengthName;
                }

                resultItem.DateModified = row["DateModified"].ToString();

                resultItems.Add(resultItem);
            }

            result.Items = resultItems.ToArray();
            return result;
        }

        [WebMethod]
        public KnowledgeBaseCategoryTicketsProxy GetKnowledgeBaseSearchResults(string searchTerm, int firstItemIndex, int pageSize)
        {

            LoginUser loginUser = TSAuthentication.GetLoginUser();

            KnowledgeBaseCategoryTicketsProxy result = new KnowledgeBaseCategoryTicketsProxy();
            result.CategoryName = "Search results";
            List<KnowledgeBaseCategoryTicketsItemProxy> resultItems = new List<KnowledgeBaseCategoryTicketsItemProxy>();

            SqlCommand command = new SqlCommand();
            command.CommandText = "Tickets_KBSearch";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FromIndex", firstItemIndex);
            command.Parameters.AddWithValue("@ToIndex", pageSize);
            command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);
            command.Parameters.AddWithValue("@SearchTerm", cleanSearchTerm(searchTerm));

            DataTable table = new DataTable();
            using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
            {
                command.Connection = connection;
                connection.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
            }

            foreach (DataRow row in table.Rows)
            {
                KnowledgeBaseCategoryTicketsItemProxy resultItem = new KnowledgeBaseCategoryTicketsItemProxy();

                resultItem.ID = (int)row["TicketID"];

                string fullLengthName = row["Name"].ToString();
                resultItem.Name = fullLengthName;

                resultItems.Add(resultItem);
            }


            result.Items = resultItems.ToArray();
            return result;

        }

        [WebMethod]
        public TicketTypeProxy[] GetTicketTypes()
        {
            TicketTypes types = new TicketTypes(TSAuthentication.GetLoginUser());
            types.LoadAllPositions(TSAuthentication.OrganizationID);
            return types.GetTicketTypeProxies();
        }

        [WebMethod]
        public TicketSeverityProxy[] GetTicketSeverities()
        {
            TicketSeverities severities = new TicketSeverities(TSAuthentication.GetLoginUser());
            severities.LoadAllPositions(TSAuthentication.OrganizationID);
            return severities.GetTicketSeverityProxies();
        }

        [WebMethod]
        public TicketStatusProxy[] GetTicketStatuses()
        {
            TicketStatuses statuses = new TicketStatuses(TSAuthentication.GetLoginUser());
            statuses.LoadByOrganizationID(TSAuthentication.OrganizationID);
            return statuses.GetTicketStatusProxies();
        }

        [WebMethod]
        public TicketStatusProxy[] GetTicketStatusesOrderedByTicketTypeName()
        {
            TicketStatuses statuses = new TicketStatuses(TSAuthentication.GetLoginUser());
            statuses.LoadByOrganizationIDOnTicketType(TSAuthentication.OrganizationID);
            return statuses.GetTicketStatusProxies();
        }

        [WebMethod]
        public TicketNextStatusProxy[] GetNextTicketStatuses()
        {
            TicketNextStatuses statuses = new TicketNextStatuses(TSAuthentication.GetLoginUser());
            statuses.LoadAll(TSAuthentication.OrganizationID);
            return statuses.GetTicketNextStatusProxies();
        }

        [WebMethod]
        public TicketStatusProxy[] GetAvailableTicketStatuses(int statusID)
        {
            TicketStatuses statuses = new TicketStatuses(TSAuthentication.GetLoginUser());
            statuses.LoadNextStatuses(statusID);
            return statuses.GetTicketStatusProxies();
        }

        [WebMethod]
        public ActionTypeProxy[] GetActionTypes()
        {
            ActionTypes types = new ActionTypes(TSAuthentication.GetLoginUser());
            types.LoadAllPositions(TSAuthentication.OrganizationID);
            return types.GetActionTypeProxies();
        }

        [WebMethod]
        public string[] ActionToText(int actionID)
        {
            List<string> result = new List<string>();
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);

            result.Add(HtmlToText.ConvertHtml(HtmlUtility.TidyHtml(action.Description)));
            result.Add(action.Description);
            return result.ToArray();

        }

        [WebMethod]
        public TicketTypeProxy GetTicketType(int ticketTypeID)
        {
            TicketType type = TicketTypes.GetTicketType(TSAuthentication.GetLoginUser(), ticketTypeID);
            if (type.OrganizationID != TSAuthentication.OrganizationID) return null;
            return type.GetProxy();
        }
        /*
        [WebMethod]
        public TicketsViewItemProxy SetTicketType(int ticketID, int ticketTypeID)
        {
          Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
          if (CanEditTicket(ticket))
          {
            TicketStatuses statuses = new TicketStatuses(ticket.Collection.LoginUser);
            statuses.LoadAvailableTicketStatuses(ticket.TicketTypeID, null);
            if (statuses.IsEmpty) return null;

            ticket.TicketTypeID = ticketTypeID;
            ticket.TicketStatusID = statuses[0].TicketStatusID;
            ticket.Collection.Save();
            return ticket.GetTicketView().GetProxy();
          }
          return null;
        }*/

        [WebMethod]
        public TagProxy[] GetTags()
        {
            Tags tags = new Tags(TSAuthentication.GetLoginUser());
            tags.LoadByOrganization(TSAuthentication.OrganizationID);
            return tags.GetTagProxies();
        }


        [WebMethod]
        public string[] GetContactsAndCustomers(int ticketID)
        {
            List<string> result = new List<string>();
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByTicketID(ticketID);

            ContactsView contacts = new ContactsView(TSAuthentication.GetLoginUser());
            contacts.LoadByTicketID(ticketID);

            foreach (Organization o in organizations) { result.Add(o.Name); }
            foreach (ContactsViewItem contact in contacts)
            {
                result.Add(contact.FirstName + " " + contact.LastName + " [" + contact.Organization + "]");
            }

            return result.ToArray();
        }

        [WebMethod]
        public ActionsViewItemProxy[] GetActions(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            ActionsView view = new ActionsView(TSAuthentication.GetLoginUser());
            view.LoadByTicketID(ticketID);
            return view.GetActionsViewItemProxies();
        }

        [WebMethod]
        public object[] GetKBTicketAndActions(int ticketID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
            //if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            ActionsView view = new ActionsView(TSAuthentication.GetLoginUser());
            view.LoadKBByTicketID(ticketID);

            List<object> result = new List<object>();
            result.Add(ticket.GetProxy());
            result.Add(view.GetActionsViewItemProxies());
            return result.ToArray();
        }

        [WebMethod]
        public ActionsViewItemProxy GetAction(int actionID)
        {
            ActionsViewItem action = ActionsView.GetActionsViewItem(TSAuthentication.GetLoginUser(), actionID);
            Ticket ticket = Tickets.GetTicket(action.Collection.LoginUser, action.TicketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            //action.Name = action.ActionTitle;
            return action.GetProxy();
        }

        [WebMethod]
        public void DeleteTag(int tagID) {
            if (!TSAuthentication.IsSystemAdmin) {
                return;
            } else {
                Tag tag = Tags.GetTag(TSAuthentication.GetLoginUser(), tagID);
                string tagValue = tag.Value;
                tag.Delete();
                tag.Collection.Save();

                // DEFLECTOR.
                DeflectorService Deflection = new DeflectorService();
                Deflection.DeleteTag(TSAuthentication.OrganizationID, tagValue);
            }
        }

        [WebMethod]
        public int RenameTag (int TagId, string Value) {
            int Result = TagId;
            if (!TSAuthentication.IsSystemAdmin) {
                // return Result;
            }
            Tags Tags = new Tags(TSAuthentication.GetLoginUser());
            Tag Tag = Tags.GetTag(Tags.LoginUser, TagId);
            Tags.LoadByValue(TSAuthentication.OrganizationID, Value);

            string OldTagValue = Tag.Value;

            // MERGE TAG.
            if (Tags.Count > 0 && Tag.TagID != Tags[0].TagID) {
                TagLinks Links = new TagLinks(Tags.LoginUser);
                Links.ReplaceTags(Tag.TagID, Tags[0].TagID);
                Tag.Delete();
                Tag.Collection.Save();
                Result = Tags[0].TagID;
            }
            // RENAME TAG.
            else {
                Tag.Value = Value;
                Tag.Collection.Save();
            }

            try {
                DeflectorAPI deflectorAPI = new DeflectorAPI();
                deflectorAPI.RenameTagAsync(TSAuthentication.OrganizationID, OldTagValue, Value);
            }
            catch (Exception ex) {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Deflector");
            }

            return Result;
        }

        [WebMethod]
        public AutocompleteItem[] SearchTickets(string searchTerm, TicketLoadFilter filter)
        {
            try
            {
                LoginUser loginUser = TSAuthentication.GetLoginUser();
                Stopwatch stopWatch = Stopwatch.StartNew();
                SearchResults results = TicketsView.GetQuickSearchTicketResults(searchTerm, loginUser, filter);
                stopWatch.Stop();
                NR.Agent.NewRelic.RecordMetric("Custom/SearchTickets", stopWatch.ElapsedMilliseconds);

                //Only record the custom parameter in NR if the search took longer than 3 seconds (I'm using this arbitrarily, seems appropiate)
                if (stopWatch.ElapsedMilliseconds > 500)
                {
                    NR.Agent.NewRelic.AddCustomParameter("SearchTickets-OrgId", TSAuthentication.OrganizationID);
                    NR.Agent.NewRelic.AddCustomParameter("SearchTickets-Term", searchTerm);
                }

                List<AutocompleteItem> items = new List<AutocompleteItem>();

                stopWatch.Restart();
                for (int i = 0; i < results.Count; i++)
                {
                    results.GetNthDoc(i);

                    items.Add(new AutocompleteItem(results.CurrentItem.DisplayName, results.CurrentItem.UserFields["TicketNumber"].ToString(), results.CurrentItem.UserFields["TicketID"].ToString()));
                }
                stopWatch.Stop();
                NR.Agent.NewRelic.RecordMetric("Custom/SearchTicketsPullData", stopWatch.ElapsedMilliseconds);

                return items.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "TicketService.SearchTickets");
            }
            return null;
        }

        [WebMethod]
        public string[] GetSearchPortalResultsTest(string searchTerm, int organizationID, int customerID)
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            LoginUser loginUser = new LoginUser(TSAuthentication.GetLoginUser().ConnectionString, TSAuthentication.UserID, organizationID, null);
            SearchResults results = GetSearchPortalResults(searchTerm, loginUser, organizationID, customerID);

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
                results.GetNthDoc(i);

                using (FileConverter fc = new FileConverter())
                {

                    // This sets up FileConverter with the input file, index location, and hits
                    fc.SetInputItem(results, 0);

                    fc.OutputToString = true;
                    fc.OutputStringMaxSize = 2000000;
                    fc.OutputFormat = OutputFormats.itHTML;

                    //String 	scriptName = Request.ServerVariables.Get ("SCRIPT_NAME");
                    //String virtPath = MakeVirtual (res.get_DocDetailItem("_filename"));
                    //String HitNavSrc = GetVirtualFolder(scriptName) + "HitNav.js";
                    //String HitNavLink =  "\r\n<script language=JavaScript src=\"" + HitNavSrc + "\"></script>\r\n";
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


                    // Set up additional settings for the file converter
                    fc.BeforeHit = "<span style=\"background-color: #FF0000\">";
                    fc.AfterHit = "</span>";

                    // If the index was built with cached text, get the document from the cache
                    fc.Flags = (fc.Flags | ConvertFlags.dtsConvertGetFromCache);

                    // Execute the file conversion
                    fc.Execute();

                    StringBuilder builder = new StringBuilder();
                    //builder.Append("<h1>" + results.CurrentItem.DisplayName + "</h1>");
                    //builder.Append("<p class=\"ui-helper-hiddenx\">" + results.CurrentItem.Synopsis + "</p>");

                    JobErrorInfo errors = fc.Errors;
                    if ((errors.Count > 0) || (fc.OutputString.Length < 2))
                    {
                        builder.AppendLine("<html><h1>Error converting document to HTML</h1>");
                        builder.AppendLine("<p>" + Server.HtmlEncode(fc.InputFile));
                        if (errors.Count > 0)
                        {
                            builder.AppendLine("<p>");
                            builder.AppendLine(Server.HtmlEncode(errors.Message(0)));
                        }
                        builder.AppendLine("<p>Note: To prevent conversion errors when highlighting hits " +
                          "use caching when building the index.  For more information, see " +
                          "<a href=\"http://www.dtsearch.com/index7.html\">http://www.dtsearch.com/index7.html</a> " +
                          "and the dtsIndexCacheOriginalFiles flag in the dtSearch Engine API documentation");

                    }
                    else
                    {
                        // Output the result of the file conversion
                        builder.AppendLine(fc.OutputString);
                    }
                    result.Add(builder.ToString());
                }



            }
            return result.ToArray();

        }

        private static SearchResults GetSearchPortalResults(string searchTerm, LoginUser loginUser, int organizationID, int customerID)
        {
            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;

            using (SearchJob job = new SearchJob())
            {
                searchTerm = searchTerm.Trim();
                job.Request = searchTerm;
                job.FieldWeights = "TicketNumber: 5000, Name: 1000";

                Organization customer = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), customerID);
                job.BooleanConditions = string.Format("(OrganizationID::{0}) AND (IsVisibleOnPortal::True) AND ((IsKnowledgeBase::True) OR (Customers::{1}))", organizationID.ToString(), customer.Name);
                job.MaxFilesToRetrieve = 25;
                job.AutoStopLimit = 100000;
                job.TimeoutSeconds = 10;
                job.SearchFlags =
                  SearchFlags.dtsSearchStemming |
                  SearchFlags.dtsSearchDelayDocInfo;

                int num = 0;
                if (!int.TryParse(searchTerm, out num))
                {
                    job.Fuzziness = 1;
                    job.Request = job.Request + "*";
                    //job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchSelectMostRecent;
                    //job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy | SearchFlags.dtsSearchSelectMostRecent;
                }

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
                job.IndexesToSearch.Add(DataUtils.GetTicketIndexPath(loginUser));
                job.Execute();
                return job.Results;
            }
        }

        [WebMethod]
        public string[] SearchTicketsTest(string searchTerm, int organizationID)
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            LoginUser loginUser = new LoginUser(TSAuthentication.GetLoginUser().ConnectionString, TSAuthentication.UserID, organizationID, null);
            SearchResults results = TicketsView.GetQuickSearchTicketResults(searchTerm, loginUser, null);

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
                results.GetNthDoc(i);

                using (FileConverter fc = new FileConverter())
                {

                    // This sets up FileConverter with the input file, index location, and hits
                    fc.SetInputItem(results, i);

                    fc.OutputToString = true;
                    fc.OutputStringMaxSize = 2000000;
                    fc.OutputFormat = OutputFormats.itHTML;

                    //String 	scriptName = Request.ServerVariables.Get ("SCRIPT_NAME");
                    //String virtPath = MakeVirtual (res.get_DocDetailItem("_filename"));
                    //String HitNavSrc = GetVirtualFolder(scriptName) + "HitNav.js";
                    //String HitNavLink =  "\r\n<script language=JavaScript src=\"" + HitNavSrc + "\"></script>\r\n";
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


                    // Set up additional settings for the file converter
                    fc.BeforeHit = "<span style=\"background-color: #FF0000\">";
                    fc.AfterHit = "</span>";

                    // If the index was built with cached text, get the document from the cache
                    fc.Flags = (fc.Flags | ConvertFlags.dtsConvertGetFromCache);

                    // Execute the file conversion
                    fc.Execute();

                    StringBuilder builder = new StringBuilder();
                    //builder.Append("<h1>" + results.CurrentItem.DisplayName + "</h1>");
                    //builder.Append("<p class=\"ui-helper-hiddenx\">" + results.CurrentItem.Synopsis + "</p>");

                    JobErrorInfo errors = fc.Errors;
                    if ((errors.Count > 0) || (fc.OutputString.Length < 2))
                    {
                        builder.AppendLine("<html><h1>Error converting document to HTML</h1>");
                        builder.AppendLine("<p>" + Server.HtmlEncode(fc.InputFile));
                        if (errors.Count > 0)
                        {
                            builder.AppendLine("<p>");
                            builder.AppendLine(Server.HtmlEncode(errors.Message(0)));
                        }
                        builder.AppendLine("<p>Note: To prevent conversion errors when highlighting hits " +
                          "use caching when building the index.  For more information, see " +
                          "<a href=\"http://www.dtsearch.com/index7.html\">http://www.dtsearch.com/index7.html</a> " +
                          "and the dtsIndexCacheOriginalFiles flag in the dtSearch Engine API documentation");

                    }
                    else
                    {
                        // Output the result of the file conversion
                        builder.AppendLine(fc.OutputString);
                    }
                    result.Add(builder.ToString());
                }



            }
            return result.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] GetTicketsByTerm(string term)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            tickets.LoadByDescription(TSAuthentication.OrganizationID, DataUtils.BuildSearchString(term, true));

            foreach (Ticket ticket in tickets)
            {
                AutocompleteItem item = new AutocompleteItem();
                item.id = ticket.TicketNumber.ToString();
                item.label = ticket.Row["TicketDescription"].ToString();
                result.Add(item);
            }

            return result.ToArray();
        }

        [WebMethod]
        public TicketsViewItemProxy GetTicket(int ticketID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            return ticket.GetProxy();
        }

        [WebMethod]
        public string GetTicketTypeTemplateText(int ticketTypeID)
        {
            if (TicketTemplates.GetTicketTypeCount(TSAuthentication.GetLoginUser()) > 0)
            {
                TicketTemplate template = TicketTemplates.GetByTicketType(TSAuthentication.GetLoginUser(), ticketTypeID);
                if (template == null) return "";
                if (template.OrganizationID != TSAuthentication.OrganizationID) return "";
                return template.TemplateText;
            }

            return null;
        }

        [WebMethod]
        public string GetValueTemplateText(string value)
        {
            int count = TicketTemplates.GetTriggerTextCount(TSAuthentication.GetLoginUser());
            if (count < 1) return null;

            TicketTemplate template = TicketTemplates.GetByTriggerText(TSAuthentication.GetLoginUser(), value);
            if (template == null) return "";
            if (template.OrganizationID != TSAuthentication.OrganizationID) return "";
            return template.TemplateText;
        }

        [WebMethod]
        public GroupProxy[] GetTicketGroups(int ticketID)
        {
            Tickets ticket = new Tickets(TSAuthentication.GetLoginUser());
            ticket.LoadByTicketID(ticketID);

            if (!ticket.IsEmpty && ticket[0].UserID != null)
            {
                int userID = (int)ticket[0].UserID;
                List<GroupProxy> groupProxies = new List<GroupProxy>();

                Groups orgGroups = new Groups(TSAuthentication.GetLoginUser());
                orgGroups.LoadByNotUserID(userID, TSAuthentication.OrganizationID);

                Groups userGroups = new Groups(TSAuthentication.GetLoginUser());
                userGroups.LoadByUserID(userID);

                groupProxies.AddRange(userGroups.GetGroupProxies().OrderBy(ug => ug.Name));
                groupProxies.AddRange(orgGroups.GetGroupProxies().OrderBy(og => og.Name));

                return groupProxies.ToArray();
            }
            else
            {
                Groups groups = new Groups(TSAuthentication.GetLoginUser());
                groups.LoadByOrganizationID(TSAuthentication.OrganizationID);
                return groups.GetGroupProxies();
            }
        }

        [WebMethod]
        public void DeleteTicket(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
            {
                try
                {
                    int deleteTicketID = ticket.TicketID;
                    ticket.Delete();
                    ticket.Collection.Save();

                    DeflectorService deflectorService = new DeflectorService();
                    deflectorService.DeleteTicket(deleteTicketID);
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "TicketService.DeleteTicket");
                    throw;
                }
            }
        }

        [WebMethod]
        public void DeleteTickets(string ticketIDs)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            foreach (int id in ids)
            {
                DeleteTicket(id);
            }
        }

        [WebMethod]
        public void RequestUpdates(string ticketIDs)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            foreach (int id in ids)
            {
                RequestUpdate(id);
            }
        }

        [WebMethod]
        public void TakeOwnerships(string ticketIDs)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            foreach (int id in ids)
            {
                TakeOwnership(id);
            }
        }

        [WebMethod]
        public ActionInfo RequestUpdate(int ticketID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket == null) return null;
            EmailPosts.SendTicketUpdateRequest(TSAuthentication.GetLoginUser(), ticketID);
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            TeamSupport.Data.Action action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
            action.ActionTypeID = null;
            action.Name = "Update Requested";
            action.ActionSource = "UpdateRequest";
            action.SystemActionTypeID = SystemActionType.UpdateRequest;
            action.Description = String.Format("<p>{0} requested an update for this ticket.</p>", user.FirstName);
            action.IsVisibleOnPortal = false;
            action.IsKnowledgeBase = false;
            action.TicketID = ticket.TicketID;
            action.Collection.Save();


            string description = String.Format("{0} requested an update from {1} for {2}", user.FirstLastName, ticket.UserName, Tickets.GetTicketLink(TSAuthentication.GetLoginUser(), ticketID));
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);

            return GetActionInfo(action.ActionID);
        }

        [WebMethod]
        public void TakeOwnership(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
            ticket.UserID = TSAuthentication.UserID;
            ticket.Collection.Save();
        }

        [WebMethod]
        public void ShowTicketPreviewPane()
        {
            const string UserSettingsKey = "ShowTicketPreviewPane";
            LoginUser user = TSAuthentication.GetLoginUser();

            UserSettings settings = new UserSettings(user);
            settings.LoadByUserSettingKey(user.UserID, UserSettingsKey);

            if (settings.Any())
            {
                string newValue = settings[0].SettingValue == "0" ? "1" : "0";

                settings[0].SettingValue = newValue;
                settings.Save();
            }
            else
            {
                settings = new UserSettings(TSAuthentication.GetLoginUser());
                UserSetting setting = settings.AddNewUserSetting();
                setting.UserID = user.UserID;
                setting.SettingKey = UserSettingsKey;
                setting.SettingValue = "0";

                settings.Save();
            }
        }

        [WebMethod]
        public void DeleteAction(int actionID)
        {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            if (!CanDeleteAction(action)) return;

            ActionLinkToJiraItem actionlink = ActionLinkToJira.GetActionLinkToJiraItemByActionID(TSAuthentication.GetLoginUser(), actionID);
            if (actionlink != null)
            {
                actionlink.Delete();
                actionlink.Collection.Save();
            }

            action.Delete();
            action.Collection.Save();
        }

        [WebMethod]
        public ActionInfo UpdateAction(ActionProxy proxy)
        {
            TeamSupport.Data.Action action = Actions.GetActionByID(TSAuthentication.GetLoginUser(), proxy.ActionID);
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);

            if (action == null)
            {
                action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
                action.TicketID = proxy.TicketID;
                action.CreatorID = TSAuthentication.UserID;
                if (!string.IsNullOrWhiteSpace(user.Signature) && proxy.IsVisibleOnPortal)
                {
                    if (!proxy.Description.Contains(user.Signature))
                    {
                        action.Description = proxy.Description + "<br/><br/>" + user.Signature;
                    }
                    else
                    {
                        action.Description = proxy.Description;
                    }
                }
                else
                {
                    action.Description = proxy.Description;
                }
            }
            else
            {
                if (proxy.IsVisibleOnPortal)
                {
                    if (!string.IsNullOrWhiteSpace(user.Signature))
                    {
                        if (!action.Description.Contains(user.Signature.Replace(" />", ">")))
                        {
                            action.Description = proxy.Description + "<br/><br/>" + user.Signature;
                        }
                        else
                        {
                            action.Description = proxy.Description;
                        }
                    }
                    else
                    {
                        action.Description = proxy.Description;
                    }
                }
                else
                {
                    action.Description = proxy.Description;
                }
            }

            if (!CanEditAction(action)) return null;


            action.ActionTypeID = proxy.ActionTypeID;
            action.DateStarted = proxy.DateStarted;
            action.TimeSpent = proxy.TimeSpent;
            action.IsKnowledgeBase = proxy.IsKnowledgeBase;
            action.IsVisibleOnPortal = proxy.IsVisibleOnPortal;
            action.Collection.Save();

            return GetActionInfo(action.ActionID);
        }

        [WebMethod]
        public bool SetIsVisibleOnPortal(int ticketID, bool value)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return value;
            ticket.IsVisibleOnPortal = value;
            ticket.Collection.Save();

            // DEFLECTOR.
            DeflectorService Deflection = new DeflectorService();
            if (ticket.IsVisibleOnPortal && ticket.IsKnowledgeBase) {
                Deflection.IndexTicket(ticket.TicketID);
            } else {
                Deflection.DeleteTicket(ticket.TicketID);
            }

            return ticket.IsVisibleOnPortal;
        }

        private void Delay(int seconds)
        {
            System.Threading.Thread.Sleep(seconds * 1000);
        }

        [WebMethod]
        public bool SetIsKB(int ticketID, bool value)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return value;
            ticket.IsKnowledgeBase = value;
            ticket.Collection.Save();

            // DEFLECTOR.
            DeflectorService Deflection = new DeflectorService();
            if (ticket.IsVisibleOnPortal && ticket.IsKnowledgeBase) {
                Deflection.IndexTicket(ticket.TicketID);
            } else {
                Deflection.DeleteTicket(ticket.TicketID);
            }

            return ticket.IsKnowledgeBase;
        }

        [WebMethod]
        public string SetTicketName(int ticketID, string name)
        {
            if (name.Trim() == "") {
                name = "[Untitled Ticket]";
            }
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) {
                return name;
            } else {
                ticket.Name = name;// HttpUtility.HtmlEncode(name);
                ticket.Collection.Save();

                if (ticket.IsVisibleOnPortal && ticket.IsKnowledgeBase)
                {
                    DeflectorService Deflection = new DeflectorService();
                    Deflection.IndexTicket(ticket.TicketID);
                }

                return ticket.Name;
            }
        }

        [WebMethod]
        public object[] SetTicketType(int ticketID, int ticketTypeID)
        {
            TransferCustomValues(ticketID, ticketTypeID);
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticketTypeID == ticket.TicketTypeID) return null;
            if (!CanEditTicket(ticket)) return null;
            TicketType ticketType = TicketTypes.GetTicketType(ticket.Collection.LoginUser, ticketTypeID);
            if (ticketType.OrganizationID != TSAuthentication.OrganizationID) return null;
            ticket.TicketTypeID = ticketTypeID;

            TicketStatuses statuses = new TicketStatuses(ticket.Collection.LoginUser);
            statuses.LoadAvailableTicketStatuses(ticketTypeID, null);
            ticket.TicketStatusID = statuses[0].TicketStatusID;
            ticket.Collection.Save();
            List<object> result = new List<object>();
            result.Add(statuses[0].GetProxy());
            result.Add(GetParentCustomValues(ticketID));
            return result.ToArray();
        }

        [WebMethod]
        public void SetTicketsStatus(string ticketIDs, string ticketStatusIDs)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            int[] statusIDs = JsonConvert.DeserializeObject<int[]>(ticketStatusIDs);

            LoginUser loginUser = TSAuthentication.GetLoginUser();
            TicketStatuses statuses = new TicketStatuses(loginUser);
            statuses.LoadByStatusIDs(TSAuthentication.OrganizationID, statusIDs);

            TicketTypes types = new TicketTypes(loginUser);
            types.LoadByOrganizationID(TSAuthentication.OrganizationID);

            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            tickets.LoadByTicketIDs(TSAuthentication.OrganizationID, ids);

            foreach (Ticket ticket in tickets)
            {
                if (!CanEditTicket(ticket)) continue;
                foreach (TicketStatus status in statuses)
                {
                    if (status.OrganizationID != ticket.OrganizationID) continue;
                    if (ticket.TicketTypeID == status.TicketTypeID)
                    {
                        ticket.TicketStatusID = status.TicketStatusID;
                    }
                }
            }

            tickets.Save();
        }

        [WebMethod]
        public TicketStatusProxy SetTicketStatus(int ticketID, int ticketStatusID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticketStatusID == ticket.TicketStatusID) return null;
            if (!CanEditTicket(ticket)) return null;
            TicketStatus status = TicketStatuses.GetTicketStatus(ticket.Collection.LoginUser, ticketStatusID);
            if (status.TicketTypeID != ticket.TicketTypeID) return null;

            if (status.OrganizationID != TSAuthentication.OrganizationID) return null;
            ticket.TicketStatusID = ticketStatusID;
            ticket.Collection.Save();
            return status.GetProxy();
        }

        [WebMethod]
        public void SetTicketsSeverity(string ticketIDs, int ticketSeverityID)
        {
            TicketSeverity severity = TicketSeverities.GetTicketSeverity(TSAuthentication.GetLoginUser(), ticketSeverityID);
            if (severity.OrganizationID != TSAuthentication.OrganizationID) return;

            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            tickets.LoadByTicketIDs(TSAuthentication.OrganizationID, ids);

            foreach (Ticket ticket in tickets)
            {
                if (ticketSeverityID == ticket.TicketSeverityID || !CanEditTicket(ticket)) continue;
                ticket.TicketSeverityID = ticketSeverityID;
            }
            tickets.Save();
        }

        [WebMethod]
        public void SetTicketsDueDate(string ticketIDs, DateTime? datetime)
        {
            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            tickets.LoadByTicketIDs(TSAuthentication.OrganizationID, ids);

            foreach (Ticket ticket in tickets)
            {
                SetDueDate(ticket.TicketID, datetime);
            }

            tickets.Save();
        }

        [WebMethod]
        public TicketSeverityProxy SetTicketSeverity(int ticketID, int ticketSeverityID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            TicketSeverity severity = TicketSeverities.GetTicketSeverity(ticket.Collection.LoginUser, ticketSeverityID);
            if (severity.OrganizationID != TSAuthentication.OrganizationID) return null;
            if (ticketSeverityID == ticket.TicketSeverityID || !CanEditTicket(ticket)) return null;
            ticket.TicketSeverityID = ticketSeverityID;
            ticket.Collection.Save();
            return severity.GetProxy();
        }

        [WebMethod]
        public ForumCategoryProxy SetTicketCommunity(int ticketID, int? categoryID, string oldCommunityName, string newCommunityName)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            string description = "Changed community from '" + oldCommunityName + "' to '" + newCommunityName + "'.";
            ActionLogs.AddActionLog(ticket.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticketID, description);

            if (categoryID == null || categoryID == -1)
            {
                ticket.RemoveCommunityTicket();
            }
            else
            {
                ticket.AddCommunityTicket((int)categoryID);
                return ForumCategories.GetForumCategory(ticket.Collection.LoginUser, (int)categoryID).GetProxy();
            }
            return null;
        }

        [WebMethod]
        public DateTime? SetDueDate(int ticketID, Object dueDate)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            StringBuilder fromValue = new StringBuilder();
            if (ticket.DueDate == null)
            {
                fromValue.Append("Unassigned");
            }
            else
            {
                fromValue.Append(ticket.DueDate.ToString());
            }

            StringBuilder toValue = new StringBuilder();
            if (dueDate == string.Empty)
            {
                toValue.Append("Unassigned");
                ticket.DueDate = null;
            }
            else
            {
                toValue.Append(dueDate != null ? dueDate.ToString() : string.Empty);
                ticket.DueDate = (DateTime?)dueDate;
            }

            string description = "Changed Due Date from '" + fromValue.ToString() + "' to '" + toValue.ToString() + "'.";
            ActionLogs.AddActionLog(ticket.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticketID, description);

            ticket.Collection.Save();
            return ticket.DueDate;
        }

        [WebMethod]
        public KnowledgeBaseCategoryProxy SetTicketKnowledgeBaseCategory(int ticketID, int? categoryID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            ticket.KnowledgeBaseCategoryID = categoryID;
            ticket.Collection.Save();
            if (categoryID != null)
            {
                return KnowledgeBaseCategories.GetKnowledgeBaseCategory(ticket.Collection.LoginUser, (int)categoryID).GetProxy();
            }
            return null;
        }

        [WebMethod]
        public void SetTicketsUser(string ticketIDs, int? userID)
        {
            UsersViewItem user = userID != null ? UsersView.GetUsersViewItem(TSAuthentication.GetLoginUser(), (int)userID) : null;
            if (user != null && user.OrganizationID != TSAuthentication.OrganizationID) return;


            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            tickets.LoadByTicketIDs(TSAuthentication.OrganizationID, ids);

            foreach (Ticket ticket in tickets)
            {
                if (!CanEditTicket(ticket)) continue;
                ticket.UserID = userID;
            }
            tickets.Save();
        }

        [WebMethod]
        public UserInfo SetTicketUser(int ticketID, int? userID)
        {
            try
            {
                Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
                if (!CanEditTicket(ticket)) return null;

                UsersViewItem user = userID != null ? UsersView.GetUsersViewItem(TSAuthentication.GetLoginUser(), (int)userID) : null;
                if (user != null && user.OrganizationID != TSAuthentication.OrganizationID) return null;
                ticket.UserID = userID;
                ticket.Collection.Save();
                return user == null ? null : new UserInfo(user);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "SetTicketUser");
                UsersViewItem user = userID != null ? UsersView.GetUsersViewItem(TSAuthentication.GetLoginUser(), (int)userID) : null;
                return user == null ? null : new UserInfo(user);
            }
        }

        [WebMethod]
        public void SetTicketsGroup(string ticketIDs, int? groupID)
        {
            Group group = groupID != null ? Groups.GetGroup(TSAuthentication.GetLoginUser(), (int)groupID) : null;
            if (group != null && group.OrganizationID != TSAuthentication.OrganizationID) return;


            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            tickets.LoadByTicketIDs(TSAuthentication.OrganizationID, ids);

            foreach (Ticket ticket in tickets)
            {
                if (!CanEditTicket(ticket)) continue;
                ticket.GroupID = groupID;
            }
            tickets.Save();
        }

        [WebMethod]
        public string SetTicketGroup(int ticketID, int? groupID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;

            Group group = groupID != null ? Groups.GetGroup(TSAuthentication.GetLoginUser(), (int)groupID) : null;
            if (groupID == ticket.GroupID) return null;
            if (group != null && group.OrganizationID != TSAuthentication.OrganizationID) return null;
            ticket.GroupID = groupID;
            ticket.Collection.Save();
            return group == null ? "" : group.Name;
        }

        [WebMethod]
        public void SetTicketChildrenGroup(int ticketID, int? groupID)
        {
            Tickets children = new Tickets(TSAuthentication.GetLoginUser());
            children.LoadChildren(ticketID);
            foreach (Ticket child in children)
            {
                child.GroupID = groupID;
            }
            children.Save();
        }

        [WebMethod]
        public AutocompleteItem SetProduct(int ticketID, int? productID) {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) {
                return null;
            } else {
                Product product = productID != null ? Products.GetProduct(TSAuthentication.GetLoginUser(), (int)productID) : null;
                if (productID == ticket.ProductID) {
                    return null;
                } else if (product != null && product.OrganizationID != TSAuthentication.OrganizationID) {
                    return null;
                } else {
                    ticket.ProductID = productID;
                    ticket.ReportedVersionID = null;
                    ticket.SolvedVersionID = null;
                    ticket.Collection.Save();

                    if (ticket.IsVisibleOnPortal && ticket.IsKnowledgeBase)
                    {
                        DeflectorService Deflection = new DeflectorService();
                        Deflection.IndexTicket(ticket.TicketID);
                    }

                    if (product != null) {
                        return new AutocompleteItem(product.Name, product.ProductID.ToString(), product.ProductFamilyID);
                    } else {
                        return new AutocompleteItem(null, null);
                    }
                }
            }
        }

        [WebMethod]
        public AutocompleteItem SetReportedVersion(int ticketID, int? versionID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;

            ProductVersion version = versionID != null ? ProductVersions.GetProductVersion(ticket.Collection.LoginUser, (int)versionID) : null;
            if (version != null)
            {
                Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), version.ProductID);
                if (product.OrganizationID != TSAuthentication.OrganizationID) return null;
            }

            if (versionID == ticket.ReportedVersionID) return null;
            ticket.ReportedVersionID = versionID;
            ticket.Collection.Save();
            return version == null ? new AutocompleteItem(null, null) : new AutocompleteItem(version.VersionNumber, version.ProductVersionID.ToString());
        }

        [WebMethod]
        public AutocompleteItem SetSolvedVersion(int ticketID, int? versionID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;

            ProductVersion version = versionID != null ? ProductVersions.GetProductVersion(ticket.Collection.LoginUser, (int)versionID) : null;
            if (version != null)
            {
                Product product = Products.GetProduct(TSAuthentication.GetLoginUser(), version.ProductID);
                if (product.OrganizationID != TSAuthentication.OrganizationID) return null;
            }

            if (versionID == ticket.SolvedVersionID) return null;
            ticket.SolvedVersionID = versionID;
            ticket.Collection.Save();
            return version == null ? new AutocompleteItem(null, null) : new AutocompleteItem(version.VersionNumber, version.ProductVersionID.ToString());
        }

        [WebMethod]
        public UserInfo AssignUser(int ticketID, int? userID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;

            UsersViewItem user = userID != null ? UsersView.GetUsersViewItem(ticket.Collection.LoginUser, (int)userID) : null;
            if (user != null)
            {
                if (user.OrganizationID != TSAuthentication.OrganizationID) return null;
            }
            if (ticket.UserID == userID) return null;
            ticket.UserID = userID;
            ticket.ModifierID = TSAuthentication.GetLoginUser().UserID;
            ticket.Collection.Save();
            return user == null ? null : new UserInfo(user);
        }


        private void TransferCustomFields(int ticketID, int oldTicketTypeID, int newTicketTypeID)
        {
            CustomFields oldFields = new CustomFields(TSAuthentication.GetLoginUser());
            oldFields.LoadByTicketTypeID(TSAuthentication.OrganizationID, oldTicketTypeID);

            CustomFields newFields = new CustomFields(oldFields.LoginUser);
            newFields.LoadByTicketTypeID(TSAuthentication.OrganizationID, newTicketTypeID);


            foreach (CustomField oldField in oldFields)
            {
                CustomField newField = newFields.FindByName(oldField.Name);
                if (newField != null)
                {
                    CustomValue newValue = CustomValues.GetValue(oldFields.LoginUser, newField.CustomFieldID, ticketID);
                    CustomValue oldValue = CustomValues.GetValue(oldFields.LoginUser, oldField.CustomFieldID, ticketID);

                    if (newValue != null && oldValue != null)
                    {
                        newValue.Value = oldValue.Value;
                        newValue.Collection.Save();
                    }
                }

            }
        }

        private bool CanEditTicket(Ticket ticket)
        {
            return true;
        }

        private bool CanEditAction(TeamSupport.Data.Action action)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), action.TicketID);
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            return (ticket.OrganizationID == TSAuthentication.OrganizationID && (action.CreatorID == TSAuthentication.UserID || TSAuthentication.IsSystemAdmin || user.AllowUserToEditAnyAction));
        }

        private bool CanDeleteAction(TeamSupport.Data.Action action)
        {
            return CanEditAction(action) && action.SystemActionTypeID != SystemActionType.Description;
        }

        [WebMethod]
        public bool SetActionKb(int actionID, bool isKb)
        {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            if (CanEditAction(action) || user.ChangeKBVisibility)
            {
                action.IsKnowledgeBase = isKb;
                action.Collection.Save();
            }
            return action.IsKnowledgeBase;
        }

        [WebMethod]
        public bool SetActionPortal(int actionID, bool isVisibleOnPortal)
        {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            User author = Users.GetUser(TSAuthentication.GetLoginUser(), action.CreatorID);
            if (CanEditAction(action) || user.ChangeTicketVisibility)
            {
                if (author != null)
                {
                    if (isVisibleOnPortal)
                    {
                        if (!string.IsNullOrWhiteSpace(author.Signature))
                        {
                            if (!action.Description.Contains(author.Signature.Replace(" />", ">")))
                                action.Description = action.Description + "<br/><br/>" + author.Signature;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(author.Signature))
                        {
                            if (action.Description.Contains(author.Signature.Replace(" />", ">")))
                            {
                                action.Description = action.Description.Replace("<p><br><br></p>\n" + author.Signature.Replace(" />", ">"), "").Replace("<br><br>" + author.Signature.Replace(" />", ">"), "");
                                //action.Description = action.Description.Replace("<br><br>" + author.Signature.Replace(" />", ">"), "");
                            }
                        }
                    }
                }
                action.IsVisibleOnPortal = isVisibleOnPortal;
                action.Collection.Save();
            }
            return action.IsVisibleOnPortal;
        }

        [WebMethod]
        public bool SetActionPinned(int ticketID, int actionID, bool pinned)
        {
            // When an action is pinned, all other actions need to be unpinned.
            if (pinned)
            {
                Actions actions = new Actions(TSAuthentication.GetLoginUser());
                actions.LoadByTicketID(ticketID);
                foreach (TeamSupport.Data.Action action in actions)
                {
                    if (action.ActionID == actionID)
                    {
                        action.Pinned = true;
                    }
                    else
                    {
                        action.Pinned = false;
                    }
                }
                actions.Save();
            }
            // When unpin we only need to update the requested action.
            else
            {
                TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
                action.Pinned = false;
                action.Collection.Save();
            }
            return pinned;
        }

        [WebMethod]
        public string SetJiraIssueKey(int ticketID, string jiraIssueKey)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string result = SetSyncWithJira(loginUser, ticketID, jiraIssueKey);

            return result;
        }

        [WebMethod]
        public string SetSyncWithJira(int ticketID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string result = SetSyncWithJira(loginUser, ticketID, null);

            return result;
        }

        [WebMethod]
        public bool UnSetSyncWithJira(int ticketID)
        {
            bool result = false;
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            try
            {
                CRMLinkTable crmlink = new CRMLinkTable(loginUser);
                crmlink.LoadByOrganizationID(TSAuthentication.GetOrganization(loginUser).OrganizationID);

                foreach (DataRow crmRow in crmlink.Table.Rows)
                {
                    if (crmRow["CRMType"].ToString() == "Jira")
                    {
                        TicketLinkToJira linkToJira = new TicketLinkToJira(loginUser);
                        linkToJira.LoadByTicketID(ticketID);

                        TicketLinkToJiraItemProxy ticketLinktoJiraProxy = GetLinkToJira(ticketID);
                        int crmLinkId = int.Parse(crmRow["CRMLinkID"].ToString());

                        if (ticketLinktoJiraProxy != null
                            && linkToJira.Count > 0
                            && ticketLinktoJiraProxy.CrmLinkID == linkToJira[0].CrmLinkID
                            && crmLinkId == ticketLinktoJiraProxy.CrmLinkID)
                        {
                            if (ticketLinktoJiraProxy.JiraID != null && !String.IsNullOrEmpty(ticketLinktoJiraProxy.JiraKey))
                            {
                                try
                                {
                                    string token = String.IsNullOrEmpty(crmRow["SecurityToken"].ToString()) ? crmRow["Password"].ToString() : crmRow["SecurityToken"].ToString();

                                    Jira.JiraClient jiraClient = new Jira.JiraClient(crmRow["HostName"].ToString(), crmRow["Username"].ToString(), token);
                                    Jira.IssueRef issueRef = new Jira.IssueRef();
                                    issueRef.id = ticketLinktoJiraProxy.JiraID.ToString();
                                    issueRef.key = ticketLinktoJiraProxy.JiraKey;
                                    var issue = jiraClient.LoadIssue(issueRef);

                                    if (issue != null)
                                    {
                                        var remoteLinks = jiraClient.GetRemoteLinks(issueRef);

                                        if (null != remoteLinks)
                                        {
                                            Jira.RemoteLink linkItem = remoteLinks.Where(p => p.url.Contains("ticketid=" + ticketID.ToString())).FirstOrDefault();

                                            if (linkItem != null)
                                            {
                                                jiraClient.DeleteRemoteLink(issueRef, linkItem);
                                            }
                                        }
                                    }
                                }
                                catch (Jira.JiraClientException jiraException)
                                {
                                    if (jiraException.Message.ToLower().Trim() != "could not load issue" && !jiraException.InnerException.Message.ToLower().Trim().Contains("not found"))
                                    {
                                        jiraException.Data.Add("TicketId", ticketID);
                                        jiraException.Data.Add("JiraId", ticketLinktoJiraProxy.JiraID);
                                        jiraException.Data.Add("JiraKey", ticketLinktoJiraProxy.JiraKey);
                                        throw jiraException;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Check if the exception is that the URI is invalid, if so then still delete the crmlink in TS, the customer might have deleted their instance settings, we still need to be able to delete the link in our side.
                                    if (!ex.Message.ToLower().Trim().Contains("invalid uri") && !ex.Message.ToLower().Trim().Contains("uri could not be determined"))
                                    {
                                        throw ex;
                                    }
                                }
                            }

                            linkToJira.DeleteFromDB(ticketLinktoJiraProxy.id);

                            ActionLinkToJira actionLinkToJira = new ActionLinkToJira(loginUser);
                            actionLinkToJira.LoadByTicketID(ticketID);
                            actionLinkToJira.DeleteAll();
                            actionLinkToJira.Save();

                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                ExceptionLogs.LogException(loginUser, ex, "UnSetSyncWithJira");
            }

            return result;
        }

        [WebMethod]
        public string SetTFSWorkItemID(int ticketID, string tfsWorkItemID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string result = SetSyncWithTFS(loginUser, ticketID, tfsWorkItemID);

            return result;
        }

        [WebMethod]
        public string SetSyncWithTFS(int ticketID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string result = SetSyncWithTFS(loginUser, ticketID, null);

            return result;
        }

        [WebMethod]
        public bool UnSetSyncWithTFS(int ticketID)
        {
            bool result = false;

            try
            {
                CRMLinkTable crmlink = new CRMLinkTable(TSAuthentication.GetLoginUser());
                crmlink.LoadByOrganizationIDAndCRMType(TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).OrganizationID, "TFS");

                foreach (DataRow crmRow in crmlink.Table.Rows)
                {
                    TicketLinkToTFS linkToTFS = new TicketLinkToTFS(TSAuthentication.GetLoginUser());
                    linkToTFS.LoadByTicketID(ticketID);

                    TicketLinkToTFSItemProxy ticketLinktoTFSProxy = GetLinkToTFS(ticketID);
                    int crmLinkId = int.Parse(crmRow["CRMLinkID"].ToString());

                    if (ticketLinktoTFSProxy != null
                        && linkToTFS.Count > 0
                        && ticketLinktoTFSProxy.CrmLinkID == linkToTFS[0].CrmLinkID
                        && crmLinkId == ticketLinktoTFSProxy.CrmLinkID)
                    {
                        if (ticketLinktoTFSProxy.TFSID != null && !String.IsNullOrEmpty(ticketLinktoTFSProxy.TFSTitle))
                        {
                            bool useNetworkCredentials = (bool)crmRow["UseNetworkCredentials"];
                            string hostName = crmRow["HostName"].ToString();
                            string username = crmRow["Username"].ToString();
                            string password = crmRow["Password"].ToString();
                            string securityToken = crmRow["SecurityToken"].ToString();
                            NetworkCredential netCred = new NetworkCredential(username, password);
                            string authorization = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", securityToken)));
                            string url = string.Format("{0}/_apis/wit/workitems/{1}?api-version=2.2&$expand=relations", hostName, ticketLinktoTFSProxy.TFSID);
                            string contentType = "application/json";
                            string getResult = string.Empty;

                            try
                            {
                                using (var client = new WebClient { UseDefaultCredentials = false })
                                {
                                    client.Headers.Add(HttpRequestHeader.ContentType, contentType);

                                    if (useNetworkCredentials)
                                    {
                                        client.Credentials = netCred;
                                    }
                                    else
                                    {
                                        client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authorization);
                                    }

                                    Stream stream = client.OpenRead(url);
                                    StreamReader sr = new StreamReader(stream);
                                    getResult = sr.ReadToEnd();
                                }

                                WorkItemRelations workItemRelations = JsonConvert.DeserializeObject<WorkItemRelations>(getResult);

                                //Find the position of the TeamSupport hyperlink
                                for (int i = 0; i < workItemRelations.relations.Count(); i++)
                                {
                                    if (string.Compare(workItemRelations.relations[i].rel, "Hyperlink", ignoreCase: true) == 0
                                        && workItemRelations.relations[i].url.Contains("teamsupport.com")
                                        && workItemRelations.relations[i].url.Contains(string.Format("ticketid={0}", ticketID.ToString())))
                                    {

                                        //Now we can try to delete the hyperlink
                                        using (var client = new WebClient { UseDefaultCredentials = false })
                                        {
                                            contentType = "application/json-patch+json";
                                            client.Headers.Add(HttpRequestHeader.ContentType, contentType);

                                            if (useNetworkCredentials)
                                            {
                                                client.Credentials = netCred;
                                            }
                                            else
                                            {
                                                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authorization);
                                            }

                                            Object[] patchDocument = new Object[1];
                                            patchDocument[0] = new
                                            {
                                                op = "remove",
                                                path = string.Format("/relations/{0}", i.ToString())
                                            };

                                            var patchValue = JsonConvert.SerializeObject(patchDocument);
                                            string postResult = client.UploadString(url, "PATCH", patchValue);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //Check if the exception is that the URI is invalid, maybe the workitem id does not longer exist. if so we should go ahead and delete the link in the TS db.
                            }
						}

                        linkToTFS.DeleteFromDB(ticketLinktoTFSProxy.id);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "UnSetSyncWithTFS");
            }

            return result;
        }

		[WebMethod]
		public string SetSnowWorkItemID(int ticketID, string tfsWorkItemID)
		{
			LoginUser loginUser = TSAuthentication.GetLoginUser();
			string result = SetSyncWithSnow(loginUser, ticketID, tfsWorkItemID);

			return result;
		}

		[WebMethod]
		public string SetSyncWithSnow(int ticketID)
		{
			LoginUser loginUser = TSAuthentication.GetLoginUser();
			string result = SetSyncWithSnow(loginUser, ticketID, null);

			return result;
		}

		[WebMethod]
		public bool UnSetSyncWithSnow(int ticketID)
		{
			bool result = false;

			try
			{
				CRMLinkTable crmlink = new CRMLinkTable(TSAuthentication.GetLoginUser());
				crmlink.LoadByOrganizationIDAndCRMType(TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).OrganizationID, "ServiceNow");

				foreach (DataRow crmRow in crmlink.Table.Rows)
				{
					TicketLinkToSnow linkToSnow = new TicketLinkToSnow(TSAuthentication.GetLoginUser());
					linkToSnow.LoadByTicketID(ticketID);

					TicketLinkToSnowItemProxy ticketLinktoSnowProxy = GetLinkToSnow(ticketID);
					int crmLinkId = int.Parse(crmRow["CRMLinkID"].ToString());

					if (ticketLinktoSnowProxy != null
						&& linkToSnow.Count > 0
						&& ticketLinktoSnowProxy.CrmLinkID == linkToSnow[0].CrmLinkID
						&& crmLinkId == ticketLinktoSnowProxy.CrmLinkID)
					{
						if (!string.IsNullOrEmpty(ticketLinktoSnowProxy.AppId) && !String.IsNullOrEmpty(ticketLinktoSnowProxy.Number))
						{
							string hostName = crmRow["HostName"].ToString();
							string username = crmRow["Username"].ToString();
							string password = crmRow["Password"].ToString();
							string encodedCredentials = DataUtils.GetEncodedCredentials(username, password);

							if (!hostName.EndsWith("/"))
							{
								hostName += "/";
							}

							string url = hostName + "api/now/table/incident/" + ticketLinktoSnowProxy.AppId;
							dynamic dynamicBodyData = new ExpandoObject();
							dynamicBodyData.u_url_teamsupport = string.Empty;

							try
							{
								var request = (HttpWebRequest)WebRequest.Create(url);
								request.Method = "PUT";
								request.ContentType = "application/json";
								request.Headers.Add("Authorization", "Basic " + encodedCredentials);
								var writer = new StreamWriter(request.GetRequestStream());
								writer.Write(JsonConvert.SerializeObject(dynamicBodyData));
								writer.Close();
								var response = (HttpWebResponse)request.GetResponse();
								HttpStatusCode resultStatusCode = response.StatusCode;
								Stream responseStream = response.GetResponseStream();

								if (responseStream != null)
								{
									var streamReader = new StreamReader(responseStream);
									streamReader.ReadToEnd();
								}
							}
							catch (Exception ex)
							{
								//something happened with the httprequest. Delete the link in TS still.
							}
						}

						linkToSnow.DeleteFromDB(ticketLinktoSnowProxy.Id);
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				result = false;
				ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "UnSetSyncWithSnow");
			}

			return result;
		}

		[WebMethod]
        public bool Subscribe(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return false;
            return Subscriptions.ToggleSubscription(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, ReferenceType.Tickets, ticketID);
        }

        [WebMethod]
        public void SetUserQueue(int ticketID, bool value)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
            if (value)
                TicketQueue.Enqueue(TSAuthentication.GetLoginUser(), ticketID, TSAuthentication.UserID);
            else
                TicketQueue.Dequeue(TSAuthentication.GetLoginUser(), ticketID, TSAuthentication.UserID);
        }

        [WebMethod]
        public void SetUserQueues(string ticketIDs, bool value)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            foreach (int id in ids)
            {
                SetUserQueue(id, value);
            }
        }

        [WebMethod]
        public void MoveUserQueueTickets(string ticketIDs, int insertBeforeTicketID, int userID)
        {
            List<int> ids = new List<int>(JsonConvert.DeserializeObject<int[]>(ticketIDs));


            TicketQueue queue = new TicketQueue(TSAuthentication.GetLoginUser());
            queue.LoadByUser(userID);
            int pos = 0;
            int newPos = -1;
            foreach (TicketQueueItem item in queue)
            {
                if (ids.IndexOf(item.TicketID) < 0)
                {
                    if (item.TicketID == insertBeforeTicketID)
                    {
                        newPos = pos;
                        pos += ids.Count;
                    }
                    item.Position = pos;
                    pos++;
                }
            }
            if (newPos < 0) newPos = pos;
            foreach (TicketQueueItem item in queue)
            {
                if (ids.IndexOf(item.TicketID) > -1)
                {
                    item.Position = newPos;
                    newPos++;
                }
            }

            queue.Save();
        }

        [WebMethod]
        public void Enqueue(int ticketID)
        {
            SetUserQueue(ticketID, true);
        }

        [WebMethod]
        public void Dequeue(int ticketID)
        {
            SetUserQueue(ticketID, false);
        }

        [WebMethod]
        public int GetTicketID(int ticketNumber)
        {
            Ticket ticket = Tickets.GetTicketByNumber(TSAuthentication.GetLoginUser(), ticketNumber);
            return ticket.TicketID;
        }

        [WebMethod]
        public int GetTicketNumber(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            return ticket.TicketNumber;
        }

        [WebMethod]
        public string GetTicketName(int ticketNumber)
        {
            Ticket ticket = Tickets.GetTicketByNumber(TSAuthentication.GetLoginUser(), ticketNumber);
            return ticket.Name;
        }

        [WebMethod]
        public AssetProxy[] AddTicketAsset(int ticketID, int assetID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Ticket ticket = Tickets.GetTicket(loginUser, ticketID);
            if (!CanEditTicket(ticket)) return null;

            Asset asset = Assets.GetAsset(ticket.Collection.LoginUser, assetID);
            if (asset.OrganizationID != TSAuthentication.OrganizationID) return null;
            ticket.Collection.AddAsset(assetID, ticketID);

            Organization account = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
            if (asset.Location == "1")
            {
                try
                {
                    AssetAssignmentsView assetAssignments = new AssetAssignmentsView(ticket.Collection.LoginUser);
                    if (account.AutoAssociateCustomerToTicketBasedOnAssetAssignment)
                    {
                        assetAssignments.LoadByAssetID(assetID);
                        foreach (AssetAssignmentsViewItem assetAssignment in assetAssignments)
                        {
                            if (assetAssignment.RefType != null && (ReferenceType)assetAssignment.RefType == ReferenceType.Contacts)
                            {
                                ContactsViewItem contact = ContactsView.GetContactsViewItem(ticket.Collection.LoginUser, (int)assetAssignment.ShippedTo);
                                if (contact.OrganizationParentID == TSAuthentication.OrganizationID)
                                {
                                    ticket.Collection.AddContact((int)assetAssignment.ShippedTo, ticketID);
                                }
                            }
                            else
                            {
                                Organization organization = Organizations.GetOrganization(ticket.Collection.LoginUser, (int)assetAssignment.ShippedTo);
                                if (organization.ParentID == TSAuthentication.OrganizationID)
                                {
                                    ticket.Collection.AddOrganization((int)assetAssignment.ShippedTo, ticketID);
                                }
                            }
                        }
                    }

                    if (account.AutoAssignCustomerWithAssetOnTickets)
                    {
                        assetAssignments = new AssetAssignmentsView(ticket.Collection.LoginUser);
                        assetAssignments.LoadByAssetID(assetID);

                        DateTime now = DateTime.UtcNow;

                        ContactsView contacts = new ContactsView(ticket.Collection.LoginUser);
                        contacts.LoadByTicketID(ticketID);

                        Boolean assignContact = false;

                        foreach (ContactsViewItem contact in contacts)
                        {
                            assignContact = true;

                            foreach (AssetAssignmentsViewItem assetAssignment in assetAssignments)
                            {
                                if (assetAssignment.RefType == (int)ReferenceType.Contacts && assetAssignment.ShippedTo == contact.UserID)
                                {
                                    assignContact = false;
                                }
                            }

                            if (assignContact)
                            {
                                AssetHistory assetHistory = new AssetHistory(ticket.Collection.LoginUser);
                                AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                                assetHistoryItem.AssetID = assetID;
                                assetHistoryItem.OrganizationID = ticket.OrganizationID;
                                assetHistoryItem.ActionTime = now;
                                assetHistoryItem.ActionDescription = "Asset assigned per ticket #" + ticket.TicketNumber.ToString() + " on " + now.Month.ToString() + "/" + now.Day.ToString() + "/" + now.Year.ToString();
                                assetHistoryItem.ShippedFrom = ticket.OrganizationID;
                                assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                                assetHistoryItem.ShippedTo = contact.UserID;
                                assetHistoryItem.ShippingMethod = "Other";

                                assetHistoryItem.DateCreated = now;
                                assetHistoryItem.Actor = ticket.Collection.LoginUser.UserID;
                                assetHistoryItem.RefType = (int)ReferenceType.Contacts;
                                assetHistoryItem.DateModified = now;
                                assetHistoryItem.ModifierID = ticket.Collection.LoginUser.UserID;

                                assetHistory.Save();

                                AssetAssignments assetAssignmentsInsert = new AssetAssignments(ticket.Collection.LoginUser);
                                AssetAssignment assetAssignmentInsert = assetAssignmentsInsert.AddNewAssetAssignment();

                                assetAssignmentInsert.HistoryID = assetHistoryItem.HistoryID;

                                assetAssignmentsInsert.Save();

                                string description = String.Format("{0} assigned asset to refID: {1} and refType: {2} by adding asset to Ticket #{3}.", TSAuthentication.GetUser(ticket.Collection.LoginUser).FirstLastName, contact.UserID.ToString(), ReferenceType.Contacts.ToString(), ticket.TicketNumber.ToString());
                                ActionLogs.AddActionLog(ticket.Collection.LoginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);
                            }
                        }

                        Boolean assignCompany = false;

                        Organizations organizations = new Organizations(ticket.Collection.LoginUser);
                        organizations.LoadByNotContactTicketID(ticketID);
                        foreach (Organization organization in organizations)
                        {
                            assignCompany = true;

                            foreach (AssetAssignmentsViewItem assetAssignment in assetAssignments)
                            {
                                if (assetAssignment.RefType == (int)ReferenceType.Organizations && assetAssignment.ShippedTo == organization.OrganizationID)
                                {
                                    assignCompany = false;
                                    break;
                                }
                            }

                            if (assignCompany)
                            {
                                foreach (AssetAssignmentsViewItem assetAssignment in assetAssignments)
                                {
                                    if (assetAssignment.RefType == (int)ReferenceType.Contacts && assetAssignment.ShippedTo != null)
                                    {
                                        Users users = new Users(ticket.Collection.LoginUser);
                                        users.LoadByUserID((int)assetAssignment.ShippedTo);
                                        if (users[0].OrganizationID == organization.OrganizationID)
                                        {
                                            assignCompany = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (assignCompany)
                            {
                                AssetHistory assetHistory = new AssetHistory(ticket.Collection.LoginUser);
                                AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                                assetHistoryItem.AssetID = assetID;
                                assetHistoryItem.OrganizationID = ticket.OrganizationID;
                                assetHistoryItem.ActionTime = now;
                                assetHistoryItem.ActionDescription = "Asset assigned per ticket #" + ticket.TicketNumber.ToString() + " on " + now.Month.ToString() + "/" + now.Day.ToString() + "/" + now.Year.ToString();
                                assetHistoryItem.ShippedFrom = ticket.OrganizationID;
                                assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                                assetHistoryItem.ShippedTo = organization.OrganizationID;
                                assetHistoryItem.ShippingMethod = "Other";

                                assetHistoryItem.DateCreated = now;
                                assetHistoryItem.Actor = ticket.Collection.LoginUser.UserID;
                                assetHistoryItem.RefType = (int)ReferenceType.Organizations;
                                assetHistoryItem.DateModified = now;
                                assetHistoryItem.ModifierID = ticket.Collection.LoginUser.UserID;

                                assetHistory.Save();

                                AssetAssignments assetAssignmentsInsert = new AssetAssignments(ticket.Collection.LoginUser);
                                AssetAssignment assetAssignmentInsert = assetAssignmentsInsert.AddNewAssetAssignment();

                                assetAssignmentInsert.HistoryID = assetHistoryItem.HistoryID;

                                assetAssignmentsInsert.Save();

                                string description = String.Format("{0} assigned asset to refID: {1} and refType: {2} by adding asset to Ticket #{3}.", TSAuthentication.GetUser(ticket.Collection.LoginUser).FirstLastName, organization.OrganizationID.ToString(), ReferenceType.Contacts.ToString(), ticket.TicketNumber.ToString());
                                ActionLogs.AddActionLog(ticket.Collection.LoginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //We tried to add the customers and or contacts assigned to the asset, but if something goes wrong, we just move on.
                }
            }
            else if (asset.Location == "2" && account.AutoAssignCustomerWithAssetOnTickets)
            {
                bool assignAssetFlag = false;
                int? assignedTo = null;
                DateTime now = DateTime.UtcNow;

                //here we need to assign the asset to each contact and also to each company that is not included for a contact.
                ContactsView contacts = new ContactsView(ticket.Collection.LoginUser);
                contacts.LoadByTicketID(ticketID);

                foreach (ContactsViewItem contact in contacts)
                {
                    AssetHistory assetHistory = new AssetHistory(ticket.Collection.LoginUser);
                    AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                    assetHistoryItem.AssetID = assetID;
                    assetHistoryItem.OrganizationID = ticket.OrganizationID;
                    assetHistoryItem.ActionTime = now;
                    assetHistoryItem.ActionDescription = "Asset assigned per ticket #" + ticket.TicketNumber.ToString() + " on " + now.Month.ToString() + "/" + now.Day.ToString() + "/" + now.Year.ToString();
                    assetHistoryItem.ShippedFrom = ticket.OrganizationID;
                    assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                    assetHistoryItem.ShippedTo = contact.UserID;
                    assetHistoryItem.ShippingMethod = "Other";

                    assetHistoryItem.DateCreated = now;
                    assetHistoryItem.Actor = ticket.Collection.LoginUser.UserID;
                    assetHistoryItem.RefType = (int)ReferenceType.Contacts;
                    assetHistoryItem.DateModified = now;
                    assetHistoryItem.ModifierID = ticket.Collection.LoginUser.UserID;

                    assetHistory.Save();

                    AssetAssignments assetAssignments = new AssetAssignments(ticket.Collection.LoginUser);
                    AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                    assetAssignment.HistoryID = assetHistoryItem.HistoryID;

                    assetAssignments.Save();

                    string description = String.Format("{0} assigned asset to refID: {1} and refType: {2} by adding asset to Ticket #{3}.", TSAuthentication.GetUser(ticket.Collection.LoginUser).FirstLastName, contact.UserID.ToString(), ReferenceType.Contacts.ToString(), ticket.TicketNumber.ToString());
                    ActionLogs.AddActionLog(ticket.Collection.LoginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);

                    assignAssetFlag = true;
                    assignedTo = contact.UserID;
                }

                Organizations organizations = new Organizations(ticket.Collection.LoginUser);
                organizations.LoadByNotContactTicketID(ticketID);
                foreach (Organization organization in organizations)
                {
                    AssetHistory assetHistory = new AssetHistory(ticket.Collection.LoginUser);
                    AssetHistoryItem assetHistoryItem = assetHistory.AddNewAssetHistoryItem();

                    assetHistoryItem.AssetID = assetID;
                    assetHistoryItem.OrganizationID = ticket.OrganizationID;
                    assetHistoryItem.ActionTime = now;
                    assetHistoryItem.ActionDescription = "Asset assigned per ticket #" + ticket.TicketNumber.ToString() + " on " + now.Month.ToString() + "/" + now.Day.ToString() + "/" + now.Year.ToString();
                    assetHistoryItem.ShippedFrom = ticket.OrganizationID;
                    assetHistoryItem.ShippedFromRefType = (int)ReferenceType.Organizations;
                    assetHistoryItem.ShippedTo = organization.OrganizationID;
                    assetHistoryItem.ShippingMethod = "Other";

                    assetHistoryItem.DateCreated = now;
                    assetHistoryItem.Actor = ticket.Collection.LoginUser.UserID;
                    assetHistoryItem.RefType = (int)ReferenceType.Organizations;
                    assetHistoryItem.DateModified = now;
                    assetHistoryItem.ModifierID = ticket.Collection.LoginUser.UserID;

                    assetHistory.Save();

                    AssetAssignments assetAssignments = new AssetAssignments(ticket.Collection.LoginUser);
                    AssetAssignment assetAssignment = assetAssignments.AddNewAssetAssignment();

                    assetAssignment.HistoryID = assetHistoryItem.HistoryID;

                    assetAssignments.Save();

                    string description = String.Format("{0} assigned asset to refID: {1} and refType: {2} by adding asset to Ticket #{3}.", TSAuthentication.GetUser(ticket.Collection.LoginUser).FirstLastName, organization.OrganizationID.ToString(), ReferenceType.Contacts.ToString(), ticket.TicketNumber.ToString());
                    ActionLogs.AddActionLog(ticket.Collection.LoginUser, ActionLogType.Update, ReferenceType.Assets, assetID, description);

                    assignAssetFlag = true;
                    assignedTo = organization.OrganizationID;
                }

                if (assignAssetFlag)
                {
                    asset.Location = "1";
                    asset.AssignedTo = assignedTo;
                    asset.DateModified = now;
                    asset.ModifierID = ticket.Collection.LoginUser.UserID;
                    asset.Collection.Save();
                }
            }

            Assets assets = new Assets(ticket.Collection.LoginUser);
            assets.LoadByTicketID(ticketID);
            return assets.GetAssetProxies();
        }

        [WebMethod]
        public AssetProxy[] RemoveTicketAsset(int ticketID, int assetID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            ticket.Collection.RemoveAsset(assetID, ticketID);
            Assets assets = new Assets(ticket.Collection.LoginUser);
            assets.LoadByTicketID(ticketID);
            if (assets.IsEmpty) return null;
            return assets.GetAssetProxies();
        }

        [WebMethod]
        public TicketCustomer[] AddTicketCustomer(int ticketID, string type, int id)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            if (type == "u")
            {
                ContactsViewItem contact = ContactsView.GetContactsViewItem(TSAuthentication.GetLoginUser(), id);
                if (contact.OrganizationParentID != TSAuthentication.OrganizationID) return null;
                ticket.Collection.AddContact(id, ticketID);
            }
            else
            {
                Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), id);
                if (organization.ParentID != TSAuthentication.OrganizationID) return null;
                ticket.Collection.AddOrganization(id, ticketID);
            }

            return GetTicketCustomers(ticketID);
        }

        [WebMethod]
        public TicketCustomer GetTicketCustomer(string type, int id)
        {
            if (type == "u")
            {
                ContactsViewItem contact = ContactsView.GetContactsViewItem(TSAuthentication.GetLoginUser(), id);
                if (contact.OrganizationParentID != TSAuthentication.OrganizationID) return null;
                TicketCustomer customer = new TicketCustomer();
                customer.Company = contact.Organization;
                customer.OrganizationID = contact.OrganizationID;
                customer.Contact = contact.FirstName + " " + contact.LastName;
                customer.UserID = contact.UserID;
                if (!(bool)contact.OrganizationActive || !contact.IsActive || contact.OrganizationSAExpirationDateUtc < DateTime.UtcNow)
                {
                    customer.Flag = true;
                }
                return customer;
            }
            else
            {
                Organization organization = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), id);
                if (organization.ParentID != TSAuthentication.OrganizationID) return null;
                TicketCustomer customer = new TicketCustomer();
                customer.Company = organization.Name;
                customer.OrganizationID = organization.OrganizationID;
                customer.UserID = null;
                if (!organization.IsActive || organization.SAExpirationDateUtc < DateTime.UtcNow)
                {
                    customer.Flag = true;
                }
                return customer;
            }

        }

        [WebMethod]
        public TicketCustomer GetChatCustomer(int chatID)
        {
            Chat chat = Chats.GetChat(TSAuthentication.GetLoginUser(), chatID);
            if (chat == null || chat.OrganizationID != TSAuthentication.OrganizationID) return null;

            int initiatorUserId = chat.GetInitiatorLinkedUserID();

            if (initiatorUserId <= 0)
            {
                ChatClients clients = new ChatClients(TSAuthentication.GetLoginUser());
                clients.LoadByChatClientID(chat.InitiatorID);

                if (clients != null && clients.Any() && !string.IsNullOrEmpty(clients[0].Email))
                {
                    string emailDomain = clients[0].Email.Substring(clients[0].Email.LastIndexOf('@') + 1).Trim();
                    Organization company = Organization.GetCompanyByDomain(chat.OrganizationID, emailDomain, TSAuthentication.GetLoginUser());

                    if (company != null && company.Name.ToLower().Trim() == clients[0].CompanyName.ToLower().Trim())
                    {
                        try
                        {
                            Users users = new Users(TSAuthentication.GetLoginUser());
                            User user = users.AddNewUser();
                            user.OrganizationID = company.OrganizationID;
                            user.Email = clients[0].Email.Trim();
                            user.FirstName = clients[0].FirstName;
                            user.LastName = clients[0].LastName;

                            user.ActivatedOn = DateTime.UtcNow;
                            user.LastLogin = DateTime.UtcNow;
                            user.LastActivity = DateTime.UtcNow.AddHours(-1);
                            user.IsPasswordExpired = true;
                            user.ReceiveTicketNotifications = true;
                            user.IsActive = true;
                            user.NeedsIndexing = true;
                            user.Collection.Save();

                            initiatorUserId = user.UserID;

                            clients[0].LinkedUserID = user.UserID;
                            clients.Save();
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "TicketService.GetChatCustomer");
                        }
                    }
                }
            }

            return GetTicketCustomer("u", initiatorUserId);
        }

        [WebMethod]
        public TicketCustomer[] RemoveTicketContact(int ticketID, int userID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            ticket.Collection.RemoveContact(userID, ticketID);
            return GetTicketCustomers(ticketID);
        }

        [WebMethod]
        public TicketCustomer[] RemoveTicketCompany(int ticketID, int organizationID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            ticket.Collection.RemoveOrganization(organizationID, ticketID);
            return GetTicketCustomers(ticketID);
        }

        [WebMethod]
        public string AdminGetTicketByNumber(int orgID, int number)
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            Ticket ticket = Tickets.GetTicketByNumber(TSAuthentication.GetLoginUser(), orgID, number);
            if (ticket == null) return null;
            return AdminGetTicketByID(ticket.TicketID);
        }

        [WebMethod]
        public string AdminGetTicketByID(int ticketID)
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            SqlCommand command = new SqlCommand();

            command.CommandText = @"
SELECT
CAST((SELECT SUM(DATALENGTH(Description)) FROM Actions WHERE TicketID = t.TicketID) / 1024 AS Varchar) + ' KB' AS [Size],
t.*
FROM TicketsView t
WHERE t.TicketID = @TicketID
";
            command.Parameters.AddWithValue("TicketID", ticketID);
            var expando = SqlExecutor.GetExpandoObject(TSAuthentication.GetLoginUser(), command)[0];
            return JsonConvert.SerializeObject(expando);
        }

        [WebMethod]
        public void AdminProcessEmails(int ticketID)
        {
            if (TSAuthentication.OrganizationID != 1078) return;
            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE EmailPosts SET HoldTime = 0 WHERE Param1=@Param1";
            command.Parameters.AddWithValue("@Param1", ticketID);
            SqlExecutor.ExecuteNonQuery(TSAuthentication.GetLoginUser(), command);
        }

        [WebMethod]
        public ActionsViewItemProxy[] AdminGetActions(int ticketID)
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            ActionsView actions = new ActionsView(TSAuthentication.GetLoginUser());
            actions.LoadByTicketID(ticketID);
            return actions.GetActionsViewItemProxies();
        }

        [WebMethod]
        public void AdminCleanAction(int actionID)
        {
            if (TSAuthentication.OrganizationID != 1078) return;
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
            action.Collection.isAdminClean = true;
            action.Collection.Save();
        }

        [WebMethod]
        public void AdminCleanAllActions(int ticketID)
        {
            if (TSAuthentication.OrganizationID != 1078) return;
            Actions actions = new Actions(TSAuthentication.GetLoginUser());
            actions.LoadByTicketID(ticketID);
            foreach (TeamSupport.Data.Action action in actions)
            {
                action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
            }
            actions.isAdminClean = true;
            actions.Save();
        }

        [WebMethod]
        public void AdminCleanAllEmailActions(int ticketID)
        {
            if (TSAuthentication.OrganizationID != 1078) return;
            Actions actions = new Actions(TSAuthentication.GetLoginUser());
            actions.LoadByTicketID(ticketID);
            foreach (TeamSupport.Data.Action action in actions)
            {
                if (action.SystemActionTypeID == SystemActionType.Email)
                    action.Description = HtmlUtility.RemoveInvalidHtmlTags(action.Description);
            }
            actions.Save();
        }
        /*
        public void UpdateUserTicketStatusIsFlagged(LoginUser loginUser, int[] ticketIDs, int userID, bool value)
        {
          UserTicketStatuses statuses = new UserTicketStatuses(loginUser);
          statuses.LoadByTicketIDs(userID, ticketIDs);
          foreach (UserTicketStatus status in statuses)
          {
            status.IsFlagged = value;
          }
          statuses.Save();
        }

        public void UpdateUserTicketStatusDateRead(LoginUser loginUser, int[] ticketIDs, int userID, DateTime dateRead)
        {
          UserTicketStatuses statuses = new UserTicketStatuses(loginUser);
          statuses.LoadByTicketIDs(userID, ticketIDs);
          foreach (UserTicketStatus status in statuses)
          {
            status.DateRead = dateRead;
          }
          statuses.Save();
        }
           */

        [WebMethod]
        public void MarkTicketAsRead(int ticketID)
        {
            try {
                UserTicketStatus uts = UserTicketStatuses.GetUserTicketStatus(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, ticketID);
                Ticket ticket = Tickets.GetTicket(uts.Collection.LoginUser, ticketID);
                if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
                uts.DateRead = DateTime.UtcNow;
                uts.Collection.Save();
            }
            catch (SqlException sqlEx) { }
            catch (Exception ex) { }
        }

        [WebMethod]
        public void SetTicketReads(string ticketIDs, bool value)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            UserTicketStatuses statuses = new UserTicketStatuses(TSAuthentication.GetLoginUser());
            statuses.LoadByTicketIDs(TSAuthentication.UserID, ids);
            List<int> leftOvers = new List<int>(ids);
            foreach (UserTicketStatus status in statuses)
            {
                status.DateRead = value ? DateTime.UtcNow : new DateTime(2000, 1, 1);
                leftOvers.Remove(status.TicketID);
            }
            statuses.Save();
            statuses = new UserTicketStatuses(TSAuthentication.GetLoginUser());

            foreach (int id in leftOvers)
            {
                UserTicketStatus status = statuses.AddNewUserTicketStatus();
                status.DateRead = value ? DateTime.UtcNow : new DateTime(2000, 1, 1);
                status.IsFlagged = false;
                status.UserID = TSAuthentication.UserID;
                status.TicketID = id;
            }
            statuses.Save();

        }

        [WebMethod]
        public void SetTicketRead(int ticketID, bool value)
        {
            UserTicketStatus uts = UserTicketStatuses.GetUserTicketStatus(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, ticketID);
            Ticket ticket = Tickets.GetTicket(uts.Collection.LoginUser, ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
            uts.DateRead = value ? DateTime.UtcNow : new DateTime(2000, 1, 1);
            uts.Collection.Save();
        }

        [WebMethod]
        public void SetTicketFlag(int ticketID, bool value)
        {
            UserTicketStatus uts = UserTicketStatuses.GetUserTicketStatus(TSAuthentication.GetLoginUser(), TSAuthentication.UserID, ticketID);
            Ticket ticket = Tickets.GetTicket(uts.Collection.LoginUser, ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return;
            uts.IsFlagged = value;
            uts.Collection.Save();
        }

        [WebMethod]
        public void SetTicketFlags(string ticketIDs, bool value)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            UserTicketStatuses statuses = new UserTicketStatuses(TSAuthentication.GetLoginUser());
            statuses.LoadByTicketIDs(TSAuthentication.UserID, ids);
            List<int> leftOvers = new List<int>(ids);
            foreach (UserTicketStatus status in statuses)
            {
                status.IsFlagged = value;
                leftOvers.Remove(status.TicketID);
            }
            statuses.Save();
            statuses = new UserTicketStatuses(TSAuthentication.GetLoginUser());

            foreach (int id in leftOvers)
            {
                UserTicketStatus status = statuses.AddNewUserTicketStatus();
                status.DateRead = new DateTime(2000, 1, 1);
                status.IsFlagged = value;
                status.UserID = TSAuthentication.UserID;
                status.TicketID = id;
            }

            statuses.Save();
        }


        [WebMethod]
        public void SetTicketVisibility(string ticketIDs, bool value)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);

            foreach (int ticketID in ids)
            {
                SetIsVisibleOnPortal(ticketID, value);
            }

        }

        [WebMethod]
        public void SetTicketSubcribes(string ticketIDs, bool value)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(ticketIDs);
            foreach (int id in ids)
            {
                SetSubscribed(id, value, null);
            }
        }

        [WebMethod]
        public UserInfo[] SetSubscribed(int ticketID, bool value, int? userID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
            UsersViewItem user = UsersView.GetUsersViewItem(ticket.Collection.LoginUser, userID == null ? TSAuthentication.UserID : (int)userID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID || user.OrganizationID != TSAuthentication.OrganizationID) return null;
            if (!value) Subscriptions.RemoveSubscription(ticket.Collection.LoginUser, user.UserID, ReferenceType.Tickets, ticketID);
            else Subscriptions.AddSubscription(ticket.Collection.LoginUser, user.UserID, ReferenceType.Tickets, ticketID);
            return GetSubscribers(ticket);
        }

        [WebMethod]
        public UserInfo[] SetQueue(int ticketID, bool value, int? userID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
            UsersViewItem user = UsersView.GetUsersViewItem(ticket.Collection.LoginUser, userID == null ? TSAuthentication.UserID : (int)userID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID || user.OrganizationID != TSAuthentication.OrganizationID) return null;
            if (!value) TicketQueue.Dequeue(user.Collection.LoginUser, ticketID, user.UserID);
            else TicketQueue.Enqueue(user.Collection.LoginUser, ticketID, user.UserID);
            return GetQueuers(ticket);
        }

        [WebMethod]
        public void EmailTicket(int ticketID, string addresses, string introduction)
        {
            addresses = addresses.Length > 200 ? addresses.Substring(0, 200) : addresses;
            EmailPosts posts = new EmailPosts(TSAuthentication.GetLoginUser());
            EmailPost post = posts.AddNewEmailPost();
            post.EmailPostType = EmailPostType.TicketSendEmail;
            post.HoldTime = 0;

            post.Param1 = TSAuthentication.UserID.ToString();
            post.Param2 = ticketID.ToString();
            post.Param3 = addresses;
            post.Text1 = introduction;
            posts.Save();
        }

        [WebMethod]
        public TicketsViewItemProxy[] GetContactTickets(int userID)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            tickets.LoadByContactID(userID, "DateModified DESC");
            return tickets.GetTicketsViewItemProxies();
        }

        [WebMethod]
        public TicketsViewItemProxy[] Load5MostRecentByOrgID(int orgID)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            tickets.LoadLatest5Tickets(orgID);
            return tickets.GetTicketsViewItemProxies();
        }

        [WebMethod]
        public TicketsViewItemProxy[] Load5MostRecentByOrgID2(int orgID, bool includeChildren)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            tickets.LoadLatest5Tickets(orgID, includeChildren);
            return tickets.GetTicketsViewItemProxies();
        }

        [WebMethod]
        public TicketsViewItemProxy[] Load5MostRecentByProductID(int productID)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            tickets.LoadLatest5ProductTickets(productID);
            return tickets.GetTicketsViewItemProxies();
        }

        [WebMethod]
        public TicketsViewItemProxy[] Load5MostRecentByProductVersionID(int productVersionID)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            tickets.LoadLatest5ProductVersionTickets(productVersionID);
            return tickets.GetTicketsViewItemProxies();
        }

        [WebMethod]
        public TicketsViewItemProxy[] Load5MostRecentByProductFamilyID(int productFamilyID)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            tickets.LoadLatest5ProductFamilyTickets(productFamilyID);
            return tickets.GetTicketsViewItemProxies();
        }

        [WebMethod]
        public TicketsViewItemProxy[] Load5MostRecentByContactID(int userID)
        {
            TicketsView tickets = new TicketsView(TSAuthentication.GetLoginUser());
            tickets.Load5MostRecentByContactID(userID, "DateModified DESC");
            return tickets.GetTicketsViewItemProxies();
        }

        [WebMethod]
        public string[] CreateDummyTickets()
        {

            List<string> result = new List<string>();
            LoginUser loginuser = TSAuthentication.GetLoginUser();

            for (int i = 0; i < 500; i++)
            {
                try
                {
                    DateTime start = DateTime.UtcNow;
                    Ticket ticket = (new Tickets(loginuser).AddNewTicket());
                    ticket.OrganizationID = 363655;
                    ticket.Name = "Dummy Ticket " + i.ToString();
                    ticket.TicketSeverityID = 9212;
                    ticket.TicketStatusID = 50658;
                    ticket.TicketTypeID = 9743;
                    ticket.Collection.Save();


                    TeamSupport.Data.Action action = (new Actions(loginuser)).AddNewAction();
                    action.TicketID = ticket.TicketID;
                    action.SystemActionTypeID = SystemActionType.Description;
                    action.Description = "Description";
                    action.Collection.Save();

                    for (int j = 0; j < 5; j++)
                    {
                        action = (new Actions(loginuser)).AddNewAction();
                        action.TicketID = ticket.TicketID;
                        action.SystemActionTypeID = SystemActionType.Custom;
                        action.ActionTypeID = 4014;
                        action.Description = "Action " + j.ToString();
                        action.Collection.Save();
                    }
                    double time = DateTime.UtcNow.Subtract(start).TotalMilliseconds;
                    result.Add(time.ToString());
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 1205)
                    {
                        result.Add("DeadLock!");
                        throw sqlEx;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(loginuser, ex, "Dummy Tickets");
                    result.Add(ex.ToString());
                    return result.ToArray();
                }
            }
            return result.ToArray();
        }

        [WebMethod]
        public string[] ReadDummyTickets()
        {
            DateTime start = DateTime.UtcNow;
            List<string> result = new List<string>();

            Tickets tickets = new Tickets(TSAuthentication.GetLoginUser());
            tickets.LoadByOrganizationID(363655);
            double time = DateTime.UtcNow.Subtract(start).TotalMilliseconds;
            result.Add(time.ToString());
            return result.ToArray();
        }

        [WebMethod]
        public ActionLogProxy[] GetTicketHistory(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            ActionLogs logs = new ActionLogs(ticket.Collection.LoginUser);

            logs.LoadByTicketID(ticketID);
            List<ActionLogProxy> logsArray = logs.GetActionLogProxies().ToList();

            foreach (var log in logsArray.Where(p => p.CreatorID < 0))
            {
                switch (log.CreatorID)
                {
                    case (int)SystemUser.API:
                        log.CreatorName = SystemUser.API.ToString();
                        break;
                    case (int)SystemUser.CRM:
                        //hack: vv. only way right now to know if the actionlog was added by Jira. We'll need to do something about this (creating a user for each service or crm)
                        //		As of right now March 2016, this is the only error logged from Jira.vb UpdateTicketWithIssueData(), so we specifically check that text in the Description,
                        //		we need to update this as needed if something there (or other integrations are handled here) changes
                        if (log.OrganizationID == null && log.Description.ToLower().StartsWith("updated ticket with jira issue key:"))
                        {
                            log.CreatorName = IntegrationType.Jira.ToString() + " Integration";
                        }
						else if(log.OrganizationID == null && log.Description.ToLower().StartsWith("updated ticket with tfs work item title:"))
						{
							log.CreatorName = IntegrationType.TFS.ToString() + " Integration";
						}
						else if (log.OrganizationID == null && log.Description.ToLower().StartsWith("updated ticket with the incident"))
						{
							log.CreatorName = IntegrationType.ServiceNow.ToString() + " Integration";
						}
						break;
                }
            }

            //TODO: vv. Right now we are only pulling and displaying in the Ticket history the Jira Integration changes. Later we'll have to add the others, here and for the Company/Contact pages.
            CRMLinkErrors integrationErrors = new CRMLinkErrors(ticket.Collection.LoginUser);
			List<IntegrationType> crmType = new List<IntegrationType>();
			crmType.Add(IntegrationType.Jira);
			crmType.Add(IntegrationType.TFS);

			integrationErrors.LoadByTicketID(ticketID, IntegrationType.Jira.ToString(), isCleared: false);

            if (integrationErrors != null && integrationErrors.Any() && integrationErrors.Count > 0)
            {
                //Get the IntegrationLogs, then translate them to a List<ActionLogProxy>
                List<ActionLogProxy> logsIntegrationArray = integrationErrors.TranslateToActionLog();
                logsArray.AddRange(logsIntegrationArray);
            }

            return logsArray.OrderByDescending(o => o.DateCreated).ToArray();
        }

        [WebMethod]
        public TicketInfo GetTicketInfo(int ticketNumber)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItemByNumber(TSAuthentication.GetLoginUser(), ticketNumber);
            if (ticket == null) return null;
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;

            User user = Users.GetUser(ticket.Collection.LoginUser, TSAuthentication.UserID);
            if (!ticket.UserHasRights(user)) return null;

            MarkTicketAsRead(ticket.TicketID);

            TicketInfo info = new TicketInfo();
            info.Ticket = ticket.GetProxy();

            if (info.Ticket.Name.ToLower() == "<no subject>")
                info.Ticket.Name = "";

            if (info.Ticket.CategoryName != null && info.Ticket.ForumCategory != null)
                info.Ticket.CategoryDisplayString = ForumCategories.GetCategoryDisplayString(TSAuthentication.GetLoginUser(), (int)info.Ticket.ForumCategory);

            if (info.Ticket.KnowledgeBaseCategoryName != null && info.Ticket.KnowledgeBaseCategoryID != null)
                info.Ticket.KnowledgeBaseCategoryDisplayString = KnowledgeBaseCategories.GetKnowledgeBaseCategoryDisplayString(TSAuthentication.GetLoginUser(), (int)info.Ticket.KnowledgeBaseCategoryID);
            info.Customers = GetTicketCustomers(ticket.TicketID);
            info.Related = GetRelatedTickets(ticket.TicketID);
            info.Tags = GetTicketTags(ticket.TicketID);
            info.CustomValues = GetParentCustomValues(ticket.TicketID);
            info.Subscribers = GetSubscribers(ticket);
            info.Queuers = GetQueuers(ticket);

            Reminders reminders = new Reminders(ticket.Collection.LoginUser);
            reminders.LoadByItem(ReferenceType.Tickets, ticket.TicketID, TSAuthentication.UserID);
            info.Reminders = reminders.GetReminderProxies();

            Assets assets = new Assets(ticket.Collection.LoginUser);
            assets.LoadByTicketID(ticket.TicketID);
            info.Assets = assets.GetAssetProxies();

            Actions actions = new Actions(ticket.Collection.LoginUser);
            actions.LoadByTicketID(ticket.TicketID);

            List<ActionInfo> actionInfos = new List<ActionInfo>();
            actions.Table.Columns["Description"].AllowDBNull = true;

            for (int i = 0; i < actions.Count; i++)
            {
                ActionInfo actionInfo = GetActionInfo(ticket.Collection.LoginUser, actions[i], i < 10 || actions[i].SystemActionTypeID == SystemActionType.Description || actions[i].Pinned);
                actionInfos.Add(actionInfo);
            }

            info.Actions = actionInfos.ToArray();
            info.LinkToJira = GetLinkToJira(ticket.TicketID);
            info.LinkToTFS = GetLinkToTFS(ticket.TicketID);
			info.LinkToSnow = GetLinkToSnow(ticket.TicketID);

            return info;
        }

        [WebMethod]
        public UsersViewItemProxy GetTicketLastSender(int ticketID)
        {
            UsersView sender = new UsersView(TSAuthentication.GetLoginUser());
            sender.LoadLastSenderByTicketNumber(TSAuthentication.OrganizationID, ticketID);
            UsersViewItemProxy result = null;
            if (sender.Count > 0)
            {
                result = sender[0].GetProxy();
            }
            return result;
        }

        private ActionInfo GetActionInfo(LoginUser loginUser, TeamSupport.Data.Action action)
        {
            return GetActionInfo(loginUser, action, true);
        }

        private ActionInfo GetActionInfo(LoginUser loginUser, TeamSupport.Data.Action action, bool includeDescription)
        {
            ActionInfo actionInfo = new ActionInfo();
            actionInfo.Action = action.GetProxy();
            if (!includeDescription) actionInfo.Action.Description = null;
            if (actionInfo.Action.Description != null)
            {
                actionInfo.Action.Description = actionInfo.Action.Description + "&nbsp;";
                //actionInfo.Action.Description = HtmlUtility.TagHtml(TSAuthentication.GetLoginUser(), HtmlUtility.Sanitize(HtmlUtility.CheckScreenR(loginUser, actionInfo.Action.Description)));
                actionInfo.Action.Description = HtmlUtility.CheckScreenR(loginUser, actionInfo.Action.Description);
            }

            UsersViewItem creator = UsersView.GetUsersViewItem(loginUser, action.CreatorID);
            if (creator != null) actionInfo.Creator = new UserInfo(creator);

            actionInfo.Attachments = Model_API.Read<AttachmentProxy[]>(action.ActionID);

            return actionInfo;
        }

        [WebMethod]
        public ActionInfo GetActionInfo(int actionID)
        {
            TeamSupport.Data.Action action = Actions.GetActionByID(TSAuthentication.GetLoginUser(), actionID);
            Ticket ticket = Tickets.GetTicket(action.Collection.LoginUser, action.TicketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            return GetActionInfo(action.Collection.LoginUser, action);
        }

        [WebMethod]
        public CustomValueProxy[] GetCustomValues(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            CustomValues values = new CustomValues(ticket.Collection.LoginUser);
            values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets, ticket.TicketTypeID, ticket.TicketID);
            return values.GetCustomValueProxies();
        }

        [WebMethod]
        public CustomValueProxy[] GetParentCustomValues(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            CustomValues values = new CustomValues(ticket.Collection.LoginUser);
            values.LoadParentsByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets, ticket.TicketTypeID, ticket.TicketID, ticket.ProductID);
            return values.GetCustomValueProxies();
        }

        [WebMethod]
        public TagProxy[] GetTicketTags(int ticketID)
        {
            Tags tags = new Tags(TSAuthentication.GetLoginUser());
            tags.LoadByReference(ReferenceType.Tickets, ticketID);
            return tags.GetTagProxies();
        }

        [WebMethod]
        public TagProxy[] AddTag(int ticketID, string value)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            value = value.Trim();
            if (value == "") return null;
            Tag tag = Tags.GetTag(ticket.Collection.LoginUser, value);
            if (tag == null) {
                Tags tags = new Tags(ticket.Collection.LoginUser);
                tag = tags.AddNewTag();
                tag.OrganizationID = TSAuthentication.OrganizationID;
                tag.Value = value;
                tags.Save();
            }

            TagLink link = TagLinks.GetTagLink(ticket.Collection.LoginUser, ReferenceType.Tickets, ticketID, tag.TagID);
            if (link == null) {
                TagLinks links = new TagLinks(ticket.Collection.LoginUser);
                link = links.AddNewTagLink();
                link.RefType = ReferenceType.Tickets;
                link.RefID = ticketID;
                link.TagID = tag.TagID;
                links.Save();
                ticket.Collection.AddTags(tag, ticketID);
            }


                // DEFLECTOR.
                if (ticket.IsVisibleOnPortal && ticket.IsKnowledgeBase)
                {
                    var item = new DeflectorItem
                    {
                        TicketID = ticket.TicketID,
                        Name = ticket.Name,
                        OrganizationID = ticket.OrganizationID,
                        ProductID = ticket.ProductID,
                        TagID = tag.TagID,
                        Value = tag.Value
                    };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                    DeflectorService Deflection = new DeflectorService();
                    Deflection.IndexDeflector(json);
                }

            return GetTicketTags(ticketID);
        }

        [WebMethod]
        public TagProxy[] RemoveTag(int ticketID, int tagID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (!CanEditTicket(ticket)) return null;
            TagLink link = TagLinks.GetTagLink(TSAuthentication.GetLoginUser(), ReferenceType.Tickets, ticketID, tagID);
            Tag tag = Tags.GetTag(TSAuthentication.GetLoginUser(), tagID);
            string tagValue = tag.Value;
            ticket.Collection.RemoveTags(tag, ticketID);
            int count = tag.GetLinkCount();
            link.Delete();
            link.Collection.Save();
            if (count < 2) {
                tag.Delete();
                tag.Collection.Save();
            }

            // DEFLECTOR
            DeflectorService Deflection = new DeflectorService();
            Deflection.RemoveTag(ticket.OrganizationID, ticketID, tagValue);

            return GetTicketTags(ticketID);
        }

        private bool IsTicketRelated(Ticket ticket1, Ticket ticket2)
        {
            if (ticket1.ParentID != null && ticket1.ParentID == ticket2.TicketID) return true;
            if (ticket2.ParentID != null && ticket2.ParentID == ticket1.TicketID) return true;
            TicketRelationship item = TicketRelationships.GetTicketRelationship(ticket1.Collection.LoginUser, ticket1.TicketID, ticket2.TicketID);
            return item != null;
        }

        [WebMethod]
        public RelatedTicket[] AddRelated(int ticketID1, int ticketID2, bool? isTicket1Parent)
        {
            if (ticketID1 == ticketID2) return null;

            Ticket ticket1 = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID1);
            Ticket ticket2 = Tickets.GetTicket(ticket1.Collection.LoginUser, ticketID2);

            if (!CanEditTicket(ticket1)) return null;

            if (isTicket1Parent == null) // just related
            {

                if (IsTicketRelated(ticket1, ticket2))
                {
                    throw new Exception("The ticket is already associated.");
                }

                TicketRelationship item = (new TicketRelationships(ticket1.Collection.LoginUser)).AddNewTicketRelationship();
                item.OrganizationID = TSAuthentication.OrganizationID;
                item.Ticket1ID = ticketID1;
                item.Ticket2ID = ticketID2;
                item.Collection.Save();

                string description = "Ticket " + ticket2.TicketNumber + " related";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket1.TicketID, description);
                description = "Ticket " + ticket1.TicketNumber + " related";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket2.TicketID, description);
            }
            else if (isTicket1Parent == true) // parent
            {
                if (ticket2.ParentID != null)
                {
                    if (ticket1.ParentID == ticket2.TicketID) return null;
                    throw new Exception("Ticket " + ticket2.TicketNumber + " is already a child of another ticket.");
                }

                if (ticket1.ParentID == ticket2.TicketID)
                {
                    throw new Exception("My Error");
                }

                TicketRelationship item = TicketRelationships.GetTicketRelationship(ticket1.Collection.LoginUser, ticketID1, ticketID2);
                if (item != null)
                {
                    item.Delete();
                    item.Collection.Save();
                }

                ticket2.ParentID = ticket1.TicketID;
                ticket2.Collection.Save();

                string description = "Ticket " + ticket2.TicketNumber + " added as child";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket1.TicketID, description);
                description = "Ticket " + ticket1.TicketNumber + " added as parent";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket2.TicketID, description);
            }
            else // child
            {
                if (ticket1.ParentID != null && ticket1.ParentID == ticket2.TicketID) return null;
                if (ticket2.ParentID == ticket1.TicketID) return null;
                TicketRelationship item = TicketRelationships.GetTicketRelationship(ticket1.Collection.LoginUser, ticketID1, ticketID2);
                if (item != null)
                {
                    item.Delete();
                    item.Collection.Save();
                }

                ticket1.ParentID = ticket2.TicketID;
                ticket1.Collection.Save();

                string description = "Ticket " + ticket2.TicketNumber + " added as parent";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket1.TicketID, description);
                description = "Ticket " + ticket1.TicketNumber + " added as child";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket2.TicketID, description);
            }
            return GetRelatedTickets(ticket1.TicketID);

        }

        [WebMethod]
        public int[] GetCustomerProductIDs(string customerIDs)
        {
            int[] list = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(customerIDs);
            if (list.Length < 1) return null;

            Products products = new Products(TSAuthentication.GetLoginUser());
            products.LoadByCustomerIDs(list);

            if (products.IsEmpty) return null;
            List<int> productIDs = new List<int>();
            foreach (Product product in products)
            {
                productIDs.Add(product.ProductID);
            }

            return productIDs.ToArray();

        }

        [WebMethod]
        public int[] GetTicketCustomerProductIDs(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            Organizations organizations = new Organizations(ticket.Collection.LoginUser);
            organizations.LoadByTicketID(ticketID);
            if (organizations.IsEmpty) return null;

            List<int> organizationIDs = new List<int>();
            foreach (Organization organization in organizations)
            {
                organizationIDs.Add(organization.OrganizationID);
            }
            Products products = new Products(ticket.Collection.LoginUser);
            products.LoadByCustomerIDs(organizationIDs.ToArray());

            if (products.IsEmpty) return null;
            List<int> productIDs = new List<int>();
            foreach (Product product in products)
            {
                productIDs.Add(product.ProductID);
            }

            return productIDs.ToArray();

        }

        [WebMethod]
        public bool? RemoveRelated(int ticketID1, int ticketID2)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Ticket ticket1 = Tickets.GetTicket(loginUser, ticketID1);
            Ticket ticket2 = Tickets.GetTicket(loginUser, ticketID2);
            if (!CanEditTicket(ticket1)) return null;

            TicketRelationship item = TicketRelationships.GetTicketRelationship(loginUser, ticketID1, ticketID2);
            if (item != null)
            {
                item.Delete();
                item.Collection.Save();

                string description = "Ticket " + ticket2.TicketNumber + " relation removed";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket1.TicketID, description);
                description = "Ticket " + ticket1.TicketNumber + " relation removed";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket2.TicketID, description);
            }

            if (ticket1.ParentID != null && ticket1.ParentID == (int)ticketID2)
            {
                ticket1.ParentID = null;
                ticket1.Collection.Save();

                string description = "Ticket " + ticket2.TicketNumber + " removed as parent";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket1.TicketID, description);
                description = "Ticket " + ticket1.TicketNumber + " removed as child";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket2.TicketID, description);
            }

            if (ticket2.ParentID != null && ticket2.ParentID == (int)ticketID1)
            {
                ticket2.ParentID = null;
                ticket2.Collection.Save();

                string description = "Ticket " + ticket2.TicketNumber + " removed as child";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket1.TicketID, description);
                description = "Ticket " + ticket1.TicketNumber + " removed as parent";
                ActionLogs.AddActionLog(ticket1.Collection.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket2.TicketID, description);
            }

            return true;
        }

        [WebMethod]
        public AutocompleteItem[] SearchTags(string term)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            Tags tags = new Tags(TSAuthentication.GetLoginUser());
            tags.LoadBySearchTerm(term);
            foreach (Tag tag in tags)
            {
                result.Add(new AutocompleteItem(tag.Value, tag.TagID.ToString()));
            }

            return result.ToArray();
        }

        public RelatedTicket[] GetRelatedTickets(int ticketID)
        {
            List<RelatedTicket> relatedTickets = new List<RelatedTicket>();
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);

            if (ticket.ParentID != null)
            {
                TicketsViewItem parent = TicketsView.GetTicketsViewItem(ticket.Collection.LoginUser, (int)ticket.ParentID);
                if (parent != null)
                {
                    RelatedTicket relatedTicket = new RelatedTicket();
                    relatedTicket.TicketID = parent.TicketID;
                    relatedTicket.TicketNumber = parent.TicketNumber;
                    relatedTicket.Name = parent.Name;
                    relatedTicket.Severity = parent.Severity;
                    relatedTicket.Status = parent.Status;
                    relatedTicket.Type = parent.TicketTypeName;
                    relatedTicket.IsParent = true;
                    relatedTicket.IsClosed = parent.IsClosed;
                    relatedTickets.Add(relatedTicket);
                }
            }


            TicketsView tickets = new TicketsView(ticket.Collection.LoginUser);
            tickets.LoadChildren(ticket.TicketID);
            foreach (TicketsViewItem item in tickets)
            {
                RelatedTicket relatedTicket = new RelatedTicket();
                relatedTicket.TicketID = item.TicketID;
                relatedTicket.TicketNumber = item.TicketNumber;
                relatedTicket.Name = item.Name;
                relatedTicket.Severity = item.Severity;
                relatedTicket.Status = item.Status;
                relatedTicket.Type = item.TicketTypeName;
                relatedTicket.IsParent = false;
                relatedTicket.IsClosed = item.IsClosed;
                relatedTickets.Add(relatedTicket);
            }

            tickets = new TicketsView(ticket.Collection.LoginUser);
            tickets.LoadRelated(ticket.TicketID);
            foreach (TicketsViewItem item in tickets)
            {
                RelatedTicket relatedTicket = new RelatedTicket();
                relatedTicket.TicketID = item.TicketID;
                relatedTicket.TicketNumber = item.TicketNumber;
                relatedTicket.Name = item.Name;
                relatedTicket.Severity = item.Severity;
                relatedTicket.Status = item.Status;
                relatedTicket.Type = item.TicketTypeName;
                relatedTicket.IsParent = null;
                relatedTicket.IsClosed = item.IsClosed;
                relatedTickets.Add(relatedTicket);
            }

            return relatedTickets.ToArray();
        }

        [WebMethod]
        public TicketCustomer[] GetTicketCustomers(int ticketID)
        {
            List<TicketCustomer> customers = new List<TicketCustomer>();

            ContactsView contacts = new ContactsView(TSAuthentication.GetLoginUser());
            contacts.LoadByTicketID(ticketID);


            foreach (ContactsViewItem contact in contacts)
            {
                TicketCustomer customer = new TicketCustomer();
                customer.Company = contact.Organization;
                customer.OrganizationID = contact.OrganizationID;
                customer.Contact = contact.FirstName + " " + contact.LastName;
                customer.UserID = contact.UserID;
                if (!(bool)contact.IsActive || !(bool)contact.OrganizationActive || contact.OrganizationSAExpirationDateUtc < DateTime.UtcNow)
                {
                    customer.Flag = true;
                }
                customers.Add(customer);
            }

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByNotContactTicketID(ticketID);
            foreach (Organization organization in organizations)
            {
                TicketCustomer customer = new TicketCustomer();
                customer.Company = organization.Name;
                customer.OrganizationID = organization.OrganizationID;
                customer.UserID = null;
                if (!organization.IsActive || organization.SAExpirationDateUtc < DateTime.UtcNow)
                {
                    customer.Flag = true;
                }
                customers.Add(customer);
            }
            return customers.ToArray();
        }

        private AssetProxy[] GetTicketAssets(int ticketID)
        {
            Assets assets = new Assets(TSAuthentication.GetLoginUser());
            assets.LoadByTicketID(ticketID);
            return assets.GetAssetProxies();
        }

        [WebMethod]
        public UserInfo[] GetSubscribers(int ticketID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            return GetSubscribers(ticket);
        }

        private UserInfo[] GetSubscribers(TicketsViewItem ticket)
        {
            UsersView users = new UsersView(ticket.Collection.LoginUser);
            users.LoadBySubscription(ticket.TicketID, ReferenceType.Tickets);
            List<UserInfo> result = new List<UserInfo>();
            foreach (UsersViewItem user in users)
            {
                result.Add(new UserInfo(user));
            }
            return result.ToArray();
        }

        private UserInfo[] GetQueuers(TicketsViewItem ticket)
        {
            UsersView users = new UsersView(ticket.Collection.LoginUser);
            users.LoadByTicketQueue(ticket.TicketID);
            List<UserInfo> result = new List<UserInfo>();
            foreach (UsersViewItem user in users)
            {
                result.Add(new UserInfo(user));
            }
            return result.ToArray();
        }

        [WebMethod]
        public void DeleteAttachment(int attachmentID)
        {
            AttachmentAPI.DeleteAttachment(attachmentID, AttachmentProxy.References.Actions);
        }

        [WebMethod]
        public NewTicketSaveInfo DummyTicketSaveInfo()
        {
            return null;
        }

        [WebMethod]
        public int[] NewTicket(string data)
        {
            List<int> result = new List<int>();
            NewTicketSaveInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<NewTicketSaveInfo>(data);

            Ticket ticket = (new Tickets(TSAuthentication.GetLoginUser())).AddNewTicket();
            ticket.OrganizationID = TSAuthentication.OrganizationID;
            ticket.TicketSource = info.ChatID != null ? "Chat" : "Agent";
            ticket.Name = info.Name;
            ticket.TicketTypeID = info.TicketTypeID;
            ticket.TicketStatusID = info.TicketStatusID;
            ticket.TicketSeverityID = info.TicketSeverityID;
            ticket.UserID = info.UserID < 0 ? null : (int?)info.UserID;
            ticket.GroupID = info.GroupID < 0 ? null : (int?)info.GroupID;
            ticket.ProductID = info.ProductID < 0 ? null : (int?)info.ProductID;
            ticket.ReportedVersionID = info.ReportedID < 0 ? null : (int?)info.ReportedID;
            ticket.SolvedVersionID = info.ResolvedID < 0 ? null : (int?)info.ResolvedID;
            ticket.ProductID = info.ProductID < 0 ? null : (int?)info.ProductID;
            ticket.IsKnowledgeBase = info.IsKnowledgebase;
            ticket.KnowledgeBaseCategoryID = info.KnowledgeBaseCategoryID < 0 ? null : (int?)info.KnowledgeBaseCategoryID;

            User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
            if (ticket.IsKnowledgeBase && !user.ChangeKBVisibility && !user.IsSystemAdmin)
            {
                ticket.IsVisibleOnPortal = false;
            }
            else
            {
                ticket.IsVisibleOnPortal = info.IsVisibleOnPortal;
            }

            ticket.ParentID = info.ParentTicketID;
            ticket.DueDate = info.DueDate;
            ticket.Collection.Save();

            if (info.CategoryID != null && info.CategoryID > -1) ticket.AddCommunityTicket((int)info.CategoryID);

            //// new action - future
            //if (TeamSupport.IDTree.ConnectionContext.IsEnabled)  // new action - new ticket
            //{
            //    ActionProxy actionProxy = new ActionProxy()
            //    {
            //        Name = "Description",
            //        SystemActionTypeID = SystemActionType.Description,
            //        Description = info.Description,
            //        IsVisibleOnPortal = ticket.IsVisibleOnPortal,
            //        IsKnowledgeBase = ticket.IsKnowledgeBase,
            //        TicketID = ticket.TicketID,
            //        TimeSpent = info.TimeSpent,
            //        DateStarted = info.DateStarted,
            //        ActionSource = ticket.TicketSource
            //    };
            //    ModelAPI.Create(actionProxy);
            //    result.Add(actionProxy.ActionID);
            //}

            TeamSupport.Data.Action action = (new Actions(ticket.Collection.LoginUser)).AddNewAction();
            action.ActionTypeID = null;
            action.Name = "Description";
            action.SystemActionTypeID = SystemActionType.Description;
            action.ActionSource = ticket.TicketSource;
            action.Description = info.Description;


            if (!string.IsNullOrWhiteSpace(user.Signature) && info.IsVisibleOnPortal)
            {
                action.Description = action.Description + "<br/><br/>" + user.Signature;
            }

            action.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
            action.IsKnowledgeBase = ticket.IsKnowledgeBase;
            action.TicketID = ticket.TicketID;
            action.TimeSpent = info.TimeSpent;
            action.DateStarted = info.DateStarted;
            action.Collection.Save();

            result.Add(ticket.TicketID);
            result.Add(action.ActionID);

            foreach (int ticketID in info.RelatedTickets.Distinct())
            {
                AddRelated(ticket.TicketID, ticketID, null);
            }

            foreach (int ticketID in info.ChildTickets.Distinct())
            {
                AddRelated(ticket.TicketID, ticketID, true);
            }

            foreach (int userID in info.Subscribers.Distinct())
            {
                Subscriptions.AddSubscription(ticket.Collection.LoginUser, userID, ReferenceType.Tickets, ticket.TicketID);
            }

            foreach (int userID in info.Queuers.Distinct())
            {
                TicketQueue.Enqueue(ticket.Collection.LoginUser, ticket.TicketID, userID);
            }

            foreach (string tag in info.Tags.Distinct())
            {
                AddTag(ticket.TicketID, tag);
            }

            foreach (int id in info.Customers.Distinct())
            {
                AddTicketCustomer(ticket.TicketID, "o", id);
            }

            foreach (int id in info.Contacts.Distinct())
            {
                AddTicketCustomer(ticket.TicketID, "u", id);
            }

            foreach (int assetID in info.Assets.Distinct())
            {
                AddTicketAsset(ticket.TicketID, assetID);
            }

            foreach (CustomFieldSaveInfo field in info.Fields)
            {
                CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), field.CustomFieldID, ticket.TicketID);
                if (field.Value == null)
                {
                    customValue.Value = "";
                }
                else
                {
					switch (customValue.FieldType)
                    {
						case CustomFieldType.DateTime:
						case CustomFieldType.Time:
                        DateTime dt;
                        if (DateTime.TryParse(((string)field.Value).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                        {
                            customValue.Value = dt.ToUniversalTime().ToString();
                        }
							break;
						case CustomFieldType.Date:
							DateTime customDate;
							string fieldValue = (string)field.Value;

							if (DateTime.TryParse(fieldValue, out customDate))
							{
								customValue.Value = customDate.ToShortDateString();
							}

							break;
						default:
                        customValue.Value = field.Value.ToString();
							break;
                    }

                }

                customValue.Collection.Save();
            }

            if (info.ChatID != null)
            {

                Chat chat = Chats.GetChat(ticket.Collection.LoginUser, (int)info.ChatID);
                if (chat != null)
                {
                    TeamSupport.Data.Action chatAction = (new Actions(ticket.Collection.LoginUser)).AddNewAction();
                    chatAction.ActionTypeID = null;
                    chatAction.Name = "Chat";
                    chatAction.ActionSource = "Chat";
                    chatAction.SystemActionTypeID = SystemActionType.Chat;
                    chatAction.Description = chat.GetHtml(true, chatAction.Collection.LoginUser.OrganizationCulture);
                    chatAction.IsVisibleOnPortal = ticket.IsVisibleOnPortal;
                    chatAction.IsKnowledgeBase = ticket.IsKnowledgeBase;
                    chatAction.TicketID = ticket.TicketID;
                    chatAction.Collection.Save();
                    chat.ActionID = chatAction.ActionID;
                    chat.Collection.Save();
                }
            }

            Reminders reminders = new Reminders(ticket.Collection.LoginUser);
            foreach (NewTicketReminderInfo reminderInfo in info.Reminders)
            {
                Reminder reminder = reminders.AddNewReminder();
                reminder.OrganizationID = TSAuthentication.OrganizationID;
                reminder.RefID = ticket.TicketID;
                reminder.RefType = ReferenceType.Tickets;
                DateTime dt;
                if (DateTime.TryParse((reminderInfo.DueDate).Replace("UTC", "GMT"), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out dt))
                {
                    reminder.DueDate = dt.ToUniversalTime();
                }
                reminder.Description = reminderInfo.Description;
                reminder.UserID = reminderInfo.UserID;
            }
            reminders.Save();

            //User user = Users.GetUser(ticket.Collection.LoginUser, TSAuthentication.UserID);
            if (user.SubscribeToNewTickets) Subscriptions.AddSubscription(ticket.Collection.LoginUser, TSAuthentication.UserID, ReferenceType.Tickets, ticket.TicketID);


            return result.ToArray();
        }


        [WebMethod]
        public void AddNewTicketCustomer(string first, string last, string email, string company, int? organizationID)
        {

        }

        [WebMethod]
        public int GetTicketWaterCoolerCount(int ticketID)
        {
            WaterCoolerView wcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
            return wcv.GetTicketWaterCoolerCount(ticketID);
        }

        private void TransferCustomValues(int ticketID, int ticketTypeID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            CustomValueProxy[] oldValues = GetCustomValues(ticketID);
            DateTime dateValue;
            CustomValues values = new CustomValues(ticket.Collection.LoginUser);
            values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets, ticketTypeID, ticketID);
            CustomValueProxy[] newValues = values.GetCustomValueProxies();

            foreach (CustomValueProxy newCustVal in newValues)
            {
                foreach (CustomValueProxy oldCustVal in oldValues)
                {
                    if (newCustVal.Name == oldCustVal.Name && newCustVal.FieldType == oldCustVal.FieldType)
                    {
                        CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), newCustVal.CustomFieldID, ticketID);
                        if (oldCustVal.Value == null)
                        {
                            customValue.Value = "";
                            customValue.Collection.Save();
                        }

                        if (customValue.FieldType == CustomFieldType.DateTime || customValue.FieldType == CustomFieldType.Date || customValue.FieldType == CustomFieldType.Time)
                        {
                            if (oldCustVal.Value != null)
                            {
                                if (DateTime.TryParse(oldCustVal.Value.ToString(), out dateValue))
                                    customValue.Value = dateValue.ToString();
                            }
                        }
                        else
                        {
                            customValue.Value = oldCustVal.Value.ToString();
                        }

                        customValue.Collection.Save();
                    }
                }

            }
        }

        [WebMethod]
        public string SavePasteImage(string image, string source)
        {
            try
            {

                String temppath = HttpContext.Current.Request.PhysicalApplicationPath + "images\\";
                string path = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentPath2(TSAuthentication.OrganizationID, TSAuthentication.GetLoginUser());
                string filename = Guid.NewGuid().ToString();

                if (source.StartsWith("data:image/png;base64,"))
                {
                    string[] tokens = source.Split(',');
                    byte[] imgbytes = Convert.FromBase64String(tokens[1]);
                    File.WriteAllBytes(temppath + "temp_" + filename + ".png", imgbytes);
                }
                else if (image != "")
                {
                    using (WebClient Client = new WebClient())
                    {
                        image = image.Replace(".ashx", "");
                        Client.DownloadFile(image, temppath + "temp_" + filename + ".png");
                    }
                }

                if (image != "")
                {
                    ImageBuilder.Current.Build(temppath + "temp_" + filename + ".png", path + '\\' + filename + ".png", new ResizeSettings(image));
                    File.Delete(temppath + "temp_" + filename + ".png");

                    return TSAuthentication.OrganizationID + "/images/" + filename + ".png";
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "Save Paste Image", image);


            }


            return "";
        }

        [WebMethod]
        public string MergeTickets(int winningTicketID, int losingTicketID)
        {
           //return  Model_API.MergeTickets(winningTicketID, losingTicketID);
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), winningTicketID);
            String errLocation = "";

            try
            {
                MergeContacts(losingTicketID, winningTicketID, ticket);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();
                errLocation = string.Format("Error merging ticket contacts. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                MergeTags(losingTicketID, winningTicketID, ticket);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging ticket tags. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                MergeSubscribers(losingTicketID, winningTicketID, ticket);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging ticket subscribers. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                MergeQueres(losingTicketID, winningTicketID, ticket);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging ticket queres. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                ticket.Collection.MergeUpdateReminders(losingTicketID, winningTicketID);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging ticket reminders. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                ticket.Collection.MergeUpdateAssets(losingTicketID, winningTicketID);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging ticket assets. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                ticket.Collection.MergeUpdateActions(losingTicketID, winningTicketID);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging ticket actions. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                ticket.Collection.MergeUpdateRelationships(losingTicketID, winningTicketID);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error merging ticket relationships. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            try
            {
                ticket.Collection.DeleteFromDB(losingTicketID);
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("We have encountered a possible error merging your tickets. Please check and ensure the loser was deleted and the tickets merged as expected. If not or if you have any concerns please contact support.");
            }

            try
            {
                ticket.ModifierID = TSAuthentication.GetLoginUser().UserID;
                ticket.DateModified = DateTime.UtcNow;
                ticket.Collection.Save();
            }
            catch (Exception e)
            {
                ExceptionLog log = (new ExceptionLogs(TSAuthentication.GetLoginUser())).AddNewExceptionLog();
                log.ExceptionName = "Merge Exception " + e.Source;
                log.Message = e.Message.Replace(Environment.NewLine, "<br />");
                log.StackTrace = e.StackTrace.Replace(Environment.NewLine, "<br />");
                log.Collection.Save();

                errLocation = string.Format("Error updating winning ticket modifier and date modified. Exception #{0}. Please report this to TeamSupport by either emailing support@teamsupport.com, or clicking Help/Support Hub in the upper right of your account.", log.ExceptionLogID);
            }

            return errLocation;
        }

        [WebMethod]
        public List<string> BulkMergeTickets(int winningTicketID, string losingTicketIDs)
        {
            int[] ids = JsonConvert.DeserializeObject<int[]>(losingTicketIDs);
            List<string> messages = new List<string>();
            foreach (int ticketID in ids)
            {
                messages.Add(MergeTickets(winningTicketID, ticketID));
            }
            return messages;
        }

        public void MergeContacts(int losingTicketID, int winningTicketID, Ticket ticket)
        {
            List<TicketCustomer> customers = new List<TicketCustomer>();

            ContactsView contacts = new ContactsView(TSAuthentication.GetLoginUser());
            contacts.LoadByTicketID(losingTicketID);

            foreach (ContactsViewItem contact in contacts)
            {
                ticket.Collection.AddContact(contact.UserID, winningTicketID);
                ticket.Collection.RemoveContact(contact.UserID, losingTicketID);
            }

            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByNotContactTicketID(losingTicketID);
            foreach (Organization organization in organizations)
            {
                ticket.Collection.AddOrganization(organization.OrganizationID, winningTicketID);
                ticket.Collection.RemoveOrganization(organization.OrganizationID, losingTicketID);
            }

            Ticket oldticket = (Ticket)Tickets.GetTicket(TSAuthentication.GetLoginUser(), losingTicketID);
            string description = "Merged '" + oldticket.TicketNumber + "' Customers";
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, winningTicketID, description);
            return;
        }

        public void MergeTags(int losingTicketID, int winningTicketID, Ticket ticket)
        {
            Tags tags = new Tags(TSAuthentication.GetLoginUser());
            tags.LoadByReference(ReferenceType.Tickets, losingTicketID);

            foreach (Tag tag in tags)
            {
                RemoveTag(losingTicketID, tag.TagID);
                AddTag(winningTicketID, tag.Value);
            }
        }

        public void MergeSubscribers(int losingTicketID, int winningTicketID, Ticket ticket)
        {
            UsersView users = new UsersView(TSAuthentication.GetLoginUser());
            users.LoadBySubscription(losingTicketID, ReferenceType.Tickets);
            List<UserInfo> result = new List<UserInfo>();
            foreach (UsersViewItem user in users)
            {
                ticket.Collection.AddSubscription(user.UserID, winningTicketID);
                ticket.Collection.RemoveSubscription(user.UserID, losingTicketID);
            }
            Ticket losingticket = (Ticket)Tickets.GetTicket(TSAuthentication.GetLoginUser(), losingTicketID);
            string description = "Merged '" + losingticket.TicketNumber + "' Subscribers";
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, winningTicketID, description);

        }

        public void MergeQueres(int losingTicketID, int winningTicketID, Ticket ticket)
        {
            UsersView users = new UsersView(ticket.Collection.LoginUser);
            users.LoadByTicketQueue(losingTicketID);
            List<UserInfo> result = new List<UserInfo>();

            foreach (UsersViewItem user in users)
            {
                TicketQueue.Dequeue(TSAuthentication.GetLoginUser(), losingTicketID, user.UserID);
                TicketQueue.Enqueue(TSAuthentication.GetLoginUser(), winningTicketID, user.UserID);
            }
            Ticket losingticket = (Ticket)Tickets.GetTicket(TSAuthentication.GetLoginUser(), losingTicketID);
            string description = "Merged '" + ticket.TicketNumber + "' Queuers";
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, winningTicketID, description);
        }

        private TicketLinkToJiraItemProxy GetLinkToJira(int ticketID)
        {
            TicketLinkToJiraItemProxy result = null;
            TicketLinkToJira linkToJira = new TicketLinkToJira(TSAuthentication.GetLoginUser());
            linkToJira.LoadByTicketID(ticketID);
            if (linkToJira.Count > 0)
            {
                result = linkToJira[0].GetProxy();
            }
            return result;
        }

        private TicketLinkToTFSItemProxy GetLinkToTFS(int ticketID)
        {
            TicketLinkToTFSItemProxy result = null;
            TicketLinkToTFS linkToTFS = new TicketLinkToTFS(TSAuthentication.GetLoginUser());
            linkToTFS.LoadByTicketID(ticketID);
            if (linkToTFS.Count > 0)
            {
                result = linkToTFS[0].GetProxy();
            }
            return result;
        }

		private TicketLinkToSnowItemProxy GetLinkToSnow(int ticketID)
		{
			TicketLinkToSnowItemProxy result = null;
			TicketLinkToSnow linkToSnow = new TicketLinkToSnow(TSAuthentication.GetLoginUser());
			linkToSnow.LoadByTicketID(ticketID);

			if (linkToSnow.Count > 0)
			{
				result = linkToSnow[0].GetProxy();
			}

			return result;
		}

		[WebMethod]
        public CustomValueProxy[] GetParentValues(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            CustomValues values = new CustomValues(ticket.Collection.LoginUser);
            values.LoadParentsByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Tickets, ticket.TicketTypeID, ticket.TicketID, ticket.ProductID);
            return values.GetCustomValueProxies();
        }

        [WebMethod]
        public CustomValueProxy[] GetMatchingParentValueFields(int ticketID, int parentFieldID, string parentValue)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            CustomValues values = new CustomValues(ticket.Collection.LoginUser);
            values.LoadByParentValue(TSAuthentication.OrganizationID, ReferenceType.Tickets, ticket.TicketTypeID, ticket.TicketID, parentFieldID, parentValue, ticket.ProductID);
            return values.GetCustomValueProxies();
        }

        [WebMethod]
        public TicketLinkToJiraItemProxy GetLinkToJiraSync(int ticketID)
        {
            TicketLinkToJiraItemProxy result = null;
            TicketLinkToJira linkToJira = new TicketLinkToJira(TSAuthentication.GetLoginUser());
            linkToJira.LoadByTicketID(ticketID);
            if (linkToJira.Count > 0)
            {
                result = linkToJira[0].GetProxy();
            }
            return result;
        }

        [WebMethod]
        public List<string> GetSessionInfo()
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            //var OpenTok = new OpenTok(45228242, "058e12ca5b9139d08c18f4fe1ece434635a4abfb");
            // Create a session that uses the OpenTok Media Router (which is required for archiving)
            var session = OpenTok.CreateSession(mediaMode: MediaMode.ROUTED, archiveMode: ArchiveMode.MANUAL);
            var token = OpenTok.GenerateToken(session.Id, Role.PUBLISHER);
            var values = new List<string>();
            // Store this sessionId in the database for later use:
            //string sessionId = session.Id;
            //var archive = OpenTok.StartArchive(session.Id);
            values.Add(session.Id);
            values.Add(token);
            values.Add(SystemSettings.GetTokApiKey());
            return values;
        }

        [WebMethod]
        public string StartArchiving(string sessionId)
        {
            //var OpenTok = new OpenTok(45228242, "058e12ca5b9139d08c18f4fe1ece434635a4abfb");
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());

            // Create a session that uses the OpenTok Media Router (which is required for archiving)
            //var session = OpenTok.CreateSession(mediaMode: MediaMode.ROUTED);
            // Store this sessionId in the database for later use:
            //string sessionId = session.Id;
            var archive = OpenTok.StartArchive(sessionId);
            return archive.Id.ToString();
        }

        [WebMethod]
        public string StartArchivingScreen(string sessionId)
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            var archive = OpenTok.StartArchive(sessionId, "", true, true, OutputMode.INDIVIDUAL);
            return archive.Id.ToString();
        }

        [WebMethod]
        public string StopArchiving(string archiveId)
        {
            //var OpenTok = new OpenTok(45228242, "058e12ca5b9139d08c18f4fe1ece434635a4abfb");
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());

            // Create a session that uses the OpenTok Media Router (which is required for archiving)
            //var session = OpenTok.CreateSession(mediaMode: MediaMode.ROUTED);
            // Store this sessionId in the database for later use:
            //string sessionId = session.Id;
            System.Threading.Thread.Sleep(1500);
            var archive = OpenTok.StopArchive(archiveId);
            do
            {

            }
            while (OpenTok.GetArchive(archiveId).Status != ArchiveStatus.UPLOADED);

            TokStorageItem ts = (new TokStorage(TSAuthentication.GetLoginUser())).AddNewTokStorageItem();
            ts.AmazonPath = string.Format("https://s3.amazonaws.com/{2}/{0}/{1}/archive.mp4", SystemSettings.GetTokApiKey(), archive.Id, SystemSettings.ReadString(TSAuthentication.GetLoginUser(), "AWS-VideoBucket", ""));
            ts.CreatedDate = DateTime.Now;
            ts.CreatorID = TSAuthentication.GetLoginUser().UserID;
            ts.OrganizationID = TSAuthentication.GetLoginUser().OrganizationID;
            ts.ArchiveID = archive.Id.ToString();
            ts.Collection.Save();

            return string.Format("https://s3.amazonaws.com/{2}/{0}/{1}/archive.mp4", SystemSettings.GetTokApiKey(), archive.Id.ToString(), SystemSettings.ReadString(TSAuthentication.GetLoginUser(), "AWS-VideoBucket", ""));

        }

        [WebMethod]
        public bool DeleteArchive(string archiveId)
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());

            // Create a session that uses the OpenTok Media Router (which is required for archiving)
            //var session = OpenTok.CreateSession(mediaMode: MediaMode.ROUTED);
            // Store this sessionId in the database for later use:
            OpenTok.DeleteArchive(archiveId);

            TokStorage ts = new TokStorage(TSAuthentication.GetLoginUser());
            ts.LoadByArchiveID(archiveId.ToString());
            ts[0].Delete();
            ts[0].Collection.Save();



            return true;
        }

        private string SetSyncWithJira(LoginUser loginUser, int ticketId, string jiraIssueKey)
        {
            dynamic result = new ExpandoObject();

            TicketLinkToJira ticketLinkToJira = new TicketLinkToJira(loginUser);
            ticketLinkToJira.LoadByTicketID(ticketId);

            if (ticketLinkToJira.Count == 0)
            {
                try
                {
                    TicketLinkToJiraItem ticketLinkToJiraItem = ticketLinkToJira.AddNewTicketLinkToJiraItem();
                    ticketLinkToJiraItem.TicketID = ticketId;
                    ticketLinkToJiraItem.JiraKey = jiraIssueKey;
                    ticketLinkToJiraItem.SyncWithJira = true;
                    ticketLinkToJiraItem.CreatorID = loginUser.UserID;

                    TicketsViewItem ticket = TicketsView.GetTicketsViewItem(loginUser, ticketId);
                    ticketLinkToJiraItem.CrmLinkID = CRMLinkTable.GetIdBy(ticket.OrganizationID, IntegrationType.Jira.ToString().ToLower(), ticket.ProductID, loginUser);

                    //If product is not associated to an instance then get the 'default' instance
                    if (ticketLinkToJiraItem.CrmLinkID == null || ticketLinkToJiraItem.CrmLinkID <= 0)
                    {
                        CRMLinkTable crmlink = new CRMLinkTable(loginUser);
                        crmlink.LoadByOrganizationID(TSAuthentication.OrganizationID);

                        ticketLinkToJiraItem.CrmLinkID = crmlink.Where(p => p.InstanceName == "Default"
                                                                            && p.CRMType.ToLower() == IntegrationType.Jira.ToString().ToLower())
                                                                            .Select(p => p.CRMLinkID).FirstOrDefault();
                    }

                    if (ticketLinkToJiraItem.CrmLinkID != null && ticketLinkToJiraItem.CrmLinkID > 0)
                    {
                        ticketLinkToJiraItem.Collection.Save();
                        result.IsSuccessful = true;
                        result.Error = null;
                    }
                    else
                    {
                        result.IsSuccessful = false;
                        result.Error = "A Jira Instance associated to this ticket's product or a Default Jira Instance was not found.";
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccessful = false;
                    result.Error = ex.Message;
                    ExceptionLogs.LogException(loginUser, ex, "SetJiraIssueKey");
                }
            }

            return JsonConvert.SerializeObject(result);
        }

        private string SetSyncWithTFS(LoginUser loginUser, int ticketId, string TFSWorkItemID)
        {
            dynamic result = new ExpandoObject();

            TicketLinkToTFS ticketLinkToTFS = new TicketLinkToTFS(loginUser);
            ticketLinkToTFS.LoadByTicketID(ticketId);

            if (ticketLinkToTFS.Count == 0)
            {
                try
                {
                    int tfsWorkItemID;
                    bool idIsNumeric = int.TryParse(TFSWorkItemID, out tfsWorkItemID);

                    //If it's numeric then we are linking to an existing workitem, it is null then we are creating a new one
                    if (idIsNumeric || string.IsNullOrEmpty(TFSWorkItemID))
                    {
                        TicketLinkToTFSItem ticketLinkToTFSItem = ticketLinkToTFS.AddNewTicketLinkToTFSItem(ticketId);
                        ticketLinkToTFSItem.TFSID = (idIsNumeric ? tfsWorkItemID : (int?)null);
                        ticketLinkToTFSItem.TFSTitle = TFSWorkItemID;
                        ticketLinkToTFSItem.SyncWithTFS = true;
                        ticketLinkToTFSItem.CreatorID = loginUser.UserID;

                        //Jira uses this to support multiple instances.
                        //TicketsViewItem ticket = TicketsView.GetTicketsViewItem(loginUser, ticketId);
                        //ticketLinkToJiraItem.CrmLinkID = CRMLinkTable.GetIdBy(ticket.OrganizationID, IntegrationType.Jira.ToString().ToLower(), ticket.ProductID, loginUser);
                        ticketLinkToTFSItem.CrmLinkID = CRMLinkTable.GetIdBy(loginUser.OrganizationID, IntegrationType.TFS.ToString().ToLower(), loginUser);

                        ////If product is not associated to an instance then get the 'default' instance
                        //if (ticketLinkToJiraItem.CrmLinkID == null || ticketLinkToJiraItem.CrmLinkID <= 0)
                        //{
                        //    CRMLinkTable crmlink = new CRMLinkTable(loginUser);
                        //    crmlink.LoadByOrganizationID(TSAuthentication.OrganizationID);

                        //    ticketLinkToJiraItem.CrmLinkID = crmlink.Where(p => p.InstanceName == "Default"
                        //                                                        && p.CRMType.ToLower() == IntegrationType.Jira.ToString().ToLower())
                        //                                                        .Select(p => p.CRMLinkID).FirstOrDefault();
                        //}

                        if (ticketLinkToTFSItem.CrmLinkID != null && ticketLinkToTFSItem.CrmLinkID > 0)
                        {
                            ticketLinkToTFSItem.Collection.Save();
                            result.IsSuccessful = true;
                            result.Error = null;
                        }
                        else
                        {
                            result.IsSuccessful = false;
                            result.Error = "A TFS Instance associated to this ticket's product or a Default TFS Instance was not found.";
                        }
                    }
                    else
                    {
                        result.IsSuccessful = false;
                        result.Error = "Entered TFS Work Item ID is not numeric.";
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccessful = false;
                    result.Error = ex.Message;
                    ExceptionLogs.LogException(loginUser, ex, "SetTFSWorkItemID");
                }
            }

            return JsonConvert.SerializeObject(result);
        }

		private string SetSyncWithSnow(LoginUser loginUser, int ticketId, string incidentId)
		{
			dynamic result = new ExpandoObject();

			TicketLinkToSnow ticketLinkToSnow = new TicketLinkToSnow(loginUser);
			ticketLinkToSnow.LoadByTicketID(ticketId);

			if (ticketLinkToSnow.Count == 0)
			{
				try
				{
					TicketLinkToSnowItem ticketLinkToSnowItem = ticketLinkToSnow.AddNewTicketLinkToSnowItem(ticketId);
					ticketLinkToSnowItem.AppId = null;
					ticketLinkToSnowItem.Number = incidentId;
					ticketLinkToSnowItem.Sync = true;
					ticketLinkToSnowItem.CreatorID = loginUser.UserID;
					ticketLinkToSnowItem.CrmLinkID = CRMLinkTable.GetIdBy(loginUser.OrganizationID, IntegrationType.ServiceNow.ToString().ToLower(), loginUser);

					if (ticketLinkToSnowItem.CrmLinkID != null && ticketLinkToSnowItem.CrmLinkID > 0)
					{
						ticketLinkToSnowItem.Collection.Save();
						result.IsSuccessful = true;
						result.Error = null;
					}
					else
					{
						result.IsSuccessful = false;
						result.Error = "An active ServiceNow integration for this ticket was not found.";
					}
				}
				catch (Exception ex)
				{
					result.IsSuccessful = false;
					result.Error = ex.Message;
					ExceptionLogs.LogException(loginUser, ex, "SetSyncWithSnow");
				}
			}
			else
			{
				result.IsSuccessful = false;
				result.Error = "A link setup for this ticket to ServiceNow is already set.";
			}

			return JsonConvert.SerializeObject(result);
		}

		[WebMethod]
        public SuggestedSolutions GetSuggestedSolutions(int ticketID, int firstItemIndex, int pageSize)
        {
            SuggestedSolutions result = new SuggestedSolutions();
            List<SuggestedSolutionsItem> resultItems = new List<SuggestedSolutionsItem>();

            if (ticketID != 0)
            {
                LoginUser loginUser = TSAuthentication.GetLoginUser();
                string searchTerm = GetSuggestedSolutionDefaultInput(ticketID);
                List<int> suggestedSolutionsIDs = GetSuggestedSolutionsIDs(searchTerm, loginUser);
                if (suggestedSolutionsIDs.Count > 0)
                {
                    string ticketIdsCommaList = string.Join(",", suggestedSolutionsIDs);

                    DataTable suggestedSolutions = new DataTable();
                    using (SqlCommand command = new SqlCommand())
                    {
                        StringBuilder builder = new StringBuilder();

                        builder.Append(@"
                            DECLARE @TempItems
                            TABLE
                            (
                              ID        int IDENTITY,
                              TicketID  int
                            )

                            INSERT INTO @TempItems
                            (
                              TicketID
                            )
                            SELECT
                                t.TicketID
                            FROM
                                Tickets t
                            WHERE
                                t.TicketID IN (" + ticketIdsCommaList + @")
                            ORDER BY
                                t.DateModified DESC

                            SELECT
                                t.TicketID
                                , t.TicketNumber
                                , t.Name
                                , dbo.uspGetTags(17, t.TicketID) AS Tags
	                            , kbc.CategoryName AS KnowledgeBaseCategoryName
                            FROM
                                @TempItems ti
                                JOIN Tickets t
                                    ON ti.TicketID = t.TicketID
                                LEFT JOIN KnowledgeBaseCategories kbc
                                    ON t.KnowledgeBaseCategoryID = kbc.CategoryID
                            WHERE
                                ti.ID BETWEEN @FromIndex AND @toIndex
                            ORDER BY
                                ti.ID"
                        );
                        command.CommandText = builder.ToString();
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@FromIndex", firstItemIndex + 1);
                        command.Parameters.AddWithValue("@ToIndex", firstItemIndex + pageSize);

                        using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                        {
                            connection.Open();
                            SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                            command.Connection = connection;
                            command.Transaction = transaction;
                            SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                            suggestedSolutions.Load(reader);
                        }
                    }

                    result.Count = suggestedSolutions.Rows.Count;
                    for (int i = 0; i < suggestedSolutions.Rows.Count; i++)
                    {
                        SuggestedSolutionsItem item = new SuggestedSolutionsItem();
                        item.ID = (int)suggestedSolutions.Rows[i]["TicketID"];
                        item.DisplayName = string.Format("{0}: {1}", suggestedSolutions.Rows[i]["TicketNumber"].ToString(), suggestedSolutions.Rows[i]["Name"].ToString());
                        item.Tags = suggestedSolutions.Rows[i]["Tags"].ToString();
                        item.KBCategory = suggestedSolutions.Rows[i]["KnowledgeBaseCategoryName"].ToString();
                        resultItems.Add(item);
                    }
                    result.Items = resultItems.ToArray();
                }
            }
            else
            {
                result.Count = 0;
            }
            return result;
        }


        [WebMethod]
        public string GetSuggestedSolutionDefaultInput(int ticketid)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            SqlCommand command = new SqlCommand();
            command.CommandText = @"
            SELECT
                Description
            FROM
                Actions
            WHERE
                TicketID = @TicketID
                and SystemActionTypeID IN (1,3,5)";
            command.Parameters.AddWithValue("@TicketID", ticketid.ToString());

            DataTable table = SqlExecutor.ExecuteQuery(loginUser, command);
            if (table.Rows.Count > 0)
            {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    result.AppendLine(HtmlUtility.StripHTML2(table.Rows[i][0].ToString()));
                }
                return result.ToString();
            }
            else
                return string.Empty;

        }

        private static List<int> GetSuggestedSolutionsIDs(string searchTerm, LoginUser loginUser)
        {
            DataTable tagList = new DataTable();

            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder builder = new StringBuilder();

                builder.Append(@"
                    SELECT
                        t.TicketID
                        , LOWER(tlv.Value)
                    FROM
                        Tickets t
                        JOIN TagLinksView tlv
                            ON  t.TicketID = tlv.RefID
                    WHERE
                        t.OrganizationID = @OrganizationID
                        AND t.IsKnowledgeBase = 1"
                );
                command.CommandText = builder.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", loginUser.OrganizationID);

                using (SqlConnection connection = new SqlConnection(loginUser.ConnectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                    command.Connection = connection;
                    command.Transaction = transaction;
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                    tagList.Load(reader);
                }
            }

            List<int> result = new List<int>();
            searchTerm = searchTerm.ToLower();
            for (int i = 0; i < tagList.Rows.Count; i++)
            {
                //if (searchTerm.Contains(tagList.Rows[i][1].ToString()))
                if (System.Text.RegularExpressions.Regex.IsMatch(searchTerm, @"\b" + tagList.Rows[i][1].ToString() + @".?\b"))
                {
                    result.Add((int)tagList.Rows[i][0]);
                }
            }
            return result;
        }
        private static string cleanSearchTerm(string searchTerm)
        {
            searchTerm = searchTerm.Replace("\"", "").Replace("'", "");
            if (!String.IsNullOrEmpty(searchTerm))
                searchTerm = "\"" + searchTerm + "\"";

            return searchTerm;
        }

    }




    [DataContract]
    public class TicketPage
    {
        public TicketPage(int pageIndex, int pageSize, int count, TicketsViewItemProxy[] tickets, TicketLoadFilter filter)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            PageCount = (int)(count / pageSize);
            if (count % pageSize > 0) PageCount++;
            Count = count;
            Tickets = tickets;
            Filter = filter;
        }
        [DataMember]
        public int PageIndex { get; set; }
        [DataMember]
        public int PageSize { get; set; }
        [DataMember]
        public int PageCount { get; set; }
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public TicketsViewItemProxy[] Tickets { get; set; }
        [DataMember]
        public TicketLoadFilter Filter { get; set; }
    }

    [DataContract]
    public class TicketInfo
    {
        [DataMember]
        public TicketsViewItemProxy Ticket { get; set; }
        [DataMember]
        public ActionInfo[] Actions { get; set; }
        [DataMember]
        public TicketCustomer[] Customers { get; set; }
        [DataMember]
        public RelatedTicket[] Related { get; set; }
        [DataMember]
        public TagProxy[] Tags { get; set; }
        [DataMember]
        public CustomValueProxy[] CustomValues { get; set; }
        [DataMember]
        public UserInfo[] Subscribers { get; set; }
        [DataMember]
        public UserInfo[] Queuers { get; set; }
        [DataMember]
        public ReminderProxy[] Reminders { get; set; }
        [DataMember]
        public AssetProxy[] Assets { get; set; }
        [DataMember]
        public TicketLinkToJiraItemProxy LinkToJira { get; set; }
        [DataMember]
        public TicketLinkToTFSItemProxy LinkToTFS { get; set; }
		[DataMember]
		public TicketLinkToSnowItemProxy LinkToSnow { get; set; }
	}

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class NewTicketSaveInfo
    {
        public NewTicketSaveInfo() { }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int TicketTypeID { get; set; }
        [DataMember]
        public int TicketStatusID { get; set; }
        [DataMember]
        public int TicketSeverityID { get; set; }
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public int GroupID { get; set; }
        [DataMember]
        public int ProductID { get; set; }
        [DataMember]
        public int ReportedID { get; set; }
        [DataMember]
        public int ResolvedID { get; set; }
        [DataMember]
        public bool IsVisibleOnPortal { get; set; }
        [DataMember]
        public bool IsKnowledgebase { get; set; }
        [DataMember]
        public int? KnowledgeBaseCategoryID { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime? DateStarted { get; set; }
        [DataMember]
        public int? TimeSpent { get; set; }
        [DataMember]
        public int? ChatID { get; set; }
        [DataMember]
        public int? CategoryID { get; set; }
        [DataMember]
        public int? ParentTicketID { get; set; }
        [DataMember]
        public List<int> RelatedTickets { get; set; }
        [DataMember]
        public List<int> ChildTickets { get; set; }
        [DataMember]
        public List<int> Customers { get; set; }
        [DataMember]
        public List<int> Contacts { get; set; }
        [DataMember]
        public List<string> Tags { get; set; }
        [DataMember]
        public List<CustomFieldSaveInfo> Fields { get; set; }
        [DataMember]
        public List<int> Subscribers { get; set; }
        [DataMember]
        public List<int> Queuers { get; set; }
        [DataMember]
        public List<NewTicketReminderInfo> Reminders { get; set; }
        [DataMember]
        public List<int> Assets { get; set; }
        [DataMember]
        public DateTime? DueDate { get; set; }

    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class NewTicketReminderInfo
    {
        public NewTicketReminderInfo() { }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string DueDate { get; set; }
        [DataMember]
        public int UserID { get; set; }
    }

    [DataContract(Namespace = "http://teamsupport.com/")]
    public class CustomFieldSaveInfo
    {
        public CustomFieldSaveInfo() { }
        [DataMember]
        public int CustomFieldID { get; set; }
        [DataMember]
        public object Value { get; set; }
    }

    [DataContract]
    public class ActionInfo
    {
        [DataMember]
        public UserInfo Creator { get; set; }
        [DataMember]
        public ActionProxy Action { get; set; }
        [DataMember]
        public AttachmentProxy[] Attachments { get; set; }
    }

    [DataContract]
    public class UserInfo
    {
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public DateTime LastLogin { get; set; }
        [DataMember]
        public DateTime LastActivity { get; set; }
        [DataMember]
        public DateTime? LastPing { get; set; }
        [DataMember]
        public bool IsSystemAdmin { get; set; }
        [DataMember]
        public bool IsPortalUser { get; set; }
        [DataMember]
        public bool IsChatUser { get; set; }
        [DataMember]
        public bool InOffice { get; set; }
        [DataMember]
        public string InOfficeComment { get; set; }
        [DataMember]
        public int OrganizationID { get; set; }
        [DataMember]
        public string Organization { get; set; }

        public UserInfo(UsersViewItem user)
        {
            this.OrganizationID = user.OrganizationID;
            this.Organization = user.Organization;
            this.InOfficeComment = user.InOfficeComment;
            this.InOffice = user.InOffice;
            this.IsChatUser = user.IsChatUser;
            this.IsPortalUser = user.IsPortalUser;
            this.IsSystemAdmin = user.IsSystemAdmin;
            this.IsActive = user.IsActive;
            this.Title = user.Title;
            this.LastName = user.LastName;
            this.MiddleName = user.MiddleName;
            this.FirstName = user.FirstName;
            this.Email = user.Email;
            this.UserID = user.UserID;
            this.LastLogin = DateTime.SpecifyKind(user.LastLogin, DateTimeKind.Local);
            this.LastActivity = DateTime.SpecifyKind(user.LastActivity, DateTimeKind.Local);
            this.LastPing = user.LastPing == null ? user.LastPing : DateTime.SpecifyKind((DateTime)user.LastPing, DateTimeKind.Local);
        }

        public UserInfo()
        {

        }
    }

    [DataContract]
    public class TicketCustomer
    {
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public string Contact { get; set; }
        [DataMember]
        public int OrganizationID { get; set; }
        [DataMember]
        public int? UserID { get; set; }
        [DataMember]
        public bool Flag { get; set; }
    }

    [DataContract]
    public class RelatedTicket
    {
        [DataMember]
        public int TicketNumber { get; set; }
        [DataMember]
        public int TicketID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Severity { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public bool? IsParent { get; set; }
        [DataMember]
        public bool IsClosed { get; set; }
    }

    [DataContract]
    public class SuggestedSolutions
    {
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public SuggestedSolutionsItem[] Items { get; set; }
    }

    [DataContract]
    public class SuggestedSolutionsItem
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string Tags { get; set; }
        [DataMember]
        public string KBCategory { get; set; }
    }

    [DataContract]
    public class DeflectorItem {
        [DataMember]
        public int TicketID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int OrganizationID { get; set; }
        [DataMember]
        public int? ProductID { get; set; }
        [DataMember]
        public int TagID { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}
