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
  
  public class RestChatRequests
  {
    public static string GetChatRequest(RestCommand command, int chatRequestID)
    {
      ChatRequest chatRequest = ChatRequests.GetChatRequest(command.LoginUser, chatRequestID);
      if (chatRequest.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return chatRequest.GetXml("ChatRequest", true);
    }
    
    public static string GetChatRequests(RestCommand command)
    {
      ChatRequests chatRequests = new ChatRequests(command.LoginUser);
      chatRequests.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return chatRequests.GetXml("ChatRequests", "ChatRequest", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
