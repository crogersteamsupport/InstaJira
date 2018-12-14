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
  
  public class RestReportFolders
  {
    public static string GetReportFolder(RestCommand command, int folderID)
    {
      ReportFolder reportFolder = ReportFolders.GetReportFolder(command.LoginUser, folderID);
      if (reportFolder.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportFolder.GetXml("ReportFolder", true);
    }
    
    public static string GetReportFolders(RestCommand command)
    {
      ReportFolders reportFolders = new ReportFolders(command.LoginUser);
      reportFolders.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportFolders.GetXml("ReportFolders", "ReportFolder", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
