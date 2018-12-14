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
  
  public class RestUsers
  {
    public static string GetUser(RestCommand command, int userID)
    {
      User user = Users.GetUser(command.LoginUser, userID);
      if (user.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return user.GetXml("User", true);
    }
    
    public static string GetUsers(RestCommand command)
    {
      Users users = new Users(command.LoginUser);
      users.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return users.GetXml("Users", "User", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
