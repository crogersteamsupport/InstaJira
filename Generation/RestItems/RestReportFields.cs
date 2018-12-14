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
  
  public class RestReportFields
  {
    public static string GetReportField(RestCommand command, int reportFieldID)
    {
      ReportField reportField = ReportFields.GetReportField(command.LoginUser, reportFieldID);
      if (reportField.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportField.GetXml("ReportField", true);
    }
    
    public static string GetReportFields(RestCommand command)
    {
      ReportFields reportFields = new ReportFields(command.LoginUser);
      reportFields.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportFields.GetXml("ReportFields", "ReportField", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
