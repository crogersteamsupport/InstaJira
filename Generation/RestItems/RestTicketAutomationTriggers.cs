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
  
  public class RestTicketAutomationTriggers
  {
    public static string GetTicketAutomationTrigger(RestCommand command, int triggerID)
    {
      TicketAutomationTrigger ticketAutomationTrigger = TicketAutomationTriggers.GetTicketAutomationTrigger(command.LoginUser, triggerID);
      if (ticketAutomationTrigger.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketAutomationTrigger.GetXml("TicketAutomationTrigger", true);
    }
    
    public static string GetTicketAutomationTriggers(RestCommand command)
    {
      TicketAutomationTriggers ticketAutomationTriggers = new TicketAutomationTriggers(command.LoginUser);
      ticketAutomationTriggers.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketAutomationTriggers.GetXml("TicketAutomationTriggers", "TicketAutomationTrigger", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
