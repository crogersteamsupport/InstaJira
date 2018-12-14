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
  
  public class RestForumPermissions
  {
    public static string GetForumPermission(RestCommand command, int categoryID)
    {
      ForumPermission forumPermission = ForumPermissions.GetForumPermission(command.LoginUser, categoryID);
      if (forumPermission.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return forumPermission.GetXml("ForumPermission", true);
    }
    
    public static string GetForumPermissions(RestCommand command)
    {
      ForumPermissions forumPermissions = new ForumPermissions(command.LoginUser);
      forumPermissions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return forumPermissions.GetXml("ForumPermissions", "ForumPermission", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
