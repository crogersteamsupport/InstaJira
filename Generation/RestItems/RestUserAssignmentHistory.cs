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
  
  public class RestUserAssignmentHistory
  {
    public static string GetUserAssignmentHistoryItem(RestCommand command, int userAssignmentHistoryID)
    {
      UserAssignmentHistoryItem userAssignmentHistoryItem = UserAssignmentHistory.GetUserAssignmentHistoryItem(command.LoginUser, userAssignmentHistoryID);
      if (userAssignmentHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return userAssignmentHistoryItem.GetXml("UserAssignmentHistoryItem", true);
    }
    
    public static string GetUserAssignmentHistory(RestCommand command)
    {
      UserAssignmentHistory userAssignmentHistory = new UserAssignmentHistory(command.LoginUser);
      userAssignmentHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return userAssignmentHistory.GetXml("UserAssignmentHistory", "UserAssignmentHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
