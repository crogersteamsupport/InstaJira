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
  
  public class RestTicketLinkToTFS
  {
    public static string GetTicketLinkToTFSItem(RestCommand command, int id)
    {
      TicketLinkToTFSItem ticketLinkToTFSItem = TicketLinkToTFS.GetTicketLinkToTFSItem(command.LoginUser, id);
      if (ticketLinkToTFSItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketLinkToTFSItem.GetXml("TicketLinkToTFSItem", true);
    }
    
    public static string GetTicketLinkToTFS(RestCommand command)
    {
      TicketLinkToTFS ticketLinkToTFS = new TicketLinkToTFS(command.LoginUser);
      ticketLinkToTFS.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketLinkToTFS.GetXml("TicketLinkToTFS", "TicketLinkToTFSItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
