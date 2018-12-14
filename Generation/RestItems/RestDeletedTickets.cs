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
  
  public class RestDeletedTickets
  {
    public static string GetDeletedTicket(RestCommand command, int iD)
    {
      DeletedTicket deletedTicket = DeletedTickets.GetDeletedTicket(command.LoginUser, iD);
      if (deletedTicket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return deletedTicket.GetXml("DeletedTicket", true);
    }
    
    public static string GetDeletedTickets(RestCommand command)
    {
      DeletedTickets deletedTickets = new DeletedTickets(command.LoginUser);
      deletedTickets.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return deletedTickets.GetXml("DeletedTickets", "DeletedTicket", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
