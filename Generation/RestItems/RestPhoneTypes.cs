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
  
  public class RestPhoneTypes
  {
    public static string GetPhoneType(RestCommand command, int phoneTypeID)
    {
      PhoneType phoneType = PhoneTypes.GetPhoneType(command.LoginUser, phoneTypeID);
      if (phoneType.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return phoneType.GetXml("PhoneType", true);
    }
    
    public static string GetPhoneTypes(RestCommand command)
    {
      PhoneTypes phoneTypes = new PhoneTypes(command.LoginUser);
      phoneTypes.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return phoneTypes.GetXml("PhoneTypes", "PhoneType", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
