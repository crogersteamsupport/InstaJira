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
  
  public class RestAssetAssignmentsView
  {
    public static string GetAssetAssignmentsViewItem(RestCommand command, int assetAssignmentsID)
    {
      AssetAssignmentsViewItem assetAssignmentsViewItem = AssetAssignmentsView.GetAssetAssignmentsViewItem(command.LoginUser, assetAssignmentsID);
      if (assetAssignmentsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return assetAssignmentsViewItem.GetXml("AssetAssignmentsViewItem", true);
    }
    
    public static string GetAssetAssignmentsView(RestCommand command, int assetID)
    {
      AssetAssignmentsView assetAssignmentsView = new AssetAssignmentsView(command.LoginUser);
      assetAssignmentsView.LoadByAssetID(assetID);

      if (command.Format == RestFormat.XML)
      {
        return assetAssignmentsView.GetXml("AssetAssignments", "AssetAssignment", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
