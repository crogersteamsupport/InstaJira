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
  
  public class RestTicketNextStatuses
  {
    public static string GetTicketNextStatus(RestCommand command, int ticketNextStatusID)
    {
      TicketNextStatus ticketNextStatus = TicketNextStatuses.GetTicketNextStatus(command.LoginUser, ticketNextStatusID);
      if (ticketNextStatus.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketNextStatus.GetXml("TicketNextStatus", true);
    }
    
    public static string GetTicketNextStatuses(RestCommand command)
    {
      TicketNextStatuses ticketNextStatuses = new TicketNextStatuses(command.LoginUser);
      ticketNextStatuses.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketNextStatuses.GetXml("TicketNextStatuses", "TicketNextStatus", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
