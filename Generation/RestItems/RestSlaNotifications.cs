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
  
  public class RestSlaNotifications
  {
    public static string GetSlaNotification(RestCommand command, int ticketID)
    {
      SlaNotification slaNotification = SlaNotifications.GetSlaNotification(command.LoginUser, ticketID);
      if (slaNotification.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return slaNotification.GetXml("SlaNotification", true);
    }
    
    public static string GetSlaNotifications(RestCommand command)
    {
      SlaNotifications slaNotifications = new SlaNotifications(command.LoginUser);
      slaNotifications.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return slaNotifications.GetXml("SlaNotifications", "SlaNotification", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
