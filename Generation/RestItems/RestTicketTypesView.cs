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
  
  public class RestTicketTypesView
  {
    public static string GetTicketTypesViewItem(RestCommand command, int ticketTypeID)
    {
      TicketTypesViewItem ticketTypesViewItem = TicketTypesView.GetTicketTypesViewItem(command.LoginUser, ticketTypeID);
      if (ticketTypesViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketTypesViewItem.GetXml("TicketTypesViewItem", true);
    }
    
    public static string GetTicketTypesView(RestCommand command)
    {
      TicketTypesView ticketTypesView = new TicketTypesView(command.LoginUser);
      ticketTypesView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketTypesView.GetXml("TicketTypesView", "TicketTypesViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
