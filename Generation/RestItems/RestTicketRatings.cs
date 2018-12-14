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
  
  public class RestTicketRatings
  {
    public static string GetTicketRating(RestCommand command, int ticketID)
    {
      TicketRating ticketRating = TicketRatings.GetTicketRating(command.LoginUser, ticketID);
      if (ticketRating.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketRating.GetXml("TicketRating", true);
    }
    
    public static string GetTicketRatings(RestCommand command)
    {
      TicketRatings ticketRatings = new TicketRatings(command.LoginUser);
      ticketRatings.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketRatings.GetXml("TicketRatings", "TicketRating", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
