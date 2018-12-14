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
  
  public class RestTasks
  {
    public static string GetTask(RestCommand command, int taskID)
    {
      Task task = Tasks.GetTask(command.LoginUser, taskID);
      if (task.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return task.GetXml("Task", true);
    }
    
    public static string GetTasks(RestCommand command)
    {
      Tasks tasks = new Tasks(command.LoginUser);
      tasks.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return tasks.GetXml("Tasks", "Task", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
