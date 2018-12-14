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
  
  public class RestTicketAutomationHistory
  {
    public static string GetTicketAutomationHistoryItem(RestCommand command, int historyID)
    {
      TicketAutomationHistoryItem ticketAutomationHistoryItem = TicketAutomationHistory.GetTicketAutomationHistoryItem(command.LoginUser, historyID);
      if (ticketAutomationHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketAutomationHistoryItem.GetXml("TicketAutomationHistoryItem", true);
    }
    
    public static string GetTicketAutomationHistory(RestCommand command)
    {
      TicketAutomationHistory ticketAutomationHistory = new TicketAutomationHistory(command.LoginUser);
      ticketAutomationHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketAutomationHistory.GetXml("TicketAutomationHistory", "TicketAutomationHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
