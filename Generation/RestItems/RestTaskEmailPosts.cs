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
  
  public class RestTaskEmailPosts
  {
    public static string GetTaskEmailPost(RestCommand command, int taskEmailPostID)
    {
      TaskEmailPost taskEmailPost = TaskEmailPosts.GetTaskEmailPost(command.LoginUser, taskEmailPostID);
      if (taskEmailPost.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return taskEmailPost.GetXml("TaskEmailPost", true);
    }
    
    public static string GetTaskEmailPosts(RestCommand command)
    {
      TaskEmailPosts taskEmailPosts = new TaskEmailPosts(command.LoginUser);
      taskEmailPosts.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return taskEmailPosts.GetXml("TaskEmailPosts", "TaskEmailPost", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
