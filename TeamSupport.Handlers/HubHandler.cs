using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Web.Security;
using System.Dynamic;
using TeamSupport.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;
using dtSearch.Engine;
using System.Diagnostics;

namespace TeamSupport.Handlers
{
    public class HubHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }


        public void ProcessRequest(HttpContext context)
        {

            //SAMPLE: http://localhost/hub/1078/search/kb?q=search%20term
            //SAMPLE: http://localhost/hub/[PARENTID]/search/kb?q=[SEARCH_STRING]

            int userID = -1;
            int parentID = -1;
            int customerHubID = -1;


            //Parse the URL and get the route and ParentID
            StringBuilder routeBuilder = new StringBuilder();
            bool parentFlag = false;
            bool routeFlag = false;
            for (int i = 0; i < context.Request.Url.Segments.Length; i++)
            {
                if (parentFlag)
                {
                    string parentSegment = context.Request.Url.Segments[i].TrimEnd('/').ToLower();
                    if (parentSegment == "admin")
                    {
                        routeBuilder.Append(context.Request.Url.Segments[i]);
                        parentID = -1;
                    }
                    else
                    {
                        parentID = int.Parse(parentSegment);
                    }
                    routeFlag = true;

                }
                else if (routeFlag)
                {
                    routeBuilder.Append(context.Request.Url.Segments[i]);
                }
                parentFlag = context.Request.Url.Segments[i].ToLower() == "hub/";
            }
            string route = routeBuilder.ToString().ToLower().TrimEnd('/');

            string json = ReadJsonData(context);
            dynamic data = string.IsNullOrWhiteSpace(json) ? null : JObject.Parse(json);

            if (parentID > -1)
            {
                Organization org = Organizations.GetOrganization(LoginUser.Anonymous, parentID);

                if (data["Token"].ToString() != org.WebServiceID.ToString())
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    context.Response.ClearContent();
                    context.Response.End();
                    return;
                }

                if (data["UserID"] != null)
                {
                    userID = (int)data["UserID"];
                    if (userID == 0) userID = -1;
                }
                else userID = -1;


                if (data["CustomerHubID"] != null)
                {
                    customerHubID = (int)data["CustomerHubID"];
                    if (customerHubID == 0) customerHubID = -1;
                }

                string searchTerm = data["q"];

                //Route to the proper method, passing ParentID and UserID (if unauthenticated -1)
                try
                {
                    ProcessSearch(context, route, parentID, userID, searchTerm, customerHubID);
                }
                catch (Exception ex)
                {
                    context.Response.ContentType = "text/html";
                    context.Response.Write(ex.Message + "<br />" + ex.StackTrace);
                }

            }
            else
            {
                
                try
                {
                    ProcessRoute(context, route, data);
                }
                catch (Exception ex)
                {
                    context.Response.ContentType = "text/html";
                    context.Response.Write(ex.Message + "<br />" + ex.StackTrace);
                }
            }
            


            context.Response.End();
        }

        private void ProcessSearch(HttpContext context, string route, int parentID, int userID, string searchTerm, int customerHubID)
        {
            switch (route)
            {
                case "search/kb": ProcessKBSearch(context, parentID, userID, searchTerm, customerHubID); break;
                case "search/wiki": ProcessWikiSearch(context, parentID, userID, searchTerm); break;
                case "search/ticket": ProcessTicketSearch(context, parentID, userID, searchTerm, customerHubID); break;
                default:
                    break;
            }
        }

        private void ProcessRoute(HttpContext context, string route, object data)
        {
            switch (route)
            {
                case "admin/gencsr": ProcessCSR(context); break;
                default:
                    break;
            }
        }


        #endregion

        private void SAMPLEProcessKBSearch(HttpContext context, int parentID, int userID)
        {
            string term = context.Request.QueryString["q"];

            //Sample code to show you how to easily get SQL to JSON
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT Name, TicketNumber, TicketID FROM Tickets WHERE TicketID = @TicketID";
            command.Parameters.AddWithValue("TicketID", 1637556);
            ExpandoObject[] tickets = SqlExecutor.GetExpandoObject(LoginUser.Anonymous, command);

            command = new SqlCommand();
            command.CommandText = "SELECT Description FROM Actions WHERE TicketID = @TicketID";
            command.Parameters.AddWithValue("TicketID", 1637556);
            ExpandoObject[] actions = SqlExecutor.GetExpandoObject(LoginUser.Anonymous, command);

            (tickets[0] as dynamic).Actions = actions;

            dynamic result = new ExpandoObject();
            result.Term = term;
            result.UserID = userID;
            result.ParentID = parentID;
            result.Tickets = tickets;

            WriteJson(context, result);
        }

        private void ProcessKBSearch(HttpContext context, int parentID, int userID, string searchTerm, int customerHubID)
        {
            SearchResults kbResults = TicketsView.GetHubSearchKBResults(searchTerm, LoginUser.Anonymous, parentID);
            List<KBSearchItem> result = GetKBResults(kbResults, LoginUser.Anonymous, userID, parentID, customerHubID);
            WriteJson(context, result);
        }

        private List<KBSearchItem> GetKBResults(SearchResults results, LoginUser loginUser, int userID, int parentID, int hubID)
        {
            bool enableCustomerSpecificKB = false;
            bool enableCustomerProductAssociation = false;
            bool enableAnonymousProductAssociation = false;

            List<KBSearchItem> items = new List<KBSearchItem>();
            int customerID = 0;
            User user = Users.GetUser(loginUser, userID);
            if (user != null) customerID = user.OrganizationID;

            CustomerHubFeatureSettings hubFeatureSettings = new CustomerHubFeatureSettings(loginUser);
            hubFeatureSettings.LoadByCustomerHubID(hubID);

            if (hubFeatureSettings.Any())
            {
                enableCustomerProductAssociation = hubFeatureSettings[0].EnableCustomerProductAssociation;
                enableAnonymousProductAssociation = hubFeatureSettings[0].EnableAnonymousProductAssociation;
                enableCustomerSpecificKB = hubFeatureSettings[0].EnableCustomerSpecificKB;
            }

            for (int i = 0; i < results.Count; i++)
            {
                results.GetNthDoc(i);
                int ticketID = int.Parse(results.CurrentItem.Filename);
                if (ticketID > 0)
                {
                    TicketsView ticketsViewHelper = new TicketsView(loginUser);
                    ticketsViewHelper.LoadHubKBByID(ticketID, parentID, customerID, enableCustomerSpecificKB, enableCustomerProductAssociation, enableAnonymousProductAssociation);

                    if (ticketsViewHelper.Any())
                    {
                        KBSearchItem item = new KBSearchItem();
                        item.HitRating = results.CurrentItem.ScorePercent;
                        item.Article = ticketsViewHelper[0].GetProxy();

                        TicketRatings ratings = new TicketRatings(loginUser);
                        ratings.LoadByTicketID(ticketID);

                        if (ratings.Any())
                        {
                            TicketRating rating = ratings[0];
                            item.VoteRating = rating.ThumbsUp;
                        }

                        items.Add(item);
                    }
                }
            }
            return items;
        }

        private void ProcessWikiSearch(HttpContext context, int parentID, int userID, string searchTerm)
        {
            SearchResults wikiResults = WikiArticlesView.GetPortalSearchWikiResults(searchTerm, LoginUser.Anonymous, parentID);
            List<WikiSearchItem> result = GetWikiResults(wikiResults, LoginUser.Anonymous, userID, parentID);
            WriteJson(context, result);
        }

        private List<WikiSearchItem> GetWikiResults(SearchResults results, LoginUser loginUser, int userID, int parentID)
        {
            List<WikiSearchItem> items = new List<WikiSearchItem>();
            WikiArticlesView articles = new WikiArticlesView(loginUser);
            articles.LoadByOrganizationID(parentID);

            for (int i = 0; i < results.Count; i++)
            {
                results.GetNthDoc(i);
                int articleID = int.Parse(results.CurrentItem.Filename);
                if (articleID > 0)
                {
                    WikiSearchItem item = new WikiSearchItem();
                    var article = articles.FindByArticleID(articleID);

                    if (article != null)
                    {
                        item.HitRating = results.CurrentItem.ScorePercent;
                        item.Article = article.GetProxy();
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        private void ProcessTicketSearch(HttpContext context, int parentID, int userID, string searchTerm, int customerHubID)
        {
            SearchResults ticketResults = TicketsView.GetHubSearchTicketResults(searchTerm, LoginUser.Anonymous, parentID);
            List<TicketSearchItem> result = GetTicketResults(ticketResults, LoginUser.Anonymous, userID, parentID, customerHubID);
            WriteJson(context, result);
        }

        private List<TicketSearchItem> GetTicketResults(SearchResults results, LoginUser loginUser, int contactID, int parentID, int customerHubID)
        {

            List<TicketSearchItem> items = new List<TicketSearchItem>();
            TicketLoadFilter filters = new TicketLoadFilter();

            int customerID = 0;
            User user = Users.GetUser(loginUser, contactID);
            if (user != null)
            {
                customerID = user.OrganizationID;
                Organizations orgHelper = new Organizations(loginUser);
                orgHelper.LoadByUnknownCompany(parentID);

                if (orgHelper.Any())
                {
                    if (orgHelper[0].OrganizationID == user.OrganizationID)
                    {
                        filters.ContactID = contactID;
                    }
                }

            }

            filters.CustomerID = user.OrganizationID;
            filters.IsVisibleOnPortal = true;
            filters.ForumCategoryID = null;

            if (customerHubID != -1)
            {
                CustomerHubFeatureSettings hubFeatureSettings = new CustomerHubFeatureSettings(loginUser);
                hubFeatureSettings.LoadByCustomerHubID(customerHubID);

                if (hubFeatureSettings.Any() && hubFeatureSettings[0].EnableProductFamilyFiltering)
                {
                    CustomerHubs customerHub = new CustomerHubs(loginUser);
                    customerHub.LoadByCustomerHubID(customerHubID);
                    if (customerHub.Any())
                    {
                        filters.ProductFamilyID = customerHub[0].ProductFamilyID;
                    }
                }
            }

            TicketsView ticketsViewHelper = new TicketsView(loginUser);
            ticketsViewHelper.LoadHubtickets(loginUser, contactID, parentID, filters, null, 0, 100000000);

            for (int i = 0; i < results.Count; i++)
            {
                results.GetNthDoc(i);
                int ticketID = int.Parse(results.CurrentItem.Filename);
                if (ticketID > 0)
                {
                    if (ticketsViewHelper.Any())
                    {
                        foreach (var ticket in ticketsViewHelper)
                        {
                            if (ticket.TicketID == ticketID)
                            {
                                TicketSearchItem item = new TicketSearchItem();
                                item.Ticket = new TicketsViewItemProxy();
                                item.Ticket.TicketID = ticket.TicketID;
                                item.Ticket.Name = ticket.Name;
                                item.Ticket.TicketNumber = ticket.TicketNumber;
                                item.Ticket.DateCreated = ticket.DateCreated;

                                item.HitRating = results.CurrentItem.ScorePercent;

                                items.Add(item);
                            }
                        }
                    }
                }
            }

            return items;
        }



        private void ProcessCSR(HttpContext context)
        {
          
            string args = @"pkcs12 -export -out test2.pfx -inkey key.cer -in cert.cer -certfile int.cer -passout pass:Muroc2008!";
            context.Response.Write(args);

            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.Arguments = args;
            p.StartInfo.FileName = Path.Combine(context.Request.MapPath("~/openssl"), "openssl.exe");
            p.StartInfo.WorkingDirectory = context.Request.MapPath("~/openssl");
            p.Start();
       
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            context.Response.ContentType = "text/html";

            context.Response.Write(output);


        }

        #region classes

        public class TicketSearchItem
        {
            public int HitRating { get; set; }
            public TicketsViewItemProxy Ticket { get; set; }
        }

        public class KBSearchItem
        {
            public int HitRating { get; set; }
            public int? VoteRating { get; set; }
            public int Views { get; set; }
            public TicketsViewItemProxy Article { get; set; }
        }

        public class WikiSearchItem
        {
            public int HitRating { get; set; }
            public WikiArticlesViewItemProxy Article { get; set; }
        }

        #endregion


        #region Utility Methods

        private void WriteJson(HttpContext context, object payload)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.AddHeader("Expires", "-1");
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Write(JsonConvert.SerializeObject(payload));
        }

        private static string ReadJsonData(HttpContext context)
        {
            string result = "";
            using (Stream receiveStream = context.Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
            }
            return result;
        }

        #endregion



    }
}
