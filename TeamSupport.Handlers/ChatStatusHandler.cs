using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using System.Net;
using System.Web.SessionState;
using System.Drawing;
using TeamSupport.Data;
using TeamSupport.WebUtils;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;

namespace TeamSupport.Handlers
{
  public class ChatStatusHandler : IHttpHandler
  {
    public bool IsReusable
    {
      get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
      context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      context.Response.AddHeader("Expires", "-1");
      context.Response.AddHeader("Pragma", "no-cache");

      if (TSAuthentication.Ticket == null)
      {
        context.Response.ContentType = "text/plain";
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        context.Response.ClearContent();
        context.Response.End();
        return;
      }

      TsChatUpdate update = new TsChatUpdate();

      using (Stream receiveStream = context.Request.InputStream)
      {
        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
        {
          string requestContent = readStream.ReadToEnd();
          JObject args = JObject.Parse(requestContent);

          int lastChatRequestID = int.Parse(args["lastChatRequestID"].ToString()); ;
          int lastChatMessageID = int.Parse(args["lastChatMessageID"].ToString()); ;

          try
          {
            User user = Users.GetUser(LoginUser.Anonymous, TSAuthentication.UserID);

            if (user == null)
            {
              context.Response.ContentType = "text/plain";
              context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
              context.Response.ClearContent();
              context.Response.End();
              return;
            }

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
        }
      }
      context.Response.ContentType = "application/json; charset=utf-8";
      context.Response.Write(JsonConvert.SerializeObject(update));
    }
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
}
