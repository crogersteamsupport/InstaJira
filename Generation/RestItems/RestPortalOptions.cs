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
  
  public class RestPortalOptions
  {
    public static string GetPortalOption(RestCommand command, int organizationID)
    {
      PortalOption portalOption = PortalOptions.GetPortalOption(command.LoginUser, organizationID);
      if (portalOption.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return portalOption.GetXml("PortalOption", true);
    }
    
    public static string GetPortalOptions(RestCommand command)
    {
      PortalOptions portalOptions = new PortalOptions(command.LoginUser);
      portalOptions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return portalOptions.GetXml("PortalOptions", "PortalOption", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
