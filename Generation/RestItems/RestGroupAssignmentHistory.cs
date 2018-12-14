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
  
  public class RestGroupAssignmentHistory
  {
    public static string GetGroupAssignmentHistoryItem(RestCommand command, int groupAssignmentHistoryID)
    {
      GroupAssignmentHistoryItem groupAssignmentHistoryItem = GroupAssignmentHistory.GetGroupAssignmentHistoryItem(command.LoginUser, groupAssignmentHistoryID);
      if (groupAssignmentHistoryItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return groupAssignmentHistoryItem.GetXml("GroupAssignmentHistoryItem", true);
    }
    
    public static string GetGroupAssignmentHistory(RestCommand command)
    {
      GroupAssignmentHistory groupAssignmentHistory = new GroupAssignmentHistory(command.LoginUser);
      groupAssignmentHistory.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return groupAssignmentHistory.GetXml("GroupAssignmentHistory", "GroupAssignmentHistoryItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
