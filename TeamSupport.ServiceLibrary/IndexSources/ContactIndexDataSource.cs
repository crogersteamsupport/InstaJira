using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TeamSupport.Data;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class ContactIndexDataSource : IndexDataSource
  {
    protected ContactIndexDataSource() { }

    public ContactIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, string table, bool isRebuilding, Logs logs)
      : base(loginUser, maxCount, organizationID, table, isRebuilding, logs)
    {
    }

    protected override void GetNextRecord()
    {
      ContactsViewItem contact = ContactsView.GetContactsViewItem(_loginUser, _itemIDList[_rowIndex]);
      _lastItemID = contact.UserID;
      UpdatedItems.Add((int)_lastItemID);

      List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
      StringBuilder builder = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(contact.Email)) builder.AppendLine(Regex.Replace(contact.Email, "[,.]+", ""));
      PhoneNumbers phoneNumbers = new PhoneNumbers(_loginUser);
      phoneNumbers.LoadByID(contact.UserID, ReferenceType.Users);
      foreach (PhoneNumber number in phoneNumbers)
      {
        phones.Add(new CustomerSearchPhone(number));
        builder.AppendLine(Regex.Replace(number.Number, "[^0-9]", ""));
      }

      Addresses addresses = new Addresses(_loginUser);
      addresses.LoadByID(contact.UserID, ReferenceType.Users);
      foreach (Address address in addresses)
      {
        builder.AppendLine(address.Description
        + " " + address.Addr1
        + " " + address.Addr2
        + " " + address.Addr3
        + " " + address.City
        + " " + address.State
        + " " + address.Zip
        + " " + address.Country);
      }

      EmailAddresses emailAddresses = new EmailAddresses(_loginUser);
      emailAddresses.LoadByRefID(contact.UserID, ReferenceType.Users);
      foreach (EmailAddress emailAddress in emailAddresses)
      {
        builder.AppendLine(Regex.Replace(emailAddress.Email, "[,.]+", ""));
      }

      _docFields.Clear();
      AddDocField("UserID", contact.UserID);
      AddDocField("OrganizationID", contact.OrganizationID);
      AddDocField("Organization", contact.Organization);
      AddDocField("Email", contact.Email);
      AddDocField("IsActive", contact.IsActive);
        
      if (string.IsNullOrWhiteSpace(contact.FirstName))
      {
        AddDocField("Name", contact.LastName);
        DocDisplayName = string.IsNullOrWhiteSpace(contact.FirstName) ? "" : contact.FirstName.Trim();
      }
      else
      {
        AddDocField("Name", contact.FirstName + " " + contact.LastName);
        DocDisplayName = contact.LastName.Trim() + (string.IsNullOrWhiteSpace(contact.FirstName) ? "" : ", " + contact.FirstName.Trim());
      }

      builder.AppendLine(Regex.Replace(DocDisplayName, "[^a-zA-Z0-9 -]", ""));
      builder.AppendLine(Regex.Replace(contact.Organization, "[^a-zA-Z0-9 -]", ""));

      DocText = builder.ToString();

      CustomerSearchContact contactItem = new CustomerSearchContact(contact);
      contactItem.phones = phones.ToArray();
      TicketsView tickets = new TicketsView(_loginUser);
      contactItem.openTicketCount = tickets.GetUserTicketCount(contact.UserID, 0);
        
      AddDocField("**JSON", JsonConvert.SerializeObject(contactItem));

      CustomValues customValues = new CustomValues(_loginUser);
      customValues.LoadByReferenceType(_organizationID, ReferenceType.Contacts, contact.UserID);
        
      foreach (CustomValue value in customValues)
      {
        object o = value.Row["CustomValue"];
        string s = o == null || o == DBNull.Value ? "" : o.ToString();
        AddDocField(value.Row["Name"].ToString(), s);
      }

      DocFields = _docFields.ToString();
      DocIsFile = false;
      DocName = contact.UserID.ToString();
      DocCreatedDate = (DateTime)contact.Row["DateCreated"];
      DocModifiedDate = (DateTime)contact.Row["DateModified"];
    }

    protected override void LoadData()
    {
      ContactsView contacts = new ContactsView(_loginUser);
      contacts.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
      foreach (ContactsViewItem contact in contacts)
      {
        _itemIDList.Add(contact.UserID);
      }
    }
  }



}
