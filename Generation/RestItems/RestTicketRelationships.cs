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
  
  public class RestTicketRelationships
  {
    public static string GetTicketRelationship(RestCommand command, int ticketRelationshipID)
    {
      TicketRelationship ticketRelationship = TicketRelationships.GetTicketRelationship(command.LoginUser, ticketRelationshipID);
      if (ticketRelationship.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketRelationship.GetXml("TicketRelationship", true);
    }
    
    public static string GetTicketRelationships(RestCommand command)
    {
      TicketRelationships ticketRelationships = new TicketRelationships(command.LoginUser);
      ticketRelationships.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketRelationships.GetXml("TicketRelationships", "TicketRelationship", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
