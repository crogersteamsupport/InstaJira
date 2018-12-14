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
  
  public class RestOrganizations
  {
    public static string GetOrganization(RestCommand command, int organizationID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return organization.GetXml("Organization", true);
    }
    
    public static string GetOrganizations(RestCommand command)
    {
      Organizations organizations = new Organizations(command.LoginUser);
      organizations.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return organizations.GetXml("Organizations", "Organization", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
