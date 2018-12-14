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
  
  public class RestProductVersionStatuses
  {
    public static string GetProductVersionStatus(RestCommand command, int productVersionStatusID)
    {
      ProductVersionStatus productVersionStatus = ProductVersionStatuses.GetProductVersionStatus(command.LoginUser, productVersionStatusID);
      if (productVersionStatus.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return productVersionStatus.GetXml("ProductVersionStatus", true);
    }
    
    public static string GetProductVersionStatuses(RestCommand command)
    {
      ProductVersionStatuses productVersionStatuses = new ProductVersionStatuses(command.LoginUser);
      productVersionStatuses.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return productVersionStatuses.GetXml("ProductVersionStatuses", "ProductVersionStatus", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
