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
  
  public class RestTicketTemplates
  {
    public static string GetTicketTemplate(RestCommand command, int ticketTemplateID)
    {
      TicketTemplate ticketTemplate = TicketTemplates.GetTicketTemplate(command.LoginUser, ticketTemplateID);
      if (ticketTemplate.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return ticketTemplate.GetXml("TicketTemplate", true);
    }
    
    public static string GetTicketTemplates(RestCommand command)
    {
      TicketTemplates ticketTemplates = new TicketTemplates(command.LoginUser);
      ticketTemplates.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return ticketTemplates.GetXml("TicketTemplates", "TicketTemplate", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
