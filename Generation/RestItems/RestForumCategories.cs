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
  
  public class RestForumCategories
  {
    public static string GetForumCategory(RestCommand command, int categoryID)
    {
      ForumCategory forumCategory = ForumCategories.GetForumCategory(command.LoginUser, categoryID);
      if (forumCategory.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return forumCategory.GetXml("ForumCategory", true);
    }
    
    public static string GetForumCategories(RestCommand command)
    {
      ForumCategories forumCategories = new ForumCategories(command.LoginUser);
      forumCategories.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return forumCategories.GetXml("ForumCategories", "ForumCategory", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
