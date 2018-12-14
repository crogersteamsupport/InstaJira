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
  
  public class RestAssetHistoryView
  {
    public static string GetAssetHistoryViewItem(RestCommand command, int historyID)
    {
      AssetHistoryViewItem assetHistoryViewItem = AssetHistoryView.GetAssetHistoryViewItem(command.LoginUser, historyID);
      if (assetHistoryViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return assetHistoryViewItem.GetXml("AssetHistoryViewItem", true);
    }
    
    public static string GetAssetHistoryView(RestCommand command)
    {
      AssetHistoryView assetHistoryView = new AssetHistoryView(command.LoginUser);
      assetHistoryView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return assetHistoryView.GetXml("AssetHistoryView", "AssetHistoryViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }

    public static string GetAssetHistoryView(RestCommand command, int assetID)
    {
      AssetHistoryView assetHistoryView = new AssetHistoryView(command.LoginUser);
      assetHistoryView.LoadByAssetID(assetID);

      if (command.Format == RestFormat.XML)
      {
        return assetHistoryView.GetXml("AssetHistory", "AssetHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }

    }
  }
  
}





  
