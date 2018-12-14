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
  
  public class RestReminders
  {
    public static string GetReminder(RestCommand command, int reminderID)
    {
      Reminder reminder = Reminders.GetReminder(command.LoginUser, reminderID);
      if (reminder.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reminder.GetXml("Reminder", true);
    }
    
    public static string GetReminders(RestCommand command)
    {
      Reminders reminders = new Reminders(command.LoginUser);
      reminders.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reminders.GetXml("Reminders", "Reminder", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
