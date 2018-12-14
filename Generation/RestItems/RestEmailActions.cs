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
  
  public class RestEmailActions
  {
    public static string GetEmailAction(RestCommand command, int eMailActionID)
    {
      EmailAction emailAction = EmailActions.GetEmailAction(command.LoginUser, eMailActionID);
      if (emailAction.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return emailAction.GetXml("EmailAction", true);
    }
    
    public static string GetEmailActions(RestCommand command)
    {
      EmailActions emailActions = new EmailActions(command.LoginUser);
      emailActions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return emailActions.GetXml("EmailActions", "EmailAction", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
