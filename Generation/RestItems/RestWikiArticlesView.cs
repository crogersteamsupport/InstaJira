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
  
  public class RestWikiArticlesView
  {
    public static string GetWikiArticlesViewItem(RestCommand command, int articleID)
    {
      WikiArticlesViewItem wikiArticlesViewItem = WikiArticlesView.GetWikiArticlesViewItem(command.LoginUser, articleID);
      if (wikiArticlesViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return wikiArticlesViewItem.GetXml("WikiArticlesViewItem", true);
    }
    
    public static string GetWikiArticlesView(RestCommand command)
    {
      WikiArticlesView wikiArticlesView = new WikiArticlesView(command.LoginUser);
      wikiArticlesView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return wikiArticlesView.GetXml("WikiArticlesView", "WikiArticlesViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
