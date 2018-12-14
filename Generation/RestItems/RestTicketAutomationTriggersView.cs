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
  
  public class RestTicketAutomationTriggersView
  {
    public static string GetTicketAutomationTriggersViewItem(RestCommand command, int triggerID)
    {
      TicketAutomationTriggersViewItem ticketAutomationTriggersViewItem = TicketAutomationTriggersView.GetTicketAutomationTriggersViewItem(command.LoginUser, triggerID);
      if (ticketAutomationTriggersViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketAutomationTriggersViewItem.GetXml("TicketAutomationTriggersViewItem", true);
    }
    
    public static string GetTicketAutomationTriggersView(RestCommand command)
    {
      TicketAutomationTriggersView ticketAutomationTriggersView = new TicketAutomationTriggersView(command.LoginUser);
      ticketAutomationTriggersView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketAutomationTriggersView.GetXml("TicketAutomationTriggersView", "TicketAutomationTriggersViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
