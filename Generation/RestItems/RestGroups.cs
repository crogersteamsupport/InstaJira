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
  
  public class RestGroups
  {
    public static string GetGroup(RestCommand command, int groupID)
    {
      Group group = Groups.GetGroup(command.LoginUser, groupID);
      if (group.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return group.GetXml("Group", true);
    }
    
    public static string GetGroups(RestCommand command)
    {
      Groups groups = new Groups(command.LoginUser);
      groups.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return groups.GetXml("Groups", "Group", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
