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
  
  public class RestActions
  {
    public static string GetAction(RestCommand command, int actionID)
    {
      Action action = Actions.GetAction(command.LoginUser, actionID);
      if (action.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return action.GetXml("Action", true);
    }
    
    public static string GetActions(RestCommand command)
    {
      Actions actions = new Actions(command.LoginUser);
      actions.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return actions.GetXml("Actions", "Action", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
