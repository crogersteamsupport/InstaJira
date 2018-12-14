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
  
  public class RestUserTickets
  {
    public static string GetUserTicket(RestCommand command, int ticketID)
    {
      UserTicket userTicket = UserTickets.GetUserTicket(command.LoginUser, ticketID);
      if (userTicket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return userTicket.GetXml("UserTicket", true);
    }
    
    public static string GetUserTickets(RestCommand command)
    {
      UserTickets userTickets = new UserTickets(command.LoginUser);
      userTickets.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return userTickets.GetXml("UserTickets", "UserTicket", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
