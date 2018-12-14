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
  
  public class RestCustomerHubs
  {
    public static string GetCustomerHub(RestCommand command, int customerHubID)
    {
      CustomerHub customerHub = CustomerHubs.GetCustomerHub(command.LoginUser, customerHubID);
      if (customerHub.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customerHub.GetXml("CustomerHub", true);
    }
    
    public static string GetCustomerHubs(RestCommand command)
    {
      CustomerHubs customerHubs = new CustomerHubs(command.LoginUser);
      customerHubs.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customerHubs.GetXml("CustomerHubs", "CustomerHub", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
