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
  
  public class RestOrganizationSettings
  {
    public static string GetOrganizationSetting(RestCommand command, int organizationSettingID)
    {
      OrganizationSetting organizationSetting = OrganizationSettings.GetOrganizationSetting(command.LoginUser, organizationSettingID);
      if (organizationSetting.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return organizationSetting.GetXml("OrganizationSetting", true);
    }
    
    public static string GetOrganizationSettings(RestCommand command)
    {
      OrganizationSettings organizationSettings = new OrganizationSettings(command.LoginUser);
      organizationSettings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return organizationSettings.GetXml("OrganizationSettings", "OrganizationSetting", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
