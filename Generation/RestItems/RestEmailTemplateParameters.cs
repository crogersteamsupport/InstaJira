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
  
  public class RestEmailTemplateParameters
  {
    public static string GetEmailTemplateParameter(RestCommand command, int emailTemplateParameterID)
    {
      EmailTemplateParameter emailTemplateParameter = EmailTemplateParameters.GetEmailTemplateParameter(command.LoginUser, emailTemplateParameterID);
      if (emailTemplateParameter.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return emailTemplateParameter.GetXml("EmailTemplateParameter", true);
    }
    
    public static string GetEmailTemplateParameters(RestCommand command)
    {
      EmailTemplateParameters emailTemplateParameters = new EmailTemplateParameters(command.LoginUser);
      emailTemplateParameters.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return emailTemplateParameters.GetXml("EmailTemplateParameters", "EmailTemplateParameter", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
