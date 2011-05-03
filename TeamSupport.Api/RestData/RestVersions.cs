﻿using System;
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
  public class RestVersions
  {

    public static string GetItem(RestCommand command, int id)
    {
      ProductVersionsViewItem item = ProductVersionsView.GetProductVersionsViewItem(command.LoginUser, id);
      if (item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return item.GetXml("Version", true);
    }

    public static string GetItems(RestCommand command, int productID)
    {
      ProductVersionsView items = new ProductVersionsView(command.LoginUser);
      if (Products.GetProduct(command.LoginUser, productID).OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      items.LoadByProductID(productID);
      return items.GetXml("Versions", "Version", true, command.Filters);
    }

    public static string CreateVersion(RestCommand command, int productID)
    {
      if (Products.GetProduct(command.LoginUser, productID).OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ProductVersions items = new ProductVersions(command.LoginUser);
      ProductVersion item = items.AddNewProductVersion();
      item.ReadFromXml(command.Data, true);
      item.ProductID = productID;
      item.Collection.Save();
      item.UpdateCustomFieldsFromXml(command.Data);
      return ProductVersionsView.GetProductVersionsViewItem(command.LoginUser, item.ProductVersionID).GetXml("Version", true);
    }

    public static string UpdateVersion(RestCommand command, int id)
    {
      ProductVersion item = ProductVersions.GetProductVersion(command.LoginUser, id);
      if (item == null) throw new RestException(HttpStatusCode.BadRequest);
      if (Products.GetProduct(command.LoginUser, item.ProductID).OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      item.ReadFromXml(command.Data, false);
      item.Collection.Save();
      item.UpdateCustomFieldsFromXml(command.Data);
      return ProductVersionsView.GetProductVersionsViewItem(command.LoginUser, item.ProductVersionID).GetXml("Version", true);
    }

    public static string GetOrganizationVersions(RestCommand command, int productID, int organizationID)
    {
      Organization item = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (item == null || item.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ProductVersionsView items = new ProductVersionsView(command.LoginUser);
      items.LoadByProductAndCustomer(productID, organizationID);
      return items.GetXml("Versions", "Version", true, command.Filters);
    }


  }
}
