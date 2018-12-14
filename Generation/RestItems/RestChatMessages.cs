using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;

namespace TeamSupport.Api
{
  
  public class RestChatMessages
  {
    public static string GetChatMessage(RestCommand command, int chatMessageID)
    {
      ChatMessage chatMessage = ChatMessages.GetChatMessage(command.LoginUser, chatMessageID);
      if (chatMessage.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return chatMessage.GetXml("ChatMessage", true);
    }
    
    public static string GetChatMessages(RestCommand command)
    {
      ChatMessages chatMessages = new ChatMessages(command.LoginUser);
      chatMessages.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return chatMessages.GetXml("ChatMessages", "ChatMessage", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
