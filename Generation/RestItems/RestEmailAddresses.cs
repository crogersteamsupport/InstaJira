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
  
  public class RestEmailAddresses
  {
    public static string GetEmailAddress(RestCommand command, int id)
    {
      EmailAddress emailAddress = EmailAddresses.GetEmailAddress(command.LoginUser, id);
      if (emailAddress.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return emailAddress.GetXml("EmailAddress", true);
    }
    
    public static string GetEmailAddresses(RestCommand command)
    {
      EmailAddresses emailAddresses = new EmailAddresses(command.LoginUser);
      emailAddresses.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return emailAddresses.GetXml("EmailAddresses", "EmailAddress", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
