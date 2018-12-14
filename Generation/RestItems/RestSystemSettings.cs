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
  
  public class RestSystemSettings
  {
    public static string GetSystemSetting(RestCommand command, int systemSettingID)
    {
      SystemSetting systemSetting = SystemSettings.GetSystemSetting(command.LoginUser, systemSettingID);
      if (systemSetting.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return systemSetting.GetXml("SystemSetting", true);
    }
    
    public static string GetSystemSettings(RestCommand command)
    {
      SystemSettings systemSettings = new SystemSettings(command.LoginUser);
      systemSettings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return systemSettings.GetXml("SystemSettings", "SystemSetting", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
