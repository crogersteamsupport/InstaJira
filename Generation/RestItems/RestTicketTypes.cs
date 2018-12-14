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
  
  public class RestTicketTypes
  {
    public static string GetTicketType(RestCommand command, int ticketTypeID)
    {
      TicketType ticketType = TicketTypes.GetTicketType(command.LoginUser, ticketTypeID);
      if (ticketType.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketType.GetXml("TicketType", true);
    }
    
    public static string GetTicketTypes(RestCommand command)
    {
      TicketTypes ticketTypes = new TicketTypes(command.LoginUser);
      ticketTypes.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketTypes.GetXml("TicketTypes", "TicketType", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
