﻿using System;
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

    public CustomerIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding, string logName)
      : base(loginUser, maxCount, organizationID, isRebuilding, logName)
    {
    }

    override public bool GetNextDoc()
    {
      base.GetNextDoc();

      try
      {
        if (_itemIDList == null) { Rewind(); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        OrganizationsViewItem organization = OrganizationsView.GetOrganizationsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing OrganizationID: " + organization.OrganizationID.ToString());

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

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "CustomerIndexDataSource");
        //Logs.WriteException(ex);
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound customers, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        OrganizationsView organizations = new OrganizationsView(_loginUser);
        organizations.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (OrganizationsViewItem organization in organizations)
        {
          _itemIDList.Add(organization.OrganizationID);
        }
        _lastItemID = null;
        _rowIndex = -1;
        //Logs.WriteEvent("Tickets Source Rewound");
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "CustomerIndexDataSource Rewind");
        //Logs.WriteException(ex);
        throw;
      }
      return true;
    }
  }



}
