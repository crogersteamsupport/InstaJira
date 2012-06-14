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
  public class RestProducts
  {

    public static string GetProduct(RestCommand command, int id)
    {
      Product item = Products.GetProduct(command.LoginUser, id);
      if (item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return item.GetXml("Product", true);
    }

    public static string GetProducts(RestCommand command)
    {
      Products items = new Products(command.LoginUser);
      items.LoadByOrganizationID(command.Organization.OrganizationID);
      return items.GetXml("Products", "Product", true, command.Filters);
    }

    public static string GetProducts(RestCommand command, int organizationID)
    {
      if (Organizations.GetOrganization(command.LoginUser, organizationID).ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Products items = new Products(command.LoginUser);
      items.LoadByCustomerID(organizationID);
      return items.GetXml("Products", "Product", true, command.Filters);
    }

    public static string CreateProduct(RestCommand command)
    {
      Products items = new Products(command.LoginUser);
      Product item = items.AddNewProduct();
      item.ReadFromXml(command.Data, true);
      item.OrganizationID = command.Organization.OrganizationID;
      item.Collection.Save();
      item.UpdateCustomFieldsFromXml(command.Data);
      return Products.GetProduct(command.LoginUser, item.ProductID).GetXml("Product", true);
    }

    public static string UpdateProduct(RestCommand command, int id)
    {
      Product item = Products.GetProduct(command.LoginUser, id);
      if (item == null) throw new RestException(HttpStatusCode.BadRequest);
      if (item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      item.ReadFromXml(command.Data, false);
      item.Collection.Save();
      item.UpdateCustomFieldsFromXml(command.Data);
      return Products.GetProduct(command.LoginUser, item.ProductID).GetXml("Product", true);
    }

    public static string GetOrganizationProducts(RestCommand command, int organizationID)
    {
      Organization item = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (item == null || item.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Products items = new Products(command.LoginUser);
      items.LoadByCustomerID(organizationID);
      return items.GetXml("Products", "Product", true, command.Filters);
    }

    public static string AddOrganizationProduct(RestCommand command, int organizationID, int productID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product == null || product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Products products = new Products(command.LoginUser);
      products.AddCustomer(organizationID, productID);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID).GetXml("Customer", true);
    }

    public static string RemoveOrganizationProduct(RestCommand command, int organizationID, int productID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product == null || product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Products products = new Products(command.LoginUser);
      products.RemoveCustomer(organizationID, productID);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID).GetXml("Customer", true);
    }

  }
}
