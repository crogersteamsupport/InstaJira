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
  
  public class RestUsersView
  {
    public static string GetUsersViewItem(RestCommand command, int userID)
    {
      UsersViewItem usersViewItem = UsersView.GetUsersViewItem(command.LoginUser, userID);
      if (usersViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return usersViewItem.GetXml("UsersViewItem", true);
    }
    
    public static string GetUsersView(RestCommand command)
    {
      UsersView usersView = new UsersView(command.LoginUser);
      usersView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return usersView.GetXml("UsersView", "UsersViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
