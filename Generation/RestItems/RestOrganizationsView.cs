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
  
  public class RestOrganizationsView
  {
    public static string GetOrganizationsViewItem(RestCommand command, int organizationID)
    {
      OrganizationsViewItem organizationsViewItem = OrganizationsView.GetOrganizationsViewItem(command.LoginUser, organizationID);
      if (organizationsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return organizationsViewItem.GetXml("OrganizationsViewItem", true);
    }
    
    public static string GetOrganizationsView(RestCommand command)
    {
      OrganizationsView organizationsView = new OrganizationsView(command.LoginUser);
      organizationsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return organizationsView.GetXml("OrganizationsView", "OrganizationsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
