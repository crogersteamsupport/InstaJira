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
  
  public class RestPhoneOptions
  {
    public static string GetPhoneOption(RestCommand command, int phoneOptionID)
    {
      PhoneOption phoneOption = PhoneOptions.GetPhoneOption(command.LoginUser, phoneOptionID);
      if (phoneOption.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return phoneOption.GetXml("PhoneOption", true);
    }
    
    public static string GetPhoneOptions(RestCommand command)
    {
      PhoneOptions phoneOptions = new PhoneOptions(command.LoginUser);
      phoneOptions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return phoneOptions.GetXml("PhoneOptions", "PhoneOption", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
