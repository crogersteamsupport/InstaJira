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
  
  public class RestAssetsView
  {
    public static string GetAssetsViewItem(RestCommand command, int assetID)
    {
      AssetsViewItem assetsViewItem = AssetsView.GetAssetsViewItem(command.LoginUser, assetID);
      if (assetsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return assetsViewItem.GetXml("Asset", true);
    }
    
    public static string GetAssetsView(RestCommand command)
    {
      AssetsView assetsView = new AssetsView(command.LoginUser);

      try
      {
        assetsView.LoadByOrganizationID(command.Organization.OrganizationID, command.Filters);
      }
      catch (Exception ex)
      {
        //if something fails use the old method
        assetsView.LoadByOrganizationID(command.Organization.OrganizationID);
        ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestAssetsView. GetAssetsView(). SQL filtering generation failed, fell into old method.");
      }

      return assetsView.GetXml("Assets", "Asset", true, command.Filters);
    }

    public static string GetAssetsView(RestCommand command, int ticketIDOrTicketNumber)
    {
      AssetsView assetsView = new AssetsView(command.LoginUser);
      assetsView.LoadByTicketID(ticketIDOrTicketNumber);

      if (assetsView.IsEmpty)
      {
        assetsView = new AssetsView(command.LoginUser);
        assetsView.LoadByTicketNumber(ticketIDOrTicketNumber, command.LoginUser.OrganizationID);      
      }
      
      if (command.Format == RestFormat.XML)
      {
        return assetsView.GetXml("Assets", "Asset", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }

    }
  }
  
}





  
