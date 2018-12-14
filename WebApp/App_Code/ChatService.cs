using System.Web.Services;
using System.Web.Script.Services;
using System.Data;
using PusherServer;
using Newtonsoft.Json;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Web.Script.Serialization;
using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using OpenTokSDK;

namespace TSWebServices
{
    [ScriptService]
    [WebService(Namespace = "http://teamsupport.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ChatService : System.Web.Services.WebService
    {
        PusherOptions options = new PusherOptions();
        Pusher pusher;
        LoginUser loginUser;
        Organization parentOrganization;

        public ChatService()
        {
            options.Encrypted = true;
            string pusherKey = SystemSettings.GetPusherKey();
            string pusherAppId = SystemSettings.GetPusherAppId();
            string pusherSecret = SystemSettings.GetPusherSecret();

            pusher = new Pusher(pusherAppId, pusherKey, pusherSecret, options);

            loginUser = TSAuthentication.GetLoginUser();
        }

		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public string GetHTMLGlobalEventAttributes()
		{
			List<string> events = Chat.GetHTMLGlobalEventAttributes();
			return JsonConvert.SerializeObject(events);
		}

		#region ClientServices

		[WebMethod]
        public bool CheckChatStatus(string chatGuid, string groupName = null, string groupID = null)
        {
            Organization org = GetOrganization(chatGuid);
            int groupParseResult;
            int? chatGroupID = ChatRequests.CalculateChatGroupID(LoginUser.Anonymous, org.OrganizationID, Int32.TryParse(groupID, out groupParseResult) ? (int?)groupParseResult : null, groupName);
            bool areOperatorsAvailable = ChatRequests.AreOperatorsAvailable(LoginUser.Anonymous, org.OrganizationID, chatGroupID);

            if (areOperatorsAvailable)
            {
                string channel = "chat-requests-" + org.ChatID;
                ChannelOccupiedInfo channelOccupiedInfo = new ChannelOccupiedInfo() { occupied = false };

                try
                {
                    IGetResult<object> channelInfo = pusher.Get<object>("/channels/" + channel.ToLower());

                    if (!string.IsNullOrEmpty(channelInfo.Body))
                    {
                        channelOccupiedInfo = JsonConvert.DeserializeObject<ChannelOccupiedInfo>(channelInfo.Body);
                    }
                    else
                    {
                        channelInfo = pusher.Get<object>("/channels/" + channel.ToUpper());

                        if (!string.IsNullOrEmpty(channelInfo.Body))
                        {
                            channelOccupiedInfo = JsonConvert.DeserializeObject<ChannelOccupiedInfo>(channelInfo.Body);
                        }
                    }

                    areOperatorsAvailable = areOperatorsAvailable && channelOccupiedInfo.occupied;
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(LoginUser.Anonymous, ex, "ChatService.CheckChatStatus");
                }
            }

            return areOperatorsAvailable;
        }

        [WebMethod]
        public string GetClientChatPropertiesByChatID(int chatID)
        {
            Chat chat = GetChat(chatID);
            ChatSettingsModel model = new ChatSettingsModel();

            model.ChatIntro = OrganizationSettings.ReadString(loginUser, chat.OrganizationID, "ChatIntroMessage", "Welcome to our live chat!");
            model.TOKScreenEnabled = bool.Parse(OrganizationSettings.ReadString(loginUser, chat.OrganizationID, "ChatTOKScreenEnabled", "false"));
            model.TOKVoiceEnabled = bool.Parse(OrganizationSettings.ReadString(loginUser, chat.OrganizationID, "ChatTOKVoiceEnabled", "false"));
            model.TOKVideoEnabled = bool.Parse(OrganizationSettings.ReadString(loginUser, chat.OrganizationID, "ChatTOKVideoEnabled", "false"));
            model.ChatAvatarsEnabled = bool.Parse(OrganizationSettings.ReadString(loginUser, chat.OrganizationID, "ChatAvatarsEnabled", "false"));

            return JsonConvert.SerializeObject(model);
        }

        [WebMethod]
        public string GetClientChatPropertiesByChatGUID(string chatGuid)
        {
            Organization org = GetOrganization(chatGuid);
            ChatSettingsModel model = new ChatSettingsModel();

            model.ChatIntro = OrganizationSettings.ReadString(loginUser, org.OrganizationID, "ChatIntroMessage", "Welcome to our live chat!");
            model.TOKScreenEnabled = bool.Parse(OrganizationSettings.ReadString(loginUser, org.OrganizationID, "ChatTOKScreenEnabled", "false"));
            model.TOKVoiceEnabled = bool.Parse(OrganizationSettings.ReadString(loginUser, org.OrganizationID, "ChatTOKVoiceEnabled", "false"));
            model.TOKVideoEnabled = bool.Parse(OrganizationSettings.ReadString(loginUser, org.OrganizationID, "ChatTOKVideoEnabled", "false"));
            model.ChatAvatarsEnabled = bool.Parse(OrganizationSettings.ReadString(loginUser, org.OrganizationID, "ChatAvatarsEnabled", "false"));
            model.OrganizationID = org.OrganizationID;

            return JsonConvert.SerializeObject(model);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPusherKey()
        {
            string pusherKey = SystemSettings.GetPusherKey();
            return JsonConvert.SerializeObject(pusherKey);
        }

        [WebMethod]
        public string GetChatInfo(int chatID)
        {
            ChatRequestProxy request = GetChatRequest(chatID);
            ChatViewObject model = new ChatViewObject(request, GetParticipant(request.RequestorID, chatID), GetChatMessages(chatID));
            return JsonConvert.SerializeObject(model);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string RequestChat(string chatGuid, string fName, string lName, string email, string description, string groupName = null, string groupID = null)
        {
            Organization org = GetOrganization(chatGuid);
            int groupParseResult;
			int? groupId = ChatRequests.CalculateChatGroupID(LoginUser.Anonymous, org.OrganizationID, Int32.TryParse(groupID, out groupParseResult) ? (int?)groupParseResult : null, groupName);
			ChatRequest request = ChatRequests.RequestChat(LoginUser.Anonymous, org.OrganizationID, fName, lName, email, description, Context.Request.UserHostAddress, groupId ?? 0);

            pusher.Trigger("chat-requests-" + org.ChatID, "new-chat-request",
				new {
					message = string.Format("{0} {1} is requesting a chat!", fName, lName),
					title = "Chat Request",
					theme = "ui-state-error",
					chatRequest = new ChatViewObject(request.GetProxy(), GetParticipant(request.RequestorID, request.ChatID), GetChatMessages(request.ChatID), groupId ?? 0)
				});
            return JsonConvert.SerializeObject(request.GetProxy());
        }

        [WebMethod]
        public void OfflineChat(string chatGuid, string fName, string lName, string email, string description, string groupID = null)
        {
            int groupParseResult;

            Organization _organization = GetOrganization(chatGuid);
            Ticket ticket = (new Tickets(LoginUser.Anonymous)).AddNewTicket();
            ticket.OrganizationID = _organization.OrganizationID;
            ticket.GroupID = Int32.TryParse(groupID, out groupParseResult) && groupParseResult > 0 ? (int)groupParseResult : _organization.DefaultPortalGroupID;
            ticket.IsKnowledgeBase = false;
            ticket.IsVisibleOnPortal = true;
            ticket.Name = "Offline Chat Question";
            ticket.TicketSeverityID = TicketSeverities.GetTop(LoginUser.Anonymous, _organization.OrganizationID).TicketSeverityID;
            ticket.TicketTypeID = TicketTypes.GetTop(LoginUser.Anonymous, _organization.OrganizationID).TicketTypeID;
            ticket.TicketStatusID = TicketStatuses.GetTop(LoginUser.Anonymous, ticket.TicketTypeID).TicketStatusID;
            ticket.TicketSource = "ChatOffline";
            ticket.PortalEmail = email;
            ticket.Collection.Save();

            StringBuilder builder = new StringBuilder();
            builder.Append("<h2>Offline Chat Request</h2>");
            builder.Append("<table cellspacing=\"0\" cellpadding=\"5\" border=\"0\">");
            builder.Append("<tr><td><strong>First Name:</strong></td><td>" + fName + "</td></tr>");
            builder.Append("<tr><td><strong>Last Name:</strong></td><td>" + lName + "</td></tr>");
            builder.Append("<tr><td><strong>Email:</strong></td><td><a href=\"mailto:" + email + "\">" + email + "</td></tr>");
            builder.Append("<tr><td colspan=\"2\"><strong>Question:</strong></td></tr>");
            builder.Append("<tr><td colspan=\"2\">" + description + "</td></tr>");
            builder.Append("</table>");


            TeamSupport.Data.Action action = (new Actions(LoginUser.Anonymous)).AddNewAction();
            action.ActionTypeID = null;
            action.SystemActionTypeID = SystemActionType.Description;
            action.Description = builder.ToString();
            action.IsKnowledgeBase = false;
            action.IsVisibleOnPortal = true;
            action.ActionSource = "ChatOffline";
            action.Name = "Description";
            action.TicketID = ticket.TicketID;
            action.Collection.Save();

            Users users = new Users(LoginUser.Anonymous);
            users.LoadByEmailOrderByActive(_organization.OrganizationID, email);
            if (!users.IsEmpty) ticket.Collection.AddContact(users[0].UserID, ticket.TicketID);
        }

        [WebMethod]
        public void MissedChat(int chatID)
        {
            ChatRequestProxy request = GetChatRequest(chatID);
            Organization organization = Organizations.GetOrganization(LoginUser.Anonymous, request.OrganizationID);

            //Clear the chat from the pending chats if this is already a missed chat
            try
            {
                var result = pusher.Trigger("chat-requests-" + organization.ChatID, "chat-request-accepted", request.ChatRequestID);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(LoginUser.Anonymous, ex, "ChatService.MissedChat");
            }

            ChatClient client = ChatClients.GetChatClient(LoginUser.Anonymous, request.RequestorID);

            Ticket ticket = (new Tickets(LoginUser.Anonymous)).AddNewTicket();
            ticket.OrganizationID = organization.OrganizationID;
            ticket.GroupID = organization.DefaultPortalGroupID;
            ticket.IsKnowledgeBase = false;
            ticket.IsVisibleOnPortal = true;
            ticket.Name = "Missed Chat";
            ticket.TicketSeverityID = TicketSeverities.GetTop(LoginUser.Anonymous, organization.OrganizationID).TicketSeverityID;
            ticket.TicketTypeID = TicketTypes.GetTop(LoginUser.Anonymous, organization.OrganizationID).TicketTypeID;
            ticket.TicketStatusID = TicketStatuses.GetTop(LoginUser.Anonymous, ticket.TicketTypeID).TicketStatusID;
            ticket.TicketSource = "ChatOffline";
            ticket.PortalEmail = client.Email;
            ticket.Collection.Save();

            StringBuilder builder = new StringBuilder();
            builder.Append("<h2>Missed Chat Request</h2>");
            builder.Append("<table cellspacing=\"0\" cellpadding=\"5\" border=\"0\">");
            builder.Append("<tr><td><strong>First Name:</strong></td><td>" + client.FirstName + "</td></tr>");
            builder.Append("<tr><td><strong>Last Name:</strong></td><td>" + client.LastName + "</td></tr>");
            builder.Append("<tr><td><strong>Email:</strong></td><td><a href=\"mailto:" + client.Email + "\">" + client.Email + "</td></tr>");
            builder.Append("<tr><td colspan=\"2\"><strong>Question:</strong></td></tr>");
            builder.Append("<tr><td colspan=\"2\">" + request.Message + "</td></tr>");
            builder.Append("</table>");


            TeamSupport.Data.Action action = (new Actions(LoginUser.Anonymous)).AddNewAction();
            action.ActionTypeID = null;
            action.SystemActionTypeID = SystemActionType.Description;
            action.Description = builder.ToString();
            action.IsKnowledgeBase = false;
            action.IsVisibleOnPortal = true;
            action.ActionSource = "ChatOffline";
            action.Name = "Description";
            action.TicketID = ticket.TicketID;
            action.Collection.Save();

            Users users = new Users(LoginUser.Anonymous);
            users.LoadByEmailOrderByActive(organization.OrganizationID, client.Email);
            if (!users.IsEmpty) ticket.Collection.AddContact(users[0].UserID, ticket.TicketID);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void Auth(string channel_name, string socket_id, int chatId, int participantID)
        {
            ChatClient client = ChatClients.GetChatClient(loginUser, participantID);

            var channelData = new PresenceChannelData()
            {
                user_id = client.ChatClientID.ToString(),
                user_info = new
                {
                    name = client.FirstName + ' ' + client.LastName,
                    company = client.CompanyName,
                    chatId = chatId
                }
            };

            var auth = pusher.Authenticate(channel_name, socket_id, channelData);
            var json = auth.ToJson();
            Context.Response.Write(json);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddMessage(string channelName, string message, int chatID, int userID)
        {
            Chat chat = GetChat(chatID);

            ChatMessage chatMessage = (new ChatMessages(loginUser)).AddNewChatMessage();
            chatMessage.Message = message;
            chatMessage.ChatID = chatID;
            chatMessage.PosterID = userID;
            chatMessage.PosterType = ChatParticipantType.External;
            chatMessage.Collection.Save();

            ChatViewMessage newMessage = new ChatViewMessage(chatMessage.GetProxy(), GetLinkedUserInfo(userID, ChatParticipantType.External));
            var result = pusher.Trigger(channelName, "new-comment", newMessage);
            return JsonConvert.SerializeObject(true);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UserTyping(string channelName, int userID, bool isTyping)
        {
            ParticipantInfoView user = GetLinkedUserInfo(userID, ChatParticipantType.External);

           var result = pusher.Trigger(channelName, (isTyping) ? "user-typing" : "user-stop-typing", string.Format("{0} {1} is typing...", user.FirstName, user.LastName));
            return JsonConvert.SerializeObject(true);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddUserUploadMessage(string channelName, int chatID, int attachmentID, int userID)
        {
            Chat chat = GetChat(chatID);
            AttachmentProxy attachment = TeamSupport.ModelAPI.Model_API.Read<AttachmentProxy>(attachmentID);
            string attachmentHTML = "";

            if (attachment.FileType.StartsWith("image/"))
                attachmentHTML = string.Format("<img src='" + SystemSettings.GetAppUrl() + "/dc/{0}/chatattachments/{1}/{2}' alt='{3}'>", attachment.OrganizationID, chatID, attachmentID, attachment.FileName);
            else
                attachmentHTML = string.Format("<a target='_blank' href='" + SystemSettings.GetAppUrl() + "/dc/{0}/chatattachments/{1}/{2}'>{3}</a>", attachment.OrganizationID, chatID, attachmentID, attachment.FileName);

            ChatMessage chatMessage = (new ChatMessages(loginUser)).AddNewChatMessage();
            chatMessage.Message = attachmentHTML;
            chatMessage.ChatID = chatID;
            chatMessage.PosterID = userID;
            chatMessage.PosterType = ChatParticipantType.External;
            chatMessage.Collection.Save();

            ChatViewMessage newMessage = new ChatViewMessage(chatMessage.GetProxy(), GetLinkedUserInfo(userID, ChatParticipantType.External));

            var result = pusher.Trigger(channelName, "new-comment", newMessage);
            return JsonConvert.SerializeObject(true);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetContact(string chatGuid, string fName, string lName, string email)
        {
            Organization org = GetOrganization(chatGuid);
            Users users = new Users(loginUser);
            users.LoadByEmail(org.OrganizationID, email);

            if (users.IsEmpty) return null;
            else return JsonConvert.SerializeObject(users[0].GetProxy());
        }

        [Obsolete("This method seems deprecated")]
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string DisconnectUser(string channelName, int chatID, int userID)
        {
            ParticipantInfoView participant = GetParticipant(userID, chatID);

            ChatMessage chatMessage = (new ChatMessages(loginUser)).AddNewChatMessage();
            chatMessage.Message = string.Format("{0} {1} has left the chat.", participant.FirstName, participant.LastName);
            chatMessage.ChatID = chatID;
            chatMessage.PosterID = userID;
            chatMessage.PosterType = ChatParticipantType.External;
            chatMessage.IsNotification = true;
            chatMessage.Collection.Save();

            ChatViewMessage newMessage = new ChatViewMessage(chatMessage.GetProxy(), GetLinkedUserInfo(userID, ChatParticipantType.External));

            var result = pusher.Trigger(channelName, "new-comment", newMessage);
            return JsonConvert.SerializeObject(true);
        }

        [WebMethod]
        public bool CustomerAbandonedChatRequest(int chatID, int userID)
        {
            Chats chat = new Chats(LoginUser.Anonymous);
            chat.LoadByChatID(chatID);

            if (!chat.IsEmpty)
            {
                Guid organizationChatGuid = Organizations.GetOrganization(LoginUser.Anonymous, chat[0].OrganizationID).ChatID;
                ChatRequests requests = new ChatRequests(LoginUser.Anonymous);
                requests.LoadByChatID(chatID, ChatRequestType.External);

                if (!requests.IsEmpty)
                {
                    var result = pusher.Trigger("chat-requests-" + organizationChatGuid.ToString(), "chat-request-abandoned", requests[0].ChatRequestID);
                    ChatMessageProxy message = Chats.AbandonedChatRequest(LoginUser.Anonymous, chat[0], userID, ChatParticipantType.External, chatID);
                }
            }

            return true;
        }

        #endregion

        #region AgentServices

        [WebMethod]
        public ChatSettingsModel GetAgentChatProperties()
        {
            ChatSettingsModel model = new ChatSettingsModel();

            model.ChatIntro = Settings.OrganizationDB.ReadString("ChatIntroMessage", "Welcome to our live chat!");
            model.TOKScreenEnabled = Settings.OrganizationDB.ReadBool("ChatTOKScreenEnabled", false);
            model.TOKVoiceEnabled = Settings.OrganizationDB.ReadBool("ChatTOKVoiceEnabled", false);
            model.TOKVideoEnabled = Settings.OrganizationDB.ReadBool("ChatTOKVideoEnabled", false);

            return model;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<ChatViewObject> GetChatRequests()
        {
            List<ChatViewObject> pendingChats = new List<ChatViewObject>();
            ChatRequests requests = new ChatRequests(loginUser);
            requests.LoadPendingRequests(loginUser.UserID, loginUser.OrganizationID);

            if (!requests.IsEmpty)
            {
                foreach (ChatRequest request in requests)
                {
                    ChatViewObject vm = new ChatViewObject(request.GetProxy(), GetParticipant(request.RequestorID, request.ChatID), null);
                    pendingChats.Add(vm);
                }
            }

            return pendingChats;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public List<ChatViewObject> GetActiveChats()
        {
            List<ChatViewObject> activeChats = new List<ChatViewObject>();

            ChatRequests requests = new ChatRequests(loginUser);
            requests.LoadActiveChatsByUserId(loginUser.UserID, loginUser.OrganizationID);

            if (!requests.IsEmpty)
            {
                foreach (ChatRequest request in requests)
                {
                    ChatViewObject vm = new ChatViewObject(request.GetProxy(), GetParticipant(request.RequestorID, request.ChatID), null);
                    activeChats.Add(vm);
                }
            }

            return activeChats;
        }

        [WebMethod]
        public int AcceptRequest(int chatRequestID, bool isTOKEnabled)
        {
            int chatID = ChatRequests.AcceptRequest(loginUser, loginUser.UserID, chatRequestID, HttpContext.Current.Request.UserHostAddress);

            if (chatID > 0)
            {
                var result = pusher.Trigger("presence-" + chatID, "agent-joined", new { isAgentTOKEnabled = isTOKEnabled });
                var result2 = pusher.Trigger("chat-requests-" + loginUser.GetOrganization().ChatID, "chat-request-accepted", chatRequestID);
            }

            return chatID;
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public void AgentAuth(string channel_name, string socket_id)
		{
			int chatId = 0;
			bool gotChatId = int.TryParse(channel_name.ToLower().Trim().Replace("presence-", ""), out chatId);

			var channelData = new PresenceChannelData()
            {
                user_id = loginUser.UserID.ToString(),
                user_info = new
                {
                    name = loginUser.GetUserFullName(),
                    isAgent = true,
                    chatId = chatId
                }

            };

            var auth = pusher.Authenticate(channel_name, socket_id, channelData);
            var json = auth.ToJson();
            Context.Response.Write(json);
        }

        [WebMethod]
        public ChatViewObject GetChatDetails(int chatID)
        {
            ChatRequestProxy request = GetChatRequest(chatID);
            ChatViewObject model = new ChatViewObject(request, GetParticipant(request.RequestorID, request.ChatID), GetChatMessages(chatID));
            return model;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddAgentMessage(string channelName, string message, int chatID)
        {
            Chat chat = GetChat(chatID);

            ChatMessage chatMessage = (new ChatMessages(loginUser)).AddNewChatMessage();
            chatMessage.Message = message;
            chatMessage.ChatID = chatID;
            chatMessage.PosterID = loginUser.UserID;
            chatMessage.PosterType = ChatParticipantType.User;
            chatMessage.Collection.Save();
            Users.UpdateUserActivityTime(loginUser, loginUser.UserID);

            User user = loginUser.GetUser();
            ChatViewMessage newMessage = new ChatViewMessage(chatMessage.GetProxy(), new ParticipantInfoView(user.UserID, user.FirstName, user.LastName, user.Email, loginUser.GetOrganization().Name));

            var result = pusher.Trigger(channelName, "new-comment", newMessage);
            return JsonConvert.SerializeObject(true);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AgentTyping(string channelName, bool isTyping)
        {
            User user = loginUser.GetUser();
            var result = pusher.Trigger(channelName, (isTyping) ? "agent-typing" : "agent-stop-typing", string.Format("{0} is typing...", user.DisplayName));
            return JsonConvert.SerializeObject(true);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddAgentUploadtMessage(string channelName, int chatID, int attachmentID)
        {
            Chat chat = GetChat(chatID);
            AttachmentProxy attachment = TeamSupport.ModelAPI.Model_API.Read<AttachmentProxy>(attachmentID);
            string attachmentHTML = "";

            if (attachment.FileType.StartsWith("image/"))
                attachmentHTML = string.Format("<img src='" + SystemSettings.GetAppUrl() + "/dc/{0}/chatattachments/{1}/{2}' alt='{3}'>", TSAuthentication.OrganizationID, chatID, attachmentID, attachment.FileName);
            else
                attachmentHTML = string.Format("<a target='_blank' href='" + SystemSettings.GetAppUrl() + "/dc/{0}/chatattachments/{1}/{2}'>{3}</a>", TSAuthentication.OrganizationID, chatID, attachmentID, attachment.FileName);

            ChatMessage chatMessage = (new ChatMessages(loginUser)).AddNewChatMessage();
            chatMessage.Message = attachmentHTML;
            chatMessage.ChatID = chatID;
            chatMessage.PosterID = loginUser.UserID;
            chatMessage.PosterType = ChatParticipantType.User;
            chatMessage.Collection.Save();
            Users.UpdateUserActivityTime(loginUser, loginUser.UserID);

            User user = loginUser.GetUser();
            ChatViewMessage newMessage = new ChatViewMessage(chatMessage.GetProxy(), new ParticipantInfoView(user.UserID, user.FirstName, user.LastName, user.Email, loginUser.GetOrganization().Name));

            var result = pusher.Trigger(channelName, "new-comment", newMessage);
            return JsonConvert.SerializeObject(true);
        }

        [WebMethod]
        public int GetTicketID(int chatID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            if (chat.ActionID == null) return -1;

            TeamSupport.Data.Action action = Actions.GetAction(loginUser, (int)chat.ActionID);
            if (action == null) return -1;
            return action.TicketID;
        }

        [WebMethod]
        public int AddTicket(int chatID, int ticketID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            if (chat != null)
            {
                Actions actions = new Actions(loginUser);
                TeamSupport.Data.Action chatAction = actions.AddNewAction();
                chatAction.ActionTypeID = null;
                chatAction.Name = "Chat";
                chatAction.ActionSource = "Chat";
                chatAction.SystemActionTypeID = SystemActionType.Chat;
                chatAction.Description = chat.GetHtml(true, loginUser.OrganizationCulture);
                chatAction.IsVisibleOnPortal = false;
                chatAction.IsKnowledgeBase = false;
                chatAction.TicketID = ticketID;
                actions.Save();
                chat.ActionID = chatAction.ActionID;
                chat.Collection.Save();

                int requestorUserId = chat.GetInitiatorLinkedUserID();

                if (requestorUserId > 0)
                {
                    (new Tickets(loginUser)).AddContact(chat.GetInitiatorLinkedUserID(), ticketID);
                }
            }
            return ticketID;
        }

        [WebMethod]
        public void RequestTransfer(int chatID, int userID)
        {
            ChatRequests.RequestTransfer(loginUser, chatID, userID);
        }

        [WebMethod]
        public void RequestInvite(int chatID, int userID)
        {
            Organization organization = loginUser.GetOrganization();
            ChatRequest request = ChatRequests.RequestInvite(loginUser, chatID, userID);
            ChatRequests customerChatRequest = new ChatRequests(loginUser);
            customerChatRequest.LoadByChatID(chatID, ChatRequestType.External);

            ParticipantInfoView initialRequestor = new ParticipantInfoView();
            ParticipantInfoView userRequestor = new ParticipantInfoView();

            if (customerChatRequest != null && customerChatRequest.Count > 0)
            {
                initialRequestor = GetParticipant(customerChatRequest[0].RequestorID, customerChatRequest[0].ChatID);
                userRequestor = null;
            }
            else
            {
                userRequestor = GetParticipant(request.RequestorID, request.ChatID);
                initialRequestor = null;
            }

            pusher.Trigger("chat-requests-" + organization.ChatID, "new-chat-request",
                new {
                    message = string.Format("{0} {1} is inviting you to a chat!", loginUser.GetUser().FirstName, loginUser.GetUser().LastName),
                    title = "Chat Invite",
                    theme = "ui-state-error",
                    chatRequest = new ChatViewObject(request.GetProxy(), initialRequestor != null ? initialRequestor : userRequestor, GetChatMessages(request.ChatID)),
                    userIdInvited = userID});
        }

        [WebMethod]
        public AutocompleteItem[] GetUsers(string searchTerm)
        {
            List<AutocompleteItem> list = new List<AutocompleteItem>();
            Users users = new Users(loginUser);
            users.LoadByName(searchTerm, loginUser.OrganizationID, true, false, true, loginUser.UserID);

            foreach (User user in users)
            {
                list.Add(new AutocompleteItem(String.Format("{0} {1}", user.FirstName, user.LastName), user.UserID.ToString()));
            }

            return list.ToArray();
        }

        [WebMethod]
        public bool CloseChat(string channelName, int chatID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            if (chat.OrganizationID != loginUser.OrganizationID) return false;
            ChatMessageProxy message = Chats.LeaveChat(loginUser, loginUser.UserID, ChatParticipantType.User, chatID);

            if (message != null)
            {
                User user = loginUser.GetUser();
                ChatViewMessage newMessage = new ChatViewMessage(message, new ParticipantInfoView(user.UserID, user.FirstName, user.LastName, user.Email, loginUser.GetOrganization().Name), hasLeft: true);
                var result = pusher.Trigger(channelName, "new-comment", newMessage);
            }

            return true;
        }

        [WebMethod]
        public bool RemoveUser(string channelName, int chatID, int userID)
        {
            Chat chat = Chats.GetChat(loginUser, chatID);
            ChatMessageProxy message = Chats.LeaveChat(loginUser, userID, ChatParticipantType.External, chatID);

            if (message != null)
            {
                ChatViewMessage newMessage = new ChatViewMessage(message, GetLinkedUserInfo(userID, ChatParticipantType.External), hasLeft: true);
                var result = pusher.Trigger(channelName, "new-comment", newMessage);
            }

            return true;
        }

        //TODO: Refactor into data layer...
        [WebMethod]
        public SuggestedSolutions GetSuggestedSolutions(int chatID, int firstItemIndex, int pageSize)
        {
            SuggestedSolutions result = new SuggestedSolutions();
            List<SuggestedSolutionsItem> resultItems = new List<SuggestedSolutionsItem>();

            if (chatID != 0)
            {
                LoginUser loginUser = TSAuthentication.GetLoginUser();
                string searchTerm = GetSuggestedSolutionDefaultInput(chatID);
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

        //TODO: Refactor into data layer...
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


        #endregion

        #region TOK

        [WebMethod]
        public List<string> GetTOKSessionInfo()
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            var session = OpenTok.CreateSession(mediaMode: MediaMode.ROUTED, archiveMode: ArchiveMode.MANUAL);
            var token = OpenTok.GenerateToken(session.Id, Role.PUBLISHER);
            var values = new List<string>();
            values.Add(session.Id);
            values.Add(token);
            values.Add(SystemSettings.GetTokApiKey());
            return values;
        }

        [WebMethod]
        public string GetTOKSessionInfoClient(string chatID = "")
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            var session = OpenTok.CreateSession(mediaMode: MediaMode.ROUTED, archiveMode: ArchiveMode.MANUAL);
            var token = OpenTok.GenerateToken(session.Id, Role.PUBLISHER);
            var values = new List<string>();
            values.Add(session.Id);
            values.Add(token);
            values.Add(SystemSettings.GetTokApiKey());
            //return values;
            return JsonConvert.SerializeObject(values);
        }


        [WebMethod]
        public string StartArchiving(string sessionId)
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            var archive = OpenTok.StartArchive(sessionId);
            return archive.Id.ToString();
        }

        [WebMethod]
        public string StartArchivingClient(string sessionId)
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            var archive = OpenTok.StartArchive(sessionId);
            //return archive.Id.ToString();
            return JsonConvert.SerializeObject(archive.Id.ToString());
        }

        [WebMethod]
        public string StopArchiving(string archiveId)
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            System.Threading.Thread.Sleep(1500);
            var archive = OpenTok.StopArchive(archiveId);
            do
            {

            }
            while (OpenTok.GetArchive(archiveId).Status != ArchiveStatus.UPLOADED);

            TokStorageItem ts = (new TokStorage(TSAuthentication.GetLoginUser())).AddNewTokStorageItem();
            ts.AmazonPath = string.Format("https://s3.amazonaws.com/{2}/{0}/{1}/archive.mp4", SystemSettings.GetTokApiKey(), archive.Id, SystemSettings.ReadString(loginUser, "AWS-VideoBucket", ""));
            ts.CreatedDate = DateTime.Now;
            ts.CreatorID = loginUser.UserID;
            ts.OrganizationID = loginUser.OrganizationID;
            ts.ArchiveID = archive.Id.ToString();
            ts.Collection.Save();

            return string.Format("https://s3.amazonaws.com/{2}/{0}/{1}/archive.mp4", SystemSettings.GetTokApiKey(), archive.Id.ToString(), SystemSettings.ReadString(loginUser, "AWS-VideoBucket", ""));
        }

        [WebMethod]
        public string StopArchivingClient(string archiveId)
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            System.Threading.Thread.Sleep(1500);
            var archive = OpenTok.StopArchive(archiveId);
            do
            {

            }
            while (OpenTok.GetArchive(archiveId).Status != ArchiveStatus.UPLOADED);

            TokStorageItem ts = (new TokStorage(TSAuthentication.GetLoginUser())).AddNewTokStorageItem();
            ts.AmazonPath = string.Format("https://s3.amazonaws.com/{2}/{0}/{1}/archive.mp4", SystemSettings.GetTokApiKey(), archive.Id, SystemSettings.ReadString(loginUser, "AWS-VideoBucket", ""));
            ts.CreatedDate = DateTime.Now;
            ts.CreatorID = loginUser.UserID;
            ts.OrganizationID = loginUser.OrganizationID;
            ts.ArchiveID = archive.Id.ToString();
            ts.Collection.Save();

            //return string.Format("https://s3.amazonaws.com/{2}/{0}/{1}/archive.mp4", SystemSettings.GetTokApiKey(), archive.Id.ToString(), SystemSettings.ReadString(loginUser, "AWS-VideoBucket", ""));
            return JsonConvert.SerializeObject(string.Format("https://s3.amazonaws.com/{2}/{0}/{1}/archive.mp4", SystemSettings.GetTokApiKey(), archive.Id.ToString(), SystemSettings.ReadString(loginUser, "AWS-VideoBucket", "")));
        }

        [WebMethod]
        public bool DeleteArchive(string archiveId)
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            OpenTok.DeleteArchive(archiveId);

            TokStorage ts = new TokStorage(loginUser);
            ts.LoadByArchiveID(archiveId.ToString());
            ts[0].Delete();
            ts[0].Collection.Save();

            return true;
        }

        [WebMethod]
        public string DeleteArchiveClient(string archiveId)
        {
            var OpenTok = new OpenTok(Int32.Parse(SystemSettings.GetTokApiKey()), SystemSettings.GetTokApiSecret());
            OpenTok.DeleteArchive(archiveId);

            TokStorage ts = new TokStorage(loginUser);
            ts.LoadByArchiveID(archiveId.ToString());
            ts[0].Delete();
            ts[0].Collection.Save();

            //return true;
            return JsonConvert.SerializeObject(true);
        }

        #endregion

        #region PrivateMethods

        private Organization GetOrganization(string orgGuid)
        {
            Organizations organizations = new Organizations(LoginUser.Anonymous);
            organizations.LoadByChatID(new Guid(orgGuid));

            if (organizations.IsEmpty) return null;
            else return organizations[0];
        }

        private Chat GetChat(int chatID)
        {
            Chats chats = new Chats(loginUser);
            chats.LoadByChatID(chatID);

            if (!chats.IsEmpty) return chats[0];
            else return null;
        }

        //private ChatClientProxy GetParticipant(int participantID)
        //{
        //    ChatClient client = ChatClients.GetChatClient(loginUser, participantID);
        //    return (client == null) ? null : client.GetProxy();
        //}

        private ParticipantInfoView GetParticipant(int participantID, int chatID)
        {
            ChatClient client = ChatClients.GetChatClient(loginUser, participantID);
            if (client != null)
            {
                return new ParticipantInfoView(client.LinkedUserID, client.FirstName, client.LastName, client.Email, client.CompanyName);
            }
            else
            {
                UsersViewItem user = UsersView.GetUsersViewItem(loginUser, participantID);

                if (user != null)
                {
                    return new ParticipantInfoView(user.UserID, user.FirstName, user.LastName, user.Email, user.Organization, user.Title);
                }

            }
            return null;
        }

        private static ParticipantInfoView GetLinkedUserInfo(int participantID, ChatParticipantType type)
        {
            ParticipantInfoView result = null;
            switch (type)
            {
                case ChatParticipantType.User:
                    UsersViewItem user = UsersView.GetUsersViewItem(TSAuthentication.GetLoginUser(), participantID);
                    if (user != null) result = new ParticipantInfoView(user.UserID, user.FirstName, user.LastName, user.Email, user.Organization, user.Title);
                    break;
                case ChatParticipantType.External:
                    ChatClientsViewItem client = ChatClientsView.GetChatClientsViewItem(TSAuthentication.GetLoginUser(), participantID);
                    if (client != null && client.LinkedUserID != null)
                    {
                        UsersViewItem userView = UsersView.GetUsersViewItem(TSAuthentication.GetLoginUser(), (int)client.LinkedUserID);
                        result = new ParticipantInfoView(userView.UserID, client.FirstName, client.LastName, client.Email, client.CompanyName);
                    }
                    else if (client != null) result = new ParticipantInfoView(null, client.FirstName, client.LastName, client.Email, client.CompanyName);
                    break;
                default:
                    break;
            }
            return result;
        }

        private ChatRequestProxy GetChatRequest(int chatID)
        {
            ChatRequests requests = new ChatRequests(loginUser);
            requests.LoadByChatID(chatID, ChatRequestType.External);
            return (requests.IsEmpty) ? null : requests[0].GetProxy();
        }

        private ChatMessageProxy[] GetChatMessages(int chatID)
        {
            ChatMessages messages = new ChatMessages(loginUser);
            messages.LoadAllByChatID(chatID);
            return (messages.IsEmpty) ? null : messages.GetChatMessageProxies();
        }

        private string GetSuggestedSolutionDefaultInput(int chatID)
        {
            ChatMessageProxy[] messages = GetChatMessages(chatID);

            StringBuilder result = new StringBuilder();
            foreach (ChatMessageProxy message in messages)
            {
                if (!message.IsNotification)
                    result.AppendLine(HtmlUtility.StripHTML2(message.Message));
            }
            return result.ToString();
        }

        #endregion

        #region Classes
        public class ChatViewObject
        {
            public int ChatID { get; set; }
            public int ChatRequestID { get; set; }
            public int OrganizationID { get; set; }
            public string CompanyName { get; set; }
            public int? InitiatorUserID { get; set; }
            public DateTime DateCreated { get; set; }
            public string InitiatorMessage { get; set; }
            public string InitiatorDisplayName { get; set; }
            public string InitiatorEmail { get; set; }
            public string InitiatorInitials { get; set; }
            public string Description { get; set; }
            public int GroupID { get; set; }
            public List<ChatViewMessage> Messages { get; set; }

            public ChatViewObject()
            {

            }

            public ChatViewObject(ChatRequestProxy request, ParticipantInfoView initiator, ChatMessageProxy[] messages, int groupID = 0)
            {
                ChatID = request.ChatID;
                ChatRequestID = request.ChatRequestID;
                OrganizationID = request.OrganizationID;
                InitiatorUserID = initiator.UserID;
                DateCreated = request.DateCreated;
                InitiatorMessage = (initiator.Title != null)
                ?   string.Format("{0} {1} - {2}, {3} ({4})", initiator.FirstName, initiator.LastName, initiator.Title, initiator.CompanyName, initiator.Email)
                :   string.Format("{0} {1}, {2} ({3})", initiator.FirstName, initiator.LastName, initiator.CompanyName, initiator.Email);

                InitiatorDisplayName = string.Format("{0} {1}", initiator.FirstName, initiator.LastName);
                InitiatorInitials = string.Format("{0}{1}", (!string.IsNullOrEmpty(initiator.FirstName)) ? initiator.FirstName.Substring(0, 1).ToUpper() : "", (!string.IsNullOrEmpty(initiator.LastName)) ? initiator.LastName.Substring(0, 1).ToUpper() : "");
                InitiatorEmail = initiator.Email;
                Description = request.Message;
                CompanyName = initiator.CompanyName;
                Messages = new List<ChatViewMessage>();
                if (messages != null)
                {
                    foreach (ChatMessageProxy message in messages)
                    {
                        Messages.Add(new ChatViewMessage(message, GetLinkedUserInfo(message.PosterID, message.PosterType)));
                    }
                }

                GroupID = groupID;
            }
        }

        public class ChatViewMessage
        {
            public int ChatID { get; set; }
            public int MessageID { get; set; }
            public int? CreatorID { get; set; }
            public ChatParticipantType CreatorType { get; set; }
            public string CreatorDisplayName { get; set; }
            public string CreatorInitials { get; set; }
            public DateTime DateCreated { get; set; }
            public string Message { get; set; }
            public bool IsNotification { get; set; }
            public bool HasLeft { get; set; }

            public ChatViewMessage()
            {

            }
            public ChatViewMessage(ChatMessageProxy message, ParticipantInfoView userInfo, bool hasLeft = false)
            {
                MessageID = message.ChatMessageID;
                DateCreated = message.DateCreated;
                CreatorID = userInfo.UserID;
                CreatorType = message.PosterType;
                CreatorDisplayName = string.Format("{0} {1}", userInfo.FirstName, userInfo.LastName);
                CreatorInitials = string.Format("{0}{1}", (!string.IsNullOrEmpty(userInfo.FirstName)) ? userInfo.FirstName.Substring(0,1).ToUpper() : "", (!string.IsNullOrEmpty(userInfo.LastName)) ? userInfo.LastName.Substring(0,1).ToUpper() : "");
                Message = message.Message;
                IsNotification = message.IsNotification;
                ChatID = message.ChatID;
                HasLeft = hasLeft;
            }
        }

        public class ParticipantInfoView
        {
            public int? UserID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string CompanyName { get; set; }
            public string Title { get; set; }
            public string PhoneNumber { get; set; }

            public ParticipantInfoView()
            {

            }
            public ParticipantInfoView(int? userID, string firstName, string lastName, string email, string companyName, string title = null, string phoneNumber = null)
            {
                UserID = userID;
                FirstName = firstName;
                LastName = lastName;
                Email = email;
                CompanyName = companyName;
                Title = title;
                PhoneNumber = phoneNumber;
            }
        }

        public class ChatSettingsModel
        {
            public string ChatIntro { get; set; }
            public bool TOKScreenEnabled { get; set; }
            public bool TOKVoiceEnabled { get; set; }
            public bool TOKVideoEnabled { get; set; }
            public bool ChatAvatarsEnabled { get; set; }
            public int OrganizationID { get; set; }
        }
        #endregion
    }
}
