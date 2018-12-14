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
  
  public class RestChatUserSettings
  {
    public static string GetChatUserSetting(RestCommand command, int userID)
    {
      ChatUserSetting chatUserSetting = ChatUserSettings.GetChatUserSetting(command.LoginUser, userID);
      if (chatUserSetting.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return chatUserSetting.GetXml("ChatUserSetting", true);
    }
    
    public static string GetChatUserSettings(RestCommand command)
    {
      ChatUserSettings chatUserSettings = new ChatUserSettings(command.LoginUser);
      chatUserSettings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return chatUserSettings.GetXml("ChatUserSettings", "ChatUserSetting", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
