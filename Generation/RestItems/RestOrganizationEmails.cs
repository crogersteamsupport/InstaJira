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
  
  public class RestOrganizationEmails
  {
    public static string GetOrganizationEmail(RestCommand command, int organizationEmailID)
    {
      OrganizationEmail organizationEmail = OrganizationEmails.GetOrganizationEmail(command.LoginUser, organizationEmailID);
      if (organizationEmail.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return organizationEmail.GetXml("OrganizationEmail", true);
    }
    
    public static string GetOrganizationEmails(RestCommand command)
    {
      OrganizationEmails organizationEmails = new OrganizationEmails(command.LoginUser);
      organizationEmails.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return organizationEmails.GetXml("OrganizationEmails", "OrganizationEmail", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
