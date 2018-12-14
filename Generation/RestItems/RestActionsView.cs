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
  
  public class RestActionsView
  {
    public static string GetActionsViewItem(RestCommand command, int actionID)
    {
      ActionsViewItem actionsViewItem = ActionsView.GetActionsViewItem(command.LoginUser, actionID);
      if (actionsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return actionsViewItem.GetXml("ActionsViewItem", true);
    }
    
    public static string GetActionsView(RestCommand command)
    {
      ActionsView actionsView = new ActionsView(command.LoginUser);
      actionsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return actionsView.GetXml("ActionsView", "ActionsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
