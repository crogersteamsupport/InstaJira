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
  
  public class RestUserTicketStatuses
  {
    public static string GetUserTicketStatus(RestCommand command, int userTicketStatusID)
    {
      UserTicketStatus userTicketStatus = UserTicketStatuses.GetUserTicketStatus(command.LoginUser, userTicketStatusID);
      if (userTicketStatus.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return userTicketStatus.GetXml("UserTicketStatus", true);
    }
    
    public static string GetUserTicketStatuses(RestCommand command)
    {
      UserTicketStatuses userTicketStatuses = new UserTicketStatuses(command.LoginUser);
      userTicketStatuses.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return userTicketStatuses.GetXml("UserTicketStatuses", "UserTicketStatus", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
