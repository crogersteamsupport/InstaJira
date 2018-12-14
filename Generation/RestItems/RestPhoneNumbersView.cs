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
  
  public class RestPhoneNumbersView
  {
    public static string GetPhoneNumbersViewItem(RestCommand command, int phoneID)
    {
      PhoneNumbersViewItem phoneNumbersViewItem = PhoneNumbersView.GetPhoneNumbersViewItem(command.LoginUser, phoneID);
      if (phoneNumbersViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return phoneNumbersViewItem.GetXml("PhoneNumbersViewItem", true);
    }
    
    public static string GetPhoneNumbersView(RestCommand command)
    {
      PhoneNumbersView phoneNumbersView = new PhoneNumbersView(command.LoginUser);
      phoneNumbersView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return phoneNumbersView.GetXml("PhoneNumbersView", "PhoneNumbersViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
