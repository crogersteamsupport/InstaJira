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
  
  public class RestTicketAutomationActions
  {
    public static string GetTicketAutomationAction(RestCommand command, int ticketActionID)
    {
      TicketAutomationAction ticketAutomationAction = TicketAutomationActions.GetTicketAutomationAction(command.LoginUser, ticketActionID);
      if (ticketAutomationAction.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketAutomationAction.GetXml("TicketAutomationAction", true);
    }
    
    public static string GetTicketAutomationActions(RestCommand command)
    {
      TicketAutomationActions ticketAutomationActions = new TicketAutomationActions(command.LoginUser);
      ticketAutomationActions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketAutomationActions.GetXml("TicketAutomationActions", "TicketAutomationAction", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
