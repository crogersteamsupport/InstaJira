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
  
  public class RestAssetHistory
  {
    public static string GetAssetHistoryItem(RestCommand command, int historyID)
    {
      AssetHistoryItem assetHistoryItem = AssetHistory.GetAssetHistoryItem(command.LoginUser, historyID);
      if (assetHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return assetHistoryItem.GetXml("AssetHistoryItem", true);
    }
    
    public static string GetAssetHistory(RestCommand command)
    {
      AssetHistory assetHistory = new AssetHistory(command.LoginUser);
      assetHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return assetHistory.GetXml("AssetHistory", "AssetHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
