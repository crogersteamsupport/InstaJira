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
  
  public class RestPortalLoginHistory
  {
    public static string GetPortalLoginHistoryItem(RestCommand command, int portalLoginID)
    {
      PortalLoginHistoryItem portalLoginHistoryItem = PortalLoginHistory.GetPortalLoginHistoryItem(command.LoginUser, portalLoginID);
      if (portalLoginHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return portalLoginHistoryItem.GetXml("PortalLoginHistoryItem", true);
    }
    
    public static string GetPortalLoginHistory(RestCommand command)
    {
      PortalLoginHistory portalLoginHistory = new PortalLoginHistory(command.LoginUser);
      portalLoginHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return portalLoginHistory.GetXml("PortalLoginHistory", "PortalLoginHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
