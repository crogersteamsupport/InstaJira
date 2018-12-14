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
  
  public class RestLoginAttempts
  {
    public static string GetLoginAttempt(RestCommand command, int loginAttemptID)
    {
      LoginAttempt loginAttempt = LoginAttempts.GetLoginAttempt(command.LoginUser, loginAttemptID);
      if (loginAttempt.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return loginAttempt.GetXml("LoginAttempt", true);
    }
    
    public static string GetLoginAttempts(RestCommand command)
    {
      LoginAttempts loginAttempts = new LoginAttempts(command.LoginUser);
      loginAttempts.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return loginAttempts.GetXml("LoginAttempts", "LoginAttempt", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
