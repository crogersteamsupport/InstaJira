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
  
  public class RestTicketSlaView
  {
    public static string GetTicketSlaViewItem(RestCommand command, int ticketID)
    {
      TicketSlaViewItem ticketSlaViewItem = TicketSlaView.GetTicketSlaViewItem(command.LoginUser, ticketID);
      if (ticketSlaViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketSlaViewItem.GetXml("TicketSlaViewItem", true);
    }
    
    public static string GetTicketSlaView(RestCommand command)
    {
      TicketSlaView ticketSlaView = new TicketSlaView(command.LoginUser);
      ticketSlaView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketSlaView.GetXml("TicketSlaView", "TicketSlaViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
