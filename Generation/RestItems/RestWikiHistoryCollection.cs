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
  
  public class RestWikiHistoryCollection
  {
    public static string GetWikiHistory(RestCommand command, int historyID)
    {
      WikiHistory wikiHistory = WikiHistoryCollection.GetWikiHistory(command.LoginUser, historyID);
      if (wikiHistory.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return wikiHistory.GetXml("WikiHistory", true);
    }
    
    public static string GetWikiHistoryCollection(RestCommand command)
    {
      WikiHistoryCollection wikiHistoryCollection = new WikiHistoryCollection(command.LoginUser);
      wikiHistoryCollection.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return wikiHistoryCollection.GetXml("WikiHistoryCollection", "WikiHistory", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
