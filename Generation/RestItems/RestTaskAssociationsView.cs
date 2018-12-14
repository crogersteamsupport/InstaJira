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
  
  public class RestTaskAssociationsView
  {
    public static string GetTaskAssociationsViewItem(RestCommand command, int reminderID)
    {
      TaskAssociationsViewItem taskAssociationsViewItem = TaskAssociationsView.GetTaskAssociationsViewItem(command.LoginUser, reminderID);
      if (taskAssociationsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return taskAssociationsViewItem.GetXml("TaskAssociationsViewItem", true);
    }
    
    public static string GetTaskAssociationsView(RestCommand command)
    {
      TaskAssociationsView taskAssociationsView = new TaskAssociationsView(command.LoginUser);
      taskAssociationsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return taskAssociationsView.GetXml("TaskAssociationsView", "TaskAssociationsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
