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
  
  public class RestAssets
  {
    public static string GetAsset(RestCommand command, int assetID)
    {
      Asset asset = Assets.GetAsset(command.LoginUser, assetID);
      if (asset.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return asset.GetXml("Asset", true);
    }
    
    public static string GetAssets(RestCommand command)
    {
      Assets assets = new Assets(command.LoginUser);
      assets.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return assets.GetXml("Assets", "Asset", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
