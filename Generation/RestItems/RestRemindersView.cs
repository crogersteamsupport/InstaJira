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
  
  public class RestRemindersView
  {
    public static string GetRemindersViewItem(RestCommand command, int reminderID)
    {
      RemindersViewItem remindersViewItem = RemindersView.GetRemindersViewItem(command.LoginUser, reminderID);
      if (remindersViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return remindersViewItem.GetXml("RemindersViewItem", true);
    }
    
    public static string GetRemindersView(RestCommand command)
    {
      RemindersView remindersView = new RemindersView(command.LoginUser);
      remindersView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return remindersView.GetXml("RemindersView", "RemindersViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
