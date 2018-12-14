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
  
  public class RestBackdoorLogins
  {
    public static string GetBackdoorLogin(RestCommand command, int backdoorLoginID)
    {
      BackdoorLogin backdoorLogin = BackdoorLogins.GetBackdoorLogin(command.LoginUser, backdoorLoginID);
      if (backdoorLogin.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return backdoorLogin.GetXml("BackdoorLogin", true);
    }
    
    public static string GetBackdoorLogins(RestCommand command)
    {
      BackdoorLogins backdoorLogins = new BackdoorLogins(command.LoginUser);
      backdoorLogins.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return backdoorLogins.GetXml("BackdoorLogins", "BackdoorLogin", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
