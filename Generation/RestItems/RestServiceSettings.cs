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
  
  public class RestServiceSettings
  {
    public static string GetServiceSetting(RestCommand command, int serviceSettingID)
    {
      ServiceSetting serviceSetting = ServiceSettings.GetServiceSetting(command.LoginUser, serviceSettingID);
      if (serviceSetting.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return serviceSetting.GetXml("ServiceSetting", true);
    }
    
    public static string GetServiceSettings(RestCommand command)
    {
      ServiceSettings serviceSettings = new ServiceSettings(command.LoginUser);
      serviceSettings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return serviceSettings.GetXml("ServiceSettings", "ServiceSetting", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
