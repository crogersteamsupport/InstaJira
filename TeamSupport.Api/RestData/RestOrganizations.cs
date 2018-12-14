using System;
using System.Collections.Generic;
using System.Linq;
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

    public static string GetOrganizations(RestCommand command, bool orderByDateCreated = false, int? limitNumber = null)
    {
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
	  bool hasBeenFiltered = false;

      if (orderByDateCreated)
      {
		//This seems to be Zapier only
        organizations.LoadByParentID(command.Organization.OrganizationID, true, "DateCreated DESC", limitNumber);
		hasBeenFiltered = true;
      }
      else
      {
        try
        {
          organizations.LoadByParentID(command.Organization.OrganizationID, true, command.Filters, command.PageNumber, command.PageSize);
		  hasBeenFiltered = true;
        }
        catch (Exception e)
        {
          organizations = new OrganizationsView(command.LoginUser);
          organizations.LoadByParentID(command.Organization.OrganizationID, true);
            ExceptionLogs.LogException(command.LoginUser, e, "API", "RestOrganizations. RestGetOrganization(). SQL filtering generation failed, fell into old method (no filters?).");
        }
      }
      return organizations.GetXml("Customers", "Customer", true, !hasBeenFiltered ? command.Filters : new System.Collections.Specialized.NameValueCollection(), command.IsPaging);
    }

    public static string CreateOrganization(RestCommand command)
    {
      Addresses addresses = new Addresses(command.LoginUser);
      Address address = addresses.AddNewAddress();
      
      PhoneNumbers phoneNumbers = new PhoneNumbers(command.LoginUser);
      PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();

      Organizations organizations = new Organizations(command.LoginUser);
      Organization organization = organizations.AddNewOrganization();
      organization.ParentID = command.Organization.OrganizationID;
      organization.IsActive = true;
      organization.FullReadFromXml(command.Data, true, ref phoneNumber, ref address);
      organization.NeedsIndexing = true;
      organization.Collection.Save();
      organization.UpdateCustomFieldsFromXml(command.Data);

      if (!String.IsNullOrEmpty(phoneNumber.Number) || !String.IsNullOrEmpty(phoneNumber.Extension))
      {
        phoneNumber.RefType = ReferenceType.Organizations;
        phoneNumber.RefID = organization.OrganizationID;
        phoneNumbers.Save();
      }

      if (!String.IsNullOrEmpty(address.Description)
      || !String.IsNullOrEmpty(address.Addr1)
      || !String.IsNullOrEmpty(address.Addr2)
      || !String.IsNullOrEmpty(address.Addr3)
      || !String.IsNullOrEmpty(address.City)
      || !String.IsNullOrEmpty(address.Country)
      || !String.IsNullOrEmpty(address.Description)
      || !String.IsNullOrEmpty(address.State)
      || !String.IsNullOrEmpty(address.Zip))
      {
        address.RefType = ReferenceType.Organizations;
        address.RefID = organization.OrganizationID;
        addresses.Save();
      }

      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organization.OrganizationID).GetXml("Customer", true);
    }

    public static string UpdateOrganization(RestCommand command, int organizationID)
    {
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null) throw new RestException(HttpStatusCode.BadRequest);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      organization.ReadFromXml(command.Data, false);
	  SetFieldIdsByName(command, ref organization);
	  
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

    public static string GetTicketOrganizations(RestCommand command, int ticketIDOrNumber, bool orderByDateCreated = false)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      if (orderByDateCreated)
      {
        organizations.LoadByTicketID(ticket.TicketID, "ot.DateCreated DESC");
      }
      else
      {
        organizations.LoadByTicketID(ticket.TicketID);
      }
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

    public static string AddTicketOrganization(RestCommand command, int ticketIDOrNumber, int organizationID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.AddOrganization(organizationID, ticket.TicketID);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID).GetXml("Customer", true);
    }

    public static string RemoveTicketOrganization(RestCommand command, int ticketIDOrNumber, int organizationID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      Organization organization = Organizations.GetOrganization(command.LoginUser, organizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.RemoveOrganization(organizationID, ticket.TicketID);
      return OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID).GetXml("Customer", true);
    }

    public static string GetProductOrganizations(RestCommand command, int productID, bool orderByDateCreated = false)
    {
      Product item = Products.GetProduct(command.LoginUser, productID);
      if (item == null || item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      if (orderByDateCreated)
      {
        organizations.LoadByProductID(productID, "op.DateCreated DESC");
      }
      else
      {
        organizations.LoadByProductID(productID);
      }
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

    public static string GetVersionOrganizations(RestCommand command, int versionID, bool orderByDateCreated = false)
    {
      ProductVersionsViewItem item = ProductVersionsView.GetProductVersionsViewItem(command.LoginUser, versionID);
      if (item == null || item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      OrganizationsView organizations = new OrganizationsView(command.LoginUser);
      if (orderByDateCreated) 
      {
        organizations.LoadByVersionID(versionID, "op.DateCreated DESC");        
      }
      else
      {
        organizations.LoadByVersionID(versionID);
      }
      return organizations.GetXml("Customers", "Customer", true, command.Filters);
    }

		/// <summary>
		/// Finds and sets the Organization fields ids by searching their name
		/// </summary>
		/// <param name="command">Command received in the request to read and process the data in the request body.</param>
		/// <param name="organizationId">OrganizationId to update its record.</param>
		private static void SetFieldIdsByName(RestCommand command, ref Organization organization)
		{
			try
			{
				//Add as necessary to the list and then to the switch-case below for the work to update it.
				List<string> fields = new List<string>() { "slaname", "defaultsupportgroup", "defaultsupportuser" };

				foreach (string field in fields.Select(p => p.ToLower()).ToList())
				{
					XmlNode node = GetNode(command, field);

					if (node != null)
					{
						switch (field)
						{
							case "slaname":
								string slaName = node.InnerText;
								int? slaLevelId = SlaLevel.GetIDByName(command.LoginUser, slaName, null);
								bool wasFoundByName = false;

								if (slaLevelId != null)
								{
									//check if id belongs to org
									SlaLevel slaLevel = SlaLevels.GetSlaLevel(command.LoginUser, (int)slaLevelId);

									if (slaLevel.OrganizationID == organization.ParentID)
									{
										organization.SlaLevelID = slaLevel.SlaLevelID;
										wasFoundByName = true;
									}
								}

								if (!wasFoundByName)
								{
									//check if also the SlaLevelId was sent and it's different than current
									string slaLevelIdField = "slalevelid";
									XmlNode nodeSlaLevelId = GetNode(command, slaLevelIdField);

									if (nodeSlaLevelId != null)
									{
										string slaLevelIdText = nodeSlaLevelId.InnerText;
										int slaIdSent = 0;

										if (int.TryParse(slaLevelIdText, out slaIdSent))
										{
											//check if id belongs to org
											SlaLevel slaLevel = SlaLevels.GetSlaLevel(command.LoginUser, slaIdSent);

											if (slaLevel != null && slaLevel.OrganizationID == organization.ParentID)
											{
												organization.SlaLevelID = slaLevel.SlaLevelID;
											}
											else
											{
												int? currentSlaLevelId = Organizations.GetOrganization(command.LoginUser, organization.OrganizationID).SlaLevelID;
												organization.SlaLevelID = currentSlaLevelId;
											}
										}
									}
								}

								break;
							//ToDo: DefaultSupportGroup, DefaultSupportUser
							default:
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionLogs.LogException(command.LoginUser, ex, "API", string.Format("OrgID: {0}{1}Verb: {2}{1}Url: {3}{1}Body: {4}", command.Organization.OrganizationID, Environment.NewLine, command.Method, command.Method, command.Data));
			}
		}

		private static XmlNode GetNode(RestCommand command, string field)
		{
			XmlDocument xml = new XmlDocument();
            xml.XmlResolver = null;
            xml.LoadXml(command.Data);
			string query = "*[translate(local-name(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='customer']" +
							"/*[translate(local-name(),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']";
			query = string.Format(query, field);
			XmlNode node = xml.SelectSingleNode(query);

			//If node not found and the request includes a top level of collection items then try again with it
			if (node == null)
			{
				query = "*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='customers']" +
						"/*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='customer']" +
						"/*[translate(local-name(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='{0}']";
				query = string.Format(query, field);
				XmlNodeList nodeList = xml.SelectNodes(query);

				if (nodeList != null && nodeList.Count > 0)
				{
					node = nodeList[0];
				}
			}

			return node;
		}
	}
}
