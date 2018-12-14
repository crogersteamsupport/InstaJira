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
  
  public class RestEmailTemplates
  {
    public static string GetEmailTemplate(RestCommand command, int emailTemplateID)
    {
      EmailTemplate emailTemplate = EmailTemplates.GetEmailTemplate(command.LoginUser, emailTemplateID);
      if (emailTemplate.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return emailTemplate.GetXml("EmailTemplate", true);
    }
    
    public static string GetEmailTemplates(RestCommand command)
    {
      EmailTemplates emailTemplates = new EmailTemplates(command.LoginUser);
      emailTemplates.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return emailTemplates.GetXml("EmailTemplates", "EmailTemplate", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
