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
  
  public class RestEmails
  {
    public static string GetEmail(RestCommand command, int emailID)
    {
      Email email = Emails.GetEmail(command.LoginUser, emailID);
      if (email.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return email.GetXml("Email", true);
    }
    
    public static string GetEmails(RestCommand command)
    {
      Emails emails = new Emails(command.LoginUser);
      emails.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return emails.GetXml("Emails", "Email", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
