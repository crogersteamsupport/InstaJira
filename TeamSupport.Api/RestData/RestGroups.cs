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
      Group item = Groups.GetGroup(command.LoginUser, groupID);
      if (item.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);

      return item.GetXml("Group", true);
    }

    public static string GetGroups(RestCommand command, bool orderByDateCreated = false)
    {
      Groups items = new Groups(command.LoginUser);
      if (orderByDateCreated)
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID, "DateCreated DESC");      
      }
      else
      {
        items.LoadByOrganizationID(command.Organization.OrganizationID);
      }
      return items.GetXml("Groups", "Group", true, command.Filters);
    }

  }
}
