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
  
  public class RestTaskEmailPostHistory
  {
    public static string GetTaskEmailPostHistoryItem(RestCommand command, int taskEmailPostID)
    {
      TaskEmailPostHistoryItem taskEmailPostHistoryItem = TaskEmailPostHistory.GetTaskEmailPostHistoryItem(command.LoginUser, taskEmailPostID);
      if (taskEmailPostHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return taskEmailPostHistoryItem.GetXml("TaskEmailPostHistoryItem", true);
    }
    
    public static string GetTaskEmailPostHistory(RestCommand command)
    {
      TaskEmailPostHistory taskEmailPostHistory = new TaskEmailPostHistory(command.LoginUser);
      taskEmailPostHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return taskEmailPostHistory.GetXml("TaskEmailPostHistory", "TaskEmailPostHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
