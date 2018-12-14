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
using Telerik.Web.UI;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using System.Runtime.Serialization;
using dtSearch.Engine;



/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*************    THIS FILE IS OLD   *****************/
/*************** DO NOT USE  FOR NEW CODE ************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/
/*****************************************************/


















namespace TeamSupport.Services
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class PrivateServices : System.Web.Services.WebService
    {

        public PrivateServices()
        {

            //Uncomment the following line if using designed components
            //InitializeComponent();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetUserOrOrganization(RadComboBoxContext context)
        {
            IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

            Organizations organizations = new Organizations(UserSession.LoginUser);
            organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString(), true);

            UsersView users = new UsersView(UserSession.LoginUser);
            users.LoadByLikeName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString());

            List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
            foreach (Organization organization in organizations)
            {
                RadComboBoxItemData itemData = new RadComboBoxItemData();
                itemData.Text = organization.Name;
                itemData.Value = 'o' + organization.OrganizationID.ToString();
                list.Add(itemData);
            }

            foreach (UsersViewItem user in users)
            {
                RadComboBoxItemData itemData = new RadComboBoxItemData();
                itemData.Text = String.Format("{0}, {1} [{2}]", user.LastName, user.FirstName, user.Organization);
                itemData.Value = 'u' + user.UserID.ToString();
                list.Add(itemData);
            }

            return list.ToArray();
        }
        /*
        [WebMethod]
        public RadComboBoxItemData[] GetOrganizationByLikeName(RadComboBoxContext context)
        {
          IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;

          Organizations organizations = new Organizations(UserSession.LoginUser);
          organizations.LoadByLikeOrganizationName(UserSession.LoginUser.OrganizationID, context["FilterString"].ToString(), !UserSession.CurrentUser.IsSystemAdmin);

          List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
          foreach (Organization organization in organizations)
          {
            RadComboBoxItemData itemData = new RadComboBoxItemData();
            itemData.Text = organization.Name;
            itemData.Value = organization.OrganizationID.ToString();
            list.Add(itemData);
          }

          return list.ToArray();
        }*/

        [WebMethod]
        public RadComboBoxItemData[] GetUsers(RadComboBoxContext context)
        {
            IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
            List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
            try
            {
                Users users = new Users(UserSession.LoginUser);
                //string search = context["FilterString"].ToString();
                //users.LoadByName(search, UserSession.LoginUser.OrganizationID, true);
                users.LoadByOrganizationID(UserSession.LoginUser.OrganizationID, true);

                foreach (User user in users)
                {
                    RadComboBoxItemData itemData = new RadComboBoxItemData();
                    itemData.Text = user.FirstLastName;
                    itemData.Value = user.UserID.ToString();
                    list.Add(itemData);
                }
            }
            catch (Exception)
            {
            }
            if (list.Count < 1)
            {
                RadComboBoxItemData noData = new RadComboBoxItemData();
                noData.Text = "[No users to display.]";
                noData.Value = "-1";
                list.Add(noData);
            }

            return list.ToArray();
        }

        [WebMethod]
        public RadComboBoxItemData[] GetQuickTicket(RadComboBoxContext context)
        {

            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;

            using (SearchJob job = new SearchJob())
            {
                string searchTerm = context["FilterString"].ToString().Trim();
                job.Request = searchTerm;
                job.FieldWeights = "TicketNumber: 5000, Name: 1000";
                job.BooleanConditions = "OrganizationID::" + TSAuthentication.OrganizationID.ToString();
                job.MaxFilesToRetrieve = 25;
                job.AutoStopLimit = 100000;
                job.TimeoutSeconds = 10;
                job.SearchFlags =
                  SearchFlags.dtsSearchSelectMostRecent |
                  SearchFlags.dtsSearchStemming |
                  SearchFlags.dtsSearchDelayDocInfo;

                int num = 0;
                if (!int.TryParse(searchTerm, out num))
                {
                    job.Fuzziness = 1;
                    job.Request = job.Request + "*";
                    job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy;
                }

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
                job.IndexesToSearch.Add(DataUtils.GetTicketIndexPath(TSAuthentication.GetLoginUser()));
                job.Execute();
                SearchResults results = job.Results;


                IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
                List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
                try
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        results.GetNthDoc(i);
                        RadComboBoxItemData itemData = new RadComboBoxItemData();
                        itemData.Text = results.CurrentItem.DisplayName;
                        itemData.Value = results.CurrentItem.Filename;
                        list.Add(itemData);
                    }
                }
                catch (Exception)
                {
                }
                if (list.Count < 1)
                {
                    RadComboBoxItemData noData = new RadComboBoxItemData();
                    noData.Text = "[No tickets to display.]";
                    noData.Value = "-1";
                    list.Add(noData);
                }

                return list.ToArray();
            }
        }


        [WebMethod]
        public RadComboBoxItemData[] GetTicketByDescription(RadComboBoxContext context)
        {
            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;

            using (SearchJob job = new SearchJob())
            {
                string searchTerm = context["FilterString"].ToString().Trim();
                job.Request = searchTerm;
                job.FieldWeights = "TicketNumber: 5000, Name: 1000";
                job.BooleanConditions = "OrganizationID::" + TSAuthentication.OrganizationID.ToString();
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
                    job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy | SearchFlags.dtsSearchSelectMostRecent;
                }

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
                job.IndexesToSearch.Add(DataUtils.GetTicketIndexPath(TSAuthentication.GetLoginUser()));
                job.Execute();
                SearchResults results = job.Results;

                IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
                List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
                try
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        results.GetNthDoc(i);
                        RadComboBoxItemData itemData = new RadComboBoxItemData();
                        itemData.Text = results.CurrentItem.DisplayName;
                        itemData.Value = results.CurrentItem.Filename + "," + results.CurrentItem.UserFields["TicketNumber"].ToString();
                        list.Add(itemData);
                    }
                }
                catch (Exception)
                {
                }
                if (list.Count < 1)
                {
                    RadComboBoxItemData noData = new RadComboBoxItemData();
                    noData.Text = "[No tickets to display.]";
                    noData.Value = "-1";
                    list.Add(noData);
                }

                return list.ToArray();
            }
        }

        [WebMethod]
        public RadComboBoxItemData[] GetKBTicketByDescription(RadComboBoxContext context)
        {
            Options options = new Options();
            options.TextFlags = TextFlags.dtsoTfRecognizeDates;

            using (SearchJob job = new SearchJob())
            {
                string searchTerm = context["FilterString"].ToString().Trim();
                job.Request = searchTerm;
                job.FieldWeights = "TicketNumber: 5000, Name: 1000";
                job.BooleanConditions = "(OrganizationID::" + TSAuthentication.OrganizationID.ToString() + ") AND (IsKnowledgeBase::True)";
                job.MaxFilesToRetrieve = 25;
                job.AutoStopLimit = 100000;
                job.TimeoutSeconds = 10;
                job.SearchFlags =
                  SearchFlags.dtsSearchSelectMostRecent |
                  SearchFlags.dtsSearchStemming |
                  SearchFlags.dtsSearchDelayDocInfo;

                int num = 0;
                if (!int.TryParse(searchTerm, out num))
                {
                    job.Fuzziness = 1;
                    job.Request = job.Request + "*";
                    job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchFuzzy;
                }

                if (searchTerm.ToLower().IndexOf(" and ") < 0 && searchTerm.ToLower().IndexOf(" or ") < 0) job.SearchFlags = job.SearchFlags | SearchFlags.dtsSearchTypeAllWords;
                job.IndexesToSearch.Add(DataUtils.GetTicketIndexPath(TSAuthentication.GetLoginUser()));
                job.Execute();
                SearchResults results = job.Results;


                IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
                List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
                try
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        results.GetNthDoc(i);
                        RadComboBoxItemData itemData = new RadComboBoxItemData();
                        itemData.Text = results.CurrentItem.DisplayName;
                        itemData.Value = results.CurrentItem.Filename + "," + results.CurrentItem.UserFields["TicketNumber"].ToString();
                        list.Add(itemData);
                    }
                }
                catch (Exception)
                {
                }
                if (list.Count < 1)
                {
                    RadComboBoxItemData noData = new RadComboBoxItemData();
                    noData.Text = "[No tickets to display.]";
                    noData.Value = "-1";
                    list.Add(noData);
                }

                return list.ToArray();
            }
        }


        [WebMethod]
        public RadComboBoxItemData[] GetTicketTags(RadComboBoxContext context)
        {
            IDictionary<string, object> contextDictionary = (IDictionary<string, object>)context;
            Tags tags = new Tags(UserSession.LoginUser);
            string search = context["FilterString"].ToString();
            tags.LoadBySearchTerm(search);


            List<RadComboBoxItemData> list = new List<RadComboBoxItemData>();
            foreach (Tag tag in tags)
            {
                RadComboBoxItemData itemData = new RadComboBoxItemData();
                itemData.Text = tag.Value;
                itemData.Value = tag.TagID.ToString();
                list.Add(itemData);
            }

            return list.ToArray();
        }

        [WebMethod]
        public void UpdateUserPingTime()
        {
            Users.UpdateUserPingTime(UserSession.LoginUser, UserSession.LoginUser.UserID);
        }

        [WebMethod]
        public void UpdateUserActivityTime()
        {
            Users.UpdateUserActivityTime(UserSession.LoginUser, UserSession.LoginUser.UserID);
        }

        [WebMethod]
        public IEnumerable GetReportNodes(RadTreeNodeData node, IDictionary context)
        {
            Reports _reports = new Reports(UserSession.LoginUser);

            switch ((ReportTypeOld)Enum.Parse(typeof(ReportTypeOld), node.Value))
            {
                case ReportTypeOld.Standard:
                    _reports.LoadStandard();
                    break;
                case ReportTypeOld.Graphical:
                    _reports.LoadGraphical(UserSession.CurrentUser.OrganizationID);
                    break;
                case ReportTypeOld.Favorite:
                    _reports.LoadFavorites();
                    break;
                default:
                    _reports.LoadCustom(UserSession.CurrentUser.OrganizationID);
                    break;
            }

            List<RadTreeNodeData> list = new List<RadTreeNodeData>();
            foreach (Report rep in _reports)
            {
                RadTreeNodeData newNode = new RadTreeNodeData();
                newNode.Text = rep.Name;
                newNode.Value = rep.ReportID.ToString();
                newNode.Attributes.Add("ExternalURL", rep.ExternalURL);
                list.Add(newNode);
            }
            return list;
        }

        [WebMethod]
        public IEnumerable GetVersionNodes(RadTreeNodeData node, IDictionary context)
        {
            ProductVersions versions = new ProductVersions(UserSession.LoginUser);
            versions.LoadByProductID(int.Parse(node.Value));

            List<RadTreeNodeData> list = new List<RadTreeNodeData>();
            foreach (ProductVersion version in versions)
            {
                RadTreeNodeData newNode = new RadTreeNodeData();
                newNode.Text = version.VersionNumber;
                newNode.Value = version.ProductVersionID.ToString();
                list.Add(newNode);
            }
            return list;
        }

        [WebMethod]
        public void SubscribeToTicket(int ticketID)
        {
            Tickets tickets = new Tickets(UserSession.LoginUser);
            if (Tickets.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, ticketID))
                tickets.RemoveSubscription(UserSession.LoginUser.UserID, ticketID);
            else
                tickets.AddSubscription(UserSession.LoginUser.UserID, ticketID);
        }

        [WebMethod]
        public bool IsSubscribedToTicket(int ticketID)
        {
            return Tickets.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, ticketID);
        }

        [WebMethod]
        public void Subscribe(ReferenceType refType, int refID)
        {
            if (Subscriptions.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, refType, refID))
                Subscriptions.RemoveSubscription(UserSession.LoginUser, UserSession.LoginUser.UserID, refType, refID);
            else
                Subscriptions.AddSubscription(UserSession.LoginUser, UserSession.LoginUser.UserID, refType, refID);
        }

        [WebMethod]
        public bool IsSubscribed(ReferenceType refType, int refID)
        {
            return Subscriptions.IsUserSubscribed(UserSession.LoginUser, UserSession.LoginUser.UserID, refType, refID);
        }

        [WebMethod]
        public void RequestTicketUpdate(int ticketID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(UserSession.LoginUser, ticketID);
            if (ticket == null) return;
            EmailPosts.SendTicketUpdateRequest(UserSession.LoginUser, ticketID);

            string description = String.Format("{0} requested an update from {1} for {2}", UserSession.CurrentUser.FirstLastName, ticket.UserName, Tickets.GetTicketLink(UserSession.LoginUser, ticketID));
            ActionLogs.AddActionLog(UserSession.LoginUser, ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);
        }

        [WebMethod]
        public int GetTicketID(int ticketNumber)
        {
            Ticket ticket = Tickets.GetTicketByNumber(UserSession.LoginUser, ticketNumber);
            if (ticket != null && ticket.OrganizationID == UserSession.LoginUser.OrganizationID)
                return ticket.TicketID;
            else
                return -1;
        }

        [WebMethod]
        public void TakeTicketOwnership(int ticketID)
        {
            Tickets tickets = new Tickets(UserSession.LoginUser);
            tickets.LoadByTicketID(ticketID);

            if (!tickets.IsEmpty)
            {
                tickets[0].UserID = UserSession.LoginUser.UserID;
                tickets.Save();
            }
        }


        [WebMethod]
        public void SetUserSetting(string key, string value)
        {
            Settings.UserDB.WriteString(key, value);
            Users.UpdateUserActivityTime(UserSession.LoginUser, UserSession.LoginUser.UserID);
        }

        [WebMethod]
        public void SetSessionSetting(string key, string value)
        {
            Settings.Session.WriteString(key, value);
            Users.UpdateUserActivityTime(UserSession.LoginUser, UserSession.LoginUser.UserID);
        }

        [WebMethod]
        public string GetUserSetting(string key, string defaultValue)
        {
            return Settings.UserDB.ReadString(key, defaultValue);
        }

        [WebMethod]
        public string GetSessionSetting(string key, string defaultValue)
        {
            return Settings.Session.ReadString(key, defaultValue);
        }

        [WebMethod]
        public string GetUserStatusText()
        {
            User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
            return user.InOfficeComment;
        }

        [WebMethod]
        public bool GetUserAvailability()
        {
            User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
            return user.InOffice;
        }

        [WebMethod]
        public string SetUserStatusText(string text)
        {
            User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
            user.InOfficeComment = Server.HtmlEncode(text);
            user.Collection.Save();
            WaterCooler watercooler = new WaterCooler(UserSession.LoginUser);
            WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
            item.Message = string.Format("<strong>{0} - </strong>{1}", user.FirstLastName, user.InOfficeComment);
            item.OrganizationID = user.OrganizationID;
            item.TimeStamp = DateTime.UtcNow;
            item.UserID = user.UserID;
            watercooler.Save();
            return user.InOfficeComment;
        }

        [WebMethod]
        public bool ToggleUserAvailability()
        {
            User user = Users.GetUser(UserSession.LoginUser, UserSession.LoginUser.UserID);
            user.InOffice = !user.InOffice;
            user.Collection.Save();
            WaterCooler watercooler = new WaterCooler(UserSession.LoginUser);
            WaterCoolerItem item = watercooler.AddNewWaterCoolerItem();
            item.Message = string.Format("<strong>{0}</strong> {1}", user.FirstLastName, user.InOffice ? "is now in the office." : "has left the office.");
            item.OrganizationID = user.OrganizationID;
            item.TimeStamp = DateTime.UtcNow;
            item.UserID = user.UserID;
            watercooler.Save();

            return user.InOffice;
        }

        [WebMethod]
        public bool ToggleUserChat()
        {
            ChatUserSetting setting = ChatUserSettings.GetSetting(UserSession.LoginUser, UserSession.LoginUser.UserID);
            setting.IsAvailable = !setting.IsAvailable;
            setting.Collection.Save();
            return setting.IsAvailable;
        }

        [WebMethod]
        public bool CanEditReport(int reportID)
        {
            Report report = (Report)Reports.GetReport(UserSession.LoginUser, reportID);
            return (
                    (report.OrganizationID != null && report.OrganizationID == UserSession.LoginUser.OrganizationID) ||
                    (report.OrganizationID == null && UserSession.LoginUser.OrganizationID == 1078)
                    )
                    &&
                    (UserSession.CurrentUser.IsSystemAdmin || report.CreatorID == UserSession.LoginUser.UserID)
                    &&
                    string.IsNullOrEmpty(report.Query);
        }

        [WebMethod]
        public bool IsFavoriteReport(int reportID)
        {
            Report report = (Report)Reports.GetReport(UserSession.LoginUser, reportID);
            return report.IsFavorite;
        }

        [WebMethod]
        public void ToggleFavoriteReport(int reportID)
        {
            Report report = (Report)Reports.GetReport(UserSession.LoginUser, reportID);
            if (report.IsFavorite) { report.IsFavorite = false; }
            else { report.IsFavorite = true; }
        }


        #region Admin Methods

        [WebMethod]
        public void DeleteTicket(int ticketID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) {
                return;
            } else {
                Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
                if (ticket.OrganizationID != UserSession.LoginUser.OrganizationID) {
                    return;
                } else {
                    TSWebServices.DeflectorService deflectorService = new TSWebServices.DeflectorService();
                    deflectorService.DeleteTicket(ticket.TicketID);

                    ticket.Delete();
                    ticket.Collection.Save();
                }
            }
        }

        [WebMethod]
        public void DeleteAction(int actionID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, actionID);
            if (action == null) return;
            Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, action.TicketID);
            if (ticket == null) return;
            if (ticket.OrganizationID != UserSession.LoginUser.OrganizationID) return;

            action.Delete();
            action.Collection.Save();
        }

        [WebMethod]
        public void DeleteProduct(int productID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            try
            {
                Products.DeleteProduct(UserSession.LoginUser, productID);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteVersion(int versionID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            try
            {
                ProductVersions.DeleteProductVersion(UserSession.LoginUser, versionID);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteProductFamily(int productFamilyID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            try
            {
                ProductFamilies.DeleteProductFamily(UserSession.LoginUser, productFamilyID);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }


        [WebMethod]
        public void DeleteGroup(int groupID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            try
            {
                Groups groups = new Groups(UserSession.LoginUser);
                groups.DeleteFromDB(groupID);

            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteAttachment(int attachmentID)
        {
            //if (!UserSession.CurrentUser.IsSystemAdmin) return;
            try
            {
                string fileName = ModelAPI.AttachmentAPI.DeleteAttachment(attachmentID, AttachmentProxy.References.None);
                string description = String.Format("{0} deleted attachment {1}", UserSession.CurrentUser.FirstLastName, fileName);
                ActionLogs.AddActionLog(UserSession.LoginUser, ActionLogType.Delete, ReferenceType.Attachments, attachmentID, description);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteGroupUser(int groupID, int userID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            try
            {
                Groups groups = new Groups(UserSession.LoginUser);
                groups.DeleteGroupUser(groupID, userID);

            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteUser(int userID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            Users.MarkUserDeleted(UserSession.LoginUser, userID);
            User user = Users.GetUser(UserSession.LoginUser, userID);

            string description = String.Format("{0} deleted user {1} ", UserSession.CurrentUser.FirstLastName, user.FirstLastName);
            ActionLogs.AddActionLog(UserSession.LoginUser, ActionLogType.Delete, ReferenceType.Organizations, user.OrganizationID, description);

            Organization org = Organizations.GetOrganization(TSAuthentication.GetLoginUser(), user.OrganizationID);
            if (org.DefaultSupportUserID == user.UserID)
            {
                org.DefaultSupportUserID = null;
                org.Collection.Save();
            }


            if (user.IsActive && org.ParentID == 1) user.EmailCountToMuroc(false);

        }

        [WebMethod]
        public void DeleteTask(int taskID)
        {
            Task task = Tasks.GetTask(UserSession.LoginUser, taskID);
            if (task.CreatorID != UserSession.CurrentUser.UserID && !UserSession.CurrentUser.IsSystemAdmin) return;

            TaskAssociations associations = new TaskAssociations(UserSession.LoginUser);
            associations.DeleteByReminderIDOnly(taskID);

            Tasks subtasks = new Tasks(UserSession.LoginUser);
            subtasks.LoadIncompleteByParentID(taskID);
            foreach (Task subtask in subtasks)
            {
                DeleteTask(subtask.TaskID);
            }

            if (task.ReminderID != null)
            {
                Data.Reminder reminder = Reminders.GetReminder(UserSession.LoginUser, (int)task.ReminderID);
                reminder.Delete();
                reminder.Collection.Save();
            }

            string description = String.Format("{0} deleted task {1} ", UserSession.CurrentUser.FirstLastName, task.Description);
            ActionLogs.AddActionLog(UserSession.LoginUser, ActionLogType.Delete, ReferenceType.Tasks, taskID, description);
            task.Delete();
            task.Collection.Save();
        }

        [WebMethod]
        public void DeleteNote(int noteID)
        {
            Note note = Notes.GetNote(UserSession.LoginUser, noteID);
            if (note.CreatorID != UserSession.CurrentUser.UserID && !UserSession.CurrentUser.IsSystemAdmin) return;

            // delete attachments which point to this Note (Activity)

            string description = String.Format("{0} deleted note {1} ", UserSession.CurrentUser.FirstLastName, note.Title);
            ActionLogs.AddActionLog(UserSession.LoginUser, ActionLogType.Delete, ReferenceType.Notes, noteID, description);

            note.Delete();
            note.Collection.Save();
        }

        [WebMethod]
        public void DeleteOrganization(int organizationID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            try
            {
                int unknownID = Organizations.GetUnknownCompanyID(UserSession.LoginUser);
                Users u = new Users(UserSession.LoginUser);
                u.UpdateDeletedOrg(organizationID, unknownID);

                Organizations.DeleteOrganizationAndAllReleatedData(UserSession.LoginUser, organizationID);

            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteOrganizationProduct(int organizationProductID, bool bypass = true)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin && bypass) return;
            try
            {
                OrganizationProducts organizationProducts = new OrganizationProducts(UserSession.LoginUser);
                organizationProducts.LoadByOrganizationProductID(organizationProductID);
                UserProducts userProducts = new UserProducts(UserSession.LoginUser);
                //userProducts.LoadByOrganizationProductAndVersionID(organizationProducts[0].OrganizationID, "hola", "adios");
                userProducts.LoadByOrganizationProductAndVersionID(organizationProducts[0].OrganizationID, organizationProducts[0].ProductID, organizationProducts[0].ProductVersionID);
                userProducts.DeleteAll();
                userProducts.Save();
                organizationProducts.DeleteFromDB(organizationProductID);

                Product p = Products.GetProduct(TSAuthentication.GetLoginUser(), organizationProducts[0].ProductID);
                string description = String.Format("{0} deleted product association to {1} ", TSAuthentication.GetUser(TSAuthentication.GetLoginUser()).FirstLastName, p.Name);
                ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Delete, ReferenceType.Organizations, organizationProducts[0].OrganizationID, description);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteUserProduct(int userProductID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            try
            {
                UserProducts userProducts = new UserProducts(UserSession.LoginUser);
                userProducts.DeleteFromDB(userProductID);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteTicketOrganization(int organizationID, int ticketID)
        {
            try
            {
                Tickets tickets = new Tickets(UserSession.LoginUser);
                tickets.RemoveOrganization(organizationID, ticketID);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteTicketContact(int userID, int ticketID)
        {
            try
            {
                Tickets tickets = new Tickets(UserSession.LoginUser);
                tickets.RemoveContact(userID, ticketID);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void AddTicketOrganization(string id, int ticketID)
        {
            try
            {
                id = id.Trim();
                if (id.Length < 1) return;
                bool isUser = id.ToLower()[0] == 'u';
                int i = int.Parse(id.Remove(0, 1));

                Tickets tickets = new Tickets(UserSession.LoginUser);

                if (isUser) tickets.AddContact(i, ticketID); else tickets.AddOrganization(i, ticketID);
            }
            catch (Exception ex)
            {
                DataUtils.LogException(UserSession.LoginUser, ex);
            }
        }

        [WebMethod]
        public void DeleteReport(int reportID)
        {
            if (!UserSession.CurrentUser.IsSystemAdmin) return;
            Report report = (Report)Reports.GetReport(UserSession.LoginUser, reportID);
            if (report.OrganizationID != null && report.OrganizationID == UserSession.LoginUser.OrganizationID)
            {
                report.Delete();
                report.Collection.Save();
            }
        }

        #endregion

        [WebMethod]
        public void AddRelatedTicket(int ticketID1, int ticketID2)
        {
            Ticket ticket1 = Tickets.GetTicket(UserSession.LoginUser, ticketID1);
            Ticket ticket2 = Tickets.GetTicket(UserSession.LoginUser, ticketID2);

            if (ticket1.ParentID == ticketID2 || ticket2.ParentID == ticketID1) return;
            if (ticketID1 == ticketID2) return;

            TicketRelationship item = TicketRelationships.GetTicketRelationship(UserSession.LoginUser, ticketID1, ticketID2);
            if (item == null)
            {
                item = (new TicketRelationships(UserSession.LoginUser)).AddNewTicketRelationship();
                item.OrganizationID = UserSession.LoginUser.OrganizationID;
                item.Ticket1ID = ticketID1;
                item.Ticket2ID = ticketID2;
                item.Collection.Save();
            }
        }

        [WebMethod]
        public void AddParentTicket(int ticketID, int parentID)
        {
            if (ticketID == parentID) return;
            Ticket parent = Tickets.GetTicket(UserSession.LoginUser, parentID);
            if (parent.ParentID == ticketID) return;
            Ticket child = Tickets.GetTicket(UserSession.LoginUser, ticketID);
            child.ParentID = parentID;
            child.Collection.Save();
        }

        [WebMethod]
        public void AddChildTicket(int ticketID, int childID)
        {
            if (ticketID == childID) return;
            Ticket parent = Tickets.GetTicket(UserSession.LoginUser, ticketID);
            if (parent.ParentID == childID) return;
            Ticket child = Tickets.GetTicket(UserSession.LoginUser, childID);
            child.ParentID = ticketID;
            child.Collection.Save();
        }

        [WebMethod]
        public void RemoveRelatedTicket(int ticketID1, int ticketID2)
        {
            TicketRelationship item = TicketRelationships.GetTicketRelationship(UserSession.LoginUser, ticketID1, ticketID2);
            //if (item.CreatorID == UserSession.LoginUser.UserID || UserSession.CurrentUser.IsSystemAdmin)
            {
                item.Delete();
                item.Collection.Save();
            }
        }

        [WebMethod]
        public void RemoveParentTicket(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
            ticket.ParentID = null;
            ticket.Collection.Save();
        }

        [WebMethod]
        public void RemoveChildTicket(int childID)
        {
            Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, childID);
            ticket.ParentID = null;
            ticket.Collection.Save();
        }

        [WebMethod]
        public void AddTicketTag(int tagID, int ticketID)
        {
            Tag tag = Tags.GetTag(UserSession.LoginUser, tagID);
            Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
            if (tag.OrganizationID != UserSession.LoginUser.OrganizationID || ticket.OrganizationID != UserSession.LoginUser.OrganizationID) return;
            TagLink link = TagLinks.GetTagLink(UserSession.LoginUser, ReferenceType.Tickets, ticketID, tagID);
            if (link == null)
            {
                TagLinks links = new TagLinks(UserSession.LoginUser);
                link = links.AddNewTagLink();
                link.RefType = ReferenceType.Tickets;
                link.RefID = ticketID;
                link.TagID = tagID;
                links.Save();
            }
        }

        [WebMethod]
        public void AddTicketTagByValue(int ticketID, string value)
        {
            value = value.Trim();
            Tag tag = Tags.GetTag(UserSession.LoginUser, value);
            if (tag == null)
            {
                Tags tags = new Tags(UserSession.LoginUser);
                tag = tags.AddNewTag();
                tag.OrganizationID = UserSession.LoginUser.OrganizationID;
                tag.Value = value;
                tags.Save();
            }

            AddTicketTag(tag.TagID, ticketID);

        }


        [WebMethod]
        public void DeleteEmail(int emailID)
        {
            EmailAddresses emails = new EmailAddresses(UserSession.LoginUser);
            emails.DeleteFromDB(emailID);
        }

        [WebMethod]
        public void DeletePhone(int phoneID)
        {
            PhoneNumbers phoneNumbers = new PhoneNumbers(UserSession.LoginUser);
            phoneNumbers.DeleteFromDB(phoneID);
        }

        [WebMethod]
        public void DeleteAddress(int addressID)
        {
            Addresses addresses = new Addresses(UserSession.LoginUser);
            addresses.DeleteFromDB(addressID);
        }

        [WebMethod]
        public TicketProxy GetTicket(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(UserSession.LoginUser, ticketID);
            return ticket.GetProxy();
        }

        [WebMethod]
        public TicketProxy GetTicketByNumber(int ticketNumber)
        {
            Tickets tickets = new Tickets(UserSession.LoginUser);
            tickets.LoadByTicketNumber(UserSession.LoginUser.OrganizationID, ticketNumber);
            return tickets[0].GetProxy();
        }


        [WebMethod]
        public void SaveCustomFieldText(int refID, int fieldID, string value)
        {
            CustomValue customValue = CustomValues.GetValue(UserSession.LoginUser, fieldID, refID);
            customValue.Value = value;
            customValue.Collection.Save();
        }

        [WebMethod]
        public void SaveCustomFieldNumber(int refID, int fieldID, int value)
        {
            CustomValue customValue = CustomValues.GetValue(UserSession.LoginUser, fieldID, refID);
            customValue.Value = value.ToString();
            customValue.Collection.Save();
        }

        [WebMethod]
        public void SaveCustomFieldDate(int refID, int fieldID, DateTime? value)
        {
            DateTime? date;
            try
            {
                if (value == null) date = null; else date = Convert.ToDateTime(value);
            }
            catch (Exception)
            {
                date = null;

            }
            CustomValue customValue = CustomValues.GetValue(UserSession.LoginUser, fieldID, refID);
            if (date != null) customValue.Value = DataUtils.DateToUtc(UserSession.LoginUser, date).ToString();
            else customValue.Value = "";
            customValue.Collection.Save();
        }

        [WebMethod]
        public void SaveCustomFieldBool(int refID, int fieldID, bool value)
        {
            CustomValue customValue = CustomValues.GetValue(UserSession.LoginUser, fieldID, refID);
            customValue.Value = value.ToString();
            customValue.Collection.Save();
        }

    }

}
