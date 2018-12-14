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
  
  public class RestActionLogs
  {
    public static string GetActionLog(RestCommand command, int actionLogID)
    {
      ActionLog actionLog = ActionLogs.GetActionLog(command.LoginUser, actionLogID);
      if (actionLog.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return actionLog.GetXml("ActionLog", true);
    }
    
    public static string GetActionLogs(RestCommand command)
    {
      ActionLogs actionLogs = new ActionLogs(command.LoginUser);
      actionLogs.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return actionLogs.GetXml("ActionLogs", "ActionLog", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
