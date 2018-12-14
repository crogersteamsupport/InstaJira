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
  
  public class RestForumTickets
  {
    public static string GetForumTicket(RestCommand command, int ticketID)
    {
      ForumTicket forumTicket = ForumTickets.GetForumTicket(command.LoginUser, ticketID);
      if (forumTicket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return forumTicket.GetXml("ForumTicket", true);
    }
    
    public static string GetForumTickets(RestCommand command)
    {
      ForumTickets forumTickets = new ForumTickets(command.LoginUser);
      forumTickets.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return forumTickets.GetXml("ForumTickets", "ForumTicket", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
