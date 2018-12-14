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
      return assetsViewItem.GetXml("AssetsViewItem", true);
    }
    
    public static string GetAssetsView(RestCommand command)
    {
      AssetsView assetsView = new AssetsView(command.LoginUser);
      assetsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return assetsView.GetXml("AssetsView", "AssetsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
