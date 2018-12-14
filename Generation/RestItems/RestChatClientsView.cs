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
  
  public class RestChatClientsView
  {
    public static string GetChatClientsViewItem(RestCommand command, int chatClientID)
    {
      ChatClientsViewItem chatClientsViewItem = ChatClientsView.GetChatClientsViewItem(command.LoginUser, chatClientID);
      if (chatClientsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return chatClientsViewItem.GetXml("ChatClientsViewItem", true);
    }
    
    public static string GetChatClientsView(RestCommand command)
    {
      ChatClientsView chatClientsView = new ChatClientsView(command.LoginUser);
      chatClientsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return chatClientsView.GetXml("ChatClientsView", "ChatClientsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
