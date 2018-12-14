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
  
  public class RestProducts
  {
    public static string GetProduct(RestCommand command, int productID)
    {
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return product.GetXml("Product", true);
    }
    
    public static string GetProducts(RestCommand command)
    {
      Products products = new Products(command.LoginUser);
      products.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return products.GetXml("Products", "Product", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
