using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Text;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Runtime.Serialization;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Diagnostics;
using ImageResizer;
using System.Net;
using System.IO;
using System.Dynamic;
using System.Text.RegularExpressions;
using TeamSupport.IDTree;
using TeamSupport.ModelAPI;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class TicketPageService : System.Web.Services.WebService
    {

        public TicketPageService() { }

        [WebMethod]
        public TicketPageInfo GetTicketInfo(int ticketNumber)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItemByNumber(TSAuthentication.GetLoginUser(), ticketNumber);
            if (ticket == null) return null;
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;

            User user = Users.GetUser(ticket.Collection.LoginUser, TSAuthentication.UserID);
            if (!ticket.UserHasRights(user)) return null;

            TicketService ticketService = new TicketService();
            ticketService.MarkTicketAsRead(ticket.TicketID);

            TicketPageInfo info = new TicketPageInfo();
            info.Ticket = ticket.GetProxy();

            info.faults = null;
            dynamic faults = new ExpandoObject();

            if (info.Ticket.Name.ToLower() == "<no subject>") {
                info.Ticket.Name = "";
            }

            TicketTypes types = new TicketTypes(ticket.Collection.LoginUser);
            types.LoadAllPositions(TSAuthentication.OrganizationID);

            if (!types.Any(a => a.TicketTypeID == info.Ticket.TicketTypeID)) {
                info.Ticket.TicketTypeID = types[0].TicketTypeID;
                ticket.TicketTypeID = info.Ticket.TicketTypeID;
                Ticket newticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticket.TicketID);
                newticket.TicketTypeID = ticket.TicketTypeID;
                newticket.Collection.Save();
            }

            // check if outside resource change ticket type and to modify the status
            TicketStatuses statuses = new TicketStatuses(ticket.Collection.LoginUser);
            statuses.LoadAvailableTicketStatuses(info.Ticket.TicketTypeID, null);

            if (!statuses.Any(a => a.TicketStatusID == info.Ticket.TicketStatusID)) {
                info.Ticket.TicketStatusID = statuses[0].TicketStatusID;
                ticket.TicketStatusID = info.Ticket.TicketStatusID;
                faults.status = "invalid";
            }

            if (info.Ticket.CategoryName != null && info.Ticket.ForumCategory != null)
                info.Ticket.CategoryDisplayString = ForumCategories.GetCategoryDisplayString(TSAuthentication.GetLoginUser(), (int)info.Ticket.ForumCategory);
            if (info.Ticket.KnowledgeBaseCategoryName != null)
                info.Ticket.KnowledgeBaseCategoryDisplayString = KnowledgeBaseCategories.GetKnowledgeBaseCategoryDisplayString(TSAuthentication.GetLoginUser(), (int)info.Ticket.KnowledgeBaseCategoryID);
            info.Customers = ticketService.GetTicketCustomers(ticket.TicketID);
            info.Related = ticketService.GetRelatedTickets(ticket.TicketID);
            info.Tags = ticketService.GetTicketTags(ticket.TicketID);
            info.CustomValues = ticketService.GetParentCustomValues(ticket.TicketID);
            info.Subscribers = GetSubscribers(ticket);
            info.Queuers = GetQueuers(ticket);
            {
                AttachmentProxy[] results;
                Model_API.ReadActionAttachmentsForTicket(ticket.TicketID, ActionAttachmentsByTicketID.ByFilename, out results);
                info.Attachments = results;
            }


            TaskService taskService = new TaskService();
            info.Tasks = taskService.GetTasksByTicketID(info.Ticket.TicketID);

            Reminders reminders = new Reminders(ticket.Collection.LoginUser);
            reminders.LoadByItemAll(ReferenceType.Tickets, ticket.TicketID, TSAuthentication.UserID);
            info.Reminders = reminders.GetReminderProxies();

            Assets assets = new Assets(ticket.Collection.LoginUser);
            assets.LoadByTicketID(ticket.TicketID);
            info.Assets = assets.GetAssetProxies();

            info.LinkToJira = GetLinkToJira(ticket.TicketID);
            info.LinkToTFS = GetLinkToTFS(ticket.TicketID);
			info.LinkToSnow = GetLinkToSnow(ticket.TicketID);

            TicketStatuses ticketStatus = new TicketStatuses(TSAuthentication.GetLoginUser());
            ticketStatus.LoadByStatusIDs(TSAuthentication.OrganizationID, new int[] { ticket.TicketStatusID });
            info.IsSlaPaused = ticketStatus != null && ticketStatus[0].PauseSLA;
            SlaTicket slaTicket = SlaTickets.GetSlaTicket(TSAuthentication.GetLoginUser(), ticket.TicketID);

            if (slaTicket != null) {
                info.SlaTriggerId = slaTicket.SlaTriggerId;
                info.IsSlaPending = slaTicket.IsPending;
            }

            try {
                Plugins plugins = new Plugins(TSAuthentication.GetLoginUser());
                plugins.LoadByOrganizationID(TSAuthentication.OrganizationID);
                PluginProxy[] pluginProxies = plugins.GetPluginProxies();
                ReplacePluginData(TSAuthentication.GetLoginUser(), ticket, pluginProxies);
                info.Plugins = pluginProxies;
            }
            catch (Exception) { }

            if (faults != null) {
                info.faults = faults;
            }

            return info;
        }

        [WebMethod]
        public TicketPageUser[] GetTicketUsers(int ticketID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organization organization = TSAuthentication.GetOrganization(loginUser);

            List<TicketPageUser> result = new List<TicketPageUser>();
            Ticket ticket = Tickets.GetTicket(loginUser, ticketID);

            UsersView sender = new UsersView(TSAuthentication.GetLoginUser());
            sender.LoadLastSenderByTicketNumber(TSAuthentication.OrganizationID, ticketID);

            List<User> ticketUsers = new List<TeamSupport.Data.User>();
            bool hasSenderBeenAdded = false;

            if (ticket.GroupID != null && organization.ShowGroupMembersFirstInTicketAssignmentList)
            {
                Users groupUsers = new Users(loginUser);
                groupUsers.LoadByGroupID((int)ticket.GroupID);
                ticketUsers.AddRange(groupUsers);
            }

            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByOrganizationID(TSAuthentication.OrganizationID, true);

            ticketUsers.AddRange(users.Where(p => !ticketUsers.Any(p2 => p2.UserID == p.UserID)).OrderBy(tu => tu.DisplayName));

            foreach (User user in ticketUsers)
            {
                TicketPageUser selectUser = new TicketPageUser();
                selectUser.ID = user.UserID;
                selectUser.Name = user.DisplayName;

                if (!string.IsNullOrEmpty(user.InOfficeComment)) selectUser.InOfficeMessage = user.InOfficeComment;
                if (sender.Count > 0 && sender[0].UserID == user.UserID) selectUser.IsSender = true;
                if (ticket.CreatorID == user.UserID) selectUser.IsCreator = true;
                if (ticket.UserID == user.UserID) selectUser.IsSelected = true;

                if (selectUser.IsSender)
                {
                    result.Insert(0, selectUser);
                    hasSenderBeenAdded = true;
                }
                else if (selectUser.IsCreator)
                {
                    if (hasSenderBeenAdded)
                    {
                        result.Insert(1, selectUser);
                    }
                    else
                        result.Insert(0, selectUser);
                }
                else
                    result.Add(selectUser);
            }

            return result.ToArray();
        }

        [WebMethod]
        public TicketPropertySelectField[] GetTicketGroups(int ticketID)
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

                return groupProxies.Select(g => new TicketPropertySelectField
                {
                    ID = g.GroupID,
                    Name = g.Name,
                    IsSelected = (g.GroupID == ticket[0].GroupID),
                    ProductFamilyID = g.ProductFamilyID
                }).ToArray();
            }
            else
            {
                Groups groups = new Groups(TSAuthentication.GetLoginUser());
                groups.LoadByOrganizationID(TSAuthentication.OrganizationID);
                return groups.Select(g => new TicketPropertySelectField
                {
                    ID = g.GroupID,
                    Name = g.Name,
                    IsSelected = (ticket[0].GroupID == g.GroupID),
                    ProductFamilyID = g.ProductFamilyID
                }).ToArray();
            }
        }

        [WebMethod]
        public TimeLineItem[] GetTimeLineItems(int ticketID, int from)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            TicketTimeLineView TimeLineView = new TicketTimeLineView(loginUser);

            try {
                TimeLineView.LoadByRange(ticketID, from, from + 9);
            } catch (Exception ex) {
                ExceptionLogs.LogException(loginUser, ex, "GetTimeLineItems", "TicketPageService.GetTimeLineItems");
            }

            return processActions(TimeLineView);
        }

        [WebMethod]
        public TimeLineItem[] getPinned (int ticketID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            TicketTimeLineView TimeLineView = new TicketTimeLineView(loginUser);
            try {
                TimeLineView.Pinned(ticketID);
            } catch (Exception ex) {
                ExceptionLogs.LogException(loginUser, ex, "pullPinned", "TicketPageService.pullPinned");
            }
            return processActions(TimeLineView);
        }

        private TimeLineItem[] processActions (TicketTimeLineView TimeLineView) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            List<TimeLineItem> timeLineItems = new List<TimeLineItem>();

            foreach (TicketTimeLineViewItem viewItem in TimeLineView) {
                if (!viewItem.IsWC) {
                    TimeLineItem timeLineItem  = new TimeLineItem();
                    timeLineItem.item          = viewItem.GetProxy();
                    timeLineItem.item.Message  = SanitizeMessage(timeLineItem.item.Message, loginUser);

                    {
                        timeLineItem.Attachments = Model_API.Read<AttachmentProxy[]>(viewItem.RefID);
                    }

                    timeLineItems.Add(timeLineItem);
                } else {
                    TimeLineItem wcItem = new TimeLineItem();
                    WaterCoolerThread thread = new WaterCoolerThread();
                    wcItem.item = viewItem.GetProxy();

                    WaterCoolerView wc = new WaterCoolerView(TSAuthentication.GetLoginUser());
                    wc.LoadMoreThreadsNoCountFilter(0, (int)viewItem.TicketNumber);

                    if (wc.Any(d => d.MessageID == viewItem.RefID)) {
                        WatercoolerMsg replies = new WatercoolerMsg(loginUser);
                        replies.LoadReplies(viewItem.RefID);
                        WatercoolerLikes likes = new WatercoolerLikes(loginUser);
                        likes.LoadByMessageID(viewItem.RefID);
                        wcItem.Likes = likes.Count();
                        wcItem.DidLike = (likes.Where(l => l.UserID == loginUser.UserID).Count() > 0);
                        List<WaterCoolerReply> wcReplies = new List<WaterCoolerReply>();

                        foreach (WatercoolerMsgItem reply in replies) {
                            WaterCoolerReply replyItem = new WaterCoolerReply();
                            replyItem.WaterCoolerReplyProxy = reply.GetProxy();
                            WatercoolerLikes replyLikes = new WatercoolerLikes(loginUser);
                            replyLikes.LoadByMessageID(reply.MessageID);
                            replyItem.Likes = replyLikes.Count();
                            replyItem.DidLike = (replyLikes.Where(rl => rl.UserID == loginUser.UserID).Count() > 0);
                            wcReplies.Add(replyItem);
                        }

                      wcItem.WaterCoolerReplies = wcReplies.ToArray();

                      WatercoolerAttachments threadAttachments = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
                      threadAttachments.LoadByMessageID(viewItem.RefID);
                      thread.Groups = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Group);
                      thread.Tickets = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Ticket);
                      thread.Products = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Product);
                      thread.Company = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Company);
                      thread.User = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.User);
                      wcItem.WatercoolerReferences = thread;
                      timeLineItems.Add(wcItem);
                    }
                }
            }
            return timeLineItems.ToArray();
        }


        [WebMethod]
        public TimeLineItem RequestUpdate(int ticketID)
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
            action.CreatorID = TSAuthentication.UserID;
            action.Collection.Save();


            string description = String.Format("{0} requested an update from {1} for {2}", user.FirstLastName, ticket.UserName, Tickets.GetTicketLink(TSAuthentication.GetLoginUser(), ticketID));
            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, description);

            return GetActionTimelineItem(action);
        }

        [WebMethod]
        public TimeLineItem CreateEmailTicketAction(int ticketID, string addresses, string introduction) {
            string actionText = string.Format("<i>Action added via the Email Ticket button to: {0} </i> <br><br>{1}", addresses, introduction);
            string logPost = string.Format("{0} sent a email introduction to {1}", TSAuthentication.GetLoginUser().GetUserFullName(), addresses);
            return LogAction(ticketID, SystemActionType.Email, "Email Introduction", actionText, logPost);
        }

        [WebMethod]
        public void EmailTicket(int ticketID, string addresses, string introduction, int actionID)
        {
            addresses = addresses.Length > 200 ? addresses.Substring(0, 200) : addresses;
            EmailPosts posts = new EmailPosts(TSAuthentication.GetLoginUser());
            EmailPost post = posts.AddNewEmailPost();
            post.EmailPostType = EmailPostType.TicketSendEmail;
            post.HoldTime = 0;

            post.Param1 = TSAuthentication.UserID.ToString();
            post.Param2 = ticketID.ToString();
            post.Param3 = addresses;
            post.Param4 = actionID.ToString();
            post.Text1 = introduction;
            posts.Save();
        }

        [WebMethod]
        public int CloneTicket(int ticketID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            int cloneTicketId = 0;

            try
            {
                Ticket originalTicket = Tickets.GetTicket(loginUser, ticketID);
                Ticket clonedTicket = originalTicket.Clone();
                cloneTicketId = clonedTicket.TicketID;
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(loginUser, ex, "Cloning Ticket", "TicketPageService.CloneTicket");
            }

            return cloneTicketId;
        }

        [WebMethod]
        public int GetActionCount(int ticketID)
        {
            return Actions.GetTicketActionCount(TSAuthentication.GetLoginUser(), ticketID);
        }

        [WebMethod]
        public void SaveTicketPageOrder(string keyname, string data)
        {
            Settings.OrganizationDB.WriteString(keyname, data);
        }

        [WebMethod]
        public TicketCategoryOrder[] GetTicketPageOrder(string KeyName)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            Organization organization = Organizations.GetOrganization(loginUser, loginUser.OrganizationID);
            string reminderCatName = @",{'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                    {'CatID':'Reminders','CatName':'Reminders','Disabled':'false'}";

            if (organization.ProductType == ProductType.Enterprise)
            {
                reminderCatName = ",{'CatID':'Sentiment','CatName':'Sentiment','Disabled':'false'},{'CatID':'hr','CatName':'Line Break','Disabled':'false'},{'CatID':'Tasks','CatName':'Tasks','Disabled':'false'}";
                if (KeyName == "NewTicketFieldsOrder")
                {
                    reminderCatName = string.Empty;
                }
            }

            string defaultOrder = @"[{'CatID':'AssignedTo','CatName':'Assigned To','Disabled':'false'},
                {'CatID':'Group','CatName':'Group','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'Type','CatName':'Type','Disabled':'false'},
                {'CatID':'Status','CatName':'Status','Disabled':'false'},
                {'CatID':'Severity','CatName':'Severity','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'SLAStatus','CatName':'SLA Status','Disabled':'false'},
                {'CatID':'VisibleToCustomers','CatName':'Visible To Customers','Disabled':'false'},
                {'CatID':'KnowledgeBase','CatName':'Knowledge Base','Disabled':'false'},
                {'CatID':'Community','CatName':'Community','Disabled':'false'},
                {'CatID':'DaysOpened','CatName':'Days Opened','Disabled':'false'},
                {'CatID':'TotalTimeSpent','CatName':'Total Time Spent','Disabled':'false'},
                {'CatID':'DueDate','CatName':'Due Date','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'Customers','CatName':'Customers','Disabled':'false'},
                {'CatID':'CustomFields','CatName':'Custom Fields','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'Product','CatName':'Product','Disabled':'false'},
                {'CatID':'Reported','CatName':'Reported','Disabled':'false'},
                {'CatID':'Resolved','CatName':'Resolved','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'Inventory','CatName':'Inventory','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'Tags','CatName':'Tags','Disabled':'false'}"
                + reminderCatName
                + @",{'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'AssociatedTickets','CatName':'Associated Tickets','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'UserQueue','CatName':'User Queue','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'false'},
                {'CatID':'SubscribedUsers','CatName':'Subscribed Users','Disabled':'false'},
                {'CatID':'hr','CatName':'Line Break','Disabled':'true'},
                {'CatID':'Jira','CatName':'Jira','Disabled':'false'},
                {'CatID':'TFS','CatName':'TFS','Disabled':'false'},
                {'CatID':'Attachments','CatName':'Attachments','Disabled':'false'}]";

            string fieldOrder = Settings.OrganizationDB.ReadString(KeyName, string.Empty);
            if (string.IsNullOrEmpty(fieldOrder))
                return JsonConvert.DeserializeObject<List<TicketCategoryOrder>>(defaultOrder).ToArray();

            List<TicketCategoryOrder> items = JsonConvert.DeserializeObject<List<TicketCategoryOrder>>(Settings.OrganizationDB.ReadString(KeyName, defaultOrder));

            // custom field order configurations are a snapshot in time and might not contain sentiment
            TicketCategoryOrder[] sentimentField = items.Where(t => t.CatName == "Sentiment").ToArray();
            if (sentimentField.Length == 0)
            {
                // insert after severity
                int i = 0;
                for (; i < items.Count; ++i)
                {
                    if (items[i].CatID == "Severity")
                    {
                        ++i;
                        break;
                    }
                }
                items.Insert(i, new TicketCategoryOrder() { CatID = "Sentiment", CatName = "Sentiment", Disabled = "false" });
            }

            return items.ToArray();
        }

        [WebMethod]
        public PluginProxy GetTicketPagePlugin(int pluginID)
        {
            Plugin plugin = Plugins.GetPlugin(TSAuthentication.GetLoginUser(), pluginID);
            if (plugin.OrganizationID == TSAuthentication.OrganizationID)
            {
                return plugin.GetProxy();
            }
            return null;
        }

        [WebMethod]
        public string GetTicketPagePluginSample(int ticketNumber, string code)
        {
            dynamic result = new ExpandoObject();
            TicketsViewItem ticket = TicketsView.GetTicketsViewItemByNumber(TSAuthentication.GetLoginUser(), ticketNumber);
            result.ticket = ticket.GetProxy();

            PluginProxy plugin = new PluginProxy();
            plugin.Code = code;
            List<PluginProxy> plugins = new List<PluginProxy>();
            plugins.Add(plugin);

            ReplacePluginData(TSAuthentication.GetLoginUser(), ticket, plugins.ToArray());
            result.code = plugins[0].Code;
            return JsonConvert.SerializeObject(result);
        }

        private void ReplacePluginData(LoginUser loginUser, TicketsViewItem ticketView, PluginProxy[] plugins)
        {
            //Ticket
            ReplaceTablePluginData(loginUser, "Ticket", plugins, ticketView.Row);
            ReplaceCustomFieldPluginData(loginUser, "Ticket Custom Fields", ReferenceType.Tickets, ticketView.TicketID, plugins, ticketView.TicketTypeID);
            EraseTablePluginData("Ticket Custom Fields", plugins);

            //User
            if (ticketView.UserID != null)
            {
                ReplaceTablePluginData(loginUser, "User", plugins, UsersView.GetUsersViewItem(loginUser, (int)ticketView.UserID).Row);
                ReplaceCustomFieldPluginData(loginUser, "User Custom Fields", ReferenceType.Users, (int)ticketView.UserID, plugins);
            }
            else
            {
                EraseTablePluginData("User", plugins);
                EraseTablePluginData("User Custom Fields", plugins);
            }

            //Customer
            OrganizationsView organizations = new OrganizationsView(loginUser);
            organizations.LoadByTicketID(ticketView.TicketID);
            if (organizations.IsEmpty)
            {
                EraseTablePluginData("Customer", plugins);
                EraseTablePluginData("Customer Address", plugins);
                EraseTablePluginData("Customer PhoneNumber", plugins);
                EraseTablePluginData("Customer Custom Fields", plugins);
            }
            else
            {
                int orgID = organizations[0].OrganizationID;
                ReplaceTablePluginData(loginUser, "Customer", plugins, organizations[0].Row);

                Addresses addresses = new Addresses(loginUser);
                addresses.LoadByID(orgID, ReferenceType.Organizations);
                if (addresses.IsEmpty) EraseTablePluginData("Customer Address", plugins);
                else ReplaceTablePluginData(loginUser, "Customer Address", plugins, addresses[0].Row);

                PhoneNumbers numbers = new PhoneNumbers(loginUser);
                numbers.LoadByID(orgID, ReferenceType.Organizations);
                if (numbers.IsEmpty) EraseTablePluginData("Customer PhoneNumber", plugins);
                else ReplaceTablePluginData(loginUser, "Customer PhoneNumber", plugins, numbers[0].Row);

                ReplaceCustomFieldPluginData(loginUser, "Customer Custom Fields", ReferenceType.Organizations, orgID, plugins);
            }

            ContactsView contacts = new ContactsView(loginUser);
            contacts.LoadByTicketID((int)ticketView.TicketID);
            if (contacts.IsEmpty)
            {
                EraseTablePluginData("Contact", plugins);
                EraseTablePluginData("Contact Address", plugins);
                EraseTablePluginData("Contact PhoneNumber", plugins);
                EraseTablePluginData("Contact Custom Fields", plugins);
            }
            else
            {
                int contactID = contacts[0].UserID;
                ReplaceTablePluginData(loginUser, "Contact", plugins, contacts[0].Row);

                Addresses addresses = new Addresses(loginUser);
                addresses.LoadByID(contactID, ReferenceType.Users);
                if (addresses.IsEmpty) EraseTablePluginData("Contact Address", plugins);
                else ReplaceTablePluginData(loginUser, "Contact Address", plugins, addresses[0].Row);

                PhoneNumbers numbers = new PhoneNumbers(loginUser);
                numbers.LoadByID(contactID, ReferenceType.Users);
                if (numbers.IsEmpty) EraseTablePluginData("Contact PhoneNumber", plugins);
                else ReplaceTablePluginData(loginUser, "Contact PhoneNumber", plugins, numbers[0].Row);

                ReplaceCustomFieldPluginData(loginUser, "Contact Custom Fields", ReferenceType.Contacts, contactID, plugins);
            }
        }

        private void EraseTablePluginData(string templateName, PluginProxy[] plugins)
        {
            foreach (PluginProxy plugin in plugins)
            {
                Regex regex = new Regex("{{" + templateName + ".(.*?)(?:}})");
                plugin.Code = regex.Replace(plugin.Code, "");
            }
        }

        private void ReplaceTablePluginData(LoginUser loginUser, string templateName, PluginProxy[] plugins, DataRow row)
        {
            foreach (PluginProxy plugin in plugins)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    plugin.Code = plugin.Code.Replace("{{" + templateName + "." + column.ColumnName + "}}", row[column].ToString());
                }
            }
        }

        private void ReplaceCustomFieldPluginData(LoginUser loginUser, string templateName, ReferenceType refType, int refID, PluginProxy[] plugins, int? auxID = null)
        {
            CustomFields fields = new CustomFields(loginUser);
            fields.LoadByReferenceType(TSAuthentication.OrganizationID, refType, auxID);

            foreach (CustomField field in fields)
            {
                string value = field.GetValue(refID) as string;

                if (string.IsNullOrWhiteSpace(value)) value = "";

                foreach (PluginProxy plugin in plugins)
                {
                    plugin.Code = plugin.Code.Replace("{{" + templateName + "." + field.Name + "}}", value);
                }
            }
        }

        [WebMethod]
        public string GetPluginTicketCustomFields(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            CustomValues values = new CustomValues(TSAuthentication.GetLoginUser());
            values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Organizations, ticketID);
            return values.GetJson();
        }

        public string GetPluginTicketActions(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            Actions actions = new Actions(TSAuthentication.GetLoginUser());
            actions.LoadByTicketID(ticketID);
            return actions.GetJson();
        }

        [WebMethod]
        public string GetPluginTicketUser(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            if (ticket.UserID == null) return "{}";
            UsersViewItem user = UsersView.GetUsersViewItem(TSAuthentication.GetLoginUser(), (int)ticket.UserID);

            dynamic result = new ExpandoObject();
            result = user.GetExpandoObject();

            Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());
            addresses.LoadByID(user.UserID, ReferenceType.Users);
            result.addresses = addresses.GetExpandoObject();

            PhoneNumbers numbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
            numbers.LoadByID(user.UserID, ReferenceType.Users);
            result.phoneNumbers = numbers.GetExpandoObject();

            CustomValues values = new CustomValues(TSAuthentication.GetLoginUser());
            values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Users, user.UserID);
            result.customValues = values.GetExpandoObject();

            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetPluginTicketContacts(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            ContactsView contacts = new ContactsView(TSAuthentication.GetLoginUser());
            contacts.LoadByTicketID(ticketID);
            dynamic result = new ExpandoObject();
            result = contacts.GetExpandoObject();

            for (int i = 0; i < contacts.Count; i++)
            {
                Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());
                addresses.LoadByID(contacts[i].UserID, ReferenceType.Users);
                result[i].addresses = addresses.GetExpandoObject();

                PhoneNumbers numbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                numbers.LoadByID(contacts[i].UserID, ReferenceType.Users);
                result[i].phoneNumbers = numbers.GetExpandoObject();

                CustomValues values = new CustomValues(TSAuthentication.GetLoginUser());
                values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Contacts, contacts[i].UserID);
                result[i].customValues = values.GetExpandoObject();
            }
            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetPluginTicketCustomers(int ticketID)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            if (ticket.OrganizationID != TSAuthentication.OrganizationID) return null;
            OrganizationsView organizations = new OrganizationsView(TSAuthentication.GetLoginUser());
            organizations.LoadByTicketID(ticketID);
            dynamic result = new ExpandoObject();
            result = organizations.GetExpandoObject();

            for (int i = 0; i < organizations.Count; i++)
            {
                Addresses addresses = new Addresses(TSAuthentication.GetLoginUser());
                addresses.LoadByID(organizations[i].OrganizationID, ReferenceType.Organizations);
                result[i].addresses = addresses.GetExpandoObject();

                PhoneNumbers numbers = new PhoneNumbers(TSAuthentication.GetLoginUser());
                numbers.LoadByID(organizations[i].OrganizationID, ReferenceType.Organizations);
                result[i].phoneNumbers = numbers.GetExpandoObject();

                CustomValues values = new CustomValues(TSAuthentication.GetLoginUser());
                values.LoadByReferenceType(TSAuthentication.OrganizationID, ReferenceType.Organizations, organizations[i].OrganizationID);
                result[i].customValues = values.GetExpandoObject();
            }
            return JsonConvert.SerializeObject(result);
        }

        [WebMethod]
        public string GetTicketPagePluginTemplates(string templateType)
        {
            List<ExpandoObject> result = new List<ExpandoObject>();
            if (templateType.ToLower() == "ticket")
            {
                result.Add(GetTableTemplate(TSAuthentication.GetLoginUser(), "Ticket", "TicketsView"));
                result.Add(GetCustomFieldNames(TSAuthentication.GetLoginUser(), "Ticket Custom Fields", TSAuthentication.OrganizationID, ReferenceType.Tickets));
                result.Add(GetTableTemplate(TSAuthentication.GetLoginUser(), "User", "UsersView"));
                result.Add(GetCustomFieldNames(TSAuthentication.GetLoginUser(), "User Custom Fields", TSAuthentication.OrganizationID, ReferenceType.Users));
                result.Add(GetTableTemplate(TSAuthentication.GetLoginUser(), "Customer", "OrganizationsView"));
                result.Add(GetTableTemplate(TSAuthentication.GetLoginUser(), "Customer Address", "Addresses"));
                result.Add(GetTableTemplate(TSAuthentication.GetLoginUser(), "Customer PhoneNumber", "PhoneNumbers"));
                result.Add(GetCustomFieldNames(TSAuthentication.GetLoginUser(), "Customer Custom Fields", TSAuthentication.OrganizationID, ReferenceType.Organizations));
                result.Add(GetTableTemplate(TSAuthentication.GetLoginUser(), "Contact", "ContactsView"));
                result.Add(GetTableTemplate(TSAuthentication.GetLoginUser(), "Contact Address", "Addresses"));
                result.Add(GetTableTemplate(TSAuthentication.GetLoginUser(), "Contact PhoneNumber", "PhoneNumbers"));
                result.Add(GetCustomFieldNames(TSAuthentication.GetLoginUser(), "Contact Custom Fields", TSAuthentication.OrganizationID, ReferenceType.Contacts));
            }
            return JsonConvert.SerializeObject(result.ToArray());
        }

        private ExpandoObject GetTableTemplate(LoginUser loginUser, string templateName, string tableName)
        {
            dynamic cat = new ExpandoObject();
            cat.name = templateName;
            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM " + tableName;
            DataTable table = SqlExecutor.ExecuteSchema(loginUser, command);
            cat.items = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
            Array.Sort(cat.items);
            return cat;
        }

        private ExpandoObject GetCustomFieldNames(LoginUser loginUser, string templateName, int organizationID, ReferenceType refType)
        {
            dynamic cat = new ExpandoObject();
            cat.name = templateName;

            CustomFields fields = new CustomFields(loginUser);
            fields.LoadByReferenceType(organizationID, refType);
            cat.items = fields.Cast<CustomField>().Select(x => x.Name).ToArray();
            Array.Sort(cat.items);
            return cat;
        }

        public string GetTicketPagePluginCode(int pluginID, int ticketID)
        {
            Plugin plugin = Plugins.GetPlugin(TSAuthentication.GetLoginUser(), pluginID);
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);

            if (plugin.OrganizationID == TSAuthentication.OrganizationID && ticket.OrganizationID == plugin.OrganizationID)
            {
                // replace fields
                return plugin.Code;
            }
            return null;
        }

        [WebMethod]
        public PluginProxy SaveTicketPagePlugin(int pluginID, string name, string code)
        {
            Plugin plugin;

            if (pluginID < 0)
            {
                Plugins plugins = new Plugins(TSAuthentication.GetLoginUser());
                plugin = plugins.AddNewPlugin();
                plugin.Code = code;
                plugin.CreatorID = TSAuthentication.UserID;
                plugin.DateCreated = DateTime.UtcNow;
                plugin.Name = name;
                plugin.OrganizationID = TSAuthentication.OrganizationID;
            }
            else
            {
                plugin = Plugins.GetPlugin(TSAuthentication.GetLoginUser(), pluginID);
                if (plugin.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
                {
                    plugin.Name = name;
                    plugin.Code = code;
                }
            }

            plugin.BaseCollection.Save();
            return plugin.GetProxy();
        }

        [WebMethod]
        public void DeleteTicketPagePlugin(int pluginID)
        {
            Plugin plugin = Plugins.GetPlugin(TSAuthentication.GetLoginUser(), pluginID);
            if (plugin.OrganizationID == TSAuthentication.OrganizationID && TSAuthentication.IsSystemAdmin)
            {
                plugin.Delete();
                plugin.BaseCollection.Save();
            }
        }

        [WebMethod]
        public AutocompleteItem[] GetUserOrOrganizationForTicket(string searchTerm)
        {
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            return GetUserOrOrganizationFiltered(searchTerm, !user.AllowAnyTicketCustomer);
        }

        [WebMethod]
        public bool CheckContactEmails(int ticketid)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            SqlCommand command = new SqlCommand();
            command.CommandText = "select u.FirstName from UserTickets as ut, Users as u where ut.TicketID=@TicketID and u.UserID = ut.UserID and (u.Email = '' or u.IsActive = 0)";
            command.Parameters.AddWithValue("@TicketID", ticketid.ToString());

            DataTable table = SqlExecutor.ExecuteQuery(loginUser, command);
            if (table.Rows.Count > 0)
                return false;
            else
                return true;

        }

        [WebMethod]
        public TimeLineItem UpdateAction(ActionProxy proxy)
        {
            // new action - future
            //if (ConnectionContext.IsEnabled && (proxy.ActionID == -1))  // new action - existing ticket
            //{
            //    proxy.CreatorID = TSAuthentication.UserID;
            //    ModelAPI.Create(proxy);
            //}

            TeamSupport.Data.Action action = Actions.GetActionByID(TSAuthentication.GetLoginUser(), proxy.ActionID);
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);

            if (action == null)
            {
                action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
                action.TicketID = proxy.TicketID;
                action.CreatorID = TSAuthentication.UserID;
                if (!string.IsNullOrWhiteSpace(user.Signature) && proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase && proxy.ActionID == -1)
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
                if (proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase && proxy.ActionID == -1)
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

            return GetActionTimelineItem(action);
        }

        [WebMethod]
        public TimeLineItem UpdateActionCopyingAttachment(ActionProxy proxy, int insertedKBTicketID)
        {
            TeamSupport.Data.Action action = Actions.GetActionByID(TSAuthentication.GetLoginUser(), proxy.ActionID);
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), TSAuthentication.UserID);

            if (action == null)
            {
                action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
                action.TicketID = proxy.TicketID;
                action.CreatorID = TSAuthentication.UserID;
                if (!string.IsNullOrWhiteSpace(user.Signature) && proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase && proxy.ActionID == -1)
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
                if (proxy.IsVisibleOnPortal && !proxy.IsKnowledgeBase && proxy.ActionID == -1)
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

            CopyInsertedKBAttachments(action.ActionID, insertedKBTicketID);

            return GetActionTimelineItem(action);
        }

        [WebMethod]
        public bool SetActionPortal(int actionID, bool isVisibleOnPortal)
        {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            User author = Users.GetUser(TSAuthentication.GetLoginUser(), action.CreatorID);
            bool isKB = action.IsKnowledgeBase;
            if (CanEditAction(action) || user.ChangeTicketVisibility)
            {
                if (author != null)
                {
                    if (isVisibleOnPortal && !isKB)
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
        public string getPosition (int ticketID, int actionID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            string json = Actions.getPosition(loginUser, ticketID, actionID);
            if (json != "nothing" && json != "negative") {
                return json;
            } else {
                return "negative";
            }
        }

        [WebMethod]
        public string PullReactions(int ticketID, int actionID) {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User author = Users.GetUser(loginUser, action.CreatorID);
            if (author != null) {
                if (loginUser.OrganizationID == author.OrganizationID) {
                    string json1 = Actions.CountReactions(loginUser, ticketID, actionID);
                    string json2 = Actions.CheckReaction(loginUser, ticketID, actionID);
                    if (json1 == "negative" || json2 == "negative") {
                        return "negative";
                    } else if (json1 == "nothing" && json2 == "nothing") {
                        return "nothing";
                    } else if (json1 != "nothing" && json2 != "nothing") {
                        return string.Format("[{0},{1}]", json1, json2);
                    } else if (json1 != "nothing") {
                        return json1;
                    } else if (json2 != "nothing") {
                        return json2;
                    } else {
                        return "negative";
                    }
                } else {
                    return "hidden";
                }
            } else {
                return "hidden";
            }
        }

        [WebMethod]
        public string PullUserList(int ticketID) {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            return Actions.PullUserList(loginUser, ticketID);
        }

        [WebMethod]
        public string ListReactions(int ticketID, int actionID) {
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = TSAuthentication.GetUser(loginUser);
            User author = Users.GetUser(loginUser, action.CreatorID);
            return Actions.ListReactions(loginUser, ticketID, actionID);
        }

        [WebMethod]
        public string UpdateReaction(int ticketID, int actionID, int value) {
            string updateReaction = string.Empty;
            TeamSupport.Data.Action action = Actions.GetAction(TSAuthentication.GetLoginUser(), actionID);
            LoginUser loginUser = TSAuthentication.GetLoginUser();

            int receiverID = Convert.ToInt32(action.CreatorID);

            updateReaction = Actions.UpdateReaction(loginUser, receiverID, ticketID, actionID, value);

            if (updateReaction == "positive" && value > 0 && action.CreatorID != loginUser.UserID) {
                EmailReaction(loginUser, receiverID, ticketID);
            }
            return updateReaction;
        }

        private void EmailReaction(LoginUser loginUser, int receiverID, int ticketID) {
            try {
                EmailPosts posts   = new EmailPosts(TSAuthentication.GetLoginUser());
                EmailPost post     = posts.AddNewEmailPost();
                post.EmailPostType = EmailPostType.Reaction;
                post.HoldTime = 5;
                post.Param1 = receiverID.ToString();
                post.Param2 = ticketID.ToString();
                post.Param3 = SystemSettings.GetAppUrl();
                posts.Save();
            }
            catch { }
        }



        public AutocompleteItem[] GetUserOrOrganizationFiltered(string searchTerm, bool filterByUserRights)
        {
            Organizations organizations = new Organizations(TSAuthentication.GetLoginUser());
            organizations.LoadByLikeOrganizationName(TSAuthentication.OrganizationID, searchTerm, true, 50, filterByUserRights);

            UsersView users = new UsersView(organizations.LoginUser);
            users.LoadByLikeName(TSAuthentication.OrganizationID, searchTerm, 50, true, true);

            List<AutocompleteItem> list = new List<AutocompleteItem>();
            for (int i = 0; i < organizations.Count; i++)
            {
                list.Add(new AutocompleteItem(organizations[i].Name, organizations[i].OrganizationID.ToString(), "o"));
            }

            for (int i = 0; i < users.Count; i++)
            {
                list.Add(new AutocompleteItem
                {
                    label = String.Format("<strong>{0} {1}</strong></br>{2}", users[i].FirstName, users[i].LastName, users[i].Organization),
                    id = users[i].UserID.ToString(),
                    data = "u",
                    value = users[i].FirstName + " " + users[i].LastName
                });
            }

            return list.ToArray();
        }

        [WebMethod]
        public AttachmentProxy[] GetActionAttachments(int ActionID)
        {
            return GetActionAttachments(ActionID, TSAuthentication.GetLoginUser());
        }

        [WebMethod]
        public AutocompleteItem[] SearchUsers(string term)
        {
            List<AutocompleteItem> result = new List<AutocompleteItem>();
            Users users = new Users(TSAuthentication.GetLoginUser());
            users.LoadByName(term, TSAuthentication.OrganizationID, true, false, false);

            //result.Add(new AutocompleteItem("Unassigned", "-1"));
            for (int i = 0; i < users.Count(); i++)
            {
                result.Add(new AutocompleteItem(users[i].DisplayName, users[i].UserID.ToString()));
            }
            return result.ToArray();
        }


        [WebMethod]
        public ProductProxy[] GetTicketCustomerProducts(int ticketID)
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

            return products.GetProductProxies();
        }

        [WebMethod]
        public string GetActionTicketTemplate(int actionTypeId)
        {
            TicketTemplate template = TicketTemplates.GetByActionType(TSAuthentication.GetLoginUser(), actionTypeId);
            if (template == null) return "";
            if (template.OrganizationID != TSAuthentication.OrganizationID) return "";
            return template.TemplateText;
        }

        [WebMethod]
        public object[] GetKBTicketAndActions(int ticketID)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
            ActionsView view = new ActionsView(TSAuthentication.GetLoginUser());
            view.LoadKBByTicketID(ticketID);

            List<object> result = new List<object>();
            result.Add(ticket.GetProxy());
            result.Add(view.GetActionsViewItemProxies());
            result.Add(GetActionAttachments(view[0].ActionID));
            return result.ToArray();
        }


        [WebMethod]
        public TimeLineItem NewWCPost(string data)
        {
            WatercoolerJsonInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<WatercoolerJsonInfo>(data);
            WatercoolerMsgItem wc = (new WatercoolerMsg(TSAuthentication.GetLoginUser())).AddNewWatercoolerMsgItem();
            int parentmsgid = info.ParentTicketID;
            int pageType = info.PageType;
            int pageID = info.PageID;

            wc.UserID = TSAuthentication.UserID;
            wc.OrganizationID = TSAuthentication.OrganizationID;
            wc.TimeStamp = DateTime.UtcNow;
            wc.LastModified = DateTime.UtcNow;
            wc.Message = info.Description;
            wc.MessageParent = parentmsgid;

            wc.Collection.Save();

            if (info.ParentTicketID != -1)
            {
                WatercoolerMsg wcm = new WatercoolerMsg(TSAuthentication.GetLoginUser());
                wcm.LoadByMessageID(parentmsgid);

                wcm[0].LastModified = DateTime.UtcNow;
                wcm[0].Collection.Save();
            }

            if (wc.MessageParent == -1 && info.PageType == 0)
                addWCAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Ticket);
            foreach (int ticketID in info.Tickets)
            {
                addWCAttachment(wc.MessageID, ticketID, WaterCoolerAttachmentType.Ticket);
                if (wc.MessageParent != -1)
                    addWCAttachment((int)wc.MessageParent, ticketID, WaterCoolerAttachmentType.Ticket);
            }

            if (wc.MessageParent == -1 && info.PageType == 1)
                addWCAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Product);
            foreach (int productID in info.Products)
            {
                addWCAttachment(wc.MessageID, productID, WaterCoolerAttachmentType.Product);
                if (wc.MessageParent != -1)
                    addWCAttachment((int)wc.MessageParent, productID, WaterCoolerAttachmentType.Product);
            }

            if (wc.MessageParent == -1 && info.PageType == 2)
                addWCAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Company);
            foreach (int CompanyID in info.Company)
            {
                addWCAttachment(wc.MessageID, CompanyID, WaterCoolerAttachmentType.Company);
                if (wc.MessageParent != -1)
                    addWCAttachment((int)wc.MessageParent, CompanyID, WaterCoolerAttachmentType.Company);
            }

            if (wc.MessageParent == -1 && info.PageType == 4)
                addWCAttachment((int)wc.MessageID, info.PageID, WaterCoolerAttachmentType.Group);
            foreach (int groupID in info.Groups)
            {
                addWCAttachment(wc.MessageID, groupID, WaterCoolerAttachmentType.Group);
                if (wc.MessageParent != -1)
                    addWCAttachment((int)wc.MessageParent, groupID, WaterCoolerAttachmentType.Group);
            }

            foreach (int UserID in info.User)
            {
                addWCAttachment(wc.MessageID, UserID, WaterCoolerAttachmentType.User);
                if (wc.MessageParent != -1)
                    addWCAttachment((int)wc.MessageParent, UserID, WaterCoolerAttachmentType.User);
            }

            //return wc.MessageID;
            return GetWCLineItem(wc);
        }

        [WebMethod]
        public object[] SetTicketType(int ticketID, int ticketTypeID)
        {
            TicketService ts = new TicketService();
            TransferCustomValues(ticketID, ticketTypeID);
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            int statusId = ticket.TicketStatusID;
            if (ticketTypeID == ticket.TicketTypeID) return null;
            //if (!CanEditTicket(ticket)) return null;
            TicketType ticketType = TicketTypes.GetTicketType(ticket.Collection.LoginUser, ticketTypeID);
            if (ticketType.OrganizationID != TSAuthentication.OrganizationID) return null;
            ticket.TicketTypeID = ticketTypeID;

            TicketStatuses statuses = new TicketStatuses(ticket.Collection.LoginUser);
            statuses.LoadAvailableTicketStatuses(ticketTypeID, null);
            ticket.TicketStatusID = statuses[0].TicketStatusID;
            TicketStatus newStatus = null;
            try
            {
                TicketStatus currStatus = TicketStatuses.GetTicketStatus(ticket.Collection.LoginUser, statusId);
                newStatus = statuses.Where(s => s.Name == currStatus.Name).FirstOrDefault();
                ticket.TicketStatusID = newStatus.TicketStatusID;
            }
            catch (Exception)
            {
            }
            ticket.Collection.Save();
            List<object> result = new List<object>();
            result.Add((newStatus != null) ? newStatus.GetProxy() : statuses[0].GetProxy());
            result.Add(ts.GetParentCustomValues(ticketID));
            TicketsViewItem tv = TicketsView.GetTicketsViewItemByNumber(TSAuthentication.GetLoginUser(), ticket.TicketNumber);
            result.Add(tv.GetProxy());
            return result.ToArray();
        }

        [WebMethod]
        public TimeLineItem ConvertActionItem(int actionID)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            TeamSupport.Data.Actions actions = new Actions(loginUser);
            actions.LoadByActionID(actionID);
            return GetActionTimelineItem(actions[0]);
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
                    else if (action.Pinned)
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
        public SlaInfo GetTicketSLAInfo(int ticketNumber)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            SlaInfo slaInfo = new SlaInfo();
            TicketsViewItem ticket = TicketsView.GetTicketsViewItemByNumber(loginUser, ticketNumber);
            if (ticket == null) return null;

            slaInfo.Ticket = ticket.GetProxy();

            SlaTickets slaTickets = new SlaTickets(loginUser);
            slaTickets.LoadByTicketId(ticket.TicketID);
            slaInfo.IsSlaPaused = false;
            slaInfo.IsSlaPending = false;
            slaInfo.SlaTriggerId = null;

            if (slaTickets != null && slaTickets.Count > 0)
            {
                TicketStatuses ticketStatus = new TicketStatuses(loginUser);
                ticketStatus.LoadByStatusIDs(TSAuthentication.OrganizationID, new int[] { ticket.TicketStatusID });
                slaInfo.IsSlaPaused = ticketStatus != null && ticketStatus[0].PauseSLA;

                slaInfo.IsSlaPending = slaTickets[0].IsPending;
                slaInfo.SlaTriggerId = slaTickets[0].SlaTriggerId;
            }

            return slaInfo;
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

        [DataContract]
        public class SlaInfo
        {
            [DataMember]
            public TicketsViewItemProxy Ticket { get; set; }
            [DataMember]
            public bool IsSlaPaused { get; set; }
            [DataMember]
            public int? SlaTriggerId { get; set; }
            [DataMember]
            public bool IsSlaPending { get; set; }
        }

        [DataContract]
        public class TicketPageInfo
        {
            [DataMember]
            public TicketsViewItemProxy Ticket { get; set; }
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
			[DataMember]
            public AttachmentProxy[] Attachments { get; set; }
            [DataMember]
            public bool IsSlaPaused { get; set; }
            [DataMember]
            public int? SlaTriggerId { get; set; }
            [DataMember]
            public bool IsSlaPending { get; set; }
            [DataMember]
            public PluginProxy[] Plugins { get; set; }
            [DataMember]
            public List<TaskDTO> Tasks { get; set; }
            [DataMember]
            public object faults { get; set; }
        }

        [DataContract]
        public class TicketPropertySelectField
        {
            [DataMember]
            public int ID { get; set; }
            [DataMember]
            public string Name { get; set; }
            [DataMember]
            public bool IsSelected { get; set; }
            [DataMember]
            public int? ProductFamilyID { get; set; }
        }

        [DataContract]
        public class TicketPageUser : TicketPropertySelectField
        {
            [DataMember]
            public string InOfficeMessage { get; set; }
            [DataMember]
            public bool IsCreator { get; set; }
            [DataMember]
            public bool IsSender { get; set; }
        }

        [DataContract]
        public class TimeLineItem
        {
            [DataMember]
            public TicketTimeLineViewItemProxy item { get; set; }
            [DataMember]
            public int? Likes { get; set; }
            [DataMember]
            public bool DidLike { get; set; }
            [DataMember]
            public AttachmentProxy[] Attachments { get; set; }
            [DataMember]
            public WaterCoolerReply[] WaterCoolerReplies { get; set; }

            public WaterCoolerThread WatercoolerReferences { get; set; }
        }

        [DataContract]
        public class WaterCoolerReply
        {
            [DataMember]
            public WatercoolerMsgItemProxy WaterCoolerReplyProxy { get; set; }
            [DataMember]
            public int Likes { get; set; }
            [DataMember]
            public bool DidLike { get; set; }
        }


        [DataContract(Namespace = "http://teamsupport.com/")]
        public class TicketCategoryOrder
        {
            [DataMember]
            public string CatID { get; set; }
            [DataMember]
            public string CatName { get; set; }
            [DataMember]
            public string Disabled { get; set; }
            [DataMember]
            public string ItemID { get; set; }
        }

        //Private Methods

        private void TransferCustomValues(int ticketID, int ticketTypeID)
        {
            TicketService ts = new TicketService();
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), ticketID);
            CustomValueProxy[] oldValues = ts.GetCustomValues(ticketID);
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

        private string CleanMessage(string message, LoginUser loginUser)
        {
            return HTMLSanitizeMessage(HtmlUtility.CheckScreenR(loginUser, message));
        }

        private string HTMLSanitizeMessage(string message)
        {
            return HtmlUtility.Sanitize(message);
        }

        private string SanitizeMessage(string message, LoginUser loginUser)
        {
            //return TagMessage(AddScreenrToMessage(message, loginUser), loginUser)
            return AddScreenrToMessage(message, loginUser);
        }

        private string TagMessage(string message, LoginUser loginUser)
        {
            return HtmlUtility.TagHtml(loginUser, message);
        }

        private string AddScreenrToMessage(string message, LoginUser loginUser)
        {
            return HtmlUtility.CheckScreenR(loginUser, message);
        }

        private bool CanEditAction(TeamSupport.Data.Action action)
        {
            Ticket ticket = Tickets.GetTicket(TSAuthentication.GetLoginUser(), action.TicketID);
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            return (ticket.OrganizationID == TSAuthentication.OrganizationID && (action.CreatorID == TSAuthentication.UserID || TSAuthentication.IsSystemAdmin || user.AllowUserToEditAnyAction));
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

            try
            {
                TicketLinkToTFS linkToTFS = new TicketLinkToTFS(TSAuthentication.GetLoginUser());
                linkToTFS.LoadByTicketID(ticketID);
                if (linkToTFS.Count > 0)
                {
                    result = linkToTFS[0].GetProxy();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "TFS getting data", "TicketPageService.GetLinkToTFS");
            }

            return result;
        }

		private TicketLinkToSnowItemProxy GetLinkToSnow(int ticketID)
		{
			TicketLinkToSnowItemProxy result = null;

			try
			{
				TicketLinkToSnow linkToSnow = new TicketLinkToSnow(TSAuthentication.GetLoginUser());
				linkToSnow.LoadByTicketID(ticketID);

				if (linkToSnow.Count > 0)
				{
					result = linkToSnow[0].GetProxy();
				}
			}
			catch (Exception ex)
			{
				ExceptionLogs.LogException(LoginUser.Anonymous, ex, "ServiceNow getting data", "TicketPageService.GetLinkToSnow");
			}

			return result;
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

        private AttachmentProxy[] GetAttachments(TicketsViewItem ticket)
        {
            AttachmentProxy[] proxies = TeamSupport.Data.Quarantine.WebAppQ.GetAttachmentProxies7(ticket.TicketID, ticket.Collection.LoginUser);

            List<AttachmentProxy> result = new List<AttachmentProxy>();
            foreach (AttachmentProxy attachment in proxies)
            {
                if (!result.Exists(a => a.FileName == attachment.FileName))
                    result.Add(attachment);
            }

            return result.ToArray();
        }


        private TimeLineItem GetActionTimelineItem(TeamSupport.Data.Action action)
        {
            LoginUser loginUser = TSAuthentication.GetLoginUser();
            User user = Users.GetUser(TSAuthentication.GetLoginUser(), action.CreatorID);
            TimeLineItem item = new TimeLineItem();

            string displayName = "";

            if (user != null)
                displayName = user.DisplayName;
            else if (Enum.IsDefined(typeof(SystemUser), action.CreatorID))
                displayName = ((SystemUser)action.CreatorID).ToString();

            TicketTimeLineViewItemProxy itemProxy = new TicketTimeLineViewItemProxy
            {
                TicketID = action.TicketID,
                RefID = action.ActionID,
                IsWC = false,
                MessageType = action.ActionTypeName,
                ActionTypeID = action.ActionTypeID,
                Message = SanitizeMessage(action.Description, loginUser),
                DateCreated = DateTime.SpecifyKind(action.DateCreatedUtc, DateTimeKind.Utc),
                DateStarted = action.DateStartedUtc != null ? DateTime.SpecifyKind((DateTime)action.DateStartedUtc, DateTimeKind.Utc) : action.DateStartedUtc,
                OrganizationID = TSAuthentication.OrganizationID,
                CreatorID = action.CreatorID,
                CreatorName = displayName,
                IsKnowledgeBase = action.IsKnowledgeBase,
                IsVisibleOnPortal = action.IsVisibleOnPortal,
                IsPinned = action.Pinned,
                TimeSpent = action.TimeSpent,
                WCUserID = null
            };

            item.item = itemProxy;

            if (SystemSettings.ReadString(loginUser, "KillScreenR", false.ToString()).ToLower().IndexOf("t") > 0)
            {
                if (action.Description != item.item.Message)
                {
                    ExceptionLog log = (new ExceptionLogs(loginUser)).AddNewExceptionLog();
                    log.ExceptionName = "TinyMCE Error";
                    log.Message = "The original message and the saved message are different and could be trimmed.";
                    log.Collection.Save();

                    return null;
                }
            }

            int actionID = item.item.RefID;
            item.Attachments = Model_API.Read<AttachmentProxy[]>(actionID);

            return item;
        }

        private TimeLineItem GetWCLineItem(TeamSupport.Data.WatercoolerMsgItem wc)
        {
            TicketTimeLineView TimeLineView = new TicketTimeLineView(TSAuthentication.GetLoginUser());
            TimeLineView.LoadByRefIDAndType(wc.MessageID, true);

            TimeLineItem wcItem = new TimeLineItem();
            wcItem.item = TimeLineView[0].GetProxy();
            wcItem.Likes = 0;
            wcItem.DidLike = false;

            List<WaterCoolerReply> wcReplies = new List<WaterCoolerReply>();
            wcItem.WaterCoolerReplies = wcReplies.ToArray();

            WatercoolerAttachments threadAttachments = new WatercoolerAttachments(TSAuthentication.GetLoginUser());
            WaterCoolerThread thread = new WaterCoolerThread();
            threadAttachments.LoadByMessageID(wcItem.item.RefID);
            thread.Groups = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Group);
            thread.Tickets = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Ticket);
            thread.Products = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Product);
            thread.Company = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.Company);
            thread.User = threadAttachments.GetWatercoolerAttachmentProxies(WaterCoolerAttachmentType.User);
            wcItem.WatercoolerReferences = thread;

            return wcItem;
        }

        private AttachmentProxy[] GetActionAttachments(int actionID, LoginUser loginUser)
        {
            // Read action attachments
            return Model_API.Read<AttachmentProxy[]>(actionID);
        }


        private void addWCAttachment(int messageID, int attachmentID, WaterCoolerAttachmentType attachmentType)
        {
            try
            {
                WatercoolerAttachment ticketAttachment = (new WatercoolerAttachments(TSAuthentication.GetLoginUser()).AddNewWatercoolerAttachment());
                ticketAttachment.MessageID = messageID;
                ticketAttachment.AttachmentID = attachmentID;
                ticketAttachment.RefType = attachmentType;
                ticketAttachment.DateCreated = DateTime.UtcNow;
                ticketAttachment.CreatorID = TSAuthentication.GetLoginUser().UserID;
                ticketAttachment.Collection.Save();
            }
            catch (Exception e)
            {

            }
        }

        private TimeLineItem LogAction(int ticketID, SystemActionType actionType, string name, string actionText, string actionLog)
        {
            TicketsViewItem ticket = TicketsView.GetTicketsViewItem(TSAuthentication.GetLoginUser(), ticketID);
            User user = TSAuthentication.GetUser(TSAuthentication.GetLoginUser());
            TeamSupport.Data.Action action = (new Actions(TSAuthentication.GetLoginUser())).AddNewAction();
            action.ActionTypeID = null;
            action.Name = name;
            action.ActionSource = name;
            action.SystemActionTypeID = actionType;
            action.Description = actionText;
            action.IsVisibleOnPortal = false;
            action.IsKnowledgeBase = false;
            action.TicketID = ticket.TicketID;
            action.CreatorID = TSAuthentication.UserID;
            action.Collection.Save();

            ActionLogs.AddActionLog(TSAuthentication.GetLoginUser(), ActionLogType.Update, ReferenceType.Tickets, ticket.TicketID, actionLog);

            return GetActionTimelineItem(action);
        }

        private void CopyInsertedKBAttachments(int actionID, int insertedKBTicketID)
        {
            AttachmentProxy[] results;
            Model_API.ReadActionAttachmentsForTicket(insertedKBTicketID, ActionAttachmentsByTicketID.KnowledgeBase, out results);
            foreach (AttachmentProxy attachment in results)
            {
                attachment.AttachmentID = 0;    // not used
                string originalAttachmentRefID = ((ActionAttachmentProxy)attachment).ActionID.ToString();
                string clonedActionAttachmentPath = attachment.Path.Substring(0, attachment.Path.IndexOf(@"\Actions\") + @"\Actions\".Length)
                                + actionID.ToString()
                                + attachment.Path.Substring(attachment.Path.IndexOf(originalAttachmentRefID) + originalAttachmentRefID.Length);
                if (!Directory.Exists(Path.GetDirectoryName(clonedActionAttachmentPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(clonedActionAttachmentPath));
                File.Copy(attachment.Path, clonedActionAttachmentPath);
                attachment.Path = clonedActionAttachmentPath;

                Model_API.Create(attachment);
            }
        }

    }
}
