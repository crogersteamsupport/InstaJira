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
  
  public class RestTicketAutomationTriggerLogic
  {
    public static string GetTicketAutomationTriggerLogicItem(RestCommand command, int triggerLogicID)
    {
      TicketAutomationTriggerLogicItem ticketAutomationTriggerLogicItem = TicketAutomationTriggerLogic.GetTicketAutomationTriggerLogicItem(command.LoginUser, triggerLogicID);
      if (ticketAutomationTriggerLogicItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketAutomationTriggerLogicItem.GetXml("TicketAutomationTriggerLogicItem", true);
    }
    
    public static string GetTicketAutomationTriggerLogic(RestCommand command)
    {
      TicketAutomationTriggerLogic ticketAutomationTriggerLogic = new TicketAutomationTriggerLogic(command.LoginUser);
      ticketAutomationTriggerLogic.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketAutomationTriggerLogic.GetXml("TicketAutomationTriggerLogic", "TicketAutomationTriggerLogicItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
