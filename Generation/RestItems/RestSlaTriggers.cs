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
  
  public class RestSlaTriggers
  {
    public static string GetSlaTrigger(RestCommand command, int slaTriggerID)
    {
      SlaTrigger slaTrigger = SlaTriggers.GetSlaTrigger(command.LoginUser, slaTriggerID);
      if (slaTrigger.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return slaTrigger.GetXml("SlaTrigger", true);
    }
    
    public static string GetSlaTriggers(RestCommand command)
    {
      SlaTriggers slaTriggers = new SlaTriggers(command.LoginUser);
      slaTriggers.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return slaTriggers.GetXml("SlaTriggers", "SlaTrigger", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
