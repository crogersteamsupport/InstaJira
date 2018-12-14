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
  
  public class RestWikiArticles
  {
    public static string GetWikiArticle(RestCommand command, int articleID)
    {
      WikiArticle wikiArticle = WikiArticles.GetWikiArticle(command.LoginUser, articleID);
      if (wikiArticle.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return wikiArticle.GetXml("WikiArticle", true);
    }
    
    public static string GetWikiArticles(RestCommand command)
    {
      WikiArticles wikiArticles = new WikiArticles(command.LoginUser);
      wikiArticles.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return wikiArticles.GetXml("WikiArticles", "WikiArticle", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
