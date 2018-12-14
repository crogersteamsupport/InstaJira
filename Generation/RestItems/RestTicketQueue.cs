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
  
  public class RestTicketQueue
  {
    public static string GetTicketQueueItem(RestCommand command, int ticketQueueID)
    {
      TicketQueueItem ticketQueueItem = TicketQueue.GetTicketQueueItem(command.LoginUser, ticketQueueID);
      if (ticketQueueItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketQueueItem.GetXml("TicketQueueItem", true);
    }
    
    public static string GetTicketQueue(RestCommand command)
    {
      TicketQueue ticketQueue = new TicketQueue(command.LoginUser);
      ticketQueue.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketQueue.GetXml("TicketQueue", "TicketQueueItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
