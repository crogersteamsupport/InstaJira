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
  
  public class RestTagLinks
  {
    public static string GetTagLink(RestCommand command, int tagLinkID)
    {
      TagLink tagLink = TagLinks.GetTagLink(command.LoginUser, tagLinkID);
      if (tagLink.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return tagLink.GetXml("TagLink", true);
    }
    
    public static string GetTagLinks(RestCommand command)
    {
      TagLinks tagLinks = new TagLinks(command.LoginUser);
      tagLinks.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return tagLinks.GetXml("TagLinks", "TagLink", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
