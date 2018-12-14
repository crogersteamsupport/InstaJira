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
  
  public class RestAssetAssignments
  {
    public static string GetAssetAssignment(RestCommand command, int assetAssignmentsID)
    {
      AssetAssignment assetAssignment = AssetAssignments.GetAssetAssignment(command.LoginUser, assetAssignmentsID);
      if (assetAssignment.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return assetAssignment.GetXml("AssetAssignment", true);
    }
    
    public static string GetAssetAssignments(RestCommand command)
    {
      AssetAssignments assetAssignments = new AssetAssignments(command.LoginUser);
      assetAssignments.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return assetAssignments.GetXml("AssetAssignments", "AssetAssignment", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
