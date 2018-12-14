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
  
  public class RestEmailPosts
  {
    public static string GetEmailPost(RestCommand command, int emailPostID)
    {
      EmailPost emailPost = EmailPosts.GetEmailPost(command.LoginUser, emailPostID);
      if (emailPost.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return emailPost.GetXml("EmailPost", true);
    }
    
    public static string GetEmailPosts(RestCommand command)
    {
      EmailPosts emailPosts = new EmailPosts(command.LoginUser);
      emailPosts.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return emailPosts.GetXml("EmailPosts", "EmailPost", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
