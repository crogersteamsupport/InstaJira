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
  
  public class RestChats
  {
    public static string GetChat(RestCommand command, int chatID)
    {
      Chat chat = Chats.GetChat(command.LoginUser, chatID);
      if (chat.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return chat.GetXml("Chat", true);
    }
    
    public static string GetChats(RestCommand command)
    {
      Chats chats = new Chats(command.LoginUser);
      chats.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return chats.GetXml("Chats", "Chat", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
