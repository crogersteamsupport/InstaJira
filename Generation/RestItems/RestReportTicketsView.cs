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
  
  public class RestReportTicketsView
  {
    public static string GetReportTicketsViewItem(RestCommand command, int ticketID)
    {
      ReportTicketsViewItem reportTicketsViewItem = ReportTicketsView.GetReportTicketsViewItem(command.LoginUser, ticketID);
      if (reportTicketsViewItem.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportTicketsViewItem.GetXml("ReportTicketsViewItem", true);
    }
    
    public static string GetReportTicketsView(RestCommand command)
    {
      ReportTicketsView reportTicketsView = new ReportTicketsView(command.LoginUser);
      reportTicketsView.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportTicketsView.GetXml("ReportTicketsView", "ReportTicketsViewItem", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
