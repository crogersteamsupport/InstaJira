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
  
  public class RestTicketAutomationPossibleActions
  {
    public static string GetTicketAutomationPossibleAction(RestCommand command, int actionID)
    {
      TicketAutomationPossibleAction ticketAutomationPossibleAction = TicketAutomationPossibleActions.GetTicketAutomationPossibleAction(command.LoginUser, actionID);
      if (ticketAutomationPossibleAction.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketAutomationPossibleAction.GetXml("TicketAutomationPossibleAction", true);
    }
    
    public static string GetTicketAutomationPossibleActions(RestCommand command)
    {
      TicketAutomationPossibleActions ticketAutomationPossibleActions = new TicketAutomationPossibleActions(command.LoginUser);
      ticketAutomationPossibleActions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketAutomationPossibleActions.GetXml("TicketAutomationPossibleActions", "TicketAutomationPossibleAction", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
