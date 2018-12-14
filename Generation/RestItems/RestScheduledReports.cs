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
  
  public class RestScheduledReports
  {
    public static string GetScheduledReport(RestCommand command, int id)
    {
      ScheduledReport scheduledReport = ScheduledReports.GetScheduledReport(command.LoginUser, id);
      if (scheduledReport.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return scheduledReport.GetXml("ScheduledReport", true);
    }
    
    public static string GetScheduledReports(RestCommand command)
    {
      ScheduledReports scheduledReports = new ScheduledReports(command.LoginUser);
      scheduledReports.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return scheduledReports.GetXml("ScheduledReports", "ScheduledReport", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
