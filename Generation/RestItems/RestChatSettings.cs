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
  
  public class RestChatSettings
  {
    public static string GetChatSetting(RestCommand command, int organizationID)
    {
      ChatSetting chatSetting = ChatSettings.GetChatSetting(command.LoginUser, organizationID);
      if (chatSetting.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return chatSetting.GetXml("ChatSetting", true);
    }
    
    public static string GetChatSettings(RestCommand command)
    {
      ChatSettings chatSettings = new ChatSettings(command.LoginUser);
      chatSettings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return chatSettings.GetXml("ChatSettings", "ChatSetting", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
