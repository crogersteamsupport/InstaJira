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
  
  public class RestAddresses
  {
    public static string GetAddress(RestCommand command, int addressID)
    {
      Address address = Addresses.GetAddress(command.LoginUser, addressID);
      if (address.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return address.GetXml("Address", true);
    }
    
    public static string GetAddresses(RestCommand command)
    {
      Addresses addresses = new Addresses(command.LoginUser);
      addresses.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return addresses.GetXml("Addresses", "Address", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
