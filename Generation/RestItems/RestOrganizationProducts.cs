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
  
  public class RestOrganizationProducts
  {
    public static string GetOrganizationProduct(RestCommand command, int organizationProductID)
    {
      OrganizationProduct organizationProduct = OrganizationProducts.GetOrganizationProduct(command.LoginUser, organizationProductID);
      if (organizationProduct.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return organizationProduct.GetXml("OrganizationProduct", true);
    }
    
    public static string GetOrganizationProducts(RestCommand command)
    {
      OrganizationProducts organizationProducts = new OrganizationProducts(command.LoginUser);
      organizationProducts.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return organizationProducts.GetXml("OrganizationProducts", "OrganizationProduct", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
