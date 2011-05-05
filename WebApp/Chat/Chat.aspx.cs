﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using TeamSupport.Data;
using System.Text;
using System.Globalization;

public partial class Chat_Chat : System.Web.UI.Page
{
  protected override void OnLoad(EventArgs e)
  {
    base.OnLoad(e);
    Master.FindControl("divHeaderButtons").Visible = true;
    fieldChatID.Value = Request["cid"];
    fieldRequestID.Value = Request["rid"];

  }


  [WebMethod(false)]
  public static ChatStatus GetChatStatus(int chatID, int lastMessageID, int chatRequestID)
  {
    ChatStatus result = new ChatStatus();
    result.ChatHtml = null;
    result.IsAccepted = false;
    result.Typers = "";
    result.LastMessageID = -1;
    result.ParticipantCount = 0;

    Chat chat = GetChat(chatID);
    if (chat == null) return result;

    result.IsAccepted = ChatRequests.GetChatRequest(LoginUser.Anonymous, chatRequestID).IsAccepted;
    ChatClients.UpdateClientPing(LoginUser.Anonymous, chat.InitiatorID);

    if (!result.IsAccepted) return result;


    ChatMessages messages = new ChatMessages(LoginUser.Anonymous);
    int i = messages.GetLastMessageID(chatID);

    Organization organization = Organizations.GetOrganization(LoginUser.Anonymous, chat.OrganizationID);

    string html = (i < 0 || i == lastMessageID) ? null : chat.GetHtml(false, new CultureInfo(organization.CultureName));
    ChatParticipants participants = new ChatParticipants(LoginUser.Anonymous);
    participants.LoadByChatID(chatID);

    result.LastMessageID = i;
    result.ChatHtml = html;
    result.ParticipantCount = participants.Count;

    participants = new ChatParticipants(LoginUser.Anonymous);
    participants.LoadTypers(chat.InitiatorID, ChatParticipantType.External, chat.ChatID, 2);
    StringBuilder typers = new StringBuilder();
    foreach (ChatParticipant item in participants)
    {
      if (typers.Length > 0) typers.Append(", ");
      typers.Append(item.FirstName);
    }

    if (typers.Length > 0)
    {
      typers.Append(" is typing a message.");
      result.Typers = typers.ToString();
    }

    return result;
  }

  [WebMethod(true)]
  public static void PostMessage(int chatID, string message)
  {
    Chat chat = GetChat(chatID);

    ChatMessage chatMessage = (new ChatMessages(LoginUser.Anonymous)).AddNewChatMessage();
    chatMessage.Message = message;
    chatMessage.ChatID = chatID;
    chatMessage.PosterID = chat.InitiatorID;
    chatMessage.PosterType = ChatParticipantType.External;
    chatMessage.Collection.Save();

  }

  [WebMethod(true)]
  public static void SetTyping(int chatID)
  {
    Chat chat = GetChat(chatID);
    ChatParticipants.UpdateTyping(LoginUser.Anonymous, chat.InitiatorID, ChatParticipantType.External, chat.ChatID);
  }

  public static Chat GetChat(int chatID)
  {
    Chat chat = Chats.GetChat(LoginUser.Anonymous, chatID);
    if (chat != null)
    {
      ChatParticipant cp = ChatParticipants.GetChatParticipant(LoginUser.Anonymous, chat.InitiatorID, ChatParticipantType.External, chatID);
      if (cp == null || cp.DateLeft != null) return null;
    }
    return chat;
  }

  [Serializable]
  public class ChatStatus
  {
    public bool IsAccepted { get; set; }
    public string Typers { get; set; }
    public string ChatHtml { get; set; }
    public int LastMessageID { get; set; }
    public int ParticipantCount { get; set; }
  }

}
