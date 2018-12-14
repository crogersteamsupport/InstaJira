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
  
  public class RestEmailTemplateTables
  {
    public static string GetEmailTemplateTable(RestCommand command, int emailTemplateTableID)
    {
      EmailTemplateTable emailTemplateTable = EmailTemplateTables.GetEmailTemplateTable(command.LoginUser, emailTemplateTableID);
      if (emailTemplateTable.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return emailTemplateTable.GetXml("EmailTemplateTable", true);
    }
    
    public static string GetEmailTemplateTables(RestCommand command)
    {
      EmailTemplateTables emailTemplateTables = new EmailTemplateTables(command.LoginUser);
      emailTemplateTables.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return emailTemplateTables.GetXml("EmailTemplateTables", "EmailTemplateTable", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
