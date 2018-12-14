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
  
  public class RestChatParticipants
  {
    public static string GetChatParticipant(RestCommand command, int chatParticipantID)
    {
      ChatParticipant chatParticipant = ChatParticipants.GetChatParticipant(command.LoginUser, chatParticipantID);
      if (chatParticipant.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return chatParticipant.GetXml("ChatParticipant", true);
    }
    
    public static string GetChatParticipants(RestCommand command)
    {
      ChatParticipants chatParticipants = new ChatParticipants(command.LoginUser);
      chatParticipants.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return chatParticipants.GetXml("ChatParticipants", "ChatParticipant", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
