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
  
  public class RestCustomerRelationships
  {
    public static string GetCustomerRelationship(RestCommand command, int customerRelationshipID)
    {
      CustomerRelationship customerRelationship = CustomerRelationships.GetCustomerRelationship(command.LoginUser, customerRelationshipID);
      if (customerRelationship.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return customerRelationship.GetXml("CustomerRelationship", true);
    }
    
    public static string GetCustomerRelationships(RestCommand command)
    {
      CustomerRelationships customerRelationships = new CustomerRelationships(command.LoginUser);
      customerRelationships.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return customerRelationships.GetXml("CustomerRelationships", "CustomerRelationship", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
