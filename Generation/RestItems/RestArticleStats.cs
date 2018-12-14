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
  
  public class RestArticleStats
  {
    public static string GetArticleStat(RestCommand command, int articleViewID)
    {
      ArticleStat articleStat = ArticleStats.GetArticleStat(command.LoginUser, articleViewID);
      if (articleStat.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return articleStat.GetXml("ArticleStat", true);
    }
    
    public static string GetArticleStats(RestCommand command)
    {
      ArticleStats articleStats = new ArticleStats(command.LoginUser);
      articleStats.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return articleStats.GetXml("ArticleStats", "ArticleStat", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
