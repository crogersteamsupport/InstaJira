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
  
  public class RestAssetTickets
  {
    public static string GetAssetTicket(RestCommand command, int ticketID)
    {
      AssetTicket assetTicket = AssetTickets.GetAssetTicket(command.LoginUser, ticketID);
      if (assetTicket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return assetTicket.GetXml("AssetTicket", true);
    }
    
    public static string GetAssetTickets(RestCommand command)
    {
      AssetTickets assetTickets = new AssetTickets(command.LoginUser);
      assetTickets.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return assetTickets.GetXml("AssetTickets", "AssetTicket", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
