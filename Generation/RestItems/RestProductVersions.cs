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
  
  public class RestProductVersions
  {
    public static string GetProductVersion(RestCommand command, int productVersionID)
    {
      ProductVersion productVersion = ProductVersions.GetProductVersion(command.LoginUser, productVersionID);
      if (productVersion.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return productVersion.GetXml("ProductVersion", true);
    }
    
    public static string GetProductVersions(RestCommand command)
    {
      ProductVersions productVersions = new ProductVersions(command.LoginUser);
      productVersions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return productVersions.GetXml("ProductVersions", "ProductVersion", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
