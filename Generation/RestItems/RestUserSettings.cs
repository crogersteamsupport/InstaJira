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
  
  public class RestUserSettings
  {
    public static string GetUserSetting(RestCommand command, int userSettingID)
    {
      UserSetting userSetting = UserSettings.GetUserSetting(command.LoginUser, userSettingID);
      if (userSetting.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return userSetting.GetXml("UserSetting", true);
    }
    
    public static string GetUserSettings(RestCommand command)
    {
      UserSettings userSettings = new UserSettings(command.LoginUser);
      userSettings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return userSettings.GetXml("UserSettings", "UserSetting", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
