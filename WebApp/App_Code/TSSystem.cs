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
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using System.Globalization;
using System.Security.Cryptography;
using System.Net;
using Newtonsoft.Json;
using System.Dynamic;


namespace TSWebServices
{

    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TSSystem : System.Web.Services.WebService
    {

        public TSSystem()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public string UtilTest()
        {
            Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), TSAuthentication.OrganizationID);
            return org.IsInBusinessHours(DateTime.UtcNow).ToString();
        }

        [WebMethod]
        public UserProxy GetCurrentUser()
        {
			UserProxy user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).GetProxy();
			user.CryptedPassword = string.Empty;
			return user;
        }

		[WebMethod]
		public List<int> GetCurrentUserGroups()
		{
			return TSAuthentication.GetUserGroups();
		}

        [WebMethod]
        public string GetDomains()
        {
            dynamic payload = new ExpandoObject();
            payload.AppUrl = SystemSettings.GetAppUrl();
            payload.PortalUrl = SystemSettings.GetPortalUrl();
            payload.DomainName = SystemSettings.GetDomain();
            payload.EmailDomain = SystemSettings.GetEmailDomain();
            return JsonConvert.SerializeObject(payload);
        }


        [WebMethod]
        public OrganizationProxy GetCurrentOrganization()
        {
            return TSAuthentication.GetOrganization(TSAuthentication.GetLoginUser()).GetProxy();
        }

        [WebMethod]
        public Culture GetCulture()
        {
            Culture result = new Culture();
            CultureInfo culture = TSAuthentication.GetLoginUser().CultureInfo;
            result.DateTimePattern.LongDate = culture.DateTimeFormat.LongDatePattern;
            result.DateTimePattern.LongDateLongTime = culture.DateTimeFormat.FullDateTimePattern;
            result.DateTimePattern.LongDateShortTime = culture.DateTimeFormat.LongDatePattern + " " + culture.DateTimeFormat.ShortTimePattern;
            result.DateTimePattern.LongTime = culture.DateTimeFormat.LongTimePattern;
            result.DateTimePattern.ShortDate = culture.DateTimeFormat.ShortDatePattern;
            result.DateTimePattern.ShortDateLongTime = culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.LongTimePattern;
            result.DateTimePattern.ShortDateShortTime = culture.DateTimeFormat.ShortDatePattern + " " + culture.DateTimeFormat.ShortTimePattern;
            result.DateTimePattern.ShortTime = culture.DateTimeFormat.ShortTimePattern;
            return result;
        }

        [WebMethod]
        public ChatUserSettingProxy GetCurrentUserChatSettings()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            ChatUserSetting setting = ChatUserSettings.GetChatUserSetting(loginUser, TSAuthentication.UserID);
            if (setting == null)
            {
                setting = (new ChatUserSettings(loginUser)).AddNewChatUserSetting();
                setting.UserID = TSAuthentication.UserID;
            }
            return setting.GetProxy();
        }

        [WebMethod]
        public void SignOut()
        {

            try
            {
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Users, TSAuthentication.UserID, "Logged out");
                TSEventLog.WriteEvent(TSEventLogEventType.LogoutSuccess, HttpContext.Current.Request, TSAuthentication.GetLoginUser().GetUser(), TSAuthentication.GetLoginUser().GetOrganization());
            }
            catch (Exception)
            {
            }

            HttpContext.Current.Response.Cookies["sl"].Value = null;
            HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Value = null;
            //HttpContext.Current.Session.Clear();
            //HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }
        [WebMethod]
        public TsMenuItem GetNewTicketViewMenuItem(int reportID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Report report = Reports.GetReport(loginUser, reportID, loginUser.UserID);

            string data = @"{{""ContentUrl"":""{0}"",""PaneInfoUrl"":""{1}""}}";
            return new TsMenuItem("tickettype", "mniTicketView_" + report.ReportID.ToString(), report.Name, "vcr/1_9_0/images/nav/20/tickets.png", string.Format(data, "vcr/1_9_0/Pages/TicketView.html?ReportID=" + report.ReportID.ToString(), "vcr/1_9_0/PaneInfo/Reports.html"));
        }

        [WebMethod]
        public TsMenuItem[] GetMainMenuItems()
        {
            List<TsMenuItem> items = new List<TsMenuItem>();
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            Organization org = TSAuthentication.GetOrganization(loginUser);
            User user = TSAuthentication.GetUser(loginUser);
            string data = @"{{""ContentUrl"":""{0}"",""PaneInfoUrl"":""{1}""}}";


            if (org.ParentID != null)
            {
                if (user.ShowWelcomePage == true && user.IsSystemAdmin == true)
                {
                    items.Add(new TsMenuItem("welcome", "mniWelcome", "Getting Started", "vcr/1_9_0/images/nav/20/GettingStarted.png", string.Format(data, "vcr/1_9_0/Pages/Welcome.html", "vcr/1_9_0/PaneInfo/Welcome.html")));
                }

                if (IsMenuItemActive(user, "mniDashboard"))
                {
                    items.Add(new TsMenuItem("dashboard", "mniDashboard", "Dashboard", "vcr/1_9_0/images/nav/20/Dashboard.png", string.Format(data, "vcr/1_9_0/Pages/Dashboard.html", "vcr/1_9_0/PaneInfo/Dashboard.html")));
                }

                if (IsMenuItemActive(user, "mniMyTickets"))
                {
                    TsMenuItem myItem = new TsMenuItem("mytickets", "mniMyTickets", "My Tickets", "vcr/1_9_0/images/nav/20/mytickets.png", string.Format(data, "vcr/1_9_0/Pages/TicketTabs.html?UserID=" + TSAuthentication.UserID, "vcr/1_9_0/PaneInfo/MyTickets.html"));
                    items.Add(myItem);

                    Reports privateTicketViews = new Reports(TSAuthentication.GetLoginUser());
                    privateTicketViews.LoadAllPrivateTicketViews(TSAuthentication.OrganizationID, TSAuthentication.UserID);

                    if (!privateTicketViews.IsEmpty)
                    {
                        foreach (Report report in privateTicketViews)
                        {
                            myItem.AddItem(new TsMenuItem("tickettype", "mniTicketView_" + report.ReportID.ToString(), report.Name, "vcr/1_9_0/images/nav/20/tickets.png", string.Format(data, "vcr/1_9_0/Pages/TicketView.html?ReportID=" + report.ReportID.ToString(), "vcr/1_9_0/PaneInfo/Reports.html")));
                        }
                    }
                }

                if (IsMenuItemActive(user, "mniTickets"))
                {
                    TsMenuItem ticketItem = new TsMenuItem("tickets", "mniTickets", "All Tickets", "vcr/1_9_0/images/nav/20/tickets.png", string.Format(data, "vcr/1_9_0/Pages/TicketTabs.html", "vcr/1_9_0/PaneInfo/Tickets.html"));
                    items.Add(ticketItem);

                    Reports publicTicketViews = new Reports(TSAuthentication.GetLoginUser());
                    publicTicketViews.LoadAllPublicTicketViews(TSAuthentication.OrganizationID, TSAuthentication.UserID);

                    if (!publicTicketViews.IsEmpty)
                    {
                        foreach (Report report in publicTicketViews)
                        {
                            ticketItem.AddItem(new TsMenuItem("tickettype", "mniTicketView_" + report.ReportID.ToString(), report.Name, "vcr/1_9_0/images/nav/20/tickets.png", string.Format(data, "vcr/1_9_0/Pages/TicketView.html?ReportID=" + report.ReportID.ToString(), "vcr/1_9_0/PaneInfo/Reports.html")));
                        }
                    }

                    TicketTypes ticketTypes = new TicketTypes(loginUser);
                    ticketTypes.LoadByOrganizationID(TSAuthentication.OrganizationID, org.ProductType);
                    foreach (TicketType ticketType in ticketTypes)
                    {
                        if (ticketType.IsActive) {
                				string mniID = "mniTicketType_" + ticketType.TicketTypeID.ToString();
                				if (IsMenuItemActive(user, mniID))
                				{
                    				ticketItem.AddItem(new TsMenuItem("tickettype", mniID, ticketType.Name, ticketType.IconUrl, string.Format(data, "vcr/1_9_0/Pages/TicketTabs.html?TicketTypeID=" + ticketType.TicketTypeID.ToString(), "vcr/1_9_0/PaneInfo/Tickets.html")));
                				}
            			}
                    }
                }

                if (org.ProductType == ProductType.Enterprise && IsMenuItemActive(user, "mniTasks"))
                {
                    items.Add(new TsMenuItem("tasks", "mniTasks", "Tasks", "vcr/1_9_0/images/nav/20/tasks.png", string.Format(data, "vcr/1_9_0/Pages/tasks.html", "vcr/1_9_0/PaneInfo/Tasks.html")));
                }

                if (IsMenuItemActive(user, "mniTicketTags"))
                {
                    items.Add(new TsMenuItem("tickettags", "mniTicketTags", "Ticket Tags", "vcr/1_9_0/images/nav/20/tag.png", string.Format(data, "Frames/TicketTags.aspx", "vcr/1_9_0/PaneInfo/TicketTags.html")));
                }

                if (IsMenuItemActive(user, "mniKB"))
                {
                    items.Add(new TsMenuItem("kb", "mniKB", "Knowledge Base", "vcr/1_9_0/images/nav/20/knowledge.png", string.Format(data, "vcr/1_9_0/Pages/KnowledgeBase.html", "vcr/1_9_0/PaneInfo/Knowledge.html")));
                }

                if (IsMenuItemActive(user, "mniForum") && org.UseForums == true)
                {
                    items.Add(new TsMenuItem("forum", "mniForum", "Community", "vcr/1_9_0/images/nav/20/forum.png", string.Format(data, "vcr/1_9_0/Pages/TicketGrid.html?tf_ForumCategoryID=-1", "vcr/1_9_0/PaneInfo/Community.html")));
                }

                if (org.ProductType != ProductType.Express && IsMenuItemActive(user, "mniWiki"))
                {
                    items.Add(new TsMenuItem("wiki", "mniWiki", "Wiki", "vcr/1_9_0/images/nav/20/wiki.png", string.Format(data, "vcr/1_9_0/Pages/Wiki_view.html", "vcr/1_9_0/PaneInfo/Wiki_view.html")));
                }

                if (IsMenuItemActive(user, "mniSearch"))
                {
                    items.Add(new TsMenuItem("search", "mniSearch", "Search", "vcr/1_9_0/images/nav/20/search.png", string.Format(data, "vcr/1_9_0/Pages/Search.html", "vcr/1_9_0/PaneInfo/Search.html")));
                }

        if (user.IsChatUser && org.ChatSeats > 0 && IsMenuItemActive(user, "mniChat"))
        {
            //old chat: items.Add(new TsMenuItem("chat", "mniChat", "Customer Chat", "vcr/1_9_0/images/nav/20/chat.png", string.Format(data, "Frames/Chat.aspx", "vcr/1_9_0/PaneInfo/Chat.html")));
            items.Add(new TsMenuItem("chat", "mniChat", "Customer Chat", "vcr/1_9_0/images/nav/20/chat.png", string.Format(data, "vcr/1_9_0/Pages/AgentCustomerChat.html", "vcr/1_9_0/PaneInfo/Chat.html")));
        }

                if (org.ProductType != ProductType.Express && IsMenuItemActive(user, "mniWC2"))
                {
                    items.Add(new TsMenuItem("wc2", "mniWC2", "Water Cooler", "vcr/1_9_0/images/nav/20/watercooler.png", string.Format(data, "vcr/1_9_0/Pages/WaterCooler.html", "vcr/1_9_0/PaneInfo/WaterCooler.html")));
                }

                items.Add(new TsMenuItem("calender", "mniCalender", "Calendar", "vcr/1_9_0/images/nav/20/calendar.png", string.Format(data, "vcr/1_9_0/Pages/Calendar.html", "vcr/1_9_0/PaneInfo/Calendar.html")));

                if (IsMenuItemActive(user, "mniUsers"))
                {
                    items.Add(new TsMenuItem("users", "mniUsers", "Users", "vcr/1_9_0/images/nav/20/users.png", string.Format(data, "vcr/1_9_0/Pages/Users.html", "vcr/1_9_0/PaneInfo/Users.html")));
                }

                if (IsMenuItemActive(user, "mniGroups"))
                {
                    items.Add(new TsMenuItem("groups", "mniGroups", "Groups", "vcr/1_9_0/images/nav/20/groups.png", string.Format(data, "vcr/1_9_0/Pages/Groups.html", "vcr/1_9_0/PaneInfo/Groups.html")));
                    //items.Add(new TsMenuItem("groups1", "mniGroups1", "Groups1", "vcr/1_9_0/images/nav/20/groups.png", string.Format(data, "Frames/Groups.aspx", "vcr/1_9_0/PaneInfo/Groups.html")));
                }

                if ((org.ProductType == ProductType.Enterprise || org.ProductType == ProductType.HelpDesk) && IsMenuItemActive(user, "mniCustomers"))
                {
                    //items.Add(new TsMenuItem("customers1", "mniCustomers1", "Customers1", "vcr/1_9_0/images/nav/20/customers.png", string.Format(data, "Frames/Organizations.aspx", "vcr/1_9_0/PaneInfo/Customers.html")));
                    items.Add(new TsMenuItem("customers", "mniCustomers", "Customers", "vcr/1_9_0/images/nav/20/customers.png", string.Format(data, "vcr/1_9_0/Pages/Customers.html", "vcr/1_9_0/PaneInfo/Customers.html")));
                }


                if ((org.ProductType == ProductType.Enterprise || org.ProductType == ProductType.BugTracking) && IsMenuItemActive(user, "mniProducts"))
                {
                    items.Add(new TsMenuItem("products", "mniProducts", "Products", "vcr/1_9_0/images/nav/20/products.png", string.Format(data, "vcr/1_9_0/Pages/Products.html", "vcr/1_9_0/PaneInfo/Products.html")));
                    //items.Add(new TsMenuItem("products", "mniProducts", "Products", "vcr/1_9_0/images/nav/20/products.png", string.Format(data, "Frames/Products.aspx", "vcr/1_9_0/PaneInfo/Products.html")));
                    //if (TSAuthentication.OrganizationID == 1078 || TSAuthentication.OrganizationID == 13679 || TSAuthentication.OrganizationID == 362372 || TSAuthentication.OrganizationID == 428340)
                    //{
                    //  items.Add(new TsMenuItem("products", "mniProducts1", "New Products", "vcr/1_9_0/images/nav/20/products.png", string.Format(data, "vcr/1_9_0/Pages/Products.html", "vcr/1_9_0/PaneInfo/Products.html")));
                    //}
                }

                if (org.IsInventoryEnabled && IsMenuItemActive(user, "mniInventory"))
                {
                    //items.Add(new TsMenuItem("inventory", "mniInventory", "Inventory", "vcr/1_9_0/images/nav/20/inventory.png", string.Format(data, "Inventory/Inventory.aspx", "vcr/1_9_0/PaneInfo/Inventory.html")));
                    items.Add(new TsMenuItem("inventory", "mniInventory", "Inventory", "vcr/1_9_0/images/nav/20/inventory.png", string.Format(data, "vcr/1_9_0/Pages/Inventory.html", "vcr/1_9_0/PaneInfo/Inventory.html")));
                }

                if ((user.IsSystemAdmin || !org.AdminOnlyReports) && IsMenuItemActive(user, "mniReports"))
                {
                    items.Add(new TsMenuItem("reports", "mniReports", "Reports", "vcr/1_9_0/images/nav/20/reports.png", string.Format(data, "vcr/1_9_0/pages/reports.html", "vcr/1_9_0/PaneInfo/Reports.html")));
                }

        if (user.IsSystemAdmin && IsMenuItemActive(user, "mniAdmin"))
          items.Add(new TsMenuItem("admin", "mniAdmin", "Admin", "vcr/1_9_0/images/nav/20/admin.png", string.Format(data, "Frames/Admin.aspx", "vcr/1_9_0/PaneInfo/Admin.html")));
        
        if (TSAuthentication.OrganizationID == 1078)
        {
                    TsMenuItem utils = new TsMenuItem("utils", "mniUtils", "Utilities", "vcr/1_9_0/images/nav/20/admin.png", string.Format(data, "vcr/1_9_0/Utils/Utils.html", "vcr/1_9_0/PaneInfo/Admin.html"));
                    items.Add(utils);
                    utils.AddItem(new TsMenuItem("utils", "utils-accounts", "Accounts", "vcr/1_9_0/images/nav/20/admin.png", string.Format(data, "vcr/1_9_0/Utils/Accounts.html", "vcr/1_9_0/PaneInfo/Admin.html")));
                    utils.AddItem(new TsMenuItem("utils", "utils-tickets", "Tickets", "vcr/1_9_0/images/nav/20/admin.png", string.Format(data, "vcr/1_9_0/Utils/Tickets.html", "vcr/1_9_0/PaneInfo/Admin.html")));
                    utils.AddItem(new TsMenuItem("utils", "utils-organizations", "Organizations", "vcr/1_9_0/images/nav/20/admin.png", string.Format(data, "vcr/1_9_0/Utils/Organizations.html", "vcr/1_9_0/PaneInfo/Admin.html")));
                    utils.AddItem(new TsMenuItem("utils", "utils-devflow", "Dev Workflow", "vcr/1_9_0/images/nav/20/admin.png", string.Format(data, "https://app.teamsupport.com/wiki/justarticle.aspx?Organizationid=1078&ArticleID=44728", "vcr/1_9_0/PaneInfo/Admin.html")));
                    if (user.UserID == 34 || user.UserID == 47 || user.UserID == 4759191)
                        utils.AddItem(new TsMenuItem("utils", "utils-reporttest", "Custom Reports", "vcr/1_9_0/images/nav/20/admin.png", string.Format(data, "vcr/1_9_0/Utils/ReportTest.html", "vcr/1_9_0/PaneInfo/Admin.html")));

                }

            }
            else
            {
                items.Add(new TsMenuItem("tsusers", "mniUsers", "System Users", "vcr/1_9_0/images/nav/20/users.png", string.Format(data, "Frames/Users.aspx", "vcr/1_9_0/PaneInfo/Users.html")));
                items.Add(new TsMenuItem("tscustomers", "mniCustomers", "System Customers", "vcr/1_9_0/images/nav/20/customers.png", string.Format(data, "Frames/Organizations.aspx", "vcr/1_9_0/PaneInfo/Organizations.html")));
            }

            return items.ToArray();
        }

        private static bool IsMenuItemActive(User user, string item)
        {
            if (user.MenuItems == null) return true;
            return user.MenuItems.IndexOf(item) > -1;
        }

        [WebMethod]
        public void LogException(string message, string location)
        {
            ExceptionLogs.AddLog(TSAuthentication.GetLoginUser(), "Javascript Error", message, location, HttpContext.Current.Request.Url.ToString(), HttpContext.Current.Request.Headers.ToString(), HttpContext.Current.Request.UserAgent);
        }

        [WebMethod]
        public string GetMainTabs()
        {
            string data = Settings.UserDB.ReadString("main-tabs", "");
            if (data == "") return null;
            dynamic tabs = JsonConvert.DeserializeObject(data);

            foreach (dynamic tab in tabs)
            {
                try
                {
                    if (tab.TabType == "company")
                    {
                        int id = int.Parse(tab.ID.ToString());
                        Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), id);
                        tab.Caption = DataUtils.EllipseString(org.Name, 13);
                    }
                    else if (tab.TabType == "contact")
                    {
                        int id = int.Parse(tab.ID.ToString());
                        User user = Users.GetUser(TSAuthentication.GetLoginUser(), id);
                        tab.Caption = DataUtils.EllipseString(user.FirstLastName, 13);
                    }
                }
                catch (Exception)
                {

                }
            }
            return JsonConvert.SerializeObject(tabs);
        }

        [WebMethod]
        public ServiceProxy[] GetServices()
        {
            if (TSAuthentication.OrganizationID != 1078) return null;
            Services services = new Services(TSAuthentication.GetLoginUser());
            services.LoadAll();
            return services.GetServiceProxies();
        }

        [WebMethod]
        public void SetMainTabs(string data)
        {
            Settings.UserDB.WriteString("main-tabs", data);
        }

        [WebMethod]
        public void UpdateLastWaterCoolerID()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = Users.GetUser(loginUser, TSAuthentication.UserID);
            user.LastWaterCoolerID = WaterCooler.GetLastMessageID(loginUser);
            user.Collection.Save();
        }

        [WebMethod]
        public int GetChatInterval()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organization org = TSAuthentication.GetOrganization(loginUser);
            return ChatRequests.GetRequestCountInLastDays(loginUser, loginUser.OrganizationID, 180) > 0 || org.DateCreated.AddDays(-30) > DateTime.UtcNow ? 10000 : 30000;
        }

        [WebMethod]
        public TsChatUpdate GetChatUpdate(int lastChatMessageID, int lastChatRequestID)
        {
            TsChatUpdate update = new TsChatUpdate();
            try
            {
                User user = Users.GetUser(LoginUser.Anonymous, TSAuthentication.UserID);
                user.UpdatePing();

                LoginUser loginUser = new LoginUser(user.UserID, user.OrganizationID, null);

                List<GrowlMessage> chatMessageGrowl = new List<GrowlMessage>();
                List<GrowlMessage> chatRequestGrowl = new List<GrowlMessage>();

                update.LastChatRequestID = lastChatRequestID;
                update.LastChatMessageID = lastChatMessageID;
                update.ChatMessageCount = 0;
                update.ChatRequestCount = 0;

                if (user.IsChatUser && ChatUserSettings.GetSetting(loginUser, user.UserID).IsAvailable)
                {
                    ChatMessages chatMessages = new ChatMessages(loginUser);
                    update.ChatMessageCount = chatMessages.GetUnreadMessageCount(loginUser.UserID, ChatParticipantType.User);
                    update.ChatRequestCount = user.IsChatUser ? ChatRequests.GetWaitingRequestCount(loginUser, loginUser.UserID, loginUser.OrganizationID) : 0;

                    if (lastChatMessageID < 0) update.LastChatMessageID = ChatMessages.GetLastMessageID(loginUser);
                    chatMessages.LoadUnpreviewedMessages(loginUser.UserID, update.LastChatMessageID);

                    foreach (ChatMessage chatMessage in chatMessages)
                    {
                        chatMessageGrowl.Add(new GrowlMessage(chatMessage.Message, chatMessage.PosterName, "ui-state-active"));
                        update.LastChatMessageID = chatMessage.ChatMessageID;
                    }

                    ChatRequests requests = new ChatRequests(loginUser);
                    requests.LoadNewWaitingRequests(loginUser.UserID, loginUser.OrganizationID, lastChatRequestID);

                    foreach (ChatRequest chatRequest in requests)
                    {
                        chatRequestGrowl.Add(new GrowlMessage(string.Format("{0} {1} is requesting a chat!", chatRequest.Row["FirstName"].ToString(), chatRequest.Row["LastName"].ToString()), "Chat Request", "ui-state-error"));
                        update.LastChatRequestID = chatRequest.ChatRequestID;
                    }
                }

                update.NewChatMessages = chatMessageGrowl.ToArray();
                update.NewChatRequests = chatRequestGrowl.ToArray();
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "Main Chat Upate");
            }
            return update;
        }

        [WebMethod]
        public TsMainPageUpdate GetUserStatusUpdate(string sessionID)
        {
            TsMainPageUpdate update = new TsMainPageUpdate();
            try
            {
                update.IsDebug = TSAuthentication.OrganizationID == 1078 || TSAuthentication.IsBackdoor;
                update.IsExpired = false;

                using (SqlConnection connection = new SqlConnection(LoginUser.Anonymous.ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = "SELECT EnforceSingleSession, SessionID, IsActive, MarkDeleted FROM Users WHERE UserID = @UserID";
                    command.Parameters.AddWithValue("UserID", TSAuthentication.UserID);
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
                    if (reader.Read())
                    {
                        if (!(bool)reader[2] || (bool)reader[3])
                        {
                            update.IsExpired = true;
                        }
                        else if ((bool)reader[0] && reader[1] != DBNull.Value)
                        {
                            if (sessionID != null && sessionID.ToLower() != reader[1].ToString().ToLower() && !TSAuthentication.IsBackdoor)
                            {
                                update.IsExpired = true;
                            }
                        }

                    }
                    else
                    {
                    }
                    reader.Close();
                }

                update.RefreshID = int.Parse(SystemSettings.ReadString(LoginUser.Anonymous, "RefreshID", "-1"));
                update.ExpireTime = TSAuthentication.Ticket.Expiration.ToShortTimeString();
                update.Version = GetVersion() + "." + GetRevision();
                //update.IsIdle = user.DateToUtc(user.LastActivity).AddMinutes(20) < DateTime.UtcNow;
                update.MyUnreadTicketCount = Tickets.GetMyOpenUnreadTicketCount(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);
            }
            catch (Exception ex)
            {
                ex.Data["SessionID"] = sessionID;
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "GetUserStatusUpdate");
            }
            return update;

        }

        private string GetRevision()
        {
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("~/revision.txt")))
                {
                    String line = sr.ReadToEnd();
                    return line;
                }
            }
            catch (Exception e)
            {
                return "0";
            }
        }

        private string GetVersion()
        {
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("~/version.txt")))
                {
                    String line = sr.ReadToEnd();
                    return line;
                }
            }
            catch (Exception e)
            {
                return "0";
            }
        }

        [WebMethod(false)]
        public string GetPendoOptions()
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = Users.GetUser(loginUser, TSAuthentication.UserID);
            Organization org = Organizations.GetOrganization(loginUser, TSAuthentication.OrganizationID);
            SqlCommand command;
            DataTable table;

            dynamic result = new ExpandoObject();
            result.apiKey = SystemSettings.ReadString(loginUser, "PendoKey", "NO API IN SYSTEMSETTINGS");
            result.usePendoAgentAPI = false;
            result.visitor = new ExpandoObject();
            result.visitor.id = user.UserID;
            result.visitor.email = user.Email;
            result.visitor.role = user.Title;
            result.visitor.name = user.FirstLastName;
            result.visitor.dateCreated = user.DateCreated;
            //result.visitor.lastLogin = 
            result.visitor.isAdmin = user.IsSystemAdmin;

            command = new SqlCommand("SELECT COUNT(*) FROM Tickets WHERE OrganizationID = @OrganizationID AND UserID = @UserID");
            command.Parameters.AddWithValue("UserID", user.UserID);
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.visitor.assignedTickets = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            command = new SqlCommand("SELECT COUNT(*) FROM Tickets WHERE OrganizationID = @OrganizationID AND CreatorID = @UserID");
            command.Parameters.AddWithValue("UserID", user.UserID);
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.visitor.ticketsCreated = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            command = new SqlCommand("SELECT COUNT(*) FROM Actions WHERE CreatorID = @UserID");
            command.Parameters.AddWithValue("UserID", user.UserID);
            //result.visitor.actionsCreated = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            //result.visitor.groups = 
            result.visitor.isChatUser = user.IsChatUser;

            result.account = new ExpandoObject();
            result.account.id = org.OrganizationID;
            result.account.name = org.Name;
            result.account.planLevel = org.UserSeats == 100 ? "Trial" : "Paying";
            result.account.creationDate = org.DateCreated;
            result.account.isActive = org.IsActive;
            //result.account.lastLogin =
            result.account.seatCount = org.UserSeats;
            result.account.apiEnabled = org.IsApiEnabled;
            command = new SqlCommand(
                @"SELECT COUNT(*) AS Cnt, TicketSource FROM Tickets 
                WHERE OrganizationID = @OrganizationID
                AND TicketSource IS NOT NULL
                AND TicketSource <> ''
                GROUP BY TicketSource
                ");
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            table = SqlExecutor.ExecuteQuery(loginUser, command);
            result.account.ticketsCreatedByEmail = 0;
            result.account.ticketsCreatedByFaceBook = 0;
            result.account.ticketsCreatedByAgent = 0;
            result.account.ticketsCreatedByForum = 0;
            result.account.ticketsCreatedByWeb = 0;
            result.account.ticketsCreatedByChatOffline = 0;
            result.account.ticketsCreatedByMobile = 0;
            result.account.ticketsCreatedByChat = 0;

            foreach (DataRow row in table.Rows)
            {
                try
                {
                    switch (row[1].ToString().ToLower())
                    {
                        case "forum": result.account.ticketsCreatedByForum = (int)row[0]; break;
                        case "agent": result.account.ticketsCreatedByAgent = (int)row[0]; break;
                        case "web": result.account.ticketsCreatedByWeb = (int)row[0]; break;
                        case "facebook": result.account.ticketsCreatedByFaceBook = (int)row[0]; break;
                        case "chatoffline": result.account.ticketsCreatedByChatOffline = (int)row[0]; break;
                        case "mobile": result.account.ticketsCreatedByMobile = (int)row[0]; break;
                        case "email": result.account.ticketsCreatedByEmail = (int)row[0]; break;
                        case "chat": result.account.ticketsCreatedByChat = (int)row[0]; break;
                        default:
                            break;
                    }

                }
                catch (Exception)
                {
                }
            }

            command = new SqlCommand("SELECT COUNT(*) FROM Tickets WHERE OrganizationID = @OrganizationID AND IsKnowledgeBase = 1");
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.account.kbCount = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            command = new SqlCommand("SELECT COUNT(*) FROM Tickets WHERE OrganizationID = @OrganizationID");
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.account.ticketCount = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            command = new SqlCommand("SELECT COUNT(*) FROM Organizations WHERE ParentID = @OrganizationID");
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.account.customerCount = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            command = new SqlCommand("SELECT COUNT(*) FROM CustomFields WHERE OrganizationID = @OrganizationID");
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.account.customFieldCount = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE OrganizationID = @OrganizationID AND MarkDeleted = 0");
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.account.actualUsers = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE OrganizationID = @OrganizationID AND IsSystemAdmin = 1 AND MarkDeleted = 0");
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.account.adminCount = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            command = new SqlCommand("SELECT COUNT(*) FROM Imports WHERE OrganizationID = @OrganizationID");
            command.Parameters.AddWithValue("OrganizationID", org.OrganizationID);
            result.account.importCount = (int)SqlExecutor.ExecuteScalar(loginUser, command);

            result.account.podName = SystemSettings.GetPodName();
            result.account.productType = org.ProductType.ToString();
            return JsonConvert.SerializeObject(result);

        }

        [WebMethod(false)]
        public void UpdateLastActivity()
        {
            TSAuthentication.SlideExpiration();

            User user = Users.GetUser(LoginUser.Anonymous, TSAuthentication.UserID);
            if (user == null)
                return;
            user.LastActivity = DateTime.UtcNow;
            user.Collection.Save();
        }

        [WebMethod]
        public AutocompleteItem[] GetLookupValues(int reportTableFieldID, string term)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            Dictionary<int, string> values = LookupField.GetValues(TSAuthentication.GetLoginUser(), reportTableFieldID, term, 10);
            result.Add(new AutocompleteItem("Unassigned", "-1"));
            if (values != null)
            {
                foreach (KeyValuePair<int, string> pair in values)
                {
                    result.Add(new AutocompleteItem(pair.Value, pair.Key.ToString()));
                }
            }
            return result.ToArray();
        }

        [WebMethod]
        public AutocompleteItem[] GetLookupDisplayNames(int reportTableFieldID, string term)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            Dictionary<int, string> values = LookupField.GetValues(TSAuthentication.GetLoginUser(), reportTableFieldID, term, 10);
            result.Add(new AutocompleteItem("Unassigned", "-1"));
            if (values != null)
            {
                foreach (KeyValuePair<int, string> pair in values)
                {
                    bool found = false;
                    foreach (AutocompleteItem item in result)
                    {
                        if (item.label.ToLower().Trim() == pair.Value.ToLower().Trim())
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found) result.Add(new AutocompleteItem(pair.Value, pair.Key.ToString()));
                }
            }
            return result.ToArray();
        }

        [WebMethod]
        public int GetCheckSum(ReferenceType refType)
        {
            return CheckSums.GetCheckSum(TSAuthentication.GetLoginUser(), refType);
        }

        [WebMethod]
        public ReminderProxy EditReminder(int? reminderID, ReferenceType refType, int refID, string description, DateTime dueDate, int userID)
        {
            Reminder reminder;
            if (reminderID == null)
            {
                string logdescription;
                reminder = (new Reminders(TSAuthentication.GetLoginUser())).AddNewReminder();
                reminder.OrganizationID = TSAuthentication.OrganizationID;
                User reminderUser = (User)Users.GetUser(TSAuthentication.GetLoginUser(), userID);
                if (refType == ReferenceType.Tickets)
                    logdescription = String.Format("Added Reminder for {0} , for {1}", reminderUser.FirstLastName, Tickets.GetTicketLink(TSAuthentication.GetLoginUser(), refID));
                else
                    logdescription = String.Format("Added Reminder for {0}", reminderUser.FirstLastName);

                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Tickets, refID, logdescription);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Insert, ReferenceType.Users, userID, logdescription);


            }
            else
            {
                reminder = Reminders.GetReminder(TSAuthentication.GetLoginUser(), (int)reminderID);
                if (reminder.OrganizationID != TSAuthentication.OrganizationID) return null;
            }

            User user = Users.GetUser(reminder.Collection.LoginUser, userID);
            if (user.OrganizationID != TSAuthentication.OrganizationID) return null;

            reminder.Description = description;
            reminder.RefType = refType;
            reminder.RefID = refID;
            reminder.DueDate = dueDate;
            reminder.UserID = userID;
            reminder.HasEmailSent = false;
            reminder.Collection.Save();
            return reminder.GetProxy();
        }

        [WebMethod]
        public void DismissReminder(int reminderID)
        {
            Reminder reminder = Reminders.GetReminder(TSAuthentication.GetLoginUser(), (int)reminderID);
            if (reminder.OrganizationID != TSAuthentication.OrganizationID) return;

            if (!TSAuthentication.IsSystemAdmin && reminder.UserID != TSAuthentication.UserID) return;
            reminder.IsDismissed = true;
            reminder.Collection.Save();
        }


        [WebMethod]
        public ReminderProxy GetReminder(int reminderID)
        {
            Reminder reminder = Reminders.GetReminder(TSAuthentication.GetLoginUser(), (int)reminderID);
            if (reminder.OrganizationID != TSAuthentication.OrganizationID) return null;
            return reminder.GetProxy();
        }

        [WebMethod]
        public ReminderProxy[] GetItemReminders(ReferenceType refType, int refID, int? userID)
        {
            Reminders reminders = new Reminders(TSAuthentication.GetLoginUser());
            reminders.LoadByItemAll(refType, refID, userID);
            return reminders.GetReminderProxies();
        }

        [WebMethod]
        public ReminderProxy[] GetUserReminders(int userID)
        {
            Reminders reminders = new Reminders(TSAuthentication.GetLoginUser());
            reminders.LoadByUser(userID);
            return reminders.GetReminderProxies();
        }

        [WebMethod]
        public string[] GenerateKeys()
        {
            List<string> result = new List<string>();
            result.Add(string.Format("<strong>validationKey (SHA1): </strong>{0}", DataUtils.GenerateRandomKey(128)));
            result.Add(string.Format("<strong>decryptionKey (AES): </strong>{0}", DataUtils.GenerateRandomKey(64)));
            return result.ToArray();
        }

        [WebMethod]
        public ExceptionLogViewItemProxy GetException(int exceptionLogID)
        {
            ExceptionLogViewItem item = ExceptionLogView.GetExceptionLogViewItem(TSAuthentication.GetLoginUser(), exceptionLogID);
            return item.GetProxy();
        }

        [WebMethod]
        public ExceptionLogViewItemProxy[] GetTopExceptions()
        {
            ExceptionLogView view = new ExceptionLogView(TSAuthentication.GetLoginUser());
            view.LoadTop(100);
            return view.GetExceptionLogViewItemProxies();
        }

        [WebMethod]
        public void ClearExceptionLogs(bool onlyCrmApi)
        {
            if (onlyCrmApi)
            {
                SqlExecutor.ExecuteNonQuery(TSAuthentication.GetLoginUser(), "DELETE FROM ExceptionLogs WHERE URL = 'CRM Processor' OR URL = 'API'");
            }
            else
            {
                SqlExecutor.ExecuteNonQuery(TSAuthentication.GetLoginUser(), "TRUNCATE TABLE ExceptionLogs");
            }
        }

        [WebMethod]
        public int GetExceptionLogCount()
        {
            return ExceptionLogs.GetCount();
        }

        [WebMethod]
        public void CreateNewException()
        {
            try
            {
                throw new Exception("This is a test exception.");
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(TSAuthentication.GetLoginUser(), ex, "TSSystem", "Extra Info");
            }
        }

        [WebMethod]
        public string SanitizeHtml(string text)
        {
            return HtmlUtility.Sanitize(text);
        }

        [WebMethod]
        public CustomValueProxy SaveCustomValue(int customFieldID, int refID, object value)
        {
            CustomValue customValue = CustomValues.GetValue(TSAuthentication.GetLoginUser(), customFieldID, refID);
            if (value == null)
            {
                customValue.Value = "";
                customValue.Collection.Save();
                return null;
            }

			switch (customValue.FieldType)
            {
				case CustomFieldType.DateTime:
                customValue.Value = ((DateTime)value).ToString();
					break;
				case CustomFieldType.Date:
					customValue.Value = ((DateTime)value).ToShortDateString();
					break;
				default:
                customValue.Value = DataUtils.CleanValueScript(value.ToString());
					break;
            }

            customValue.Collection.Save();
            return customValue.GetProxy();
        }

        [WebMethod]
        public void ChangeHealthCheck(bool value)
        {
            SqlExecutor.ExecuteNonQuery(TSAuthentication.GetLoginUser(), new SqlCommand(value == true ? "UPDATE Services SET HealthMaxMinutes = 20" : "UPDATE Services SET HealthMaxMinutes = 999999"));
        }

        [WebMethod]
        public int GetLatestWatercoolerCount()
        {
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            WaterCoolerView wcv = new WaterCoolerView(TSAuthentication.GetLoginUser());
            return wcv.GetLatestWatercoolerCount(user.LastLoginUtc.ToString());
        }

        [WebMethod]
        public string[] GetFontFamilies()
        {
            List<string> result = new List<string>();
            Array fontFamilyValues = Enum.GetValues(typeof(FontFamily));
            int x = 0;
            foreach (FontFamily value in fontFamilyValues)
            {
                result.Add(Enums.GetDescription(value));
                x = (int)value;
                result.Add(x.ToString());
            }
            return result.ToArray();
        }

        [WebMethod]
        public string[] GetFontSizes()
        {
            List<string> result = new List<string>();
            Array fontSizeValues = Enum.GetValues(typeof(FontSize));
            int x = 0;
            foreach (FontSize value in fontSizeValues)
            {
                result.Add(Enums.GetDescription(value));
                x = (int)value;
                result.Add(x.ToString());
            }
            return result.ToArray();
        }

    }

    [Serializable]
    public class TsMenuItem
    {
        List<TsMenuItem> _items;
        public TsMenuItem(string type, string id, string caption, string imageUrl, string data)
        {
            _items = new List<TsMenuItem>();
            ID = id;
            Type = type;
            Caption = caption;
            ImageUrl = imageUrl;
            Data = data;
        }
        public string ID { get; set; }
        public string Type { get; set; }
        public string Caption { get; set; }
        public string ImageUrl { get; set; }
        public string Data { get; set; }
        public TsMenuItem[] Items { get { return _items.ToArray(); } }
        public void AddItem(TsMenuItem item) { _items.Add(item); }
    }

    [DataContract]
    public class GrowlMessage
    {
        public GrowlMessage(string message, string title, string state)
        {
            Message = message;
            Title = title;
            State = state;
        }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string State { get; set; }
    }


    [DataContract]
    public class TsChatUpdate
    {
        [DataMember]
        public int ChatMessageCount { get; set; }
        [DataMember]
        public int ChatRequestCount { get; set; }
        [DataMember]
        public GrowlMessage[] NewChatMessages { get; set; }
        [DataMember]
        public GrowlMessage[] NewChatRequests { get; set; }
        [DataMember]
        public int LastChatMessageID { get; set; }
        [DataMember]
        public int LastChatRequestID { get; set; }
    }

    [DataContract]
    public class TsMainPageUpdate
    {
        [DataMember]
        public bool IsExpired { get; set; }
        [DataMember]
        public string ExpireTime { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public int RefreshID { get; set; }
        [DataMember]
        public bool IsIdle { get; set; }
        [DataMember]
        public bool IsDebug { get; set; }
        [DataMember]
        public int MyUnreadTicketCount { get; set; }
    }


    [DataContract]
    public class TypeAheadItem
    {
        public TypeAheadItem() { }
        public TypeAheadItem(string value, string id)
        {
            this.id = id;
            this.value = value;
        }

        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string value { get; set; }
    }

    [DataContract]
    public class AutocompleteItem
    {
        public AutocompleteItem() { }

        public AutocompleteItem(string label, string id)
        {
            this.label = label;
            this.value = label;
            this.id = id;
        }

        public AutocompleteItem(string label, string id, object data)
        {
            this.label = label;
            this.value = label;
            this.id = id;
            this.data = data;
        }

        [DataMember]
        public string label { get; set; }
        [DataMember]
        public string value { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public object data { get; set; }
    }

    [DataContract]
    public class IDList
    {
        public IDList() { }
        [DataMember]
        public List<int> List { get; set; }
    }
    [DataContract]
    public class Culture
    {
        public Culture()
        {
            DateTimePattern = new DateTimePattern();
        }

        [DataMember]
        public DateTimePattern DateTimePattern { get; set; }

    }

    [DataContract]
    public class DateTimePattern
    {
        [DataMember]
        public string ShortDate { get; set; }
        [DataMember]
        public string LongDate { get; set; }
        [DataMember]
        public string ShortTime { get; set; }
        [DataMember]
        public string LongTime { get; set; }
        [DataMember]
        public string ShortDateLongTime { get; set; }
        [DataMember]
        public string ShortDateShortTime { get; set; }
        [DataMember]
        public string LongDateShortTime { get; set; }
        [DataMember]
        public string LongDateLongTime { get; set; }
    }

}