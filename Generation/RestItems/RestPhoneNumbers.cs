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
  
  public class RestPhoneNumbers
  {
    public static string GetPhoneNumber(RestCommand command, int phoneID)
    {
      PhoneNumber phoneNumber = PhoneNumbers.GetPhoneNumber(command.LoginUser, phoneID);
      if (phoneNumber.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return phoneNumber.GetXml("PhoneNumber", true);
    }
    
    public static string GetPhoneNumbers(RestCommand command)
    {
      PhoneNumbers phoneNumbers = new PhoneNumbers(command.LoginUser);
      phoneNumbers.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return phoneNumbers.GetXml("PhoneNumbers", "PhoneNumber", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
