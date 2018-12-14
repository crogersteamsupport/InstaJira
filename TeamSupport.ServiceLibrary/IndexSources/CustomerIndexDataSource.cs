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
  class CustomerIndexDataSource : IndexDataSource
  {
    protected CustomerIndexDataSource() { }

    public CustomerIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, string table, bool isRebuilding, Logs logs)
      : base(loginUser, maxCount, organizationID, table, isRebuilding, logs)
    {
    }

    override protected void GetNextRecord()
    {
      OrganizationsViewItem organization = OrganizationsView.GetOrganizationsViewItem(_loginUser, _itemIDList[_rowIndex]);
      _lastItemID = organization.OrganizationID;
      UpdatedItems.Add((int)_lastItemID);

      StringBuilder builder = new StringBuilder();
      List<CustomerSearchPhone> phones = new List<CustomerSearchPhone>();
      PhoneNumbers phoneNumbers = new PhoneNumbers(_loginUser);
      phoneNumbers.LoadByID(organization.OrganizationID, ReferenceType.Organizations);
      foreach (PhoneNumber number in phoneNumbers)
      {
        phones.Add(new CustomerSearchPhone(number));
        builder.AppendLine(Regex.Replace(number.Number, "[^0-9]", ""));
      }

      Addresses addresses = new Addresses(_loginUser);
      addresses.LoadByID(organization.OrganizationID, ReferenceType.Organizations);
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

      builder.AppendLine(Regex.Replace(organization.Name, "[^a-zA-Z0-9 -]", ""));
        
      DocText = builder.ToString();
      _docFields.Clear();
      AddDocField("OrganizationID", organization.OrganizationID);
      AddDocField("Name", organization.Name);
      AddDocField("Description", organization.Description);
      AddDocField("Website", organization.Website);
      AddDocField("IsActive", organization.IsActive);
      AddDocField("PrimaryContact", organization.PrimaryContact);
      AddDocField("IsParent", Organizations.GetIsParent(_loginUser, organization.OrganizationID));

      CustomerSearchCompany companyItem = new CustomerSearchCompany(organization);
      companyItem.phones = phones.ToArray();
      TicketsView tickets = new TicketsView(_loginUser);
      companyItem.openTicketCount = tickets.GetOrganizationTicketCount(organization.OrganizationID, 0);

      AddDocField("**JSON", JsonConvert.SerializeObject(companyItem));

      CustomValues customValues = new CustomValues(_loginUser);
      customValues.LoadByReferenceType(_organizationID, ReferenceType.Organizations, organization.OrganizationID);

      foreach (CustomValue value in customValues)
      {
        object o = value.Row["CustomValue"];
        string s = o == null || o == DBNull.Value ? "" : o.ToString();
        AddDocField(value.Row["Name"].ToString(), s);
      }

      DocFields = _docFields.ToString();
      DocIsFile = false;
      DocName = organization.OrganizationID.ToString();
      DocDisplayName = string.IsNullOrWhiteSpace(organization.Name) ? "" : organization.Name.Trim();
      DocCreatedDate = (DateTime)organization.Row["DateCreated"];
      DocModifiedDate = (DateTime)organization.Row["DateModified"];
    }

    override protected void LoadData()
    {
      OrganizationsView organizations = new OrganizationsView(_loginUser);
      organizations.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
      foreach (OrganizationsViewItem organization in organizations)
      {
        _itemIDList.Add(organization.OrganizationID);
      }
    }
  }



}
