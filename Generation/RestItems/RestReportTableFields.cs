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
  
  public class RestReportTableFields
  {
    public static string GetReportTableField(RestCommand command, int reportTableFieldID)
    {
      ReportTableField reportTableField = ReportTableFields.GetReportTableField(command.LoginUser, reportTableFieldID);
      if (reportTableField.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportTableField.GetXml("ReportTableField", true);
    }
    
    public static string GetReportTableFields(RestCommand command)
    {
      ReportTableFields reportTableFields = new ReportTableFields(command.LoginUser);
      reportTableFields.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportTableFields.GetXml("ReportTableFields", "ReportTableField", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
