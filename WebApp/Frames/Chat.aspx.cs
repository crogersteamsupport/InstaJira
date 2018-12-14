using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.Text;
using Telerik.Web.UI;


public partial class Frames_Chat : BaseFramePage
{
  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    if (UserSession.CurrentUser.ProductType == ProductType.Express || UserSession.CurrentUser.ProductType == ProductType.BugTracking)
    {
      tbChat.FindItemByValue("opencustomer").Visible = false;
    }

    ChatUserSetting setting = ChatUserSettings.GetSetting(UserSession.LoginUser, UserSession.LoginUser.UserID);
    if (!setting.IsAvailable)
    {
      RadToolBarButton button = (RadToolBarButton)tbChat.FindItemByValue("available");
      button.ImageUrl = "../images/icons/chat_d.png";
      button.Text = "I am not available";
    }
  }

  [WebMethod]
  public static UserInfoProxy GetUserInfo()
  {
    return UserSession.CurrentUser.Proxy;
  }

  [WebMethod(false)]
  public static string GetChatRequestHtml(UserInfoProxy userInfo, bool forceUpdate)
  {
    LoginUser loginUser = new LoginUser(userInfo.UserID, userInfo.OrganizationID, null);
    StringBuilder builder = new StringBuilder();

    ChatRequests requests = new ChatRequests(loginUser);
    requests.LoadWaitingRequests(loginUser.UserID, loginUser.OrganizationID);

    foreach (ChatRequest request in requests)
    {
      ParticipantInfo participant = ChatParticipant.GetParticipantInfo(loginUser, request.RequestorID, request.RequestorType);
      string row = "<tr><td class=\"col1\">{0}</td><td class=\"col2\">{1}</td></tr>";
      string stateClass = request.RequestType != ChatRequestType.External ? "ui-state-highlight" : "ui-state-default";
      builder.Append(string.Format("<div class=\"ts-panellist-header ui-helper-reset {0} ui-corner-all\" role=\"tab\" aria-expanded=\"false\" tabindex=\"-1\">", stateClass));
      builder.Append("<span class=\"ui-icon ui-icon-triangle-1-e \" />");
      builder.Append("<a href=\"#\">" + participant.FirstName + " " + participant.LastName + " - " + request.DateCreated.ToString("t", loginUser.CultureInfo) + "</a>");
      builder.Append("</div>");
      builder.Append("<div class=\"content\">");
      builder.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"5\">");
      builder.Append(string.Format(row, "Email:", participant.Email));
      if (participant.CompanyName != "")
        builder.Append(string.Format(row, "Company:", participant.CompanyName));
      builder.Append(string.Format(row, "Time:", request.DateCreated.ToString("t", loginUser.CultureInfo)));
      builder.Append(string.Format(row, "Message:", request.Message));
      builder.Append("</table>");
      builder.Append(string.Format("<div class=\"accept\"><a class=\"ts-link-button {0} ui-corner-all \" href=\"#\" onclick=\"AcceptRequest({1}); return false;\">Accept</a></div>", stateClass, request.ChatRequestID.ToString()));
      builder.Append("</div>");
    }
    string html = builder.ToString().Trim();

    if (html == "")
    {
      html = "<div class=\"content\">There are no chat requests.</div>";
    }


    if (!forceUpdate && HttpContext.Current.Session["LastRequestHtml"] != null && (string)HttpContext.Current.Session["LastRequestHtml"] == html)
    {
      return "";
    }

    HttpContext.Current.Session["LastRequestHtml"] = html;
    return html;
  
  }

  [WebMethod(false)]
  public static string[] GetActiveChatsHtml(UserInfoProxy userInfo, bool forceUpdate, int activeChatID)
  {
    LoginUser loginUser = new LoginUser(userInfo.UserID, userInfo.OrganizationID, null);
    Chats.KickOutDisconnectedClients(loginUser, loginUser.OrganizationID);
    StringBuilder builder = new StringBuilder();

    ChatParticipants participants = new ChatParticipants(loginUser);
    participants.LoadByUserID(loginUser.UserID, loginUser.OrganizationID);

    if (!participants.IsEmpty)
    {
      int chatID = -1;

      StringBuilder names = new StringBuilder();
      foreach (ChatParticipant item in participants)
      {
        if (item.ParticipantType != ChatParticipantType.External) continue;

        bool isNew = (int)item.Row["LastPostedMessageID"] != (int)item.Row["MyLastMessageID"];
        string state = isNew ? "ui-state-highlight" : "chat-state-normal";
        state = activeChatID == item.ChatID ? "ui-state-default" : state;
          
        if (chatID != item.ChatID)
        {
          if (names.Length > 0) names.Append("</div>");
          names.Append(string.Format("<div id=\"divChat{0}\" class=\"chat ui-corner-all ts-icon-container ui-widget ui-helper-reset {1} \">", item.ChatID.ToString(), state));
        }
        string name = item.FirstName + " " + item.LastName;
        if (!string.IsNullOrEmpty(item.CompanyName))
        {
          name = name + " - " + item.CompanyName;
        }
        
        names.Append(string.Format("<span class=\"ts-icon {1}\"/><span class=\"ts-icon-text\">{0}</span>", name, item.IsOnline ? "ts-icon-online" : "ts-icon-offline"));
        chatID = item.ChatID;
      }
      names.Append("</div>");
      builder.Append(names.ToString());
    }

    string html = builder.ToString().Trim();
    if (html == "") html = "<div class=\"chat\">There are no active chats.</div>";
    if (!forceUpdate && HttpContext.Current.Session["LastChatHtml"] != null && (string)HttpContext.Current.Session["LastChatHtml"] == html) return null;
    HttpContext.Current.Session["LastChatHtml"] = html;
    return new string[] {html, activeChatID.ToString()};

  }

  [WebMethod(false)]
  public static string[] GetChatHtml(UserInfoProxy userInfo, int chatID, int lastMessageID)
  {
    LoginUser loginUser = new LoginUser(userInfo.UserID, userInfo.OrganizationID, null);
    ChatUserSetting setting = ChatUserSettings.GetSetting(loginUser, loginUser.UserID);
    setting.CurrentChatID = chatID;
    setting.Collection.Save();

    Chat chat = Chats.GetChat(loginUser, chatID);
    if (chat == null || chat.OrganizationID != loginUser.OrganizationID) return null;

    ChatMessages messages = new ChatMessages(loginUser);
    int i = messages.GetLastMessageID(chatID);
    string chatHtml = "";

    if (i > -1 && i != lastMessageID)
    {
      ChatParticipants.UpdateLastMessageID(loginUser, loginUser.UserID, ChatParticipantType.User, chatID, i);
      chatHtml = chat.GetHtml(true, loginUser.CultureInfo);
    }

    
    // load typers
    ChatParticipants participants = new ChatParticipants(loginUser);
    participants.LoadTypers(loginUser.UserID, ChatParticipantType.User, chatID, 2);
    StringBuilder typers = new StringBuilder();
    foreach (ChatParticipant item in participants)
    {
      if (typers.Length > 0) typers.Append(", ");
      typers.Append(item.FirstName);
    }

    if (typers.Length > 0) typers.Append(" is typing a message.");
    
    return new string[] { chatID.ToString(), i.ToString(), chatHtml, typers.ToString() };
  }

  [WebMethod]
  public static int AcceptRequest(int chatRequestID)
  {
    int chatID = ChatRequests.AcceptRequest(UserSession.LoginUser, UserSession.LoginUser.UserID, chatRequestID, HttpContext.Current.Request.UserHostAddress);
    return chatID;
  }

  [WebMethod]
  public static int[] GetActionID(int chatID)
  {
    List<int> result = new List<int>();
    result.Add(chatID);
    Chat chat = Chats.GetChat(UserSession.LoginUser, chatID);
    if (chat == null) return null;
    result.Add(chat == null || chat.ActionID == null ? -1 : (int)chat.ActionID);

    int? userID = null;
    if (chat.InitiatorType == ChatParticipantType.External)
    {
      ChatClients clients = new ChatClients(UserSession.LoginUser);
      clients.LoadByChatClientID(chat.InitiatorID);
      if (!clients.IsEmpty) userID = clients[0].LinkedUserID;
    }

    result.Add(chat.GetInitiatorLinkedUserID());

    return result.ToArray();
  }
  
  [WebMethod]
  public static int GetTicketID(int chatID)
  {
    Chat chat = Chats.GetChat(UserSession.LoginUser, chatID);
    if (chat.ActionID == null) return -1;

    TeamSupport.Data.Action action = Actions.GetAction(UserSession.LoginUser, (int)chat.ActionID);
    if (action == null) return -1;
    return action.TicketID;
  }

  [WebMethod]
  public static int AddTicket(int chatID, int ticketID)
  {
    Chat chat = Chats.GetChat(UserSession.LoginUser, chatID);
    if (chat != null)
    {
      Actions actions = new Actions(UserSession.LoginUser);
      TeamSupport.Data.Action chatAction = actions.AddNewAction();
      chatAction.ActionTypeID = null;
      chatAction.Name = "Chat";
      chatAction.ActionSource = "Chat";
      chatAction.SystemActionTypeID = SystemActionType.Chat;
      chatAction.Description = chat.GetHtml(true, UserSession.LoginUser.OrganizationCulture);
      chatAction.IsVisibleOnPortal = false;
      chatAction.IsKnowledgeBase = false;
      chatAction.TicketID = ticketID;
      actions.Save();
      chat.ActionID = chatAction.ActionID;
      chat.Collection.Save();

      (new Tickets(UserSession.LoginUser)).AddContact(chat.GetInitiatorLinkedUserID(), ticketID);
    }
    return ticketID;
  }

  [WebMethod]
  public static void SetCurrentChatID(int chatID)
  {
    ChatUserSetting setting = ChatUserSettings.GetSetting(UserSession.LoginUser, UserSession.LoginUser.UserID);
    setting.CurrentChatID = chatID;
    setting.Collection.Save();
  }

  [WebMethod]
  public static void RequestTransfer(int chatID, int userID)
  {
    ChatRequests.RequestTransfer(UserSession.LoginUser, chatID, userID);
  }

  [WebMethod]
  public static void RequestInvite(int chatID, int userID)
  {
    ChatRequests.RequestInvite(UserSession.LoginUser, chatID, userID);
  }

  [WebMethod]
  public static void SetTyping(int chatID)
  {
    ChatParticipants.UpdateTyping(UserSession.LoginUser, UserSession.LoginUser.UserID, ChatParticipantType.User, chatID);
  }

  [WebMethod]
  public static void PostMessage(string message, int chatID)
  {
    Chat chat = Chats.GetChat(UserSession.LoginUser, chatID);
    if (chat.OrganizationID != UserSession.LoginUser.OrganizationID) return;
    ChatMessage chatMessage = (new ChatMessages(UserSession.LoginUser)).AddNewChatMessage();
    chatMessage.Message = message;
    chatMessage.ChatID = chatID;
    chatMessage.PosterID = UserSession.LoginUser.UserID;
    chatMessage.PosterType = ChatParticipantType.User;
    chatMessage.Collection.Save();
    Users.UpdateUserActivityTime(UserSession.LoginUser, UserSession.LoginUser.UserID);  
  }

  [WebMethod]
  public static bool CloseChat(int chatID)
  {
    Chat chat = Chats.GetChat(UserSession.LoginUser, chatID);
    if (chat.OrganizationID != UserSession.LoginUser.OrganizationID) return false;
    Chats.LeaveChat(UserSession.LoginUser, UserSession.LoginUser.UserID, ChatParticipantType.User, chatID);
    return true;
  }

  [WebMethod]
  public static bool ToggleAvailable()
  {
    ChatUserSetting setting = ChatUserSettings.GetSetting(UserSession.LoginUser, UserSession.LoginUser.UserID);
    setting.IsAvailable = !setting.IsAvailable;
    setting.Collection.Save();
    return setting.IsAvailable;
  }

  

}
