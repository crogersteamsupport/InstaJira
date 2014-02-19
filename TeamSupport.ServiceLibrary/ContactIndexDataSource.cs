﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  [Serializable]
  class ContactIndexDataSource : IndexDataSource
  {
    protected ContactIndexDataSource() { }

    public ContactIndexDataSource(LoginUser loginUser, int maxCount, int organizationID, bool isRebuilding)
      : base(loginUser, maxCount, organizationID, isRebuilding)
    {
      _logs = new Logs("Contact Indexer DataSource");
    }

    override public bool GetNextDoc()
    {
      try
      {
        if (_itemIDList == null) { Rewind(); }
        if (_lastItemID != null) { UpdatedItems.Add((int)_lastItemID); }
        _rowIndex++;
        if (_itemIDList.Count <= _rowIndex) { return false; }

        ContactsViewItem contact = ContactsView.GetContactsViewItem(_loginUser, _itemIDList[_rowIndex]);
        _logs.WriteEvent("Started Processing UserID: " + contact.UserID.ToString());

        _lastItemID = contact.UserID;

        StringBuilder builder = new StringBuilder();
        PhoneNumbers phoneNumbers = new PhoneNumbers(_loginUser);
        phoneNumbers.LoadByID(contact.UserID, ReferenceType.Contacts);
        foreach (PhoneNumber number in phoneNumbers)
        {
          builder.AppendLine(Regex.Replace(number.Number, "[^0-9]", ""));
        }

        Addresses addresses = new Addresses(_loginUser);
        addresses.LoadByID(contact.UserID, ReferenceType.Contacts);
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

        DocText = string.Format("<html>{1} {0}</html>", "CUSTOM FIELDS", builder.ToString());

        List<string> columnsToIndex = new List<string>();
        columnsToIndex.Add("UserID");
        columnsToIndex.Add("Email");
        columnsToIndex.Add("Name");
        columnsToIndex.Add("FirstName");
        columnsToIndex.Add("MiddleName");
        columnsToIndex.Add("LastName");
        columnsToIndex.Add("OrganizationName");

        DocFields = string.Empty;
        foreach (DataColumn column in contact.Collection.Table.Columns)
        {
          if (columnsToIndex.Contains(column.ColumnName))
          {
            object value = contact.Row[column];
            string s = value == null || value == DBNull.Value ? "" : value.ToString();
            DocFields += column.ColumnName + "\t" + s.Replace("\t", " ") + "\t";
          }
        }

        CustomValues customValues = new CustomValues(_loginUser);
        customValues.LoadByReferenceType(_organizationID, ReferenceType.Contacts, contact.UserID);
        
        foreach (CustomValue value in customValues)
        {
          object o = value.Row["CustomValue"];
          string s = o == null || o == DBNull.Value ? "" : o.ToString();
          DocFields += value.Row["Name"].ToString() + "\t" + s.Replace("\t", " ") + "\t";
        }

        DocIsFile = false;
        DocName = contact.UserID.ToString();
        if (string.IsNullOrWhiteSpace(contact.LastName))
        {
          DocFields += "SortName\t" + (string.IsNullOrWhiteSpace(contact.FirstName) ? "" : contact.FirstName.Trim()) + "\t";
          DocDisplayName = string.IsNullOrWhiteSpace(contact.FirstName) ? "" : contact.FirstName.Trim();
        }
        else
        {
          DocFields += "SortName\t" + (string.IsNullOrWhiteSpace(contact.FirstName) ? "" : contact.FirstName.Trim()) + "\t";
          DocDisplayName = contact.LastName.Trim() + (string.IsNullOrWhiteSpace(contact.FirstName) ? "" : ", " + contact.FirstName.Trim());
        }
        DocCreatedDate = (DateTime)contact.Row["DateCreated"];
        DocModifiedDate = (DateTime)contact.Row["DateModified"];

        return true;
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ContactIndexDataSource");
        //Logs.WriteException(ex);
        throw;
      }
    }

    override public bool Rewind()
    {
      try
      {
        _logs.WriteEvent("Rewound users, OrgID: " + _organizationID.ToString());
        _itemIDList = new List<int>();
        ContactsView contacts = new ContactsView(_loginUser);
        contacts.LoadForIndexing(_organizationID, _maxCount, _isRebuilding);
        foreach (ContactsViewItem contact in contacts)
        {
          _itemIDList.Add(contact.UserID);
        }
        _lastItemID = null;
        _rowIndex = -1;
        //Logs.WriteEvent("Tickets Source Rewound");
      }
      catch (Exception ex)
      {
        ExceptionLogs.LogException(_loginUser, ex, "ContactIndexDataSource Rewind");
        //Logs.WriteException(ex);
        throw;
      }
      return true;
    }
  }
}
