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
  
  public class RestReportTables
  {
    public static string GetReportTable(RestCommand command, int reportTableID)
    {
      ReportTable reportTable = ReportTables.GetReportTable(command.LoginUser, reportTableID);
      if (reportTable.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportTable.GetXml("ReportTable", true);
    }
    
    public static string GetReportTables(RestCommand command)
    {
      ReportTables reportTables = new ReportTables(command.LoginUser);
      reportTables.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportTables.GetXml("ReportTables", "ReportTable", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
