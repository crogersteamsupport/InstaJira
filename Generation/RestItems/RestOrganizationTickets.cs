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
  
  public class RestOrganizationTickets
  {
    public static string GetOrganizationTicket(RestCommand command, int ticketID)
    {
      OrganizationTicket organizationTicket = OrganizationTickets.GetOrganizationTicket(command.LoginUser, ticketID);
      if (organizationTicket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return organizationTicket.GetXml("OrganizationTicket", true);
    }
    
    public static string GetOrganizationTickets(RestCommand command)
    {
      OrganizationTickets organizationTickets = new OrganizationTickets(command.LoginUser);
      organizationTickets.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return organizationTickets.GetXml("OrganizationTickets", "OrganizationTicket", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
