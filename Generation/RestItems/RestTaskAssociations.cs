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
  
  public class RestTaskAssociations
  {
    public static string GetTaskAssociation(RestCommand command, int reminderID)
    {
      TaskAssociation taskAssociation = TaskAssociations.GetTaskAssociation(command.LoginUser, reminderID);
      if (taskAssociation.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return taskAssociation.GetXml("TaskAssociation", true);
    }
    
    public static string GetTaskAssociations(RestCommand command)
    {
      TaskAssociations taskAssociations = new TaskAssociations(command.LoginUser);
      taskAssociations.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return taskAssociations.GetXml("TaskAssociations", "TaskAssociation", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
