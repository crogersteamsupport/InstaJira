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
  
  public class RestActionLogsView
  {
    public static string GetActionLogsViewItem(RestCommand command, int actionLogID)
    {
      ActionLogsViewItem actionLogsViewItem = ActionLogsView.GetActionLogsViewItem(command.LoginUser, actionLogID);
      if (actionLogsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return actionLogsViewItem.GetXml("ActionLogsViewItem", true);
    }
    
    public static string GetActionLogsView(RestCommand command)
    {
      ActionLogsView actionLogsView = new ActionLogsView(command.LoginUser);
      actionLogsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return actionLogsView.GetXml("ActionLogsView", "ActionLogsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
