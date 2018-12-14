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
    public static string CreateOrganizationProduct(RestCommand command, int organizationID, int productID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product == null || product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.NotFound);

      OrganizationProducts organizationProducts = new OrganizationProducts(command.LoginUser);
      OrganizationProduct organizationProduct = organizationProducts.AddNewOrganizationProduct();
      organizationProduct.OrganizationID = organizationID;
      organizationProduct.ProductID = productID;
      organizationProduct.ReadFromXml(command.Data, true);
      organizationProduct.Collection.Save();
      organizationProduct.UpdateCustomFieldsFromXml(command.Data);

      return OrganizationProductsView.GetOrganizationProductsViewItem(command.LoginUser, organizationProduct.OrganizationProductID).GetXml("OrganizationProduct", true);
    }

    public static string GetOrganizationProduct(RestCommand command, int organizationProductID)
    {
      OrganizationProduct organizationProduct = OrganizationProducts.GetOrganizationProduct(command.LoginUser, organizationProductID);
      if (organizationProduct.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return organizationProduct.GetXml("OrganizationProduct", true);
    }

    public static string GetOrganizationProductItems(RestCommand command, int organizationID, int productID, bool orderByDateCreated = false)
    {
      Organization item = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (item == null || item.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product == null || product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.NotFound);

      OrganizationProductsView organizationProductsView = new OrganizationProductsView(command.LoginUser);
      if (orderByDateCreated)
      {
        organizationProductsView.LoadByOrganizationAndProductIDs(organizationID, productID, "DateCreated DESC");
      }
      else
      {
        organizationProductsView.LoadByOrganizationAndProductIDs(organizationID, productID);
      }

      return organizationProductsView.GetXml("OrganizationProducts", "OrganizationProduct", true, command.Filters);
    }

    public static string GetOrganizationProductItem(RestCommand command, int organizationID, int productID, int organizationProductID)
    {
      Organization item = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (item == null || item.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product == null || product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.NotFound);
      OrganizationProduct organizationProduct = OrganizationProducts.GetOrganizationProduct(command.LoginUser, organizationProductID);
      if (organizationProduct == null || organizationProduct.OrganizationID != organizationID) throw new RestException(HttpStatusCode.NotFound);

      OrganizationProductsView organizationProductsView = new OrganizationProductsView(command.LoginUser);
      organizationProductsView.LoadByOrganizationProductID(organizationProductID, true);

      return organizationProductsView.GetXml("OrganizationProducts", "OrganizationProduct", true, command.Filters);
    }

    public static string GetOrganizationProductVersionItems(RestCommand command, int organizationID, int productID, int productVersionID)
    {
      Organization item = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (item == null || item.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product == null || product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.NotFound);
      ProductVersion productVersion = ProductVersions.GetProductVersion(command.LoginUser, productVersionID);
      if (productVersion == null || productVersion.ProductID != product.ProductID) throw new RestException(HttpStatusCode.NotFound);

      OrganizationProductsView organizationProductsView = new OrganizationProductsView(command.LoginUser);
      organizationProductsView.LoadByOrganizationProductAndVersionIDs(organizationID, productID, productVersionID);

      return organizationProductsView.GetXml("OrganizationProducts", "OrganizationProduct", true, command.Filters);
    }

    public static string GetOrganizationProducts(RestCommand command, int organizationID, bool orderByDateCreated = false)
    {
      Organization item = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (item == null || item.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationProductsView organizationProductsView = new OrganizationProductsView(command.LoginUser);
      if (orderByDateCreated)
      {
        organizationProductsView.LoadByOrganizationID(organizationID, "DateCreated DESC");
      }
      else
      {
        organizationProductsView.LoadByOrganizationID(organizationID);
      }

      return organizationProductsView.GetXml("OrganizationProducts", "OrganizationProduct", true, command.Filters);
    }

    public static string UpdateOrganizationProductItem(RestCommand command, int organizationID, int productID, int organizationProductID)
    {
      Organization item = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (item == null || item.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product == null || product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.NotFound);
      OrganizationProduct organizationProduct = OrganizationProducts.GetOrganizationProduct(command.LoginUser, organizationProductID);
      if (organizationProduct == null || organizationProduct.OrganizationID != organizationID) throw new RestException(HttpStatusCode.NotFound);

      organizationProduct.ReadFromXml(command.Data, false);
      organizationProduct.Collection.Save();
      organizationProduct.UpdateCustomFieldsFromXml(command.Data);

      return OrganizationProductsView.GetOrganizationProductsViewItem(command.LoginUser, organizationProduct.OrganizationProductID).GetXml("OrganizationProduct", true);
    }

    public static string DeleteOrganizationProductItem(RestCommand command, int organizationID, int productID, int organizationProductID)
    {
      Organization item = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (item == null || item.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Product product = Products.GetProduct(command.LoginUser, productID);
      if (product == null || product.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.NotFound);
      OrganizationProduct organizationProduct = OrganizationProducts.GetOrganizationProduct(command.LoginUser, organizationProductID);
      if (organizationProduct == null || organizationProduct.OrganizationID != organizationID) throw new RestException(HttpStatusCode.NotFound);

      string result = OrganizationProductsView.GetOrganizationProductsViewItem(command.LoginUser, organizationProduct.OrganizationProductID).GetXml("OrganizationProduct", true);
      OrganizationProducts organizationProducts = new OrganizationProducts(command.LoginUser);
      organizationProducts.DeleteFromDB(organizationProductID);
      return result;
    }

  }
  
}





  
