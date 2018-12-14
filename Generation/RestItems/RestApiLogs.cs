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
  
  public class RestApiLogs
  {
    public static string GetApiLog(RestCommand command, int apiLogID)
    {
      ApiLog apiLog = ApiLogs.GetApiLog(command.LoginUser, apiLogID);
      if (apiLog.OrganizationID != command.Organization.OrganizationID) throw new RestException(HttpStatusCode.Unauthorized);
      return apiLog.GetXml("ApiLog", true);
    }
    
    public static string GetApiLogs(RestCommand command)
    {
      ApiLogs apiLogs = new ApiLogs(command.LoginUser);
      apiLogs.LoadByOrganizationID(command.Organization.OrganizationID);

      if (command.Format == RestFormat.XML)
      {
        return apiLogs.GetXml("ApiLogs", "ApiLog", true, command.Filters);
      }
      else
      {
        throw new RestException(HttpStatusCode.BadRequest, "Invalid data format");
      }
      
    }    
  }
  
}





  
