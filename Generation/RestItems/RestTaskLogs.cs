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
  
  public class RestTaskLogs
  {
    public static string GetTaskLog(RestCommand command, int taskLogID)
    {
      TaskLog taskLog = TaskLogs.GetTaskLog(command.LoginUser, taskLogID);
      if (taskLog.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return taskLog.GetXml("TaskLog", true);
    }
    
    public static string GetTaskLogs(RestCommand command)
    {
      TaskLogs taskLogs = new TaskLogs(command.LoginUser);
      taskLogs.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return taskLogs.GetXml("TaskLogs", "TaskLog", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
