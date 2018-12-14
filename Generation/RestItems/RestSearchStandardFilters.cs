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
  
  public class RestSearchStandardFilters
  {
    public static string GetSearchStandardFilter(RestCommand command, int standardFilterID)
    {
      SearchStandardFilter searchStandardFilter = SearchStandardFilters.GetSearchStandardFilter(command.LoginUser, standardFilterID);
      if (searchStandardFilter.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return searchStandardFilter.GetXml("SearchStandardFilter", true);
    }
    
    public static string GetSearchStandardFilters(RestCommand command)
    {
      SearchStandardFilters searchStandardFilters = new SearchStandardFilters(command.LoginUser);
      searchStandardFilters.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return searchStandardFilters.GetXml("SearchStandardFilters", "SearchStandardFilter", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
