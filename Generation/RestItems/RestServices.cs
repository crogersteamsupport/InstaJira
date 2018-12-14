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
  
  public class RestServices
  {
    public static string GetService(RestCommand command, int serviceID)
    {
      Service service = Services.GetService(command.LoginUser, serviceID);
      if (service.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return service.GetXml("Service", true);
    }
    
    public static string GetServices(RestCommand command)
    {
      Services services = new Services(command.LoginUser);
      services.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return services.GetXml("Services", "Service", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
