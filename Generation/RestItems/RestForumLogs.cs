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
  
  public class RestForumLogs
  {
    public static string GetForumLog(RestCommand command, int forumLogID)
    {
      ForumLog forumLog = ForumLogs.GetForumLog(command.LoginUser, forumLogID);
      if (forumLog.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return forumLog.GetXml("ForumLog", true);
    }
    
    public static string GetForumLogs(RestCommand command)
    {
      ForumLogs forumLogs = new ForumLogs(command.LoginUser);
      forumLogs.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return forumLogs.GetXml("ForumLogs", "ForumLog", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
