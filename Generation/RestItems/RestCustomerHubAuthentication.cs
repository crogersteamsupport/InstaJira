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
  
  public class RestCustomerHubAuthentication
  {
    public static string GetCustomerHubAuthenticationItem(RestCommand command, int customerHubAuthenticationID)
    {
      CustomerHubAuthenticationItem customerHubAuthenticationItem = CustomerHubAuthentication.GetCustomerHubAuthenticationItem(command.LoginUser, customerHubAuthenticationID);
      if (customerHubAuthenticationItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customerHubAuthenticationItem.GetXml("CustomerHubAuthenticationItem", true);
    }
    
    public static string GetCustomerHubAuthentication(RestCommand command)
    {
      CustomerHubAuthentication customerHubAuthentication = new CustomerHubAuthentication(command.LoginUser);
      customerHubAuthentication.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customerHubAuthentication.GetXml("CustomerHubAuthentication", "CustomerHubAuthenticationItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
