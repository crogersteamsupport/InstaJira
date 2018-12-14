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
  
  public class RestProductFamilies
  {
    public static string GetProductFamily(RestCommand command, int productFamilyID)
    {
      ProductFamily productFamily = ProductFamilies.GetProductFamily(command.LoginUser, productFamilyID);
      if (productFamily.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return productFamily.GetXml("ProductFamily", true);
    }
    
    public static string GetProductFamilies(RestCommand command)
    {
      ProductFamilies productFamilies = new ProductFamilies(command.LoginUser);
      productFamilies.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return productFamilies.GetXml("ProductFamilies", "ProductFamily", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
