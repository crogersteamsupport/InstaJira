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
  
  public class RestTicketsView
  {
    public static string GetTicketsViewItem(RestCommand command, int ticketID)
    {
      TicketsViewItem ticketsViewItem = TicketsView.GetTicketsViewItem(command.LoginUser, ticketID);
      if (ticketsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketsViewItem.GetXml("TicketsViewItem", true);
    }
    
    public static string GetTicketsView(RestCommand command)
    {
      TicketsView ticketsView = new TicketsView(command.LoginUser);
      ticketsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketsView.GetXml("TicketsView", "TicketsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
