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
  
  public class RestProductVersionsView
  {
    public static string GetProductVersionsViewItem(RestCommand command, int productVersionID)
    {
      ProductVersionsViewItem productVersionsViewItem = ProductVersionsView.GetProductVersionsViewItem(command.LoginUser, productVersionID);
      if (productVersionsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return productVersionsViewItem.GetXml("ProductVersionsViewItem", true);
    }
    
    public static string GetProductVersionsView(RestCommand command)
    {
      ProductVersionsView productVersionsView = new ProductVersionsView(command.LoginUser);
      productVersionsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return productVersionsView.GetXml("ProductVersionsView", "ProductVersionsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
