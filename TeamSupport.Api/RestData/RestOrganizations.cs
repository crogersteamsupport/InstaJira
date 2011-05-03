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
  public class RestOrganizations
  {

    public static string GetOrganization(RestCommand command, int organizationID)
    {
      OrganizationsViewItem organization = OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return organization.GetXml("Customer", true);
    }

    public static string GetOrganizations(RestCommand command)
    {
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      organizations.LoadByParentID(command.Organization.OrganizationID, true);
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

    public static string CreateOrganization(RestCommand command)
    {
      Organizations organizations = new Organizations(command.LoginUser);
      Organization organization = organizations.AddNewOrganization();
      organization.ParentID = command.Organization.OrganizationID;
      organization.ReadFromXml(command.Data, true);
      organization.Collection.Save();
      organization.UpdateCustomFieldsFromXml(command.Data);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organization.OrganizationID).GetXml("Customer", true);
    }

    public static string UpdateOrganization(RestCommand command, int organizationID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null) throw new RestException(HttpStatusCode.BadRequest);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      organization.ReadFromXml(command.Data, false);
      organization.Collection.Save();
      organization.UpdateCustomFieldsFromXml(command.Data);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organization.OrganizationID).GetXml("Customer", true);
    }

    public static string DeleteOrganization(RestCommand command, int organizationID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null) throw new RestException(HttpStatusCode.BadRequest);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      string result = organization.GetXml("Customer", true);
      Organizations.DeleteOrganizationAndAllReleatedData(command.LoginUser, organizationID);
      return result;
    }

    public static string GetTicketOrganizations(RestCommand command, int ticketID)
    {
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      organizations.LoadByTicketID(ticketID);
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

    public static string AddTicketOrganization(RestCommand command, int ticketID, int organizationID)
    {
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.AddOrganization(organizationID, ticketID);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID).GetXml("Customer", true);
    }

    public static string RemoveTicketOrganization(RestCommand command, int ticketID, int organizationID)
    {
      Ticket ticket = Tickets.GetTicket(command.LoginUser, ticketID);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.RemoveOrganization(organizationID, ticketID);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID).GetXml("Customer", true);
    }

    public static string GetProductOrganizations(RestCommand command, int productID)
    {
      Product item = Products.GetProduct(command.LoginUser, productID);
      if (item == null || item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      organizations.LoadByProductID(productID);
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

    public static string GetVersionOrganizations(RestCommand command, int versionID)
    {
      ProductVersionsViewItem item = ProductVersionsView.GetProductVersionsViewItem(command.LoginUser, versionID);
      if (item == null || item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      organizations.LoadByVersionID(versionID);
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

  }
}
