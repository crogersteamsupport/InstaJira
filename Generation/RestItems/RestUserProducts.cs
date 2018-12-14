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
  
  public class RestUserProducts
  {
    public static string GetUserProduct(RestCommand command, int userProductID)
    {
      UserProduct userProduct = UserProducts.GetUserProduct(command.LoginUser, userProductID);
      if (userProduct.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return userProduct.GetXml("UserProduct", true);
    }
    
    public static string GetUserProducts(RestCommand command)
    {
      UserProducts userProducts = new UserProducts(command.LoginUser);
      userProducts.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return userProducts.GetXml("UserProducts", "UserProduct", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
