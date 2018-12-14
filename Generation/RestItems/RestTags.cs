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
  
  public class RestTags
  {
    public static string GetTag(RestCommand command, int tagID)
    {
      Tag tag = Tags.GetTag(command.LoginUser, tagID);
      if (tag.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return tag.GetXml("Tag", true);
    }
    
    public static string GetTags(RestCommand command)
    {
      Tags tags = new Tags(command.LoginUser);
      tags.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return tags.GetXml("Tags", "Tag", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
