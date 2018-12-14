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
  
  public class RestTicketSeverities
  {
    public static string GetTicketSeverity(RestCommand command, int ticketSeverityID)
    {
      TicketSeverity ticketSeverity = TicketSeverities.GetTicketSeverity(command.LoginUser, ticketSeverityID);
      if (ticketSeverity.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketSeverity.GetXml("TicketSeverity", true);
    }
    
    public static string GetTicketSeverities(RestCommand command)
    {
      TicketSeverities ticketSeverities = new TicketSeverities(command.LoginUser);
      ticketSeverities.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketSeverities.GetXml("TicketSeverities", "TicketSeverity", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
