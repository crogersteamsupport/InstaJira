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
  
  public class RestChatClients
  {
    public static string GetChatClient(RestCommand command, int chatClientID)
    {
      ChatClient chatClient = ChatClients.GetChatClient(command.LoginUser, chatClientID);
      if (chatClient.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return chatClient.GetXml("ChatClient", true);
    }
    
    public static string GetChatClients(RestCommand command)
    {
      ChatClients chatClients = new ChatClients(command.LoginUser);
      chatClients.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return chatClients.GetXml("ChatClients", "ChatClient", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
