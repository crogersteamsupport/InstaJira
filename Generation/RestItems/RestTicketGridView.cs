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
  
  public class RestTicketGridView
  {
    public static string GetTicketGridViewItem(RestCommand command, int ticketID)
    {
      TicketGridViewItem ticketGridViewItem = TicketGridView.GetTicketGridViewItem(command.LoginUser, ticketID);
      if (ticketGridViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketGridViewItem.GetXml("TicketGridViewItem", true);
    }
    
    public static string GetTicketGridView(RestCommand command)
    {
      TicketGridView ticketGridView = new TicketGridView(command.LoginUser);
      ticketGridView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketGridView.GetXml("TicketGridView", "TicketGridViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
