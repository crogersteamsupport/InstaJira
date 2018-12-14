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
  
  public class RestTasksView
  {
    public static string GetTasksViewItem(RestCommand command, int )
    {
      TasksViewItem tasksViewItem = TasksView.GetTasksViewItem(command.LoginUser, );
      if (tasksViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return tasksViewItem.GetXml("TasksViewItem", true);
    }
    
    public static string GetTasksView(RestCommand command)
    {
      TasksView tasksView = new TasksView(command.LoginUser);
      tasksView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return tasksView.GetXml("TasksView", "TasksViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
