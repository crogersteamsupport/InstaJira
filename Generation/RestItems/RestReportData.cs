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
  
  public class RestReportData
  {
    public static string GetReportDataItem(RestCommand command, int reportDataID)
    {
      ReportDataItem reportDataItem = ReportData.GetReportDataItem(command.LoginUser, reportDataID);
      if (reportDataItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportDataItem.GetXml("ReportDataItem", true);
    }
    
    public static string GetReportData(RestCommand command)
    {
      ReportData reportData = new ReportData(command.LoginUser);
      reportData.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportData.GetXml("ReportData", "ReportDataItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
