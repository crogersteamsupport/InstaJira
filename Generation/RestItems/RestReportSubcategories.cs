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
  
  public class RestReportSubcategories
  {
    public static string GetReportSubcategory(RestCommand command, int reportSubcategoryID)
    {
      ReportSubcategory reportSubcategory = ReportSubcategories.GetReportSubcategory(command.LoginUser, reportSubcategoryID);
      if (reportSubcategory.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return reportSubcategory.GetXml("ReportSubcategory", true);
    }
    
    public static string GetReportSubcategories(RestCommand command)
    {
      ReportSubcategories reportSubcategories = new ReportSubcategories(command.LoginUser);
      reportSubcategories.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return reportSubcategories.GetXml("ReportSubcategories", "ReportSubcategory", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
