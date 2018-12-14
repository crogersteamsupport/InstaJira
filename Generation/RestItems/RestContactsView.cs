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
  
  public class RestContactsView
  {
    public static string GetContactsViewItem(RestCommand command, int userID)
    {
      ContactsViewItem contactsViewItem = ContactsView.GetContactsViewItem(command.LoginUser, userID);
      if (contactsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return contactsViewItem.GetXml("ContactsViewItem", true);
    }
    
    public static string GetContactsView(RestCommand command)
    {
      ContactsView contactsView = new ContactsView(command.LoginUser);
      contactsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return contactsView.GetXml("ContactsView", "ContactsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
