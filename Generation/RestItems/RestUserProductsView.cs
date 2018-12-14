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
  
  public class RestUserProductsView
  {
    public static string GetUserProductsViewItem(RestCommand command, int userProductID)
    {
      UserProductsViewItem userProductsViewItem = UserProductsView.GetUserProductsViewItem(command.LoginUser, userProductID);
      if (userProductsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return userProductsViewItem.GetXml("UserProductsViewItem", true);
    }
    
    public static string GetUserProductsView(RestCommand command)
    {
      UserProductsView userProductsView = new UserProductsView(command.LoginUser);
      userProductsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return userProductsView.GetXml("UserProductsView", "UserProductsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
