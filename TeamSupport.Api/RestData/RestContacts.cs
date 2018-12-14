using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using TeamSupport.Data;
using System.Net;
using System.Web.Security;

namespace TeamSupport.Api
{
  public class RestContacts
  {

    public static string GetItem(RestCommand command, int id)
    {
      ContactsViewItem item = ContactsView.GetContactsViewItem(command.LoginUser, id);
      if (item.OrganizationParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return item.GetXml("Contact", true);
    }

		public static string GetItems(RestCommand command, bool orderByDateCreated = false, int? limitNumber = null, bool isCustomer = false)
		{
			ContactsView items = new ContactsView(command.LoginUser);
			string xmlString = string.Empty;

			try
			{
				if (orderByDateCreated)
				{
					//This seems to be Zapier only
					items.LoadByParentOrganizationID(command.Organization.OrganizationID, command.Filters, null, null, "DateCreated DESC", limitNumber, isCustomer);
				}
				else
				{
					items.LoadByParentOrganizationID(command.Organization.OrganizationID, command.Filters, command.PageNumber, command.PageSize, isCustomer: isCustomer);
				}

				//SQL filtering was done already, there's no need for .NET filtering thus the empty collection for the last parameter
				xmlString = items.GetXml("Contacts", "Contact", true, new System.Collections.Specialized.NameValueCollection(), command.IsPaging);
			}
			catch (Exception ex)
			{
				//if something fails use the old method. This one does not have paging.
				//Something went wrong with the sql filtering and an exception was thrown and caught, .NET filtering will need to be done
				items = new ContactsView(command.LoginUser);

				if (orderByDateCreated)
				{
					items.LoadByParentOrganizationID(command.Organization.OrganizationID, "DateCreated DESC", limitNumber);
				}
				else
				{
					items.LoadByParentOrganizationID(command.Organization.OrganizationID);
				}

				xmlString = items.GetXml("Contacts", "Contact", true, command.Filters);
                ExceptionLogs.LogException(command.LoginUser, ex, "API", "RestContacts. GetItems(). SQL filtering generation failed, fell into old method.");
            }

			return xmlString;
		}

    public static string GetItems(RestCommand command, int organizationID, bool orderByDateCreated = false)
    {
      if (Organizations.GetOrganization(command.LoginUser, organizationID).ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ContactsView items = new ContactsView(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(organizationID, "DateCreated DESC");
      }
      else
      {
        items.LoadByOrganizationID(organizationID);
      }
      return items.GetXml("Contacts", "Contact", true, command.Filters);
    }

    public static string GetTicketContacts(RestCommand command, int ticketIDOrNumber, bool orderByDateCreated = false)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      ContactsView contacts = new ContactsView(command.LoginUser);
      if (orderByDateCreated)
      {
        contacts.LoadByTicketID(ticket.TicketID, "ut.DateCreated DESC");
      }
      else
      {
        contacts.LoadByTicketID(ticket.TicketID);
      }
      return contacts.GetXml("Contacts", "Contact", true, command.Filters);
    }

    public static string AddTicketContact(RestCommand command, int ticketIDOrNumber, int contactID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      User user = Users.GetUser(command.LoginUser, contactID);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);

      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
      tickets.AddContact(user.UserID, ticket.TicketID);
      return ContactsView.GetContactsViewItem(command.LoginUser, user.UserID).GetXml("Contact", true);
    }

    public static string RemoveTicketContact(RestCommand command, int ticketIDOrNumber, int contactID)
    {
      TicketsViewItem ticket = TicketsView.GetTicketsViewItemByIdOrNumber(command.LoginUser, ticketIDOrNumber);
      if (ticket == null || ticket.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      User user = Users.GetUser(command.LoginUser, contactID);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
      if (organization == null || organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      Tickets tickets = new Tickets(command.LoginUser);
	  tickets.RemoveContact(user.UserID, ticket.TicketID);
      return ContactsView.GetContactsViewItem(command.LoginUser, user.UserID).GetXml("Contact", true);
    }

    public static string CreateContact(RestCommand command, int? organizationID)
    {
      Addresses addresses = new Addresses(command.LoginUser);
      Address address = addresses.AddNewAddress();

      PhoneNumbers phoneNumbers = new PhoneNumbers(command.LoginUser);
      PhoneNumber phoneNumber = phoneNumbers.AddNewPhoneNumber();

      Users users = new Users(command.LoginUser);
      User user = users.AddNewUser();
      user.IsActive = true;
      user.FullReadFromXml(command.Data, true, ref phoneNumber, ref address);
      if (organizationID == null && user.OrganizationID == 0)
      {
        if (!String.IsNullOrEmpty(user.Email) && user.Email.Contains("@"))
        {
          Organizations matchDomainCompany = new Organizations(command.LoginUser);
          matchDomainCompany.LoadFirstDomainMatch(command.LoginUser.OrganizationID, user.Email.Substring(user.Email.LastIndexOf("@") + 1));
          if (!matchDomainCompany.IsEmpty)
          {
            user.OrganizationID = matchDomainCompany[0].OrganizationID;
          }
        }

        if (user.OrganizationID == 0)
        {
          user.OrganizationID = Organizations.GetUnknownCompanyID(command.LoginUser);
        }
      }
      else if (organizationID != null)
      {
        user.OrganizationID = (int)organizationID;
      }
      user.ActivatedOn = DateTime.UtcNow;
      user.LastLogin = DateTime.UtcNow;
      user.LastActivity = DateTime.UtcNow.AddHours(-1);
      user.IsPasswordExpired = true;
      user.NeedsIndexing = true;

        if (!string.IsNullOrEmpty(user.CryptedPassword))
        {
            user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(user.CryptedPassword, "MD5");
        }

      user.Collection.Save();
      user.UpdateCustomFieldsFromXml(command.Data);

      if (!String.IsNullOrEmpty(phoneNumber.Number) || !String.IsNullOrEmpty(phoneNumber.Extension))
      {
        phoneNumber.RefType = ReferenceType.Users;
        phoneNumber.RefID = user.UserID;
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
        address.RefType = ReferenceType.Users;
        address.RefID = user.UserID;
        addresses.Save();
      }

      return ContactsView.GetContactsViewItem(command.LoginUser, user.UserID).GetXml("Contact", true);
    }

    public static string UpdateContact(RestCommand command, int id)
    {
      User user = Users.GetUser(command.LoginUser, id);
      User oldUser = Users.GetUser(command.LoginUser, id);
      if (user == null) throw new RestException(HttpStatusCode.BadRequest);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      bool allowOrganizationIdUpdate = true;
      user.ReadFromXml(command.Data, false, allowOrganizationIdUpdate);

      if (user.CryptedPassword != oldUser.CryptedPassword) 
        user.CryptedPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(user.CryptedPassword, "MD5");

      bool isChangingOrganizationId = oldUser.OrganizationID != user.OrganizationID;

      if (isChangingOrganizationId)
      {
        Organization newOrganization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
        if (newOrganization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

        //Let's follow the same reflexes as the UI (..\webapp\app_code\customerservice.cs SetContactCompany())
        Tickets t = new Tickets(command.LoginUser);
        t.LoadByContact(id);

        foreach (Ticket tix in t)
        {
          tix.Collection.RemoveContact(id, tix.TicketID);
        }

        user.PortalAutoReg = false;

      user.Collection.Save();

        foreach (Ticket tix in t)
        {
          tix.Collection.AddContact(id, tix.TicketID);
        }

        EmailPosts ep = new EmailPosts(command.LoginUser);
        ep.LoadByRecentUserID(id);
        ep.DeleteAll();
        ep.Save();
      }
      else
      {
        user.Collection.Save();
      }
      

      user.UpdateCustomFieldsFromXml(command.Data);
      return ContactsView.GetContactsViewItem(command.LoginUser, user.UserID).GetXml("Contact", true);
    }

    public static string DeleteContact(RestCommand command, int id)
    {
      ContactsViewItem user = ContactsView.GetContactsViewItem(command.LoginUser, id);
      if (user == null) throw new RestException(HttpStatusCode.BadRequest);
      Organization organization = Organizations.GetOrganization(command.LoginUser, user.OrganizationID);
      if (organization.ParentID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      string result = user.GetXml("Contact", true);
      Users.MarkUserDeleted(command.LoginUser, id);
      return result;
    }
  }
}