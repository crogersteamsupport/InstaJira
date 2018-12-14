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
  
  public class RestReports
  {
    public static string GetReport(RestCommand command, int reportID)
    {
      Report report = Reports.GetReport(command.LoginUser, reportID);
      if (report.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return report.GetXml("Report", true);
    }
    
    public static string GetReports(RestCommand command)
    {
      Reports reports = new Reports(command.LoginUser);
      reports.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reports.GetXml("Reports", "Report", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
