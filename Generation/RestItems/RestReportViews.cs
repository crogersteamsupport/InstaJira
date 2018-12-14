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
  
  public class RestReportViews
  {
    public static string GetReportView(RestCommand command, int reportViewID)
    {
      ReportView reportView = ReportViews.GetReportView(command.LoginUser, reportViewID);
      if (reportView.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportView.GetXml("ReportView", true);
    }
    
    public static string GetReportViews(RestCommand command)
    {
      ReportViews reportViews = new ReportViews(command.LoginUser);
      reportViews.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportViews.GetXml("ReportViews", "ReportView", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
